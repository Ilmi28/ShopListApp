using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopListApp.Models;

namespace ShopListApp.Database
{
    public class ShopListUserStore : UserStore<User>
    {
        public ShopListUserStore(ShopListDbContext context) : base(context) { }

        public override Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return Users.Where(u => !u.IsDeleted).FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }

        public override Task<User?> FindByNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return Users.Where(u => !u.IsDeleted).FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        }

        public override Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Users.Where(u => !u.IsDeleted).FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}
