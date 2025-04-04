using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL_QLBS;
using DTO_QLBS;

namespace BLL_QLBS
{
    public class LoginBLL
    {
        LoginDAL login = new LoginDAL();

        public LoginBLL()
        {

        }

        public bool getUserNameAndPassword(string username, string password)
        {
            return login.getUserNameAndPassword(username, password); 
        }

        public string getFullName(string username, string password)
        {
            return login.getFullName(username, password);
        }

        public string getRole(string username, string password)
        {
            return login.getRole(username, password);
        }

        public string getRoleCode(string username, string password)
        {
            return login.getRoleCode(username, password);
        }
    }
}
