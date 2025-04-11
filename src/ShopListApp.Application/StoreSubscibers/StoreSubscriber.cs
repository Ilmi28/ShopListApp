using ShopListApp.Core.Commands.Other;
using ShopListApp.Core.Exceptions;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Interfaces.Parsing;
using ShopListApp.Core.Interfaces.StoreObserver;
using ShopListApp.Core.Models;

namespace ShopListApp.Application.StoreSubscibers;

public class StoreSubscriber(IParser parser, IProductRepository productRepository, ICategoryRepository categoryRepository, IStoreRepository storeRepository) : IStoreSubscriber
{
    public async Task Update()
    {
        var productsForDeletion = new HashSet<int>();
        var dbProducts = await productRepository.GetAllProducts();
        foreach ( var dbProduct in dbProducts )
            productsForDeletion.Add(dbProduct.Id);
        var parsedProducts = await parser.GetParsedProducts();
        foreach (var parsedProduct in parsedProducts)
        {
            var existingProduct = await productRepository.GetProductByName(parsedProduct.Name);
            if (existingProduct == null)
            {
                await AddParsedProductToDb(parsedProduct);
            }
            else
            {
                productsForDeletion.Remove(existingProduct.Id);
                await UpdateParsedProductInDb(parsedProduct, existingProduct);
            }
        }
        foreach (var id in productsForDeletion)
            await productRepository.RemoveProduct(id);
    }

    private async Task AddParsedProductToDb(ParseProductCommand cmd)
    {
        var category = await categoryRepository.GetCategoryByName(cmd.CategoryName);
        var store = await storeRepository.GetStoreById(cmd.StoreId) ?? throw new StoreNotFoundException();
        var product = new Product
        {
            Name = cmd.Name,
            Price = cmd.Price,
            Category = category,
            Store = store,
            ImageUrl = cmd.ImageUrl,
        };
        await productRepository.AddProduct(product);
    }

    private async Task<bool> UpdateParsedProductInDb(ParseProductCommand cmd, Product existingProduct)
    {
        var category = await categoryRepository.GetCategoryByName(cmd.CategoryName);
        var store = await storeRepository.GetStoreById(cmd.StoreId) ?? throw new StoreNotFoundException();
        var product = new Product
        {
            Name = cmd.Name,
            Price = cmd.Price,
            Category = category,
            Store = store,
            ImageUrl = cmd.ImageUrl,
        };
        return await productRepository.UpdateProduct(existingProduct.Id, product);
    }
}
