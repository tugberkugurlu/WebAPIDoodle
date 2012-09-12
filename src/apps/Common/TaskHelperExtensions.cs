using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace System.Threading.Tasks {

    /// <summary>
    /// Proper extensions for Task class which makes it easy to use with it (mostly taken from ASP.NET Web Stack source: http://aspnetwebstack.codeplex.com)
    /// </summary>
    internal static class TaskHelperExtensions {

        private static Task<AsyncVoid> _defaultCompleted = TaskHelpers.FromResult<AsyncVoid>(default(AsyncVoid));

        /// <summary>
        /// Calls the given continuation, after the given task completes, if it ends in a faulted state.
        /// Will not be called if the task did not fault (meaning, it will not be called if the task ran
        /// to completion or was canceled). Intended to roughly emulate C# 5's support for "try/catch" in
        /// async methods. Note that this method allows you to return a Task, so that you can either return
        /// a completed Task (indicating that you swallowed the exception) or a faulted task (indicating that
        /// that the exception should be propagated). In C#, you cannot normally use await within a catch
        /// block, so returning a real async task should never be done from Catch().
        /// </summary>
        internal static Task Catch(this Task task, Func<CatchInfo, CatchInfo.CatchResult> continuation, CancellationToken cancellationToken = default(CancellationToken)) {

            // Fast path for successful tasks, to prevent an extra TCS allocation
            if (task.Status == TaskStatus.RanToCompletion) {

                return task;
            }

            return task.CatchImpl(() =>
                continuation(new CatchInfo(task)).Task.ToTask<AsyncVoid>(), cancellationToken);
        }

        /// <summary>
        /// Calls the given continuation, after the given task completes, if it ends in a faulted state.
        /// Will not be called if the task did not fault (meaning, it will not be called if the task ran
        /// to completion or was canceled). Intended to roughly emulate C# 5's support for "try/catch" in
        /// async methods. Note that this method allows you to return a Task, so that you can either return
        /// a completed Task (indicating that you swallowed the exception) or a faulted task (indicating that
        /// that the exception should be propagated). In C#, you cannot normally use await within a catch
        /// block, so returning a real async task should never be done from Catch().
        /// </summary>
        internal static Task<TResult> Catch<TResult>(this Task<TResult> task, Func<CatchInfo<TResult>, CatchInfo<TResult>.CatchResult> continuation, CancellationToken cancellationToken = default(CancellationToken)) {

            // Fast path for successful tasks, to prevent an extra TCS allocation
            if (task.Status == TaskStatus.RanToCompletion) {

                return task;
            }

            return task.CatchImpl(() => continuation(new CatchInfo<TResult>(task)).Task, cancellationToken);
        }

        private static Task<TResult> CatchImpl<TResult>(this Task task, Func<Task<TResult>> continuation, CancellationToken cancellationToken) {

            // Stay on the same thread if we can
            if (task.IsCompleted) {

                if (task.IsFaulted) {

                    try {

                        Task<TResult> resultTask = continuation();
                        if (resultTask == null) {

                            // Not a resource because this is an internal class, and this is a guard clause that's intended
                            // to be thrown by us to us, never escaping out to end users.
                            throw new InvalidOperationException("You must set the Task property of the CatchInfo returned from the TaskHelpersExtensions.Catch continuation.");
                        }

                        return resultTask;
                    }
                    catch (Exception ex) {

                        return TaskHelpers.FromError<TResult>(ex);
                    }
                }
                if (task.IsCanceled || cancellationToken.IsCancellationRequested) {

                    return TaskHelpers.Canceled<TResult>();
                }

                if (task.Status == TaskStatus.RanToCompletion) {

                    TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
                    tcs.TrySetFromTask(task);
                    return tcs.Task;
                }
            }

            // Split into a continuation method so that we don't create a closure unnecessarily
            return CatchImplContinuation(task, continuation);
        }

        private static Task<TResult> CatchImplContinuation<TResult>(Task task, Func<Task<TResult>> continuation) {

            SynchronizationContext syncContext = SynchronizationContext.Current;

            TaskCompletionSource<Task<TResult>> tcs = new TaskCompletionSource<Task<TResult>>();

            // this runs only if the inner task did not fault
            task.ContinueWith(innerTask => tcs.TrySetFromTask(innerTask), TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

            // this runs only if the inner task faulted
            task.ContinueWith(innerTask => {

                if (syncContext != null) {

                    syncContext.Post(state => {

                        try {

                            Task<TResult> resultTask = continuation();
                            if (resultTask == null) {

                                throw new InvalidOperationException("You cannot return null from the TaskHelpersExtensions.Catch continuation. You must return a valid task or throw an exception.");
                            }

                            tcs.TrySetResult(resultTask);
                        }
                        catch (Exception ex) {

                            tcs.TrySetException(ex);
                        }

                    }, state: null);
                }
                else {

                    try {

                        Task<TResult> resultTask = continuation();
                        if (resultTask == null) {

                            throw new InvalidOperationException("You cannot return null from the TaskHelpersExtensions.Catch continuation. You must return a valid task or throw an exception.");
                        }

                        tcs.TrySetResult(resultTask);
                    }
                    catch (Exception ex) {

                        tcs.TrySetException(ex);
                    }
                }

            }, TaskContinuationOptions.OnlyOnFaulted);

            return tcs.Task.FastUnwrap();
        }

        /// <summary>
        /// A version of task.Unwrap that is optimized to prevent unnecessarily capturing the
        /// execution context when the antecedent task is already completed.
        /// </summary>
        internal static Task FastUnwrap(this Task<Task> task) {

            Task innerTask = task.Status == TaskStatus.RanToCompletion ? task.Result : null;
            return innerTask ?? task.Unwrap();
        }

        /// <summary>
        /// A version of task.Unwrap that is optimized to prevent unnecessarily capturing the
        /// execution context when the antecedent task is already completed.
        /// </summary>
        internal static Task<TResult> FastUnwrap<TResult>(this Task<Task<TResult>> task) {

            Task<TResult> innerTask = task.Status == TaskStatus.RanToCompletion ? task.Result : null;
            return innerTask ?? task.Unwrap();
        }

        /// <summary>
        /// Calls the given continuation, after the given task has completed, if the task successfully ran
        /// to completion (i.e., was not cancelled and did not fault).
        /// </summary>
        internal static Task Then(this Task task, Action continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {

            return task.ThenImpl(t => ToAsyncVoidTask(continuation), cancellationToken, runSynchronously);
        }

        /// <summary>
        /// Calls the given continuation, after the given task has completed, if the task successfully ran
        /// to completion (i.e., was not cancelled and did not fault).
        /// </summary>
        internal static Task<TOuterResult> Then<TOuterResult>(this Task task, Func<TOuterResult> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {

            return task.ThenImpl(t => TaskHelpers.FromResult(continuation()), cancellationToken, runSynchronously);
        }

        /// <summary>
        /// Calls the given continuation, after the given task has completed, if the task successfully ran
        /// to completion (i.e., was not cancelled and did not fault).
        /// </summary>
        internal static Task Then(this Task task, Func<Task> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {

            return task.Then(() => continuation().Then(() => default(AsyncVoid)),
                             cancellationToken, runSynchronously);
        }

        /// <summary>
        /// Calls the given continuation, after the given task has completed, if the task successfully ran
        /// to completion (i.e., was not cancelled and did not fault).
        /// </summary>
        internal static Task<TOuterResult> Then<TOuterResult>(this Task task, Func<Task<TOuterResult>> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {

            return task.ThenImpl(t => continuation(), cancellationToken, runSynchronously);
        }

        /// <summary>
        /// Calls the given continuation, after the given task has completed, if the task successfully ran
        /// to completion (i.e., was not cancelled and did not fault). The continuation is provided with the
        /// result of the task as its sole parameter.
        /// </summary>
        internal static Task Then<TInnerResult>(this Task<TInnerResult> task, Action<TInnerResult> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {

            return task.ThenImpl<Task<TInnerResult>, AsyncVoid>(t => 
                ToAsyncVoidTask(() => continuation(t.Result)), cancellationToken, runSynchronously);
        }

        /// <summary>
        /// Calls the given continuation, after the given task has completed, if the task successfully ran
        /// to completion (i.e., was not cancelled and did not fault). The continuation is provided with the
        /// result of the task as its sole parameter.
        /// </summary>
        internal static Task<TOuterResult> Then<TInnerResult, TOuterResult>(this Task<TInnerResult> task, Func<TInnerResult, TOuterResult> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {

            return task.ThenImpl<Task<TInnerResult>, TOuterResult>(t => 
                TaskHelpers.FromResult(continuation(t.Result)), cancellationToken, runSynchronously);
        }

        /// <summary>
        /// Calls the given continuation, after the given task has completed, if the task successfully ran
        /// to completion (i.e., was not cancelled and did not fault). The continuation is provided with the
        /// result of the task as its sole parameter.
        /// </summary>
        internal static Task Then<TInnerResult>(this Task<TInnerResult> task, Func<TInnerResult, Task> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {

            return task.ThenImpl<Task<TInnerResult>, AsyncVoid>(t =>
                continuation(t.Result).ToTask<AsyncVoid>(), cancellationToken, runSynchronously);
        }

        /// <summary>
        /// Calls the given continuation, after the given task has completed, if the task successfully ran
        /// to completion (i.e., was not cancelled and did not fault). The continuation is provided with the
        /// result of the task as its sole parameter.
        /// </summary>
        internal static Task<TOuterResult> Then<TInnerResult, TOuterResult>(this Task<TInnerResult> task, Func<TInnerResult, Task<TOuterResult>> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {

            return task.ThenImpl(t => continuation(t.Result), cancellationToken, runSynchronously);
        }

        private static Task<TOuterResult> ThenImpl<TTask, TOuterResult>(this TTask task, Func<TTask, Task<TOuterResult>> continuation, CancellationToken cancellationToken, bool runSynchronously) where TTask : Task { 
            
            //We want to stay on th same thread if we can.
            //If the task is already completed, we avoid unnecessary continuations
            if (task.IsCompleted) {

                //Check if the task is in the Faulted state or not
                //If the condition is met, then return with a faulted task which contains the errors
                if (task.IsFaulted) {

                    return TaskHelpers.FromErrors<TOuterResult>(task.Exception.InnerExceptions);
                }

                //Check if the task is in the Canceled state or not. Also check if the cancellation is requested or not.
                //If the condition is met, then return with a canceled task
                if (task.IsCanceled || cancellationToken.IsCancellationRequested) {

                    return TaskHelpers.Canceled<TOuterResult>();
                }

                //Check if the task is in the RanToCompletion state which indicates the success
                if (task.Status == TaskStatus.RanToCompletion) {

                    try {

                        return continuation(task);
                    }
                    catch (Exception ex) {

                        return TaskHelpers.FromError<TOuterResult>(ex);
                    }
                }
            }

            // Split into a continuation method so that we don't create a closure unnecessarily
            return ThenImplContinuation(task, continuation, cancellationToken, runSynchronously);
        }

        private static Task<TOuterResult> ThenImplContinuation<TOuterResult, TTask>(TTask task, Func<TTask, Task<TOuterResult>> continuation, CancellationToken cancallationToken, bool runSynchronously = false) where TTask : Task { 

            //Grap the syncContext first
            SynchronizationContext syncContext = SynchronizationContext.Current;

            TaskCompletionSource<Task<TOuterResult>> tcs = new TaskCompletionSource<Task<TOuterResult>>();

            task.ContinueWith(innerTask => {

                //Check if the task is in the Faulted state or not
                //If the condition is met, then try to set exceptions on the TCS
                if (innerTask.IsFaulted) {

                    tcs.TrySetException(innerTask.Exception.InnerExceptions);
                }

                //Check if the task is in the Canceled state or not. Also check if the cancellation is requested or not.
                //If the condition is met, then try set the TCS as canceled
                else if (innerTask.Status == TaskStatus.Canceled || cancallationToken.IsCancellationRequested) {

                    tcs.TrySetCanceled();
                }

                //If we get here, this means that the innerTask.Status is RanToCompletion
                else { 

                    //Firstly, check if the syncContext is null or not.
                    //If not null, we want to invoke the continuation at the context thread
                    if (syncContext != null) {

                        syncContext.Post(state => {

                            try {
                                tcs.TrySetResult(continuation(task));
                            }
                            catch (Exception ex) {

                                tcs.TrySetException(ex);
                            }
                        }, null);
                    }
                    else {

                        tcs.TrySetResult(continuation(task));
                    }
                }

            }, runSynchronously ? TaskContinuationOptions.ExecuteSynchronously : TaskContinuationOptions.None);

            return tcs.Task.FastUnwrap();
        }

        /// <summary>
        /// Changes the return value of a task to the given result, if the task ends in the RanToCompletion state.
        /// This potentially imposes an extra ContinueWith to convert a non-completed task, so use this with caution.
        /// </summary>
        internal static Task<TResult> ToTask<TResult>(this Task task, CancellationToken cancellationToken = default(CancellationToken), TResult result = default(TResult)) {

            if (task == null) {

                return null;
            }

            // Stay on the same thread if we can
            if (task.IsCompleted) {

                if (task.IsFaulted) {

                    return TaskHelpers.FromErrors<TResult>(task.Exception.InnerExceptions);
                }
                if (task.IsCanceled || cancellationToken.IsCancellationRequested) {

                    return TaskHelpers.Canceled<TResult>();
                }
                if (task.Status == TaskStatus.RanToCompletion) {

                    return TaskHelpers.FromResult(result);
                }
            }

            // Split into a continuation method so that we don't create a closure unnecessarily
            return ToTaskContinuation(task, result);
        }

        private static Task<TResult> ToTaskContinuation<TResult>(Task task, TResult result) {

            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();

            task.ContinueWith(innerTask => {

                if (task.Status == TaskStatus.RanToCompletion) {

                    tcs.TrySetResult(result);
                }
                else {

                    tcs.TrySetFromTask(innerTask);
                }

            }, TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        /// <summary>
        /// Adapts any action into a Task (returning AsyncVoid, so that it's usable with Task{T} extension methods).
        /// </summary>
        private static Task<AsyncVoid> ToAsyncVoidTask(Action action) {

            return TaskHelpers.RunSynchronously<AsyncVoid>(() => {
                action();
                return _defaultCompleted;
            });
        }

        /// <summary>
        /// Used as the T in a "conversion" of a Task into a Task{T}
        /// </summary>
        private struct AsyncVoid { }
    }

    internal abstract class CatchInfoBase<TTask> where TTask : Task {

        private Exception _exception;
        private TTask _task;

        protected CatchInfoBase(TTask task) {

            if (task == null) {

                throw new ArgumentNullException("task");
            }

            _task = task;
            _exception = _task.Exception.GetBaseException(); // Observe the exception early, to prevent tasks tearing down the app domain
        }

        /// <summary>
        /// The exception that was thrown to cause the Catch block to execute.
        /// </summary>
        public Exception Exception {
            get { return _exception; }
        }

        /// <summary>
        /// Returns a CatchResult that re-throws the original exception.
        /// </summary>
        public CatchResult Throw() {

            return new CatchResult { Task = _task };
        }

        /// <summary>
        /// Represents a result to be returned from a Catch handler.
        /// </summary>
        internal struct CatchResult {

            /// <summary>
            /// Gets or sets the task to be returned to the caller.
            /// </summary>
            internal TTask Task { get; set; }
        }
    }

    internal class CatchInfo : CatchInfoBase<Task> {

        private static CatchResult _completed = new CatchResult { Task = TaskHelpers.Completed() };

        public CatchInfo(Task task) : base(task) { }

        /// <summary>
        /// Returns a CatchResult that returns a completed (non-faulted) task.
        /// </summary>
        public CatchResult Handled() {

            return _completed;
        }

        /// <summary>
        /// Returns a CatchResult that executes the given task and returns it, in whatever state it finishes.
        /// </summary>
        /// <param name="task">The task to return.</param>
        public CatchResult Task(Task task) {

            return new CatchResult { Task = task };
        }

        /// <summary>
        /// Returns a CatchResult that throws the given exception.
        /// </summary>
        /// <param name="ex">The exception to throw.</param>
        public CatchResult Throw(Exception ex) {

            return new CatchResult { Task = TaskHelpers.FromError<object>(ex) };
        }
    }

    internal class CatchInfo<T> : CatchInfoBase<Task<T>> {
        public CatchInfo(Task<T> task)
            : base(task) {
        }

        /// <summary>
        /// Returns a CatchResult that returns a completed (non-faulted) task.
        /// </summary>
        /// <param name="returnValue">The return value of the task.</param>
        public CatchResult Handled(T returnValue) {

            return new CatchResult { Task = TaskHelpers.FromResult(returnValue) };
        }

        /// <summary>
        /// Returns a CatchResult that executes the given task and returns it, in whatever state it finishes.
        /// </summary>
        /// <param name="task">The task to return.</param>
        public CatchResult Task(Task<T> task) {

            return new CatchResult { Task = task };
        }

        /// <summary>
        /// Returns a CatchResult that throws the given exception.
        /// </summary>
        /// <param name="ex">The exception to throw.</param>
        public CatchResult Throw(Exception ex) {

            return new CatchResult { Task = TaskHelpers.FromError<T>(ex) };
        }
    }
}