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
        bool checkbox;

        public confirm_password(bool checkbox=false)
        {
            InitializeComponent();
            this.checkbox = checkbox;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == constants.password)
            {
                if (!checkbox)
                {
                    ((buscar_cotizacion)Application.OpenForms["buscar_cotizacion"]).confirmarEliminacion();
                }
                else
                {
                    ((buscar_cotizacion)Application.OpenForms["buscar_cotizacion"]).habilitarEliminacionSegura(false);
                }
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
