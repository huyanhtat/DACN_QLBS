using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QLBS
{
    public class BillDTO
    {
        public int ID { get; set; }
        public string nameBook { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public decimal VAT { get; set; }
        public decimal totalPrice { get; set; }
        public string nameMethod { get; set; }
    }
}
