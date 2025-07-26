// ArifMenu.Infrastructure/Services/MerchantService.cs
using ArifMenu.Application.DTOs;
using ArifMenu.Application.Helper;
using ArifMenu.Application.Interfaces;
using ArifMenu.Domain.Entities;
using ArifMenu.Domain.Enums;
using ArifMenu.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace ArifMenu.Infrastructure.Services;

public class MerchantService : IMerchantService
{
    private readonly ArifMenuDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailService;

    public MerchantService(ArifMenuDbContext context, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;

    }

    public async Task<MerchantResponse> CreateMerchantAsync(MerchantCreateRequest request)
    {
        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new Exception("A user with this email already exists.");

        // Create User
        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
            Role = UserRole.Merchant
        };

        
        //user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync(); // Save early to get User ID

        var token = Guid.NewGuid().ToString();

        // Create Merchant
        var merchant = new Merchant
        {
            UserId = user.Id,
            BusinessName = request.BusinessName,
            BrandName = request.BrandName,
            TradeLicenseNumber = request.TradeLicenseNumber,
            VatRegistrationNumber = request.VatRegistrationNumber,
            Description = request.Description,
            LogoImageUrl = request.LogoImageUrl,
            FullName = request.FullName,
            MobilePhone = request.MobilePhone,
            Email = request.Email,
            InvitationToken = token,
            InvitationTokenExpiresAt = DateTime.UtcNow.AddDays(2),
            Region = request.Region,
            City = request.City,
            SubCity = request.SubCity,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CreatedAt = DateTime.UtcNow
        };

        _context.Merchants.Add(merchant);
        await _context.SaveChangesAsync();


        var requestUrl = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{requestUrl?.Scheme}://{requestUrl?.Host.Value}";

        var resetLink = $"{baseUrl}/api/merchants/reset-password?token={token}";

