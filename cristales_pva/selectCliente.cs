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
    public partial class selectCliente : Form
    {
        public selectCliente()
        {
            InitializeComponent();
            textBox1.KeyDown += TextBox1_KeyDown;
            dataGridView1.CellClick += DataGridView1_CellClick;
            dataGridView1.CellLeave += DataGridView1_CellLeave;
        }

        private void DataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                dataGridView1.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
            }
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                sqlDateBaseManager sql = new sqlDateBaseManager();
                dataGridView1.DataSource = null;
                sql.dropTableOnGridViewWithFilter(dataGridView1, "clientes", "nombre", textBox1.Text);
            }
        }

        private void selectCliente_Load(object sender, EventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            sql.dropTableOnGridView(dataGridView1, "clientes");
            textBox1.Select();    
        }

        private void seleccionarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((guardar_cotizacion)Application.OpenForms["guardar_cotizacion"]).setCliente(dataGridView1.CurrentRow.Cells[1].Value.ToString());
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            dataGridView1.DataSource = null;
            sql.dropTableOnGridViewWithFilter(dataGridView1, "clientes", "nombre", textBox1.Text);
        }
    }
}
