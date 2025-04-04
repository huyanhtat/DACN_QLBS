using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BLL_QLBS;
using DTO_QLBS;

namespace QLBanSach
{
    public partial class Employee : Form
    {
        private EmployeeBLL employeeBLL = new EmployeeBLL();

        public Employee()
        {
            InitializeComponent();
            LoadEmployees();
            LoadGenderComboBox();
            txtID.Enabled = false;

            // Gán các sự kiện cho các nút và DataGridView
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
            dgvEmployee.CellClick += DgvEmployees_CellClick;
            this.btnSearch.Click += BtnSearch_Click;
        }

        private string EncryptMD5(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string name = "";
            if(txtFullName.Text != "")
            {
                name = txtFullName.Text;
            }
            
            dgvEmployee.DataSource = employeeBLL.SearchEmployee(name);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            LoadEmployees();
        }

        // Sự kiện khi chọn một hàng trong DataGridView để hiển thị chi tiết nhân viên
        private void DgvEmployees_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvEmployee.Rows[e.RowIndex];
                txtID.Text = row.Cells["id"].Value.ToString();
                txtFullName.Text = row.Cells["full_name"].Value.ToString();
                txtPhone.Text = row.Cells["phone"].Value.ToString();
                txtAddress.Text = row.Cells["address"].Value.ToString();
                txtEmail.Text = row.Cells["email"].Value.ToString();
                cboGender.SelectedItem = row.Cells["gender"].Value.ToString();
                txtUserName.Text = row.Cells["user_name"].Value.ToString();
                txtPassword.Text = row.Cells["password"].Value.ToString();
                dtpDateOfBirth.Value = Convert.ToDateTime(row.Cells["date_of_birth"].Value);
            }
        }

        // Sự kiện xóa thông tin nhân viên
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployee.CurrentRow != null)
            {
                int employeeId = Convert.ToInt32(txtID.Text);

                if (employeeBLL.DeleteEmployee(employeeId))
                {
                    MessageBox.Show("Xóa nhân viên thành công!");
                    LoadEmployees();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi xóa nhân viên.");
                }
            }
        }

        // Sự kiện cập nhật thông tin nhân viên
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvEmployee.CurrentRow != null && ValidateEmployeeInput())
            {
                user updateEmployee = new user
                {
                    id = Convert.ToInt32(txtID.Text),
                    full_name = txtFullName.Text,
                    phone = txtPhone.Text,
                    address = txtAddress.Text,
                    email = txtEmail.Text,
                    gender = cboGender.SelectedItem?.ToString(),
                    date_of_birth = dtpDateOfBirth.Value,
                    code_role = "EMPLOYEE", // Đặt vai trò là "EMPLOYEE"

                    // Đảm bảo các trường bắt buộc không NULL
                    user_name = txtUserName.Text,
                    password = EncryptMD5(txtPassword.Text),
                    modified_date = DateTime.Now,
                    modified_by = "admin", // Hoặc tên người dùng hiện tại

                    // Thông tin tạo có thể được giữ nguyên hoặc lấy từ cơ sở dữ liệu
                    create_by = "admin",
                    status = 1,
                    create_date = DateTime.Now
                };

                if (employeeBLL.UpdateEmployee(updateEmployee))
                {
                    MessageBox.Show("Cập nhật nhân viên thành công!");
                    LoadEmployees();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi cập nhật nhân viên.");
                }
            }
        }

        // Sự kiện thêm mới nhân viên
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (ValidateEmployeeInput())
            {
                user newEmployee = new user
                {
                    full_name = txtFullName.Text,
                    phone = txtPhone.Text,
                    address = txtAddress.Text,
                    email = txtEmail.Text,
                    gender = cboGender.SelectedItem?.ToString(),
                    date_of_birth = dtpDateOfBirth.Value,
                    code_role = "EMPLOYEE",
                    user_name = txtUserName.Text,
                    password = EncryptMD5(txtPassword.Text),
                    image = "",
                    image64bit = "",
                    modified_date = DateTime.Now,
                    modified_by = "admin",
                    create_by = "admin",
                    status = 1,
                    create_date = DateTime.Now
                };

                if (employeeBLL.AddEmployee(newEmployee))
                {
                    MessageBox.Show("Thêm nhân viên thành công!");
                    LoadEmployees();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi thêm nhân viên.");
                }
            }
        }

        // ============================================================
        // Nạp danh sách nhân viên vào DataGridView
        private void LoadEmployees()
        {
            List<EmployeeDTO> employees = employeeBLL.GetAllEmployees();
            dgvEmployee.DataSource = employees;
        }

        // Kiểm tra các trường nhập liệu không để trống
        private bool ValidateEmployeeInput()
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

            if (cboGender.SelectedIndex == -1)
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
            cboGender.SelectedIndex = -1;
            dtpDateOfBirth.Value = DateTime.Now;
            txtUserName.Clear();
            txtPassword.Clear();
        }

        // Nạp dữ liệu vào ComboBox giới tính
        private void LoadGenderComboBox()
        {
            cboGender.DataSource = new List<string> { "Nam", "Nữ" };
        }

        private void Employee_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); // Thoát ứng dụng
        }
    }
}