        await _emailService.SendEmailAsync(
            merchant.Email,
            "You're invited to ArifMenu!",
            $"Hi {merchant.BusinessName},\n\nYou're invited to ArifMenu.\nPlease set your password using the link below:\n\n{resetLink}\n\nThis link expires in 48 hours."
            );
        return new MerchantResponse
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            BusinessName = merchant.BusinessName,
            BrandName = merchant.BrandName,
            CreatedAt = merchant.CreatedAt
        };
    }

    public async Task<bool> UpdatePasswordAsync(Guid merchantId, MerchantPasswordUpdateRequest request)
    {
        var merchant = await _context.Merchants.Include(m => m.User)
                                               .FirstOrDefaultAsync(m => m.UserId == merchantId);

        if (merchant == null || merchant.User == null)
            return false;

        var user = merchant.User;
        var hasher = new PasswordHasher<User>();

        // ✅ Verify the current password
        var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
        if (verificationResult != PasswordVerificationResult.Success)
            return false; // ❌ Incorrect current password

        // ✅ Update to new hashed password
        user.PasswordHash = hasher.HashPassword(user, request.NewPassword);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }


    public async Task ResetPasswordUsingTokenAsync(string token, string newPassword)
    {
        var merchant = await _context.Merchants
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.InvitationToken == token);

        if (merchant == null)
            throw new Exception("Invalid invitation token.");

        if (merchant.InvitationTokenExpiresAt < DateTime.UtcNow)
            throw new Exception("Invitation token has expired.");

        var user = merchant.User;

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, newPassword);

        // Clear token to prevent reuse
        merchant.InvitationToken = null;
        merchant.InvitationTokenExpiresAt = null;

        await _context.SaveChangesAsync();
    }




    public async Task<PagedResult<MerchantResponse>> GetAllMerchantsAsync(int limit = 10, int offset = 0)
    {
        var query = _context.Merchants.Include(m => m.User);

        // Total count before pagination
        var totalCount = await query.CountAsync();

        // Apply limit and offset
        var merchants = await query
            .OrderBy(m => m.CreatedAt) // optional, but useful for consistent results
            .Skip(offset)
            .Take(limit)
            .Select(m => new MerchantResponse
            {
                Id = m.User.Id,
                Username = m.User.UserName,
                Email = m.Email,
                BusinessName = m.BusinessName,
                BrandName = m.BrandName,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<MerchantResponse>
        {
            Items = merchants,
            TotalCount = totalCount
        };
    }

    public async Task<Merchant?> GetByIdAsync(Guid id)
    {
        return await _context.Merchants
            .Include(m => m.User)  // optional if you want the user loaded
            .FirstOrDefaultAsync(m => m.UserId == id);
    }


    public async Task<MerchantResponse?> GetMerchantByIdAsync(Guid id)
    {
        var merchant = await _context.Merchants
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.UserId == id);

        if (merchant == null) return null;

        return new MerchantResponse
        {
            Id = merchant.User.Id,
            Username = merchant.User.UserName,
            Email = merchant.Email,
            BusinessName = merchant.BusinessName,
            BrandName = merchant.BrandName,
            IsActive = merchant.IsActive,
            CreatedAt = merchant.CreatedAt
        };
    }


    public async Task<IEnumerable<MerchantResponse>> SearchMerchantsByBusinessNameAsync(string businessName)
    {
        var merchants = await _context.Merchants
            .Where(m => m.BusinessName.Contains(businessName))
            .Include(m => m.User)
            .ToListAsync();

        return merchants.Select(m => new MerchantResponse
        {
            Id = m.UserId,
            BusinessName = m.BusinessName,
            Email = m.User?.Email,
            IsActive = m.IsActive,
            //LogoPath = m.LogoPath
        });
    }


    public async Task<IEnumerable<MerchantResponse>> GetActiveMerchantsAsync()
    {
        return await _context.Merchants
            .Include(m => m.User)
            .Where(m => m.IsActive)
            .Select(m => new MerchantResponse
            {
                Id = m.User.Id,
                Username = m.User.UserName,
                Email = m.Email,
                BusinessName = m.BusinessName,
                BrandName = m.BrandName,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();
    }




    public async Task<IEnumerable<MerchantResponse>> GetInactiveMerchantsAsync()
    {
        return await _context.Merchants
            .Include(m => m.User)
            .Where(m => !m.IsActive)
            .Select(m => new MerchantResponse
            {
                Id = m.User.Id,
                Username = m.User.UserName,
                Email = m.Email,
                BusinessName = m.BusinessName,
                BrandName = m.BrandName,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync();
    }


    public async Task<MerchantResponse?> UpdateMerchantAsync(Guid id, MerchantCreateRequestWithFile request)
    {
        var merchant = await _context.Merchants.Include(m => m.User).FirstOrDefaultAsync(m => m.UserId == id);
        if (merchant == null) return null;

        // Update User fields
        merchant.User.UserName = request.Username;
        merchant.User.Email = request.Email;

        // Update password if provided
        //if (!string.IsNullOrWhiteSpace(request.Password))
        //{
        //    merchant.User.PasswordHash = new PasswordHasher<User>().HashPassword(merchant.User, request.Password);
        //}

        // Update Merchant fields
        merchant.BusinessName = request.BusinessName;
        merchant.BrandName = request.BrandName;
        merchant.TradeLicenseNumber = request.TradeLicenseNumber;
        merchant.VatRegistrationNumber = request.VatRegistrationNumber;
        merchant.Description = request.Description;
        merchant.FullName = request.FullName;
        merchant.MobilePhone = request.MobilePhone;
        merchant.Region = request.Region;
        merchant.City = request.City;
        merchant.SubCity = request.SubCity;
        merchant.Latitude = request.Latitude;
        merchant.Longitude = request.Longitude;

        // Handle logo file upload (if provided)
        if (request.LogoFile != null && request.LogoFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(request.LogoFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.LogoFile.CopyToAsync(fileStream);
            }

            merchant.LogoImageUrl = Path.Combine("Uploads", uniqueFileName).Replace("\\", "/");
        }

        await _context.SaveChangesAsync();

        return new MerchantResponse
        {
            Id = merchant.User.Id,
            Username = merchant.User.UserName,
            Email = merchant.Email,
            BusinessName = merchant.BusinessName,
            BrandName = merchant.BrandName,
            CreatedAt = merchant.CreatedAt
        };
    }


    public async Task<bool> SetMerchantStatusesAsync(List<Guid> merchantIds, bool isActive)
    {
        if (merchantIds == null || merchantIds.Count == 0)
            return false;

        var merchants = await _context.Merchants
                                    .Where(m => merchantIds.Contains(m.UserId))
                                    .ToListAsync();

        if (merchants.Count == 0)
            return false;

        foreach (var merchant in merchants)
        {
            merchant.IsActive = isActive;
        }

        _context.Merchants.UpdateRange(merchants);
        await _context.SaveChangesAsync();
        return true;
    }







}
