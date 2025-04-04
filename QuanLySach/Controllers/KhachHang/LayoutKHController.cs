using QuanLySach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLySach.Controllers.KhachHang
{
    public class LayoutKHController : Controller
    {
        // GET: Layout
        BookManagementDataContext db = new BookManagementDataContext();
        //public ActionResult Category()
        //{
        //    // Use LINQ to get categories and their active products
        //    var categories = db.book_categories
        //        .Select(c => new CategoryViewModel
        //        {
        //            name = c.name,
        //            Products = db.books
        //                .Where(p => p.status == 1 && p.code_category == c.code)
        //                .OrderByDescending(p => p.id)
        //                .Select(p => new ProductViewModel
        //                {
        //                    Id = p.id,
        //                    Name = p.name,
        //                    Price = p.price,
        //                    ImageUrl = "https://websitedemos.net/book-store-02/wp-content/uploads/sites/834/2021/05/author-book-store-book-img-05.jpg" // Assuming you have images by product ID
        //                })
        //                .ToList()
        //        })
        //        .Take(3).ToList();

        //    return View(categories);
        //}
        // GET: Layout
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult HeaderKH()
        {
            return View();
        }

    }
}