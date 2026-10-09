namespace ShopApi.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }

        Task<int> SaveAsync();
    }
}
