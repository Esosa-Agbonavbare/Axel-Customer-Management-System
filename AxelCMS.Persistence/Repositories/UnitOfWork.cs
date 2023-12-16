using AxelCMS.Application.Interfaces.Repositories;
using AxelCMS.Domain.Entities;
using AxelCMS.Persistence.Context;

namespace AxelCMS.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AxelCMSDbContext _dbContext;

        public UnitOfWork(AxelCMSDbContext dbContext)
        {
            _dbContext = dbContext;
            UserRepository = new GenericRepository<User>(_dbContext);
            OrderRepository = new GenericRepository<Order>(_dbContext);
            OrderItemRepository = new GenericRepository<OrderItem>(_dbContext);
            ProductRepository = new GenericRepository<Product>(_dbContext);
            AddressRepository = new GenericRepository<Address>(_dbContext);
        }
        public IGenericRepository<User> UserRepository { get; }

        public IGenericRepository<Order> OrderRepository { get; }

        public IGenericRepository<OrderItem> OrderItemRepository { get; }

        public IGenericRepository<Product> ProductRepository { get; }

        public IGenericRepository<Address> AddressRepository { get; }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
