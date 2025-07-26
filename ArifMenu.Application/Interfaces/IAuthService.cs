using ArifMenu.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<bool> SendPasswordResetOtpAsync(SendOtpRequest request);

        Task<bool> VerifyOtpAsync(VerifyOtpRequest request);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
    }

}
