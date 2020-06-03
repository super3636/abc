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
        public Form1()
        {
            InitializeComponent();
        }

        private void btnVao_Click(object sender, EventArgs e)
        {
            string ten = txtTen.Text;
            Form2 f = new Form2(ten);
            f.Show();
            this.Hide();
        }
    }
}
