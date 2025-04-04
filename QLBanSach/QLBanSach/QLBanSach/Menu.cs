using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ENUM;

namespace QLBanSach
{
    public partial class Menu : UserControl
    {
        private List<Form> openForms = new List<Form>(); // Danh sách các form đang mở
        public Menu()
        {
            InitializeComponent();
            label2.Text = Properties.Settings.Default.username;
            label3.Text = Properties.Settings.Default.role;
            phanQuyenNV();
        }



        private void toolStripDropDownButton2_Click(object sender, EventArgs e)
        {
            this.ParentForm.Hide();
            Cart cart = new Cart();
            cart.Show();

        }

        private void quảnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ParentForm.Hide();
            Book book = new Book();
            book.Show();
        }

        private void quảnLýDanhMụcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ParentForm.Hide();
            Category category = new Category();
            category.Show();
        }

        private void quảnLíNVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ParentForm.Hide();
            Employee employee = new Employee();
            employee.Show();
        }

        private void quảnLíKHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripDropDownButton6_Click(object sender, EventArgs e)
        {
            this.ParentForm.Hide();
            Bill bill = new Bill();
            bill.Show();
        }

        private void toolStripDropDownButton7_Click(object sender, EventArgs e)
        {
            this.ParentForm.Hide();
            Discount discount = new Discount();
            discount.Show();
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ParentForm.Hide();
            Login login = new Login();
            login.Show();
        }

        private void quảnLíNCCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void quảnLýTácGiảToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ParentForm.Hide();
            Author author = new Author();
            author.Show();
        }

        private void phanQuyenNV()
        {
            if (Properties.Settings.Default.rolecode == Role.EMPLOYEE.ToString())
            {
                toolStripDropDownButton4.Visible = false;
                phânQuyềnToolStripMenuItem1.Visible = false;
                toolStripDropDownButton2.Visible = false;
                toolStripDropDownButton5.Visible = false;
            }
            else
            {
                toolStripDropDownButton2.Visible=false;
            }
        }

        private void toolStripDropDownButton9_Click(object sender, EventArgs e)
        {
            this.ParentForm.Hide();
            Statistical statistical = new Statistical();
            statistical.Show();
        }

        private void phânQuyềnToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.ParentForm.Hide();
            PhanQuyen phanQuyen = new PhanQuyen();
            phanQuyen.Show();
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

        private void quảnLýNhàCungCấpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ParentForm.Hide();
            Publisher publisher = new Publisher();
            publisher.Show();
        }

        private void toolStripDropDownButton5_Click(object sender, EventArgs e)
        {
            this.ParentForm.Hide();
            Customer customer = new Customer();
            customer.Show();
        }
    }
}
