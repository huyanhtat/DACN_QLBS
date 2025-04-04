using DTO_QLBS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENUM;

namespace DAL_QLBS
{
    public class PhanQuyenDAL
    {
        BookManagementDataContext qlbs = new BookManagementDataContext();
        public PhanQuyenDAL()
        {

        }
        public List<UserDTO> loadDataEmployee()
        {
            var e = (from t in qlbs.users
                     where t.code_role != Role.USER.ToString()
                     select new UserDTO
                     {
                         Id = t.id,
                         HoTen = t.full_name,
                         SoDienThoai = t.phone,
                         NgaySinh = t.date_of_birth.ToString(),
                         GioiTinh = t.gender,
                         Email = t.email,
                         DiaChi = t.address,
                         ChucVu = t.code_role
                     }).ToList();
            return e;
        }

        public List<UserDTO> searchEmployee(string name)
        {
            var e = (from t in qlbs.users
                     where t.full_name.Contains(name)
                     select new UserDTO
                     {
                         Id = t.id,
                         HoTen = t.full_name,
                         SoDienThoai = t.phone,
                         NgaySinh = t.date_of_birth.ToString(),
                         GioiTinh = t.gender,
                         Email = t.email,
                         DiaChi = t.address,
                         ChucVu = t.code_role
                     }).ToList();
            return e;
        }

        public IQueryable loadRole()
        {
            return qlbs.roles.Select(r => r.name);
        }

        public List<UserDTO> searchRole(string role)
        {
            var e = (from t in qlbs.users
                     join r in qlbs.roles on t.code_role equals r.code
                     where r.name == role
                     select new UserDTO
                     {
                         Id = t.id,
                         HoTen = t.full_name,
                         SoDienThoai = t.phone,
                         NgaySinh = t.date_of_birth.ToString(),
                         GioiTinh = t.gender,
                         Email = t.email,
                         DiaChi = t.address,
                         ChucVu = t.code_role
                     }).ToList();
            return e;
        }

        public void phanQuyen(int id, string role)
        {
            var e = qlbs.users.FirstOrDefault(t => t.id == id);
            if (e != null)
            {
                e.id = id;
                e.code_role = role.ToUpper();
                e.modified_date = DateTime.Now;
                e.modified_by = "admin";
            }
            qlbs.SubmitChanges();

        }
    }
}
