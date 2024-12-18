using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductTask.Models;

namespace ProductTask.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var productsQuery = _context.Productss
                .Include(p => p.Category)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                });

            var totalCount = productsQuery.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var products = productsQuery
                .OrderBy(p => p.ProductId) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            return View(products);
        }


        [HttpGet]
        public ActionResult Create()
        {
            
            var categoriess = _context.Categoriess.ToList();  
            if (categoriess.Any())
            {
                ViewBag.Categories = new SelectList(categoriess, "CategoryId", "CategoryName");
            }
            else
            {
                // Handle case where there are no categories
                ViewBag.Categories = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            return View(new Product());
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                // Add the new product to the context
                _context.Productss.Add(product);

                // Save changes asynchronously
                await _context.SaveChangesAsync();

                // Redirect to the action that shows the list of products
                return RedirectToAction("GetProduct");
            }

            // If the model is not valid, return to the Create view with the current product data
            ViewBag.Categories = new SelectList(_context.Categoriess, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }




        [HttpGet]
        public ActionResult Edit(int id)
        {
            var product = _context.Productss.Include(p => p.Category).FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();  
            }
            ViewBag.Categories = new SelectList(_context.Categoriess, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }


       
        [HttpPost]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(product).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(_context.Categoriess, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var product = _context.Productss.Find(id);
            if (product == null)
            {
                return NotFound();  
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = _context.Productss.Find(id);
            if (product != null)
            {
                _context.Productss.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
