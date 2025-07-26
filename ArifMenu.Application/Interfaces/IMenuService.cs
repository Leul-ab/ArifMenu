using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArifMenu.Application.DTOs;

namespace ArifMenu.Application.Interfaces
{
    public interface IMenuService
    {
        Task<MenuResponse> AddMenuAsync(Guid merchantId, MenuRequest request);

        Task<PaginatedResponse<MenuResponse>> GetMyMenusAsync(Guid merchantUserId, int pageNumber, int pageSize);

        Task<MenuResponse> UpdateMenuAsync(Guid merchantUserId, Guid menuId, UpdateMenuRequest request);

        Task SetMenuActiveStatusAsync(Guid merchantUserId, Guid menuId, bool isActive);

        Task DeleteMenuPermanentlyAsync(Guid userId, Guid menuId);

        Task SetMenuSpecialStatusAsync(Guid merchantUserId, Guid menuId, bool isSpecial);// mark special or none

        Task<List<MenuResponse>> SearchMyMenusAsync(Guid merchantUserId, string searchText);


    }

}
