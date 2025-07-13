namespace ShopListApp.Core.Exceptions;

public class UserWithUserNameAlreadyExistsException(string message) : UserAlreadyExistsException(message)
{
}
