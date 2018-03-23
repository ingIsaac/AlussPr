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
    public partial class confirm_password : Form
    {
        public confirm_password()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == constants.password)
            {
                ((buscar_cotizacion)Application.OpenForms["buscar_cotizacion"]).confirmarEliminacion();
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
