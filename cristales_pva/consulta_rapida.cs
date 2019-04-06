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
    public partial class consulta_rapida : Form
    {
        BackgroundWorker bg;
        DataTable dt;

        public consulta_rapida()
        {
            InitializeComponent();
            dt = null;
            bg = new BackgroundWorker();
            bg.DoWork += Bg_DoWork;
            bg.RunWorkerCompleted += Bg_RunWorkerCompleted;
            textBox1.KeyDown += TextBox1_KeyDown;
        }

        private void Bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
            label3.Text = "Se encontrarón (" + (datagridviewNE1.RowCount) + ") registros.";
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                buscarArticulo();
            }
        }

        private void buscarArticulo()
        {
            if (!bg.IsBusy)
            {
                if (comboBox2.Text != string.Empty)
                {
                    if (comboBox1.Text != string.Empty)
                    {
                        if (textBox1.Text != string.Empty)
                        {
                            pictureBox1.Visible = true;
                            bg.RunWorkerAsync();
                        }
                        else
                        {
                            MessageBox.Show("[Error] se necesita ingresar un parámetro de búsqueda.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] se necesita seleccionar una tienda.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("[Error] se necesita seleccionar un listado de artículos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Bg_DoWork(object sender, DoWorkEventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            int tienda_id = sql.getTiendaID(comboBox1.Text);
            if (datagridviewNE1.InvokeRequired)
            {
                datagridviewNE1.Invoke((MethodInvoker)delegate
                {
                    dt = sql.getArticuloInventario(comboBox2.Text, tienda_id, textBox1.Text);
                    datagridviewNE1.DataSource = dt;
                    foreach(DataGridViewRow x in datagridviewNE1.Rows)
                    {
                        if(x.Cells[6].Value.ToString() != "0")
                        {
                            x.Cells[6].Style.BackColor = Color.LightGreen;
                        }
                        else
                        {
                            x.Cells[6].Style.BackColor = Color.Red;
                        }
                    }
                });
            }
            else
            {
                dt = sql.getArticuloInventario(comboBox2.Text, tienda_id, textBox1.Text);
                datagridviewNE1.DataSource = dt;
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    if (x.Cells[6].Value.ToString() != "0")
                    {
                        x.Cells[6].Style.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        x.Cells[6].Style.BackColor = Color.Red;
                    }
                }
            }
        }

        private void consulta_rapida_Load(object sender, EventArgs e)
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
                comboBox1.Text = constants.org_name;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            buscarArticulo();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new print_inventarios(dt, comboBox2.Text, "", "", comboBox1.Text).ShowDialog(this);
        }
    }
}
