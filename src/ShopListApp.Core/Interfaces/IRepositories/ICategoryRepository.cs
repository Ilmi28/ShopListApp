using ShopListApp.Core.Models;

namespace ShopListApp.Core.Interfaces.IRepositories
{
    public interface ICategoryRepository
    {
        Task<bool> RemoveCategory(int id);
        Task<Category?> GetCategoryById(int? id);
        Task AddCategory(Category category);
        Task<Category?> GetCategoryByName(string? name);
        Task<ICollection<Category>> GetAllCategories();
    }
}
