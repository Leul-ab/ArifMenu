using System.Security.Claims;
using ArifMenu.Application.DTOs;
using ArifMenu.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArifMenu.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Merchant")]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private Guid GetMerchantId() =>
            Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

       
        [HttpPost("add-category")]
        public async Task<IActionResult> AddCategory([FromForm] CategoryRequest request)
        {
            var merchantId = GetMerchantId();
            var result = await categoryService.AddCategoryAsync(merchantId, request);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMyCategories([FromQuery] string query)
        {
            var merchantId = GetMerchantId();
            var result = await categoryService.SearchMyCategoriesAsync(merchantId, query);
            return Ok(result);
        }

        [HttpGet("my-categories")]
        public async Task<IActionResult> GetMyCategories()
        {
            var merchantId = GetMerchantId();
            var categories = await categoryService.GetMyCategoriesAsync(merchantId);
            return Ok(categories);
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromForm] CategoryRequest request)
        {
            var merchantId = GetMerchantId();
            var result = await categoryService.UpdateCategoryAsync(merchantId, categoryId, request);
            return Ok(result);
        }

       

        [HttpPut("set-active/{categoryId}")]
        public async Task<IActionResult> SetCategoryActiveStatus(Guid categoryId, [FromQuery] bool isActive)
        {
            var merchantId = GetMerchantId();
            await categoryService.SetCategoryActiveStatusAsync(merchantId, categoryId, isActive);
            return Ok(new { message = $"Category {(isActive ? "activated" : "deactivated")} successfully." });
        }



        [HttpDelete("{categoryId}/permanent")]
        public async Task<IActionResult> PermanentDeleteCategory(Guid categoryId)
        {
            var merchantId = GetMerchantId();
            await categoryService.DeleteCategoryPermanentlyAsync(merchantId, categoryId);
            return Ok(new { message = "Category permanently deleted." });
        }
    }
}
