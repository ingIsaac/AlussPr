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
    public partial class variaciones : Form
    {
        string linea;

        public variaciones(string linea)
        {
            InitializeComponent();
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            this.linea = linea;
            load();
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label2.Visible = false;
        }

        private void load()
        {
            if (!backgroundWorker1.IsBusy)
            {
                label2.Visible = true;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if(datagridviewNE1.RowCount <= 0)
            {
                e.Cancel = true;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            new sqlDateBaseManager().getVariaciones(datagridviewNE1, linea);
        }

        private void añadirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(datagridviewNE1.RowCount > 0)
            {
                int id = (int)datagridviewNE1.CurrentRow.Cells[0].Value;
                List<string> v = new sqlDateBaseManager().getVariacion(id);
                if(v.Count == 6)
                {
                    if(Application.OpenForms["config_modulo"] != null)
                    {
                        ((config_modulo)Application.OpenForms["config_modulo"]).loadVariaciones(v[3], v[4], v[5]);
                        this.Close();
                    }
                }
            }
        }
    }
}
