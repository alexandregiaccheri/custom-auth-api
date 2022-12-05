using CustomAuthApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CustomAuthApi.Testing.ControllerTests
{
    public class TestAuthenticationControllerTests
    {
        [Fact]
        public void AuthenticationNotRequired_Returns_Ok()
        {
            // Arrange
            var testAuthController = new TestAuthenticationController();

            // Act
            var result = testAuthController.AuthenticationNotRequired();
            var resultOkObj = result.Result as OkObjectResult;

            // Assert
            Assert.Equal("No authentication was required for this request",
                resultOkObj?.Value);
            Assert.IsType<ActionResult<string>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void AuthenticationRequired_Returns_Ok()
        {
            // Arrange
            var testAuthController = new TestAuthenticationController();

            // Act
            var result = testAuthController.AuthenticationRequired();
            var resultOkObj = result.Result as OkObjectResult;

            // Assert
            Assert.Equal("If you are reading this, the JWT token has been " +
                "succesfully validated", resultOkObj?.Value);
            Assert.IsType<ActionResult<string>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void AdminRoleRequired_Returns_Ok()
        {
            // Arrange
            var testAuthController = new TestAuthenticationController();

            // Act
            var result = testAuthController.AdminRoleRequired();
            var resultOkObj = result.Result as OkObjectResult;

            // Assert
            Assert.Equal("If you are reading this, the JWT token has been " +
                "successfully validated and has an admin role", resultOkObj?.Value);
            Assert.IsType<ActionResult<string>>(result);
            Assert.NotNull(result);
        }
    }
}