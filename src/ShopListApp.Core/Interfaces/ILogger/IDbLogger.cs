using ShopListApp.Enums;

namespace ShopListApp.Core.Interfaces.ILogger
{
    public interface IDbLogger<T>
    {
        Task Log(Operation operation, T loggedObject);
    }
}
