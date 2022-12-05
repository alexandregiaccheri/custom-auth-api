using CustomAuthApi.Controllers;
using CustomAuthApi.DTO;
using CustomAuthApi.Models;
using CustomAuthApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;

namespace CustomAuthApi.Testing.ControllerTests
{
    public class AuthenticationControllerTests
    {
        //----------------------------//
        // Tests for the Login Action //
        //----------------------------//

        [Fact]
        public async Task Login_With_Correct_Credentials()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var authController = new
                AuthenticationController(userServiceMock.Object);

            var payload = new LoginDTO()
            {
                Email = "email",
                Password = "password"
            };
            var user = new User()
            {
                Email = "email",
                Password = Encoding.UTF8.GetBytes("password")
            };

            userServiceMock.Setup(s => s.GetUserAsync(payload.Email))
                .ReturnsAsync(user);

            userServiceMock.Setup(s =>
                s.CheckUserPasswordCombination(payload, user)).Returns(true);

            userServiceMock.Setup(s => s.GenerateJWT(user)).Returns("validToken");

            // Act
            var login = await authController.Login(payload);
            var loginOkObj = login.Result as ObjectResult;

            // Assert
            Assert.NotNull(login);
            Assert.IsType<ActionResult<string>>(login);
            Assert.Equal("validToken", loginOkObj?.Value);
            userServiceMock.Verify(s => s.GetUserAsync(payload.Email), Times.Once());
            userServiceMock.Verify(s =>
                s.CheckUserPasswordCombination(payload, user), Times.Once());
            userServiceMock.Verify(s => s.GenerateJWT(user), Times.Once());
        }

        [Fact]
        public async Task Login_With_Incorrect_Password()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var authController = new AuthenticationController(userServiceMock.Object);

            var payload = new LoginDTO()
            {
                Email = "email",
                Password = "password"
            };
            var user = new User()
            {
                Email = "email",
                Password = Encoding.UTF8.GetBytes("password")
            };

            userServiceMock.Setup(s => s.GetUserAsync(payload.Email))
                .ReturnsAsync(user);

            userServiceMock.Setup(s =>
                s.CheckUserPasswordCombination(payload, user)).Returns(false);

            // Act
            var login = await authController.Login(payload);
            var loginOkObj = login.Result as ObjectResult;

            // Assert
            Assert.Equal("Wrong email/password combination", loginOkObj?.Value);
            Assert.IsType<ActionResult<string>>(login);
            Assert.NotNull(login);
            userServiceMock.Verify(s => s.GetUserAsync(payload.Email), Times.Once());
            userServiceMock.Verify(s =>
                s.CheckUserPasswordCombination(payload, user), Times.Once());
        }

        [Fact]
        public async Task Login_With_Inexistent_User_Email()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var authController = new
                AuthenticationController(userServiceMock.Object);

            var payload = new LoginDTO()
            {
                Email = "email",
                Password = "password"
            };

            userServiceMock.Setup(s => s.GetUserAsync(payload.Email))
                .ReturnsAsync(() => null);

            // Act
            var login = await authController.Login(payload);
            var loginOkObj = login.Result as ObjectResult;

            // Assert
            Assert.NotNull(login);
            Assert.IsType<ActionResult<string>>(login);
            Assert.Equal("User not found", loginOkObj?.Value);
            userServiceMock.Verify(s => s.GetUserAsync(payload.Email), Times.Once());
        }

        //-------------------------------//
        // Tests for the Register Action //
        //-------------------------------//

        [Fact]
        public async Task Register_With_Accepted_Credentials()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var authController = new
                AuthenticationController(userServiceMock.Object);

            var payload = new CreateUserDTO()
            {
                Email = "validemail@email.com",
                Password = "V@lidPass0rd",
                RepeatPassword = "V@lidPass0rd",
                Role = "admin"
            };

            userServiceMock.Setup(s =>
                s.CheckForInvalidRole(payload.Role)).Returns(false);

            // Act
            var register = await authController.Register(payload);
            var registerOkObj = register.Result as ObjectResult;

            // Assert
            Assert.NotNull(register);
            Assert.IsType<ActionResult<string>>(register);
            Assert.Equal("User successfully created", registerOkObj?.Value);
            userServiceMock.Verify(s =>
                s.CheckForInvalidRole(payload.Role), Times.Once());
            userServiceMock.Verify(s => s.CreateUserAsync(payload), Times.Once());
        }

        [Fact]
        public async Task Register_With_Already_Used_Email()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var authController = new
                AuthenticationController(userServiceMock.Object);

            var payload = new CreateUserDTO() { };

            userServiceMock.Setup(s =>
                s.GetUserAsync(payload.Email)).ReturnsAsync(new User());

            // Act
            var register = await authController.Register(payload);
            var registerObj = register.Result as ObjectResult;

            // Assert
            Assert.Equal("This email address is alaready registered",
                registerObj?.Value);
            Assert.IsType<ActionResult<string>>(register);
            Assert.NotNull(register);
        }

        [Fact]
        public async Task Register_With_Invalid_Email()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var authController = new
                AuthenticationController(userServiceMock.Object);

            var payload = new CreateUserDTO()
            {
                Email = "invalidemail"
            };

            // Act
            var register = await authController.Register(payload);
            var registerObj = register.Result as ObjectResult;

            // Assert
            Assert.Equal("Invalid email address", registerObj?.Value);
            Assert.IsType<ActionResult<string>>(register);
            Assert.NotNull(register);
        }

        [Fact]
        public async Task Register_With_Invalid_Password()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var authController = new
                AuthenticationController(userServiceMock.Object);

            var payload = new CreateUserDTO()
            {
                Email = "validemail@email.com",
                Password = "weakPassword"
            };

            // Act
            var register = await authController.Register(payload);
            var registerObj = register.Result as ObjectResult;

            // Assert
            Assert.Equal("The password must contain at least: 8+ " +
                    "characters, one or more upper case letters, one or more " +
                    "lower case letters, one or more numbers and at least one " +
                    "special character!", registerObj?.Value);
            Assert.IsType<ActionResult<string>>(register);
            Assert.NotNull(register);
        }

        [Fact]
        public async Task Register_With_Different_Passwords()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var authController = new
                AuthenticationController(userServiceMock.Object);

            var payload = new CreateUserDTO()
            {
                Email = "validemail@email.com",
                Password = "V@lidPass0rd",
                RepeatPassword = "V@lidPass0rD"
            };

            // Act
            var register = await authController.Register(payload);
            var registerObj = register.Result as ObjectResult;

            // Assert
            Assert.Equal("Passwords do not match!", registerObj?.Value);
            Assert.IsType<ActionResult<string>>(register);
            Assert.NotNull(register);
        }

        [Fact]
        public async Task Register_With_Invalid_Role()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var authController = new
                AuthenticationController(userServiceMock.Object);

            var payload = new CreateUserDTO()
            {
                Email = "validemail@email.com",
                Password = "V@lidPass0rd",
                RepeatPassword = "V@lidPass0rd"
            };

            userServiceMock.Setup(s =>
                s.CheckForInvalidRole(payload.Role)).Returns(true);

            // Act
            var register = await authController.Register(payload);
            var registerObj = register.Result as ObjectResult;

            // Assert
            Assert.Equal("Accepted roles are \"admin\" and \"user\"",
                registerObj?.Value);
            Assert.IsType<ActionResult<string>>(register);
            Assert.NotNull(register);
        }
    }
}
