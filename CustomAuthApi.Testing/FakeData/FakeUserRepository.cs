using CustomAuthApi.Data.Models;
using CustomAuthApi.Data.Repository.IRepository;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace CustomAuthApi.Testing.FakeData
{
    public class FakeUserRepository : IUserRepository
    {
        private List<User> _users;

        public FakeUserRepository()
        {
            using (var hmac = new HMACSHA512())
            {
                var hashedPassword = hmac.ComputeHash(
                    Encoding.UTF8.GetBytes("P@ssw0rd!"));
                var salt = hmac.Key;

                _users = new List<User>();
                var user = new User()
                {
                    Id = Guid.Parse("7E42B498-6858-4834-8B9A-F43E9415A33F"),
                    Email = "fakeuser@email.com",
                    Password = hashedPassword,
                    PasswordSalt = salt,
                    Role = "admin"
                };
                _users.Add(user);
            }
        }

        public Task AddAsync(User entity)
        {
            _users.Add(entity);
            return Task.CompletedTask;
        }

        public async Task<User?> GetFirstOrDefaultAsync(
            Expression<Func<User, bool>> filter)
        {
            var func = filter.Compile();
            Predicate<User> predicate = func.Invoke;
            User? user = _users.Find(predicate);
            return await Task.FromResult(user);
        }
    }
}
