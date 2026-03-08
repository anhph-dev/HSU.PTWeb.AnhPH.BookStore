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

        // Thêm sản phẩm vào giỏ hàng
        public async Task<IActionResult> AddToCart(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, CartSessionKey) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = 1,
                    ImageUrl = product.ImageUrl
                });
            }
            else
            {
                item.Quantity++;
            }

            SessionHelper.SetObject(HttpContext.Session, CartSessionKey, cart);
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng của 1 item
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, CartSessionKey) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0) cart.Remove(item);
                else item.Quantity = quantity;
                SessionHelper.SetObject(HttpContext.Session, CartSessionKey, cart);
            }
            return RedirectToAction("Index");
        }

        // Xóa 1 item khỏi giỏ
        public IActionResult RemoveItem(int productId)
        {
            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, CartSessionKey) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SessionHelper.SetObject(HttpContext.Session, CartSessionKey, cart);
            }
            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ
        public IActionResult ClearCart()
        {
            SessionHelper.SetObject(HttpContext.Session, CartSessionKey, new List<CartItem>());
            return RedirectToAction("Index");
        }
    }
}
