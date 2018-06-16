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
    public partial class asignar_dir : Form
    {
        public asignar_dir()
        {
            InitializeComponent();
            textBox1.KeyDown += TextBox1_KeyDown;
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
           if(e.KeyData == Keys.Enter)
            {
                int p = constants.stringToInt(textBox1.Text);
                if (p >= 0 && p <= 888)
                {
                    ((merge_items)Application.OpenForms["merge_items"]).asignacionManual(p);
                    Close();
                }
                else
                {
                    MessageBox.Show(this, "[Error] dato no válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
