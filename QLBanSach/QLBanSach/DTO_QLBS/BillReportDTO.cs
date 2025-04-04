using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QLBS
{
    public class BillReportDTO
    {
        public int id { get; set; }
        public string MethodName { get; set; }
        public string ShippingName { get; set; }
        public string UserName { get; set; }
        public DateTime create_date { get; set; }
        public string status { get; set; }
        public decimal total_price { get; set; }
    }
}
