using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIDoodle {

    /// <summary>
    /// Helpers for safely using Task libraries (taken from ASP.NET Web Stack source: http://aspnetwebstack.codeplex.com)
    /// </summary>
    internal static class TaskHelpers {

        private static readonly Task _defaultCompleted = FromResult<AsyncVoid>(default(AsyncVoid));

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
        /// Used as the T in a "conversion" of a Task into a Task{T}
        /// </summary>
        private struct AsyncVoid { }
    }
}