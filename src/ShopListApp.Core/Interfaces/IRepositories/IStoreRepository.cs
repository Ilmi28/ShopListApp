using ShopListApp.Models;

namespace ShopListApp.Interfaces.IRepositories
{
    public interface IStoreRepository
    {
        Task<ICollection<Store>> GetStores();
        Task<Store?> GetStoreById(int id);
    }
}
