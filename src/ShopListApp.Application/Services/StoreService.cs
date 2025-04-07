using ShopListApp.Core.Exceptions;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Interfaces.IServices;
using ShopListApp.Core.Responses;

namespace ShopListApp.Application.Services
{
    public class StoreService : IStoreService
    {
        private IStoreRepository _storeRepository;
        public StoreService(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<ICollection<StoreResponse>> GetStores()
        {
            try
            {
                var stores = await _storeRepository.GetStores();
                var storeViews = new List<StoreResponse>();
                foreach (var store in stores)
                {
                    var storeView = new StoreResponse
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

        public async Task<StoreResponse> GetStoreById(int id)
        {
            try
            {
                var store = await _storeRepository.GetStoreById(id) ?? throw new StoreNotFoundException();
                var storeView = new StoreResponse
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
