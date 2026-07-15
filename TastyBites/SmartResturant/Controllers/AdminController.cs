using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartResturant.Data;
using SmartResturant.Models;

namespace SmartResturant.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            var today = DateTime.Today;

            ViewBag.ItemsCount = _context.Items.Count();
            ViewBag.CategoriesCount = _context.Categories.Count();
            ViewBag.CustomersCount = _context.Customers.Count();
            ViewBag.TotalOrders = _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .Count();
            ViewBag.TotalRevenue = _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;

            var recentOrders = _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToList();

            return View(recentOrders); 
        }

    }
}
