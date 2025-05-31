using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ruhanBack.Interfaces;
using ruhanBack.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;



public class CheckEmailRepository : ICheckEmailRepository
{
    private readonly UserManager<IdentityUser> _userManager;

    private readonly ApplicationDbContext _context;

    private readonly ISendEmailRepository _sendEmailRepository;

    private readonly IConfiguration _configuration;

    private readonly IOtpRepository _otpRepository;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CheckEmailRepository(UserManager<IdentityUser> userManager,ISendEmailRepository sendEmailRepository,IConfiguration configuration,IOtpRepository otpRepository,ApplicationDbContext context,IHttpContextAccessor httpContextAccessor){
        _userManager=userManager;
        _sendEmailRepository=sendEmailRepository;   
        _configuration = configuration;
        _otpRepository = otpRepository;
        _context=context;
        _httpContextAccessor=httpContextAccessor;
        
    }

    public async Task<string> CheckEmail(string email)
{
    var result = await _userManager.FindByEmailAsync(email);
    if (result != null)
        return "Exist";

    return "User Not Found"; 
}

public async Task<string> CheckEmailVerified(string email)
{
    var user = await _userManager.FindByEmailAsync(email);
        var check =await _userManager.IsEmailConfirmedAsync(user);
        if (check == true)
        return "User Already Verified";
        else
        return "Not Verified";
    
}

public async  Task verifyEmailRequest(string email)
{
    var verified=await CheckEmailVerified(email);
    if(verified=="Not Verified"){
    await _otpRepository.GenerateOtp(email);
    }
     
}
    public async Task<string> verifyEmailValidation(string email)
    {
        var verified = await CheckEmailVerified(email);
        if (verified == "Not Verified")
        {
            var user = await _userManager.FindByEmailAsync(email);
            var tokena = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, tokena);
            await _sendEmailRepository.SendEmail(email, "Email Verified", "you have succefully Verified Your Email");
            var verifiedEmail = await _userManager.IsEmailConfirmedAsync(user);

            // Get role IDs and names
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();


            var roles = await _context.Roles
                .Where(r => userRoles.Contains(r.Id))
                .Select(r => r.Name)
                .ToListAsync();


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
        return "no ver";
}
}
