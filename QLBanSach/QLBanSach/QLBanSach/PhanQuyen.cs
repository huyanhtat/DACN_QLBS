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
    public partial class PhanQuyen : Form
    {
        PhanQuyenBLL p = new PhanQuyenBLL();
        public PhanQuyen()
        {
            InitializeComponent();
            this.Load += PhanQuyen_Load;
            this.btnPhanQuyen.Click += BtnPhanQuyen_Click;
            this.dataGridView1.CellContentClick += DataGridView1_CellContentClick;
            this.cbbRole.SelectedIndexChanged += CbbRole_SelectedIndexChanged;
            this.button4.Click += Button4_Click;
            this.cbbRole.Click += CbbRole_Click;
        }

        private void CbbRole_Click(object sender, EventArgs e)
        {
            cbbRole.DataSource = p.loadRole();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim();
            dataGridView1.DataSource = p.searchEmployee(name);
            loadRoleRow();
        }

        private void CbbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedRole = cbbRole.SelectedValue.ToString();

            dataGridView1.DataSource = p.searchRole(selectedRole);

            loadRoleRow();
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 2))
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i != e.ColumnIndex)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[i].Value = false;
                    }
                }
            }
        }

        private void BtnPhanQuyen_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int id = Convert.ToInt32(row.Cells["Id"].Value);

                bool isAdmin = row.Cells["Admin"].Value != null && row.Cells["Admin"].Value.ToString() == "1";
                bool isEmployee = row.Cells["Employee"].Value != null && row.Cells["Employee"].Value.ToString() == "1";
                bool isSaler = row.Cells["Saler"].Value != null && row.Cells["Saler"].Value.ToString() == "1";


                if (isAdmin)
                {
                    p.phanQuyen(id, "Admin");
                    MessageBox.Show("Đã cấp quyền Admin");
                }
                else if (isEmployee)
                {
                    p.phanQuyen(id, "Employee");
                    MessageBox.Show("Đã cấp quyền Nhân viên");
                }
                else if (isSaler)
                {
                    p.phanQuyen(id, "Saler");
                    MessageBox.Show("Đã cấp quyền Nhân viên bán hàng");
                }
            }
        }

        private void PhanQuyen_Load(object sender, EventArgs e)
        {
            load();

            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns["ChucVu"].Visible = false;
        }
        // ========================
        public void loadRoleRow()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string role = row.Cells["ChucVu"].Value?.ToString();

                // Đánh dấu các CheckBox tương ứng với quyền
                row.Cells["Admin"].Value = (role == "ADMIN");
                row.Cells["Employee"].Value = (role == "EMPLOYEE");
                row.Cells["Saler"].Value = (role == "SALER");
            }
        }
        public void load()
        {
            if (!dataGridView1.Columns.Contains("Admin"))
            {
                DataGridViewCheckBoxColumn adminColumn = new DataGridViewCheckBoxColumn();
                adminColumn.HeaderText = "Quản trị viên";
                adminColumn.Name = "Admin";
                adminColumn.FalseValue = "0";
                adminColumn.TrueValue = "1";
                dataGridView1.Columns.Add(adminColumn);
            }

            if (!dataGridView1.Columns.Contains("Employee"))
            {
                DataGridViewCheckBoxColumn employeeColumn = new DataGridViewCheckBoxColumn();
                employeeColumn.HeaderText = "Nhân viên";
                employeeColumn.Name = "Employee";
                employeeColumn.FalseValue = "0";
                employeeColumn.TrueValue = "1";
                dataGridView1.Columns.Add(employeeColumn);
            }

            if (!dataGridView1.Columns.Contains("Saler"))
            {
                DataGridViewCheckBoxColumn salerColumn = new DataGridViewCheckBoxColumn();
                salerColumn.HeaderText = "Nhân viên bán hàng";
                salerColumn.Name = "Saler";
                salerColumn.FalseValue = "0";
                salerColumn.TrueValue = "1";
                dataGridView1.Columns.Add(salerColumn);
            }
            dataGridView1.DataSource = p.loadDataEmployee();

            loadRoleRow();
        }

        private void PhanQuyen_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); // Thoát ứng dụng
        }
    }
}
