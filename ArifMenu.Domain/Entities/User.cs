using ArifMenu.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; } =  string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } // Assuming Role is a string, you can change it to an enum if needed
        public Merchant? Merchant { get; set; } // Optional, if the user is a merchant
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;




        //forget password otp 

        public string? PasswordResetOtp { get; set; }
        public DateTime? OtpGeneratedAt { get; set; }
        public bool IsOtpVerified { get; set; } = false;


    }
}
