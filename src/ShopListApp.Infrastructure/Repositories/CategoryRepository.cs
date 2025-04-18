﻿using Microsoft.EntityFrameworkCore;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Models;
using ShopListApp.Infrastructure.Database.Context;

namespace ShopListApp.Infrastructure.Repositories;

public class CategoryRepository(ShopListDbContext context) : ICategoryRepository
{
    public async Task<ICollection<Category>> GetAllCategories()
    {
        return await context.Categories.ToListAsync();
    }

    public async Task<Category?> GetCategoryById(int? id)
    {
        return await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Category?> GetCategoryByName(string? name)
    {
        return await context.Categories.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task AddCategory(Category category)
    {
        await context.Categories.AddAsync(category);
    }

    public async Task<bool> RemoveCategory(int id)
    {
        var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
        if (category == null)
            return false;
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return true;
    }
}
