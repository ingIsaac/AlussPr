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
    public partial class diseño : Form
    {
        List<string> esquemas = new List<string>();
        List<string> esquemas_diseño = new List<string>();
        List<string> config = new List<string>();

        public diseño()
        {
            InitializeComponent();
            progressBar1.Maximum = 100;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            treeView1.NodeMouseClick += TreeView1_NodeMouseClick;
            treeView1.AfterSelect += TreeView1_AfterSelect;
            checkBox1.Checked = true;
        }

        //esquema preview
        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count == 0)
            {
                if (char.IsDigit(e.Node.Text[0]) == true)
                {
                    constants.setImage("esquemas\\" + e.Node.Parent.Text, e.Node.Text, "png", pictureBox1);
                }
            }
        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Nodes.Count == 0)
            {
                if (char.IsDigit(e.Node.Text[0]) == true)
                {
                    constants.setImage("esquemas\\" + e.Node.Parent.Text, e.Node.Text, "png", pictureBox1);
                }
            }
        }
        //

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            label2.Visible = false;
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {           
            backgroundWorker1.ReportProgress(0);
            getEsquemas(0, "puertas");
            backgroundWorker1.ReportProgress(25);
            getEsquemas(1, "corredizas");
            backgroundWorker1.ReportProgress(50);
            getEsquemas(2, "ventanas_abatibles");
            backgroundWorker1.ReportProgress(75);
            getEsquemas(3, "ventanas_fijas");
            getEsquemas(4, "templados");
            getEsquemas(5, "otros");
            backgroundWorker1.ReportProgress(100);
        }

        private void diseño_Load(object sender, EventArgs e)
        {
            comboBox1.Text = "1";
            comboBox2.Text = "1";
            progressBar1.Visible = true;
            label2.Visible = true;
            hScrollBar1.Maximum = tableLayoutPanel1.Width;
            hScrollBar1.Value = tableLayoutPanel1.Width;
            vScrollBar1.Maximum = tableLayoutPanel1.Height;
            vScrollBar1.Value = tableLayoutPanel1.Height;
            backgroundWorker1.RunWorkerAsync();
        }

        //Esquemas
        private void getEsquemas(int tree_node, string type)
        {
            esquemas.Clear();
            try {
                esquemas = Directory.GetFiles(Application.StartupPath + "\\pics\\esquemas\\" + type).Select(Path.GetFileNameWithoutExtension).ToList();
                foreach (string x in esquemas)
                {
                    if(treeView1.InvokeRequired == true)
                    {
                        treeView1.Invoke((MethodInvoker)delegate
                        {
                            treeView1.Nodes[tree_node].Nodes.Add(x);
                        });
                    }
                    else
                    {
                        treeView1.Nodes[tree_node].Nodes.Add(x);
                    }
                }
            }catch(Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        //Reset_nodes
        private void resetEsquemas(int types)
        {
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add("puertas");
            treeView1.Nodes.Add("corredizas");
            treeView1.Nodes.Add("ventanas_abatibles");
            treeView1.Nodes.Add("ventanas_fijas");
            treeView1.Nodes.Add("templados");
            treeView1.Nodes.Add("otros");
        }

        //Secciones en columnas
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (constants.stringToInt(comboBox2.Text) >= tableLayoutPanel1.Controls.Count)
                {
                    tableLayoutPanel1.ColumnCount = constants.stringToInt(comboBox2.Text);
                    tableLayoutPanel1.ColumnStyles.Clear();
                    for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
                    {
                        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / tableLayoutPanel1.ColumnCount));
                    }
                }
                comboBox2.Text = tableLayoutPanel1.ColumnCount.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show(this, "[Error]: existe una mala conversión de esquemas.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Secciones en filas
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if ((constants.stringToInt(comboBox1.Text) * tableLayoutPanel1.ColumnCount) >= tableLayoutPanel1.Controls.Count)
                {
                    tableLayoutPanel1.RowCount = constants.stringToInt(comboBox1.Text);
                    tableLayoutPanel1.RowStyles.Clear();
                    for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
                    {
                        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100 / tableLayoutPanel1.RowCount));
                    }
                }
                comboBox1.Text = tableLayoutPanel1.RowCount.ToString();
            }
            catch (Exception)
            {
                MessageBox.Show(this, "[Error]: existe una mala conversión de esquemas.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Contar los controles incluidos
        private int getControlCount()
        {
            int r = 0;
            foreach(Control x in tableLayoutPanel1.Controls)
            {
                if (tableLayoutPanel1.GetColumnSpan(x) > 1) {
                    r = r + tableLayoutPanel1.GetColumnSpan(x);
                }
                else if(tableLayoutPanel1.GetRowSpan(x) > 1)
                {
                    r = r + tableLayoutPanel1.GetRowSpan(x);
                }
                else
                {
                    r++;
                }
            }
            return r;
        }

        //Agregar Esquema
        private void agregarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(treeView1.SelectedNode.Nodes.Count == 0)
            {
                try
                {
                    if (getControlCount() < (tableLayoutPanel1.ColumnCount * tableLayoutPanel1.RowCount))
                    {
                        PictureBox a1 = new PictureBox();
                        a1.Dock = DockStyle.Fill;
                        a1.InitialImage = Properties.Resources.loading_gif;
                        a1.SizeMode = PictureBoxSizeMode.StretchImage;
                        a1.Margin = new Padding(0, 0, 0, 0);
                        constants.setImage("esquemas\\" + treeView1.SelectedNode.Parent.Text, treeView1.SelectedNode.Text, "png", a1);
                        tableLayoutPanel1.Controls.Add(a1);                       
                        esquemas_diseño.Add(treeView1.SelectedNode.Text);
                        string[] esquema = constants.leerEsquema(treeView1.SelectedNode.Text);
                        int s = constants.stringToInt(esquema[0]);
                        if (s > 1)
                        {
                            if (esquema[2] == "V")
                            {
                                tableLayoutPanel1.SetRowSpan(a1, s);
                            }
                            else if (esquema[2] == "H")
                            {
                                tableLayoutPanel1.SetColumnSpan(a1, s);
                            }
                            else if (esquema[2] == "VH")
                            {
                                tableLayoutPanel1.SetRowSpan(a1, constants.stringToInt(esquema[3]));
                                tableLayoutPanel1.SetColumnSpan(a1, constants.stringToInt(esquema[4]));
                            }
                        }
                        if (esquema[1] != "" || esquema[1] != null)
                        {
                            config.Add(esquema[1]);
                        }
                        textBox1.Text = getConfig();
                    }
                    else
                    {
                        MessageBox.Show(this, "[Error]: no queda mas espacio dentro del panel.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch(Exception)
                {
                    tableLayoutPanel1.Controls.RemoveAt(tableLayoutPanel1.Controls.Count - 1);
                    esquemas_diseño.RemoveAt(tableLayoutPanel1.Controls.Count - 1);
                    MessageBox.Show(this, "[Error]: no queda mas espacio dentro del panel.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Eliminar Seccion
        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tableLayoutPanel1.Controls.Count > 0)
            {
                try
                {
                    esquemas_diseño.RemoveAt(tableLayoutPanel1.Controls.Count - 1);
                    config.RemoveAt(tableLayoutPanel1.Controls.Count - 1);
                    tableLayoutPanel1.Controls.RemoveAt(tableLayoutPanel1.Controls.Count - 1);
                }
                catch (Exception)
                {
                    MessageBox.Show(this, "[Error]: existe una mala conversión de esquemas.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                textBox1.Text = getConfig();
            }
        }

        //Eliminar todas las secciones
        private void button1_Click(object sender, EventArgs e)
        {
            tableLayoutPanel1.Controls.Clear();
            esquemas_diseño.Clear();
            config.Clear();
            textBox1.Text = getConfig();
        }   

        //Reload esquemas
        private void button2_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == false)
            {
                progressBar1.Visible = true;
                label2.Visible = true;
                resetEsquemas(treeView1.Nodes.Count);
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private string getConfig()
        {
            string r = string.Empty;
            foreach(string x in config)
            {
                r = r + x;
            }
            return r;
        }

        //Nuevo Diseño
        private void nuevoDiseño()
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if (textBox2.Text != "")
            {
                if (comboBox3.Text != "")
                {
                    string name = "(" + textBox1.Text + ") " + textBox2.Text;
                    if (sql.findSQLValue("nombre", "nombre", "esquemas", name) == false)
                    {
                        if (tableLayoutPanel1.Controls.Count > 0)
                        {
                            if (checkBox1.Checked == true)
                            {
                                sql.crearEsquema(name, constants.stringToInt(comboBox1.Text), constants.stringToInt(comboBox2.Text), textBox1.Text, capturarEsquemas(), true, comboBox3.Text);
                                MessageBox.Show(this, "Se ha creado un nuevo diseño.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                sql.crearEsquema(name, constants.stringToInt(comboBox1.Text), constants.stringToInt(comboBox2.Text), textBox1.Text, capturarEsquemas(), false, comboBox3.Text);
                                MessageBox.Show(this, "Se ha creado un nuevo diseño.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            MessageBox.Show(this, "[Error] el diseño no maneja esquemas.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        DialogResult r = MessageBox.Show(this, "Ya existe un diseño con ese nombre, ¿Desea continuar?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (r == DialogResult.Yes)
                        {
                            if (tableLayoutPanel1.Controls.Count > 0)
                            {
                                if (checkBox1.Checked == true)
                                {
                                    sql.crearEsquema(name, constants.stringToInt(comboBox1.Text), constants.stringToInt(comboBox2.Text), textBox1.Text, capturarEsquemas(), true, comboBox3.Text);
                                    MessageBox.Show(this, "Se ha creado un nuevo diseño.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    sql.crearEsquema(name, constants.stringToInt(comboBox1.Text), constants.stringToInt(comboBox2.Text), textBox1.Text, capturarEsquemas(), false, comboBox3.Text);
                                    MessageBox.Show(this, "Se ha creado un nuevo diseño.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            else
                            {
                                MessageBox.Show(this, "[Error] el diseño no maneja esquemas.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }                  
                }
                else
                {
                    MessageBox.Show(this, "[Error] necesitas asignarle un grupo al diseño.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "[Error] necesitas darle nombre a al diseño.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //capturar esquemas
        private string capturarEsquemas()
        {
            string r = string.Empty;
            foreach(string x in esquemas_diseño)
            {
                r = r + x + ",";
            }
            return r;
        }      

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (hScrollBar1.Value > (hScrollBar1.Maximum * 0.2))
            {
                tableLayoutPanel1.Width = hScrollBar1.Value;
            }
        }

        //Marco
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                if (comboBox3.Text == "puerta")
                {
                    tableLayoutPanel1.Padding = new Padding(10, 10, 10, 0);
                }
                else
                {
                    tableLayoutPanel1.Padding = new Padding(10, 10, 10, 10);
                }
            }
            else
            {
                tableLayoutPanel1.Padding = new Padding(0, 0, 0, 0);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            nuevoDiseño();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                if(comboBox3.Text == "puerta")
                {
                    tableLayoutPanel1.Padding = new Padding(10, 10, 10, 0);
                }
                else
                {
                    tableLayoutPanel1.Padding = new Padding(10, 10, 10, 10);
                }
            }
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (vScrollBar1.Value > (vScrollBar1.Maximum * 0.2))
            {
                tableLayoutPanel1.Height = vScrollBar1.Value;
            }
        }
    }
}