using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ProductsDto
{
    public class ProductSearchRequestDto
    {
        // بنبعت رقم العربية عشان نجيب الـ (Make, Model, Year) من الداتا بيز بدل ما نخليه يكتبهم
        public int? VehicleId { get; set; }

        public int? CategoryId { get; set; }
        public string? ProductName { get; set; }

        public int? HorizontalPosition { get; set; } // 1 (Front), 2 (Rear)
        public int? VerticalPosition { get; set; }   // 1 (Right), 2 (Left)
    }

}
