using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ProductsDto
{
    public class ProductSearchResultDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProviderName { get; set; }

        public string Condition { get; set; }
        public string StockStatus { get; set; }
        
        public decimal Price { get; set; }

        // المسافة بالكيلومتر
        public double DistanceKm { get; set; }
    }
}
