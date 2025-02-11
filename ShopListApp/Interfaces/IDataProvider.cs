using ShopListApp.Models;

namespace ShopListApp.Interfaces
{
    public interface IDataProvider
    {
        ICollection<Product> ProvideProducts();
    }
}
