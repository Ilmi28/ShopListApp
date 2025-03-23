using ShopListApp.Enums;

namespace ShopListApp.Interfaces
{
    public interface IDbLogger<T>
    {
        Task Log(Operation operation, T loggedObject);
    }
}
