using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ShopListApp.Core.Commands.Create;
using ShopListApp.Core.Commands.Update;
using ShopListApp.Core.Dtos;
using ShopListApp.Core.Interfaces.Identity;
using ShopListApp.Core.Models;
using ShopListApp.Core.Responses;
using ShopListApp.Infrastructure.Database.Context;
using ShopListApp.Infrastructure.Database.Identity.AppUser;
using ShopListApp.IntegrationTests.IntegrationTests.WebApplicationFactories;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ShopListApp.IntegrationTests.IntegrationTests;

public class ShopListTests : IClassFixture<ShopListWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ShopListDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ITokenManager _tokenManager;

    public ShopListTests(ShopListWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ShopListDbContext>();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        _tokenManager = scope.ServiceProvider.GetRequiredService<ITokenManager>();
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        SeedData();
    }

    private void SeedData()
    {
        var store = _context.Stores.First(x => x.Id == 1);
        var category1 = _context.Categories.First(x => x.Id == 1);
        var category2 = _context.Categories.First(x => x.Id == 2);
        var user = new User
        {
            Id = "1",
            UserName = "test",
            Email = "test@gmail.com"
        };
        var otherUser = new User
        {
            Id = "2",
            UserName = "test2",
            Email = "test2@gmail.com"
        };
        _userManager.CreateAsync(user, "Password123@");
        _userManager.CreateAsync(otherUser, "Password123@");
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Ziemniak myty jadalny 2 kg", Price = 7.99m, Category = category1, Store = store },
            new Product { Id = 2, Name = "Papryka Czerwona luz", Price = 15.99m, Category = category1, Store = store },
            new Product { Id = 3, Name = "Ogórki szklarniowe kg", Price = 13.99m, Category = category1, Store = store }
        };
        _context.Products.AddRange(products);
        _context.SaveChanges();
        var shopList = new ShopList { Id = 1, Name = "Lista zakupów", UserId = user.Id };
        var shopList2 = new ShopList { Id = 2, Name = "Lista zakupów 2", UserId = otherUser.Id };
        _context.ShopLists.Add(shopList);
        _context.ShopLists.Add(shopList2);
        _context.SaveChanges();
        var shopListProducts = new List<ShopListProduct>
        {
            new ShopListProduct { Id = 1, Product = products[0], ShopList = shopList, Quantity = 2 },
            new ShopListProduct { Id = 2, Product = products[1], ShopList = shopList, Quantity = 1 },
            new ShopListProduct { Id = 4, Product = products[0], ShopList = shopList2, Quantity = 3 }
        };
        _context.ShopListProducts.AddRange(shopListProducts);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateShopList_ValidModel_ReturnsOK()
    {
        var user = await _userManager.FindByIdAsync("1");
        var cmd = new CreateShopListCommand
        {
            Name = "Test shop list"
        };
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PostAsJsonAsync("api/shoplist/create", cmd);
        var dbShopList = _context.ShopLists.FirstOrDefault(x => x.Name == "Test shop list");

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Test shop list", dbShopList!.Name);
    }

    [Theory]
    [InlineData("invalidToken")]
    [InlineData(null!)]
    public async Task CreateShopList_InvalidToken_ReturnsUnathorized(string? token)
    {
        var cmd = new CreateShopListCommand
        {
            Name = "Test shop list"
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var result = await _client.PostAsJsonAsync("api/shoplist/create", cmd);
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Theory]
    [InlineData("1")]
    [InlineData(null!)]
    public async Task CreateShopList_InvalidName_ReturnsBadRequest(string? name)
    {
        var user = await _userManager.FindByIdAsync("1");
        var cmd = new CreateShopListCommand
        {
            Name = name!
        };
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var result = await _client.PostAsJsonAsync("api/shoplist/create", cmd);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task CreateShopList_NullRequest_ReturnsBadRequest()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PostAsJsonAsync("api/shoplist/create", (CreateShopListCommand?)null);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task CreateShopList_UserNotExists_ReturnsBadRequest()
    {
        var cmd = new CreateShopListCommand
        {
            Name = "Test shop list"
        };
        var userDto = new UserDto
        {
            Id = "3",
            UserName = "test",
            Email = "test@gmail.com"
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var result = await _client.PostAsJsonAsync("api/shoplist/create", cmd);
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task DeleteShopList_OwnerAndAuthorized_ReturnsOK()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.DeleteAsync($"api/shoplist/delete/1");

        var dbShopList = _context.ShopLists.Count();
        var dbShopListProducts = _context.ShopListProducts.Count();
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(1, dbShopList);
        Assert.Equal(1, dbShopListProducts);
    }

    [Fact]
    public async Task DeleteShopList_InvalidIdAndAuthorized_ReturnsNotFound()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.DeleteAsync($"api/shoplist/delete/3");

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task DeleteShopList_Unauthorized_ReturnsUnathorized()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalidToken");
        var result = await _client.DeleteAsync($"api/shoplist/delete/1");
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task DeleteShopList_UserNotOwner_ReturnsForbidden()
    {
        var user = await _userManager.FindByIdAsync("2");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.DeleteAsync($"api/shoplist/delete/1");

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task AddProductToShopList_ProductAlreadyInShopListAndAuthorizedWIthQuantity_ReturnsOKAndAddsQuantity()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/add-product/1/1?quantity=2", null!);

        var shopListProduct = _context.ShopListProducts.First(x => x.Product.Id == 1 && x.ShopList.Id == 1);
        await _context.Entry(shopListProduct).ReloadAsync();

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(4, shopListProduct.Quantity);
    }

    [Fact]
    public async Task AddProductToShopList_ProductAlreadyInShopListAndAuthorizedWIthoutQuantity_ReturnsOKAndAddsOneQuantity()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/add-product/1/1", null!);

        var shopListProduct = _context.ShopListProducts.First(x => x.Product.Id == 1 && x.ShopList.Id == 1);
        await _context.Entry(shopListProduct).ReloadAsync();

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(3, shopListProduct.Quantity);
    }

    [Fact]
    public async Task AddProductToShopList_ProductNotInShopListAndAuthorizedWithQuantity_ReturnsOKAndAddsNewShopListProduct()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/add-product/1/3?quantity=2", null!);

        var shopListProduct = _context.ShopListProducts.First(x => x.Product.Id == 3 && x.ShopList.Id == 1);
        int shopListProductCount = _context.ShopListProducts.Count();
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(2, shopListProduct.Quantity);
        Assert.Equal(4, shopListProductCount);
    }

    [Fact]
    public async Task AddProductToShopList_ProductNotInShopListAndAuthorizedWithoutQuantity_ReturnsOKAndAddsNewShopListProductWIthOneQuantity()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/add-product/1/3", null!);

        var shopListProduct = _context.ShopListProducts.First(x => x.Product.Id == 3 && x.ShopList.Id == 1);
        int shopListProductCount = _context.ShopListProducts.Count();
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(1, shopListProduct.Quantity);
        Assert.Equal(4, shopListProductCount);
    }

    [Fact]
    public async Task AddProductToShopList_NonPositiveQuantity_ReturnsBadRequest()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/add-product/1/1?quantity=0", null!);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(1, 4)]
    public async Task AddProductToShopList_ShopListOrProductDoesntExist_ReturnsNotFound(int shopListId, int productId)
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/add-product/{shopListId}/{productId}?quantity=1", null!);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task AddProductToShopList_Unathorized_ReturnsUnathorized()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalidToken");

        var result = await _client.PatchAsync($"api/shoplist/update/add-product/1/1?quantity=1", null!);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task AddProductToShopList_UserNotOwner_ReturnsForbidden()
    {
        var user = await _userManager.FindByIdAsync("2");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/add-product/1/1?quantity=1", null!);

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task RemoveProductFromShopList_ProductInShopListAndAuthorizedWithoutQuantity_ReturnsOKAndRemovesProductEntirely()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/delete-product/1/1", null!);

        var shopListProductCount = _context.ShopListProducts.Count();
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(2, shopListProductCount);
    }

    [Fact]
    public async Task RemoveProductFromShopList_ProductInShopListAndAuthorizedWithQuantity_ReturnsOKAndRemovesProductQuantity()
    {
        var user = await _userManager.FindByIdAsync("2");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/delete-product/2/1?quantity=2", null!);

        var shopListProduct = _context.ShopListProducts.First(x => x.ShopList.Id == 2 && x.Product.Id == 1);
        await _context.Entry(shopListProduct).ReloadAsync();
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(1, shopListProduct.Quantity);
    }

    [Fact]
    public async Task RemoveProductFromShopList_ProductInShopListAndAuthorizedWithoutQuantity_ReturnsOkAndRemovesEntireProduct()
    {
        var user = await _userManager.FindByIdAsync("2");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/delete-product/2/1", null!);

        var shopListProductCount = _context.ShopListProducts.Count();
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(2, shopListProductCount);
    }

    [Fact]
    public async Task RemoveProductFromShopList_ProductNotInShopList_ReturnsNotFound()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/delete-product/1/3", null!);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task RemoveProductFromShopList_ProductInShopListButWrongUser_ReturnsForbidden()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/delete-product/2/1", null!);

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task RemoveProductFromShopList_NonPositiveQuantity_ReturnsBadRequest()
    {
        var user = await _userManager.FindByIdAsync("2");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/delete-product/2/1?quantity=0", null!);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task RemoveProductFromShopList_Unathorized_ReturnsUnathorized()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalidToken");

        var result = await _client.PatchAsync($"api/shoplist/update/delete-product/1/1", null!);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task RemoveProductFromShopList_UserNotOwner_ReturnsForbidden()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PatchAsync($"api/shoplist/update/delete-product/2/1", null!);

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task GetShopList_AuthorizedAndValidId_ReturnsShopList()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.GetAsync("api/shoplist/get/1");

        var shopList = await result.Content.ReadFromJsonAsync<ShopListResponse>();
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Lista zakupów", shopList!.Name);
    }

    [Fact]
    public async Task GetShopList_Unauthorized_ReturnsUnathorized()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalidToken");

        var result = await _client.GetAsync("api/shoplist/get/1");

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task GetShopList_AuthorizedAndShopListDoesntExist_ReturnsNotFound()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var result = await _client.GetAsync("api/shoplist/get/3");
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task GetShopList_UserNotOwner_ReturnsForbidden()
    {
        var user = await _userManager.FindByIdAsync("2");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.GetAsync("api/shoplist/get/1");

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task GetShopList_Unathorized_ReturnsUnathorized()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalidToken");

        var result = await _client.GetAsync("api/shoplist/get/1");

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task UpdateShopList_AuthorizedAndValidModel_ReturnsOK()
    {
        var user = await _userManager.FindByIdAsync("1");
        var cmd = new UpdateShopListCommand
        {
            Name = "Updated shop list"
        };
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PutAsJsonAsync("api/shoplist/update/1", cmd);

        var dbShopList = _context.ShopLists.First(x => x.Id == 1);
        await _context.Entry(dbShopList).ReloadAsync();
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Updated shop list", dbShopList.Name);
    }

    [Fact]
    public async Task UpdateShopList_ShopListNotExists_ReturnsNotFound()
    {
        var user = await _userManager.FindByIdAsync("1");
        var cmd = new UpdateShopListCommand
        {
            Name = "Updated shop list"
        };
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PutAsJsonAsync("api/shoplist/update/3", cmd);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task UpdateShopList_UserNotOwner_ReturnsForbidden()
    {
        var user = await _userManager.FindByIdAsync("2");
        var cmd = new UpdateShopListCommand
        {
            Name = "Updated shop list"
        };
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.PutAsJsonAsync("api/shoplist/update/1", cmd);

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task UpdateShopList_Unathorized_ReturnsUnathorized()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalidToken");
        var cmd = new UpdateShopListCommand
        {
            Name = "Updated shop list"
        };
        var result = await _client.PutAsJsonAsync("api/shoplist/update/1", cmd);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task GetAllShopLists_Authorized_ReturnsShopLists()
    {
        var user = await _userManager.FindByIdAsync("1");
        var userDto = new UserDto
        {
            Id = user!.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };
        string jwtToken = _tokenManager.GenerateAccessToken(userDto!);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var result = await _client.GetAsync("api/shoplist/get-all");

        var shopLists = await result.Content.ReadFromJsonAsync<List<ShopListResponse>>();
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Single(shopLists!);
    }

    [Fact]
    public async Task GetAllShopLists_Unauthorized_ReturnsUnathorized()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalidToken");

        var result = await _client.GetAsync("api/shoplist/get-all");

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }
}
