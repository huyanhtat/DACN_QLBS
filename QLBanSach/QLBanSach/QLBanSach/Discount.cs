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

namespace QLBanSach
{
    public partial class Discount : Form
    {
        private DiscountBLL discountBLL;

        public Discount()
        {
            InitializeComponent();
            discountBLL = new DiscountBLL();
            LoadDiscounts();
            txtID.Enabled = false;
            //=============
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
            dgvDiscount.CellClick += DgvDiscount_CellClick;
        }

        private void DgvDiscount_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvDiscount.Rows[e.RowIndex];
                txtID.Text = row.Cells["id"].Value.ToString();
                txtDiscountCode.Text = row.Cells["discount_code"].Value.ToString();
                txtDescription.Text = row.Cells["description"].Value.ToString();
                // Kiểm tra null trước khi gán giá trị
                txtDiscountPercentage.Text = row.Cells["discount_percentage"].Value != null
                    ? row.Cells["discount_percentage"].Value.ToString()
                    : string.Empty;

                txtDiscountAmount.Text = row.Cells["discount_amount"].Value != null
                    ? row.Cells["discount_amount"].Value.ToString()
                    : string.Empty;
                dtpStartDate.Value = DateTime.Parse(row.Cells["start_date"].Value.ToString());
                dtpDateEnd.Value = DateTime.Parse(row.Cells["end_date"].Value.ToString());
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtID.Clear();
            txtDiscountCode.Clear();
            txtDescription.Clear();
            txtDiscountPercentage.Clear();
            txtDiscountAmount.Clear();
            dtpStartDate.Value = DateTime.Now;
            dtpDateEnd.Value = DateTime.Now;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtID.Text, out int id))
            {
                if (discountBLL.DeleteDiscount(id))
                {
                    MessageBox.Show("Xóa khuyến mãi thành công!");
                    LoadDiscounts();
                }
                else
                {
                    MessageBox.Show("Xóa khuyến mãi thất bại!");
                }
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtID.Text, out int id))
                {
                    MessageBox.Show("ID không hợp lệ!");
                    return;
                }

                decimal discountPercentage = 0;
                decimal discountAmount = 0;

                // Kiểm tra và chuyển đổi giá trị
                if (!string.IsNullOrWhiteSpace(txtDiscountPercentage.Text))
                {
                    if (!decimal.TryParse(txtDiscountPercentage.Text, out discountPercentage))
                    {
                        MessageBox.Show("Phần trăm giảm giá phải là số hợp lệ!");
                        return;
                    }
                }

                if (!string.IsNullOrWhiteSpace(txtDiscountAmount.Text))
                {
                    if (!decimal.TryParse(txtDiscountAmount.Text, out discountAmount))
                    {
                        MessageBox.Show("Số tiền giảm giá phải là số hợp lệ!");
                        return;
                    }
                }

                var discount = new discount
                {
                    id = id,
                    discount_code = txtDiscountCode.Text,
                    description = txtDescription.Text,
                    discount_percentage = discountPercentage,
                    discount_amount = discountAmount,
                    start_date = dtpStartDate.Value,
                    end_date = dtpDateEnd.Value,
                    status = 0 // Có thể thêm kiểm tra checkbox trạng thái nếu cần
                };

                if (discountBLL.UpdateDiscount(discount))
                {
                    MessageBox.Show("Cập nhật khuyến mãi thành công!");
                    LoadDiscounts();
                }
                else
                {
                    MessageBox.Show("Cập nhật khuyến mãi thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                decimal discountPercentage = 0;
                decimal discountAmount = 0;

                // Kiểm tra và chuyển đổi giá trị
                if (!string.IsNullOrWhiteSpace(txtDiscountPercentage.Text))
                {
                    if (!decimal.TryParse(txtDiscountPercentage.Text, out discountPercentage))
                    {
                        MessageBox.Show("Phần trăm giảm giá phải là số hợp lệ!");
                        return;
                    }
                }

                if (!string.IsNullOrWhiteSpace(txtDiscountAmount.Text))
                {
                    if (!decimal.TryParse(txtDiscountAmount.Text, out discountAmount))
                    {
                        MessageBox.Show("Số tiền giảm giá phải là số hợp lệ!");
                        return;
                    }
                }

                var discount = new discount
                {
                    discount_code = txtDiscountCode.Text,
                    description = txtDescription.Text,
                    discount_percentage = discountPercentage,
                    discount_amount = discountAmount,
                    start_date = dtpStartDate.Value,
                    end_date = dtpDateEnd.Value,
                    status = 1 
                };

                if (discountBLL.AddDiscount(discount))
                {
                    MessageBox.Show("Thêm khuyến mãi thành công!");
                    LoadDiscounts();
                }
                else
                {
                    MessageBox.Show("Thêm khuyến mãi thất bại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        //==========================
        private void LoadDiscounts()
        {
            dgvDiscount.DataSource = discountBLL.GetAllDiscounts().Select(d => new
            {
                d.id,
                d.discount_code,
                d.description,
                d.discount_percentage,
                d.discount_amount,
                d.start_date,
                d.end_date,
                d.status
            }).ToList();
        }

        private void Discount_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); // Thoát ứng dụng
        }
    }
}
