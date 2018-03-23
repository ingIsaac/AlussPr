using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cristales_pva
{
    public partial class descipcion : Form
    {
        public descipcion(string descipcion)
        {
            InitializeComponent();
            richTextBox1.Text = descipcion;
        }

        private void button1_Click(object sender, EventArgs e)
        {          
            ((Form1)Application.OpenForms["form1"]).setDescription(richTextBox1.Text);
            this.Close();     
        }
    }
}
