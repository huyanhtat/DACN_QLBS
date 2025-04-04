using QuanLySach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLySach.Controllers.KhachHang
{
    public class WishlistController : Controller
    {
        BookManagementDataContext db = new BookManagementDataContext();

        [HttpPost]
        public ActionResult AddToWishlist(int bookId)
        {
            // Kiểm tra người dùng đã đăng nhập hay chưa
            if (Session["id"] != null)
            {
                int userId = (int)Session["id"];

                // Kiểm tra xem sách đã tồn tại trong wishlist của người dùng chưa
                var existingItem = db.wishlists.FirstOrDefault(w => w.id_user == userId && w.id_book == bookId);
                if (existingItem == null)
                {
                    // Thêm sách vào wishlist
                    var wishlistItem = new wishlist
                    {
                        id_user = userId,
                        id_book = bookId,
                        create_date = DateTime.Now
                    };

                    db.wishlists.InsertOnSubmit(wishlistItem);
                    db.SubmitChanges();
                    return Json(new { success = true, message = "Đã thêm vào wishlist!" });
                }
                else
                {
                    return Json(new { success = false, message = "Sách đã có trong wishlist!" });
                }
            }
            else
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để lưu vào wishlist." });
            }
        }

    }
}