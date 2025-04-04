using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QLBS
{
    public class UserDTO
    {
        private int id;
        private string hoTen;
        private string soDienThoai;
        private string diaChi;
        private string email;
        private string ngaySinh;
        private string gioiTinh;
        private string chucVu;

        public string HoTen { get => hoTen; set => hoTen = value; }
        public string SoDienThoai { get => soDienThoai; set => soDienThoai = value; }
        public string DiaChi { get => diaChi; set => diaChi = value; }
        public string Email { get => email; set => email = value; }
        public string NgaySinh { get => ngaySinh; set => ngaySinh = value; }
        public string GioiTinh { get => gioiTinh; set => gioiTinh = value; }
        public string ChucVu { get => chucVu; set => chucVu = value; }
        public int Id { get => id; set => id = value; }
    }
}