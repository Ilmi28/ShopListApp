using ShopListApp.Core.Exceptions.BaseExceptions;

namespace ShopListApp.Core.Exceptions;

public class ShopListProductNotFoundException(string message) : NotFoundException(message)
{
}
