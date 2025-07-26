using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Application.DTOs
{
    public class MenuResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public string Ingredients { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string? CategoryName { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSpecial { get; set; } = false;
    }

}
