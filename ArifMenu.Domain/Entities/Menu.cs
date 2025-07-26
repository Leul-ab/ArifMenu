using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Domain.Entities
{
  
        public class Menu
        {
            public Guid Id { get; set; }
            public Guid MerchantId { get; set; }
            public Guid CategoryId { get; set; }

            public string Name { get; set; } = default!;
            public string Ingredients { get; set; } = default!;
            public decimal Price { get; set; }
            public string ImageUrl { get; set; } = default!;
            public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSpecial { get; set; } = false; //special or none

        //navigation
        public MenuCategory Category { get; set; } = default!;
            public Merchant Merchant { get; set; } = default!;
        }
    }


