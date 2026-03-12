using Microsoft.AspNetCore.Mvc;
using HSU.PTWeb.AnhPH.BookStore.Data;
using HSU.PTWeb.AnhPH.BookStore.Helpers;
using HSU.PTWeb.AnhPH.BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace HSU.PTWeb.AnhPH.BookStore.Controllers
{
    // Controller quản lý giỏ hàng (lưu trong Session)
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        // Khóa session để lưu giỏ hàng
        private const string CartSessionKey = "cart";

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // Xem giỏ hàng
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, CartSessionKey) ?? new List<CartItem>();
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ hàng với kiểm tra số lượng kho
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction("Index", "Product");
            }

            // Kiểm tra số lượng hợp lệ
            if (quantity <= 0)
            {
                TempData["Error"] = "Số lượng phải lớn hơn 0!";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, CartSessionKey) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            
            // Tính tổng số lượng trong giỏ + số lượng muốn thêm
            var currentQuantityInCart = item?.Quantity ?? 0;
            var totalQuantity = currentQuantityInCart + quantity;

            // Kiểm tra số lượng tồn kho
            if (totalQuantity > product.Stock)
            {
                TempData["Error"] = $"Không đủ hàng trong kho! Chỉ còn {product.Stock} sản phẩm. Bạn đã có {currentQuantityInCart} trong giỏ.";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl
                });
            }
            else
            {
                item.Quantity += quantity;
            }

            SessionHelper.SetObject(HttpContext.Session, CartSessionKey, cart);
            TempData["Success"] = $"Đã thêm {quantity} sản phẩm vào giỏ hàng!";
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng của 1 item với kiểm tra kho
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction("Index");
            }

            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, CartSessionKey) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            
            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                    TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
                }
                else
                {
                    // Kiểm tra số lượng tồn kho
                    if (quantity > product.Stock)
                    {
                        TempData["Error"] = $"Không đủ hàng trong kho! Chỉ còn {product.Stock} sản phẩm.";
                        return RedirectToAction("Index");
                    }

                    item.Quantity = quantity;
                    TempData["Success"] = "Đã cập nhật số lượng!";
                }
                
                SessionHelper.SetObject(HttpContext.Session, CartSessionKey, cart);
            }
            
            return RedirectToAction("Index");
        }

        // Xóa 1 item khỏi giỏ
        [HttpPost]
        public IActionResult RemoveItem(int productId)
        {
            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, CartSessionKey) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SessionHelper.SetObject(HttpContext.Session, CartSessionKey, cart);
                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
            }
            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ
        [HttpPost]
        public IActionResult ClearCart()
        {
            SessionHelper.SetObject(HttpContext.Session, CartSessionKey, new List<CartItem>());
            TempData["Success"] = "Đã xóa toàn bộ giỏ hàng!";
            return RedirectToAction("Index");
        }
    }
}
