using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HSU.PTWeb.AnhPH.BookStore.Data;
using HSU.PTWeb.AnhPH.BookStore.Models;

namespace HSU.PTWeb.AnhPH.BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private const int DefaultPageSize = 10;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        public ProductsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index(string search, int? categoryId, int page = 1, int pageSize = DefaultPageSize)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
                ViewData["Search"] = search;
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
                ViewData["CategoryId"] = categoryId.Value;
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, page);
            if (page > totalPages) page = totalPages == 0 ? 1 : totalPages;

            var products = await query
                .OrderBy(p => p.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;
            ViewData["Categories"] = await _context.Categories.ToListAsync();

            return View(products);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["Categories"] = _context.Categories.ToList();
            return View();
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model, IFormFile? imageFile)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Image");
            ModelState.Remove("Title");
            ModelState.Remove("Id");
            ModelState.Remove("Category");
            ModelState.Remove("OrderDetails");
            ModelState.Remove("CreatedDate");

            if (imageFile != null && imageFile.Length > 0)
            {
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("", "Chỉ chấp nhận file ảnh: JPG, PNG, GIF");
                }
                if (imageFile.Length > MaxFileSize)
                {
                    ModelState.AddModelError("", "Kích thước file không được vượt quá 5MB");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["Categories"] = _context.Categories.ToList();
                return View(model);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                try
                {
                    var uploads = Path.Combine(_env.WebRootPath, "images", "products");
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploads, fileName);
                    
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    
                    model.ImageUrl = "products/" + fileName;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi upload file: {ex.Message}");
                    ViewData["Categories"] = _context.Categories.ToList();
                    return View(model);
                }
            }

            model.CreatedDate = DateTime.UtcNow;
            _context.Products.Add(model);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Tạo sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewData["Categories"] = _context.Categories.ToList();
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product model, IFormFile? imageFile)
        {
            if (id != model.ProductId)
            {
                return NotFound();
            }

            ModelState.Remove("ImageUrl");
            ModelState.Remove("Image");
            ModelState.Remove("Title");
            ModelState.Remove("Id");
            ModelState.Remove("Category");
            ModelState.Remove("OrderDetails");
            ModelState.Remove("CreatedDate");

            if (imageFile != null && imageFile.Length > 0)
            {
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("imageFile", "Chỉ chấp nhận file ảnh: JPG, PNG, GIF");
                }
                if (imageFile.Length > MaxFileSize)
                {
                    ModelState.AddModelError("imageFile", "Kích thước file không được vượt quá 5MB");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["Categories"] = await _context.Categories.ToListAsync();
                return View(model);
            }

            try
            {
                var product = await _context.Products.FindAsync(model.ProductId);
                if (product == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm!";
                    return RedirectToAction(nameof(Index));
                }

                product.ProductName = model.ProductName;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Stock = model.Stock;
                product.CategoryId = model.CategoryId;

                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        var uploads = Path.Combine(_env.WebRootPath, "images", "products");
                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                        }

                        if (!string.IsNullOrEmpty(product.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(_env.WebRootPath, "images", product.ImageUrl);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                try
                                {
                                    System.IO.File.Delete(oldImagePath);
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Cannot delete old image: {ex.Message}");
                                }
                            }
                        }

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploads, fileName);
                        
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        
                        product.ImageUrl = "products/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("imageFile", $"Lỗi khi upload file: {ex.Message}");
                        ViewData["Categories"] = await _context.Categories.ToListAsync();
                        return View(model);
                    }
                }

                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                ViewData["Categories"] = await _context.Categories.ToListAsync();
                return View(model);
            }
        }

        // POST: Admin/Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var imagePath = Path.Combine(_env.WebRootPath, "images", product.ImageUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Xóa sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
