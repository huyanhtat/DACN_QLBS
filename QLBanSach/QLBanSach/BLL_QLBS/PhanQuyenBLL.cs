using DTO_QLBS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_QLBS;

namespace BLL_QLBS
{
    public class PhanQuyenBLL
    {
        PhanQuyenDAL p = new PhanQuyenDAL();
        public PhanQuyenBLL()
        {

        }
        public List<UserDTO> loadDataEmployee()
        {
            return p.loadDataEmployee();
        }
        public void phanQuyen(int id, string role)
        {
            p.phanQuyen(id, role);
        }

        public IQueryable loadRole()
        {
            return p.loadRole();
        }
        public List<UserDTO> searchRole(string role)
        {
            return p.searchRole(role);
        }

        public List<UserDTO> searchEmployee(string name)
        {
            return p.searchEmployee(name);
        }
    }
}
