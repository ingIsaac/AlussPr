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
    public partial class informe : Form
    {
        string[] param = null;
        public informe(string[] param)
        {
            InitializeComponent();
            this.param = param;
            label1.Text = param[0];
            label2.Text = param[1] + " - " + param[2];
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
        }

        private void loadInforme()
        {
            if (!backgroundWorker1.IsBusy)
            {
                pictureBox1.Visible = true;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(Application.OpenForms["historial_registros"] != null)
            {
                ((historial_registros)Application.OpenForms["historial_registros"]).getInformeToPrint(param);
                Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["historial_registros"] != null)
            {
                ((historial_registros)Application.OpenForms["historial_registros"]).getInformeToPrint(param);
                Close();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            richTextBox1.Text = new sqlDateBaseManager().selectRegistroPresupuestos(constants.stringToInt(param[0]), "informe");
        }

        private void informe_Load(object sender, EventArgs e)
        {
            loadInforme();
        }
    }
}
