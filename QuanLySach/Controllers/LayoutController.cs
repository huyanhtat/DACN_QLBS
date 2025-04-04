using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLySach.Models;

namespace QuanLySach.Controllers
{
    public class LayoutController : Controller
    {
        BookManagementDataContext db = new BookManagementDataContext();
        public ActionResult Category()
        {
            // Use LINQ to get categories and their active products
            var categories = db.book_categories
                .Select(c => new CategoryViewModel
                {
                    name = c.name,
                    Products = db.books
                        .Where(p => p.status == 1 && p.code_category == c.code && p.priority != 4)
                        .OrderByDescending(p=>p.id)
                        .Select(p => new ProductViewModel
                        {
                            Id = p.id,
                            Name = p.name,
                            Price = p.price,
                            Quantity = p.quantity,
                        })
                        .Take(4).ToList()
                })
                .OrderByDescending(c=>c.name).Take(5).ToList();
            
            return View(categories);
        }
        // GET: Layout
        
        public ActionResult Header()
        {
            return View();
        }
        public ActionResult HeaderKH()
        {
            return View();
        }

        public ActionResult Footer()
        {
            return View();
        }

        public ActionResult Banner()
        {
            var books = db.books.Where(s=>s.priority!=4 && s.status == 1).ToList(); 
            return View(books);
        }
    }
}