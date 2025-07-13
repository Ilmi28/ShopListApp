using ShopListApp.Core.Exceptions.BaseExceptions;

namespace ShopListApp.Core.Exceptions;

public class CategoryNotFoundException(string message) : NotFoundException(message)
{
}