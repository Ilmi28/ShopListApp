using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopListApp.Core.Responses
{
    public class LoginRegisterResponse
    {
        public required string IdentityToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
