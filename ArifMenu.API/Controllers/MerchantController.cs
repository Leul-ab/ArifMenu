using ArifMenu.Application.DTOs;
using ArifMenu.Application.Interfaces;
using ArifMenu.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArifMenu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Merchant")]
    public class MerchantController : ControllerBase
    {
        private readonly IMerchantService _merchantService;

        public MerchantController(IMerchantService merchantService)
        {
            _merchantService = merchantService;
        }

        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var merchantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (merchantIdClaim == null)
                return Unauthorized();

            var merchantId = Guid.Parse(merchantIdClaim);
            var merchant = await _merchantService.GetMerchantByIdAsync(merchantId);

            if (merchant == null)
                return NotFound();

            return Ok(merchant);
        }

        
        [HttpPut("me/update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] MerchantPasswordUpdateRequest request)
        {
            var merchantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (merchantIdClaim == null) return Unauthorized();

            var merchantId = Guid.Parse(merchantIdClaim);
            try
            {
                var result = await _merchantService.UpdatePasswordAsync(merchantId, request);
                if (!result) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _merchantService.ResetPasswordUsingTokenAsync(request.Token, request.NewPassword);
                return Ok(new { message = "Password has been set successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
