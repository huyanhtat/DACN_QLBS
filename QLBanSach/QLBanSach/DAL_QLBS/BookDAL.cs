using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using DTO_QLBS;

namespace DAL_QLBS
{
    public class BookDAL
    {
        private BookManagementDataContext qlbs = new BookManagementDataContext();

        public BookDAL() { }
        // Phương thức lấy tất cả sách đang còn
        public List<BookDTO> GetAllBooks()
        {
            return qlbs.books
               .Where(book => book.status == 1)
               .Select(book => new BookDTO
               {
                id = book.id,
                name = book.name,
                barcode = book.barcode,
                price = book.price,
                quantity = book.quantity,
                description = book.description,
                id_publisher = book.id_publisher,
                code_category = book.code_category,
                image = book.image != null ? book.image.ToArray() : null,
                priority = book.priority
            }).ToList();
        }

        // Thêm sách
        public bool AddBook(book book)
        {
            try
            {
                qlbs.books.InsertOnSubmit(book);
                qlbs.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddBook: {ex.Message}");
                return false;
            }
        }


        // Cập nhật sách
        public bool UpdateBook(book book)
        {
            try
            {
                var existingBook = qlbs.books.SingleOrDefault(b => b.id == book.id);
                if (existingBook != null)
                {
                    existingBook.name = book.name;
                    existingBook.barcode = book.barcode;
                    existingBook.price = book.price;
                    existingBook.quantity = book.quantity;
                    existingBook.description = book.description;
                    existingBook.id_publisher = book.id_publisher;
                    existingBook.code_category = book.code_category;

                    // Kiểm tra nếu hình ảnh không null mới cập nhật
                    if (book.image != null)
                    {
                        existingBook.image = book.image;
                    }

                    existingBook.priority = book.priority;
                    qlbs.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Xóa sách
        public bool DeleteBook(int bookId)
        {
            try
            {
                Console.WriteLine($"Đang xử lý cập nhật status cho sách với ID: {bookId}");

                // Lấy sách từ bảng books
                var book = qlbs.books.SingleOrDefault(b => b.id == bookId);
                if (book != null)
                {
                    Console.WriteLine($"Đang cập nhật status cho sách: {book.name}");
                    book.status = 0; // Cập nhật status thành 0
                }
                else
                {
                    Console.WriteLine($"Không tìm thấy sách với ID: {bookId}");
                    return false;
                }

                // Lưu thay đổi
                qlbs.SubmitChanges();
                Console.WriteLine("Cập nhật status của sách thành công!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật status của sách: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return false;
            }
        }

        // Lấy ra tác giả của cuốn sách
        public string GetAuthorNameByBookId(int bookId)
        {
            var authorName = (from bja in qlbs.book_join_authors
                              join a in qlbs.authors on bja.id_author equals a.id
                              where bja.id_book == bookId
                              select a.name).FirstOrDefault();
            return authorName;
        }

        public List<KeyValuePair<int, string>> GetPriorities()
        {
            var priorityList = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(1, "Sách mới nhất"),
                new KeyValuePair<int, string>(2, "Sách nổi bật"),
                new KeyValuePair<int, string>(3, "Sách thường"),
                new KeyValuePair<int, string>(4, "Sách sắp ra mắt")
            };

            return priorityList;
        }

    }
}