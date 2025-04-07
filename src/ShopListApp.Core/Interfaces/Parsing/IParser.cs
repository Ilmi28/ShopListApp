using ShopListApp.Core.Commands.Other;

namespace ShopListApp.Core.Interfaces.Parsing
{
    public interface IParser
    {
        Task<ICollection<ParseProductCommand>> GetParsedProducts();
    }
}
