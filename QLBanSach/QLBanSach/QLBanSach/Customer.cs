using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL_QLBS;
using DTO_QLBS;

namespace QLBanSach
{
    public partial class Customer : Form
    {
        private CustomerBLL customerBLL = new CustomerBLL();
        public Customer()
        {
            InitializeComponent();
            LoadCustomers();
            LoadGenderComboBox();
            txtID.Enabled = false;
            // Gán các sự kiện cho các nút và DataGridView
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
            dgvCustomer.CellClick += DgvCustomers_CellClick;
        }

        private void DgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCustomer.Rows[e.RowIndex];
                txtID.Text = row.Cells["id"].Value.ToString();
                txtFullName.Text = row.Cells["full_name"].Value.ToString();
                txtPhone.Text = row.Cells["phone"].Value.ToString();
                txtAddress.Text = row.Cells["address"].Value.ToString();
                txtEmail.Text = row.Cells["email"].Value.ToString();
                cmbGender.SelectedItem = row.Cells["gender"].Value.ToString();
                dtpDateOfBirth.Value = Convert.ToDateTime(row.Cells["date_of_birth"].Value);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.CurrentRow != null)
            {
                int customerId = Convert.ToInt32(txtID.Text);

                if (customerBLL.DeleteCustomer(customerId))
                {
                    MessageBox.Show("Xóa khách hàng thành công!");
                    LoadCustomers();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi xóa khách hàng.");
                }
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.CurrentRow != null && ValidateCustomerInput())
            {
                user updateUser = new user
                {
                    id = Convert.ToInt32(txtID.Text),
                    full_name = txtFullName.Text,
                    phone = txtPhone.Text,
                    address = txtAddress.Text,
                    email = txtEmail.Text,
                    gender = cmbGender.SelectedItem?.ToString(),
                    date_of_birth = dtpDateOfBirth.Value,
                    code_role = "USER", // Hoặc lấy từ giao diện nếu có

                    // Đảm bảo các trường bắt buộc không NULL
                    user_name = txtFullName.Text.Trim().Replace(" ", "_").ToLower(),
                    password = "default_password", // Cung cấp mật khẩu mặc định hoặc từ trường nhập
                    modified_date = DateTime.Now,
                    modified_by = "System", // Hoặc tên người dùng hiện tại

                    // Dữ liệu khác có thể giữ nguyên hoặc lấy từ cơ sở dữ liệu
                    create_by = "System", // Giá trị mặc định
                    create_date = DateTime.Now // Giá trị mặc định
                };

                if (customerBLL.UpdateCustomer(updateUser))
                {
                    MessageBox.Show("Cập nhật khách hàng thành công!");
                    LoadCustomers();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi cập nhật khách hàng.");
                }
            }
        }

        // Thêm khách hàng mua trực tiếp
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (ValidateCustomerInput())
            {
                user newUser = new user
                {
                    full_name = txtFullName.Text,
                    phone = txtPhone.Text,
                    address = txtAddress.Text,
                    email = txtEmail.Text,
                    gender = cmbGender.SelectedItem?.ToString(),
                    date_of_birth = dtpDateOfBirth.Value,
                    code_role = "USER", // Mặc định vai trò là "user"
                    user_name = "",
                    password = "", // Giá trị mặc định cho password
                    image = "default_image_path",
                    image64bit = "default_image_data",
                    modified_date = DateTime.Now,
                    modified_by = "System",
                    create_by = "System", // Giá trị mặc định cho create_by, bạn có thể thay thế bằng tên người tạo nếu có
                    create_date = DateTime.Now // Ngày tạo hiện tại
                };

                if (customerBLL.AddCustomer(newUser))
                {
                    MessageBox.Show("Thêm khách hàng thành công!");
                    LoadCustomers();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi thêm khách hàng.");
                }
            }
        }
    
        


        // ============================================================
        // Nạp danh sách khách hàng vào DataGridView
        private void LoadCustomers()
        {
            List<CustomerDTO> customers = customerBLL.GetAllCustomers();
            
            dgvCustomer.DataSource = customers;
        }

        // Kiểm tra các trường nhập liệu không để trống
        private bool ValidateCustomerInput()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text) || txtPhone.Text.Length != 10 || !txtPhone.Text.All(char.IsDigit))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại hợp lệ (10 chữ số).");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ email hợp lệ.");
                return false;
            }

            if (cmbGender.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn giới tính.");
                return false;
            }

            return true;
        }

        // Phương thức để xóa thông tin nhập liệu
        private void ClearFields()
        {
            txtID.Clear();
            txtFullName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtEmail.Clear();
            cmbGender.SelectedIndex = -1;
            dtpDateOfBirth.Value = DateTime.Now;
        }
        // Phương thức nạp dữ liệu vào ComboBox giới tính
        private void LoadGenderComboBox()
        {
            // Đặt hai giá trị "Nam" và "Nữ" trực tiếp vào ComboBox
            cmbGender.DataSource = new List<string> { "Nam", "Nữ" };
        }

        private void Customer_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); // Thoát ứng dụng
        }

        private void Customer_Load(object sender, EventArgs e)
        {
            
        }
    }
}
