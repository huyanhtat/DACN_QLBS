using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QLBS
{
    public class BillDetailReportDTO
    {
        public string BookName { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public decimal Total { get; set; }
    }
}
