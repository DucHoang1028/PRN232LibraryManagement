using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Identity;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        public AuthService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> RegisterUser(RegisterRequest user)
        {
            var identityUser = new IdentityUser
            {
                Email = user.Email,
            };
            var result = await _userManager.CreateAsync(identityUser, user.Password);
            return result.Succeeded;
        }
    }
}
