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
        int p = 0;

        public asignar_dir()
        {
            InitializeComponent();
            textBox1.KeyDown += TextBox1_KeyDown;
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
           if(e.KeyData == Keys.Enter)
            {
                if (validateInput())
                {
                    ((merge_items)Application.OpenForms["merge_items"]).asignacionManual(p);
                    Close();
                }
                else
                {
                    MessageBox.Show(this, "[Error] configuración no válida.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool validateInput()
        {
            string[] t = textBox1.Text.Split(',');
            bool r = false;
            if(t.Length == 3)
            {
                if (constants.stringToInt(t[0]) >= 1 && constants.stringToInt(t[0]) <= 8 && constants.stringToInt(t[1]) >= 1 && constants.stringToInt(t[1]) <= 8 && (constants.stringToInt(t[2]) == 0 || constants.stringToInt(t[2]) == 1))
                {
                    int u = constants.stringToInt(t[0] + t[1] + t[2]);
                    if (u >= 0 && u <= 888)
                    {
                        p = u;
                        r = true;
                    }
                }
            }
            return r;
        }
    }
}
