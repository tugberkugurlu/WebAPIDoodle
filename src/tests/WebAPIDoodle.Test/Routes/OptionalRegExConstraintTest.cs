using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Routing;
using WebAPIDoodle.Routes;
using Xunit;

namespace WebAPIDoodle.Test.Routes {
    
    public class OptionalRegExConstraintTest {

        [Fact]
        public void OptionalRegExConstraint_ThrowsArgumentNullException_If_ExpressionParamIsNull() {
        }

        [Fact]
        public void Match_ReturnsTrue_If_RouteValueIsRouteParamaterOptional() { 
        }

        [Fact]
        public void Match_ReturnsTrue_If_RouteValueIsNotRouteParamaterOptional_And_RegExMatchesTheValue() {
        }

        [Fact]
        public void Match_ReturnsFalse_If_RouteValueIsNotRouteParamaterOptional_And_RegExNotMatchesTheValue() {
        }
    }
}