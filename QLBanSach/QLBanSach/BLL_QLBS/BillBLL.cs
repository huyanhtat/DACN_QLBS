using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLBS;
using DAL_QLBS;

namespace BLL_QLBS
{
    public class BillBLL
    {
        private BillDAL billDAL;

        public BillBLL()
        {
            billDAL = new BillDAL();
        }

        public List<dynamic> LoadBills()
        {
            try
            {
                // Gọi phương thức từ BillDAL để lấy danh sách hóa đơn
                return billDAL.GetBills();
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần (có thể sử dụng log framework như NLog hoặc Serilog)
                Console.WriteLine($"Error loading bills: {ex.Message}");
                throw new Exception("Failed to load bills from the database.", ex);
            }
        }
        public List<dynamic> SearchReceive(string data)
        {
            return billDAL.SearchReceive(data);
        }

        public List<dynamic> GetReceive()
        {
            return billDAL.GetReceive();
        }
        public dynamic GetBillById(int billId)
        {
            try
            {
                // Gọi phương thức từ BillDAL để lấy thông tin hóa đơn theo ID
                return billDAL.GetBillById(billId);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                Console.WriteLine($"Error in BLL GetBillById: {ex.Message}");
                throw new Exception("Failed to load bill by ID.", ex);
            }
        }

        public BillReportDTO GetLatestBillForGuest()
        {
            return billDAL.GetLatestBillForGuest();
        }

        public bool UpdateBillStatus(int billId, string status)
        {
            return billDAL.UpdateBillStatus(billId, status);
        }
        public bool UpdateBillReceive(int billId, string status)
        {
            return billDAL.UpdateBillReceive(billId, status);
        }
    }
}
