using ArifMenu.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArifMenu.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Merchant")]
public class MerchantDashboardController : ControllerBase
{
    private readonly IMerchantDashboardService _dashboardService;

    public MerchantDashboardController(IMerchantDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetDashboard()
    {
        var merchantId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var data = await _dashboardService.GetMerchantDashboardAsync(merchantId);
        return Ok(data);
    }
}
