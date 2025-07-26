using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArifMenu.Application.DTOs;
using ArifMenu.Application.Interfaces;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class QrLinkController : ControllerBase
{
    private readonly IQrLinkService _qrLinkService;

    public QrLinkController(IQrLinkService qrLinkService)
    {
        _qrLinkService = qrLinkService;
    }

   
    [Authorize(Roles = "Merchant")]
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateQrLink()
    {
        var merchantUserId = GetMerchantUserId(); // from token
        var result = await _qrLinkService.GenerateOrGetQrLinkAsync(merchantUserId);
        return Ok(result);
    }

   
    [HttpGet("/menu/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicMenu(string slug)
    {
        var result = await _qrLinkService.GetPublicMenuBySlugAsync(slug);
        return Ok(result);
    }

    
    private Guid GetMerchantUserId()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("Invalid token: User ID not found.");

        return Guid.Parse(userId);
    }
}
