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
        int type = 1;

        public anuncios()
        {
            InitializeComponent();
            label2.Text = "v." + constants.version;
            this.KeyDown += Anuncios_KeyDown;
        }

        private void Anuncios_KeyDown(object sender, KeyEventArgs e)
        {
            this.Close();
        }

        private void anuncios_Load(object sender, EventArgs e)
        {
            this.Text = constants.org_name;
            load();
            button1.Focus();
        }

        private void load()
        {
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> list;
            if (type == 1)
            {
                type = 2;
                list = new sqlDateBaseManager().getAnuncios();                                  
            }
            else
            {
                type = 1;
                list = new sqlDateBaseManager().getChangelog();               
            }
            //
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(type == 2)
            {
                button2.Text = "Anuncios";
                button2.Image = Properties.Resources.Actions_arrow_left_icon;
                button2.TextImageRelation = TextImageRelation.ImageBeforeText;
            }
            else
            {
                button2.Text = "Changelog";
                button2.Image = Properties.Resources.Actions_arrow_right_icon;
                button2.TextImageRelation = TextImageRelation.TextBeforeImage;
            }
            richTextBox1.ScrollToCaret();
            load();
        }
    }
}
