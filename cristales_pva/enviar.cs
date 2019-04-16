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
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            this.Shown += Enviar_Shown;
        }

        private void Enviar_Shown(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                label3.Text = "Cargando...";
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label3.Text = string.Empty;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
        }

        private void cargarTiendas()
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            List<string> usuarios = sql.getUsersList();
            if (usuarios.Count > 0)
            {
                comboBox2.Items.Clear();
                foreach (string x in usuarios)
                {
                    comboBox2.Items.Add(x);
                }
            }
            //
            constants.setTiendas(comboBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Una vez enviada la cotización el documento ya NO podrá ser consultado mas por este usuario. ¿Desea continuar?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(r == DialogResult.Yes)
            {
                if (comboBox1.Text != "")
                {
                    sqlDateBaseManager sql = new sqlDateBaseManager();
                    if (sql.getUserId(comboBox2.Text) > 0)
                    {
                        if (sql.isFolioExist(constants.folio_abierto) == true)
                        {
                            sql.updateCotizacionUsuario(constants.folio_abierto, comboBox2.Text, comboBox1.Text);
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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            cargarTiendas();                
        }
    }
}
