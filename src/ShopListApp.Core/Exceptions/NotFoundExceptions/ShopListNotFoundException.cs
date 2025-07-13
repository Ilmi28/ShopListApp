using ShopListApp.Core.Exceptions.BaseExceptions;

namespace ShopListApp.Core.Exceptions;

public class ShopListNotFoundException(string message) : NotFoundException(message)
{
}
