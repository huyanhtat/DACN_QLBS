using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;

namespace DAL
{
    public class EmployeeDAL
    {
        BookManagementDataContext qlbs = new BookManagementDataContext();
        public EmployeeDAL()
        {

        }

        public List<customer> getALLEmployee()
        {
            return qlbs.customers.Where(e => e.code_role == "NV").toList<customer>(); 
        }
    }
}
