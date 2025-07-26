using ArifMenu.Application.DTOs;
using ArifMenu.Application.Helper;
using ArifMenu.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArifMenu.Application.Interfaces
{
    public interface IMerchantService
    {
        // Create a merchant (Admin use)
        Task<MerchantResponse> CreateMerchantAsync(MerchantCreateRequest request);

        // Get all merchants
        Task<PagedResult<MerchantResponse>> GetAllMerchantsAsync(int limit = 10, int offset = 0);

        // Get a specific merchant by ID
        Task<MerchantResponse?> GetMerchantByIdAsync(Guid id);

        // Get the Merchant entity (optional internal use)
        Task<Merchant?> GetByIdAsync(Guid id);

        // Update merchant details (Admin use)
        Task<MerchantResponse?> UpdateMerchantAsync(Guid id, MerchantCreateRequestWithFile request);



        // Update only the password (Merchant use)
        Task<bool> UpdatePasswordAsync(Guid merchantId, MerchantPasswordUpdateRequest request);
        Task ResetPasswordUsingTokenAsync(string token, string newPassword);

        // ✅ Add these:
        Task<IEnumerable<MerchantResponse>> GetActiveMerchantsAsync();
        Task<IEnumerable<MerchantResponse>> GetInactiveMerchantsAsync();


        Task<bool> SetMerchantStatusesAsync(List<Guid> merchantIds, bool isActive);

        Task<IEnumerable<MerchantResponse>> SearchMerchantsByBusinessNameAsync(string businessName);

        // Application.Contracts.Interfaces


    }
}
