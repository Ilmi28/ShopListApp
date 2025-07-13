using ShopListApp.Core.Exceptions.BaseExceptions;

namespace ShopListApp.Core.Exceptions;

public class ProductNotFoundException (string message) : NotFoundException(message)
{
}
