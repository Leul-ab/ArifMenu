// ArifMenu.Domain/Entities/Merchant.cs
namespace ArifMenu.Domain.Entities;

public class Merchant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Business Information
    public string BusinessName { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string TradeLicenseNumber { get; set; } = string.Empty;
    public string VatRegistrationNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LogoImageUrl { get; set; } = string.Empty;

    // Contact Info
    public string FullName { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true; // Indicates if the merchant is active or not
    public string? InvitationToken { get; set; }
    public DateTime? InvitationTokenExpiresAt { get; set; }



    // Address
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string SubCity { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public DateTime CreatedAt { get; set; }


    // Relationship
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;



    // ✅ Add navigation properties
    public ICollection<MenuCategory> MenuCategories { get; set; } = new List<MenuCategory>();
    public ICollection<Menu> Menus { get; set; } = new List<Menu>();

}
