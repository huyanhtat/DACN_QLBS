using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLBanSach
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void quảnLíNVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Employee employee = new Employee();
            employee.Show();
        }

        private void quảnLíKHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customer customer = new Customer();
            customer.Show();
        }

        private void quảnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Book book = new Book();
            book.Show();
        }

        private void tìmKiếmSáchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search_Book search_Book = new Search_Book();
            search_Book.Show();
        }

        private void tìmKiếmNVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search_Employee search_Employee = new Search_Employee();
            search_Employee.Show();
        }

        private void tìmKiếmKHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search_Customer search_Customer = new Search_Customer();
            search_Customer.Show();
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Hide();
        }

        private void toolStripDropDownButton7_Click(object sender, EventArgs e)
        {
            Sales sales = new Sales();
            sales.Show();
        }
    }
}
