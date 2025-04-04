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
using System.IO;

namespace QLBanSach
{
    public partial class Author : Form
    {
        private AuthorBLL authorBLL;
        public Author()
        {
            InitializeComponent();
            authorBLL = new AuthorBLL();
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
            dgvAuthor.CellClick += DgvAuthor_CellClick;
            btnChoose.Click += BtnChoose_Click;
            btnSearch.Click += BtnSearch_Click;
            txtID.Enabled = false;
            //==============
            LoadAuthors();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show("Vui lòng nhập từ khoá để tìm kiếm!");
                return;
            }

            var authors = authorBLL.GetAllAuthors();
            var filteredAuthors = authors.Where(a =>
                (a.name != null && a.name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (a.address != null && a.address.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (a.bio != null && a.bio.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (a.phone != null && a.phone.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) ||
                (a.email != null && a.email.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
            ).Select(a => new
            {
                a.id,
                a.name,
                a.address,
                a.bio,
                a.phone,
                a.email
            }).ToList();

            dgvAuthor.DataSource = filteredAuthors;

            if (filteredAuthors.Count == 0)
            {
                MessageBox.Show("Không tìm thấy tác giả nào phù hợp với từ khoá!");
            }
        }

        private void BtnChoose_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Load hình ảnh gốc
                        using (Image originalImage = Image.FromFile(ofd.FileName))
                        {
                            // Thay đổi kích thước về 220x280
                            Bitmap resizedImage = new Bitmap(270, 300);
                            using (Graphics g = Graphics.FromImage(resizedImage))
                            {
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                g.DrawImage(originalImage, 0, 0, 270, 300);
                            }

                            // Gán ảnh đã thay đổi kích thước vào PictureBox
                            picAuthor.Image?.Dispose(); // Giải phóng hình ảnh cũ nếu có
                            picAuthor.Image = resizedImage;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xử lý hình ảnh: " + ex.Message);
                    }
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearInputFields();
            LoadAuthors();
        }

        private void DgvAuthor_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvAuthor.Rows[e.RowIndex];
                txtID.Text = row.Cells["id"].Value.ToString();
                txtName.Text = row.Cells["name"].Value.ToString();
                txtAddress.Text = row.Cells["address"].Value?.ToString() ?? string.Empty;
                txtBio.Text = row.Cells["bio"].Value.ToString();
                txtPhone.Text = row.Cells["phone"].Value?.ToString() ?? string.Empty;
                txtEmail.Text = row.Cells["email"].Value?.ToString() ?? string.Empty;

                // Lấy hình ảnh từ cơ sở dữ liệu dựa trên ID
                int id = int.Parse(txtID.Text);
                var author = authorBLL.GetAllAuthors().FirstOrDefault(a => a.id == id);
                if (author != null && author.image != null)
                {
                    picAuthor.SizeMode = PictureBoxSizeMode.CenterImage; // Đặt chế độ căn giữa
                    var originalImage = ByteArrayToImage(author.image.ToArray());
                    picAuthor.Image = ResizeImage(originalImage, 270, 300); // Resize cố định 100x100
                }
                else
                {
                    picAuthor.Image = null; // Nếu không có ảnh
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtID.Text) && int.TryParse(txtID.Text, out int id))
            {
                if (authorBLL.DeleteAuthor(id))
                {
                    MessageBox.Show("Xoá tác giả thành công !");
                    LoadAuthors();
                    ClearInputFields();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi xoá !");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tác giả !");
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtID.Text) && int.TryParse(txtID.Text, out int id))
            {
                author author = new author
                {
                    id = id,
                    name = txtName.Text,
                    address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text,
                    bio = txtBio.Text,
                    phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text,
                    email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text,
                    image = picAuthor.Image != null ? ImageToByteArray(picAuthor.Image) : null
                };

                if (authorBLL.UpdateAuthor(author))
                {
                    MessageBox.Show("Cập nhật tác giả thành công!");
                    LoadAuthors();
                    ClearInputFields();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi cập nhật!");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tác giả hợp lệ để cập nhật.");
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtBio.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin yêu cầu (Tên và Tiểu sử).");
                return;
            }

            Image image = picAuthor.Image;
            byte[] imageBytes = null;

            if (image != null)
            {
                try
                {
                    imageBytes = ImageToByteArray(image);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi chuyển đổi hình ảnh: {ex.Message}");
                    return;
                }
            }

            author author = new author
            {
                name = txtName.Text,
                address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text,
                bio = txtBio.Text,
                phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text,
                email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text,
                image = imageBytes
            };

            if (authorBLL.AddAuthor(author))
            {
                MessageBox.Show("Thêm thành công tác giả mới!");
                LoadAuthors();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi thêm tác giả.");
            }
        }

        //============================
        private void LoadAuthors()
        {
            List<author> authors = authorBLL.GetAllAuthors();
            dgvAuthor.DataSource = authors.Select(a => new
            {
                a.id,
                a.name,
                a.address,
                a.bio,
                a.phone,
                a.email
            }).ToList();
        }


        private void ClearInputFields()
        {
            txtID.Text = string.Empty;
            txtName.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtBio.Text = string.Empty;
            txtPhone.Text = string.Empty;
            txtEmail.Text = string.Empty;
        }
        private byte[] ImageToByteArray(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "Image cannot be null.");
            }

            using (var ms = new MemoryStream())
            {
                try
                {
                    image.Save(ms, image.RawFormat);
                }
                catch (ArgumentNullException)
                {
                    // Nếu RawFormat không hợp lệ, sử dụng định dạng mặc định
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                }
                return ms.ToArray();
            }
        }

        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (var ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        // Hàm thay đổi kích thước hình ảnh
        private Image ResizeImage(Image image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, width, height);
            }
            return resizedImage;
        }

        private void Author_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); // Thoát ứng dụng
        }
    }
}
