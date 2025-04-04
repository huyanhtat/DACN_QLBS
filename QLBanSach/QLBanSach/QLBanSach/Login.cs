using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL_QLBS;
using ENUM;

namespace QLBanSach
{
    public partial class Login : Form
    {
        LoginBLL login = new LoginBLL();
        public Login()
        {
            InitializeComponent();
            this.btn_login.Click += Btn_login_Click;
        }

        private void Btn_login_Click(object sender, EventArgs e)
        {
            
            if (login.getUserNameAndPassword(txt_username.Text, txt_password.Text))
            {
                MessageBox.Show("Đăng nhập thành công !");
                Properties.Settings.Default.username = login.getFullName(txt_username.Text, txt_password.Text);
                Properties.Settings.Default.role = login.getRole(txt_username.Text, txt_password.Text);
                Properties.Settings.Default.rolecode = login.getRoleCode(txt_username.Text, txt_password.Text);

                this.Hide();

                if (login.getRoleCode(txt_username.Text, txt_password.Text) == Role.ADMIN.ToString())
                {
                    Home home = new Home();
                    home.Show();
                }
                else if (login.getRoleCode(txt_username.Text, txt_password.Text) == Role.EMPLOYEE.ToString())
                {
                    Home home = new Home();
                    home.Show();
                }
                else if (login.getRoleCode(txt_username.Text, txt_password.Text) == Role.SALER.ToString())
                {
                    Cart cart = new Cart();
                    cart.Show();
                }
                else
                {
                    MessageBox.Show("Bạn chưa được cấp quyền để truy cập !");
                }
            }
            else
            {
                MessageBox.Show("Đăng nhập thất bại !");
            }
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
