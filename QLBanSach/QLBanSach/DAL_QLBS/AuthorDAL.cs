using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DTO_QLBS;
using ENUM;

namespace DAL_QLBS
{
    public class AuthorDAL
    {
        BookManagementDataContext qlbs = new BookManagementDataContext();

        public AuthorDAL()
        {
            
        }

        // Thêm tác giả mới
        public bool AddAuthor(author author)
        {
            try
            {
                qlbs.authors.InsertOnSubmit(author);
                qlbs.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Lấy ra danh sách tác giả
        public List<author> GetAllAuthors()
        {
            return qlbs.authors.ToList();
        }

        // Cập nhật tác giả
        public bool UpdateAuthor(author updatedAuthor)
        {
            try
            {
                var author = qlbs.authors.FirstOrDefault(a => a.id == updatedAuthor.id);
                if (author != null)
                {
                    author.name = updatedAuthor.name;
                    author.address = updatedAuthor.address;
                    author.bio = updatedAuthor.bio;
                    author.phone = updatedAuthor.phone;
                    author.email = updatedAuthor.email;
                    author.image = updatedAuthor.image;

                    qlbs.SubmitChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log hoặc hiển thị chi tiết lỗi
                MessageBox.Show($"Lỗi: {ex.Message}");
                return false;
            }
        }

        // Xoá tác giả
        public bool DeleteAuthor(int id)
        {
            try
            {
                var author = qlbs.authors.FirstOrDefault(a => a.id == id);
                if (author != null)
                {
                    qlbs.authors.DeleteOnSubmit(author);
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
    }
}
