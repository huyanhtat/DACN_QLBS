using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL_QLBS;
using DTO_QLBS;

namespace BLL_QLBS
{
    public class BookBLL
    {
        private BookDAL bookDAL;
        private PublisherDAL publisherDAL;
        private CategoryDAL categoryDAL;

        public BookBLL()
        {
            bookDAL = new BookDAL();
            publisherDAL = new PublisherDAL();
            categoryDAL = new CategoryDAL();
        }
        // Phương thức lấy tất cả sách
        public List<BookDTO> GetAllBooks()
        {
            return bookDAL.GetAllBooks();
        }

        // Thêm sách
        public bool AddBook(book book)
        {
            if (!ValidateBookData(book))
            {
                MessageBox.Show("Dữ liệu sách không hợp lệ. Vui lòng kiểm tra lại!");
                return false;
            }

            return bookDAL.AddBook(book);
        }

        // Cập nhật sách
        public bool UpdateBook(book book)
        {
            return bookDAL.UpdateBook(book);
        }

        // Xóa sách
        public bool DeleteBook(int bookId)
        {
            return bookDAL.DeleteBook(bookId);
        }

        // Phương thức lấy tên nhà xuất bản theo ID
        public string GetPublisherName(int publisherId)
        {
            return publisherDAL?.GetPublisherNameById(publisherId) ?? string.Empty;
        }

        // Phương thức lấy tên danh mục theo mã
        public string GetCategoryName(string categoryCode)
        {
            return categoryDAL?.GetCategoryNameByCode(categoryCode) ?? string.Empty;
        }

        // Phương thức lấy tên tác giả theo mã sách
        public string GetAuthorName(int bookId)
        {
            return bookDAL.GetAuthorNameByBookId(bookId);
        }

        // Kiểm tra trước khi thêm
        private bool ValidateBookData(book book)
        {
            if (string.IsNullOrWhiteSpace(book.name)) return false;
            if (string.IsNullOrWhiteSpace(book.barcode)) return false;
            if (book.price <= 0) return false;
            if (book.quantity < 0) return false;
            if (book.id_publisher <= 0) return false;
            if (string.IsNullOrWhiteSpace(book.code_category)) return false;
            return true;
        }

        // Lấy độ ưu tiên
        public List<KeyValuePair<int, string>> GetPriorities()
        {
            return bookDAL.GetPriorities();
        }

    }
}
