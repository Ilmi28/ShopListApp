using ShopListApp.Core.Exceptions.BaseExceptions;

namespace ShopListApp.Core.Exceptions;

public class FetchingErrorException (string message) : InternalServerErrorException(message)
{
}
