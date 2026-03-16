using HSU.PTWeb.AnhPH.BookStore.Data;
using HSU.PTWeb.AnhPH.BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HSU.PTWeb.AnhPH.BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class WardsController : Controller
    {
        private readonly AppDbContext _context;

        public WardsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? cityId, string search)
        {
            var query = _context.Wards.Include(w => w.City).AsQueryable();
            if (cityId.HasValue && cityId.Value > 0)
            {
                query = query.Where(w => w.CityId == cityId.Value);
                ViewData["CityId"] = cityId.Value;
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(w => w.WardName.Contains(search));
                ViewData["Search"] = search;
            }

            ViewData["Cities"] = await _context.Cities.OrderBy(c => c.CityName).ToListAsync();
            var wards = await query.OrderBy(w => w.City.CityName).ThenBy(w => w.WardName).ToListAsync();
            return View(wards);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Cities"] = await _context.Cities.OrderBy(c => c.CityName).ToListAsync();
            return View(new Ward());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ward model)
        {
            ModelState.Remove("City");
            if (await _context.Wards.AnyAsync(w => w.CityId == model.CityId && w.WardName == model.WardName))
                ModelState.AddModelError("WardName", "Phường/Xã đã tồn tại trong Tỉnh/Thành phố này.");

            if (!ModelState.IsValid)
            {
                ViewData["Cities"] = await _context.Cities.OrderBy(c => c.CityName).ToListAsync();
                return View(model);
            }

            _context.Wards.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm Phường/Xã thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ward = await _context.Wards.FindAsync(id);
            if (ward == null) return NotFound();

            ViewData["Cities"] = await _context.Cities.OrderBy(c => c.CityName).ToListAsync();
            return View(ward);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ward model)
        {
            if (id != model.WardId) return NotFound();
            ModelState.Remove("City");

            if (await _context.Wards.AnyAsync(w => w.WardId != model.WardId && w.CityId == model.CityId && w.WardName == model.WardName))
                ModelState.AddModelError("WardName", "Phường/Xã đã tồn tại trong Tỉnh/Thành phố này.");

            if (!ModelState.IsValid)
            {
                ViewData["Cities"] = await _context.Cities.OrderBy(c => c.CityName).ToListAsync();
                return View(model);
            }

            var ward = await _context.Wards.FindAsync(id);
            if (ward == null) return NotFound();

            ward.CityId = model.CityId;
            ward.WardName = model.WardName;
            ward.IsActive = model.IsActive;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ward = await _context.Wards.FindAsync(id);
            if (ward == null)
            {
                TempData["Error"] = "Không tìm thấy Phường/Xã.";
                return RedirectToAction(nameof(Index));
            }

            _context.Wards.Remove(ward);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa.";
            return RedirectToAction(nameof(Index));
        }
    }
}
