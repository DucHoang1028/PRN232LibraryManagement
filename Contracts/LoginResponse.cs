using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Contracts
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public MemberDto? Member { get; set; } // optional - if you return user profile too
    }
}


