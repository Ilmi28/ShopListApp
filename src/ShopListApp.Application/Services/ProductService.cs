﻿using ShopListApp.Core.Exceptions;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Interfaces.IServices;
using ShopListApp.Core.Interfaces.StoreObserver;
using ShopListApp.Core.Models;
using ShopListApp.Core.Responses;

namespace ShopListApp.Application.Services;

public class ProductService(IProductRepository productRepository, IStoreRepository storeRepository,
    ICategoryRepository categoryRepository, IStorePublisher storePublisher) : IProductService
{
    public async Task<ICollection<ProductResponse>> GetAllProducts()
    {
        var products = await productRepository.GetAllProducts();
        var productViews = GetProductViewsList(products);
        return productViews;
    }

    public async Task<ICollection<ProductResponse>> GetProductsByCategoryId(int categoryId)
    {
        _ = await categoryRepository.GetCategoryById(categoryId)
                ?? throw new CategoryNotFoundException();
        var products = await productRepository.GetProductsByCategoryId(categoryId);
        var productViews = GetProductViewsList(products);
        return productViews;
    }

    public async Task<ICollection<ProductResponse>> GetProductsByStoreId(int storeId)
    {
        _ = await storeRepository.GetStoreById(storeId)
               ?? throw new StoreNotFoundException();
        var products = await productRepository.GetProductsByStoreId(storeId);
        var productViews = GetProductViewsList(products);
        return productViews;
    }

    public async Task RefreshProducts()
    {
        storePublisher.AddSubscribers();
        await storePublisher.Notify();
    }

    public async Task<ICollection<CategoryResponse>> GetCategories()
    {
        var categories = await categoryRepository.GetAllCategories();
        var categoryViews = new List<CategoryResponse>();
        foreach (var category in categories)
        {
            var categoryView = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
            };
            categoryViews.Add(categoryView);
        }
        return categoryViews;
    }

    private ICollection<ProductResponse> GetProductViewsList(ICollection<Product> products)
    {
        var productViews = new List<ProductResponse>();
        foreach (var product in products)
        {
            var productView = new ProductResponse
            {
                Id = product.Id,  
                Name = product.Name,
                Price = product.Price,
                StoreId = product.Store.Id,
                StoreName = product.Store.Name,
                CategoryId = product.Category?.Id,
                CategoryName = product.Category?.Name,
                ImageUrl = product.ImageUrl,
            };
            productViews.Add(productView);
        }
        return productViews;
    }

    public async Task<ProductResponse?> GetProductById(int id)
    {
        var product = await productRepository.GetProductById(id) ?? throw new ProductNotFoundException();
        var productView = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            StoreId = product.Store.Id,
            CategoryId = product.Category?.Id,
            CategoryName = product.Category?.Name,
            StoreName = product.Store.Name,
            ImageUrl = product.ImageUrl,
        };
        return productView;
    }
}
