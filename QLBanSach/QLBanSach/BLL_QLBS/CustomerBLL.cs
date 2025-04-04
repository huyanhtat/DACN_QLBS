using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_QLBS;
using DTO_QLBS;

namespace BLL_QLBS
{
    public class CustomerBLL
    {
        private CustomerDAL customerDAL;

        public CustomerBLL()
        {
            customerDAL = new CustomerDAL();
        }

        // Thêm khách hàng mới
        public bool AddCustomer(user customer)
        {
            return customerDAL.AddCustomer(customer);
        }

        // Lấy danh sách tất cả khách hàng có code_role là "user"
        public List<CustomerDTO> GetAllCustomers()
        {
            return customerDAL.GetAllCustomers();
        }


        // Tìm khách hàng theo ID
        public user GetCustomerById(int customerId)
        {
            return customerDAL.GetCustomerById(customerId);
        }

        // Cập nhật thông tin khách hàng
        public bool UpdateCustomer(user customer)
        {
            return customerDAL.UpdateCustomer(customer);
        }

        // Xóa khách hàng theo ID
        public bool DeleteCustomer(int customerId)
        {
            return customerDAL.DeleteCustomer(customerId);
        }

    }
}
