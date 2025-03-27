using HtmlAgilityPack;
using ShopListApp.Application.StoreSubscibers;
using ShopListApp.Core.Interfaces.Parsing;
using ShopListApp.Core.Interfaces.StoreObserver;
using ShopListApp.DataProviders;
using ShopListApp.Interfaces;
using ShopListApp.Interfaces.IRepositories;

namespace ShopListApp.StoreObserver
{
    public class StorePublisher : IStorePublisher
    {
        private List<IStoreSubscriber> _subscribers = new List<IStoreSubscriber>();
        private readonly IHtmlFetcher<HtmlNode, HtmlDocument> _htmlFetcher;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStoreRepository _storeRepository;
        public StorePublisher(IHtmlFetcher<HtmlNode, HtmlDocument> htmlFetcher, 
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IStoreRepository storeRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _storeRepository = storeRepository;
            _htmlFetcher = htmlFetcher;
        }
        public async Task Notify()
        {
            foreach (var subscriber in _subscribers)
            {
                await subscriber.Update();
            }
        }

        public void AddSubscribers()
        {
            var biedronkaParser = new BiedronkaParser(_htmlFetcher);
            var biedronkaSubscriber = new StoreSubscriber(biedronkaParser, _productRepository, _categoryRepository, _storeRepository);
            _subscribers.Add(biedronkaSubscriber);
        }
    }
}
