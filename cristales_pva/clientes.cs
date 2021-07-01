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
    public partial class clientes : Form
    {
        public clientes()
        {
            InitializeComponent();
            textBox2.Leave += TextBox2_Leave;
            textBox2.Enter += TextBox2_Enter;
            textBox3.Leave += TextBox3_Leave;
            textBox3.Enter += TextBox3_Enter;
            textBox5.Leave += TextBox5_Leave;
            textBox5.Enter += TextBox5_Enter;
            textBox6.Leave += TextBox6_Leave;
            textBox6.Enter += TextBox6_Enter;
            textBox7.KeyPress += TextBox7_KeyPress;
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if(dataGridView1.RowCount == 0)
            {
                e.Cancel = true;
            }
        }

        private void TextBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                loadClients();
            }
        }       

        public void setEditTab()
        {
            tabControl1.SelectedTab = tabPage2;
        }

        private void loadClients()
        {
            if(backgroundWorker1.IsBusy == false)
            {
                pictureBox1.Visible = true;
                backgroundWorker1.RunWorkerAsync();
            }            
        }

        //guardar nuevo cliente
        private void button1_Click(object sender, EventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if (textBox1.Text != "")
            {
                if (sql.findSQLValue("nombre", "nombre", "clientes", textBox1.Text) == false)
                {
                    sql.insertNewClient(textBox1.Text, textBox2.ForeColor == Color.LightGray ? string.Empty : textBox2.Text, textBox3.ForeColor == Color.LightGray ? string.Empty : textBox3.Text, textBox8.Text);
                    textBox1.Text = "";
                    textBox8.Text = "";
                    //----------------->
                    textBox2.Text = "(669)974-3456";
                    textBox2.ForeColor = Color.LightGray;
                    //----------------->
                    textBox3.Text = "user@mail.com";
                    textBox3.ForeColor = Color.LightGray;
                    MessageBox.Show("Un nuevo cliente ha sido ingresado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("[Error] ese nombre de cliente ya existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] es necesario un nombre de cliente.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        //buscar cliente
        private void button3_Click(object sender, EventArgs e)
        {
            loadClients();
        }

        //guardar cliente editado
        private void button2_Click(object sender, EventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if (textBox4.Text != "")
            {
                sql.updateClient(textBox4.Text, textBox5.ForeColor == Color.LightGray ? string.Empty : textBox5.Text, textBox6.ForeColor == Color.LightGray ? string.Empty : textBox6.Text, textBox9.Text);
                loadClients();
                MessageBox.Show("Actualización completa.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("[Error] necesitas seleccionar un cliente de la base de datos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        //edit button
        private void editarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                textBox4.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                textBox5.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                textBox6.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                textBox9.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                textBox5.ForeColor = SystemColors.WindowText;
                textBox6.ForeColor = SystemColors.WindowText;
            }
        }
        //

        //eliminar button
        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                sqlDateBaseManager sql = new sqlDateBaseManager();
                DialogResult r = MessageBox.Show("¿Estás seguro de eliminar este cliente?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (r == DialogResult.Yes)
                {
                    sql.deleteClient(dataGridView1.CurrentRow.Cells[1].Value.ToString());
                    textBox1.Text = "";
                    textBox8.Text = "";
                    //----------------->
                    textBox5.Text = "(669)974-3456";
                    textBox5.ForeColor = Color.LightGray;
                    //----------------->
                    textBox6.Text = "user@mail.com";
                    textBox6.ForeColor = Color.LightGray;
                    loadClients();
                }
            }
        }

        private void TextBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.ForeColor == Color.LightGray)
            {
                textBox2.Text = string.Empty;
                textBox2.ForeColor = SystemColors.WindowText;
            }
        }

        private void TextBox2_Leave(object sender, EventArgs e)
        {          
            if (constants.isLong(textBox2.Text) == true)
            {
                if (textBox2.Text.Length == 10)
                {
                    textBox2.Text = string.Format("{0:(###)###-##-##}", Convert.ToInt64(textBox2.Text));
                }
                else if (textBox2.Text.Length == 7)
                {
                    textBox2.Text = string.Format("{0:###-##-##}", Convert.ToInt64(textBox2.Text));
                }
                else if (textBox2.Text.Length == 6)
                {
                    textBox2.Text = string.Format("{0:##-##-##}", Convert.ToInt64(textBox2.Text));
                }
            }         
            
            if (textBox2.Text.Length == 0)
            {
                textBox2.Text = "(669)974-3456";
                textBox2.ForeColor = Color.LightGray;
            }
        }

        private void TextBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.ForeColor == Color.LightGray)
            {
                textBox3.Text = string.Empty;
                textBox3.ForeColor = SystemColors.WindowText;
            }
        }

        private void TextBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text.Length == 0)
            {
                textBox3.Text = "user@mail.com";
                textBox3.ForeColor = Color.LightGray;
            }
        }

        private void TextBox5_Enter(object sender, EventArgs e)
        {
            if (textBox5.ForeColor == Color.LightGray)
            {
                textBox5.Text = string.Empty;
                textBox5.ForeColor = SystemColors.WindowText;
            }
        }

        private void TextBox5_Leave(object sender, EventArgs e)
        {
            if (constants.isLong(textBox5.Text) == true)
            {
                if (textBox5.Text.Length == 10)
                {
                    textBox5.Text = string.Format("{0:(###)###-##-##}", Convert.ToInt64(textBox5.Text));
                }
                else if (textBox5.Text.Length == 7)
                {
                    textBox5.Text = string.Format("{0:###-##-##}", Convert.ToInt64(textBox5.Text));
                }
                else if (textBox5.Text.Length == 6)
                {
                    textBox5.Text = string.Format("{0:##-##-##}", Convert.ToInt64(textBox5.Text));
                }
            }

            if (textBox5.Text.Length == 0)
            {
                textBox5.Text = "(669)974-3456";
                textBox5.ForeColor = Color.LightGray;
            }
        }

        private void TextBox6_Enter(object sender, EventArgs e)
        {
            if (textBox6.ForeColor == Color.LightGray)
            {
                textBox6.Text = string.Empty;
                textBox6.ForeColor = SystemColors.WindowText;
            }
        }

        private void TextBox6_Leave(object sender, EventArgs e)
        {
            if (textBox6.Text.Length == 0)
            {
                textBox6.Text = "user@mail.com";
                textBox6.ForeColor = Color.LightGray;
            }
        }

        //show clients
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if (textBox7.Text != "")
            {
                sql.dropTableOnGridViewWithFilter(dataGridView1, "clientes", "nombre", textBox7.Text);
            }
            else
            {
                sql.dropTableOnGridView(dataGridView1, "clientes");
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
        }
        // 
    }
}
