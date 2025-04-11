using HtmlAgilityPack;
using ShopListApp.Application.StoreSubscibers;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Interfaces.Parsing;
using ShopListApp.Core.Interfaces.StoreObserver;
using ShopListApp.Infrastructure.Parsers;

namespace ShopListApp.Infrastructure.StorePublisher;

public class StorePublisher(IHtmlFetcher<HtmlNode, HtmlDocument> htmlFetcher,
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IStoreRepository storeRepository) : IStorePublisher
{
    private List<IStoreSubscriber> _subscribers = new List<IStoreSubscriber>();

    public async Task Notify()
    {
        foreach (var subscriber in _subscribers)
        {
            await subscriber.Update();
        }
    }

    public void AddSubscribers()
    {
        var biedronkaParser = new BiedronkaParser(htmlFetcher);
        var biedronkaSubscriber = new StoreSubscriber(biedronkaParser, productRepository, categoryRepository, storeRepository);
        _subscribers.Add(biedronkaSubscriber);
    }
}
