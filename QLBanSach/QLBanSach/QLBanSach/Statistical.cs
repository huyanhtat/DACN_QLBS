using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DTO_QLBS;

namespace QLBanSach
{
    public partial class Statistical : Form
    {
        connect cnt = new connect();
        public Statistical()
        {
            InitializeComponent();
            // Thêm các loại thống kê vào ComboBox
            cmbStatisticsType.Items.Add("Ngày");
            cmbStatisticsType.Items.Add("Tháng");
            cmbStatisticsType.Items.Add("Năm");
            cmbStatisticsType.SelectedIndex = 0; // Mặc định là "Ngày"
            //================================
            btnGenerateReport.Click += BtnGenerateReport_Click;
        }

        private void BtnGenerateReport_Click(object sender, EventArgs e)
        {
            string query = "";
            string labelColumn = "";

            DateTime startDate = dtpStartDate.Value;
            DateTime endDate = dtpEndDate.Value;

            // Xác định truy vấn dựa trên loại thống kê
            switch (cmbStatisticsType.SelectedItem.ToString())
            {
                case "Ngày":
                    query = @"SELECT 
                        CONVERT(VARCHAR(10), create_date, 120) AS Date,
                        SUM(total_price) AS Revenue
                      FROM bill
                      WHERE create_date BETWEEN @StartDate AND @EndDate
                      GROUP BY CONVERT(VARCHAR(10), create_date, 120)
                      ORDER BY Date;";
                    labelColumn = "Date";
                    break;

                case "Tháng":
                    query = @"SELECT 
                        FORMAT(create_date, 'yyyy-MM') AS Month,
                        SUM(total_price) AS Revenue
                      FROM bill
                      WHERE create_date BETWEEN @StartDate AND @EndDate
                      GROUP BY FORMAT(create_date, 'yyyy-MM')
                      ORDER BY Month;";
                    labelColumn = "Month";
                    break;

                case "Năm":
                    query = @"SELECT 
                        YEAR(create_date) AS Year,
                        SUM(total_price) AS Revenue
                      FROM bill
                      WHERE create_date BETWEEN @StartDate AND @EndDate
                      GROUP BY YEAR(create_date)
                      ORDER BY Year;";
                    labelColumn = "Year";
                    break;
            }

            // Lấy dữ liệu từ cơ sở dữ liệu
            DataTable data = GetRevenueData(query, startDate, endDate);

            // Hiển thị dữ liệu trên biểu đồ
            DisplayChart(data, labelColumn, "Revenue");
        }

        private DataTable GetRevenueData(string query, DateTime startDate, DateTime endDate)
        {
            string connectionString = cnt.ConnectionString;
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
            }

            return dt;
        }

        private void DisplayChart(DataTable data, string labelColumn, string valueColumn)
        {
            ChartValues<double> values = new ChartValues<double>();
            List<string> labels = new List<string>();

            foreach (DataRow row in data.Rows)
            {
                labels.Add(row[labelColumn].ToString());
                values.Add(Convert.ToDouble(row[valueColumn]));
            }

            cartesianChartRevenue.Series.Clear();
            cartesianChartRevenue.AxisX.Clear();
            cartesianChartRevenue.AxisY.Clear();

            // Thêm dữ liệu vào biểu đồ
            cartesianChartRevenue.Series.Add(new ColumnSeries
            {
                Title = "Doanh thu",
                Values = values
            });

            // Thêm trục X
            cartesianChartRevenue.AxisX.Add(new Axis
            {
                Title = labelColumn,
                Labels = labels
            });

            // Thêm trục Y
            cartesianChartRevenue.AxisY.Add(new Axis
            {
                Title = "Doanh thu (VNĐ)"
            });
        }

       

        private void Statistical_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit(); // Thoát ứng dụng
        }
    }
}
