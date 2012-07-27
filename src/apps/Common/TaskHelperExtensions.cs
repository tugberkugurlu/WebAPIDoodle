using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading.Tasks {

    /// <summary>
    /// Proper extensions for Task class which makes it easy to use with it (mostly taken from ASP.NET Web Stack source: http://aspnetwebstack.codeplex.com)
    /// </summary>
    internal static class TaskHelperExtensions {

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
        internal static Task<TOuterResult> Then<TOuterResult>(this Task task, Func<Task<TOuterResult>> continuation, CancellationToken cancellationToken = default(CancellationToken), bool runSynchronously = false) {

            return task.ThenImpl(t => continuation(), cancellationToken, runSynchronously);
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
    }
}