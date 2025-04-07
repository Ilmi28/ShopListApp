using Microsoft.EntityFrameworkCore;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Models;
using ShopListApp.Infrastructure.Database.Context;

namespace ShopListApp.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private ShopListDbContext _context;
        public CategoryRepository(ShopListDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryById(int? id)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Category?> GetCategoryByName(string? name)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task AddCategory(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task<bool> RemoveCategory(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
                return false;
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
