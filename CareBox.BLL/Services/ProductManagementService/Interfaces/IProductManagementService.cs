using CareBox.BLL.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.ProductManagementService.Interfaces
{
    public interface IProductManagementService
    {
        Task<bool> CreateProductAsync(int userId, CreateProductDto dto);
        Task<bool> UpdateProductAsync(int userId, int productId, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int userId, int productId);

        Task<IEnumerable<ProductCategoryResponseDto>> GetProviderCategoriesAsync(int userId);



        Task<IEnumerable<ProductResponseDto>> GetProviderProductsAsync(int userId, int? categoryId = null, int? condition = null);
        Task<IEnumerable<InventoryProductDto>> GetInventoryAsync(int userId);
        Task<InventoryStatusDto> GetInventoryStatusSummaryAsync(int userId);

        Task<bool> UpdateProductStockAsync(int userId, int productId, int newQuantity);
    }
}
