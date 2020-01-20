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
            richTextBox1.Text = ((reportes)Application.OpenForms["reportes"]).deserializarPrecioSpecial(constants.precio_especial_desc, constants.sub_folio);
        }

        private string serializePrecioSpecial(string precio_especial)
        {
            string r = string.Empty;
            System.Collections.Hashtable table = new System.Collections.Hashtable();

            string[] u = precio_especial.Split('#');
           
            foreach(string x in u)
            {
                string[] k = x.Split('&');
                if(k.Length == 2)
                {
                    table.Add(constants.stringToInt(k[0]), k[1]);
                }
            }
            if (table.ContainsKey(constants.sub_folio))
            {
                table[constants.sub_folio] = richTextBox1.Text;
            }
            else
            {
                table.Add(constants.sub_folio, richTextBox1.Text);
            }
                                   
            foreach (System.Collections.DictionaryEntry v in table)
            {
                if (r != string.Empty)
                {
                    r = r + "#" + v.Key + "&" + v.Value;
                }
                else
                {
                    r = v.Key + "&" + v.Value;
                }
            }
                          
            return r;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!richTextBox1.Text.Contains("&") && !richTextBox1.Text.Contains("#"))
            {
                constants.precio_especial_desc = serializePrecioSpecial(constants.precio_especial_desc);
                //-------------------------------------------------->
                MessageBox.Show(this, "Para que los cambios se guarden de manera permanente es necesario guardar la cotización.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                ((reportes)Application.OpenForms["reportes"]).reload();
                Close();
            }
            else
            {
                MessageBox.Show(this, "[Error] el texto contiene caracteres no permitidos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
