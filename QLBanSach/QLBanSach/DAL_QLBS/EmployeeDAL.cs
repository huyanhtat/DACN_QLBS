using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DTO_QLBS;
using ENUM;

namespace DAL_QLBS
{
    public class EmployeeDAL
    {
        BookManagementDataContext qlbs = new BookManagementDataContext();

        public EmployeeDAL() { }

        // Thêm nhân viên mới
        public bool AddEmployee(user employee)
        {
            try
            {
                qlbs.users.InsertOnSubmit(employee);
                qlbs.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding employee: {ex}");
                MessageBox.Show($"Error adding employee: {ex.Message}");
                return false;
            }
        }

        // Lấy danh sách tất cả nhân viên có code_role là "EMPLOYEE" (chỉ bao gồm các thuộc tính cần thiết)
        public List<EmployeeDTO> GetAllEmployees()
        {
            try
            {
                return qlbs.users
                           .Where(u => u.code_role == "EMPLOYEE" || u.code_role == "SALER")
                           .Select(u => new EmployeeDTO
                           {
                               id = u.id,
                               full_name = u.full_name,
                               phone = u.phone,
                               address = u.address,
                               email = u.email,
                               gender = u.gender,
                               date_of_birth = (DateTime)u.date_of_birth,
                               user_name = u.user_name,
                               password = u.password
                           })
                           .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving employees: {ex.Message}");
                return new List<EmployeeDTO>();
            }
        }

        // Tìm nhân viên theo ID
        public user GetEmployeeById(int employeeId)
        {
            try
            {
                return qlbs.users.FirstOrDefault(e => e.id == employeeId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving employee by ID: {ex.Message}");
                return null;
            }
        }

        public IQueryable SearchEmployee(string data)
        {
            var d = from e in qlbs.users
                    where (e.full_name.Contains(data)) && e.code_role != "ADMIN" && e.code_role != "USER"
                    select new { e.id, e.full_name, e.phone, e.address, e.email, e.gender, e.user_name, e.password, e.date_of_birth };
            return d;
        }

        // Cập nhật thông tin nhân viên
        public bool UpdateEmployee(user employee)
        {
            try
            {
                var existingEmployee = qlbs.users.FirstOrDefault(e => e.id == employee.id);
                if (existingEmployee != null)
                {
                    existingEmployee.full_name = employee.full_name;
                    existingEmployee.phone = employee.phone;
                    existingEmployee.address = employee.address;
                    existingEmployee.email = employee.email;
                    existingEmployee.image = employee.image;
                    existingEmployee.image64bit = employee.image64bit;
                    existingEmployee.user_name = employee.user_name;
                    existingEmployee.password = employee.password;
                    existingEmployee.gender = employee.gender;
                    existingEmployee.date_of_birth = employee.date_of_birth;
                    existingEmployee.status = employee.status;
                    existingEmployee.create_date = employee.create_date;
                    existingEmployee.create_by = employee.create_by;
                    existingEmployee.modified_date = employee.modified_date;
                    existingEmployee.modified_by = employee.modified_by;
                    existingEmployee.code_role = employee.code_role;

                    qlbs.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating employee: {ex.Message}");
                return false;
            }
        }

        // Xóa nhân viên theo ID
        public bool DeleteEmployee(int employeeId)
        {
            try
            {
                var employee = qlbs.users.FirstOrDefault(e => e.id == employeeId);
                if (employee != null)
                {
                    qlbs.users.DeleteOnSubmit(employee);
                    qlbs.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting employee: {ex.Message}");
                return false;
            }
        }
    }
}
