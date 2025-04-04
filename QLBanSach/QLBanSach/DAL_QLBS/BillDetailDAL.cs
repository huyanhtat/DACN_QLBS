using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLBS;
using ENUM;

namespace DAL_QLBS
{
    public class BillDetailDAL
    {
        private BookManagementDataContext qlbs = new BookManagementDataContext();

        public BillDetailDAL()
        {
            
        }
        public List<BillDetailReportDTO> GetBillDetailsByBillId(int idBill)
        {
            try
            {
                var billDetails = (from bd in qlbs.bill_details
                                   join b in qlbs.books on bd.id_book equals b.id
                                   where bd.id_bill == idBill
                                   select new BillDetailReportDTO
                                   {
                                       
                                       BookName = b.name,
                                       quantity = bd.quantity,
                                       price = bd.price,
                                       Total = (decimal)((bd.quantity * bd.price) * (1 + (bd.vat / 100)))
                                   }).ToList();

                return billDetails;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBillDetailsByBillId: {ex.Message}");
                throw new Exception("Failed to retrieve bill details by ID.", ex);
            }
        }
        public List<BillDetailReportDTO> GetBillDetails_LastGuest(int billId)
        {
            try
            {
                var billDetails = qlbs.bill_details
                    .Where(bd => bd.id_bill == billId)
                    .Select(bd => new BillDetailReportDTO
                    {
                        BookName = bd.book.name,
                        quantity = bd.quantity,
                        price = bd.price,
                        Total = bd.quantity * bd.price
                    })
                    .ToList();

                return billDetails;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting bill details: " + ex.Message);
                return new List<BillDetailReportDTO>();
            }
        }
    }
}
