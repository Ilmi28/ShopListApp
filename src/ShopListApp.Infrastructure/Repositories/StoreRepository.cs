﻿using Microsoft.EntityFrameworkCore;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Models;
using ShopListApp.Infrastructure.Database.Context;

namespace ShopListApp.Infrastructure.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly ShopListDbContext _context;
        public StoreRepository(ShopListDbContext context)
        {
            _context = context;
        }

        public async Task<Store?> GetStoreById(int id)
        {
            return await _context.Stores.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<ICollection<Store>> GetStores()
        {
            return await _context.Stores.ToListAsync();
        }
    }
}
