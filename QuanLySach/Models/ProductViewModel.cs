﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLySach.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}