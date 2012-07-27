
using System.Collections.Generic;
namespace System.Threading.Tasks {

    /// <summary>
    /// Helpers for safely using Task libraries (taken from ASP.NET Web Stack source: http://aspnetwebstack.codeplex.com)
    /// </summary>
    internal static class TaskHelpers {

        private static readonly Task<object> _completedTaskReturningNull = FromResult<object>(null);
        private static readonly Task _defaultCompleted = FromResult<AsyncVoid>(default(AsyncVoid));

        internal static Task<object> NullResult() {

            return _completedTaskReturningNull;
        }

        /// <summary>
        /// Returns a successful completed task with the given result.  
        /// </summary>
        internal static Task<TResult> FromResult<TResult>(TResult result) {

            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            tcs.SetResult(result);
            return tcs.Task;
        }

        /// <summary>
        /// Returns an error task. The task is Completed, IsCanceled = False, IsFaulted = True
        /// </summary>
        internal static Task FromError(Exception exception) {

            return FromError<AsyncVoid>(exception);
        }

        /// <summary>
        /// Returns an error task of the given type. The task is Completed, IsCanceled = False, IsFaulted = True
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        internal static Task<TResult> FromError<TResult>(Exception exception) {

            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exception);
            return tcs.Task;
        }

        /// <summary>
        /// Returns an error task of the given type. The task is Completed, IsCanceled = False, IsFaulted = True
        /// </summary>
        internal static Task FromErrors(IEnumerable<Exception> exceptions) {

            return FromErrors<AsyncVoid>(exceptions);
        }

        /// <summary>
        /// Returns an error task of the given type. The task is Completed, IsCanceled = False, IsFaulted = True
        /// </summary>
        internal static Task<TResult> FromErrors<TResult>(IEnumerable<Exception> exceptions) {

            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exceptions);
            return tcs.Task;
        }

        /// <summary>
        /// Returns a completed task that has no result. 
        /// </summary>
        internal static Task Completed() {

            return _defaultCompleted;
        }

        /// <summary>
        /// Returns a canceled Task. The task is completed, IsCanceled = True, IsFaulted = False.
        /// </summary>
        internal static Task Canceled() {

            return Canceled<AsyncVoid>();
        }

        /// <summary>
        /// Returns a canceled Task of the given type. The task is completed, IsCanceled = True, IsFaulted = False.
        /// </summary>
        internal static Task<TResult> Canceled<TResult>() {

            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            tcs.SetCanceled();
            return tcs.Task;
        }

        /// <summary>
        /// Replacement for Task.Factory.StartNew when the code can run synchronously. 
        /// We run the code immediately and avoid the thread switch. 
        /// This is used to help synchronous code implement task interfaces.
        /// </summary>
        /// <param name="action">action to run synchronouslyt</param>
        /// <param name="token">cancellation token. This is only checked before we run the task, and if cancelled, we immediately return a cancelled task.</param>
        /// <returns>a task who result is the result from Func()</returns>
        /// <remarks>
        /// Avoid calling Task.Factory.StartNew.         
        /// This avoids gotchas with StartNew:
        /// - ensures cancellation token is checked (StartNew doesn't check cancellation tokens).
        /// - Keeps on the same thread. 
        /// - Avoids switching synchronization contexts.
        /// Also take in a lambda so that we can wrap in a try catch and honor task failure semantics.        
        /// </remarks>
        internal static Task RunSynchronously(Action action, CancellationToken token = default(CancellationToken)) {

            if (token.IsCancellationRequested) {

                return Canceled();
            }

            try {

                action();
                return Completed();
            }
            catch (Exception e) {

                return FromError(e);
            }

        }

        /// <summary>
        /// Replacement for Task.Factory.StartNew when the code can run synchronously. 
        /// We run the code immediately and avoid the thread switch. 
        /// This is used to help synchronous code implement task interfaces.
        /// </summary>
        /// <typeparam name="TResult">type of result that task will return.</typeparam>
        /// <param name="func">function to run synchronously and produce result</param>
        /// <param name="cancellationToken">cancellation token. This is only checked before we run the task, and if cancelled, we immediately return a cancelled task.</param>
        /// <returns>a task who result is the result from Func()</returns>
        /// <remarks>
        /// Avoid calling Task.Factory.StartNew.         
        /// This avoids gotchas with StartNew:
        /// - ensures cancellation token is checked (StartNew doesn't check cancellation tokens).
        /// - Keeps on the same thread. 
        /// - Avoids switching synchronization contexts.
        /// Also take in a lambda so that we can wrap in a try catch and honor task failure semantics.        
        /// </remarks>
        internal static Task<TResult> RunSynchronously<TResult>(Func<TResult> func, CancellationToken token = default(CancellationToken)) {

            if (token.IsCancellationRequested) {

                return Canceled<TResult>();
            }

            try {

                return FromResult(func());
            }
            catch (Exception e) {

                return FromError<TResult>(e);
            }
        }

        /// <summary>
        /// Overload of RunSynchronously that avoids a call to Unwrap(). 
        /// This overload is useful when func() starts doing some synchronous work and then hits IO and 
        /// needs to create a task to finish the work. 
        /// </summary>
        /// <typeparam name="TResult">type of result that Task will return</typeparam>
        /// <param name="func">function that returns a task</param>
        /// <param name="cancellationToken">cancellation token. This is only checked before we run the task, and if cancelled, we immediately return a cancelled task.</param>
        /// <returns>a task, created by running func().</returns>
        internal static Task<TResult> RunSynchronously<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default(CancellationToken)) {

            if (cancellationToken.IsCancellationRequested) {

                return Canceled<TResult>();
            }

            try {

                return func();
            }
            catch (Exception e) {

                return FromError<TResult>(e);
            }
        }

        /// <summary>
        /// Used as the T in a "conversion" of a Task into a Task{T}
        /// </summary>
        private struct AsyncVoid { }
    }
}