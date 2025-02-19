using Microsoft.EntityFrameworkCore;
using ShopListApp.Database;
using ShopListApp.Interfaces;
using ShopListApp.Models;

namespace ShopListApp.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ShopListDbContext _context;
        public TokenRepository(ShopListDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddToken(Token token)
        {
            var user = await _context.Users.FindAsync(token.UserId);
            if (user == null)
                return false;
            var lastToken = _context.Tokens.FirstOrDefault(x => x.UserId == token.UserId && !x.IsRevoked);
            if (lastToken != null)
                lastToken.IsRevoked = true;
            await _context.Tokens.AddAsync(token);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Token?> GetToken(string refreshToken)
        { 
            return await _context.Tokens.FirstOrDefaultAsync(x => x.RefreshTokenHash == refreshToken && x.ExpirationDate > DateTime.Now && !x.IsRevoked);
        }
    }
}
