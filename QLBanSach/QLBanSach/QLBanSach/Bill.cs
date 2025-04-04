using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL_QLBS;
using DTO_QLBS;
using Microsoft.Reporting.WinForms;

namespace QLBanSach
{
    public partial class Bill : Form
    {
        private BillBLL billBLL;
        private BillDetailBLL billDetailBLL;
        private int selectedBillId = -1;
        public Bill()
        {
            InitializeComponent();
            billBLL = new BillBLL();
            billDetailBLL = new BillDetailBLL();
            txtID.Enabled = false;
            txtMethod.Enabled = false;
            txtShipping.Enabled = false;
            txtTotalPrice.Enabled = false;
            txtUser.Enabled = false;
            txtStatus.Enabled = false;
            dtpCreateDate.Enabled = false;
            txtQuantityOrder.Enabled = false;
            txtTotalOrder.Enabled = false;
            // Chi tiết hoá đơn
            txtQuantityProduct.Enabled = false;
            txtTotalBill.Enabled = false;
            LoadBillList();
            //Xử lí sự kiện
            dgvBill.CellClick += DgvBill_CellClick;
            btnClear.Click += BtnClear_Click;
            dgvBillDetail.CellClick += DgvBillDetail_CellClick;
            btnPrint.Click += BtnPrint_Click;
            btnDuyet.Click += BtnDuyet_Click;
            btnSearch.Click += BtnSearch_Click;
            
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the search text from the textbox
                string searchText = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(comboBox1.Text))
                {
                    dgvBill.DataSource = billBLL.SearchReceive(comboBox1.SelectedItem.ToString());

                    // Tính tổng tiền hoá đơn sau khi lọc
                    CalculateTotalsFromDataGridView();
                    return;
                }
                // If no search text is entered, load all the bills again
                if (string.IsNullOrEmpty(searchText))
                {
                    LoadBillList();
                    return;
                }
               

