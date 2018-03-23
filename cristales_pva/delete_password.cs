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
    public partial class delete_password : Form
    {
        bool delete;

        public delete_password(bool delete=true)
        {
            InitializeComponent();
            this.delete = delete;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (delete == true)
            {
                if (textBox1.Text == constants.ps_dl)
                {
                    ((admin_panel)Application.OpenForms["admin_panel"]).setPermiso(true);
                    label2.Text = "";
                    this.Close();
                }
                else
                {
                    ((admin_panel)Application.OpenForms["admin_panel"]).setPermiso(false);
                    label2.Text = "Error: contraseña no válida.";
                }
            }
            else
            {
                if (textBox1.Text == constants.ps_dl)
                {
                    ((config_modulo)Application.OpenForms["config_modulo"]).permitirCambioParametros(true);                 
                    label2.Text = "";
                    this.Close();
                }
                else
                {
                    ((config_modulo)Application.OpenForms["config_modulo"]).permitirCambioParametros(false);                   
                    label2.Text = "Error: contraseña no válida.";
                }
            }
        }
    }
}
