using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLBS;
using ENUM;

namespace DAL_QLBS
{
    public class BillDAL
    {
        private BookManagementDataContext qlbs = new BookManagementDataContext();

        public BillDAL()
        {

        }

        public List<dynamic> GetReceive()
        {
            var bills = (from b in qlbs.bills
                        select b.receive).Distinct();

            return bills.ToList<dynamic>();
        }

        public List<dynamic> SearchReceive(string data)
        {
            var bills = from b in qlbs.bills
                        join m in qlbs.methods on b.id_method equals m.id
                        join s in qlbs.shippings on b.id_shipping equals s.id into shippingJoin
                        from sj in shippingJoin.DefaultIfEmpty()
                        join u in qlbs.users on b.id_user equals u.id into userJoin
                        from uj in userJoin.DefaultIfEmpty()
                        where b.receive.Contains(data)
                        select new
                        {
                            b.id,
                            b.total_price,
                            MethodName = m.name,
                            ShippingName = sj != null ? sj.method_name : "N/A",
                            UserName = uj != null ? uj.full_name : "N/A",
                            b.create_date,
                            b.status,
                            b.note
                        };

            return bills.ToList<dynamic>();
        }

        public List<dynamic> GetBills()
        {
            var bills = from b in qlbs.bills
                        join m in qlbs.methods on b.id_method equals m.id
                        join s in qlbs.shippings on b.id_shipping equals s.id into shippingJoin
                        from sj in shippingJoin.DefaultIfEmpty()
                        join u in qlbs.users on b.id_user equals u.id into userJoin
                        from uj in userJoin.DefaultIfEmpty()
                        where b.status != "Đã nhận hàng" && b.status != "Chờ thanh toán"
                        select new
                        {
                            b.id,
                            b.total_price,
                            MethodName = m.name,
                            ShippingName = sj != null ? sj.method_name : "N/A",
                            UserName = uj != null ? uj.full_name : "N/A",
                            b.create_date,
                            b.status,
                            b.note
                        };

            return bills.ToList<dynamic>();
        }

        public BillReportDTO GetBillById(int billId)
        {
            try
            {
                var bill = (from b in qlbs.bills
                            join m in qlbs.methods on b.id_method equals m.id
                            join s in qlbs.shippings on b.id_shipping equals s.id into shippingJoin
                            from sj in shippingJoin.DefaultIfEmpty()
                            join u in qlbs.users on b.id_user equals u.id into userJoin
                            from uj in userJoin.DefaultIfEmpty()
                            where b.id == billId
                            select new BillReportDTO
                            {
                                id = b.id,
                                MethodName = m.name,
                                ShippingName = sj != null ? sj.method_name : "N/A",
                                UserName = uj != null ? uj.full_name : "N/A",
                                create_date = b.create_date,
                                status = b.status,
                                total_price = b.total_price
                            }).FirstOrDefault();

                return bill;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBillById: {ex.Message}");
                throw new Exception("Failed to retrieve bill by ID.", ex);
            }
        }

        public BillReportDTO GetLatestBillForGuest()
        {
            try
            {
                var latestBill = qlbs.bills
                    .Where(b => b.id_user == null)
                    .OrderByDescending(b => b.id)
                    .Select(b => new BillReportDTO
                    {
                        id = b.id,
                        UserName = "Khách vãng lai",
                        create_date = b.create_date,
                        status = b.status,
                        total_price = b.total_price
                    })
                    .FirstOrDefault();

                return latestBill;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting latest bill for guest: " + ex.Message);
                return null;
            }
        }
        public bool UpdateBillStatus(int billId, string status)
        {
            try
            {
                // Lấy hóa đơn từ cơ sở dữ liệu
                var bill = qlbs.bills.FirstOrDefault(b => b.id == billId);

                if (bill == null)
                {
                    return false; // Không tìm thấy hóa đơn
                }

                // Cập nhật trạng thái
                bill.status = status;

                // Lưu thay đổi vào cơ sở dữ liệu
                qlbs.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating bill status: {ex.Message}");
                return false;
            }
        }

        public bool UpdateBillReceive(int billId, string status)
        {
            try
            {
                // Lấy hóa đơn từ cơ sở dữ liệu
                var bill = qlbs.bills.FirstOrDefault(b => b.id == billId);

                if (bill == null)
                {
                    return false; // Không tìm thấy hóa đơn
                }

                // Cập nhật trạng thái của hóa đơn
                bill.status = status;

                
                    var billDetails = qlbs.bill_details.Where(bd => bd.id_bill == billId).ToList();

                    foreach (var bd in billDetails)
                    {
                        // Cập nhật số lượng sách
                        var book = qlbs.books.FirstOrDefault(b => b.id == bd.id_book);
                        if (book != null)
                        {
                            book.quantity -= bd.quantity;
                            // Chuyển số lượng sách đã nhận
                            book.quantity_retained = 0;
                            
                        }
                    }
                

                // Lưu thay đổi vào cơ sở dữ liệu
                qlbs.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating bill status and book quantity: {ex.Message}");
                return false;
            }
        }
    }
}
