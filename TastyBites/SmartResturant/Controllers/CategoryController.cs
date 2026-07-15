using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartResturant.Data;
using SmartResturant.Models;

namespace SmartResturant.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }


        //======================= Index =======================

        [HttpGet]
        public IActionResult Index(string searchTerm)
        {
            var categories = _context.Categories
                .Include(c => c.Items) // إذا كنت تريد عرض عدد العناصر
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                categories = categories.Where(c => c.Name.Contains(searchTerm));
            }

            ViewBag.SearchTerm = searchTerm;
            return View(categories.OrderBy(c => c.Name).ToList());
        }

        //======================= Create =======================

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }
        [HttpPost]
        public IActionResult Create(Category cat)
        {
            if (ModelState.IsValid == true)
            {
                if (cat.ImageFile != null)
                {
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(cat.ImageFile.FileName);

                    string fullPath = Path.Combine(folderPath, imageName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        cat.ImageFile.CopyTo(stream);
                    }

                    cat.ImageName = imageName;
                }
                _context.Categories.Add(cat);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View("Create", cat);
            }
        }

        //======================= Search =======================

        public IActionResult SearchResults(string searchTerm)
        {
            var results = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                results = results.Where(c => c.Name.ToLower().Contains(searchTerm));
            }

            ViewBag.SearchTerm = searchTerm;

            return View(results.ToList());
        }


        //======================= Edit =======================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Category cat = _context.Categories.Find(id);
            if (cat == null)
            {
                return NotFound();
            }
            else
            {
                return View(cat);
            }
        }
        [HttpPost]
        public IActionResult EditCurrent(Category cat)
        {
            if (ModelState.IsValid)
            {
                var oldCategory = _context.Categories.Find(cat.Id);
                if (oldCategory == null) return NotFound();

                oldCategory.Name = cat.Name;

                if (cat.ImageFile != null)
                {
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    string imageName = Guid.NewGuid().ToString() + Path.GetExtension(cat.ImageFile.FileName);
                    string fullPath = Path.Combine(folderPath, imageName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        cat.ImageFile.CopyTo(stream);
                    }

                    if (!string.IsNullOrEmpty(oldCategory.ImageName))
                    {
                        string oldPath = Path.Combine(folderPath, oldCategory.ImageName);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    oldCategory.ImageName = imageName;
                }

                _context.Categories.Update(oldCategory);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View("Edit", cat);
            }
        }


        //======================= Details =======================

        [HttpGet]
        public IActionResult Details(int id)
        {
            Category cat = _context.Categories.Include(cat => cat.Items).FirstOrDefault(i => i.Id == id);
            if (cat == null)
            {
                return NotFound();
            }
            else
            {
                return View(cat);
            }
        }

        //======================= Delete =======================

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Category cat = _context.Categories.Find(id);
            if (cat == null)
            {
                return NotFound();
            }
            else
            {
                return View("Delete", cat);
            }
        }
        [HttpPost]
        public IActionResult DeleteCurrent(int id)
        {
            Category cat = _context.Categories.Find(id);
            _context.Categories.Remove(cat);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}