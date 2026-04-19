using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.DTOs.ProductsDto
{
    public class ProductPositionsResponseDto
    {
        public List<int> AvailableHorizontalPositions { get; set; } = new List<int>();
        public List<int> AvailableVerticalPositions { get; set; } = new List<int>();
    }
}
