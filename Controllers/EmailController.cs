using Microsoft.AspNetCore.Mvc;
using ruhanBack.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ruhanBack.Dtos;
namespace ruhanBack.Controllers
{
    [Route("api/check")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly ICheckEmailRepository _checkEmailRepository;
        

        public EmailController(ICheckEmailRepository checkEmailRepository,IOtpRepository otpRepository)
        {
            _checkEmailRepository=checkEmailRepository;
        }

        [HttpPost("EmailVerfificationRequest")]
        public async Task<ActionResult<string>> verifyEmailRequest([FromBody] EmailDto emailDto)
        {
        var result = await _checkEmailRepository.CheckEmailVerified(emailDto.email);
        if(result == "Not Verified"){
            await _checkEmailRepository.verifyEmailRequest(emailDto.email);
            return Ok();
        }
        return BadRequest(new{message=result});
        }

         [HttpPost("verifyEmailValidation")]
        public async Task<ActionResult<string>> verifyEmailValidation([FromBody] EmailDto emailDto)
        {
        var result = await _checkEmailRepository.CheckEmailVerified(emailDto.email);
        if(result == "Not Verified"){
            var response=await _checkEmailRepository.verifyEmailValidation(emailDto.email);
            return Ok(new{token = response});
        }
        return BadRequest(new{message=result});
        }
    }

}

