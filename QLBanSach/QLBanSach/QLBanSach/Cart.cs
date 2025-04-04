using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using BLL_QLBS;
using DTO_QLBS;
using ZXing;
using System.Media;

namespace QLBanSach
{
    public partial class Cart : Form
    {
        ThanhToanBLL b = new ThanhToanBLL();
        private Timer timer;
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice videoCaptureDevice;

        public Cart()
        {
            InitializeComponent();
            label9.Text = Properties.Settings.Default.username;
            this.button3.Click += Button3_Click;
            this.button4.Click += Button4_Click;
            //khoi tao thiet bi
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo info in filterInfoCollection)
            {
                comboBox1.Items.Add(info.Name);
            }
            comboBox1.SelectedIndex = 0;
            this.button1.Click += Button1_Click;
            InitializeDataGridView();
            InitializeTimer();
            this.button5.Click += Button5_Click;
            this.btnReset.Click += BtnReset_Click;
            this.btnGiamGia.Click += BtnGiamGia_Click;
            this.textBox8.Click += TextBox8_Click;
            btnGiamGia.Enabled = false;
        }

        private void TextBox8_Click(object sender, EventArgs e)
        {
            btnGiamGia.Enabled = true;
        }

        private void BtnGiamGia_Click(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            try
            {
                // Danh sách chi tiết hóa đơn
                List<bill_detail> billDetails = new List<bill_detail>();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["barcode"].Value != null && row.Cells["quantity"].Value != null)
                    {
                        var bookDetails = b.GetBookByBarcode(row.Cells["barcode"].Value.ToString());

                        if (bookDetails != null)
                        {
                            var billDetail = new bill_detail
                            {
                                id_book = bookDetails.id,
                                quantity = Convert.ToInt32(row.Cells["quantity"].Value),
                                price = bookDetails.price,
                                vat = 0
                            };

                            billDetails.Add(billDetail); // Thêm vào danh sách
                        }

                    }
                }

                // Gọi phương thức saveBill để lưu hóa đơn và chi tiết hóa đơn
                ThanhToanBLL thanhToanBLL = new ThanhToanBLL();
                thanhToanBLL.saveBill(billDetails);

                Payment p = new Payment();
                p.TongTien = Convert.ToDecimal(textBox7.Text);
                p.Tag = thanhToanBLL.GetLastBill(); // Truyền hóa đơn vừa tạo
                p.Show();
                this.Hide();
            }
            catch
            {
                MessageBox.Show("Sản phẩm không tồn tại trong máy");
            }
        }


        private string InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 1000; // Cập nhật mỗi 1000ms = 1 giây
            timer.Tick += Timer_Tick; // Gán sự kiện Tick
            timer.Start(); // Bắt đầu Timer
            return timer.ToString();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Cập nhật thời gian hiện tại
            label1.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); // Hiển thị giờ phút giây
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string barcode = textBox2.Text.Trim();
            var bookDetails = b.GetBookByBarcode(barcode);

            if (bookDetails != null)
            {
                bool exists = false;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["barcode"].Value != null && row.Cells["barcode"].Value.ToString() == barcode)
                    {
                        int currentQuantity = Convert.ToInt32(row.Cells["quantity"].Value);
                        row.Cells["quantity"].Value = currentQuantity + 1;
                        if(Convert.ToInt32(row.Cells["quantity"].Value) > 9)
                        {
                            MessageBox.Show("Quá giới hạn 10 cuốn sách");
                            textBox2.Clear();
                        }
                        row.Cells["total"].Value = Convert.ToInt32(row.Cells["quantity"].Value) * Convert.ToInt32(row.Cells["price"].Value);
                        exists = true;

                        break;
                    }

                }

                // If product does not exist, add as new row
                if (!exists)
                {
                    dataGridView1.Rows.Add(
                    bookDetails.name,
                    bookDetails.barcode,
                    bookDetails.price,
                    1,
                    bookDetails.price
                ); ;
                }
                CalculateTotals();
            }
            else
            {
                MessageBox.Show("Sản Phẩm Không Tìm Thấy.");
            }
        }

        private void PlayBeepSound()
        {
            try
            {
                // Lấy tệp âm thanh từ tài nguyên nhúng
                using (System.IO.UnmanagedMemoryStream stream = Properties.Resources.beep_07a)
                {
                    using (SoundPlayer player = new SoundPlayer(stream))
                    {
                        player.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot play sound: " + ex.Message);
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (videoCaptureDevice.IsRunning)
            {
                videoCaptureDevice.Stop();
                videoCaptureDevice = null;
                pictureBox1.Image = null;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (videoCaptureDevice != null && videoCaptureDevice.IsRunning)
            {
                videoCaptureDevice.Stop();
            }
            videoCaptureDevice = new VideoCaptureDevice(filterInfoCollection[comboBox1.SelectedIndex].MonikerString);
            videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;
            videoCaptureDevice.Start();
        }



        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            BarcodeReader reader = new BarcodeReader();
            var result = reader.Decode(bitmap);
            if (result != null)
            {
                textBox2.Invoke(new MethodInvoker(delegate ()
                {
                    textBox2.Text = result.ToString();
                    PlayBeepSound();
                    
                    Button1_Click(null, null);
                      
                }));
            }
            pictureBox1.Image = bitmap;
        }

        private void InitializeDataGridView()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("name", "Tên Sách");
            dataGridView1.Columns.Add("barcode", "Barcode");
            dataGridView1.Columns.Add("price", "Giá");
            dataGridView1.Columns.Add("quantity", "Số Lượng");
            dataGridView1.Columns.Add("total", "Tổng Tiền");
        }

        private void CalculateTotals()
        {
            decimal totalAmount = 0;
            int totalQuantity = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["quantity"].Value != null && row.Cells["total"].Value != null)
                {
                    totalQuantity += Convert.ToInt32(row.Cells["quantity"].Value);
                    totalAmount += Convert.ToDecimal(row.Cells["total"].Value);
                }
            }
            decimal? discount = b.getDiscount(textBox8.Text);
            textBox4.Text = totalQuantity.ToString();
            textBox5.Text = totalAmount.ToString();
            textBox6.Text = "0";
            textBox7.Text = discount == null ? totalAmount.ToString() : (totalAmount - discount).ToString();

        }


        private void textBox8_Click(object sender, EventArgs e)
        {

        }

        private void textBox8_MouseClick(object sender, MouseEventArgs e)
        {
            textBox8.Enabled = true;
        }

        private void Cart_FormClosed(object sender, FormClosedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Hide();
        }
    }
}