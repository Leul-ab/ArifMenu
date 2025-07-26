using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Domain.Entities
{
    
        public class MenuCategory
        {
            public Guid Id { get; set; }
            public Guid MerchantId { get; set; }

            public string Name { get; set; } = default!;
            public string? Description { get; set; }
            public bool IsActive { get; set; } = true;
            public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; } // image field


        //navigatin
        public Merchant Merchant { get; set; } = default!;
            public ICollection<Menu> Menus { get; set; } = new List<Menu>();
        }
    }


