using ShopListApp.Core.Exceptions.BaseExceptions;

namespace ShopListApp.Core.Exceptions;

public class StoreNotFoundException(string message) : NotFoundException(message)
{
}
