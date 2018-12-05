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
    public partial class subfolio_title : Form
    {
        public subfolio_title()
        {
            InitializeComponent();
            textBox1.Text = constants.getSubfoliotitle(constants.sub_folio);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != string.Empty)
            {
                constants.setSubfoliotitle(constants.sub_folio, textBox1.Text);
                if (Application.OpenForms["articulos_cotizacion"] != null) { ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("[Error] necesitas ingresar un título al Sub-Folio.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
