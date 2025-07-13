namespace ShopListApp.Core.Exceptions;

public class UserWithEmailAlreadyExistsException(string message) : UserAlreadyExistsException(message)
{
}
