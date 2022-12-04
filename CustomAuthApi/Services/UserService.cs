using CustomAuthApi.DTO;
using CustomAuthApi.Models;
using CustomAuthApi.Repository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CustomAuthApi.Services
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

        public bool CheckForValidRole(string role)
        {
            if (role == "admin" || role == "user") return true;
            else return false;
        }

        public bool CheckUserPasswordCombination(LoginDTO payload, User user)
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

        public async Task<User> CreateUserAsync(CreateUserDTO payload)
        {
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

            return newUser;
        }

        public string GenerateJWT(User user)
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

        public async Task<User?> GetUserAsync(string email)
        {
            return await _userRepository.GetFirstOrDefaultAsync(u =>
                u.Email == email.Trim().ToLower());
        }
    }
}
