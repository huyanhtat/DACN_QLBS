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
    public partial class Category : Form
    {
        private CategoryBLL categoryBLL = new CategoryBLL();
        public Category()
        {
            InitializeComponent();
            LoadCategories();
            txtID.Enabled = false;
            // Xử lí sự kiện
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
            dgvCategory.CellClick += DgvCategory_CellClick;
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void DgvCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCategory.Rows[e.RowIndex];
                txtID.Text = row.Cells["id"].Value.ToString();
                txtNameCate.Text = row.Cells["name"].Value.ToString();
                txtCode.Text = row.Cells["code"].Value.ToString();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            book_category category = new book_category()
            {
                name = txtNameCate.Text,
                code = txtCode.Text
            };

            if (categoryBLL.AddCategory(category))
            {
                MessageBox.Show("Thêm danh mục thành công !");
                LoadCategories();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi thêm danh mục");
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu có hàng nào đang được chọn và các trường nhập liệu hợp lệ
            if (dgvCategory.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một danh mục để cập nhật.");
                return;
            }

            if (!ValidateCategoryInput())
            {
                return; // Dừng lại nếu dữ liệu không hợp lệ
            }

            // Kiểm tra và chuyển đổi ID
            if (!int.TryParse(txtID.Text, out int categoryId))
            {
                MessageBox.Show("ID không hợp lệ.");
                return;
            }

            // Tạo đối tượng category để cập nhật
            book_category updateCategory = new book_category
            {
                id = categoryId,
                name = txtNameCate.Text.Trim(),
                code = txtCode.Text.Trim()
            };

            try
            {
                // Gọi phương thức cập nhật từ BLL
                bool isUpdated = categoryBLL.UpdateCategory(updateCategory);

                if (isUpdated)
                {
                    MessageBox.Show("Cập nhật danh mục thành công!");
                    LoadCategories(); // Tải lại danh sách danh mục sau khi cập nhật
                    ClearFields(); // Xóa các trường nhập liệu sau khi cập nhật
                }
                else
                {
                    MessageBox.Show("Không tìm thấy danh mục để cập nhật.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi cập nhật danh mục: " + ex.Message);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCategory.CurrentRow != null)
            {
                int categoryId = Convert.ToInt32(txtID.Text);

                if (categoryBLL.DeleteCategory(categoryId))
                {
                    MessageBox.Show("Xoá danh mục thành công !");
                    LoadCategories();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi xoá danh mục");
                }
            }
        }

        // =================================================
        private void LoadCategories()
        {
            // Lấy danh sách danh mục từ cơ sở dữ liệu
            List<book_category> categories = categoryBLL.GetAllCategories();
            // Gán lại danh sách mới cho DataGridView
            dgvCategory.DataSource = null;
            dgvCategory.DataSource = categories;
        }
        private void ClearFields()
        {
            txtID.Clear();
            txtNameCate.Clear();
            txtCode.Clear();
            dgvCategory.ClearSelection(); // Bỏ chọn hàng trong DataGridView
        }
        private bool ValidateCategoryInput()
        {
            if (string.IsNullOrWhiteSpace(txtNameCate.Text))
            {
                MessageBox.Show("Vui lòng nhập tên danh mục.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                MessageBox.Show("Vui lòng nhập mã danh mục.");
                return false;
            }

            return true;
        }

        private void Category_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); // Thoát ứng dụng
        }
    }
}
