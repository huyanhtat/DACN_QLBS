using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLySach.Models
{
    public class CategoryViewModel
    {
        public string name { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }
}