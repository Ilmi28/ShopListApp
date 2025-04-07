using ShopListApp.Core.Models;

namespace ShopListApp.Core.Interfaces.IRepositories
{
    public interface IStoreRepository
    {
        Task<ICollection<Store>> GetStores();
        Task<Store?> GetStoreById(int id);
    }
}
