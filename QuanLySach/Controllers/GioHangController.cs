using QuanLySach.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace QuanLySach.Controllers
{
    public class GioHangController : Controller
    {
        BookManagementDataContext db = new BookManagementDataContext();
        // GET: GioHang
        public ActionResult Index()
        {
            return View();
        }
        private cart GetOrCreateCart(int userId)
        {
            // Check if a cart exists for the user
            var cart = db.carts.FirstOrDefault(c => c.id_user == userId);

            // If no cart exists, create a new one
            if (cart == null)
            {
                cart = new cart
                {
                    id_user = userId,
                    create_date = DateTime.Now,
                    update_date = DateTime.Now
                };

                // Add the new cart to the database
                db.carts.InsertOnSubmit(cart);
                db.SubmitChanges(); // Save changes to generate the cart ID
            }

            return cart;
        }

        [HttpPost]
        public JsonResult CapNhatGioHang(int MaSP, int txtSoLuong)
        {
            if (Session["id"] == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện thao tác này." });
            }

            try
            {
                int userId = Convert.ToInt32(Session["id"]);
                var cart = GetOrCreateCart(userId);

                // Tìm sản phẩm trong chi tiết giỏ hàng
                var cartDetail = db.cart_details.FirstOrDefault(cd => cd.id_cart == cart.id && cd.id_book == MaSP);

                if (cartDetail != null)
                {
                    if (txtSoLuong > 0)
                    {
                        // Cập nhật số lượng mới
                        cartDetail.quantity = txtSoLuong;
                    }
                    else
                    {
                        // Nếu số lượng mới là 0, xóa sản phẩm khỏi giỏ hàng
                        db.cart_details.DeleteOnSubmit(cartDetail);
                    }

                    // Cập nhật ngày sửa giỏ hàng
                    cart.update_date = DateTime.Now;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SubmitChanges();

                    // Tính tổng số lượng và tổng tiền giỏ hàng sau khi cập nhật
                    var totalQuantity = db.cart_details
                        .Where(cd => cd.id_cart == cart.id)
                        .Sum(cd => cd.quantity);

                    var totalPrice = db.cart_details
                        .Where(cd => cd.id_cart == cart.id)
                        .Sum(cd => cd.quantity * cd.book.price);

                    return Json(new
                    {
                        success = true,
                        totalQuantity = totalQuantity,
                        totalPrice = string.Format("{0:N0}", totalPrice) + " VND"
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }



        // Add an item to the cart
        public ActionResult GioHang()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            int userId = Convert.ToInt32(Session["id"]);
            var cart = GetOrCreateCart(userId);

            var cartItems = from cd in db.cart_details
                            join b in db.books on cd.id_book equals b.id
                            where cd.id_cart == cart.id
                            select new GioHang
                            {
                                MaSach = b.id,
                                TenSach = b.name,
                                SoLuong = cd.quantity,
                                DonGia = cd.price,
                                ThanhTien = cd.price
                            };

            // Tính tổng số lượng và tổng thành tiền
            ViewBag.TongSoLuong = cart.cart_details.Sum(cd=>cd.quantity);
            ViewBag.TongSanPham = cart.cart_details.Count();
            ViewBag.TongThanhTien = cart.cart_details.Sum(cd => cd.quantity * cd.price);
            return View(cartItems); 
        }

        public ActionResult ThemGioHang(int ms)
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            int userId = Convert.ToInt32(Session["id"]);
            var cart = GetOrCreateCart(userId);

       
            var cartDetail = db.cart_details.FirstOrDefault(cd => cd.id_cart == cart.id && cd.id_book == ms);

            if (cartDetail == null)
            {
     
                var book = db.books.FirstOrDefault(b => b.id == ms);
                if (book == null)
                {
                    return HttpNotFound("Book not found");
                }

                cartDetail = new cart_detail
                {
                    id_cart = cart.id,
                    id_book = ms,
                    quantity = 1, 
                    price = book.price 
                };

                db.cart_details.InsertOnSubmit(cartDetail);
            }
            else
            {
                cartDetail.quantity++;
            }

            cart.update_date = DateTime.Now;

            db.SubmitChanges();

            return RedirectToAction("GioHang", "GioHang");
            
        }

        public ActionResult AddGioHang(int ms)
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            int userId = Convert.ToInt32(Session["id"]);
            var cart = GetOrCreateCart(userId);


            var cartDetail = db.cart_details.FirstOrDefault(cd => cd.id_cart == cart.id && cd.id_book == ms);

            if (cartDetail == null)
            {

                var book = db.books.FirstOrDefault(b => b.id == ms);
                if (book == null)
                {
                    return HttpNotFound("Book not found");
                }

                cartDetail = new cart_detail
                {
                    id_cart = cart.id,
                    id_book = ms,
                    quantity = 1,
                    price = book.price
                };

                db.cart_details.InsertOnSubmit(cartDetail);
            }
            else
            {
                cartDetail.quantity++;
            }

            cart.update_date = DateTime.Now;

            db.SubmitChanges();

            return RedirectToAction("XemChiTiet", "Sach", new { @ms = ms });
        }


        //    // Remove an item from the cart
        public ActionResult XoaGioHang(int ms)
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            int userId = Convert.ToInt32(Session["id"]);
            var cart = GetOrCreateCart(userId);

            // Find the cart detail item
            var cartDetail = db.cart_details.FirstOrDefault(cd => cd.id_cart == cart.id && cd.id_book == ms);

            if (cartDetail != null)
            {
                // Remove the item from the cart
                db.cart_details.DeleteOnSubmit(cartDetail);
                db.SubmitChanges();
            }

            return RedirectToAction("GioHang");
        }

        [HttpGet]
        public ActionResult ThanhToan()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            int userId = Convert.ToInt32(Session["id"]);
            var cart = GetOrCreateCart(userId);

            // Find the cart detail item
            var cartDetail = db.cart_details.FirstOrDefault(cd => cd.id_cart == cart.id);
            if (cartDetail == null)
            {
                return RedirectToAction("TatCaSach", "Sach");
            }
           
            // Lấy thông tin khách hàng hiện tại
            var existingUser = db.users.FirstOrDefault(u => u.id == userId);
            if (existingUser == null)
            {
                return HttpNotFound("Không tìm thấy người dùng.");
            }

            // Tạo đối tượng thongTinKH từ thông tin người dùng
            var khachHang = new thongTinKH
            {
                MaKH = existingUser.id,
                HoTen = existingUser.full_name,
                Email = existingUser.email,
                DiaChi = existingUser.address,
                SoDienThoai = existingUser.phone
            };
            
            if (Session["payment"] == null)
            {
                Session["payment"] = ""; // Giá trị mặc định nếu cần
            }
            if (Session["urlPayment"] != null)
            {
                pageLoad();
            }
            
            return View(khachHang);
        }
        
        private List<GioHang> LayGioHang()
        {
            if (Session["id"] == null)
            {
                return new List<GioHang>(); 
            }

            int userId = Convert.ToInt32(Session["id"]);

            var cart = db.carts.FirstOrDefault(c => c.id_user == userId);
            if (cart == null)
            {
                return new List<GioHang>(); 
            }

            var gioHangItems = from cd in db.cart_details
                               join b in db.books on cd.id_book equals b.id
                               where cd.id_cart == cart.id
                               select new GioHang
                               {
                                   MaSach = b.id,
                                   TenSach = b.name,
                                   SoLuong = cd.quantity,
                                   DonGia = cd.price,                                   
                                   ThanhTien = cd.quantity * cd.price
                               };
           
            
            return gioHangItems.ToList();
        }


        [HttpPost]
        
        public JsonResult ThanhToanND(FormCollection form, thongTinKH khachHang)
        {
            if (Session["id"] == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để tiếp tục thanh toán." });
            }
            if(form["HoTen"] == "" || form["SoDienThoai"] == "" || form["DiaChi"] == "")
            {
                return Json(new { success = false, message = "Bạn cần nhập đầy đủ thông tin có dấu (*) bên trên!." });
            }
            try
            {
                int userId = Convert.ToInt32(Session["id"]);
                var deliveryMethod = form["delivery_type"];
                var note = form["note"];
                var paymentMethod = form["payment_method"];
                if (string.IsNullOrEmpty(paymentMethod))
                {
                    return Json(new { success = false, message = "Vui lòng chọn phương thức thanh toán." });
                }

                if (string.IsNullOrEmpty(deliveryMethod))
                {
                    return Json(new { success = false, message = "Vui lòng chọn phương thức giao hàng." });
                }

                var existingUser = db.users.FirstOrDefault(u => u.id == userId);
                
                if (existingUser != null)
                {
                    
                    existingUser.full_name = khachHang.HoTen;
                    
                    existingUser.phone = khachHang.SoDienThoai;
                    if(existingUser.email != null)
                    {
                        
                        existingUser.email = khachHang.Email;
                    }
                    
                    existingUser.address = khachHang.DiaChi;
                    db.SubmitChanges();
                }
                List<GioHang> gioHang = LayGioHang();

                decimal originalTotal = gioHang.Sum(item => item.ThanhTien);
                decimal finalTotal = Session["Total"] != null ? (decimal)Session["Total"] : originalTotal;
                bill bill;
                if(Convert.ToInt32(paymentMethod) == 2)
                {
                    bill = new bill
                    {
                        id_user = userId,
                        create_date = DateTime.Now,
                        status = "Đang xử lý",
                        id_method = Convert.ToInt32(paymentMethod),
                        total_price = finalTotal,
                        id_shipping = 2,
                        receive = Convert.ToInt32(deliveryMethod) == 1 ? "Nhận Hàng Tại Trung Tâm" : "Nhận Hàng Tại Nhà",
                        note = note
                    };
                    db.bills.InsertOnSubmit(bill);
                    db.SubmitChanges();
                }
                else 
                {
                    bill = db.bills.OrderByDescending(b => b.id).Where(u => u.id_user == userId).First();
                }
                

                foreach (var item in gioHang)
                {
                    var billDetail = new bill_detail
                    {
                        id_bill = bill.id,
                        id_book = item.MaSach,
                        quantity = item.SoLuong,
                        price = item.DonGia,
                        vat = 0 
                    };
                    db.bill_details.InsertOnSubmit(billDetail);
                }
                db.SubmitChanges();
                foreach (var item in gioHang)
                {
                    var bookToUpdate = db.books.FirstOrDefault(b => b.id == item.MaSach);
                    
                    var soLuong = bookToUpdate.quantity;
                    if (bookToUpdate != null)
                    {
                        bookToUpdate.number_of_purchases += 1;
                        bookToUpdate.quantity -= item.SoLuong;
                        if(bookToUpdate.quantity_retained == null)
                        {
                            bookToUpdate.quantity_retained = item.SoLuong;
                        }
                        else
                        {
                            bookToUpdate.quantity_retained += item.SoLuong;
                        }
                    }
                    if(bookToUpdate.quantity < 0)
                    {
                        return Json(new { success = false, message = "sản phẩm chỉ còn "+ soLuong+" cuốn" });
                    }
                    
                    db.SubmitChanges();
                }

                foreach (var item in gioHang)
                {
                    var history = new purchase_history
                    {
                        id_user = userId,
                        id_book = item.MaSach,
                        purchase_date = DateTime.Now,
                        quantity = item.SoLuong,
                        total_price = item.DonGia,
                        id_payment_method = Convert.ToInt32(paymentMethod),
                        id_shipping = Convert.ToInt32(deliveryMethod),
                        status = 1
                    };
                    db.purchase_histories.InsertOnSubmit(history);
                }
                db.SubmitChanges();
                

                // Clear cart items after checkout
                var cart = GetOrCreateCart(userId);
                List<cart_detail> cartDetail = db.cart_details.Where(cd => cd.id_cart == cart.id).ToList();
                foreach (var item in cartDetail)
                {
                    db.cart_details.DeleteOnSubmit(item);
                }
                db.SubmitChanges();
                
                if (bill != null && Session["payment"] != "")
                {
                    bill.status = "Đã thanh toán";
                    db.SubmitChanges();
                }

                if (Convert.ToInt32(paymentMethod) == 2)
                {
                    return Json(new { success = true, message = "Cảm ơn bạn đã mua hàng. Sách sẽ được giao tới bạn khoảng 3 - 5 ngày" });
                }
                else
                {
                    return Json(new { success = true, message = "Cảm ơn bạn đã mua hàng. Bạn hãy chờ xác nhận giao dịch thành công"});
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, message = "Thanh toán thất bại! Vui lòng thử lại sau." });
            }
        }

        

        public JsonResult ApplyDiscountCode(string discountCode)
        {
            UpdateExpiredDiscounts();
            var discount = db.discounts.FirstOrDefault(d => d.discount_code == discountCode && d.status == 1
                                                            && d.start_date <= DateTime.Now
                                                            && d.end_date >= DateTime.Now);

            if (discount == null)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết hạn." });
            }
            
            int userId = Convert.ToInt32(Session["id"]);
            var cart = GetOrCreateCart(userId);
            
            decimal originalTotal = cart.cart_details.Sum(cd => cd.quantity * cd.price); 
            decimal discountedTotal = 0;
            if (discount.discount_amount.HasValue && originalTotal < discount.discount_amount.Value * 2.67m)
            {
                return Json(new
                {
                    success = false,
                    message = $"Mã giảm giá {discount.discount_amount.Value:N0} chỉ áp dụng cho đơn hàng từ {(discount.discount_amount.Value * 2.67m):N0} trở lên."
                });
            }
            if (discount.discount_percentage != null && (int)discount.discount_amount == 0)
            {
                discountedTotal = originalTotal * (1 - (decimal)discount.discount_percentage / 100);
            }
            else if (discount.discount_amount != null && (int)discount.discount_percentage == 0)
            {
                discountedTotal = originalTotal - (decimal)discount.discount_amount;
            }
            Session["Total"] = originalTotal - discountedTotal;
            discount.status = 0;
            db.SubmitChanges();
            
            return Json(new
            {
                success = true,
                discountedTotal = discountedTotal.ToString("N0"), 
                discountAmount = discountedTotal.ToString("N0")
                
            });
        }

        
        public void UpdateExpiredDiscounts()
        {
            var expiredDiscounts = db.discounts.Where(d => d.end_date < DateTime.Now && d.status == 1).ToList();

            foreach (var discount in expiredDiscounts)
            {
                discount.status = 0;
            }

            db.SubmitChanges();
        }
      
        public ActionResult Payment()
        {
            if (Session["id"] == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }

            int userId = Convert.ToInt32(Session["id"]);
            string paymentUrl = UrlPayment(userId);

            return Redirect(paymentUrl);
        }

        public ActionResult GioHangPartial()
        {
            int ms = Convert.ToInt32(Session["id"]);
            var cart = GetOrCreateCart(ms);
            ViewBag.TongSoLuong = cart.cart_details.Count();
            return View();
        }

        private static readonly ILog log =
          LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected string UrlPayment(int userId)
        {
            
            // Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; // URL nhận kết quả trả về
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; // URL thanh toán của VNPAY
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; // Terminal Id
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; // Secret Key

            // Tìm giỏ hàng của người dùng
            var cart = GetOrCreateCart(userId);
            if (cart.cart_details.Count == 0)
            {
                throw new Exception("Giỏ hàng trống. Không thể thực hiện thanh toán.");
            }

            // Tạo hóa đơn trước khi chuyển hướng đến VNPAY
            var bill = new bill
            {
                id_user = userId,
                create_date = DateTime.Now,
                status = "Chờ thanh toán",
                id_method = 1, // Giả định 3 là phương thức VNPAY
                total_price = cart.cart_details.Sum(cd => cd.quantity * cd.price),
                id_shipping = 1
            };

            db.bills.InsertOnSubmit(bill);
            db.SubmitChanges();

           

            // Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((int)bill.total_price * 100).ToString()); // Tổng tiền nhân 100
            //vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán hóa đơn #" + bill.id);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", bill.id.ToString()); // Gán bill ID làm mã tham chiếu giao dịch

            // Tạo URL thanh toán
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            Session["urlPayment"] = paymentUrl;
            return paymentUrl;
        }

        public void pageLoad()
        {
            log.InfoFormat("Begin VNPAY Return, URL={0}", Request.RawUrl);
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];
                
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        //Thanh toan thanh cong
                        Session["payment"] = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                        log.InfoFormat("Thanh toan thanh cong, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        Session["payment"] = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                        log.InfoFormat("Thanh toan loi, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                    }
                }
                else
                {
                    log.InfoFormat("Invalid signature, InputData={0}", Request.RawUrl);
                    Session["payment"] = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }
        }

    }
}