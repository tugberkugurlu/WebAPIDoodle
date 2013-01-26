using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApiDoodle.Net.Http.Client.Internal;
using WebApiDoodle.Net.Http.Client.Model;
using Xunit;

namespace WebApiDoodle.Net.Http.Client.Test.Internal {
    
    public class UriUtilTest {

        private const string BaseRequestPath = "api/cars";
        private const string BaseRequestUriPath = "http://localhost/api/cars";

        public class ResolveUriTemplate {

            [Fact]
            public void Returns_The_Expected_Uri_When_The_Inputs_Valid() {

                // Arrange
                var id = Guid.NewGuid().ToString();
                var requestCommand = new FakeNameRequestCommand { Name = "foo" };

                // Act
                var requestUri = UriUtil.ResolveUriTemplate(
                    "api/cars/{id}?name={name}", id,
                    new QueryStringCollection(requestCommand));

                // Assert
                Assert.Equal(string.Format("api/cars/{0}?name={1}", id, requestCommand.Name.ToLowerInvariant()), requestUri, StringComparer.InvariantCulture);
            }

            [Fact]
            public void Returns_The_Expected_Uri_When_The_Inputs_Valid_With_No_QueryStringCollection() {

                // Arrange
                var id = Guid.NewGuid().ToString();

                // Act
                var requestUri = UriUtil.ResolveUriTemplate(
                    "api/cars/{id}", id, null);

                // Assert
                Assert.Equal(string.Format("api/cars/{0}", id), requestUri, StringComparer.InvariantCulture);
            }

            [Fact]
            public void Returns_The_Expected_Uri_When_The_Inputs_Valid_With_No_Id() {

                // Arrange
                var requestCommand = new FakeNameRequestCommand { Name = "foo" };

                // Act
                var requestUri = UriUtil.ResolveUriTemplate<string>(
                    "api/cars?name={name}", null, 
                    new QueryStringCollection(requestCommand));

                // Assert
                Assert.Equal(string.Format("api/cars?name={0}", requestCommand.Name.ToLowerInvariant()), requestUri, StringComparer.InvariantCulture);
            }

            [Fact]
            public void Returns_The_Expected_Uri_When_The_Inputs_Valid_With_No_Template() {

                // Act
                var requestUri = UriUtil.ResolveUriTemplate<string>(
                    "api/cars", null, null);

                // Assert
                Assert.Equal("api/cars", requestUri, StringComparer.InvariantCulture);
            }

            [Fact]
            public void Returns_The_Expected_Uri_When_The_Inputs_Valid_With_No_Template_And_QueryStringCollection() {

                // Arrange
                var requestCommand = new FakeNameAgeRequestCommand { Name = "Foo", Age = 36 };

                // Act
                var requestUri = UriUtil.ResolveUriTemplate<string>(
                    "api/cars", null, 
                    new QueryStringCollection(requestCommand));

                // Assert
                Assert.Equal(string.Format("api/cars?age={1}&name={0}", requestCommand.Name.ToLowerInvariant(), requestCommand.Age.ToString()), requestUri, StringComparer.InvariantCulture);
            }

            [Fact]
            public void Throws_InvalidOperationException_When_QueryStringCollection_Has_Less_Input_Than_Expected() {

                // Arrange
                var requestCommand = new FakeNameRequestCommand { Name = "foo" };

                // Assert
                Assert.Throws<InvalidOperationException>(() => // Act
                    UriUtil.ResolveUriTemplate<string>(
                        "api/cars?name={name}&surname={surname}", 
                        null, new QueryStringCollection(requestCommand)));
            }

            [Fact]
            public void Throws_InvalidOperationException_If_QueryStringCollection_Is_Null_When_Needed() {

                // Assert
                Assert.Throws<InvalidOperationException>(() => // Act
                    UriUtil.ResolveUriTemplate<string>(
                        "api/cars?name={name}&surname={surname}", null, null));
            }
        }

        private class FakeDto : IDto {

            public int Id { get; set; }
        }

        private class FakeNameRequestCommand {

            public string Name { get; set; }
        }

        private class FakeNameAgeRequestCommand : FakeNameRequestCommand {

            public int Age { get; set; }
        }
    }
}