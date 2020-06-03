using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace server
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        serverProgram Server = new serverProgram();
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Server.SetDataFunction = new serverProgram.SetDataControl(Setdata);
            Server.SetSocketFunction = new serverProgram.SetSocketName(Setdata2);
            Server.RemoveSocketFunction = new serverProgram.RemoveSocket(Remove1);
        }
        public void Setdata(string data)
        {
            this.listBox1.Items.Add(data);
        }
        public void Setdata2(string data)
        {
            this.listBox2.Items.Add(data);
        }
        public void Remove1(string data)
        {
            this.listBox2.Items.Remove(data);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Server.Listen();
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serverProgram.Loop = false;
        }
    }
}
