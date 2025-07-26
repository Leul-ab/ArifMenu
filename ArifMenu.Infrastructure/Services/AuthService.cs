using ArifMenu.Application.DTOs;
using ArifMenu.Application.Helper;
using ArifMenu.Application.Interfaces;
using ArifMenu.Application.Services;
using ArifMenu.Domain.Entities;
using ArifMenu.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ArifMenu.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ArifMenuDbContext _context;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;
    private readonly PasswordHasherService _passwordHasher;

    public AuthService(ArifMenuDbContext context, IConfiguration config, IEmailService emailService, PasswordHasherService passwordHasher)
    {
        _context = context;
        _config = config;
        _emailService = emailService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return null;

        //var hasher = new PasswordHasher<User>();
        //var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        //if (result != PasswordVerificationResult.Success)
        //    return null;

        if (!_passwordHasher.VerifyPassword(user, request.Password))
            return null;


        var token = GenerateJwtToken(user);

        return new LoginResponse
        {
            Token = token,
            UserName = user.UserName,
            Role = user.Role.ToString()
        };
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiryMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> SendPasswordResetOtpAsync(SendOtpRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return false;

        var otp = new Random().Next(100000, 999999).ToString();
        user.PasswordResetOtp = otp;
        user.OtpGeneratedAt = DateTime.UtcNow;
        user.IsOtpVerified = false;

        await _context.SaveChangesAsync();

        // Generate styled HTML using template
        string htmlBody = EmailTemplates.GenerateOtpHtml(user.UserName ?? "User", otp);

        // Send email
        await _emailService.SendEmailAsync(user.Email, "ArifMenu OTP Code", htmlBody);

        return true;
    }



    public async Task<bool> VerifyOtpAsync(VerifyOtpRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || user.PasswordResetOtp != request.Otp)
            return false;

        if (user.OtpGeneratedAt == null || (DateTime.UtcNow - user.OtpGeneratedAt.Value).TotalMinutes > 10)
            return false; // OTP expired

        user.IsOtpVerified = true;
        await _context.SaveChangesAsync();

        return true;
    }


    public async Task<bool> ResetPasswordAsync(string email, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || !user.IsOtpVerified)
            return false;

        //var hasher = new PasswordHasher<User>();
        //user.PasswordHash = hasher.HashPassword(user, newPassword);

        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);


        user.IsOtpVerified = false;
        user.PasswordResetOtp = null;
        user.OtpGeneratedAt = null;

        await _context.SaveChangesAsync();
        return true;
    }



}
