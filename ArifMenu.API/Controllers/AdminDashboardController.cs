using ArifMenu.Application.DTOs;
using ArifMenu.Application.Enums;
using ArifMenu.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/admin-dashboard")]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public AdminDashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("metrics")]
    public async Task<ActionResult<DashboardMetricsDto>> GetMetrics()
    {
        return Ok(await _dashboardService.GetDashboardMetricsAsync());
    }

    [HttpGet("historical-data")]
    public async Task<IActionResult> GetHistoricalData([FromQuery] GranularityType granularity = GranularityType.Monthly)
    {
        try
        {
            var data = await _dashboardService.GetHistoricalMerchantDataAsync(granularity);
            return Ok(new
            {
                status = "success",
                data
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                status = "error",
                message = ex.Message
            });
        }
    }


}
