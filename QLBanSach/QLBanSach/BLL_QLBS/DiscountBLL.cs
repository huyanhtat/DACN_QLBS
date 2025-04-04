using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLBS;
using DAL_QLBS;

namespace BLL_QLBS
{
    public class DiscountBLL
    {
        private DiscountDAL discountDAL;
        public DiscountBLL()
        {
            discountDAL = new DiscountDAL();
        }
        public bool AddDiscount(discount discount)
        {
            return discountDAL.AddDiscount(discount);
        }

        public bool UpdateDiscount(discount discount)
        {
            return discountDAL.UpdateDiscount(discount);
        }

        public bool DeleteDiscount(int id)
        {
            return discountDAL.DeleteDiscount(id);
        }

        public List<discount> GetAllDiscounts()
        {
            return discountDAL.GetAllDiscounts();
        }
    }
}
