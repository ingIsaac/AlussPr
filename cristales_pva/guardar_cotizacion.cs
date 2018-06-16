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
    public partial class guardar_cotizacion : Form
    {
        public bool new_cotizacion = false;

        public guardar_cotizacion()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker2.RunWorkerCompleted += BackgroundWorker2_RunWorkerCompleted;
            this.FormClosing += Guardar_cotizacion_FormClosing;
        }

        private void Guardar_cotizacion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(backgroundWorker1.IsBusy || backgroundWorker2.IsBusy)
            {
                e.Cancel = true;
            }
        }

        public void setCliente(string cliente)
        {
            textBox1.Text = cliente;
        }

        private void guardar_cotizacion_Load(object sender, EventArgs e)
        {
            label3.Text = constants.user;
            progressBar1.Visible = false;
            progressBar1.Maximum = 100;
            if(constants.folio_abierto >= 0)
            {
                label5.Text = constants.folio_abierto.ToString();
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                button3.Enabled = false;
            }
            else
            {
                checkBox1.Visible = false;
                label5.Text = constants.setFolio().ToString();
            }
            textBox1.Text = constants.nombre_cotizacion;
            textBox2.Text = constants.nombre_proyecto;
            if(textBox1.Text == "")
            {
                textBox1.Select();
            }
        }

        //progreso de guardar---------------------
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            pictureBox1.Visible = true;
            label7.Text = "Cargando...";
            new cotizaciones().guardarCotizacion(textBox1, textBox2, button3, label5, backgroundWorker1, new_cotizacion, this);
            if (backgroundWorker1.CancellationPending == false)
            {
                label7.Text = "Guardando Datos...";
                constants.clearCotizacionesLocales();
                backgroundWorker1.ReportProgress(15);
                sql.dropTableArticulosCotizacionesToLocal("cristales_cotizados", constants.folio_abierto);
                backgroundWorker1.ReportProgress(32);
                sql.dropTableArticulosCotizacionesToLocal("aluminio_cotizado", constants.folio_abierto);
                backgroundWorker1.ReportProgress(49);
                sql.dropTableArticulosCotizacionesToLocal("herrajes_cotizados", constants.folio_abierto);
                backgroundWorker1.ReportProgress(66);
                sql.dropTableArticulosCotizacionesToLocal("otros_cotizaciones", constants.folio_abierto);
                backgroundWorker1.ReportProgress(83);
                sql.dropTableArticulosCotizacionesToLocal("modulos_cotizaciones", constants.folio_abierto);
                backgroundWorker1.ReportProgress(100);
            }
            else
            {              
                e.Cancel = true;
            }                  
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            progressBar1.Value = 0;
            pictureBox1.Visible = false;
            if (e.Cancelled == false)
            {
                if (constants.cotizacion_error == false)
                {
                    MessageBox.Show(this, "Se ha guardado está cotización.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    constants.save_onEdit.Clear();
                    pictureBox1.Visible = true;
                    backgroundWorker2.RunWorkerAsync();
                }
            }
            label7.Text = "";
        }

        //cancelar
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //guardar
        private void button1_Click(object sender, EventArgs e)
        {
            if (new sqlDateBaseManager().setServerConnection() == true)
            {
                if (backgroundWorker1.IsBusy != true && backgroundWorker2.IsBusy != true)
                {
                    progressBar1.Visible = true;
                    pictureBox1.Visible = true;                
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        //ver clientes
        private void button3_Click(object sender, EventArgs e)
        {
            new selectCliente().ShowDialog();
        }

        //Cambiar parametros
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button3.Enabled = true;
            }
            else
            {
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                button3.Enabled = false;
            }
        }

        //abriendo cotizacion
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            label7.Text = "Abriendo cotización...";
            constants.setClienteToPropiedades(constants.folio_abierto, constants.nombre_cotizacion, constants.nombre_proyecto, constants.desc_cotizacion, constants.utilidad_cotizacion);
            constants.deleteFilasBorradasFromLocalDB();
            if (constants.ac_cotizacion == true && constants.reload_precios == true)
            {
                constants.errors_Open.Clear();
                for (int i = 1; i < 5; i++)
                {
                    constants.reloadPreciosCotizaciones(i);
                }
            }
            constants.reloadCotizaciones();
            ((Form1)Application.OpenForms["Form1"]).setFolioLabel();
            ((Form1)Application.OpenForms["Form1"]).setEditImage(false, false);
            constants.cotizacion_guardada = true;
        }
       
        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
            label7.Text = "";
            this.Close();
        }
    }
}
