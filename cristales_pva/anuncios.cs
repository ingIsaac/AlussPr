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
    public partial class anuncios : Form
    {
        public anuncios()
        {
            InitializeComponent();
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Select();
            button1.Focus();
        }

        private void anuncios_Load(object sender, EventArgs e)
        {
            this.Text = constants.org_name;
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> list = new sqlDateBaseManager().getAnuncios();
            if (list.Count > 0)
            {
                if (richTextBox1.InvokeRequired)
                {
                    richTextBox1.Invoke((MethodInvoker)delegate
                    {
                        richTextBox1.Lines = list.ToArray();
                    });
                }
                else
                {
                    richTextBox1.Lines = list.ToArray();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
