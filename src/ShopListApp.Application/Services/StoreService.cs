﻿using ShopListApp.Core.Exceptions;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Interfaces.IServices;
using ShopListApp.Core.Responses;

namespace ShopListApp.Application.Services;

public class StoreService(IStoreRepository storeRepository) : IStoreService
{
    public async Task<ICollection<StoreResponse>> GetStores()
    {
        var stores = await storeRepository.GetStores();
        var storeViews = new List<StoreResponse>();
        foreach (var store in stores)
        {
            var storeView = new StoreResponse
            {
                Id = store.Id,
                Name = store.Name,
            };
            storeViews.Add(storeView);
        }
        return storeViews;
    }

    public async Task<StoreResponse> GetStoreById(int id)
    {
        var store = await storeRepository.GetStoreById(id) ?? throw new StoreNotFoundException($"Store with id {id} not found.");
        var storeView = new StoreResponse
        {
            Id = store.Id,
            Name = store.Name,
        };
        return storeView;
    }
}
