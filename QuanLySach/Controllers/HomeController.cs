using QuanLySach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace QuanLySach.Controllers
{

    public class HomeController : Controller
    {
       
        BookManagementDataContext db = new BookManagementDataContext();

        public ActionResult Index()
        {
            
                var sp = (from s in db.books
                          join c in db.book_categories on s.code_category equals c.code
                          join j in db.book_join_authors on s.id equals j.id_book
                          join a in db.authors on j.id_author equals a.id
                          where s.priority != 4 && s.status == 1
                          orderby s.id descending
                          select new ChiTietSach
                          {
                              Id = s.id,
                              tenSach = s.name,
                              gia = (float)s.price,
                              moTa = s.description,
                              tenTacGia = a.name,
                              chuDe = c.name,
                              bio = a.bio,

                          }).FirstOrDefault();
                return View(sp);
            
            
        }
        public ActionResult IndexKH()
        {
           
                var sp = (from s in db.books
                          join c in db.book_categories on s.code_category equals c.code
                          join j in db.book_join_authors on s.id equals j.id_book
                          join a in db.authors on j.id_author equals a.id
                          where s.priority != 4 && s.status == 1
                          orderby s.id descending
                          select new ChiTietSach
                          {
                              Id = s.id,
                              tenSach = s.name,
                              gia = (float)s.price,
                              moTa = s.description,
                              tenTacGia = a.name,
                              chuDe = c.name,
                              bio = a.bio,

                          }).FirstOrDefault();
                return View(sp); 
            
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult Wishlist()
        {
            if (Session["id"] != null)
            {
                int userId = (int)Session["id"];
                var wishlistItems = db.wishlists
                                      .Where(w => w.id_user == userId)
                                      .Join(db.books,
                                            w => w.id_book,
                                            b => b.id,
                                            (w, b) => b) // Lấy thông tin sách từ bảng books
                                      .ToList();
                return View(wishlistItems);
            }
            else
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
        }
        public ActionResult ComingSoon()
        {
            return View(db.books.Where(t => t.priority == 4 && t.status == 1).ToList());
        }
        
    }
}