using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebAPIDoodle.Test {

    //mostly taken from ASP.NET Web Stack source: http://aspnetwebstack.codeplex.com
    public class TaskHelpersTest {

        //TaskHelpers.Canceled
        [Fact]
        public void Canceled_ReturnsCanceledTask() {

            Task task = TaskHelpers.Canceled();

            Assert.NotNull(task);
            Assert.True(task.IsCanceled);
        }

        //TaskHelpers.Canceled<T>
        [Fact]
        public void Canceled_Generic_ReturnsCanceledTask() {

            Task<string> task = TaskHelpers.Canceled<string>();

            Assert.NotNull(task);
            Assert.True(task.IsCanceled);
        }

        //TaskHelpers.Completed
        [Fact]
        public void Completed_RuturnsCompletedTask() {

            Task task = TaskHelpers.Completed();

            Assert.NotNull(task);
            Assert.Equal(TaskStatus.RanToCompletion, task.Status);
        }

        //TaskHelpers.FromError
        [Fact]
        public void FromError_RuturnsFaultedTaskWithTheGivenException() {

            var exception = new Exception();
            Task task = TaskHelpers.FromError(exception);

            Assert.NotNull(task);
            Assert.True(task.IsFaulted);
            Assert.Same(exception, task.Exception.InnerException);
        }

        //TaskHelpers.FromError<T>
        [Fact]
        public void FromError_Generic_RuturnsFaultedTaskWithTheGivenException() {
            
            var exception = new Exception();
            Task<int> task = TaskHelpers.FromError<int>(exception);

            Assert.NotNull(task);
            Assert.True(task.IsFaulted);
            Assert.Same(exception, task.Exception.InnerException);
        }

        //TaskHelpers.FromErrors
        [Fact]
        public void FromErrors_RuturnsFaultedTaskWithTheGivenExceptions() {

            var exceptions = new[] { new Exception(), new InvalidOperationException() };
            Task task = TaskHelpers.FromErrors(exceptions);

            Assert.NotNull(task);
            Assert.True(task.IsFaulted);
            Assert.Equal(exceptions, task.Exception.InnerExceptions.ToArray());
        }

        //TODO:
        //TaskHelpers.FromErrors<T>

        //TODO:
        //TaskHelpers.FromResult<T>

        //TODO:
        //TaskHelpers.RunSynchronously
    }
}