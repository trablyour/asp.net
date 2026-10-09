using ShopApi.Models;

namespace ShopApi.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetByIdWithProductsAsync(int id);
    }
}
