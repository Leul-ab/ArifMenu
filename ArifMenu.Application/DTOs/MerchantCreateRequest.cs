// ArifMenu.Application/DTOs/MerchantCreateRequest.cs
using System.ComponentModel.DataAnnotations;

namespace ArifMenu.Application.DTOs;

public class MerchantCreateRequest
{
    // User Info
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    //[Required, MinLength(6)]
    //public string Password { get; set; } = string.Empty;

    // Business Info
    public string BusinessName { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string TradeLicenseNumber { get; set; } = string.Empty;
    public string VatRegistrationNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LogoImageUrl { get; set; } = string.Empty;

    // Contact & Location Info
    public string FullName { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string SubCity { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
