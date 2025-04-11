using Microsoft.EntityFrameworkCore;
using ShopListApp.Core.Interfaces.IRepositories;
using ShopListApp.Core.Models;
using ShopListApp.Infrastructure.Database.Context;

namespace ShopListApp.Infrastructure.Repositories;

public class TokenRepository(ShopListDbContext context) : ITokenRepository
{
    public async Task<bool> AddToken(Token token)
    {
        var user = await context.Users.FindAsync(token.UserId);
        if (user == null)
            return false;
        var lastToken = context.Tokens.FirstOrDefault(x => x.UserId == token.UserId && !x.IsRevoked);
        if (lastToken != null)
            lastToken.IsRevoked = true;
        await context.Tokens.AddAsync(token);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<Token?> GetToken(string refreshToken)
    { 
        return await context.Tokens.FirstOrDefaultAsync(x => x.RefreshTokenHash == refreshToken && x.ExpirationDate > DateTime.Now && !x.IsRevoked);
    }
}
