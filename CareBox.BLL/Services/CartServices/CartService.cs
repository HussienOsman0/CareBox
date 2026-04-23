using CareBox.BLL.DTOs.CartDto;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.CartServices.Interface;
using CareBox.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Add to cart
        public async Task<bool> AddToCartAsync(int userId, AddToCartDto dto)
        {
            // 1. جلب بيانات العميل (Client)
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId, new[] { "Cart.CartItems" });
            if (client == null) throw new Exception("Client record not found.");

            // 2. التأكد من وجود سلة للعميل، وإذا لم توجد ننشئ واحدة جديدة
            var cart = client.Cart;
            if (cart == null)
            {
                cart = new Cart { ClientId = client.ClientID };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveAsync(); // حفظ لكي نأخذ الـ CartId
            }

            foreach (var itemDto in dto.Items)
            {
                // 3. التحقق من وجود المنتج وصحة الكمية المطلوبة
                var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);
                if (product == null) continue; // أو يمكن رمي Exception حسب الرغبة

                if (product.StockQuantity < itemDto.Quantity)
                    throw new Exception($"The requested quantity of product {product.Name} is not available in stock.");

                // 4. فحص هل المنتج موجود بالفعل في السلة؟
                var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == itemDto.ProductId);

                if (existingItem != null)
                {
                    // تحديث الكمية (مع التأكد من المخزن مرة أخرى للكمية الإجمالية)
                    if (product.StockQuantity < (existingItem.Quantity + itemDto.Quantity))
                        throw new Exception($"The requested quantity of product {product.Name} is not available in stock.");

                    existingItem.Quantity += itemDto.Quantity;
                }
                else
                {
                    // إضافة عنصر جديد للسلة
                    var newItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity
                    };
                    cart.CartItems.Add(newItem);
                }
            }

            return await _unitOfWork.SaveAsync() > 0;
        }
        #endregion

        #region Remove Single Item From Cart
        public async Task<bool> RemoveItemFromCartAsync(int userId, int productId)
        {
            // 1. جلب بيانات العميل
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            // 2. البحث عن سلة العميل مع العناصر
            var cart = await _unitOfWork.Carts.FindAsync(
                c => c.ClientId == client.ClientID,
                new[] { "CartItems" }
            );

            // لو مفيش سلة أصلاً، مفيش حاجة تتحذف
            if (cart == null) return false;

            // 3. البحث عن المنتج جوه السلة
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null) return false; // المنتج مش موجود في السلة

            // 4. حذف العنصر من الداتا بيز
            _unitOfWork.CartItems.Delete(cartItem);

            return await _unitOfWork.SaveAsync() > 0;
        }
        #endregion

        #region Clear Entire Cart
        public async Task<bool> ClearCartAsync(int userId)
        {
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            var cart = await _unitOfWork.Carts.FindAsync(
                c => c.ClientId == client.ClientID,
                new[] { "CartItems" }
            );

            if (cart == null || !cart.CartItems.Any()) return true; // السلة فاضية بالفعل

            // حذف كل العناصر اللي جوه السلة
            foreach (var item in cart.CartItems.ToList())
            {
                _unitOfWork.CartItems.Delete(item);
            }

            return await _unitOfWork.SaveAsync() > 0;
        }
        #endregion

        #region UpdateCartItemQuantityAsync
        public async Task<bool> UpdateCartItemQuantityAsync(int userId, int productId, int newQuantity)
        {
            // 1. جلب العميل وسلته
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            var cart = await _unitOfWork.Carts.FindAsync(
                c => c.ClientId == client.ClientID,
                new[] { "CartItems" }
            );
            if (cart == null) throw new Exception("Cart not found.");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null) throw new Exception("Product not found in cart.");

            var product = await _unitOfWork.Products.FindAsync(p => p.ProductId == productId);
            if (product == null) throw new Exception("Product not found.");

            // 3. التأكد من أن الكمية الجديدة متوفرة في المخزن
            if (newQuantity > product.StockQuantity)
                throw new Exception($"Sorry, the requested quantity is unavailable. Only {product.StockQuantity} pieces are available.");

            // 4. منع الكميات الصفرية أو السالبة (لو عاوز يمسح يستخدم Delete)
            if (newQuantity <= 0)
                throw new Exception("The quantity must be at least 1.");

            // 5. تحديث الكمية
            cartItem.Quantity = newQuantity;

            _unitOfWork.CartItems.Update(cartItem);
            return await _unitOfWork.SaveAsync() > 0;
        }
        #endregion

        #region GetCartAsync
        public async Task<CartResponseDto> GetCartAsync(int userId)
        {
            // 1. جلب العميل وسلته مع كل البيانات المربوطة (Eager Loading)
            var client = await _unitOfWork.Clients.FindAsync(c => c.AppUserId == userId);
            if (client == null) throw new Exception("Client not found.");

            // 2. جلب السلة مع كل البيانات المرتبطة (المنتج والورشة)
            var cart = await _unitOfWork.Carts.FindAsync(
                c => c.ClientId == client.ClientID,
                new[] { "CartItems.Product", "CartItems.Product.ServiceProvider" }
            );

            if (cart == null || !cart.CartItems.Any())
            {
                return new CartResponseDto();
            }
            
            // 2. تحويل العناصر لـ DTO وحساب إجمالي كل منتج
            var items = client.Cart.CartItems.Select(ci => new CartItemResponseDto
            {
                ProductId = ci.ProductId,
                Name = $"{ci.Product.Name} {(ci.Product.VerticalPosition.HasValue ? ci.Product.VerticalPosition.ToString() : "")} {(ci.Product.HorizontalPosition.HasValue ? ci.Product.HorizontalPosition.ToString() : "")}".TrimEnd(),
                ImageUrl = ci.Product.ProductImageUrl?? "No Image",
                Price = ci.Product.Price,
                StoreName = ci.Product.ServiceProvider.Name,
                StockQuantity = ci.Product.StockQuantity,
                SelectedQuantity = ci.Quantity,
                TotalItemPrice = ci.Product.Price * ci.Quantity
            }).ToList();

            // 3. حساب إجمالي السلة بالكامل
            return new CartResponseDto
            {
                Items = items,
                TotalCartPrice = items.Sum(i => i.TotalItemPrice)
            };
        }

        #endregion



    }
}
