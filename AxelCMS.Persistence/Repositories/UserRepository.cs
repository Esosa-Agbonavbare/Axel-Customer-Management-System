using AxelCMS.Application.Interfaces.Repositories;
using AxelCMS.Domain.Entities;
using AxelCMS.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AxelCMS.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AxelCMSDbContext dbContext) : base(dbContext) {}

        public async Task AddUserAsync(User user) => await AddAsync(user);

        public async Task<bool> CheckUserAsync(Expression<Func<User, bool>> condition) => await ExistsAsync(condition);

        public async Task DeleteUserAsync(User user) => await DeleteAsync(user);

        public async Task<IList<User>> FindUserAsync(Expression<Func<User, bool>> condition) => await FindAsync(condition);

        public async Task<IList<User>> GetAllUsersAsync() => await GetAllAsync();

        public async Task<User> GetUserByIdAsync(string id) => await GetByIdAsync(id);

        public async Task UpdateUserAsync(User user) => await UpdateAsync(user);
    }
}
