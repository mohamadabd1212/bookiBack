using Microsoft.AspNetCore.Mvc;
using ruhanBack.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ruhanBack.Dtos;
namespace ruhanBack.Controllers
{
    [Route("api/change")]
    [ApiController]
    public class ResetPassword : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ISendEmailRepository _SendEmailRepository;
        private readonly ICheckEmailRepository _CheckEmailRepository;
        private readonly IOtpRepository _OtpRepository;
        private readonly IResetPasswordRepository _ResetPasswordRepositor;

        public ResetPassword(
            ISendEmailRepository SendEmailRepository,
            ICheckEmailRepository CheckEmailRepository,
            IOtpRepository OtpRepository,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            IResetPasswordRepository ResetPasswordRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _configuration = configuration;
            _SendEmailRepository = SendEmailRepository;
            _CheckEmailRepository = CheckEmailRepository;
            _OtpRepository = OtpRepository;
            _ResetPasswordRepositor = ResetPasswordRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("ResetPasswordRequest")]
        public async Task<ActionResult<string>> ResetPasswordRequest([FromBody] EmailDto emaildto)
        {
            var checkEmail = await _CheckEmailRepository.CheckEmail(emaildto.email);
            if (checkEmail == "Exist")
            {
                var otp = await _OtpRepository.GenerateOtp(emaildto.email);

                var key = _configuration["Jwt:SecretKey"];
                if (string.IsNullOrEmpty(key)) return StatusCode(500, "JWT secret key not configured");

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Email, emaildto.email)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(2),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                

                return Ok(new {token=jwtToken });
            }

            return BadRequest(new { message = checkEmail });
        }

        [HttpPost("ValidateResetPasswordRequest")]
        public async Task<ActionResult<string>> ValidateOtpRequest([FromBody] EmailDto emailDto)
        {
           

                var user = await _userManager.FindByEmailAsync(emailDto.email);
                if (user == null)
                {
                    return BadRequest(new { message = "User not found with the provided email." });
                }

                var tokenReset = await _userManager.GeneratePasswordResetTokenAsync(user);

                var key = _configuration["Jwt:SecretKey"];
                if (string.IsNullOrEmpty(key)) return StatusCode(500, "JWT secret key not configured");

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.Email, emailDto.email),
                new Claim(ClaimTypes.Actor, tokenReset)
            }),
                    Expires = DateTime.UtcNow.AddMinutes(2),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
               
                return Ok(new {token= jwtToken});

        }


        [HttpPost("ResetPassword")]
        public async Task<ActionResult<string>> ResetPssword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var result = await _ResetPasswordRepositor.ResetPassword(resetPasswordDto.email, resetPasswordDto.token, resetPasswordDto.password);
            if (result == "Password reset successfully")
            {
                await _SendEmailRepository.SendEmail(resetPasswordDto.email, "Password Reset", "Password reset successfully");
                return Ok();
            }

            return BadRequest(new { message = result });
        }
    }
}
