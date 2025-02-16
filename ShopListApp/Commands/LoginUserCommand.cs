namespace ShopListApp.Commands
{
    public class LoginUserCommand
    {
        public required string UserIdentifier { get; set; }
        public required string Password { get; set; }
    }
}
