using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLBS;
using DAL_QLBS;

namespace BLL_QLBS
{
    public class BillDetailBLL
    {
        private BillDetailDAL billDetailDAL;

        public BillDetailBLL()
        {
            billDetailDAL = new BillDetailDAL();
        }
        public List<BillDetailReportDTO> GetBillDetails(int idBill)
        {
            try
            {
                // Gọi phương thức từ BillDetailDAL để lấy danh sách chi tiết hóa đơn
                return billDetailDAL.GetBillDetailsByBillId(idBill);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
                Console.WriteLine($"Error in GetBillDetails: {ex.Message}");
                throw new Exception("Failed to retrieve bill details.", ex);
            }
        }
        public List<BillDetailReportDTO> GetBillDetails_LastGuest(int billId)
        {
            return billDetailDAL.GetBillDetails_LastGuest(billId);
        }
    }
}
