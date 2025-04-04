using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DTO_QLBS;
using ENUM;

namespace DAL_QLBS
{
    public class CustomerDAL
    {
        BookManagementDataContext qlbs = new BookManagementDataContext();

        public CustomerDAL() { }

        // Thêm khách hàng mới
        public bool AddCustomer(user customer)
        {
            try
            {
                qlbs.users.InsertOnSubmit(customer);
                qlbs.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding customer: {ex}");
                MessageBox.Show($"Error adding customer: {ex.Message}");
                return false;
            }
        }

        // Lấy danh sách tất cả khách hàng có code_role là "user"
        // Lấy danh sách tất cả khách hàng có code_role là "user" (chỉ bao gồm các thuộc tính cần thiết)
        public List<CustomerDTO> GetAllCustomers()
        {
            try
            {
                return qlbs.users
                           .Where(u => u.code_role == "USER")
                           .Select(u => new CustomerDTO
                           {
                               id = u.id,
                               full_name = u.full_name,
                               phone = u.phone,
                               address = u.address,
                               email = u.email,
                               gender = u.gender,
                               date_of_birth = (DateTime)u.date_of_birth == null ? DateTime.Now: (DateTime)u.date_of_birth
                           })
                           .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving customers: {ex.Message}");
                return new List<CustomerDTO>();
            }
        }


        // Tìm khách hàng theo ID
        public user GetCustomerById(int customerId)
        {
            try
            {
                return qlbs.users.FirstOrDefault(c => c.id == customerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving customer by ID: {ex.Message}");
                return null;
            }
        }

        // Cập nhật thông tin khách hàng
        public bool UpdateCustomer(user customer)
        {
            try
            {
                var existingCustomer = qlbs.users.FirstOrDefault(c => c.id == customer.id);
                if (existingCustomer != null)
                {
                    existingCustomer.full_name = customer.full_name;
                    existingCustomer.phone = customer.phone;
                    existingCustomer.address = customer.address;
                    existingCustomer.email = customer.email;
                    existingCustomer.image = customer.image;
                    existingCustomer.image64bit = customer.image64bit;
                    existingCustomer.user_name = customer.user_name;
                    existingCustomer.password = customer.password;
                    existingCustomer.gender = customer.gender;
                    existingCustomer.date_of_birth = customer.date_of_birth;
                    existingCustomer.status = customer.status;
                    existingCustomer.create_date = customer.create_date;
                    existingCustomer.create_by = customer.create_by;
                    existingCustomer.modified_date = customer.modified_date;
                    existingCustomer.modified_by = customer.modified_by;
                    existingCustomer.code_role = customer.code_role;

                    qlbs.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating customer: {ex.Message}");
                return false;
            }
        }

        // Xóa khách hàng theo ID
        public bool DeleteCustomer(int customerId)
        {
            try
            {
                var customer = qlbs.users.FirstOrDefault(c => c.id == customerId);
                if (customer != null)
                {
                    qlbs.users.DeleteOnSubmit(customer);
                    qlbs.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting customer: {ex.Message}");
                return false;
            }
        }
    }
}
