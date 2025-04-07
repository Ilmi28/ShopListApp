namespace ShopListApp.Core.Responses
{
    public class LoginRegisterResponse
    {
        public required string IdentityToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
