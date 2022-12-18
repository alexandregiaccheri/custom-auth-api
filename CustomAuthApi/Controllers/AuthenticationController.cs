using CustomAuthApi.Data.DTO;
using CustomAuthApi.Services.Users;
using Microsoft.AspNetCore.Mvc;

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
            return await _userService.LoginUserAsync(payload);
        }

        [HttpPost]
        public async Task<ActionResult<string>> Register(CreateUserDTO payload)
        {
            return await _userService.CreateUserAsync(payload);
        }
    }
}
