using QuanLySach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLySach.Controllers
{
    public class LibraryController : Controller
    {
        BookManagementDataContext db = new BookManagementDataContext();

        public ActionResult Library()
        {
            return View();
        }

        public ActionResult Library_Filter()
        {
            return View(db.books.Where(s=>s.priority != 4).OrderByDescending(t=>t.id).Take(2));
        }

    }
}