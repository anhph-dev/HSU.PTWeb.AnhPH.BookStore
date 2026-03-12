using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HSU.PTWeb.AnhPH.BookStore.Data;

namespace HSU.PTWeb.AnhPH.BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Index()
        {
            var today = DateTime.UtcNow.Date;
            var todayEnd = today.AddDays(1);

            // Thống kê tổng quan
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalCustomers = await _context.Users.Where(u => u.Role == "Customer").CountAsync();
            var todayRevenue = await _context.Orders
                .Where(o => o.OrderDate >= today && o.OrderDate < todayEnd)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            // Đơn hàng mới nhất
            var recentOrders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();

            // Sản phẩm sắp hết hàng
            var lowStockProducts = await _context.Products
                .Where(p => p.Stock <= 10 && p.Stock > 0)
                .OrderBy(p => p.Stock)
                .Take(5)
                .ToListAsync();

            ViewData["TotalProducts"] = totalProducts;
            ViewData["TotalOrders"] = totalOrders;
            ViewData["TotalCustomers"] = totalCustomers;
            ViewData["TodayRevenue"] = todayRevenue;
            ViewData["RecentOrders"] = recentOrders;
            ViewData["LowStockProducts"] = lowStockProducts;

            return View();
        }

        // GET: Admin/Dashboard/Revenue
        public async Task<IActionResult> Revenue(DateTime? date)
        {
            var selected = date ?? DateTime.UtcNow.Date;

            // Doanh thu trong ngày
            var dayStart = selected.Date;
            var dayEnd = dayStart.AddDays(1);
            var totalDay = await _context.Orders
                .Where(o => o.OrderDate >= dayStart && o.OrderDate < dayEnd)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            // Doanh thu trong tháng
            var monthStart = new DateTime(selected.Year, selected.Month, 1);
            var monthEnd = monthStart.AddMonths(1);
            var totalMonth = await _context.Orders
                .Where(o => o.OrderDate >= monthStart && o.OrderDate < monthEnd)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            // Doanh thu trong năm
            var yearStart = new DateTime(selected.Year, 1, 1);
            var yearEnd = yearStart.AddYears(1);
            var totalYear = await _context.Orders
                .Where(o => o.OrderDate >= yearStart && o.OrderDate < yearEnd)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            // Dữ liệu cho biểu đồ: 7 ngày gần nhất
            var labels = new List<string>();
            var values = new List<decimal>();
            for (int i = 6; i >= 0; i--)
            {
                var d = selected.Date.AddDays(-i);
                var s = d;
                var e = d.AddDays(1);
                var v = await _context.Orders
                    .Where(o => o.OrderDate >= s && o.OrderDate < e)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
                labels.Add(d.ToString("dd/MM"));
                values.Add(v);
            }

            ViewData["SelectedDate"] = selected.ToString("yyyy-MM-dd");
            ViewData["TotalDay"] = totalDay;
            ViewData["TotalMonth"] = totalMonth;
            ViewData["TotalYear"] = totalYear;
            ViewData["ChartLabels"] = labels;
            ViewData["ChartValues"] = values;

            return View();
        }
    }
}
