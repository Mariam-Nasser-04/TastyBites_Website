// Controllers/MenuController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartResturant.Data;
using SmartResturant.Models;

namespace SmartResturant.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string category = null)
        {
            ViewBag.Categories = _context.Categories
                .Include(c => c.Items)
                .OrderBy(c => c.Name)
                .ToList();

            if (!string.IsNullOrEmpty(category))
            {
                var selectedCategory = _context.Categories
                    .Include(c => c.Items)
                    .FirstOrDefault(c => c.Name == category);

                if (selectedCategory != null)
                {
                    return View(selectedCategory.Items.ToList());
                }
            }

            var allItems = _context.Items
                .Include(i => i.Category)
                .OrderBy(i => i.Name)
                .ToList();

            return View(allItems);
        }
    }
}