using ShopListApp.Exceptions;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Interfaces.IServices;
using ShopListApp.Models;
using ShopListApp.ViewModels;

namespace ShopListApp.Services
{
    public class StoreService : IStoreService
    {
        private IStoreRepository _storeRepository;
        public StoreService(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<ICollection<StoreView>> GetStores()
        {
            try
            {
                var stores = await _storeRepository.GetStores();
                var storeViews = new List<StoreView>();
                foreach (var store in stores)
                {
                    var storeView = new StoreView
                    {
                        Id = store.Id,
                        Name = store.Name,
                    };
                    storeViews.Add(storeView);
                }
                return storeViews;
            }
            catch
            {
                throw new DatabaseErrorException();
            }
        }

        public async Task<StoreView> GetStoreById(int id)
        {
            try
            {
                var store = await _storeRepository.GetStoreById(id) ?? throw new StoreNotFoundException();
                var storeView = new StoreView
                {
                    Id = store.Id,
                    Name = store.Name,
                };
                return storeView;
            }
            catch (StoreNotFoundException) { throw; }
            catch { throw new DatabaseErrorException(); }
        }
    }
}
