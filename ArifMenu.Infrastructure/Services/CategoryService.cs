using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArifMenu.Application.DTOs;
using ArifMenu.Application.Interfaces;
using ArifMenu.Domain.Entities;
using ArifMenu.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArifMenu.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ArifMenuDbContext _context;

        public CategoryService(ArifMenuDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryResponse> AddCategoryAsync(Guid userId, CategoryRequest request)
        {
            var merchant = await _context.Merchants.FirstOrDefaultAsync(m => m.UserId == userId);
            if (merchant == null) throw new Exception($"Merchant not found for User ID: {userId}");

            string? imageUrl = null;
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                var currentDir = Directory.GetCurrentDirectory();
                var folderPath = Path.Combine(currentDir, "wwwroot", "Uploads", merchant.Id.ToString(), "Categories");
                Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImageFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.ImageFile.CopyToAsync(stream);
                }

                imageUrl = Path.Combine("Uploads", merchant.Id.ToString(), "Categories", fileName).Replace("\\", "/");
            }

            var category = new MenuCategory
            {
                Id = Guid.NewGuid(),
                MerchantId = merchant.Id,
                Name = request.Name,
                Description = request.Description,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.MenuCategories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl
            };
        }

        public async Task<List<CategoryResponse>> GetMyCategoriesAsync(Guid userId)
        {
            var merchant = await _context.Merchants.FirstOrDefaultAsync(m => m.UserId == userId);
            if (merchant == null) throw new Exception("Merchant not found.");

            var categories = await _context.MenuCategories
                .Where(c => c.MerchantId == merchant.Id && c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
               ImageUrl = c.ImageUrl,
               IsActive=c.IsActive,
               CreatedAt =c.CreatedAt
    }).ToList();
        }


        public async Task<List<CategoryResponse>> SearchMyCategoriesAsync(Guid merchantId, string query)
        {
            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(m => m.UserId == merchantId);
            if (merchant == null)
                throw new Exception("Merchant not found.");

            var categories = await _context.MenuCategories
                .Where(c => c.MerchantId == merchant.Id &&
                            c.IsActive &&
                            c.Name.ToLower().Contains(query.ToLower()))
                .ToListAsync();

            return categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            }).ToList();
        }


        public async Task<CategoryResponse> UpdateCategoryAsync(Guid userId, Guid categoryId, CategoryRequest request)
        {
            var merchant = await _context.Merchants.FirstOrDefaultAsync(m => m.UserId == userId);
            if (merchant == null) throw new Exception("Merchant not found.");

            var category = await _context.MenuCategories.FirstOrDefaultAsync(c => c.Id == categoryId && c.MerchantId == merchant.Id);
            if (category == null) throw new Exception("Category not found or doesn't belong to this merchant.");

            category.Name = request.Name;
            category.Description = request.Description;

            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                // Delete old image
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", category.ImageUrl.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (File.Exists(oldImagePath)) File.Delete(oldImagePath);
                }

                // Save new image
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", merchant.Id.ToString(), "Categories");
                Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.ImageFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.ImageFile.CopyToAsync(stream);
                }

                category.ImageUrl = Path.Combine("Uploads", merchant.Id.ToString(), "Categories", fileName).Replace("\\", "/");
            }

            await _context.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl
            };
        }

        public async Task SetCategoryActiveStatusAsync(Guid userId, Guid categoryId, bool isActive)
        {
            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(m => m.UserId == userId);
            if (merchant == null)
                throw new Exception("Merchant not found.");

            var category = await _context.MenuCategories
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.MerchantId == merchant.Id);
            if (category == null)
                throw new Exception("Category not found or doesn't belong to this merchant.");

            category.IsActive = isActive;
            await _context.SaveChangesAsync();
        }


        public async Task DeleteCategoryPermanentlyAsync(Guid userId, Guid categoryId)
        {
            var merchant = await _context.Merchants.FirstOrDefaultAsync(m => m.UserId == userId);
            if (merchant == null) throw new Exception("Merchant not found.");

            var category = await _context.MenuCategories
                .Include(c => c.Menus)
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.MerchantId == merchant.Id);

            if (category == null) throw new Exception("Category not found or doesn't belong to this merchant.");

            if (category.Menus.Any())
                throw new Exception("Cannot delete category that has menus.");

            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", category.ImageUrl.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }

            _context.MenuCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
