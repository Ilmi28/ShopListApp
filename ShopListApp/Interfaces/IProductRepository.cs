using ShopListApp.Models;

namespace ShopListApp.Interfaces
{
    public interface IProductRepository
    {
        Task AddProduct(Product product);
        Task<bool> RemoveProduct(int id);
        Task<bool> UpdateProduct(int id, Product updatedProduct);
        Task<Product?> GetProductById(int id);
    }
}
