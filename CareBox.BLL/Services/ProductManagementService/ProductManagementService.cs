using CareBox.BLL.DTOs.Products;
using CareBox.BLL.Repositories;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.FileServices.Interfaces;
using CareBox.BLL.Services.ProductManagementService.Interfaces;
using CareBox.DAL.Enums;
using CareBox.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CareBox.BLL.Services.ProductManagementService
{
    public class ProductManagementService : IProductManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public ProductManagementService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        #region Add product
        public async Task<bool> CreateProductAsync(int userId, CreateProductDto dto)
        {
            // 1. جلب بيانات الورشة (Provider)
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            int? categoryId = null;

            // 2. 👇 لو فيه اسم Category مبعوث، نطبق لوجيك الإنشاء التلقائي
            if (!string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                // بندور هل الورشة دي عندها Category بالاسم ده فعلاً؟
                var category = await _unitOfWork.ProductCategories.FindAsync(
                    c => c.Name.ToLower() == dto.CategoryName.ToLower() 
                        
                );

                if (category == null)
                {
                    // لو مش موجود، بنكريت واحد جديد للورشة دي
                    category = new ProductCategory
                    {
                        Name = dto.CategoryName,
                        
                    };
                    await _unitOfWork.ProductCategories.AddAsync(category);

                    // لازم نعمل Save هنا عشان ناخد الـ ID بتاعه للمنتج
                    await _unitOfWork.SaveAsync();
                }
                categoryId = category.Id;
            }
            string? imageUrl = null;
            if (dto.Image != null)
            {
                // هنحفظ الصورة في فولدر اسمه "products"
                imageUrl = await _fileService.SaveFileAsync(dto.Image, "products");
            }

            // 3. إنشاء كائن المنتج وربطه بالـ Category والـ Provider
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                Make = dto.Make,
                ForModel = dto.ForModel,
                Year = dto.Year,
                ServiceProviderId = provider.ServiceProviderId,
                ProductCategoryId = categoryId,

                ProductImageUrl = imageUrl,
                CreatedAt =DateTime.Now,
                UpdatedAt=DateTime.Now,

                // تحويل القيم لـ Enums
                Condition = (ProductCondition)dto.Condition,
                StockStatus = dto.StockQuantity == 0 ? StockStatus.OutOfStock :
                              (dto.StockQuantity <=20 ? StockStatus.LowStock : StockStatus.InStock),
                HorizontalPosition = (HorizontalPosition?)dto.HorizontalPosition,
                VerticalPosition = (VerticalPosition?)dto.VerticalPosition
            };

            await _unitOfWork.Products.AddAsync(product);
            return await _unitOfWork.SaveAsync() > 0;
        }
        #endregion


        #region Update Product
        public async Task<bool> UpdateProductAsync(int userId, int productId, UpdateProductDto dto)
        {
            // 1. جلب بيانات الورشة
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) return false;

            // 2. جلب المنتج والتأكد إنه يخص الورشة دي تحديداً 🛡️ (مهم جداً للأمان)
            var product = await _unitOfWork.Products.FindAsync(
                p => p.ProductId == productId && p.ServiceProviderId == provider.ServiceProviderId
            );
            if (product == null) throw new Exception("Product not found or unauthorized.");

            // 3. لوجيك تعديل قسم المنتج (Category)
            if (!string.IsNullOrWhiteSpace(dto.CategoryName))
            {
                var category = await _unitOfWork.ProductCategories.FindAsync(
                    c => c.Name.ToLower() == dto.CategoryName.ToLower() 
                        
                );

                if (category == null)
                {
                    category = new ProductCategory
                    {
                        Name = dto.CategoryName,
                        
                    };
                    await _unitOfWork.ProductCategories.AddAsync(category);
                    await _unitOfWork.SaveAsync();
                }
                product.ProductCategoryId = category.Id;
            }
            else
            {
                // لو بعت الاسم فاضي، معناه إنه عاوز يشيل المنتج من القسم
                product.ProductCategoryId = null;
            }

            if (dto.Image != null)
            {
                if (!string.IsNullOrEmpty(product.ProductImageUrl))
                {
                    _fileService.DeleteFile(product.ProductImageUrl); // مسح الصورة القديمة
                }
                product.ProductImageUrl = await _fileService.SaveFileAsync(dto.Image, "products"); // حفظ الجديدة
            }

            // 4. تحديث باقي البيانات
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.Make = dto.Make;
            product.ForModel = dto.ForModel;
            product.Year = dto.Year;
            product.UpdatedAt = DateTime.Now;

            product.Condition = (ProductCondition)dto.Condition;
            product.HorizontalPosition = (HorizontalPosition?)dto.HorizontalPosition;
            product.VerticalPosition = (VerticalPosition?)dto.VerticalPosition;

            // تحديث حالة المخزن تلقائياً بناءً على العدد الجديد
            product.StockStatus = dto.StockQuantity == 0 ? StockStatus.OutOfStock :
                     (dto.StockQuantity <= 20 ? StockStatus.LowStock : StockStatus.InStock);

            _unitOfWork.Products.Update(product);
            return await _unitOfWork.SaveAsync() > 0;
        } 
        #endregion

        #region Delete Product
        public async Task<bool> DeleteProductAsync(int userId, int productId)
        {
            // 1. جلب بيانات الورشة
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) return false;

            // 2. جلب المنتج مع التأكد من الملكية
            var product = await _unitOfWork.Products.FindAsync(
                p => p.ProductId == productId && p.ServiceProviderId == provider.ServiceProviderId
            );

            if (product == null) throw new Exception("Product not found or unauthorized.");

            // 3. مسح المنتج
            _unitOfWork.Products.Delete(product);
            return await _unitOfWork.SaveAsync() > 0;
        }
        #endregion

        #region Product Category Operations

        // 1. جلب جميع الأقسام الخاصة بالورشة/التاجر فقط
        public async Task<IEnumerable<ProductCategoryResponseDto>> GetProviderCategoriesAsync(int userId)
        {
            // الحصول على بيانات الورشة
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            // جلب الأقسام المرتبطة بهذه الورشة فقط
            var categories = await _unitOfWork.ProductCategories.GetAllAsync();

            // تحويلها لـ DTO
            return categories.Select(c => new ProductCategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name
            });
        }
       
        #endregion


        #region Get Provider Products
        public async Task<IEnumerable<ProductResponseDto>> GetProviderProductsAsync(int userId, int? categoryId = null, int? condition = null)
        {
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            // جلب المنتجات مع تضمين بيانات القسم (Category)
            var products = await _unitOfWork.Products.FindAllAsync(
                p => p.ServiceProviderId == provider.ServiceProviderId,
                new[] { "ProductCategory" }
            );

            // تطبيق الفلترة الاختيارية في الذاكرة أو تحويلها لـ Queryable إذا كان الـ Repo يدعم ذلك
            var filteredProducts = products.AsQueryable();

            if (categoryId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.ProductCategoryId == categoryId.Value);
            }

            if (condition.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => (int)p.Condition == condition.Value);
            }

            return filteredProducts.Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                ImageUrl = p.ProductImageUrl?? "No Image",
                CategoryName = p.ProductCategory != null ? p.ProductCategory.Name : "No Category",
                Condition = p.Condition.ToString(),
                Price = p.Price,
                StockQuantity = p.StockQuantity
            }).ToList();
        }
        #endregion

        #region GetInventory
        public async Task<IEnumerable<InventoryProductDto>> GetInventoryAsync(int userId)
        {
            // 1. جلب بيانات الورشة
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            // 2. جلب المنتجات مع الأقسام وترتيبها بالأحدث تحديثاً
            var products = await _unitOfWork.Products.FindAllAsync(
                p => p.ServiceProviderId == provider.ServiceProviderId,
                new[] { "ProductCategory" }
            );

            // 3. التحويل لـ DTO مع تنسيق التاريخ
            return products
                .OrderByDescending(p => p.UpdatedAt) // الأحدث يظهر أولاً
                .Select(p => new InventoryProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    CategoryName = p.ProductCategory?.Name ?? "No Category",
                    Status = p.StockStatus.ToString(),
                    CurrentStock = p.StockQuantity,
                    LastUpdate = p.UpdatedAt.ToString("yyyy-MM-dd hh:mm tt") // تنسيق التاريخ للفرونت إند
                }).ToList();
        }
        #endregion

        #region GetInventoryStatusSummaryAsync
        public async Task<InventoryStatusDto> GetInventoryStatusSummaryAsync(int userId)
        {
            // 1. جلب بيانات الورشة
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            // 2. جلب جميع منتجات هذه الورشة
            // استخدمنا AsNoTracking أو جلبنا القائمة مباشرة لأننا نحتاج فقط لعمليات عد (Read-only)
            var products = await _unitOfWork.Products.FindAllAsync(
                p => p.ServiceProviderId == provider.ServiceProviderId
            );

            // 3. حساب الإحصائيات بناءً على الـ Enum
            return new InventoryStatusDto
            {
                TotalProducts = products.Count(),
                InStock = products.Count(p => p.StockStatus == StockStatus.InStock),
                LowStock = products.Count(p => p.StockStatus == StockStatus.LowStock),
                OutOfStock = products.Count(p => p.StockStatus == StockStatus.OutOfStock)
            };
        }
        #endregion

        #region UpdateProductStockAsync
        public async Task<bool> UpdateProductStockAsync(int userId, int productId, int newQuantity)
        {
            // 1. التحقق من هوية الورشة
            var provider = await _unitOfWork.ServiceProviders.FindAsync(p => p.AppUserId == userId);
            if (provider == null) throw new Exception("Provider not found.");

            // 2. جلب المنتج والتأكد من ملكيته للورشة
            var product = await _unitOfWork.Products.FindAsync(
                p => p.ProductId == productId && p.ServiceProviderId == provider.ServiceProviderId
            );

            if (product == null) throw new Exception("Product not found or unauthorized.");

            // 3. تحديث الكمية (التأكد أنها ليست سالبة)
            product.StockQuantity = newQuantity < 0 ? 0 : newQuantity;

            // 4. تحديث الحالة تلقائياً بناءً على اللوجيك المتفق عليه
            product.StockStatus = product.StockQuantity == 0 ? StockStatus.OutOfStock :
                                 (product.StockQuantity <= 20 ? StockStatus.LowStock : StockStatus.InStock);

            // 5. تحديث تاريخ آخر تعديل
            product.UpdatedAt = DateTime.Now;

            _unitOfWork.Products.Update(product);
            return await _unitOfWork.SaveAsync() > 0;
        }
        #endregion
    }
}
