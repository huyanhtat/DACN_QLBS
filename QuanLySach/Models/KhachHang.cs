using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLySach.Models
{
    public class KhachHang
    {
        public string tenKhachHang { get; set; }
        public string username  { get; set; }
        public string password { get; set; }    
        public string diaChi { get; set; }
        public string SDT   { get; set; }
    }
}