using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ArifMenu.Application.DTOs
{
    public class MenuRequest
    {
        public string Name { get; set; } = default!;
        public Guid CategoryId { get; set; } = default;
        public decimal Price { get; set; } = default;
        public string Ingredients { get; set; } = default!;
        public IFormFile ImageFile { get; set; } = default!;
        public bool IsSpecial { get; set; } = false; 

    }

}
