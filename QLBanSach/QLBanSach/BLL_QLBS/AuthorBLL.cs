using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLBS;
using DAL_QLBS;

namespace BLL_QLBS
{
    public class AuthorBLL
    {
        private AuthorDAL authorDAL;

        public AuthorBLL()
        {
            authorDAL = new AuthorDAL();
        }

        public bool AddAuthor(author author)
        {
            return authorDAL.AddAuthor(author);
        }

        public List<author> GetAllAuthors()
        {
            return authorDAL.GetAllAuthors();
        }

        public bool UpdateAuthor(author author)
        {
            return authorDAL.UpdateAuthor(author);
        }

        public bool DeleteAuthor(int id)
        {
            return authorDAL.DeleteAuthor(id);
        }
    }
}
