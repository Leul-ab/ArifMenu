using ArifMenu.Application.DTOs;
using ArifMenu.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArifMenu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Merchant")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        private Guid GetMerchantId() =>
            Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

      

        [HttpPost("add-menu")]
        public async Task<IActionResult> AddMenu([FromForm] MenuRequest request)
        {
            var merchantId = GetMerchantId();
            var result = await _menuService.AddMenuAsync(merchantId, request);
            return Ok(result);
        }



        [HttpGet("my-menus")]
        public async Task<IActionResult> GetMyMenus([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetMerchantId();
            var result = await _menuService.GetMyMenusAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("search-my-menus")]
        public async Task<IActionResult> SearchMyMenus([FromQuery] string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return BadRequest("Search text cannot be empty.");

            var userId = GetMerchantId();
            var result = await _menuService.SearchMyMenusAsync(userId, search);
            return Ok(result);
        }

        [HttpPut("edit/{menuId}")]
        public async Task<IActionResult> UpdateMenu(Guid menuId, [FromForm] UpdateMenuRequest request)
        {
            // Get merchantId from claims 
            var merchantId = GetMerchantId();

            var updatedMenu = await _menuService.UpdateMenuAsync(merchantId, menuId, request);
            return Ok(updatedMenu);
        }

    



        [HttpPut("deactivate/{menuId}")]
        public async Task<IActionResult> DeactivateMenu(Guid menuId, [FromQuery] bool isActive)
        {
            var merchantId = GetMerchantId(); 
            await _menuService.SetMenuActiveStatusAsync(merchantId, menuId, isActive);
            return Ok(new { message = $"Menu {(isActive ? "activated" : "deactivated")} successfully." });
        }



        [HttpPut("mark-special/{menuId}")]
        public async Task<IActionResult> MarkAsSpecial(Guid menuId, [FromQuery] bool isSpecial)
        {
            var userId = GetMerchantId();
            await _menuService.SetMenuSpecialStatusAsync(userId, menuId, isSpecial);
            return Ok(new { message = $"Menu special status updated to {isSpecial}" });
        }




        [HttpDelete("delete/{menuId}")]
        public async Task<IActionResult> DeleteMenuPermanently(Guid menuId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _menuService.DeleteMenuPermanentlyAsync(userId, menuId);
            return Ok(new { message = "Menu deleted." });
        }


    }



}
