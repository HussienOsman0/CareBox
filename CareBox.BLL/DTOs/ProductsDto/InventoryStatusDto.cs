using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.Products
{
    public class InventoryStatusDto
    {
        public int TotalProducts { get; set; } // إجمالي عدد المنتجات
        public int InStock { get; set; }       // عدد المنتجات المتوفرة
        public int LowStock { get; set; }      // عدد المنتجات التي قاربت على النفاذ
        public int OutOfStock { get; set; }    // عدد المنتجات التي نفذت
    }
}
