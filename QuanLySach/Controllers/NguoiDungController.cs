using QuanLySach.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace QuanLySach.Controllers
{
    public class NguoiDungController : Controller
    {
        BookManagementDataContext db = new BookManagementDataContext();
        // GET: NguoiDung
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DangKy(user kh, FormCollection f)
        {
            var hoTen = f["HoTenKH"];
            var matKhau = f["MatKhau"];
            var reMatKhau = f["ReMatKhau"];


            if (String.IsNullOrEmpty(hoTen))
            {
                return Json(new { success = false, message = "Họ Tên Không Được Bỏ Trống" });
            }

            if (String.IsNullOrEmpty(matKhau))
            {
                return Json(new { success = false, message = "Vui Lòng Nhập Mật Khẩu" });
            }

            var passwordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$";
            if (!Regex.IsMatch(matKhau, passwordPattern))
            {
                return Json(new { success = false, message = "Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ hoa, chữ số và ký tự đặc biệt." });
            }

            if (matKhau != reMatKhau)
            {
                return Json(new { success = false, message = "Mật Khẩu Và Nhập Lại Mật Khẩu Không Khớp" });
            }

            kh.user_name = hoTen;
            kh.password = EncryptMD5(matKhau);
            kh.create_date = DateTime.Now;
            kh.create_by = "user";
            kh.code_role = "USER";
            kh.status = 1;

            try
            {
                db.users.InsertOnSubmit(kh);
                db.SubmitChanges();
                return Json(new { success = true, redirectUrl = Url.Action("DangNhap", "NguoiDung") });
            }
            catch
            {
                return Json(new { success = false, message = "Đăng ký thất bại. Vui lòng thử lại." });
            }
        }



        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DangNhapDN(string username, string password)
        {
            var user = db.users.FirstOrDefault(s => s.user_name == username);

            if (user != null)
            {
                
                var encryptedPassword = EncryptMD5(password);

                if (user.password == encryptedPassword)
                {
                    
                    //Session["HoTenKH"] = user.user_name;
                    Session["id"] = user.id;


                    return Json(new { success = true, redirectUrl = Url.Action("IndexKH", "Home") });
                }
                else
                {
                    return Json(new { success = false, message = "Đăng Nhập Thất Bại. Vui lòng kiểm tra tài khoản và mật khẩu." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Đăng Nhập Thất Bại. Vui lòng kiểm tra tài khoản và mật khẩu." });
            }
        }


        [HttpGet]
        public ActionResult UpdateProfile()
        {
            int userId = (int)Session["id"];
            var user = db.users.FirstOrDefault(u => u.id == userId);

            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        // Action để lưu cập nhật thông tin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(user updatedUser)
        {
            int userId = (int)Session["id"];
            var user = db.users.FirstOrDefault(u => u.id == userId);

            if (user != null)
            {
                user.full_name = updatedUser.full_name;
                user.phone = updatedUser.phone;
                user.address = updatedUser.address;
                user.email = updatedUser.email;
                user.gender = updatedUser.gender;
                user.date_of_birth = updatedUser.date_of_birth;
                user.modified_date = DateTime.Now;
                user.modified_by = user.user_name;

                db.SubmitChanges();
                ViewBag.Message = "Cập nhật thông tin thành công!";
            }
            else
            {
                ViewBag.Message = "Không thể cập nhật thông tin!";
            }

            return View(user);
        }

        public ActionResult HistoryPurchase()
        {
            int userId = Convert.ToInt32(Session["id"]);

            // Lấy danh sách lịch sử mua hàng của người dùng
            var purchaseHistories = db.purchase_histories
                .Where(ph => ph.id_user == userId)
                .OrderByDescending(ph => ph.purchase_date) // Sắp xếp theo ngày mua giảm dần
                .ToList();

            if (!purchaseHistories.Any())
            {
                return HttpNotFound("Không tìm thấy lịch sử mua hàng.");
            }

            // Gộp các sản phẩm theo ngày mua
            var groupedOrderDetails = purchaseHistories
                .Join(db.books,
                    ph => ph.id_book,
                    bk => bk.id,
                    (ph, bk) => new
                    {
                        ph.purchase_date,
                        BookName = bk.name,
                        Quantity = ph.quantity,
                        UnitPrice = bk.price,
                        VAT = 0, // Giả sử không có VAT
                        TotalPrice = bk.price * ph.quantity,
                        
                    })
                .Where(d => d.Quantity > 0 && d.TotalPrice > 0) // Lọc các mục hợp lệ
                .GroupBy(d => d.purchase_date) // Nhóm theo ngày mua
                .Select(g => new OrderViewModel
                {
                    OrderDate = g.Key,
                    TotalPrice = g.Sum(p => p.TotalPrice), // Tính tổng giá trị theo ngày
                    Status = "Đã thanh toán",
                    OrderDetails = g.Select(p => new OrderDetailViewModel
                    {
                        BookName = p.BookName,
                        Quantity = p.Quantity,
                        UnitPrice = p.UnitPrice,
                        VAT = p.VAT,
                        TotalPrice = p.TotalPrice,
                        
                    }).ToList()
                })
                .OrderByDescending(g => g.OrderDate) // Sắp xếp theo ngày mua giảm dần
                .ToList();

            return View(groupedOrderDetails);
        }



        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // Action để xử lý thay đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (Session["id"] == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            int userId = (int)Session["id"];
            var user = db.users.FirstOrDefault(u => u.id == userId);

            // Kiểm tra nếu mật khẩu hiện tại không chính xác
            if (user == null || user.password != oldPassword) // Thay thế bằng mã hóa nếu cần
            {
                ViewBag.ErrorMessage = "Mật khẩu hiện tại không đúng.";
                return View();
            }

            // Kiểm tra nếu mật khẩu mới và xác nhận mật khẩu không trùng khớp
            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không khớp.";
                return View();
            }

            // Cập nhật mật khẩu mới
            user.password = EncryptMD5(newPassword); 
            user.modified_date = DateTime.Now;
            db.SubmitChanges();

            ViewBag.SuccessMessage = "Đổi mật khẩu thành công!";
            return View();
        }
        private string EncryptMD5(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
        public ActionResult Logout()
        {
            
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult XemDonHang()
        {
            int userId = Convert.ToInt32(Session["id"]);

            // Lấy danh sách ID các đơn hàng của người dùng
            var orders = db.bills
                .Where(b => b.id_user == userId)
                .OrderByDescending(b => b.id)
                .Select(b => b.id)
                .FirstOrDefault();


            if (orders==0)
            {
                return HttpNotFound("Không tìm thấy đơn hàng nào.");
            }

            // Lấy thông tin chi tiết của tất cả các đơn hàng
            var orderDetails = from bd in db.bill_details
                               join bk in db.books on bd.id_book equals bk.id
                               where orders == bd.id_bill
                               select new OrderDetailViewModel
                               {
                                   BookName = bk.name,
                                   Quantity = bd.quantity,
                                   UnitPrice = bd.price,
                                   VAT = bd.vat,
                                   
                                   TotalPrice = bd.price * bd.quantity
                               };

            var orderInfos = db.bills
                .Where(b => orders.Equals(b.id))
                .Select(b => new OrderViewModel
                {
                    BillID = b.id,
                    OrderDate = b.create_date,
                    Status = b.status,
                    TotalPrice = b.total_price,
                    PaymentMethod = b.method.name,
                    ShippingMethod = b.shipping.method_name,
                    TrakingShipping = b.shipping.tracking_url,
                    Description = b.shipping.description,
                    Receive = b.receive,
                    OrderDetails = orderDetails
                        .Where(d => d.Quantity > 0 && d.TotalPrice > 0)
                        .ToList()
                })
                .OrderByDescending(b=>b.BillID).ToList();

            if (!orderInfos.Any())
            {
                return HttpNotFound("Không tìm thấy thông tin đơn hàng.");
            }
            Session["payment"] = null;
            return View(orderInfos.Take(1));
        }



    }
}