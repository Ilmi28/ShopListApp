namespace ShopListApp.Core.Commands.Auth
{
    public class LoginUserCommand
    {
        public required string UserIdentifier { get; set; }
        public required string Password { get; set; }
    }
}
