using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Application.DTOs
{
    public class MerchantUpdateRequest
    {
        public string Username { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string SubCity { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }


}
