using Microsoft.IdentityModel.Tokens;
using ShopListApp.Commands;
using ShopListApp.Exceptions;
using ShopListApp.Interfaces;
using ShopListApp.Interfaces.IRepositories;
using ShopListApp.Models;
using ShopListApp.Repositories;

namespace ShopListApp.StoreObserver
{
    public class StoreSubscriber : IStoreSubscriber
    {
        private readonly IParser _parser;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStoreRepository _storeRepository;
        public StoreSubscriber(IParser parser, IProductRepository productRepository, ICategoryRepository categoryRepository, IStoreRepository storeRepository)
        {
            _parser = parser;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _storeRepository = storeRepository;
        }
        public async Task Update()
        {
            var productsForDeletion = new HashSet<int>();
            var dbProducts = await _productRepository.GetAllProducts();
            foreach ( var dbProduct in dbProducts )
                productsForDeletion.Add(dbProduct.Id);
            var parsedProducts = await _parser.GetParsedProducts();
            foreach (var parsedProduct in parsedProducts)
            {
                var existingProduct = await _productRepository.GetProductByName(parsedProduct.Name);
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
                await _productRepository.RemoveProduct(id);
        }

        private async Task AddParsedProductToDb(ParseProductCommand cmd)
        {
            var category = await _categoryRepository.GetCategoryByName(cmd.CategoryName);
            var store = await _storeRepository.GetStoreById(cmd.StoreId) ?? throw new StoreNotFoundException();
            var product = new Product
            {
                Name = cmd.Name,
                Price = cmd.Price,
                CategoryId = category?.Id,
                StoreId = cmd.StoreId,
                ImageUrl = cmd.ImageUrl,
            };
            await _productRepository.AddProduct(product);
        }

        private async Task UpdateParsedProductInDb(ParseProductCommand cmd, Product existingProduct)
        {
            var category = await _categoryRepository.GetCategoryByName(cmd.CategoryName);
            var store = await _storeRepository.GetStoreById(cmd.StoreId) ?? throw new StoreNotFoundException();
            var product = new Product
            {
                Name = cmd.Name,
                Price = cmd.Price,
                CategoryId = category?.Id,
                StoreId = cmd.StoreId,
                ImageUrl = cmd.ImageUrl,
            };
            await _productRepository.UpdateProduct(existingProduct.Id, product);
        }
    }
}
