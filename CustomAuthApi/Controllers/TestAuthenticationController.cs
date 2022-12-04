using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomAuthApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TestAuthenticationController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> AuthenticationNotRequired()
        {
            return Ok("No authentication was required for this request");
        }

        [HttpGet, Authorize]
        public ActionResult<string> AuthenticationRequired()
        {
            return Ok("If you are reading this, the JWT token has been " +
                "succesfully validated");
        }

        [HttpGet, Authorize(Roles = "admin")]
        public ActionResult<string> AdminRoleRequired()
        {
            return Ok("If you are reading this, the JWT token has been " +
                "successfully validated and has an admin role");
        }
    }
}
