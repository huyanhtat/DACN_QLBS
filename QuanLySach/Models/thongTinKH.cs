using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLySach.Models
{
    public class thongTinKH
    {
        public int MaKH { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        public string HoTen { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        public string DiaChi { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SoDienThoai { get; set; }

        public thongTinKH() { }

        public thongTinKH(int maKH, string hoTen, string email, string diaChi, string soDienThoai)
        {
            MaKH = maKH;
            HoTen = hoTen;
            Email = email;
            DiaChi = diaChi;
            SoDienThoai = soDienThoai;
        }
    }
}