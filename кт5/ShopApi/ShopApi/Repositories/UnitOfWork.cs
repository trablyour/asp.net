using ShopApi.Data;

namespace ShopApi.Repositories
{
    // все репозитории работают с одним контекстом, а сохранение идет одним вызовом SaveAsync
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Products = new ProductRepository(context);
            Categories = new CategoryRepository(context);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
