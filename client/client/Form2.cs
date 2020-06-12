using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client
{
    public partial class Form2 : Form
    {
        string ten;
        clientProgram Client = new clientProgram();
         DataTable table = null;
         public delegate void ShowFormData();
         public ShowFormData ShowFormFunction = null; 
        public Form2(string ten)
        {  
            this.ten = ten;
            Client.setName(ten);
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            table = new DataTable();
            table.Columns.Add("name",typeof(string));
            table.Columns.Add("IP",typeof(string));
            DataRow row = table.NewRow();
            row[0] = "(All)";
            table.Rows.Add(row);
            cbOnline.DataSource = table;
            cbOnline.DisplayMember = "name";
            cbOnline.ValueMember = "IP";
            //DataTable table2 = new DataTable();
            //table2.Columns.Add("name", typeof(string));
            //table2.Columns.Add("id", typeof(int));
            //DataRow dt = table2.NewRow();
            //dt[0] = "buu1";
            //dt[1] = 0;
            //table2.Rows.Add(dt);
            //comboBox1.DataSource = table2;
            //comboBox1.DisplayMember = "name";
            //comboBox1.ValueMember = "id";
            Client.SetDataFunction = new clientProgram.SetDataControl(setData);
            Client.SetSocketFunction = new clientProgram.SetSocketData(setSocket);
            Client.RemoveSocketFunction = new clientProgram.RemoveSocketData(removeSocket);
            Client.close = new clientProgram.Close(Closing1);
        }
        public void setData(string data)
        {
            lbTinNhan.Items.Add(data);

        }
        public void setSocket(Client cl)
        {
            DataRow row = table.NewRow();
            row[0] = cl.name;
            row[1] = cl.clIP.ToString();
            table.Rows.Add(row);
            cbOnline.DataSource = table;
        }
        public void removeSocket(Client cl)
        {
            DataRow []row = table.Select("IP = '"+cl.clIP.ToString()+"'");
            for (int i = 0; i < row.Length;i++)
            {
                row[i].Delete();
            }
              
            table.AcceptChanges();
         
            cbOnline.DataSource = table;
      
        }

        private void Form2_Load(object sender, EventArgs e)
        {   
            label4.Text = ten;
            Client.Connect("127.0.0.1", 8888);
            try
            {
                Client.SendName(ten);
            }
            catch
            {
                this.Hide();
            }
            txtMessage.Select();
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        { 
                clientProgram.loop = false;
                Client.Disconect();
            
                Environment.Exit(1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cbOnline.SelectedIndex == 0)
            {
                Client.SendMessage(txtMessage.Text);
                txtMessage.Text = "";
            }
            else
            {
                Client.SendMessage("(Private) " + txtMessage.Text + " "+cbOnline.Text+" "+ cbOnline.SelectedValue.ToString());           
                txtMessage.Text = "";
            }
        }
        public void Closing1()
        {
            this.Hide();
            
        }

        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {

                btnSent.PerformClick();
                e.Handled = true;
            }
        }
    }
}
