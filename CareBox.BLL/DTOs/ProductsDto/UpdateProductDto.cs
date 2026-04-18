using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace CareBox.BLL.DTOs.Products
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public string Make { get; set; } = null!;
        public string ForModel { get; set; } = null!;
        public short Year { get; set; }

        public int Condition { get; set; }
        public int? HorizontalPosition { get; set; }
        public int? VerticalPosition { get; set; }

        public IFormFile? Image { get; set; }

        public string? CategoryName { get; set; }
    }
}
