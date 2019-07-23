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
    public partial class precio_especial : Form
    {
        public precio_especial()
        {
            InitializeComponent();
            loadPrecioEspecial();
        }

        private void loadPrecioEspecial()
        {
            richTextBox1.Text = constants.precio_especial_desc;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            constants.precio_especial_desc = richTextBox1.Text;
            //-------------------------------------------------->
            MessageBox.Show(this, "Para que los cambios se guarden de manera permanente es necesario guardar la cotización.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            ((reportes)Application.OpenForms["reportes"]).reload();
            Close();
        }
    }
}
