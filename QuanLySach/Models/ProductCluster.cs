using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLySach.Models
{
    public class ProductCluster
    {
        [ColumnName("PredictedLabel")]
        public uint Cluster { get; set; }

        [ColumnName("Score")]
        public float[] Distances { get; set; }
    }
}