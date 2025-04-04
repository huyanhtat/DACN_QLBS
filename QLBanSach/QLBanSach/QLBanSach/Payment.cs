using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DTO_QLBS;
using BLL_QLBS;
using Microsoft.Reporting.WinForms;
using System.IO;

namespace QLBanSach
{
    public partial class Payment : Form
    {
        BookManagementDataContext db = new BookManagementDataContext();
        private BillBLL billBLL;
        private BillDetailBLL billDetailBLL;
        private decimal totalPrice = 0; // Tổng số tiền nhập
        private decimal sum = 0;       // Tổng số tiền hiện tại
        private decimal tongTien;      // Số tiền tổng ban đầu

        public decimal TongTien { get => tongTien; set => tongTien = value; }

        public Payment()
        {
            InitializeComponent();
            billBLL = new BillBLL();
            billDetailBLL = new BillDetailBLL();
            this.textBox8.TextChanged += TextBox8_TextChanged;
            this.button1.Click += Button1_Click;
            this.button2.Click += Button2_Click;
            this.button3.Click += Button3_Click;
            this.button6.Click += Button6_Click;
            this.button7.Click += Button7_Click;
            this.button8.Click += Button8_Click;
            this.button9.Click += Button9_Click;
            this.button5.Click += Button5_Click;
            this.button10.Click += Button10_Click;
            this.button11.Click += Button11_Click;
            this.button12.Click += Button12_Click;
            this.button14.Click += Button14_Click;
            this.button15.Click += Button15_Click;
            this.button17.Click += Button17_Click;
            this.button16.Click += Button16_Click;
            this.button18.Click += Button18_Click;
            this.button19.Click += Button19_Click;
            this.button20.Click += Button20_Click;
            this.button21.Click += Button21_Click;
            this.button22.Click += Button22_Click;
            this.button23.Click += Button23_Click;
            this.button24.Click += Button24_Click;
            this.button25.Click += Button25_Click;
            this.button26.Click += Button26_Click;
            this.button27.Click += Button27_Click;
            this.button28.Click += Button28_Click;
            this.button29.Click += Button29_Click;
            this.Load += Payment_Load1;
        }

