using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Services
{
    public interface IAuthService
    {
        Task<bool> RegisterUser(RegisterRequest user);
    }
}
