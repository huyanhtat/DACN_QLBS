using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_QLBS
{
    public class EmployeeDTO
    {
        public int id { get; set; }
        public string full_name { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public DateTime date_of_birth { get; set; }

        // Constructor with parameters
        public EmployeeDTO(int id, string full_name, string phone, string address, string email, string gender, DateTime date_of_birth, string user_name, string password)
        {
            this.id = id;
            this.full_name = full_name;
            this.phone = phone;
            this.address = address;
            this.email = email;
            this.gender = gender;
            this.date_of_birth = date_of_birth;
            this.user_name = user_name;
            this.password = password;
        }

        // Default constructor
        public EmployeeDTO() { }

        // Optional: Override ToString() for easier debugging and logging
        public override string ToString()
        {
            return $"ID: {id}, Name: {full_name}, Phone: {phone}, Address: {address}, Email: {email}, Gender: {gender}, Date of Birth: {date_of_birth.ToShortDateString()}";
        }
    }
}
