using ShopListApp.Core.Exceptions.BaseExceptions;

namespace ShopListApp.Core.Exceptions;

public class UserNotFoundException(string message) : NotFoundException(message)
{
}
