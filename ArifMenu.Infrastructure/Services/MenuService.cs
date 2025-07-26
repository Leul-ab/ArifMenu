using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArifMenu.Application.DTOs;
using ArifMenu.Application.Interfaces;
using ArifMenu.Domain.Entities;
using ArifMenu.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace ArifMenu.Infrastructure.Services
{
    public class MenuService : IMenuService
    {
        private readonly ArifMenuDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MenuService(ArifMenuDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }




        public async Task<MenuResponse> AddMenuAsync(Guid userId, MenuRequest request)
        {
            // Save image
            var folderPath = Path.Combine("Uploads",userId.ToString());
            var fullFolderPath = Path.Combine(_env.WebRootPath, folderPath);
            Directory.CreateDirectory(fullFolderPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImageFile.FileName);
            var filePath = Path.Combine(fullFolderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.ImageFile.CopyToAsync(stream);
            }

            var imageUrl = Path.Combine(folderPath, fileName).Replace("\\", "/");


            var merchant = await _context.Merchants
               .FirstOrDefaultAsync(m => m.UserId == userId);

            var menu = new Menu
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                MerchantId = merchant.Id,
                CategoryId = request.CategoryId,
                Price = request.Price,
                Ingredients = request.Ingredients,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow,
                IsSpecial = request.IsSpecial
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return new MenuResponse
            {
                Id = menu.Id,
                Name = menu.Name,
                Price = menu.Price,
                Ingredients = menu.Ingredients,
                ImageUrl = menu.ImageUrl,
                IsSpecial =false
            };
        }

        public async Task<PaginatedResponse<MenuResponse>> GetMyMenusAsync(Guid merchantUserId, int pageNumber, int pageSize)
        {
            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(m => m.UserId == merchantUserId);

            if (merchant == null)
                throw new Exception("Merchant not found");

            var query = _context.Menus
                .Where(m => m.MerchantId == merchant.Id)
                .Include(m => m.Category)
                .OrderByDescending(m => m.CreatedAt);

            var totalCount = await query.CountAsync();

            var menus = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var menuDtos = menus.Select(menu => new MenuResponse
            {
                Id = menu.Id,
                Name = menu.Name,
                Price = menu.Price,
                Ingredients = menu.Ingredients,
                ImageUrl = menu.ImageUrl,
                CreatedAt = menu.CreatedAt,
                IsActive = menu.IsActive,
                IsSpecial = menu.IsSpecial,
                CategoryName = menu.Category?.Name
            }).ToList();

            return new PaginatedResponse<MenuResponse>
            {
                Items = menuDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }



        public async Task<MenuResponse> UpdateMenuAsync(Guid merchantUserId, Guid menuId, UpdateMenuRequest request)
        {
            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(m => m.UserId == merchantUserId);

            if (merchant == null)
                throw new Exception("Merchant not found.");

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == menuId && m.MerchantId == merchant.Id);

            if (menu == null)
                throw new Exception("Menu not found or doesn't belong to this merchant.");

            // Update fields
            menu.Name = request.Name;
            menu.Price = request.Price;
            menu.Ingredients = request.Ingredients;
            menu.CategoryId = request.CategoryId;

            // Update image if a new one is uploaded
            if (request.ImageFile != null)
            {
                // Delete old image
                var oldImagePath = Path.Combine(_env.WebRootPath, menu.ImageUrl);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // Save new image
                var folderPath = Path.Combine("Uploads", merchant.Id.ToString());
                var fullFolderPath = Path.Combine(_env.WebRootPath, folderPath);
                Directory.CreateDirectory(fullFolderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImageFile.FileName);
                var filePath = Path.Combine(fullFolderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.ImageFile.CopyToAsync(stream);
                }

                var imageUrl = Path.Combine(folderPath, fileName).Replace("\\", "/");
                menu.ImageUrl = imageUrl;
            }

            await _context.SaveChangesAsync();

            return new MenuResponse
            {
                Id = menu.Id,
                Name = menu.Name,
                Ingredients = menu.Ingredients,
                Price = menu.Price,
                ImageUrl = menu.ImageUrl,
                CreatedAt = menu.CreatedAt,
                CategoryName = (await _context.MenuCategories.FindAsync(menu.CategoryId))?.Name
            };
        }


        public async Task SetMenuActiveStatusAsync(Guid merchantUserId, Guid menuId, bool isActive)
        {
            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(m => m.UserId == merchantUserId);

            if (merchant == null)
                throw new Exception("Merchant not found.");

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == menuId && m.MerchantId == merchant.Id);

            if (menu == null)
                throw new Exception("Menu not found or doesn't belong to this merchant.");

            menu.IsActive = isActive;
            await _context.SaveChangesAsync();
        }




        public async Task SetMenuSpecialStatusAsync(Guid merchantUserId, Guid menuId, bool isSpecial)
        {
            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(m => m.UserId == merchantUserId);
            if (merchant == null) throw new Exception("Merchant not found.");

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == menuId && m.MerchantId == merchant.Id);
            if (menu == null) throw new Exception("Menu not found or does not belong to the merchant.");

            menu.IsSpecial = isSpecial;
            await _context.SaveChangesAsync();
        }


        public async Task<List<MenuResponse>> SearchMyMenusAsync(Guid merchantUserId, string searchText)
        {
            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(m => m.UserId == merchantUserId);

            if (merchant == null)
                throw new Exception("Merchant not found");

            var menus = await _context.Menus
                .Where(m => m.MerchantId == merchant.Id &&
                       (m.Name.ToLower().Contains(searchText.ToLower()) ||
                        m.Category.Name.ToLower().Contains(searchText.ToLower())))
                .Include(m => m.Category)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return menus.Select(menu => new MenuResponse
            {
                Id = menu.Id,
                Name = menu.Name,
                Price = menu.Price,
                Ingredients = menu.Ingredients,
                ImageUrl = menu.ImageUrl,
                CreatedAt = menu.CreatedAt,
                IsActive = menu.IsActive,
                IsSpecial = menu.IsSpecial,
                CategoryName = menu.Category?.Name
            }).ToList();
        }


        public async Task DeleteMenuPermanentlyAsync(Guid userId, Guid menuId)
        {
            var merchant = await _context.Merchants.FirstOrDefaultAsync(m => m.UserId == userId);
            if (merchant == null)
                throw new Exception("Merchant not found.");

            var menu = await _context.Menus.FirstOrDefaultAsync(m => m.Id == menuId && m.MerchantId == merchant.Id);
            if (menu == null)
                throw new Exception("Menu not found or doesn't belong to this merchant.");

            // Delete image file
            if (!string.IsNullOrEmpty(menu.ImageUrl))
            {
                //"Uploads/{merchantUserId}/{filename}"
                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var imagePath = Path.Combine(rootPath, menu.ImageUrl.Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
        }


    }

}
