using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLBS;
using ENUM;

namespace DAL_QLBS
{
    public class CategoryDAL
    {
        BookManagementDataContext qlbs = new BookManagementDataContext();
        public CategoryDAL()
        {
            
        }
        // Create
        public bool AddCategory(book_category category)
        {
            try
            {
                qlbs.book_categories.InsertOnSubmit(category);
                qlbs.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Read
        public List<book_category> GetAllCategories()
        {
            return qlbs.book_categories.ToList();
        }

        // Update
        public bool UpdateCategory(book_category category)
        {
            try
            {
                Console.WriteLine("Đang cập nhật danh mục với ID: " + category.id);

                // Tìm danh mục trong cơ sở dữ liệu
                var existingCategory = qlbs.book_categories.FirstOrDefault(c => c.id == category.id);

                if (existingCategory != null)
                {
                    // Gán lại giá trị mới
                    existingCategory.name = category.name;
                    existingCategory.code = category.code;

                    qlbs.SubmitChanges(); // Lưu thay đổi vào DB
                    return true;
                }
                else
                {
                    Console.WriteLine("Không tìm thấy danh mục với ID: " + category.id);
                    return false; // Không tìm thấy danh mục cần cập nhật
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi cập nhật danh mục: " + ex.Message);
                return false;
            }
        }

        // Delete
        public bool DeleteCategory(int categoryId)
        {
            try
            {
                var categoryToDelete = qlbs.book_categories.FirstOrDefault(c => c.id == categoryId);
                if (categoryToDelete != null)
                {
                    qlbs.book_categories.DeleteOnSubmit(categoryToDelete);
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

        // Lấy danh mục theo id
        public string GetCategoryNameByCode(string categoryCode)
        {
            var category = qlbs.book_categories.FirstOrDefault(c => c.code == categoryCode);
            return category != null ? category.name : null;
        }

    }
}
