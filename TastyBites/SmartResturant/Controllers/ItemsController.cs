using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartResturant.Data;
using SmartResturant.Models;

namespace SmartResturant.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }


        //======================= Index =======================

        [HttpGet]
        public IActionResult Index(string searchTerm, int? categoryId)
        {
            var items = _context.Items
                .Include(i => i.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                items = items.Where(i => i.Name.Contains(searchTerm));
            }

            if (categoryId.HasValue && categoryId > 0)
            {
                items = items.Where(i => i.CategoryId == categoryId.Value);
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.AllCategories = _context.Categories.OrderBy(c => c.Name).ToList();
            ViewBag.SelectedCategoryId = categoryId ?? 0; // تغيير هنا لضمان قيمة افتراضية

            return View(items.OrderBy(i => i.Name).ToList());
        }

        //======================= Search =======================

        public IActionResult SearchResults(string searchTerm, int? categoryId)
        {
            var items = _context.Items.Include(i => i.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                items = items.Where(i => i.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            if (categoryId.HasValue && categoryId != 0)
            {
                items = items.Where(i => i.CategoryId == categoryId.Value);
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.AllCategories = _context.Categories.OrderBy(c => c.Name).ToList();

            return View(items.OrderBy(i => i.Name).ToList());
        }


        //======================= Create =======================

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.AllCategories = _context.Categories.OrderBy(c => c.Name).ToList();
            return View("Create");
        }
        [HttpPost]
        public IActionResult Create(Item item)
        {
            if (ModelState.IsValid == true)
            {
                if (item.ImageFile != null)
                {
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(item.ImageFile.FileName);

                    string fullPath = Path.Combine(folderPath, imageName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        item.ImageFile.CopyTo(stream);
                    }

                    item.ImageName = imageName;
                }
                _context.Items.Add(item);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.AllCategories = _context.Categories.OrderBy(c => c.Name).ToList();
                return View("Create", item);
            }
        }

        //======================= Edit =======================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Item item = _context.Items.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                ViewBag.AllCategories = _context.Categories.ToList();
                return View(item);
            }
        }
        [HttpPost]
        public IActionResult EditCurrent(Item item)
        {
            if (ModelState.IsValid == true)
            {
                var oldItem = _context.Items.Find(item.Id);
                if (oldItem == null) return NotFound();

                oldItem.Name = item.Name;
                oldItem.Price = item.Price;
                oldItem.CategoryId = item.CategoryId;

                if (item.ImageFile != null)
                {
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(item.ImageFile.FileName);
                    string fullPath = Path.Combine(folderPath, imageName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        item.ImageFile.CopyTo(stream);
                    }

                    if (!string.IsNullOrEmpty(oldItem.ImageName))
                    {
                        string oldPath = Path.Combine(folderPath, oldItem.ImageName);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    oldItem.ImageName = imageName;
                }
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.AllCategories = _context.Categories.OrderBy(c => c.Name).ToList();
                return View("Edit", item);
            }
        }

        //======================= Details =======================

        [HttpGet]
        public IActionResult Details(int id)
        {
            Item item = _context.Items.Include(cat => cat.Category).FirstOrDefault(item => item.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                return View(item);
            }
        }

        //======================= Delete =======================

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Item item = _context.Items.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                return View("Delete", item);
            }
        }
        [HttpPost]
        public IActionResult DeleteCurrent(int id)
        {
            Item item = _context.Items.Find(id);
            _context.Items.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}