using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_QLBS;
using DTO_QLBS;

namespace BLL_QLBS
{
    public class EmployeeBLL
    {
        private EmployeeDAL employeeDAL;

        public EmployeeBLL()
        {
            employeeDAL = new EmployeeDAL();
        }
        public IQueryable SearchEmployee(string data)
        {
            return employeeDAL.SearchEmployee(data);
        }
        // Add a new employee
        public bool AddEmployee(user employee)
        {
            return employeeDAL.AddEmployee(employee);
        }

        // Get all employees with code_role "Employee"
        public List<EmployeeDTO> GetAllEmployees()
        {
            return employeeDAL.GetAllEmployees();
        }

        // Find employee by ID
        public user GetEmployeeById(int employeeId)
        {
            return employeeDAL.GetEmployeeById(employeeId);
        }

        // Update employee information
        public bool UpdateEmployee(user employee)
        {
            return employeeDAL.UpdateEmployee(employee);
        }

        // Delete employee by ID
        public bool DeleteEmployee(int employeeId)
        {
            return employeeDAL.DeleteEmployee(employeeId);
        }
    }
}