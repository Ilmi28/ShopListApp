using ShopListApp.Interfaces;

namespace ShopListApp.StoreObserver
{
    public class StorePublisher
    {
        private List<IStoreSubscriber> _subscribers = new List<IStoreSubscriber>();
        public void Subscribe(IStoreSubscriber subscriber)
        {
            _subscribers.Add(subscriber);
        }
        public void Unsubscribe(IStoreSubscriber subscriber)
        {
            _subscribers.Remove(subscriber);
        }
        public async Task Notify()
        {
            foreach (var subscriber in _subscribers)
            {
                await subscriber.Update();
            }
        }
    }
}
