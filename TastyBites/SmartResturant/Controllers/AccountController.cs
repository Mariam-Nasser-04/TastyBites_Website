using Microsoft.AspNetCore.Mvc;
using SmartResturant.Data;
using SmartResturant.Models;

namespace SmartResturant.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // تحقق من admin
            var admin = _context.Admins.FirstOrDefault(a => a.Email == email && a.Password == password);
            if (admin != null)
            {
                HttpContext.Session.SetString("UserType", "Admin");
                return RedirectToAction("Dashboard", "Admin");
            }

            // تحقق من customer
            var customer = _context.Customers.FirstOrDefault(c => c.Email == email && c.Password == password);
            if (customer != null)
            {
                HttpContext.Session.SetString("UserType", "Customer");
                HttpContext.Session.SetString("CustomerName", customer.Name);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // تحقق إن البريد غير مستخدم قبل كده
                var exists = _context.Customers.Any(c => c.Email == customer.Email);
                if (exists)
                {
                    ViewBag.Error = "Email is already registered.";
                    return View(customer);
                }

                _context.Customers.Add(customer);
                _context.SaveChanges();

                // توجيه بعد التسجيل (ممكن تخليه يروح login)
                return RedirectToAction("Login");
            }

            return View(customer);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            TempData.Clear();

            return RedirectToAction("Login", "Account");
        }
    }
}
