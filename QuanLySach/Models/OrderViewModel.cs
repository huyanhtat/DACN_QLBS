using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLySach.Models
{
    public class OrderViewModel
    {
        // Thông tin chung của đơn hàng
        public int BillID { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingMethod { get; set; }
        public string TrakingShipping { get; set; }
        public string Description { get; set; }

        public string Receive { get; set; }

        // Danh sách chi tiết đơn hàng
        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }

}