using ArifMenu.Application.DTOs;
using ArifMenu.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArifMenu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMerchantService _merchantService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(IMerchantService merchantService, IWebHostEnvironment webHostEnvironment)
        {
            _merchantService = merchantService;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpPost("add-merchants")]
        [RequestSizeLimit(10 * 1024 * 1024)] // optional: limit upload size, e.g. 10 MB
        public async Task<IActionResult> CreateMerchant([FromForm] MerchantCreateRequestWithFile request)
        {
            string? logoPath = null;

            if (request.LogoFile != null && request.LogoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.LogoFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.LogoFile.CopyToAsync(fileStream);
                }

                // Store relative path (to use later in the app)
                logoPath = Path.Combine("Uploads", uniqueFileName).Replace("\\", "/");
            }

            // Map fields from request to your existing CreateMerchantAsync call,
            // including logoPath for LogoImageUrl

            var merchant = await _merchantService.CreateMerchantAsync(new MerchantCreateRequest
            {
                Username = request.Username,
                Email = request.Email,
                //Password = request.Password,
                BusinessName = request.BusinessName,
                BrandName = request.BrandName,
                TradeLicenseNumber = request.TradeLicenseNumber,
                VatRegistrationNumber = request.VatRegistrationNumber,
                Description = request.Description,
                FullName = request.FullName,
                MobilePhone = request.MobilePhone,
                Region = request.Region,
                City = request.City,
                SubCity = request.SubCity,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                LogoImageUrl = logoPath
            });


            return Ok(merchant);
        }


        [HttpGet("merchants-list")]
        public async Task<IActionResult> GetMerchants()
        {
            var merchants = await _merchantService.GetAllMerchantsAsync();
            return Ok(merchants);
        }

        [HttpGet("search-merchant-by-name")]
        public async Task<IActionResult> Search([FromQuery] string businessName)
        {
            var results = await _merchantService.SearchMerchantsByBusinessNameAsync(businessName);
            return Ok(results);
        }

        [HttpGet("active-merchants")]
        public async Task<IActionResult> GetActiveMerchants()
        {
            var merchants = await _merchantService.GetActiveMerchantsAsync();
            return Ok(merchants);
        }

        [HttpGet("inactive-merchants")]
        public async Task<IActionResult> GetInactiveMerchants()
        {
            var merchants = await _merchantService.GetInactiveMerchantsAsync();
            return Ok(merchants);
        }

        

        [HttpGet("get-merchant/{id}")]
        public async Task<IActionResult> GetMerchantById(Guid id)
        {
            var merchant = await _merchantService.GetByIdAsync(id);
            if (merchant == null)
                return NotFound();

            return Ok(merchant);
        }

        [HttpPut("update-merchant/{id}")]
        public async Task<IActionResult> UpdateMerchant(Guid id, [FromForm] MerchantCreateRequestWithFile request)
        {
            var updatedMerchant = await _merchantService.UpdateMerchantAsync(id, request);
            if (updatedMerchant == null)
                return NotFound();

            return Ok(updatedMerchant);
        }



        public class MerchantStatusRequest
        {
            public List<Guid> MerchantIds { get; set; } = new();
            public bool IsActive { get; set; }
        }


        [HttpPut("set-status")]
        public async Task<IActionResult> SetMerchantStatuses([FromBody] MerchantStatusRequest request)
        {
            if (request.MerchantIds == null || request.MerchantIds.Count == 0)
                return BadRequest("At least one merchant ID is required.");

            var success = await _merchantService.SetMerchantStatusesAsync(request.MerchantIds, request.IsActive);

            if (!success)
                return NotFound("No matching merchants found.");

            return Ok(new
            {
                Message = $"Merchant{(request.MerchantIds.Count > 1 ? "s" : "")} {(request.IsActive ? "enabled" : "disabled")} successfully."
            });
        }



    }

}
