using ruhanBack.Models;
using ruhanBack.Data;
using Microsoft.AspNetCore.Identity;
using ruhanBack.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ruhanBack.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IdentityResult> Register(string userName, string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "EmailAlreadyExists",
                    Description = "The email address is already in use."
                });
            }

            var user = new IdentityUser
            {
                UserName = userName,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var roleExist = await _roleManager.RoleExistsAsync("User");
                if (!roleExist)
                {
                    var role = new IdentityRole("User");
                    await _roleManager.CreateAsync(role);
                }

                await _userManager.AddToRoleAsync(user, "User");
            }

            return result;
        }

        public async Task<string> Login(string email, string password)
{
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null || !await _userManager.CheckPasswordAsync(user, password))
    {
        return "Invalid Email or Password";
    }
    var verifiedEmail = await _userManager.IsEmailConfirmedAsync(user);

    // Get role IDs and names
    var userRoles = await _context.UserRoles
        .Where(ur => ur.UserId == user.Id)
        .Select(ur => ur.RoleId)
        .ToListAsync();

    if (userRoles == null || userRoles.Count == 0)
    {
        return "User has no roles assigned.";
    }

    var roles = await _context.Roles
        .Where(r => userRoles.Contains(r.Id))
        .Select(r => r.Name)
        .ToListAsync();

    if (roles == null || roles.Count == 0)
    {
        return "No roles found for user.";
    }

    // Create claims
    var claims = new List<System.Security.Claims.Claim>
    {
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Actor, verifiedEmail.ToString())
    };
    

    foreach (var role in roles)
    {
        claims.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role));
    }

    var secretKey = _configuration["Jwt:SecretKey"];
    if (string.IsNullOrEmpty(secretKey))
    {
        throw new InvalidOperationException("JWT Secret Key is not configured.");
    }

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds
    );

    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

    return jwtToken;
}

    }
}
