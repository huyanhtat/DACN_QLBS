using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QLBS
{
    public class BookDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string barcode { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public string description { get; set; }
        public int id_publisher { get; set; }
        public string code_category { get; set; }
        public byte[] image { get; set; }
        public byte? priority { get; set; }
        public byte? status { get; set; }
    }
}
