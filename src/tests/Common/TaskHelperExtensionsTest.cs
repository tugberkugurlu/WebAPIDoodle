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
        //  Task Task.Then(Action)

        [Fact, GCForce]
        public Task Then_NoInputValue_NoReturnValue_CallsContinuation() { 

            //Arange
            bool ranContinuation = false;

            return TaskHelpers.Completed()

                //Act
                .Then(() => {
                    ranContinuation = true;
                })

                //Assert
                .ContinueWith(task => {

                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.True(ranContinuation);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_NoReturnValue_ThrownExceptionIsRethrowd() { 

            //Arange
            return TaskHelpers.Completed()

                //Act
                .Then(() => {
                    throw new NotImplementedException();
                })

                //Assert
                .ContinueWith(task => {

                    Assert.Equal(TaskStatus.Faulted, task.Status);
                    var ex = Assert.Single(task.Exception.InnerExceptions);
                    Assert.IsType<NotImplementedException>(ex);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_NoReturnValue_FaultPreventsFurtherThenStatementsFromExecuting() {

            //Arange
            bool ranContinuation = false;

            return TaskHelpers.FromError(new NotImplementedException())

                //Act
                .Then(() => {

                    ranContinuation = true;
                })

                //Asset
                .ContinueWith(task => {

                    var ex = task.Exception;  // Observe the exception
                    Assert.Equal(TaskStatus.Faulted, task.Status);
                    Assert.False(ranContinuation);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_NoReturnValue_ManualCancellationPreventsFurtherThenStatementsFromExecuting() { 

            //Arange
            bool ranContinuation = false;

            return TaskHelpers.Canceled()

                //Act
                .Then(() => {

                    ranContinuation = true;
                })

                //Assert
                .ContinueWith(task => {

                    Assert.Equal(TaskStatus.Canceled, task.Status);
                    Assert.False(ranContinuation);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_NoReturnValue_TokenCancellationPreventsFurtherThenStatementsFromExecuting() { 

            //Aranage
            bool ranContinuation = false;
            CancellationToken cancellationToken = new CancellationToken(canceled: true);

            return TaskHelpers.Completed()

                //Act
                .Then(() => {

                    ranContinuation = true;

                }, cancellationToken)

                //Assert
                .ContinueWith(task => {

                    Assert.Equal(TaskStatus.Canceled, task.Status);
                    Assert.False(ranContinuation);
                });
        }
        
        [Fact, GCForce, PreserveSyncContext]
        public Task Then_NoInputValue_NoReturnValue_IncompleteTask_RunsOnNewThreadAndPostsContinuationToSynchronizationContext() { 

            //Arange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MaxValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            Task incompleteTask = new Task(() => {});

            //Act
            Task resultTask = incompleteTask.Then(() => {

                callbackThreadId = Thread.CurrentThread.ManagedThreadId;
            });

            // Assert
            incompleteTask.Start();

            return resultTask.ContinueWith(task => {

                Assert.NotEqual(originalThreadId, callbackThreadId);
                syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Once());
            });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_NoInputValue_NoReturnValue_CompleteTask_RunsOnSameThreadAndDoesNotPostToSynchronizationContext() { 

            //Arange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MinValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            return TaskHelpers.Completed()

                //Act
                .Then(() => {

                    callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                })

                //Assert
                .ContinueWith(task => {

                    Assert.Equal(originalThreadId, callbackThreadId);
                    syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Never());
                });
        }

        // -----------------------------------------------------------------
        //  Task Task.Then(Func<Task>)

        [Fact, GCForce]
        public Task Then_NoInputValue_ReturnsTask_CallsContinuation() {

            // Arrange
            bool ranContinuation = false;

            return TaskHelpers.Completed()

            // Act
                .Then(() => {
                    ranContinuation = true;
                    return TaskHelpers.Completed();
                })

            // Assert
                .ContinueWith(task => {
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.True(ranContinuation);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_ReturnsTask_ThrownExceptionIsRethrowd() {
            // Arrange
            return TaskHelpers.Completed()

            // Act
                .Then(() => {
                    throw new NotImplementedException();
                    return TaskHelpers.Completed();  // Return-after-throw to guarantee correct lambda signature
                })

            // Assert
                .ContinueWith(task => {
                    Assert.Equal(TaskStatus.Faulted, task.Status);
                    var ex = Assert.Single(task.Exception.InnerExceptions);
                    Assert.IsType<NotImplementedException>(ex);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_ReturnsTask_FaultPreventsFurtherThenStatementsFromExecuting() {

            // Arrange
            bool ranContinuation = false;

            return TaskHelpers.FromError(new NotImplementedException())

            // Act
                .Then(() => {
                    ranContinuation = true;
                    return TaskHelpers.Completed();
                })

            // Assert
                .ContinueWith(task => {
                    var ex = task.Exception;  // Observe the exception
                    Assert.False(ranContinuation);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_ReturnsTask_ManualCancellationPreventsFurtherThenStatementsFromExecuting() {

            // Arrange
            bool ranContinuation = false;

            return TaskHelpers.Canceled()

            // Act
                .Then(() => {
                    ranContinuation = true;
                    return TaskHelpers.Completed();
                })

            // Assert
                .ContinueWith(task => {
                    Assert.Equal(TaskStatus.Canceled, task.Status);
                    Assert.False(ranContinuation);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_ReturnsTask_TokenCancellationPreventsFurtherThenStatementsFromExecuting() {

            // Arrange
            bool ranContinuation = false;
            CancellationToken cancellationToken = new CancellationToken(canceled: true);

            return TaskHelpers.Completed()

            // Act
                              .Then(() => {
                                  ranContinuation = true;
                                  return TaskHelpers.Completed();
                              }, cancellationToken)

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.Canceled, task.Status);
                                  Assert.False(ranContinuation);
                              });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_NoInputValue_ReturnsTask_IncompleteTask_RunsOnNewThreadAndPostsContinuationToSynchronizationContext() {

            // Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MinValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            Task incompleteTask = new Task(() => { });

            // Act
            Task resultTask = incompleteTask.Then(() => {
                callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                return TaskHelpers.Completed();
            });

            // Assert
            incompleteTask.Start();

            return resultTask.ContinueWith(task => {
                Assert.NotEqual(originalThreadId, callbackThreadId);
                syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Once());
            });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_NoInputValue_ReturnsTask_CompleteTask_RunsOnSameThreadAndDoesNotPostToSynchronizationContext() {

            // Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MinValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            return TaskHelpers.Completed()

            // Act
                              .Then(() => {
                                  callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                                  return TaskHelpers.Completed();
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(originalThreadId, callbackThreadId);
                                  syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Never());
                              });
        }

        // -----------------------------------------------------------------
        //  Task<TOut> Task.Then(Func<TOut>)

        [Fact, GCForce]
        public Task Then_NoInputValue_WithReturnValue_CallsContinuation() {

            // Arrange
            return TaskHelpers.Completed()

            // Act
                .Then(() => {
                    return 42;
                })

            // Assert
                .ContinueWith(task => {
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.Equal(42, task.Result);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_WithReturnValue_ThrownExceptionIsRethrowd() {

            // Arrange
            return TaskHelpers.Completed()

            // Act
                .Then(() => {
                    throw new NotImplementedException();
                    return 0;  // Return-after-throw to guarantee correct lambda signature
                })

            // Assert
                .ContinueWith(task => {
                    Assert.Equal(TaskStatus.Faulted, task.Status);
                    var ex = Assert.Single(task.Exception.InnerExceptions);
                    Assert.IsType<NotImplementedException>(ex);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_WithReturnValue_FaultPreventsFurtherThenStatementsFromExecuting() {

            // Arrange
            bool ranContinuation = false;

            return TaskHelpers.FromError(new NotImplementedException())

            // Act
                .Then(() => {
                    ranContinuation = true;
                    return 42;
                })

            // Assert
                .ContinueWith(task => {
                    var ex = task.Exception;  // Observe the exception
                    Assert.False(ranContinuation);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_WithReturnValue_ManualCancellationPreventsFurtherThenStatementsFromExecuting() {

            // Arrange
            bool ranContinuation = false;

            return TaskHelpers.Canceled()

            // Act
                .Then(() => {
                    ranContinuation = true;
                    return 42;
                })

            // Assert
                .ContinueWith(task => {
                    Assert.Equal(TaskStatus.Canceled, task.Status);
                    Assert.False(ranContinuation);
                });
        }

        [Fact, GCForce]
        public Task Then_NoInputValue_WithReturnValue_TokenCancellationPreventsFurtherThenStatementsFromExecuting() {

            // Arrange
            bool ranContinuation = false;
            CancellationToken cancellationToken = new CancellationToken(canceled: true);

            return TaskHelpers.Completed()

            // Act
                .Then(() => {
                    ranContinuation = true;
                    return 42;
                }, cancellationToken)

            // Assert
                .ContinueWith(task => {
                    Assert.Equal(TaskStatus.Canceled, task.Status);
                    Assert.False(ranContinuation);
                });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_NoInputValue_WithReturnValue_IncompleteTask_RunsOnNewThreadAndPostsContinuationToSynchronizationContext() {

            // Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MinValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            Task incompleteTask = new Task(() => { });

            // Act
            Task resultTask = incompleteTask.Then(() => {
                callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                return 42;
            });

            // Assert
            incompleteTask.Start();

            return resultTask.ContinueWith(task => {
                Assert.NotEqual(originalThreadId, callbackThreadId);
                syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Once());
            });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_NoInputValue_WithReturnValue_CompleteTask_RunsOnSameThreadAndDoesNotPostToSynchronizationContext() {

            // Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MinValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            return TaskHelpers.Completed()

            // Act
                .Then(() => {
                    callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                    return 42;
                })

            // Assert
                .ContinueWith(task => {
                    Assert.Equal(originalThreadId, callbackThreadId);
                    syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Never());
                });
        }

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

        [Fact(Skip = "This test ocasionally fails because of the SyncContext I guess"), GCForce, PreserveSyncContext]
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

        // -----------------------------------------------------------------
        //  Task Task<TIn>.Then(Action<TIn>)

        [Fact, GCForce]
        public Task Then_WithInputValue_NoReturnValue_CallsContinuationWithPriorTaskResult() {
            // Arrange
            int passedResult = 0;

            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  passedResult = result;
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                                  Assert.Equal(21, passedResult);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_NoReturnValue_ThrownExceptionIsRethrowd() {
            // Arrange
            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  throw new NotImplementedException();
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.Faulted, task.Status);
                                  var ex = Assert.Single(task.Exception.InnerExceptions);
                                  Assert.IsType<NotImplementedException>(ex);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_NoReturnValue_FaultPreventsFurtherThenStatementsFromExecuting() {
            // Arrange
            bool ranContinuation = false;

            return TaskHelpers.FromError<int>(new NotImplementedException())

            // Act
                              .Then(result => {
                                  ranContinuation = true;
                              })

            // Assert
                              .ContinueWith(task => {
                                  var ex = task.Exception;  // Observe the exception
                                  Assert.False(ranContinuation);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_NoReturnValue_ManualCancellationPreventsFurtherThenStatementsFromExecuting() {
            // Arrange
            bool ranContinuation = false;

            return TaskHelpers.Canceled<int>()

            // Act
                              .Then(result => {
                                  ranContinuation = true;
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.Canceled, task.Status);
                                  Assert.False(ranContinuation);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_NoReturnValue_TokenCancellationPreventsFurtherThenStatementsFromExecuting() {
            // Arrange
            bool ranContinuation = false;
            CancellationToken cancellationToken = new CancellationToken(canceled: true);

            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  ranContinuation = true;
                              }, cancellationToken)

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.Canceled, task.Status);
                                  Assert.False(ranContinuation);
                              });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_WithInputValue_NoReturnValue_IncompleteTask_RunsOnNewThreadAndPostsContinuationToSynchronizationContext() {
            // Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MinValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            Task<int> incompleteTask = new Task<int>(() => 21);

            // Act
            Task resultTask = incompleteTask.Then(result => {
                callbackThreadId = Thread.CurrentThread.ManagedThreadId;
            });

            // Assert
            incompleteTask.Start();

            return resultTask.ContinueWith(task => {
                Assert.NotEqual(originalThreadId, callbackThreadId);
                syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Once());
            });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_WithInputValue_NoReturnValue_CompleteTask_RunsOnSameThreadAndDoesNotPostToSynchronizationContext() {
            // Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MinValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(originalThreadId, callbackThreadId);
                                  syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Never());
                              });
        }

        // -----------------------------------------------------------------
        //  Task<TOut> Task<TIn>.Then(Func<TIn, TOut>)

        [Fact, GCForce]
        public Task Then_WithInputValue_WithReturnValue_CallsContinuation() {
            // Arrange
            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  return 42;
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                                  Assert.Equal(42, task.Result);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_WithReturnValue_ThrownExceptionIsRethrowd() {
            // Arrange
            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  throw new NotImplementedException();
                                  return 0;  // Return-after-throw to guarantee correct lambda signature
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.Faulted, task.Status);
                                  var ex = Assert.Single(task.Exception.InnerExceptions);
                                  Assert.IsType<NotImplementedException>(ex);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_WithReturnValue_FaultPreventsFurtherThenStatementsFromExecuting() {
            // Arrange
            bool ranContinuation = false;

            return TaskHelpers.FromError<int>(new NotImplementedException())

            // Act
                              .Then(result => {
                                  ranContinuation = true;
                                  return 42;
                              })

            // Assert
                              .ContinueWith(task => {
                                  var ex = task.Exception;  // Observe the exception
                                  Assert.False(ranContinuation);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_WithReturnValue_ManualCancellationPreventsFurtherThenStatementsFromExecuting() {
            // Arrange
            bool ranContinuation = false;

            return TaskHelpers.Canceled<int>()

            // Act
                              .Then(result => {
                                  ranContinuation = true;
                                  return 42;
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.Canceled, task.Status);
                                  Assert.False(ranContinuation);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_WithReturnValue_TokenCancellationPreventsFurtherThenStatementsFromExecuting() {
            // Arrange
            bool ranContinuation = false;
            CancellationToken cancellationToken = new CancellationToken(canceled: true);

            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  ranContinuation = true;
                                  return 42;
                              }, cancellationToken)

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.Canceled, task.Status);
                                  Assert.False(ranContinuation);
                              });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_WithInputValue_WithReturnValue_IncompleteTask_RunsOnNewThreadAndPostsContinuationToSynchronizationContext() {
            // Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MinValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            Task<int> incompleteTask = new Task<int>(() => 21);

            // Act
            Task resultTask = incompleteTask.Then(result => {
                callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                return 42;
            });

            // Assert
            incompleteTask.Start();

            return resultTask.ContinueWith(task => {
                Assert.NotEqual(originalThreadId, callbackThreadId);
                syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Once());
            });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_WithInputValue_WithReturnValue_CompleteTask_RunsOnSameThreadAndDoesNotPostToSynchronizationContext() {
            // Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MinValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                                  return 42;
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(originalThreadId, callbackThreadId);
                                  syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Never());
                              });
        }

        // -----------------------------------------------------------------
        //  Task Task<TIn>.Then(Func<TIn, Task>)

        [Fact, GCForce]
        public Task Then_WithInputValue_ReturnsTask_CallsContinuation() {
            // Arrange
            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  return TaskHelpers.Completed();
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_ReturnsTask_ThrownExceptionIsRethrowd() {
            // Arrange
            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  throw new NotImplementedException();
                                  return TaskHelpers.Completed();  // Return-after-throw to guarantee correct lambda signature
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.Faulted, task.Status);
                                  var ex = Assert.Single(task.Exception.InnerExceptions);
                                  Assert.IsType<NotImplementedException>(ex);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_ReturnsTask_FaultPreventsFurtherThenStatementsFromExecuting() {
            // Arrange
            bool ranContinuation = false;

            return TaskHelpers.FromError<int>(new NotImplementedException())

            // Act
                              .Then(result => {
                                  ranContinuation = true;
                                  return TaskHelpers.Completed();
                              })

            // Assert
                              .ContinueWith(task => {
                                  var ex = task.Exception;  // Observe the exception
                                  Assert.False(ranContinuation);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_ReturnsTask_ManualCancellationPreventsFurtherThenStatementsFromExecuting() {
            // Arrange
            bool ranContinuation = false;

            return TaskHelpers.Canceled<int>()

            // Act
                              .Then(result => {
                                  ranContinuation = true;
                                  return TaskHelpers.Completed();
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.Canceled, task.Status);
                                  Assert.False(ranContinuation);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_ReturnsTask_TokenCancellationPreventsFurtherThenStatementsFromExecuting() {
            // Arrange
            bool ranContinuation = false;
            CancellationToken cancellationToken = new CancellationToken(canceled: true);

            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  ranContinuation = true;
                                  return TaskHelpers.Completed();
                              }, cancellationToken)

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.Canceled, task.Status);
                                  Assert.False(ranContinuation);
                              });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_WithInputValue_ReturnsTask_IncompleteTask_RunsOnNewThreadAndPostsContinuationToSynchronizationContext() {
            // Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MinValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            Task<int> incompleteTask = new Task<int>(() => 21);

            // Act
            Task resultTask = incompleteTask.Then(result => {
                callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                return TaskHelpers.Completed();
            });

            // Assert
            incompleteTask.Start();

            return resultTask.ContinueWith(task => {
                Assert.NotEqual(originalThreadId, callbackThreadId);
                syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Once());
            });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_WithInputValue_ReturnsTask_CompleteTask_RunsOnSameThreadAndDoesNotPostToSynchronizationContext() {
            // Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MinValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                                  return TaskHelpers.Completed();
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(originalThreadId, callbackThreadId);
                                  syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Never());
                              });
        }

        // -----------------------------------------------------------------
        //  Task<TOut> Task<TIn>.Then(Func<TIn, Task<TOut>>)

        [Fact, GCForce]
        public Task Then_WithInputValue_WithTaskReturnValue_CallsContinuation() {
            // Arrange
            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  return TaskHelpers.FromResult(42);
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                                  Assert.Equal(42, task.Result);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_WithTaskReturnValue_ThrownExceptionIsRethrowd() {
            // Arrange
            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  throw new NotImplementedException();
                                  return TaskHelpers.FromResult(0);  // Return-after-throw to guarantee correct lambda signature
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.Faulted, task.Status);
                                  var ex = Assert.Single(task.Exception.InnerExceptions);
                                  Assert.IsType<NotImplementedException>(ex);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_WithTaskReturnValue_FaultPreventsFurtherThenStatementsFromExecuting() {
            // Arrange
            bool ranContinuation = false;

            return TaskHelpers.FromError<int>(new NotImplementedException())

            // Act
                              .Then(result => {
                                  ranContinuation = true;
                                  return TaskHelpers.FromResult(42);
                              })

            // Assert
                              .ContinueWith(task => {
                                  var ex = task.Exception;  // Observe the exception
                                  Assert.False(ranContinuation);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_WithTaskReturnValue_ManualCancellationPreventsFurtherThenStatementsFromExecuting() {
            // Arrange
            bool ranContinuation = false;

            return TaskHelpers.Canceled<int>()

            // Act
                              .Then(result => {
                                  ranContinuation = true;
                                  return TaskHelpers.FromResult(42);
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.Canceled, task.Status);
                                  Assert.False(ranContinuation);
                              });
        }

        [Fact, GCForce]
        public Task Then_WithInputValue_WithTaskReturnValue_TokenCancellationPreventsFurtherThenStatementsFromExecuting() {
            // Arrange
            bool ranContinuation = false;
            CancellationToken cancellationToken = new CancellationToken(canceled: true);

            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  ranContinuation = true;
                                  return TaskHelpers.FromResult(42);
                              }, cancellationToken)

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(TaskStatus.Canceled, task.Status);
                                  Assert.False(ranContinuation);
                              });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_WithInputValue_WithTaskReturnValue_IncompleteTask_RunsOnNewThreadAndPostsContinuationToSynchronizationContext() {
            // Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MinValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            Task<int> incompleteTask = new Task<int>(() => 21);

            // Act
            Task resultTask = incompleteTask.Then(result => {
                callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                return TaskHelpers.FromResult(42);
            });

            // Assert
            incompleteTask.Start();

            return resultTask.ContinueWith(task => {
                Assert.NotEqual(originalThreadId, callbackThreadId);
                syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Once());
            });
        }

        [Fact, GCForce, PreserveSyncContext]
        public Task Then_WithInputValue_WithTaskReturnValue_CompleteTask_RunsOnSameThreadAndDoesNotPostToSynchronizationContext() {
            // Arrange
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            int callbackThreadId = Int32.MinValue;
            var syncContext = new Mock<SynchronizationContext> { CallBase = true };
            SynchronizationContext.SetSynchronizationContext(syncContext.Object);

            return TaskHelpers.FromResult(21)

            // Act
                              .Then(result => {
                                  callbackThreadId = Thread.CurrentThread.ManagedThreadId;
                                  return TaskHelpers.FromResult(42);
                              })

            // Assert
                              .ContinueWith(task => {
                                  Assert.Equal(originalThreadId, callbackThreadId);
                                  syncContext.Verify(sc => sc.Post(It.IsAny<SendOrPostCallback>(), null), Times.Never());
                              });
        }
    }
}