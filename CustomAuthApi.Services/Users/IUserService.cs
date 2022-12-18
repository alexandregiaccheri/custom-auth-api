using CustomAuthApi.Data.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CustomAuthApi.Services.Users
{
    public interface IUserService
    {
        Task<ActionResult> CreateUserAsync(CreateUserDTO payload);

        Task<ActionResult<string>> LoginUserAsync(LoginDTO payload);
    }
}
