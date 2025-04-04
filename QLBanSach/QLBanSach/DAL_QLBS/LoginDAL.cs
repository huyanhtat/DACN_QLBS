using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO_QLBS;

namespace DAL_QLBS
{
    public class LoginDAL
    {
        BookManagementDataContext qlbs = new BookManagementDataContext();

        public LoginDAL()
        {

        }


        public bool getUserNameAndPassword(string username, string password)
        {
            return qlbs.users.Where(u => u.user_name == username && u.password == password).Any()?true:false;
        }

        public string getFullName(string username, string password)
        {
            return qlbs.users.Where(u => u.user_name == username && u.password == (password)).Select(t => t.full_name).FirstOrDefault();
        }
        public string getRole(string username, string password)
        {
            var role = from u in qlbs.users
                       join r in qlbs.roles
                       on u.code_role equals r.code
                       where u.user_name == username && u.password == (password)
                       select r.name;

            return role.FirstOrDefault();
        }

        public string getRoleCode(string username, string password)
        {
            var roleCode = from u in qlbs.users
                       join r in qlbs.roles
                       on u.code_role equals r.code
                       where u.user_name == username && u.password == (password)
                       select r.code;

            return roleCode.FirstOrDefault();
        }

        private string EncryptMD5(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
