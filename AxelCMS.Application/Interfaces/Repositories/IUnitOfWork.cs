using AxelCMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxelCMS.Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Order> OrderRepository { get; }
        IGenericRepository<OrderItem> OrderItemRepository { get; }
        IGenericRepository<Product> ProductRepository { get; }
        IGenericRepository<Address> AddressRepository { get; }
        Task SaveChangesAsync();
    }
}
