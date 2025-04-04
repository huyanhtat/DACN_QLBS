using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLySach.Models
{
    public class ChiTietSach
    {
        public int Id { get; set; }
        public string tenSach { get; set; }
        public string moTa { get; set; }
        public float gia { get; set; }
        public string tenTacGia { get; set; }
        public string chuDe { get; set; }
        public string NXB { get; set; }
        public float number_of_views { get; set; }
        public float number_of_purchases { get; set; }
        public string bio { get; set; }
        //kmean
        public int ClusterId { get; set; }
        public int soLuong { get; set; }
    }
}