using CareBox.BLL.DTOs.Products;
using CareBox.BLL.DTOs.ProductsDto;
using CareBox.BLL.Services.ProductManagementService;
using CareBox.BLL.Services.ProductManagementService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CareBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductManagementService _productManagementService; // استبدل بـ Interface الخاص بمنتجاتك لو اسمه مختلف

        public ProductsController(IProductManagementService productManagementService)
        {
            _productManagementService = productManagementService;
        }

        #region Helper
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new Exception("Invalid Token or User found");
            return int.Parse(userIdClaim.Value);
        }
        #endregion

        #region Add Product

        [HttpPost("Provider/add-product")]
        [Authorize(Roles = "SERVICEPROVIDER")]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductDto dto)
        {
            // 1. التأكد إن الداتا اللي جاية سليمة بناءً على الـ DataAnnotations في الـ DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // 2. استخراج الـ User ID من التوكن (JWT) بتاع الشخص اللي عامل Login
                var userId = GetCurrentUserId();

                // 3. نده السيرفيس اللي انت كتبتها 
                var isCreated = await _productManagementService.CreateProductAsync(userId, dto);

                if (isCreated)
                {
                    return Ok(new { message = "The product has been added successfully!" });
                }

                return BadRequest(new { message = "An error occurred while adding the product, please try again later." });
            }
            catch (Exception ex)
            {
                // 4. مسك أي إيرور (Exception) زي مثلاً "Provider not found" اللي انت عاملها في الـ BLL
                return StatusCode(500, new { message = ex.Message });
            }
        }
        #endregion

        #region Update Product
        [HttpPut("Update-product/{productId}")]
        [Authorize(Roles = "SERVICEPROVIDER")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromForm] UpdateProductDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();

                var result = await _productManagementService.UpdateProductAsync(userId, productId, dto);

                if (result)
                    return Ok(new { success = true, message = "Product updated successfully." });

                return BadRequest(new { success = false, message = "Failed to update product." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Delete Product
        [HttpDelete("Delete-product/{productId}")]
        [Authorize(Roles = "SERVICEPROVIDER")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var result = await _productManagementService.DeleteProductAsync(userId, productId);

                if (result)
                    return Ok(new { success = true, message = "Product deleted successfully." });

                return BadRequest(new { success = false, message = "Failed to delete product." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetCategories
        [HttpGet("Provider/Categories")]
        [Authorize(Roles = "SERVICEPROVIDER")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var userId = GetCurrentUserId();
                var data = await _productManagementService.GetProviderCategoriesAsync(userId);
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex) { return BadRequest(new { success = false, message = ex.Message }); }
        } 
        #endregion


        #region Get Products
        [HttpGet("Provider/my-products")]
        [Authorize(Roles = "SERVICEPROVIDER")]
        public async Task<IActionResult> GetMyProducts([FromQuery] int? categoryId, [FromQuery] int? condition)
        {
            try
            {
                var userId = GetCurrentUserId();

                var products = await _productManagementService.GetProviderProductsAsync(userId, categoryId, condition);
                return Ok(new { success = true, data = products });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetInventory

        [HttpGet("Provider/Inventory")]
        [Authorize(Roles = "SERVICEPROVIDER")]
        public async Task<IActionResult> GetInventory()
        {
            try
            {
                var userId = GetCurrentUserId();
                var data = await _productManagementService.GetInventoryAsync(userId);
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetInventoryStatusSummary
        [HttpGet("Provider/Inventory/Status")]
        [Authorize(Roles = "SERVICEPROVIDER")]
        public async Task<IActionResult> GetInventoryStatusSummary()
        {
            try
            {
                var userId = GetCurrentUserId();
                var data = await _productManagementService.GetInventoryStatusSummaryAsync(userId);

                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region UpdateStock
        [HttpPatch("Provider/UpdateProductCountStock/{productId}")]
        [Authorize(Roles = "SERVICEPROVIDER")]
        public async Task<IActionResult> UpdateStock(int productId, [FromBody] UpdateStockDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();

                var result = await _productManagementService.UpdateProductStockAsync(userId, productId, dto.NewQuantity);

                if (result)
                    return Ok(new { success = true, message = "Stock quantity updated successfully." });

                return BadRequest(new { success = false, message = "Failed to update stock." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion



        



        #region Client Product Search
        [HttpPost("SearchProducts")]
        [Authorize(Roles = "CLIENT")]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] double lat,
            [FromQuery] double lon,
            [FromBody] ProductSearchRequestDto request)
        {
            try
            {
                var clientId = GetCurrentUserId();

                // تمرير الـ lat والـ lon للـ Service
                var results = await _productManagementService.SearchProductsForClientAsync(clientId, request, lat, lon);

                if (!results.Any())
                {
                    return Ok(new
                    {
                        message = "No products matching your search criteria were found.",
                        data = results // هترجع [] عشان الموبايل ميضربش إيرور لو متوقع مصفوفة
                    });
                }

                return Ok(new { success = true, data = results });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Get Category Filter Options
        [HttpGet("CategoryFilterOptions/{categoryId}")]
        [Authorize(Roles = "CLIENT")]
        public async Task<IActionResult> GetCategoryFilterOptions(int categoryId)
        {
            try
            {
                var data = await _productManagementService.GetCategoryFilterOptionsAsync(categoryId);

                // إرجاع الـ DTO مباشرة للحصول على شكل الـ JSON المطلوب
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                // في حالة الخطأ فقط نرجع شكل مختلف يوضح المشكلة
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Get Product Positions By Name
        [HttpGet("positions-by-name")]
        [AllowAnonymous] // أو [Authorize(Roles = "CLIENT")] حسب رغبتك
        public async Task<IActionResult> GetPositionsByName([FromQuery] string productName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productName))
                    return BadRequest(new { message = "No Product name" });

                var data = await _productManagementService.GetProductPositionsByNameAsync(productName);

                // إرجاع الـ DTO مباشرة ليظهر الـ JSON بالشكل المسطح المطلوب
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GetCategories
        [HttpGet("Categories")]
        [Authorize(Roles = "CLIENT")]
        public async Task<IActionResult> GetForClientCategories()
        {
            try
            {
                var userId = GetCurrentUserId();
                var data = await _productManagementService.GetClientCategoriesAsync(userId);
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex) { return BadRequest(new { success = false, message = ex.Message }); }
        }
        #endregion

    }
}
