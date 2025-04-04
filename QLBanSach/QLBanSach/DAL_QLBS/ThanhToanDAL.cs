using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLBS;

namespace DAL_QLBS
{
    public class ThanhToanDAL
    {
        BookManagementDataContext db = new BookManagementDataContext();
        public ThanhToanDAL()
        {

        }

        public BookDTO GetBookByBarcode(string barcode)
        {
            var bookDetail = (from b in db.books
                              where b.barcode.Equals(barcode)
                              select new BookDTO
                              {
                                  id = b.id,
                                  name = b.name,
                                  barcode = b.barcode,
                                  price = b.price,

                              }).FirstOrDefault();

            return bookDetail;
        }

        public decimal? getDiscount(string code)
        {
            var discount = db.discounts.Where(t => t.discount_code == code && t.status == 1);
            var discountAmount = discount.Select(t => t.discount_amount).FirstOrDefault();
            var discountPercentage = discount.Select(t => t.discount_percentage).FirstOrDefault();
            if (discount != null)
            {
                if (discountAmount != null)
                {
                    return discountAmount;
                }
                else
                {
                    return discountPercentage;
                }
            }
            return 0;
        }

        public void saveBill(List<bill_detail> billDetails)
        {
            try
            {
                // Tính tổng tiền từ tất cả các chi tiết hóa đơn
                decimal totalPrice = billDetails.Sum(bd => bd.price * bd.quantity);


                var bill = new bill
                {
                    id_method = 2,
                    create_date = DateTime.Now,
                    status = "Đang Thanh Toán",
                    total_price = totalPrice
                };

                // Thêm hóa đơn vào bảng bills
                db.bills.InsertOnSubmit(bill);
                db.SubmitChanges();

                // Thêm từng chi tiết hóa đơn
                foreach (var billDetail in billDetails)
                {
                    var newBillDetail = new bill_detail
                    {
                        id_bill = bill.id,
                        id_book = billDetail.id_book,
                        quantity = billDetail.quantity,
                        price = billDetail.price,
                        vat = billDetail.vat
                    };

                    db.bill_details.InsertOnSubmit(newBillDetail);
                }

                // Lưu tất cả chi tiết hóa đơn
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving bill: " + ex.Message);
            }
        }

        public bill GetLastBill()
        {
            try
            {
                // Lấy hóa đơn mới nhất dựa trên ID lớn nhất
                var lastBill = db.bills.OrderByDescending(b => b.id).FirstOrDefault();
                return lastBill;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting last bill: " + ex.Message);
                return null;
            }
        }

    }
}
