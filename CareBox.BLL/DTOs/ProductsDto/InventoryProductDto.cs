using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.Products
{
    public class InventoryProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string? CategoryName { get; set; }
        public string Status { get; set; } = null!; // (In Stock, Low Stock, Out of Stock)
        public int CurrentStock { get; set; }
        public string LastUpdate { get; set; } = null!; // سنرسله كنص منسق
    }

}
