using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLBS;
using ENUM;

namespace DAL_QLBS
{
    public class DiscountDAL
    {
        private BookManagementDataContext qlbs = new BookManagementDataContext();
        public DiscountDAL()
        {
            
        }
        // Thêm một khuyến mãi mới
        public bool AddDiscount(discount discount)
        {
            try
            {
                qlbs.discounts.InsertOnSubmit(discount);
                qlbs.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Cập nhật một khuyến mãi
        public bool UpdateDiscount(discount discount)
        {
            try
            {
                var existingDiscount = qlbs.discounts.FirstOrDefault(d => d.id == discount.id);
                if (existingDiscount != null)
                {
                    existingDiscount.discount_code = discount.discount_code;
                    existingDiscount.description = discount.description;
                    existingDiscount.discount_percentage = discount.discount_percentage;
                    existingDiscount.discount_amount = discount.discount_amount;
                    existingDiscount.start_date = discount.start_date;
                    existingDiscount.end_date = discount.end_date;
                    existingDiscount.status = discount.status;
                    qlbs.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Xóa một khuyến mãi
        public bool DeleteDiscount(int id)
        {
            try
            {
                var discount = qlbs.discounts.FirstOrDefault(d => d.id == id);
                if (discount != null)
                {
                    qlbs.discounts.DeleteOnSubmit(discount);
                    qlbs.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Lấy tất cả các khuyến mãi
        public List<discount> GetAllDiscounts()
        {
            return qlbs.discounts.ToList();
        }
    }
}
