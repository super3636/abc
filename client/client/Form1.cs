using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace client
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        SocketEntity db = new SocketEntity();
        Form2 f;
        public Form1()
        {   
            InitializeComponent();
        
        }

        private void btnVao_Click(object sender, EventArgs e)
        {
            var result = db.Accounts.Where(c => c.TenDangNhap == txtTen.Text && c.MatKhau == txtMatKhau.Text).FirstOrDefault();
            if (result == null)
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu");
                return;
            }
            else
            {
                string ten = (result as Account).TenHienThi;
                f = new Form2(ten);
                f.Show();
                this.Hide();
            }  
        }
        private void txtTen_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                btnVao.PerformClick();
            }
        }
        public void ShowForm()
        {
            this.Show();
        }
        private void btnDangKi_Click(object sender, EventArgs e)
        {
            DangKi f = new DangKi();
            f.Show();
            this.Hide();
        }
    }
}
