using CustomAuthApi.Data;
using CustomAuthApi.Models;

namespace CustomAuthApi.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(CustomAuthDbContext dbContext) : base(dbContext) { }
    }
}
