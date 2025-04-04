using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_QLBS;
using DTO_QLBS;

namespace BLL_QLBS
{
    public class BookJoinAuthorBLL
    {
        private BookJoinAuthorDAL bookJoinAuthorDAL;
        public BookJoinAuthorBLL()
        {
            bookJoinAuthorDAL = new BookJoinAuthorDAL();
        }

        // Thêm một bản ghi vào bảng book_join_author
        public bool AddBookJoinAuthor(int idBook, int idAuthor, string role)
        {
            var newRecord = new book_join_author
            {
                id_book = idBook,
                id_author = idAuthor,
                role = role
            };

            return bookJoinAuthorDAL.AddBookJoinAuthor(newRecord);
        }
        // Cập nhật tác giả liên kết với sách
        public bool UpdateBookJoinAuthor(int idBook, int idAuthor)
        {
            return bookJoinAuthorDAL.UpdateBookJoinAuthor(idBook, idAuthor);
        }
        // Lấy ID tác giả theo ID sách
        public int? GetAuthorIdByBookId(int idBook)
        {
            return bookJoinAuthorDAL.GetAuthorIdByBookId(idBook);
        }

        // Lấy tất cả các bản ghi từ bảng book_join_author
        public List<book_join_author> GetAllBookJoinAuthors()
        {
            return bookJoinAuthorDAL.GetAllBookJoinAuthors();
        }
    }
}
