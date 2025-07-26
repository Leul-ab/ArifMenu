using ArifMenu.Application.DTOs;
using ArifMenu.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArifMenu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Invalid login request.");
            }


            var response = await _authService.LoginAsync(request);
            if (response == null)
                return Unauthorized(new { message = "Invalid credentials." });

            return Ok(response);
        }


        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            var result = await _authService.SendPasswordResetOtpAsync(request);
            return result ? Ok("OTP sent.") : NotFound("User not found.");
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var result = await _authService.VerifyOtpAsync(request);
            return result ? Ok("OTP verified.") : BadRequest("Invalid or expired OTP.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromQuery] string email, [FromQuery] string newPassword)
        {
            var result = await _authService.ResetPasswordAsync(email, newPassword);
            return result ? Ok("Password updated.") : BadRequest("OTP not verified or user not found.");
        }


    }
}
