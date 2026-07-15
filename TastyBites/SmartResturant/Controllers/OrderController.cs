// Controllers/OrderController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartResturant.Data;
using SmartResturant.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SmartResturant.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var items = _context.Items.Include(i => i.Category).ToList();
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order, List<int> itemIds, List<int> quantities)
        {
            if (ModelState.IsValid)
            {
                decimal totalAmount = 0;
                for (int i = 0; i < itemIds.Count; i++)
                {
                    var item = await _context.Items.FindAsync(itemIds[i]);
                    if (item != null && quantities[i] > 0)
                    {
                        totalAmount += item.Price * quantities[i];
                        order.OrderItems.Add(new OrderItem
                        {
                            ItemId = item.Id,
                            Quantity = quantities[i],
                            UnitPrice = item.Price
                        });
                    }
                }

                if (order.OrderItems.Count == 0)
                {
                    ModelState.AddModelError("", "Please select at least one item");
                    var items = _context.Items.Include(i => i.Category).ToList();
                    return View(items);
                }

                order.TotalAmount = totalAmount;
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return RedirectToAction("Confirmation", new { id = order.Id });
            }

            var allItems = _context.Items.Include(i => i.Category).ToList();
            return View(allItems);
        }

        public IActionResult Confirmation(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        public IActionResult Manage()
        {
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Manage");
        }
    }
}