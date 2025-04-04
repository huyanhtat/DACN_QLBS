using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLBS;
using DAL_QLBS;

namespace BLL_QLBS
{
    public class CategoryBLL
    {
        private CategoryDAL categoryDAL;
        public CategoryBLL()
        {
            categoryDAL = new CategoryDAL();
        }
        public bool AddCategory(book_category category)
        {
            return categoryDAL.AddCategory(category);
        }

        public List<book_category> GetAllCategories()
        {
            return categoryDAL.GetAllCategories();
        }

        public bool UpdateCategory(book_category category)
        {
            return categoryDAL.UpdateCategory(category);
        }

        public bool DeleteCategory(int categoryId)
        {
            return categoryDAL.DeleteCategory(categoryId);
        }
    }
}
