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
    public partial class load_report : Form
    {
        float sub_total, iva, total;

        public load_report(string sub_total, string iva, string total)
        {
            InitializeComponent();
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            FormClosing += Load_report_FormClosing;
            this.sub_total = constants.stringToFloat(sub_total);
            this.iva = constants.stringToFloat(iva);
            this.total = constants.stringToFloat(total);
        }

        private void Load_report_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                e.Cancel = true;
            }
        }

        private void load_report_Load(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == false)
            {
                backgroundWorker1.RunWorkerAsync();              
            }           
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            new reportes(constants.nombre_cotizacion, constants.nombre_proyecto, constants.folio_abierto.ToString(), sub_total, iva, total, constants.desc_cotizacion, constants.desc_cant).Show();
            Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
        }
    }
}
