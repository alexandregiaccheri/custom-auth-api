using CustomAuthApi.Data.DTO;
using CustomAuthApi.Services.Users;
using CustomAuthApi.Testing.FakeData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;

namespace CustomAuthApi.Testing.ServicesTests
{
    public class UserServiceTests
    {
        private readonly static IConfiguration s_configurationMock =
            new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();
        private readonly static FakeUserRepository _fakeUserRepository = new();
        private readonly static UserService _userService = new
            UserService(_fakeUserRepository, s_configurationMock);

        public static IEnumerable<object[]> FakeCreateUserData => new[]
        {
            new object[]
            {
                new CreateUserDTO()
                {
                    Email = "anotherfakeuser@email.com",
                    Password = "P@ssw0rd!",
                    RepeatPassword = "P@ssw0rd!",
                    Role = "user"
                }, true
            },

            new object[]
            {
                new CreateUserDTO()
                {
                    Email = "anotherfakeuser",
                    Password = "P@ssw0rd!",
                    RepeatPassword = "P@ssw0rd!",
                    Role = "user"
                }, false
            },

            new object[]
            {
                new CreateUserDTO()
                {
                    Email = "anotherfakeuser@email.com",
                    Password = "password",
                    RepeatPassword = "password",
                    Role = "user"
                }, false
            },

            new object[]
            {
                new CreateUserDTO()
                {
                    Email = "anotherfakeuser@email.com",
                    Password = "P@ssw0rd!",
                    RepeatPassword = "P@ssw0rd!",
                    Role = "manager"
                }, false
            },
        };

        public static IEnumerable<object[]> FakeLoginUserData => new[]
        {
            new object[] {
                new LoginDTO()
                    { Email = "fakeuser@email.com", Password = "P@ssw0rd!" }
                , true },

            new object[] {
                new LoginDTO()
                    { Email = "FAkEusEr@eMaIl.cOm", Password = "P@ssw0rd!" }
                , true },

            new object[] {
                new LoginDTO()
                    { Email = " fakeuser@email.com", Password = "P@ssw0rd!" }
                , true },

            new object[] {
                new LoginDTO()
                    { Email = "fakeuser@email.com ", Password = "P@ssw0rd!" }
                , true },

            new object[] {
                new LoginDTO()
                    { Email = " fakeuser@email.com ", Password = "P@ssw0rd!" }
                , true },

            new object[] {
                new LoginDTO()
                    { Email = "fakeuser@email.com", Password = "P@ssw0rd!!" }
                , false },

            new object[] {
                new LoginDTO()
                    { Email = "fakeuser@email.com", Password = " P@ssw0rd!" }
                , false },

            new object[] {
                new LoginDTO()
                    { Email = "fakeuser@email.com", Password = "P@ssw0rd! " }
                , false },

            new object[] {
                new LoginDTO()
                    { Email = "fakeuser@email.com", Password = "" }
                , false },

            new object[] {
                new LoginDTO()
                    { Email = "email@email.com", Password = "P@ssw0rd!" }
                , false },

            new object[] {
                new LoginDTO()
                    { Email = "email@email.com", Password = "password" }
                , false },

            new object[] {
                new LoginDTO()
                    { Email = "email@email.com", Password = "" }
                , false },

            new object[] {
                new LoginDTO()
                    { Email = "", Password = "" }
                , false },
        };

        // Testing Login

        [Theory]
        [MemberData(nameof(FakeLoginUserData))]
        public async Task
            LoginUserAsync_Returns_Readable_JWT_Only_When_Using_Correct_Credentials(
            LoginDTO payload, bool expectedResult)
        {
            var jwt = await _userService.LoginUserAsync(payload);
            var jwtResult = jwt.Result as ObjectResult;
            var jwtHandler = new JsonWebTokenHandler();

            Assert.Equal(jwtHandler.CanReadToken(
                jwtResult!.Value!.ToString()), expectedResult);
        }

        [Fact]
        public async Task LoginUserAsync_Inexistent_User_Returns_NotFound()
        {
            LoginDTO payload = new()
            {
                Email = "wrongemail@email.com",
                Password = "password"
            };
            var response = await _userService.LoginUserAsync(payload);
            var responseResult = response.Result as ObjectResult;

            Assert.Equal(404, responseResult?.StatusCode);
            Assert.Equal("User not found", responseResult?.Value?.ToString());
        }

        [Fact]
        public async Task LoginUserAsync_Wrong_Password_Must_Return_BadRequest()
        {
            LoginDTO payload = new()
            {
                Email = "fakeuser@email.com",
                Password = "password"
            };
            var response = await _userService.LoginUserAsync(payload);
            var responseResult = response.Result as ObjectResult;

            Assert.Equal(400, responseResult?.StatusCode);
            Assert.Equal("Wrong email/password combination",
                responseResult?.Value?.ToString());
        }

        // Testing Register

        [Fact]
        public async Task CreateUserAsync_Existing_Email_Returns_BadRequest()
        {
            var payloadArray = new CreateUserDTO[]
            {
                new()
                {
                    Email = "fakeuser@email.com",
                    Password = "NewP@ssw0rd!",
                    RepeatPassword = "NewP@ssw0rd!",
                    Role = "user"
                },

                new()
                {
                    Email = "fakeuser@email.com ",
                    Password = "NewP@ssw0rd!",
                    RepeatPassword = "NewP@ssw0rd!",
                    Role = "user"
                },

                new()
                {
                    Email = " fakeuser@email.com",
                    Password = "NewP@ssw0rd!",
                    RepeatPassword = "NewP@ssw0rd!",
                    Role = "user"
                },

                new()
                {
                    Email = "FAKEUser@eMail.Com",
                    Password = "NewP@ssw0rd!",
                    RepeatPassword = "NewP@ssw0rd!",
                    Role = "user"
                },
                new()
                {
                    Email = "fakeuser@email.com",
                    Password = "",
                    RepeatPassword = "",
                    Role = ""
                }
            };

            foreach (var payload in payloadArray)
            {
                var responseResult = await
                    _userService.CreateUserAsync(payload) as ObjectResult;

                Assert.Equal(400, responseResult?.StatusCode);
                Assert.Equal("This email address is alaready registered",
                    responseResult?.Value?.ToString());
            }
        }

