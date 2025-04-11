namespace ShopListApp.Core.Interfaces.StoreObserver;

public interface IStorePublisher
{
    void AddSubscribers();
    Task Notify();
}
