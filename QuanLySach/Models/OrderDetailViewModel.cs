using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLySach.Models
{
    public class OrderDetailViewModel
    {
        public string BookName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? VAT { get; set; }
        public decimal TotalPrice { get; set; }
    }
}