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
    public partial class acceso_password : Form
    {
        public bool access = false;

        public acceso_password()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == constants.ps_dl)
            {
                access = true;
                label2.Text = "";
                this.Close();
            }
            else
            {
                label2.Text = "Error: contraseña no válida.";
            }
        }
    }
}
