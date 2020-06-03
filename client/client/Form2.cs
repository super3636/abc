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
    public partial class Form2 : Form
    {
        string ten;
        clientProgram Client = new clientProgram();

        public Form2(string ten)
        {
            this.ten = ten;
            Client.setName(ten);
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Client.SetDataFunction = new clientProgram.SetDataControl(setData);
        }
        public void setData(string data)
        {
            lbTinNhan.Items.Add(data);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            label1.Text = ten;
            Client.Connect("127.0.0.1", 8888);
            Client.SendName(ten);
           
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {

            DialogResult result = MessageBox.Show("Do you really want to exit?", "Dialog Title", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                clientProgram.loop = false;
                Client.Disconect();
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Client.SendMessage(textBox1.Text);
            textBox1.Text = "";
        }

  
    }
}
