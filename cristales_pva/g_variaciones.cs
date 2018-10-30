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
    public partial class g_variaciones : Form
    {
        int id = 0;

        public g_variaciones()
        {
            InitializeComponent();
            //Adds
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
            contextMenuStrip2.Opening += ContextMenuStrip2_Opening;
            //Others
            setLines();
        }

        private void setLines()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox1.Items.AddRange(constants.getLineasModulo().ToArray());
            comboBox2.Items.AddRange(constants.getLineasModulo().ToArray());
        }

        private void ContextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if(datagridviewNE2.Rows.Count <= 0)
            {
                e.Cancel = true;
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if(datagridviewNE1.Rows.Count <= 0)
            {
                e.Cancel = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox2.SelectedIndex >= 0)
            {
                if (!backgroundWorker1.IsBusy)
                {
                    label5.Visible = true;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            new sqlDateBaseManager().getVariaciones(datagridviewNE2, comboBox2.Text);
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label5.Visible = false;
        }

        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(datagridviewNE2.RowCount > 0)
            {
                DialogResult r = MessageBox.Show("¿Estás seguro de eliminar esta variación?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (r == DialogResult.Yes)
                {
                    int id = (int)datagridviewNE2.CurrentRow.Cells[0].Value;
                    new sqlDateBaseManager().deleteVariacion(id);
                    if (comboBox2.SelectedIndex >= 0)
                    {
                        if (!backgroundWorker1.IsBusy)
                        {
                            label5.Visible = true;
                            backgroundWorker1.RunWorkerAsync();
                        }
                    }
                    reset();
                }
            }
        }

        //Reload
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex >= 0)
            {
                if (!backgroundWorker1.IsBusy)
                {
                    label5.Visible = true;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if(textBox1.Text != string.Empty)
            {
                if(comboBox1.Text != string.Empty)
                {
                    if(datagridviewNE1.RowCount > 0)
                    {
                        if (id > 0)
                        {
                            DialogResult r = MessageBox.Show("¿Estás seguro de modificar esta variación?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (r == DialogResult.Yes)
                            {                                
                                sql.updateVariacion(id, comboBox1.Text, getCambios(), getNuevos(), richTextBox1.Text);
                                MessageBox.Show("Se ha guardado la nueva configuración.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                reset();                              
                            }
                        }
                        else
                        {
                            if (!sql.existVariacion(textBox1.Text))
                            {
                                sql.newVariacion(textBox1.Text, comboBox1.Text, getCambios(), getNuevos(), richTextBox1.Text);
                                MessageBox.Show("Se ha guardado una nueva variación.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                reset();
                            }
                            else
                            {
                                MessageBox.Show("[Error] ya se ha ingresado una variación con el mismo nombre.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] debes de añadir uno o mas artículos a la variación.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("[Error] debes de asignarle una linea a la variación.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] debes de asignarle un nombre a la variación.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }           
        }

        private void reset()
        {
            textBox1.Clear();
            comboBox1.SelectedIndex = -1;
            datagridviewNE1.Rows.Clear();
            richTextBox1.Clear();
            id = 0;
            textBox1.Enabled = true;
        }

        private void removerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(datagridviewNE1.RowCount > 0)
            {
                datagridviewNE1.Rows.Remove(datagridviewNE1.CurrentRow);
            }
        }

        public void setNewItem(int cp, string clave, string operacion, float cantidad = -1)
        {
            listas_entities_pva listas = new listas_entities_pva();

            if (datagridviewNE1.InvokeRequired)
            {
                datagridviewNE1.Invoke((MethodInvoker)delegate 
                {               
                    if (cp == 1)
                    {
                        var perfiles = (from x in listas.perfiles where x.clave == clave select x).SingleOrDefault();
                        if (perfiles != null)
                        {
                            datagridviewNE1.Rows.Add(cp, clave, perfiles.articulo, cantidad, operacion);
                        }
                    }
                    else if (cp == 2)
                    {
                        var cristales = (from x in listas.lista_costo_corte_e_instalado where x.clave == clave select x).SingleOrDefault();
                        if (cristales != null)
                        {
                            datagridviewNE1.Rows.Add(cp, clave, cristales.articulo, cantidad, operacion);
                        }
                    }
                    else if (cp == 3)
                    {
                        var herrajes = (from x in listas.herrajes where x.clave == clave select x).SingleOrDefault();
                        if (herrajes != null)
                        {
                            datagridviewNE1.Rows.Add(cp, clave, herrajes.articulo, cantidad, operacion);
                        }
                    }
                    else if (cp == 4)
                    {
                        var otros = (from x in listas.otros where x.clave == clave select x).SingleOrDefault();
                        if (otros != null)
                        {
                            datagridviewNE1.Rows.Add(cp, clave, otros.articulo, cantidad, operacion);
                        }
                    }

                    foreach (DataGridViewRow x in datagridviewNE1.Rows)
                    {
                        if (x.Cells[4].Value.ToString() != "")
                        {
                            foreach (DataGridViewCell n in x.Cells)
                            {
                                if (n.OwningColumn.HeaderText != "cantidad")
                                {
                                    n.Style.BackColor = Color.Yellow;
                                }
                            }
                        }
                        else
                        {
                            foreach (DataGridViewCell n in x.Cells)
                            {
                                if (n.OwningColumn.HeaderText != "cantidad")
                                {
                                    n.Style.BackColor = Color.LightGreen;
                                }
                            }
                        }
                    }
                });
            }
            else
            {
                if (cp == 1)
                {
                    var perfiles = (from x in listas.perfiles where x.clave == clave select x).SingleOrDefault();
                    if (perfiles != null)
                    {
                        datagridviewNE1.Rows.Add(cp, clave, perfiles.articulo, cantidad, operacion);
                    }
                }
                else if (cp == 2)
                {
                    var cristales = (from x in listas.lista_costo_corte_e_instalado where x.clave == clave select x).SingleOrDefault();
                    if (cristales != null)
                    {
                        datagridviewNE1.Rows.Add(cp, clave, cristales.articulo, cantidad, operacion);
                    }
                }
                else if (cp == 3)
                {
                    var herrajes = (from x in listas.herrajes where x.clave == clave select x).SingleOrDefault();
                    if (herrajes != null)
                    {
                        datagridviewNE1.Rows.Add(cp, clave, herrajes.articulo, cantidad, operacion);
                    }
                }
                else if (cp == 4)
                {
                    var otros = (from x in listas.otros where x.clave == clave select x).SingleOrDefault();
                    if (otros != null)
                    {
                        datagridviewNE1.Rows.Add(cp, clave, otros.articulo, cantidad, operacion);
                    }
                }

                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    if (x.Cells[4].Value.ToString() != "")
                    {
                        foreach (DataGridViewCell n in x.Cells)
                        {
                            if (n.OwningColumn.HeaderText != "cantidad")
                            {
                                n.Style.BackColor = Color.Yellow;
                            }
                        }
                    }
                    else
                    {
                        foreach (DataGridViewCell n in x.Cells)
                        {
                            if (n.OwningColumn.HeaderText != "cantidad")
                            {
                                n.Style.BackColor = Color.LightGreen;
                            }
                        }
                    }
                }
            }
        }

        private string getCambios()
        {
            string r = string.Empty;
            foreach(DataGridViewRow x in datagridviewNE1.Rows)
            {
                if(x.Cells[4].Value.ToString() != "")
                {
                    if(r.Length > 0)
                    {
                        r = r + "," + x.Cells[0].Value + ":" + x.Cells[1].Value + ":" + x.Cells[3].Value + ":" + x.Cells[4].Value;
                    }
                    else
                    {
                        r = x.Cells[0].Value + ":" + x.Cells[1].Value + ":" + x.Cells[3].Value + ":" + x.Cells[4].Value;
                    }                   
                }
            }
            return r;
        }

        private string getNuevos()
        {
            string r = string.Empty;
            foreach (DataGridViewRow x in datagridviewNE1.Rows)
            {
                if (x.Cells[4].Value.ToString() == "")
                {
                    if (r.Length > 0)
                    {
                        r = r + "," + x.Cells[0].Value + ":" + x.Cells[1].Value + ":" + x.Cells[3].Value + ":" + x.Cells[4].Value;
                    }
                    else
                    {
                        r = x.Cells[0].Value + ":" + x.Cells[1].Value + ":" + x.Cells[3].Value + ":" + x.Cells[4].Value;
                    }
                }
            }
            return r;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["add_variaciones"] == null)
            {
                new add_variaciones().Show(this);
            }
            else
            {
                if (Application.OpenForms["add_variaciones"].WindowState == FormWindowState.Minimized)
                {
                    Application.OpenForms["add_variaciones"].WindowState = FormWindowState.Normal;
                }
                Application.OpenForms["add_variaciones"].Select();
            }
        }

        private void modificarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE2.RowCount > 0)
            {
                datagridviewNE1.Rows.Clear();
                int id = (int)datagridviewNE2.CurrentRow.Cells[0].Value;

                sqlDateBaseManager sql = new sqlDateBaseManager();

                List<string> list = sql.getVariacion(id);

                if (list.Count == 6)
                {
                    this.id = constants.stringToInt(list[0]);
                    textBox1.Text = list[1];
                    comboBox1.Text = list[2];
                    richTextBox1.Text = list[5];
                    string[] cambios = list[3].Split(',');
                    string[] s = null;
                    foreach (string x in cambios)
                    {
                        s = x.Split(':');
                        if (s.Length == 4)
                        {
                            setNewItem(constants.stringToInt(s[0]), s[1], s[3], constants.stringToFloat(s[2]));
                        }
                    }
                    string[] nuevos = list[4].Split(',');
                    foreach (string x in nuevos)
                    {
                        s = x.Split(':');
                        if (s.Length == 4)
                        {
                            setNewItem(constants.stringToInt(s[0]), s[1], s[3], constants.stringToFloat(s[2]));
                        }
                    }
                    tabControl1.SelectedTab = tabPage1;
                    textBox1.Enabled = false;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            reset();
        }
        ///------------------------------------------------------------------------>
    }
}
