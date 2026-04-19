using CareBox.BLL.DTOs.Products;
using CareBox.BLL.DTOs.ProductsDto;
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
        Task<IEnumerable<ProductResponseDto>> GetProviderProductsAsync(int userId, int? categoryId = null, int? condition = null);


        Task<IEnumerable<ProductCategoryResponseDto>> GetProviderCategoriesAsync(int userId);
        Task<IEnumerable<ProductCategoryResponseDto>> GetClientCategoriesAsync(int userId);
        Task<CategoryFilterOptionsDto> GetCategoryFilterOptionsAsync(int categoryId);
        Task<ProductPositionsResponseDto> GetProductPositionsByNameAsync(string productName);
        Task<IEnumerable<ProductSearchResultDto>> SearchProductsForClientAsync(int clientId, ProductSearchRequestDto request, double userLat, double userLon);



        Task<IEnumerable<InventoryProductDto>> GetInventoryAsync(int userId);
        Task<InventoryStatusDto> GetInventoryStatusSummaryAsync(int userId);
        Task<bool> UpdateProductStockAsync(int userId, int productId, int newQuantity);
    }
}
