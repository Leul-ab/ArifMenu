using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArifMenu.Application.DTOs;

namespace ArifMenu.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponse> AddCategoryAsync(Guid merchantId, CategoryRequest request);

        Task<List<CategoryResponse>> GetMyCategoriesAsync(Guid userId);

        Task<List<CategoryResponse>> SearchMyCategoriesAsync(Guid merchantId, string query);


        Task SetCategoryActiveStatusAsync(Guid userId, Guid categoryId, bool isActive);

        Task<CategoryResponse> UpdateCategoryAsync(Guid merchantId, Guid categoryId, CategoryRequest request);


        Task DeleteCategoryPermanentlyAsync(Guid merchantId, Guid categoryId);   // Permanent delete
    }
}
