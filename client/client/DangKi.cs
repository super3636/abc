using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client
{
    public partial class DangKi : Form
    {
        SocketEntity db = new SocketEntity();
        public DangKi()
        {
            InitializeComponent();
        }

        private void btnDangKi_Click(object sender, EventArgs e)
        {
            if(txtMatKhau.Text!=txtReEnter.Text)
            {
                MessageBox.Show("Nhập lại đúng mật khẩu");
                return;
            }
            else
            {
                string ten = txtTenDangNhap.Text;
                var result = db.Accounts.Where(c => c.TenDangNhap == ten).FirstOrDefault();
                if(result!=null)
                {
                    MessageBox.Show("Đã có người sử dụng tên tài khoản này");
                    return;
                }
                else
                {
                    Account ac = new Account { TenDangNhap = txtTenDangNhap.Text, TenHienThi = txtTenHienThi.Text, MatKhau = txtMatKhau.Text };
                    db.Accounts.Add(ac);
                    db.SaveChanges();
                    MessageBox.Show("Tạo tài khoản thành công");
                    this.Hide();
                    Form1 f = new Form1();
                    f.Show();
                }
            }
        }
    }
}
