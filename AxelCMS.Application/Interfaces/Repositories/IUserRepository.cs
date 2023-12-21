using AxelCMS.Domain.Entities;
using System.Linq.Expressions;

namespace AxelCMS.Application.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task AddUserAsync(User user);
        Task<User> GetUserByIdAsync(string id);
        Task<IList<User>> GetAllUsersAsync();
        Task<IList<User>> FindUserAsync(Expression<Func<User, bool>> condition);
        Task<bool> CheckUserAsync(Expression<Func<User, bool>> condition);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
    }
}
