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
    public partial class Publisher : Form
    {
        private PublisherBLL publisherBLL;
        public Publisher()
        {
            InitializeComponent();
            publisherBLL = new PublisherBLL();
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
            dgvPublisher.CellClick += DgvPublisher_CellClick;
            txtID.Enabled = false;
            //====================
            LoadPublishers();
        }

        private void DgvPublisher_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvPublisher.Rows[e.RowIndex];
                txtID.Text = row.Cells["id"].Value.ToString();
                txtName.Text = row.Cells["name"].Value.ToString();
                txtAddress.Text = row.Cells["address"].Value.ToString();
                txtPhone.Text = row.Cells["phone"].Value.ToString();
                txtEmail.Text = row.Cells["email"].Value.ToString();
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearInputFields();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtID.Text) && int.TryParse(txtID.Text, out int id))
            {
                if (publisherBLL.DeletePublisher(id))
                {
                    MessageBox.Show("Xoá thành công nhà cung cấp !");
                    LoadPublishers();
                    ClearInputFields();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi xoá !");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng hãy chọn nhà cung cấp để xoá !");
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtID.Text) && int.TryParse(txtID.Text, out int id))
            {
                publisher publisher = new publisher
                {
                    id = id,
                    name = txtName.Text,
                    address = txtAddress.Text,
                    phone = txtPhone.Text,
                    email = txtEmail.Text
                };

                if (publisherBLL.UpdatePublisher(publisher))
                {
                    MessageBox.Show("Thêm nhà cung cấp thành công !");
                    LoadPublishers();
                    ClearInputFields();
                }
                else
                {
                    MessageBox.Show("Lỗi xảy ra khi cập nhật !");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp !");
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            publisher publisher = new publisher
            {
                name = txtName.Text,
                address = txtAddress.Text,
                phone = txtPhone.Text,
                email = txtEmail.Text
            };

            if (publisherBLL.AddPublisher(publisher))
            {
                MessageBox.Show("Thêm nhà cung cấp thành công !");
                LoadPublishers();
                ClearInputFields();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi thêm nhà cung cấp");
            }
        }

        // =====================
        private void LoadPublishers()
        {
            List<publisher> publishers = publisherBLL.GetAllPublishers();
            dgvPublisher.DataSource = publishers;
        }
        private void ClearInputFields()
        {
            txtID.Text = string.Empty;
            txtName.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
        }

        private void Publisher_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); // Thoát ứng dụng
        }
    }
}
