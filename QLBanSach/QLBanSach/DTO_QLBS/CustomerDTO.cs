using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QLBS
{
    public class CustomerDTO
    {
        public int id { get; set; }
        public string full_name { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public DateTime date_of_birth { get; set; }
    }
}
