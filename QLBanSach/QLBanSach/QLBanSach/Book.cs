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

namespace QLBanSach
{
    public partial class Book : Form
    {
        private BookBLL bookBLL;
        private CategoryBLL categoryBLL;
        private PublisherBLL publisherBLL;
        private AuthorBLL authorBLL;
        public Book()
        {
            InitializeComponent();
            //==============================
            bookBLL = new BookBLL();
            categoryBLL = new CategoryBLL();
            publisherBLL = new PublisherBLL();
            authorBLL = new AuthorBLL();
            dgvBook.CellClick += DgvBook_CellClick;
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnChoose.Click += BtnChoose_Click;
            btnClear.Click += BtnClear_Click;
            btnSearch.Click += BtnSearch_Click;
            txtID.Enabled = false;
            //================================
            LoadCategories();
            LoadPublishers();
            LoadBooks();
            LoadAuthors();
            LoadPriorities();
        }


        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string searchQuery = txtName.Text.Trim();
            LoadBooks_search(searchQuery);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtID.Text))
                {
                    MessageBox.Show("Vui lòng chọn sách cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int bookId = int.Parse(txtID.Text);

                var confirmResult = MessageBox.Show(
                    "Việc xóa sách sẽ vô hiệu hóa sách này. Bạn có chắc chắn muốn tiếp tục?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    bool isDeleted = bookBLL.DeleteBook(bookId); // Gọi hàm cập nhật status
                    if (isDeleted)
                    {
                        MessageBox.Show("Sách đã được vô hiệu hóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadBooks();
                        BtnClear_Click(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Không thể vô hiệu hóa sách. Kiểm tra log để biết thêm thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong DeleteBook: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Lỗi chi tiết: {ex.InnerException.Message}");
                }
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đầu vào
                if (!ValidateInput())
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin cần sửa!");
                    return;
                }

                int bookId = int.Parse(txtID.Text);

                // Kiểm tra tồn tại của sách dựa trên ID
                var existingBook = bookBLL.GetAllBooks().FirstOrDefault(b => b.id == bookId);
                if (existingBook == null)
                {
                    MessageBox.Show("Sách không tồn tại. Không thể cập nhật!");
                    return;
                }

                // Kiểm tra hình ảnh: cả ảnh mới và cũ đều không tồn tại
                if (picBook.Image == null && !HasExistingImage(bookId))
                {
                    MessageBox.Show("Không có hình ảnh nào được chọn. Vui lòng chọn hoặc giữ lại hình ảnh cũ!");
                    return;
                }

                // Lấy hình ảnh (mới hoặc giữ nguyên)
                byte[] image = picBook.Image != null ? ImageToByteArray(picBook.Image) : existingBook.image;
                
                // Tạo đối tượng sách để cập nhật
                var bookToUpdate = CreateBookObjectForUpdate(bookId, image);

                Console.WriteLine($"Calling UpdateBookAndAuthor with authorId: {cboAuthor.SelectedValue}");

                // Thực hiện cập nhật sách và tác giả
                if (UpdateBookAndAuthor(bookToUpdate))
                {
                    MessageBox.Show("Cập nhật sách và tác giả thành công!");
                    LoadBooks();
                }
                else
                {
                    MessageBox.Show("Cập nhật sách thành công nhưng tác giả thất bại. Vui lòng thử lại!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            // Xóa sạch các trường nhập liệu
            txtID.Clear();
            txtName.Clear();
            txtBarcode.Clear();
            txtPrice.Clear();
            txtQuantity.Clear();
            txtDescription.Clear();

            // Đặt lại ComboBox về giá trị mặc định
            if (cboCategory.Items.Count > 0)
                cboCategory.SelectedIndex = 0;
            if (cboPublisher.Items.Count > 0)
                cboPublisher.SelectedIndex = 0;
            if (cboAuthor.Items.Count > 0)
                cboAuthor.SelectedIndex = 0;

            // Xóa hình ảnh được chọn
            picBook.Image = null;

            // Đặt lại tiêu điểm vào trường nhập đầu tiên
            txtName.Focus();
        }

        // Chọn hình ảnh để thêm vào
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
                            Bitmap resizedImage = new Bitmap(220, 280);
                            using (Graphics g = Graphics.FromImage(resizedImage))
                            {
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                g.DrawImage(originalImage, 0, 0, 220, 280);
                            }

                            // Gán ảnh đã thay đổi kích thước vào PictureBox
                            picBook.Image?.Dispose(); // Giải phóng hình ảnh cũ nếu có
                            picBook.Image = resizedImage;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xử lý hình ảnh: " + ex.Message);
                    }
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
            string.IsNullOrWhiteSpace(txtBarcode.Text) ||
            string.IsNullOrWhiteSpace(txtPrice.Text) ||
            string.IsNullOrWhiteSpace(txtQuantity.Text) ||
            cboAuthor.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            try
            {
                string barcode = txtBarcode.Text;
                var existingBook = bookBLL.GetAllBooks().FirstOrDefault(b => b.barcode == barcode);

                if (existingBook != null)
                {
                    // Nếu sản phẩm đã tồn tại, hiển thị thông báo xác nhận
                    var result = MessageBox.Show(
                        "Sản phẩm đã tồn tại. Bạn có muốn cập nhật số lượng không?",
                        "Xác nhận cập nhật",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        existingBook.quantity += int.Parse(txtQuantity.Text);
                        bool isUpdated = bookBLL.UpdateBook(new book
                        {
                            id = existingBook.id,
                            name = existingBook.name,
                            barcode = existingBook.barcode,
                            price = existingBook.price,
                            quantity = existingBook.quantity,
                            description = existingBook.description,
                            id_publisher = existingBook.id_publisher,
                            code_category = existingBook.code_category,
                            image = existingBook.image,
                            create_by = Properties.Settings.Default.username,
                            create_date = DateTime.Now
                        });
                        if (isUpdated)
                        {
                            MessageBox.Show("Số lượng đã được cập nhật.");
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật số lượng thất bại.");
                        }
                    }
                }
                else
                {
                    // Nếu sách chưa tồn tại, thêm sách mới
                    var newBook = new book
                    {
                        name = txtName.Text,
                        barcode = txtBarcode.Text,
                        price = decimal.Parse(txtPrice.Text),
                        quantity = int.Parse(txtQuantity.Text),
                        description = txtDescription.Text,
                        status = 1,
                        id_publisher = int.Parse(cboPublisher.SelectedValue.ToString()),
                        code_category = cboCategory.SelectedValue.ToString(),
                        image = picBook.Image != null ? ImageToByteArray(picBook.Image) : null,
                        create_by = Properties.Settings.Default.username,
                        number_of_purchases = 0,
                        number_of_views = 0,
                        priority = Convert.ToByte(cboPrio.SelectedValue),
                        create_date = DateTime.Now
                    };

                    if (bookBLL.AddBook(newBook))
                    {
                        // Lấy ID sách vừa thêm
                        var addedBook = bookBLL.GetAllBooks().FirstOrDefault(b => b.barcode == barcode);
                        if (addedBook != null)
                        {
                            // Thêm vào bảng book_join_author
                            int authorId = int.Parse(cboAuthor.SelectedValue.ToString());
                            var bookJoinAuthorBLL = new BookJoinAuthorBLL();
                            bool isAdded = bookJoinAuthorBLL.AddBookJoinAuthor(addedBook.id, authorId, "Primary");

                            if (isAdded)
                            {
                                MessageBox.Show("Thêm sách và tác giả thành công.");
                            }
                            else
                            {
                                MessageBox.Show("Thêm sách thành công nhưng không thể liên kết với tác giả.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Thêm sách thất bại.");
                    }
                }
                LoadBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private void DgvBook_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Lấy thông tin từ dòng hiện tại
                DataGridViewRow row = dgvBook.Rows[e.RowIndex];

                // Kiểm tra null trước khi lấy dữ liệu từ từng ô
                int bookId = row.Cells["id"].Value != null ? Convert.ToInt32(row.Cells["id"].Value) : 0;
                int publisherId = row.Cells["id_publisher"].Value != null ? Convert.ToInt32(row.Cells["id_publisher"].Value) : 0;
                string categoryCode = row.Cells["code_category"].Value != null ? row.Cells["code_category"].Value.ToString() : string.Empty;

                var book = bookBLL.GetAllBooks().FirstOrDefault(b => b.id == bookId);
                // Hiển thị Priority trong ComboBox
                if (book != null && book.priority.HasValue)
                {
                    cboPrio.SelectedValue = Convert.ToInt32(book.priority.Value); // Chuyển đổi sang int
                }
                else
                {
                    cboPrio.SelectedIndex = -1; // Nếu không có giá trị Priority, xóa lựa chọn
                }

                // Hiển thị tên tác giả nếu bookId khác 0
                if (bookId != 0)
                {
                    string authorName = bookBLL.GetAuthorName(bookId);
                    cboAuthor.Text = !string.IsNullOrEmpty(authorName) ? authorName : "Không xác định";
                }
                else
                {
                    cboAuthor.Text = string.Empty;
                }

                // Hiển thị tên nhà xuất bản nếu publisherId khác 0
                if (publisherId != 0)
                {
                    string publisherName = bookBLL.GetPublisherName(publisherId);
                    cboPublisher.Text = !string.IsNullOrEmpty(publisherName) ? publisherName : "Không xác định";
                }
                else
                {
                    cboPublisher.Text = string.Empty;
                }

                // Hiển thị tên danh mục nếu categoryCode không rỗng
                if (!string.IsNullOrEmpty(categoryCode))
                {
                    string categoryName = bookBLL.GetCategoryName(categoryCode);
                    cboCategory.Text = categoryName;
                }
                else
                {
                    cboCategory.Text = string.Empty;
                }

                // Hiển thị hình ảnh
                if (bookId != 0)
                {
                    if (book != null && book.image != null)
                    {
                        // Nếu book.image là byte[], chuyển đổi thành Image
                        picBook.SizeMode = PictureBoxSizeMode.CenterImage; // Đặt chế độ căn giữa
                        var originalImage = ByteArrayToImage(book.image.ToArray());
                        picBook.Image = ResizeImage(originalImage, 220, 280); // Resize cố định 100x100
                    }
                    else
                    {
                        // Nếu không có ảnh hoặc book.image null
                        picBook.Image = null;
                    }
                }
                else
                {
                    picBook.Image = null;
                }

                if (bookId != 0)
                {
                    // Lấy ID tác giả từ bảng book_join_author
                    var bookJoinAuthorBLL = new BookJoinAuthorBLL();
                    int? authorId = bookJoinAuthorBLL.GetAuthorIdByBookId(bookId);

                    if (authorId.HasValue)
                    {
                        // Hiển thị tên tác giả trong cboAuthor
                        var authors = authorBLL.GetAllAuthors();
                        var selectedAuthor = authors.FirstOrDefault(a => a.id == authorId.Value);
                        if (selectedAuthor != null)
                        {
                            cboAuthor.Text = selectedAuthor.name;
                        }
                        else
                        {
                            cboAuthor.Text = "Không xác định";
                        }
                    }
                    else
                    {
                        cboAuthor.Text = "Không xác định";
                    }
                }
                else
                {
                    cboAuthor.Text = string.Empty;
                }

                // Hiển thị các thông tin khác lên các điều khiển khác nếu cần
                txtID.Text = bookId.ToString();
                txtName.Text = row.Cells["name"].Value?.ToString() ?? string.Empty;
                txtBarcode.Text = row.Cells["barcode"].Value?.ToString() ?? string.Empty;
                txtPrice.Text = row.Cells["price"].Value != null ? row.Cells["price"].Value.ToString() : "0";
                txtQuantity.Text = row.Cells["quantity"].Value != null ? row.Cells["quantity"].Value.ToString() : "0";
                txtDescription.Text = row.Cells["description"].Value?.ToString() ?? string.Empty;
            }
        }

        //=================================
        // Tải danh sách danh mục vào cboCategory
        private void LoadCategories()
        {
            var categories = categoryBLL.GetAllCategories();
            cboCategory.DataSource = categories;
            cboCategory.DisplayMember = "name";
            cboCategory.ValueMember = "code";
        }

        // Tải danh sách nhà cung cấp vào cboPublisher
        private void LoadPublishers()
        {
            var publishers = publisherBLL.GetAllPublishers();
            cboPublisher.DataSource = publishers;
            cboPublisher.DisplayMember = "name";
            cboPublisher.ValueMember = "id";
        }
        
        // Tải danh sách sách
        private void LoadBooks()
        {
            var books = bookBLL.GetAllBooks();
            dgvBook.DataSource = books.Select(book => new
            {
                book.id,
                book.name,
                book.barcode,
                book.price,
                book.quantity,
                book.description,
                book.id_publisher,
                book.code_category,
                image = book.image != null ? ResizeImage(ByteArrayToImage(book.image), 70, 90) : null
            }).ToList();

            dgvBook.Columns["id_publisher"].Visible = false;
            dgvBook.Columns["code_category"].Visible = false;

            // Điều chỉnh cột hình ảnh
            if (!dgvBook.Columns.Contains("image"))
            {
                dgvBook.Columns.Add(new DataGridViewImageColumn
                {
                    Name = "image",
                    HeaderText = "Hình ảnh",
                    ImageLayout = DataGridViewImageCellLayout.Zoom // Để ảnh vừa khung và giữ tỷ lệ
                });
            }

            // Điều chỉnh chiều cao và chiều rộng
            dgvBook.RowTemplate.Height = 100; // Chiều cao hàng

            // Căn giữa chữ trong tất cả các cột
            foreach (DataGridViewColumn column in dgvBook.Columns)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Căn giữa ngang
                column.DefaultCellStyle.WrapMode = DataGridViewTriState.True; // Tự động xuống dòng nếu dài
            }

            // Căn giữa chữ trong tiêu đề cột (Header)
            dgvBook.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Cập nhật dữ liệu hình ảnh vào cột
            foreach (DataGridViewRow row in dgvBook.Rows)
            {
                var image = row.Cells["image"].Value;
                row.Cells["image"].Value = image ?? SystemIcons.Warning.ToBitmap(); // Icon cảnh báo
            }

            dgvBook.Columns["id"].Visible = false;
        }

        private void LoadBooks_search(string searchQuery = "")
        {
            var books = bookBLL.GetAllBooks();

            // Nếu có từ khóa tìm kiếm, lọc danh sách sách theo tên
            if (!string.IsNullOrEmpty(searchQuery))
            {
                books = books.Where(book => book.name != null && book.name.Equals(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            dgvBook.DataSource = books.Select(book => new
            {
                book.id,
                book.name,
                book.barcode,
                book.price,
                book.quantity,
                book.description,
                book.id_publisher,
                book.code_category,
                image = book.image != null ? ResizeImage(ByteArrayToImage(book.image), 70, 90) : null
            }).ToList();

            dgvBook.Columns["id_publisher"].Visible = false;
            dgvBook.Columns["code_category"].Visible = false;

            // Điều chỉnh cột hình ảnh
            if (!dgvBook.Columns.Contains("image"))
            {
                dgvBook.Columns.Add(new DataGridViewImageColumn
                {
                    Name = "image",
                    HeaderText = "Hình ảnh",
                    ImageLayout = DataGridViewImageCellLayout.Zoom
                });
            }

            dgvBook.RowTemplate.Height = 100;

            foreach (DataGridViewColumn column in dgvBook.Columns)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }

            dgvBook.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewRow row in dgvBook.Rows)
            {
                var image = row.Cells["image"].Value;
                row.Cells["image"].Value = image ?? SystemIcons.Warning.ToBitmap();
            }

            dgvBook.Columns["id"].Visible = false;
        }


        // Giảm kích thước hình ảnh
        private Image ResizeImage(Image image, int width, int height)
        {
            if (image == null) return null;

            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, width, height);
            }
            return resizedImage;
        }

        // Tải danh sách tác giả vào cboAuthor
        private void LoadAuthors()
        {
            var authors = authorBLL.GetAllAuthors();  // authorBLL là một thể hiện của lớp AuthorBLL
            cboAuthor.DataSource = authors;
            cboAuthor.DisplayMember = "name";
            cboAuthor.ValueMember = "id";
        }

        // Chuyển đổi hình ảnh thành mảng byte
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

        // Chuyển đổi mảng byte thành ảnh
        private Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null || byteArrayIn.Length == 0)
                return null;

            using (var ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        // Đổ độ ưu tiên lên cboPrio
        private void LoadPriorities()
        {
            var priorities = bookBLL.GetPriorities();
            cboPrio.DataSource = priorities;
            cboPrio.DisplayMember = "Value"; // Hiển thị tên mức độ ưu tiên
            cboPrio.ValueMember = "Key";     // Giá trị thực tế
        }

        // ===========================Các hàm xử lí kiểm tra khi UPDATE
        private bool ValidateInput()
        {
            return !(string.IsNullOrWhiteSpace(txtID.Text) ||
             string.IsNullOrWhiteSpace(txtName.Text) ||
             string.IsNullOrWhiteSpace(txtBarcode.Text) ||
             string.IsNullOrWhiteSpace(txtPrice.Text) ||
             string.IsNullOrWhiteSpace(txtQuantity.Text) ||
             cboAuthor.SelectedValue == null);
        }

        // Lấy hình ảnh hiện tại của sách từ cơ sở dữ liệu
        private byte[] GetExistingImage(int bookId)
        {
            var existingBook = bookBLL.GetAllBooks().FirstOrDefault(b => b.id == bookId);
            return existingBook?.image;
        }

        // Kiểm tra xem sách có hình ảnh cũ không
        private bool HasExistingImage(int bookId)
        {
            return GetExistingImage(bookId) != null;
        }

        // Tạo đối tượng sách để cập nhật
        private book CreateBookObjectForUpdate(int bookId, byte[] image)
        {
            return new book
            {
                id = bookId,
                name = txtName.Text,
                barcode = txtBarcode.Text,
                price = decimal.Parse(txtPrice.Text),
                quantity = int.Parse(txtQuantity.Text),
                description = txtDescription.Text,
                id_publisher = int.Parse(cboPublisher.SelectedValue.ToString()),
                code_category = cboCategory.SelectedValue.ToString(),
                image = image,
                priority = cboPrio.SelectedValue != null ? Convert.ToByte(cboPrio.SelectedValue) : (byte?)null
            };
        }

        // Thực hiện cập nhật sách và tác giả
        private bool UpdateBookAndAuthor(book bookToUpdate)
        {
            if (bookBLL.UpdateBook(bookToUpdate))
            {
                int authorId = int.Parse(cboAuthor.SelectedValue.ToString());
                Console.WriteLine($"cboAuthor SelectedValue changed to: {cboAuthor.SelectedValue}");
                var bookJoinAuthorBLL = new BookJoinAuthorBLL();
                return bookJoinAuthorBLL.UpdateBookJoinAuthor(bookToUpdate.id, authorId);
            }
            return false;
        }

        private void Book_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); // Thoát ứng dụng
        }
    }
}
