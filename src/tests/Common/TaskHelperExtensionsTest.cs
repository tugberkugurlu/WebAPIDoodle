using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace WebAPIDoodle.Test.Common {

    //Mostly taken from ASP.NET Web Stack source: http://aspnetwebstack.codeplex.com
    public class TaskHelperExtensionsTest {

        // -----------------------------------------------------------------
        //  Task<TOut> Task.Then(Func<Task<TOut>>)

        [Fact, GCForce]
        public Task Then_NoInputValue_WithTaskReturnValue_CallsContinuation() { 

            //Arange
            return TaskHelpers.Completed()

            //Act
                .Then(() => {
                    return TaskHelpers.FromResult(0);
                })

            //Assert
                .ContinueWith(task => {

                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.Equal(0, task.Result);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_WithTaskReturnValue_ThrownExceptionIsRethrowd() { 

            //Arange
            return TaskHelpers.Completed()

            //Act
                .Then(() => {
                    throw new NotImplementedException();
                    return TaskHelpers.FromResult(0);
                })

            //Assert
                .ContinueWith(task => {
                    
                    Assert.Equal(TaskStatus.Faulted, task.Status);
                    var ex = Assert.Single(task.Exception.InnerExceptions);
                    Assert.IsType<NotImplementedException>(ex);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_WithTaskReturnValue_FaultPreventsFurtherThenStatementsFromExecuting() {

            // Arrange
            bool ranContinuation = false;
            return TaskHelpers.FromError(new NotImplementedException())

            //Act
                .Then(() => {
                    ranContinuation = true;
                    return TaskHelpers.FromResult(0);
                })

            //Assert
                .ContinueWith(task => { 

                    //Observe the exception
                    var ex = task.Exception;
                    Assert.False(ranContinuation);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_WithTaskReturnValue_ManualCancellationPreventsFurtherThenStatementsFromExecuting() {

            //Arrange
            bool ranContinuation = false;
            return TaskHelpers.Canceled()

            //Act
                .Then(() => {
                    ranContinuation = true;
                    return TaskHelpers.FromResult(0);
                })

            //Assert
                .ContinueWith(task => {
                    Assert.Equal(TaskStatus.Canceled, task.Status);
                    Assert.False(ranContinuation);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_WithTaskReturnValue_TokenCancellationPreventsFurtherThenStatementsFromExecuting() { 

            // Arrange
            bool ranContinuation = false;
            CancellationToken cancellationToken = new CancellationToken(canceled: true);

            return TaskHelpers.Completed()

            //Act
                .Then(() => {

                    ranContinuation = true;
                    return TaskHelpers.FromResult(0);
                }, cancellationToken)

            //Assert
                .ContinueWith(task => {

                    Assert.Equal(TaskStatus.Canceled, task.Status);
                    Assert.False(ranContinuation);
                });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_NoInputValue_WithTaskReturnValue_IncompleteTask_RunsOnNewThreadAndPostsContinuationToSynchronizationContext() {

            // Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MaxValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            Task incompleteTask = new Task(() => { });

            //Act
            Task resultTask = incompleteTask.Then(() => { 

                callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                return TaskHelpers.FromResult(0);
            });

            incompleteTask.Start();

            //Assert
            return incompleteTask.ContinueWith(task => {
                Assert.NotEqual(originalThreadId, callbackThreadId);
                syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Once());
            });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_NoInputValue_WithTaskReturnValue_CompleteTask_RunsOnSameThreadAndDoesNotPostToSynchronizationContext() {

            //Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MaxValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            return TaskHelpers.Completed()

            //Act
                .Then(() => {
                    callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                    return TaskHelpers.FromResult(1);
                })

            //Assert
                .ContinueWith(task => {
                    Assert.Equal(originalThreadId, callbackThreadId);
                    syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Never());
                });
        }
    }
}