                // Filter the list of bills based on the search text
                var filteredBills = billBLL.LoadBills()
                    .Where(bill =>
                        bill.UserName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        bill.MethodName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        bill.Status.Contains(searchText, StringComparison.OrdinalIgnoreCase)||
                        bill.Receive.Contains(comboBox1.SelectedIndex.ToString(), StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                // Update the DataGridView with the filtered bills
                dgvBill.DataSource = filteredBills;

                // Tính tổng tiền hoá đơn sau khi lọc
                CalculateTotalsFromDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDuyet_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedBillId == -1)
                {
                    MessageBox.Show("Vui lòng chọn hóa đơn cần duyệt.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy hóa đơn cần thay đổi trạng thái
                var bill = billBLL.GetBillById(selectedBillId);

                if (bill == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn trong hệ thống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra trạng thái hiện tại
                if (bill.status == "Đã duyệt đơn hàng")
                {
                    MessageBox.Show("Hóa đơn này đã được duyệt.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Thay đổi trạng thái thành "Success"
                bill.status = "Đã duyệt đơn hàng";
                
                // Gọi phương thức cập nhật hóa đơn
                bool isUpdated = billBLL.UpdateBillStatus(selectedBillId, "Đã duyệt đơn hàng");

                if (isUpdated)
                {
                    MessageBox.Show("Hóa đơn đã được duyệt thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tải lại danh sách hóa đơn
                    LoadBillList();
                }
                else
                {
                    MessageBox.Show("Không thể duyệt hóa đơn. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi duyệt hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy dữ liệu hóa đơn
                var bill = billBLL.GetBillById(selectedBillId);

                // Lấy chi tiết hóa đơn
                var billDetails = billDetailBLL.GetBillDetails(selectedBillId);

                if (bill == null || billDetails == null || !billDetails.Any())
                {
                    MessageBox.Show("Không có dữ liệu để in.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Đường dẫn tới RDLC
                string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\HelpReport", "BillReport.rdlc");

                //Kiểm tra sự tồn tại của file RDLC
                if (!File.Exists(reportPath))
                {
                    MessageBox.Show("Không tìm thấy file báo cáo. Vui lòng kiểm tra lại đường dẫn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Cấu hình ReportViewer
                ReportViewer reportViewer = new ReportViewer
                {
                    ProcessingMode = ProcessingMode.Local,
                    LocalReport = { ReportPath = reportPath }
                };

                // Gắn dữ liệu vào RDLC
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Bill", new List<BillReportDTO> { bill }));
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("BillDetail", billDetails));

                // Làm mới báo cáo
                reportViewer.RefreshReport();

                // Hiển thị báo cáo
                Form reportForm = new Form
                {
                    Text = "In Hóa Đơn",
                    Width = 800,
                    Height = 600
                };
                reportViewer.Dock = DockStyle.Fill;
                reportForm.Controls.Add(reportViewer);
                reportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi in hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvBillDetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CalculateBillDetailTotals();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            LoadBillList();
        }

        private void DgvBill_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng có nhấp vào một dòng hợp lệ không
            if (e.RowIndex >= 0 && e.RowIndex < dgvBill.Rows.Count)
            {
                DataGridViewRow selectedRow = dgvBill.Rows[e.RowIndex];
                selectedBillId = Convert.ToInt32(selectedRow.Cells["id"].Value);
                
                // Lấy dữ liệu từ dòng được chọn
                txtID.Text = selectedRow.Cells["id"].Value?.ToString();
                txtTotalPrice.Text = selectedRow.Cells["total_price"].Value?.ToString();
                txtMethod.Text = selectedRow.Cells["MethodName"].Value?.ToString();
                txtShipping.Text = selectedRow.Cells["ShippingName"].Value?.ToString();
                txtUser.Text = selectedRow.Cells["UserName"].Value?.ToString();
                txtStatus.Text = selectedRow.Cells["status"].Value?.ToString();

                // Xử lý ngày tạo (đảm bảo dữ liệu không bị null)
                if (selectedRow.Cells["create_date"].Value != null)
                {
                    dtpCreateDate.Value = Convert.ToDateTime(selectedRow.Cells["create_date"].Value);
                }
                else
                {
                    dtpCreateDate.Value = DateTime.Now; // Giá trị mặc định nếu null
                }

                // Xử lí việc lấy danh sách chi tiết hoá đơn
                if (int.TryParse(selectedRow.Cells["id"].Value?.ToString(), out int billId))
                {
                    LoadBillDetails(billId);
                }
                
            }
        }
        //=======================
        private void LoadBillList()
        {
            try
            {
                // Lấy danh sách hóa đơn từ BLL
                var bills = billBLL.LoadBills();
                dgvBill.DataSource = bills;

                // Cấu hình DataGridView hiển thị
                dgvBill.Columns["id"].HeaderText = "ID";
                dgvBill.Columns["total_price"].HeaderText = "Tổng tiền";
                dgvBill.Columns["MethodName"].HeaderText = "Phương thức";
                dgvBill.Columns["ShippingName"].HeaderText = "Giao hàng";
                dgvBill.Columns["UserName"].HeaderText = "Người dùng";
                dgvBill.Columns["create_date"].HeaderText = "Ngày tạo";
                dgvBill.Columns["status"].HeaderText = "Trạng thái";

                // Tính tổng tiền hoá đơn
                CalculateTotalsFromDataGridView();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Tính tổng tiền tất cả hoá đơn
        private void CalculateTotalsFromDataGridView()
        {
            try
            {
                int totalOrders = 0; // Đếm số lượng hóa đơn
                decimal totalAmount = 0; // Tính tổng tiền

                foreach (DataGridViewRow row in dgvBill.Rows)
                {
                    // Kiểm tra dòng hợp lệ (tránh dòng trống)
                    if (row.Cells["id"].Value != null)
                    {
                        totalOrders++; // Tăng số lượng hóa đơn

                        // Tính tổng tiền từ cột total_price
                        if (decimal.TryParse(row.Cells["total_price"].Value?.ToString(), out decimal price))
                        {
                            totalAmount += price;
                        }
                    }
                }

                // Hiển thị kết quả vào các TextBox
                txtQuantityOrder.Text = totalOrders.ToString();
                txtTotalOrder.Text = totalAmount.ToString("N0"); // Hiển thị định dạng tiền tệ
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tính toán tổng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Lấy danh sách chi tiết hoá đơn theo mã hoá đơn
        private void LoadBillDetails(int billId)
        {
            try
            {
                // Gọi phương thức BLL để lấy danh sách chi tiết hóa đơn
                var billDetails = billDetailBLL.GetBillDetails(billId);

                // Kiểm tra dữ liệu
                if (billDetails == null || !billDetails.Any())
                {
                    // Nếu không có dữ liệu, xóa nội dung DataGridView và thông báo
                    dgvBillDetail.DataSource = null;
                    MessageBox.Show("Không có chi tiết hóa đơn cho hóa đơn này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Gán dữ liệu vào DataGridView
                dgvBillDetail.DataSource = billDetails;

                // Đặt tên cột hiển thị (đảm bảo các cột khớp với BillDetailReportDTO)
                
                dgvBillDetail.Columns["BookName"].HeaderText = "Tên sách";
                dgvBillDetail.Columns["quantity"].HeaderText = "Số lượng";
                dgvBillDetail.Columns["price"].HeaderText = "Giá";
                dgvBillDetail.Columns["Total"].HeaderText = "Tổng cộng";

                // Định dạng các cột số liệu
                dgvBillDetail.Columns["price"].DefaultCellStyle.Format = "N0"; // Hiển thị giá với định dạng tiền tệ
                dgvBillDetail.Columns["Total"].DefaultCellStyle.Format = "N0"; // Hiển thị tổng cộng với định dạng tiền tệ

                // Tự động điều chỉnh độ rộng cột
                dgvBillDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi nếu xảy ra vấn đề
                MessageBox.Show($"Lỗi khi tải chi tiết hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Tính tổng danh sách chi tiết hoá đơn
        private void CalculateBillDetailTotals()
        {
            try
            {
                int totalQuantity = 0; // Tổng số lượng sản phẩm
                decimal totalPrice = 0; // Tổng tiền hóa đơn

                // Duyệt qua tất cả các dòng trong DataGridView
                foreach (DataGridViewRow row in dgvBillDetail.Rows)
                {
                    // Tính tổng số lượng sản phẩm
                    if (row.Cells["quantity"].Value != null && int.TryParse(row.Cells["quantity"].Value.ToString(), out int quantity))
                    {
                        totalQuantity += quantity;
                    }

                    // Tính tổng tiền hóa đơn
                    if (row.Cells["Total"].Value != null && decimal.TryParse(row.Cells["Total"].Value.ToString(), out decimal total))
                    {
                        totalPrice += total;
                    }
                }

                // Hiển thị kết quả lên TextBox
                txtQuantityProduct.Text = totalQuantity.ToString();
                txtTotalBill.Text = totalPrice.ToString("N0"); // Hiển thị định dạng tiền tệ
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tính toán tổng chi tiết hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       

        private void Bill_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(new string[] { "Giao Hàng Tại Trung Tâm", "Giao Hàng Tại Nhà" });
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Bill_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); // Thoát ứng dụng
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedBillId == -1)
                {
                    MessageBox.Show("Vui lòng chọn hóa đơn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy hóa đơn cần thay đổi trạng thái
                var bill = billBLL.GetBillById(selectedBillId);

                if (bill == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn trong hệ thống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra trạng thái hiện tại
                if (bill.status == "Đã nhận hàng")
                {
                    MessageBox.Show("Hóa đơn này đã được nhận.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Thay đổi trạng thái thành "Success"
                bill.status = "Đã nhận hàng";

                // Gọi phương thức cập nhật hóa đơn
                bool isUpdated = billBLL.UpdateBillReceive(selectedBillId, "Đã nhận hàng");

                if (isUpdated)
                {
                    MessageBox.Show("Hóa đơn đã được nhận hàng thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tải lại danh sách hóa đơn
                    LoadBillList();
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật nhận hàng hóa đơn. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn nhận hàng hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
