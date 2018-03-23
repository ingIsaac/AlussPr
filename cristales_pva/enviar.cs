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
    public partial class enviar : Form
    {
        public enviar()
        {
            InitializeComponent();
            cargarTiendas();
        }

        private void cargarTiendas()
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            List<string> tiendas = sql.getTiendas();
            if (tiendas.Count > 0)
            {
                comboBox1.Items.Clear();
                foreach (string x in tiendas)
                {
                    comboBox1.Items.Add(x);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Una vez enviada la cotización el documento ya NO podrá ser consultado mas por este usuario. ¿Desea continuar?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(r == DialogResult.Yes)
            {
                if (comboBox1.Text != "")
                {
                    sqlDateBaseManager sql = new sqlDateBaseManager();
                    if (sql.getUserId(textBox1.Text) > 0)
                    {
                        if (sql.isFolioExist(constants.folio_abierto) == true)
                        {
                            sql.updateCotizacionUsuario(constants.folio_abierto, textBox1.Text, comboBox1.Text);
                            ((Form1)Application.OpenForms["form1"]).borrarCotizacion();
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("[Error] el folio de está cotización no existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] el usuario no existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("[Error] necesitas colocar el destino.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Close();
            }
        }
    }
}
