using HSU.PTWeb.AnhPH.BookStore.Data;
using HSU.PTWeb.AnhPH.BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HSU.PTWeb.AnhPH.BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CitiesController : Controller
    {
        private readonly AppDbContext _context;

        public CitiesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Cities.Include(c => c.Wards).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.CityName.Contains(search));
                ViewData["Search"] = search;
            }

            var cities = await query.OrderBy(c => c.CityName).ToListAsync();
            return View(cities);
        }

        public IActionResult Create() => View(new City());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(City model)
        {
            ModelState.Remove("Wards");
            if (await _context.Cities.AnyAsync(c => c.CityName == model.CityName))
                ModelState.AddModelError("CityName", "Tỉnh/Thành phố đã tồn tại.");

            if (!ModelState.IsValid) return View(model);

            _context.Cities.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm Tỉnh/Thành phố thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null) return NotFound();
            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, City model)
        {
            if (id != model.CityId) return NotFound();
            ModelState.Remove("Wards");

            if (await _context.Cities.AnyAsync(c => c.CityId != model.CityId && c.CityName == model.CityName))
                ModelState.AddModelError("CityName", "Tỉnh/Thành phố đã tồn tại.");

            if (!ModelState.IsValid) return View(model);

            var city = await _context.Cities.FindAsync(id);
            if (city == null) return NotFound();

            city.CityName = model.CityName;
            city.IsActive = model.IsActive;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var city = await _context.Cities.Include(c => c.Wards).FirstOrDefaultAsync(c => c.CityId == id);
            if (city == null)
            {
                TempData["Error"] = "Không tìm thấy Tỉnh/Thành phố.";
                return RedirectToAction(nameof(Index));
            }

            if (city.Wards.Any())
            {
                TempData["Error"] = "Không thể xóa Tỉnh/Thành phố đang có Phường/Xã.";
                return RedirectToAction(nameof(Index));
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa.";
            return RedirectToAction(nameof(Index));
        }
    }
}
