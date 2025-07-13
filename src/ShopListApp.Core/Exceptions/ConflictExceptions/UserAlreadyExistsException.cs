using ShopListApp.Core.Exceptions.BaseExceptions;

namespace ShopListApp.Core.Exceptions;

public class UserAlreadyExistsException(string message) : ConflictException(message)
{
}
