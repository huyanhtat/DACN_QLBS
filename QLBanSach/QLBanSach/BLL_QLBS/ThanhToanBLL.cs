using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_QLBS;
using DTO_QLBS;

namespace BLL_QLBS
{
    public class ThanhToanBLL
    {
        ThanhToanDAL d = new ThanhToanDAL();

        public ThanhToanBLL()
        {

        }

        public BookDTO GetBookByBarcode(string barcode)
        {
            return d.GetBookByBarcode(barcode);
        }
        public void saveBill(List<bill_detail> billDetails)
        {
            d.saveBill(billDetails);
        }
        public decimal? getDiscount(string code)
        {
            return d.getDiscount(code);
        }
        public bill GetLastBill()
        {
            return d.GetLastBill();
        }
    }
}
