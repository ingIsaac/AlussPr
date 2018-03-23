using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace cristales_pva
{
    public partial class historial_registros : Form
    {
        sqlDateBaseManager sql = new sqlDateBaseManager();
        string file_name = string.Empty;

        public historial_registros()
        {
            InitializeComponent();
            datagridviewNE1.CellClick += DatagridviewNE1_CellClick;
            textBox1.KeyPress += TextBox1_KeyPress;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            backgroundWorker2.RunWorkerCompleted += BackgroundWorker2_RunWorkerCompleted;
            this.Text = this.Text + " - " + constants.org_name;
            comboBox2.Text = getMesName(DateTime.Now.Month.ToString());
            comboBox3.Text = DateTime.Now.Year.ToString();
            loadPresupuestos();
        }

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Enabled = true;
            pictureBox1.Visible = false;
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                loadPresupuestos(textBox1.Text);
            }
        }

        private void DatagridviewNE1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(backgroundWorker2.IsBusy == false)
            {
                pictureBox1.Visible = true;
                file_name = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                backgroundWorker2.RunWorkerAsync();
            }
        }

        private void loadPresupuestos(string filter="")
        {
            if (backgroundWorker1.IsBusy == false)
            {
                string[] s = new string[] {getMesInt(comboBox2.Text) + "/" + comboBox3.Text, filter};
                richTextBox1.Clear();
                pictureBox1.Visible = true;
                this.Enabled = false;
                backgroundWorker1.RunWorkerAsync(s);              
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            loadPresupuestos();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            loadPresupuestos(textBox1.Text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadPresupuestos(comboBox1.Text);
        }

        private void setcolors()
        {
            DateTime time;
            string etapa;
            foreach (DataGridViewRow x in datagridviewNE1.Rows)
            {
                time = DateTime.Parse(x.Cells[6].Value.ToString());
                etapa = x.Cells[4].Value.ToString();
                //Fechas
                if ((time.AddDays(-10) > DateTime.Today) && etapa != "Proyecto Terminado")
                {
                    x.Cells[6].Style.BackColor = Color.LightGreen;
                }
                else if ((time > DateTime.Today) && etapa != "Proyecto Terminado")
                {
                    x.Cells[6].Style.BackColor = Color.Yellow;
                }
                else if ((time <= DateTime.Today) && etapa != "Proyecto Terminado")
                {
                    x.Cells[6].Style.BackColor = Color.Red;
                }

                //Etapas
                if (etapa == "Cotización Aceptada" || etapa == "Requisito de Material")
                {
                    x.Cells[4].Style.BackColor = Color.LightBlue;
                }
                else if (etapa == "Fabricación" || etapa == "Instalación")
                {
                    x.Cells[4].Style.BackColor = Color.LightPink;
                }
                else if(etapa == "Proyecto Terminado")
                {
                    x.Cells[4].Style.BackColor = Color.LightGreen;
                    x.DefaultCellStyle.BackColor = Color.Gray;
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] s = e.Argument as string[];
            sql.dropPresupuestosOnGridView(datagridviewNE1, s[0], s[1]);
            setcolors();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            richTextBox1.Text = sql.selectRegistroPresupuestos((int)datagridviewNE1.CurrentRow.Cells[0].Value, "informe");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (file_name != "")
            {
                try {
                    string path = string.Empty;
                    saveFileDialog1.Filter = "txt files (*.doc)|*.doc|All files (*.*)|*.*";
                    saveFileDialog1.FileName = file_name;
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        using (Stream s = File.Open(saveFileDialog1.FileName, FileMode.CreateNew))
                        using (StreamWriter sw = new StreamWriter(s))
                        {
                            sw.Write(richTextBox1.Text);
                        }
                    }
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                }
            }
        }

        private string getMesInt(string mes)
        {
            switch (mes)
            {
                case "Enero":
                    return "01";
                case "Febrero":
                    return "02";
                case "Marzo":
                    return "03";
                case "Abril":
                    return "04";
                case "Mayo":
                    return "05";
                case "Junio":
                    return "06";
                case "Julio":
                    return "07";
                case "Agosto":
                    return "08";
                case "Septiembre":
                    return "09";
                case "Octubre":
                    return "10";
                case "Noviembre":
                    return "11";
                case "Diciembre":
                    return "12";
                default:
                    return "";
            }
        }

        private string getMesName(string mes)
        {
            switch (mes)
            {
                case "1":
                    return "Enero";
                case "2":
                    return "Febrero";
                case "3":
                    return "Marzo";
                case "4":
                    return "Abril";
                case "5":
                    return "Mayo";
                case "6":
                    return "Junio";
                case "7":
                    return "Julio";
                case "8":
                    return "Agosto";
                case "9":
                    return "Septiembre";
                case "10":
                    return "Octubre";
                case "11":
                    return "Noviembre";
                case "12":
                    return "Diciembre";
                default:
                    return "";
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadPresupuestos();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadPresupuestos();
        }
    }
}
