using ShopListApp.Commands;

namespace ShopListApp.Interfaces
{
    public interface IParser
    {
        Task<ICollection<ParseProductCommand>> GetParsedProducts();
    }
}
