using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Contracts;
using DataAccessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext dbContext,
        IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = dbContext;
            _configuration = configuration;
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> RegisterUserAsync(RegisterRequest request)
        {
            var identityUser = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var createResult = await _userManager.CreateAsync(identityUser, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                return (false, errors);
            }

            if (!await _roleManager.RoleExistsAsync(request.Role))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(request.Role));
                if (!roleResult.Succeeded)
                    return (false, "Failed to create role.");
            }

            var assignResult = await _userManager.AddToRoleAsync(identityUser, request.Role);
            if (!assignResult.Succeeded)
                return (false, "Failed to assign role.");

            var member = new Member
            {
                MemberId = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                JoinDate = DateTime.UtcNow,
                Role = request.Role,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<LoginResponse> LoginUserAsync(LoginRequest request)
        {
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];
            var adminRole = _configuration["AdminAccount:Role"];

            Console.WriteLine("email of request: " + request.Email);
            Console.WriteLine("email of admin: " + adminEmail);

            if (request.Email == adminEmail && request.Password == adminPassword)
            {
                var adminMember = new MemberDto
                {
                    MemberId = Guid.Empty,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "",
                    Role = "Admin",
                    Phone = "",
                    Address = "",
                    JoinDate = DateTime.UtcNow,
                    LibraryCardNumber = "",
                    Barcode = ""
                };

                return GenerateJwtToken(adminEmail, adminRole, adminMember);
            }

            // Regular users
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return new LoginResponse { Success = false, Message = "Invalid credentials." };
            }
            var userFromDatabase = await _context.Members.Where(u => u.Email == request.Email).FirstOrDefaultAsync();

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "Member";

            var memberDto = new MemberDto
            {
                MemberId = userFromDatabase.MemberId,
                Email = user.Email,
                FirstName = userFromDatabase.FirstName,
                LastName = userFromDatabase.LastName,
                Role = userRole,
                Phone = user.PhoneNumber,
                Address = userFromDatabase.Address,
                JoinDate = userFromDatabase.JoinDate,
                LibraryCardNumber = userFromDatabase.LibraryCardNumber,
                Barcode = userFromDatabase.Barcode,
            };

            return GenerateJwtToken(user.Email, userRole, memberDto);
        }


        private LoginResponse GenerateJwtToken(string email, string role, MemberDto? member = null)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var expiresInMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"]);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
    };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: credentials);

            return new LoginResponse
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                Role = role,
                Message = "Login successful",
                Member = member
            };
        }

    }
}