        private void Button29_Click(object sender, EventArgs e)
        {
            Cart cart = new Cart();
            cart.Show();
            this.Hide();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy hóa đơn mới nhất có id_user = NULL (khách hàng mua trực tiếp)
                var bill = billBLL.GetLatestBillForGuest();

                // Kiểm tra nếu không tìm thấy hóa đơn
                if (bill == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn cho khách hàng mua trực tiếp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lấy chi tiết hóa đơn
                var billDetails = billDetailBLL.GetBillDetails_LastGuest(bill.id);

                // Kiểm tra nếu không có chi tiết hóa đơn
                if (billDetails == null || !billDetails.Any())
                {
                    MessageBox.Show("Không có chi tiết hóa đơn để in.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Đường dẫn tới RDLC dựa trên thư mục chạy
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
                    Text = "In Hóa Đơn - Khách Hàng Mua Trực Tiếp",
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

        private void Payment_Load1(object sender, EventArgs e)
        {
            // Hiển thị tổng tiền
            textBox1.Text = TongTien.ToString("N0") + " đ";
            textBox2.Text = TongTien.ToString("N0") + " đ";
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            try
            {
                // Hiển thị tiền thối
                MessageBox.Show("Tiền cần thối: " + textBox9.Text);

                // Lấy thông tin hóa đơn
                var billId = ((bill)this.Tag)?.id; // Lấy ID của hóa đơn từ Tag
                if (billId != null)
                {
                    // Lấy hóa đơn từ CSDL
                    var currentBill = db.bills.FirstOrDefault(b => b.id == billId);
                    if (currentBill != null)
                    {
                        if (textBox9.Text.Contains("-"))
                        {
                            MessageBox.Show("Số tiền trả chưa đủ! Quay lại tiếp tục trả.");
                            return;
                        }
                        
                        // Cập nhật trạng thái hóa đơn
                        currentBill.status = "Thanh toán thành công";
                        db.SubmitChanges();

                        // Lấy danh sách chi tiết hóa đơn
                        var billDetails = db.bill_details.Where(d => d.id_bill == billId).ToList();

                        // Trừ số lượng sản phẩm trong kho
                        foreach (var detail in billDetails)
                        {
                            var book = db.books.FirstOrDefault(b => b.id == detail.id_book);
                            if (book != null)
                            {
                                if (book.quantity >= detail.quantity)
                                {
                                    book.quantity -= detail.quantity; // Trừ số lượng
                                }
                                else
                                {
                                    MessageBox.Show($"Sản phẩm '{book.name}' không đủ số lượng trong kho.");
                                    throw new Exception("Số lượng không đủ");
                                }
                            }
                        }
                        
                        db.SubmitChanges(); // Lưu thay đổi vào CSDL

                        MessageBox.Show("Thanh toán thành công.");
                        
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy hóa đơn để cập nhật.");
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin hóa đơn trong Tag.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi!!!");
            }
        }

        private void Button12_Click(object sender, EventArgs e)
        {
            textBox8.Text += "0";
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            textBox8.Text += "9";
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            textBox8.Text += "8";
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            textBox8.Text += "7";
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            textBox8.Text += "6";
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            textBox8.Text += "5";
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            textBox8.Text += "4";
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            textBox8.Text += "3";
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            textBox8.Text += "2";
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            // Thêm giá trị vào tổng
            decimal input;
            if (decimal.TryParse(textBox8.Text, out input))
            {
                sum += input;
                textBox8.Text = sum.ToString("N0") + " đ";
            }
            else
            {
                MessageBox.Show("Giá trị nhập không hợp lệ.");
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            // Trừ giá trị
            decimal input;
            if (decimal.TryParse(textBox8.Text, out input))
            {
                if (sum >= input)
                {
                    sum -= input;
                    textBox9.Text = sum.ToString("N0") + " đ";
                    textBox8.Clear();
                }
                else
                {
                    MessageBox.Show("Không thể trừ số lớn hơn tổng hiện tại.");
                }
            }
            else
            {
                MessageBox.Show("Giá trị nhập không hợp lệ.");
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // Tính tổng khi thêm giá trị
            decimal input;
            if (decimal.TryParse(textBox8.Text, out input))
            {
                sum += input;
                textBox8.Clear();
            }
            else
            {
                MessageBox.Show("Giá trị nhập không hợp lệ.");
            }
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            textBox8.Text += "1";
        }

        private void Button18_Click(object sender, EventArgs e)
        {
            // Xóa tổng và đặt lại về 0
            textBox8.Clear();
            sum = 0;
            totalPrice = 0;
            textBox9.Text = "0 đ";
        }

        private void Button28_Click(object sender, EventArgs e)
        {
            AddToTotal(20000);
        }

        private void Button27_Click(object sender, EventArgs e)
        {
            AddToTotal(500000);
        }

        private void Button26_Click(object sender, EventArgs e)
        {
            AddToTotal(200000);
        }

        private void Button25_Click(object sender, EventArgs e)
        {
            AddToTotal(100000);
        }

        private void Button24_Click(object sender, EventArgs e)
        {
            AddToTotal(50000);
        }

        private void Button23_Click(object sender, EventArgs e)
        {
            AddToTotal(10000);
        }

        private void Button22_Click(object sender, EventArgs e)
        {
            AddToTotal(5000);
        }

        private void Button21_Click(object sender, EventArgs e)
        {
            AddToTotal(2000);
        }

        private void Button20_Click(object sender, EventArgs e)
        {
            AddToTotal(1000);
        }

        private void Button19_Click(object sender, EventArgs e)
        {
            AddToTotal(500);
        }

        private void TextBox8_TextChanged(object sender, EventArgs e)
        {
            // Hiển thị tổng còn lại sau khi trừ
            if (string.IsNullOrWhiteSpace(textBox8.Text))
            {
                textBox9.Text = "0 đ";
            }
            else
            {
                decimal input;
                if (decimal.TryParse(textBox8.Text, out input))
                {
                    textBox9.Text = (input - TongTien).ToString("N0") + " đ";
                }
                else
                {
                    textBox9.Text = "Sai định dạng.";
                }
            }
        }

        private void AddToTotal(decimal amount)
        {
            totalPrice += amount;
            textBox8.Text = totalPrice.ToString("N0");
        }

        private void Payment_FormClosed(object sender, FormClosedEventArgs e)
        {
            Cart cart = new Cart();
            cart.Show();
            this.Hide();
        }
    }
}
