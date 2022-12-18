using CustomAuthApi.Data.DTO;
using CustomAuthApi.Data.Models;
using CustomAuthApi.Data.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CustomAuthApi.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        private string GenerateJWT(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var symmKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Security:JWTKey").Value!));

            var credentials = new SigningCredentials(symmKey,
                SecurityAlgorithms.HmacSha512Signature);

            var jwtToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(3),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return jwt;
        }

        private async Task<User?> GetUserAsync(string email)
        {
            return await _userRepository.GetFirstOrDefaultAsync(u =>
                u.Email == email.Trim().ToLower());
        }

        private bool ValidEmailFormatCheck(string email)
        {
            var emailCheck = new Regex("^\\S+@\\S+\\.\\S+$");
            if (emailCheck.IsMatch(email.Trim().ToLower())) return true;
            else return false;
        }

        private bool ValidPasswordFormatCheck(string password)
        {
            var passwordCheck = new
                Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            if (passwordCheck.IsMatch(password)) return true;
            else return false;
        }

        private bool ValidRoleCheck(string role)
        {
            if (role == "admin" || role == "user") return true;
            else return false;
        }

        private bool VerifyUserPasswordCombination(LoginDTO payload, User user)
        {
            if (user == null) return false;

            byte[] payloadPassword;
            using (var hmac = new HMACSHA512())
            {
                hmac.Key = user.PasswordSalt;
                payloadPassword = hmac.ComputeHash(Encoding.UTF8
                    .GetBytes(payload.Password));
            }

            if (payloadPassword.SequenceEqual(user.Password)) return true;
            else return false;
        }

        public async Task<ActionResult> CreateUserAsync(CreateUserDTO payload)
        {
            if (await GetUserAsync(payload.Email) != null)
                return new BadRequestObjectResult(
                    "This email address is alaready registered");

            if (ValidEmailFormatCheck(payload.Email) == false)
                return new BadRequestObjectResult("Invalid email address");

            if (ValidPasswordFormatCheck(payload.Password) == false)
                return new BadRequestObjectResult(
                    "The password must contain at least: 8+ characters, one or " +
                    "more upper case letters, one or more lower case letters, " +
                    "one or more numbers and at least one special character!");

            if (payload.Password != payload.RepeatPassword)
                return new BadRequestObjectResult("Passwords do not match!");

            if (ValidRoleCheck(payload.Role) == false)
                return new BadRequestObjectResult(
                    "Accepted roles are \"admin\" and \"user\"");

            var newUser = new User
            {
                Email = payload.Email.Trim().ToLower(),
                Role = payload.Role
            };

            using (var hmac = new HMACSHA512())
            {
                newUser.PasswordSalt = hmac.Key;
                newUser.Password = hmac.ComputeHash(Encoding.UTF8
                    .GetBytes(payload.Password));
            }

            await _userRepository.AddAsync(newUser);

            return new ObjectResult("User successfully created") { StatusCode = 201 };
        }

        public async Task<ActionResult<string>> LoginUserAsync(LoginDTO payload)
        {
            var user = await GetUserAsync(payload.Email);
            if (user == null)
                return new NotFoundObjectResult("User not found");

            if (VerifyUserPasswordCombination(payload, user) == false)
                return new BadRequestObjectResult(
                    "Wrong email/password combination");

            var jwt = GenerateJWT(user);
            return new OkObjectResult(jwt);
        }
    }
}
