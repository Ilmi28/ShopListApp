using Microsoft.AspNetCore.Html;
using ShopListApp.Commands;
using ShopListApp.Models;

namespace ShopListApp.Interfaces
{
    public interface IParser
    {
        Task<ICollection<ParseProductCommand>> GetParsedProducts();
    }
}
