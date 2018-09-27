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
            this.KeyDown += Anuncios_KeyDown;
            richTextBox1.KeyDown += RichTextBox1_KeyDown;
        }

        private void RichTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            this.Close();
        }

        private void Anuncios_KeyDown(object sender, KeyEventArgs e)
        {
            this.Close();            
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
    }
}
