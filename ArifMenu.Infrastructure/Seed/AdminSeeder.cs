using ArifMenu.Application.Services;
using ArifMenu.Domain.Entities;
using ArifMenu.Domain.Enums;
using ArifMenu.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Infrastructure.Seed
{
    public class AdminSeeder
    {
        public static async Task SeedAdminAsync(ArifMenuDbContext context, string username, string email, string password)
        {
            if (!await context.Users.AnyAsync(u => u.Role == Domain.Enums.UserRole.Admin))
            {
                var admin = new User
                {
                    UserName = username,
                    Email = email,
                    Role = UserRole.Admin,
                    
                };

                var hasher = new PasswordHasher<User>();
                admin.PasswordHash = hasher.HashPassword(admin, password);

                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
