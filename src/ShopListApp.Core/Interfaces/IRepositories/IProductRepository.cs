using ShopListApp.Core.Models;

namespace ShopListApp.Core.Interfaces.IRepositories;

public interface IProductRepository
{
    Task AddProduct(Product product);
    Task<bool> RemoveProduct(int id);
    Task<bool> UpdateProduct(int id, Product updatedProduct);
    Task<Product?> GetProductById(int id);
    Task<Product?> GetProductByName(string name);
    Task<ICollection<Product>> GetProductsByCategoryId(int categoryId);
    Task<ICollection<Product>> GetProductsByStoreId(int storeId);
    Task<ICollection<Product>> GetAllProducts();
}
