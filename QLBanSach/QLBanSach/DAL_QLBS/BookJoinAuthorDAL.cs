using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLBS;
using ENUM;

namespace DAL_QLBS
{
    public class BookJoinAuthorDAL
    {
        private BookManagementDataContext qlbs = new BookManagementDataContext();
        public BookJoinAuthorDAL()
        {
            
        }
        // Thêm một bản ghi vào bảng book_join_author
        public bool AddBookJoinAuthor(book_join_author newRecord)
        {
            try
            {
                qlbs.book_join_authors.InsertOnSubmit(newRecord);
                qlbs.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Sửa bản ghi trong book_join_author
        public bool UpdateBookJoinAuthor(int bookId, int authorId)
        {
            try
            {
                // Tìm bản ghi cần cập nhật trong bảng book_join_author
                var existingRecord = qlbs.book_join_authors.SingleOrDefault(bja => bja.id_book == bookId && bja.id_author == authorId);

                if (existingRecord != null)
                {
                    Console.WriteLine($"Found record: bookId = {existingRecord.id_book}, authorId = {existingRecord.id_author}");
                    // Cập nhật id_author
                    existingRecord.id_author = authorId;
                    qlbs.SubmitChanges();
                    return true;
                }
                else
                {
                    Console.WriteLine("No existing record found, creating a new one.");
                    // Nếu không tìm thấy bản ghi, thêm mới
                    var newRecord = new book_join_author
                    {
                        id_book = bookId,
                        id_author = authorId,
                        role = "Tác giả chính" 
                    };
                    qlbs.book_join_authors.InsertOnSubmit(newRecord);
                    qlbs.SubmitChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateBookJoinAuthor: {ex.Message}");
                return false;
            }
        }


        // Lấy ID tác giả theo ID sách
        public int? GetAuthorIdByBookId(int idBook)
        {
            var record = qlbs.book_join_authors.FirstOrDefault(bja => bja.id_book == idBook);
            return record?.id_author;
        }

        // Lấy tất cả các bản ghi từ bảng book_join_author
        public List<book_join_author> GetAllBookJoinAuthors()
        {
            return qlbs.book_join_authors.ToList();
        }
    }
}
