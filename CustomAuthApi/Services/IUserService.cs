using CustomAuthApi.DTO;
using CustomAuthApi.Models;

namespace CustomAuthApi.Services
{
    public interface IUserService
    {
        bool CheckForValidRole(string role);

        bool CheckUserPasswordCombination(LoginDTO payload, User user);

        Task<User> CreateUserAsync(CreateUserDTO payload);

        string GenerateJWT(User user);

        Task<User?> GetUserAsync(string email);
    }
}
