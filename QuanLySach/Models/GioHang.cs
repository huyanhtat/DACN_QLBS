using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLySach.Models
{
    public class GioHang
    {
        public int MaSach { get; set; }       
        public string TenSach { get; set; } 
        public int SoLuong { get; set; }      
        public decimal DonGia { get; set; } 
        public string HinhAnh { get; set; }   

        public decimal ThanhTien { get { return SoLuong * DonGia; } set { } }
        

        
    }
}