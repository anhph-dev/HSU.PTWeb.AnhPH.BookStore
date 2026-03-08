using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HSU.PTWeb.AnhPH.BookStore.Data;
using HSU.PTWeb.AnhPH.BookStore.Models;

namespace HSU.PTWeb.AnhPH.BookStore.Controllers
{
    // Controller cho quản lý xem sản phẩm (không yêu cầu đăng nhập)
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private const int PageSize = 10;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách sản phẩm với phân trang, tìm kiếm, lọc danh mục, sắp xếp theo giá
        public async Task<IActionResult> Index(string search, int? categoryId, string sort, int page = 1)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Tìm kiếm theo tên sản phẩm
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
                ViewData["Search"] = search;
            }

            // Lọc theo danh mục
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
                ViewData["CategoryId"] = categoryId.Value;
            }

            // Sắp xếp theo giá
            switch (sort)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                default:
                    query = query.OrderBy(p => p.ProductName);
                    break;
            }
            ViewData["Sort"] = sort;

            // Phân trang
            var totalItems = await query.CountAsync();
            var products = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

            ViewData["Page"] = page;
            ViewData["TotalPages"] = (int)System.Math.Ceiling(totalItems / (double)PageSize);

            // Danh sách category để hiển thị dropdown lọc
            ViewData["Categories"] = await _context.Categories.ToListAsync();

            return View(products);
        }

        // Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}
