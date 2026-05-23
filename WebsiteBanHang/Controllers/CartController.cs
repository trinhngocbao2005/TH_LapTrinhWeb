using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using WebsiteBanHang.Models;
using WebsiteBanHang.Repositories;

namespace WebsiteBanHang.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private const string CartSessionKey = "SessionCart";

        public CartController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Helper to get cart from Session
        private List<CartItem> GetCart()
        {
            var sessionData = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(sessionData))
            {
                return new List<CartItem>();
            }
            return JsonSerializer.Deserialize<List<CartItem>>(sessionData) ?? new List<CartItem>();
        }

        // Helper to save cart to Session
        private void SaveCart(List<CartItem> cart)
        {
            var sessionData = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, sessionData);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _productRepository.GetById(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
            }

            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Price = product.Price,
                    AvatarUrl = product.AvatarUrl,
                    Quantity = quantity
                });
            }
            else
            {
                item.Quantity += quantity;
            }

            SaveCart(cart);

            var totalItems = cart.Sum(c => c.Quantity);
            var totalPrice = cart.Sum(c => c.TotalPrice);

            return Json(new { 
                success = true, 
                message = "Đã thêm sản phẩm vào giỏ hàng thành công!",
                totalItems = totalItems,
                totalPrice = totalPrice.ToString("N0") + "đ"
            });
        }

        // POST: Cart/RemoveFromCart
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            var totalItems = cart.Sum(c => c.Quantity);
            var totalPrice = cart.Sum(c => c.TotalPrice);

            return Json(new { 
                success = true, 
                message = "Đã xóa sản phẩm khỏi giỏ hàng!",
                totalItems = totalItems,
                totalPrice = totalPrice.ToString("N0") + "đ"
            });
        }

        // GET: Cart/GetCartDetails (for Sidebar Drawer refresh)
        [HttpGet]
        public IActionResult GetCartDetails()
        {
            var cart = GetCart();
            var totalItems = cart.Sum(c => c.Quantity);
            var totalPrice = cart.Sum(c => c.TotalPrice);

            return Json(new {
                items = cart.Select(c => new {
                    productId = c.ProductId,
                    productName = c.ProductName,
                    price = c.Price.ToString("N0") + "đ",
                    avatarUrl = c.AvatarUrl,
                    quantity = c.Quantity,
                    totalItemPrice = c.TotalPrice.ToString("N0") + "đ"
                }),
                totalItems = totalItems,
                totalPrice = totalPrice.ToString("N0") + "đ"
            });
        }
    }
}
