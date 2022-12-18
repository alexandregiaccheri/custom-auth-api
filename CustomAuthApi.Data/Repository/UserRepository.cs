using CustomAuthApi.Data.Context;
using CustomAuthApi.Data.Models;
using CustomAuthApi.Data.Repository;
using CustomAuthApi.Data.Repository.IRepository;

namespace CustomAuthApi.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(CustomAuthDbContext dbContext) : base(dbContext) { }
    }
}
