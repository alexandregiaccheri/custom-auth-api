using CustomAuthApi.DTO;
using CustomAuthApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CustomAuthApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Login(LoginDTO payload)
        {
            var user = await _userService.GetUserAsync(payload.Email);
            if (user != null)
            {
                if (_userService.CheckUserPasswordCombination(payload, user))
                    return Ok(_userService.GenerateJWT(user));

                else return BadRequest("Wrong email/password combination");
            }

            else return BadRequest("User not found");
        }

        [HttpPost]
        public async Task<ActionResult<string>> Register(CreateUserDTO payload)
        {
            var emailCheck = new Regex("^\\S+@\\S+\\.\\S+$");
            var passwordCheck = new
                Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");

            if (await _userService.GetUserAsync(payload.Email) != null)
                return BadRequest("This email address is alaready registered");

            if (!emailCheck.IsMatch(payload.Email))
                return BadRequest("Invalid email address");

            if (!passwordCheck.IsMatch(payload.Password))
                return BadRequest("The password must contain at least: 8+ " +
                    "characters, one or more upper case letters, one or more " +
                    "lower case letters, one or more numbers and at least one " +
                    "special character!");

            if (!payload.Password.Equals(payload.RepeatPassword))
                return BadRequest("Passwords do not match!");

            if (_userService.CheckForInvalidRole(payload.Role))
                return BadRequest("Accepted roles are \"admin\" and \"user\"");

            await _userService.CreateUserAsync(payload);

            return StatusCode(201, "User successfully created");
        }
    }
}