        [Fact]
        public async Task CreateUserAsync_Invalid_Email_Format_Returns_BadRequest()
        {
            var payloadArray = new CreateUserDTO[]
            {
                new()
                {
                    Email = "fakeuser@email",
                    Password = "NewP@ssw0rd!",
                    RepeatPassword = "NewP@ssw0rd!",
                    Role = "user"
                },

                new()
                {
                    Email = "fakeuser",
                    Password = "NewP@ssw0rd!",
                    RepeatPassword = "NewP@ssw0rd!",
                    Role = "user"
                },

                new()
                {
                    Email = "fakeuser@email",
                    Password = "",
                    RepeatPassword = "",
                    Role = ""
                }
            };

            foreach (var payload in payloadArray)
            {
                var responseResult = await
                    _userService.CreateUserAsync(payload) as ObjectResult;

                Assert.Equal(400, responseResult?.StatusCode);
                Assert.Equal("Invalid email address",
                    responseResult?.Value?.ToString());
            }
        }

        [Fact]
        public async Task CreateUserAsync_Weak_Password_Returns_BadRequest()
        {
            var payloadArray = new CreateUserDTO[]
            {
                new()
                {
                    Email = "newuser@email.com",
                    Password = "newuser123",
                    RepeatPassword = "newuser123",
                    Role = "user"
                },

                new()
                {
                    Email = "newuser@email.com",
                    Password = "@ab12A.",
                    RepeatPassword = "@ab12A.",
                    Role = "user"
                },

                new()
                {
                    Email = "newuser@email.com",
                    Password = "@abccA.a",
                    RepeatPassword = "@abccA.a",
                    Role = "user"
                },

                new()
                {
                    Email = "newuser@email.com",
                    Password = "@ab12a.a",
                    RepeatPassword = "@ab12a.a",
                    Role = "user"
                },

                new()
                {
                    Email = "newuser@email.com",
                    Password = "Aab12ACa",
                    RepeatPassword = "Aab12ACa",
                    Role = "user"
                },

                new()
                {
                    Email = "newuser@email.com",
                    Password = "",
                    RepeatPassword = "",
                    Role = "user"
                }
            };

            foreach (var payload in payloadArray)
            {
                var requestResult = await _userService.CreateUserAsync(payload)
                    as ObjectResult;

                Assert.Equal(400, requestResult?.StatusCode);
                Assert.Equal(
                    "The password must contain at least: 8+ characters, one or " +
                    "more upper case letters, one or more lower case letters, " +
                    "one or more numbers and at least one special character!",
                    requestResult?.Value?.ToString()
                    );
            }
        }

        [Fact]
        public async Task CreateUserAsync_Different_Passwords_Returns_BadRequest()
        {
            var payloadArray = new CreateUserDTO[]
            {
                new()
                {
                    Email = "newuser@email.com",
                    Password = "NewP@ssw0rd!",
                    RepeatPassword = "NewP@sssw0rd!",
                    Role = "user"
                },

                new()
                {
                    Email = "newuser@email.com",
                    Password = "NewP@ssw0rd!",
                    RepeatPassword = "",
                    Role = "user"
                }
            };

            foreach (var payload in payloadArray)
            {
                var requestResult = await
                    _userService.CreateUserAsync(payload) as ObjectResult;

                Assert.Equal(400, requestResult?.StatusCode);
                Assert.Equal("Passwords do not match!",
                    requestResult?.Value?.ToString());
            }
        }

        [Fact]
        public async Task CreateUserAsync_Invalid_Role_Returns_BadRequest()
        {
            var payloadArray = new CreateUserDTO[]
            {
                new()
                {
                    Email = "newuser@email.com",
                    Password = "NewP@ssw0rd!",
                    RepeatPassword = "NewP@ssw0rd!",
                    Role = ""
                },

                new()
                {
                    Email = "newuser@email.com",
                    Password = "NewP@ssw0rd!",
                    RepeatPassword = "NewP@ssw0rd!",
                    Role = "abc"
                },
            };

            foreach (var payload in payloadArray)
            {
                var requestResult = await
                    _userService.CreateUserAsync(payload) as ObjectResult;

                Assert.Equal(400, requestResult?.StatusCode);
                Assert.Equal("Accepted roles are \"admin\" and \"user\"",
                    requestResult?.Value?.ToString());
            }
        }

        [Fact]
        public async Task CreateUserAsync_Succesful_Register_Returns_Ok()
        {
            var payloadArray = new CreateUserDTO[]
            {
                new()
                {
                    Email = "newvaliduser@email.com",
                    Password = "NewP@ssw0rd!",
                    RepeatPassword = "NewP@ssw0rd!",
                    Role = "user"
                },

                new()
                {
                    Email = "newvalidadmin@email.com ",
                    Password = "NewP@ssw0rd!",
                    RepeatPassword = "NewP@ssw0rd!",
                    Role = "admin"
                },
            };

            foreach (var payload in payloadArray)
            {
                var requestResult = await
                    _userService.CreateUserAsync(payload) as ObjectResult;

                Assert.Equal(201, requestResult?.StatusCode);
                Assert.Equal("User successfully created",
                    requestResult?.Value?.ToString());
            }
        }
    }
}
