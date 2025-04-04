using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Helpers
{
    public static class ReportHelper
    {
        public static DataTable GetBillDataTable()
        {
            var table = new DataTable("Bill");
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("total_price", typeof(decimal));
            table.Columns.Add("method_name", typeof(string));
            table.Columns.Add("shipping_name", typeof(string));
            table.Columns.Add("user_name", typeof(string));
            table.Columns.Add("create_date", typeof(DateTime));
            table.Columns.Add("status", typeof(string));
            return table;
        }

        public static DataTable GetBillDetailDataTable()
        {
            var table = new DataTable("BillDetail");
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("id_bill", typeof(int));
            table.Columns.Add("book_name", typeof(string));
            table.Columns.Add("quantity", typeof(int));
            table.Columns.Add("price", typeof(decimal));
            table.Columns.Add("vat", typeof(decimal));
            table.Columns.Add("total", typeof(decimal));
            return table;
        }
    }
}
