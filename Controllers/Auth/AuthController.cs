using Microsoft.AspNetCore.Mvc;
using ruhanBack.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ruhanBack.Dtos;
namespace ruhanBack.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _AuthRepository;

        public AuthController(IAuthRepository AuthRepository)
        {
            _AuthRepository = AuthRepository;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _AuthRepository.Register(registerDto.userName, registerDto.email, registerDto.password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User Registered Successfully" });
            }

            var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { message = errorMessage });
        }

        [HttpPost("Login")]
public async Task<ActionResult<string>> Login([FromBody] LoginDto loginDto)
{
    var result = await _AuthRepository.Login(loginDto.email, loginDto.password);
    if (result == "Invalid Email or Password")
    {
        return BadRequest(new { message = result });
    }

    return Ok(new { token = result });
}

        
    }

}

