using ArifMenu.Application.DTOs;
using ArifMenu.Domain.Entities;
using ArifMenu.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

public class QrLinkService : IQrLinkService
{
    private readonly ArifMenuDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public QrLinkService(ArifMenuDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CreateOrRegenerateQrLinkResponse> GenerateOrGetQrLinkAsync(Guid merchantUserId)
    {
        var merchant = await _context.Merchants
            .FirstOrDefaultAsync(m => m.UserId == merchantUserId);
        if (merchant == null) throw new Exception("Merchant not found");

        var existingLink = await _context.MerchantQrLinks
            .FirstOrDefaultAsync(q => q.MerchantId == merchant.Id);

        string newSlug = Guid.NewGuid().ToString("N")[..12];

        if (existingLink == null)
        {
            // Create a new link
            existingLink = new MerchantQrLink
            {
                MerchantId = merchant.Id,
                QrSlug = newSlug,
                CreatedAt = DateTime.UtcNow,
                ScanCount = 0 
            };

            await _context.MerchantQrLinks.AddAsync(existingLink);
        }
        else
        {
            // DO NOT reset ScanCount
            existingLink.QrSlug = newSlug;
            existingLink.CreatedAt = DateTime.UtcNow;
           
        }

        await _context.SaveChangesAsync();

        // Build full public menu URL
        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host.Value}";

        return new CreateOrRegenerateQrLinkResponse
        {
            QrUrl = $"{baseUrl}/menu/{existingLink.QrSlug}"
        };
    }


    public async Task<List<PublicMenuResponse>> GetPublicMenuBySlugAsync(string slug)
    {
        var qr = await _context.MerchantQrLinks
            .Include(x => x.Merchant)
            .FirstOrDefaultAsync(x => x.QrSlug == slug);

        if (qr == null)
            throw new Exception("Invalid QR link");

        // Increment total counter for legacy or display
        qr.ScanCount++;

        // Log the scan in QrScanLogs for analytics
        var scanLog = new QrScanLog
        {
            MerchantId = qr.MerchantId,
            ScanDate = DateTime.UtcNow
        };

        await _context.QrScanLogs.AddAsync(scanLog);
        await _context.SaveChangesAsync();

        var menus = await _context.Menus
            .Where(m => m.MerchantId == qr.MerchantId && m.IsActive)
            .Include(m => m.Category)
            .Select(m => new PublicMenuResponse
            {
                Category = m.Category.Name,
                Name = m.Name,
                ImageUrl = m.ImageUrl,
                Price = m.Price,
                Ingredients = m.Ingredients,
                IsSpecial = m.IsSpecial
            })
            .ToListAsync();

        return menus;
    }

}
