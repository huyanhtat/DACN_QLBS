using QuanLySach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLySach.Controllers.KhachHang
{
    public class HomeKHController : Controller
    {
        // GET: Home
        BookManagementDataContext db = new BookManagementDataContext();
        // GET: Home
        [HttpGet]
        public ActionResult IndexKH()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult Wishlist()
        {
            return View();
        }
        public ActionResult ComingSoon()
        {
            return View(db.books.Where(t => t.priority == 2).ToList());
        }
    }
}