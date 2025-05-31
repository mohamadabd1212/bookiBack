using Microsoft.AspNetCore.Mvc;
using ruhanBack.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ruhanBack.Dtos;
namespace ruhanBack.Controllers
{
    [Route("api/Otp")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IOtpRepository _otpRepository;

        public OtpController(IOtpRepository otpRepository)
        {
            _otpRepository = otpRepository;
        }
        [HttpPost("ResendOtp")]
        public async Task<ActionResult<string>> ResendOtp([FromBody] EmailDto emailDto)
        {
            var result = await _otpRepository.GenerateOtp(emailDto.email);
            if (result == "Error Sending The Email")
                return BadRequest(new { message = "Error Sending The Email" });

            return Ok(new { message = "OTP resent successfully" });
        }

        [HttpPost("ValidateOtp")]
        public async Task<ActionResult<string>> ValidateOtp([FromBody] ValidateOtpDto validateOtpDto)
        {
            try
            {
                var result = await _otpRepository.ValidateOtp(validateOtpDto.email, validateOtpDto.date, validateOtpDto.otp);
                if(result == "Valid")
                return Ok();
                else
                return BadRequest(new {message=result});
            }
            catch (Exception Error)
            {
                return BadRequest(new { message = "Error Sending The Email" });
            }

        }


    }
}

