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
    public partial class diseño_p : Form
    {
        List<string> esquemas = new List<string>();
        List<string> esquemas_diseño = new List<string>();
        List<string> config = new List<string>();
        int id;
        string acabado = string.Empty;

        public diseño_p(int id)
        {            
            InitializeComponent();
            treeView1.NodeMouseClick += TreeView1_NodeMouseClick;
            treeView1.AfterSelect += TreeView1_AfterSelect;
            treeView2.NodeMouseClick += TreeView2_NodeMouseClick;
            treeView2.AfterSelect += TreeView2_AfterSelect;
            progressBar1.Maximum = 100;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            this.id = id;
            this.Shown += Diseño_p_Shown;
            cargarEsquemasInherentes();
            comboBox5.Enabled = false;
            comboBox6.Enabled = false;
            checkBox1.Checked = true;         
        }

        private void Diseño_p_Shown(object sender, EventArgs e)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            var data = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();
         
            if (data != null)
            {
                if (data.modulo_id != -2)
                {
                    comboBox4.Enabled = false;
                    acabado = data.acabado_perfil;
                }
                if (data.new_desing != "" && data.new_desing != null)
                {
                    getNewDesing(data.new_desing);
                }
            }
        }

        private void checkAcabado(string acabado)
        {
            if (acabado != "")
            {
                if (constants.imageExist("acabados_especiales", acabado, "jpg") == true)
                {
                    for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
                    {
                        constants.setBackgroundImg("acabados_especiales", acabado, "jpg", tableLayoutPanel1.Controls[i]);
                    }
                    constants.setBackgroundImg("acabados_especiales", acabado, "jpg", tableLayoutPanel1);
                }
                else
                {
                    for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
                    {
                        constants.setBackgroundImg("acabados_perfil", acabado, "jpg", tableLayoutPanel1.Controls[i]);
                    }
                    constants.setBackgroundImg("acabados_perfil", acabado, "jpg", tableLayoutPanel1);
                }
            }
        }

        //Esquemas inherentes
        private void TreeView2_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {         
            if (e.Node.Nodes.Count == 0)
            {
                if (char.IsDigit(e.Node.Text[0]) == true)
                {
                    if (File.Exists(constants.folder_resources_dir + "esquemas\\corredizas\\" + e.Node.Text + ".png"))
                    {
                        constants.setImage("esquemas\\corredizas\\", e.Node.Text, "png", pictureBox1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\puertas\\" + e.Node.Text + ".png"))
                    {
                        constants.setImage("esquemas\\puertas\\", e.Node.Text, "png", pictureBox1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_abatibles\\" + e.Node.Text + ".png"))
                    {
                        constants.setImage("esquemas\\ventanas_abatibles\\", e.Node.Text, "png", pictureBox1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_fijas\\" + e.Node.Text + ".png"))
                    {
                        constants.setImage("esquemas\\ventanas_fijas\\", e.Node.Text, "png", pictureBox1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\templados\\" + e.Node.Text + ".png"))
                    {
                        constants.setImage("esquemas\\templados\\", e.Node.Text, "png", pictureBox1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\otros\\" + e.Node.Text + ".png"))
                    {
                        constants.setImage("esquemas\\otros\\", e.Node.Text, "png", pictureBox1);
                    }
                }
            }        
        }

        private void TreeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count == 0)
            {
                if (char.IsDigit(e.Node.Text[0]) == true)
                {
                    if (File.Exists(constants.folder_resources_dir + "esquemas\\corredizas\\" + e.Node.Text + ".png"))
                    {
                        constants.setImage("esquemas\\corredizas\\", e.Node.Text, "png", pictureBox1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\puertas\\" + e.Node.Text + ".png"))
                    {
                        constants.setImage("esquemas\\puertas\\", e.Node.Text, "png", pictureBox1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_abatibles\\" + e.Node.Text + ".png"))
                    {
                        constants.setImage("esquemas\\ventanas_abatibles\\", e.Node.Text, "png", pictureBox1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_fijas\\" + e.Node.Text + ".png"))
                    {
                        constants.setImage("esquemas\\ventanas_fijas\\", e.Node.Text, "png", pictureBox1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\templados\\" + e.Node.Text + ".png"))
                    {
                        constants.setImage("esquemas\\templados\\", e.Node.Text, "png", pictureBox1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\otros\\" + e.Node.Text + ".png"))
                    {
                        constants.setImage("esquemas\\otros\\", e.Node.Text, "png", pictureBox1);
                    }
                }
            }
        }
        //

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        //Esquemas generales
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
        //

        //Cargar esquemas inherentes
        private void cargarEsquemasInherentes()
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            var modulo_c = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == id select x);

            if(modulo_c != null)
            {
                foreach (var n in modulo_c)
                {
                    listas_entities_pva listas = new listas_entities_pva();
                    int modulo_id = (int)n.modulo_id;
                    var modulo = (from x in listas.modulos where x.id == modulo_id select x).SingleOrDefault();
                    if (modulo != null)
                    {
                        int id_diseño = (int)modulo.id_diseño;
                        var diseño = (from x in listas.esquemas where x.id == id_diseño select x).SingleOrDefault();
                        if (diseño != null)
                        {
                            string[] e = diseño.esquemas.ToString().Split(',');
                            foreach (string v in e)
                            {
                                if (v != "")
                                {
                                    treeView2.Nodes[0].Nodes.Add(v);
                                }                              
                            }
                        }
                    }                
                }
            }
        }       

        //Esquemas
        private void getEsquemas(int tree_node, string type)
        {
            esquemas.Clear();
            try
            {
                esquemas = Directory.GetFiles(Application.StartupPath + "\\pics\\esquemas\\" + type).Select(Path.GetFileNameWithoutExtension).ToList();
                foreach (string x in esquemas)
                {
                    if (treeView1.InvokeRequired == true)
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
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        //Cargar esquemas
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

        //Cargar esquema -boton
        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == false)
            {
                progressBar1.Visible = true;
                resetEsquemas(treeView1.Nodes.Count);
                backgroundWorker1.RunWorkerAsync();
            }
        }

        //Agregar esquema
        private void agregarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Nodes.Count == 0)
            {
                try
                {
                    if (tableLayoutPanel1.Controls.Count < (tableLayoutPanel1.ColumnCount * tableLayoutPanel1.RowCount))
                    {
                        PictureBox a1 = new PictureBox();
                        a1.Dock = DockStyle.Fill;
                        a1.InitialImage = Properties.Resources.loading_gif;
                        a1.SizeMode = PictureBoxSizeMode.StretchImage;
                        a1.BackgroundImageLayout = ImageLayout.Stretch;
                        a1.Margin = new Padding(0, 0, 0, 0);
                        constants.setImage("esquemas\\" + treeView1.SelectedNode.Parent.Text, treeView1.SelectedNode.Text, "png", a1);
                        if (tableLayoutPanel1.BackgroundImage != null)
                        {
                            a1.BackgroundImage = tableLayoutPanel1.BackgroundImage;
                        }
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
                        checkAcabado(acabado);
                    }
                    else
                    {
                        MessageBox.Show("[Error]: no queda mas espacio dentro del panel.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception)
                {
                    esquemas_diseño.RemoveAt(tableLayoutPanel1.Controls.Count - 1);
                    tableLayoutPanel1.Controls.RemoveAt(tableLayoutPanel1.Controls.Count - 1);
                    MessageBox.Show("[Error]: no queda mas espacio dentro del panel.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Columns
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (constants.stringToInt(comboBox1.Text) >= tableLayoutPanel1.Controls.Count)
            {
                tableLayoutPanel1.ColumnCount = constants.stringToInt(comboBox1.Text);
                tableLayoutPanel1.ColumnStyles.Clear();
                for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
                {
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / tableLayoutPanel1.ColumnCount));
                }
            }
            comboBox1.Text = tableLayoutPanel1.ColumnCount.ToString();
            comboBox6.Items.Clear();
            for (int i = 0; i < tableLayoutPanel1.ColumnStyles.Count-1; i++)
            {
                comboBox6.Items.Add(i);
            }
            comboBox5.Text = "0";
            comboBox6.Text = "0";
        }

        //Filas
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((constants.stringToInt(comboBox2.Text) * tableLayoutPanel1.ColumnCount) >= tableLayoutPanel1.Controls.Count)
            {
                tableLayoutPanel1.RowCount = constants.stringToInt(comboBox2.Text);
                tableLayoutPanel1.RowStyles.Clear();
                for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
                {
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100 / tableLayoutPanel1.RowCount));
                }
            }
            comboBox2.Text = tableLayoutPanel1.RowCount.ToString();
            comboBox5.Items.Clear();
            for(int i = 0; i < tableLayoutPanel1.RowStyles.Count-1; i++)
            {
                comboBox5.Items.Add(i);
            }           
            comboBox5.Text = "0";
            comboBox6.Text = "0";
        }

        //Eliminar esquema
        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tableLayoutPanel1.Controls.Count > 0)
            {
                esquemas_diseño.RemoveAt(tableLayoutPanel1.Controls.Count - 1);
                config.RemoveAt(tableLayoutPanel1.Controls.Count - 1);
                tableLayoutPanel1.Controls.RemoveAt(tableLayoutPanel1.Controls.Count - 1);
            }
        }

        //Eliminar todo
        private void button2_Click(object sender, EventArgs e)
        {
            tableLayoutPanel1.Controls.Clear();
            esquemas_diseño.Clear();
            config.Clear();
            tableLayoutPanel1.BackgroundImage = null;
        }

        //Marco
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
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

        //Grupos
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
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
        }

        //set width to control
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (checkBox2.Checked == false)
            {
                if (hScrollBar1.Value > (hScrollBar1.Maximum * 0.2))
                {
                    tableLayoutPanel1.Width = hScrollBar1.Value;
                }
            }
            else
            {
                tableLayoutPanel1.ColumnStyles[constants.stringToInt(comboBox6.Text)].Width = hScrollBar1.Value;
                tableLayoutPanel1.ColumnStyles[tableLayoutPanel1.ColumnCount-1].Width = getOtherColumns();
                tableLayoutPanel1.Refresh();
            }
            
        }

        private float getOtherColumns()
        {
            float r = 0;
            for(int i=0; i < tableLayoutPanel1.ColumnStyles.Count - 1; i++)
            {
                r = tableLayoutPanel1.ColumnStyles[i].Width + r;
            }
            r = tableLayoutPanel1.Width - r;
            r = r - 20;
            return r < 1 ? 1 : r;
        }

        private float getOtherRows()
        {
            float r = 0;
            for (int i = 0; i < tableLayoutPanel1.RowStyles.Count - 1; i++)
            {
                r = tableLayoutPanel1.RowStyles[i].Height + r;
            }
            r = tableLayoutPanel1.Height - r;
            r = r - 20;
            return r < 1 ? 1 : r;
        }

        //set height to control
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (checkBox2.Checked == false)
            {
                if (vScrollBar1.Value > (vScrollBar1.Maximum * 0.2))
                {
                    tableLayoutPanel1.Height = vScrollBar1.Value;
                }
            }
            else
            {
                tableLayoutPanel1.RowStyles[constants.stringToInt(comboBox5.Text)].Height = vScrollBar1.Value;
                tableLayoutPanel1.RowStyles[tableLayoutPanel1.RowCount-1].Height = getOtherRows();
                tableLayoutPanel1.Refresh();
            }
        }

        private void diseño_p_Load(object sender, EventArgs e)
        {
            comboBox1.Text = "1";
            comboBox2.Text = "1";
            progressBar1.Visible = true;
            hScrollBar1.Maximum = tableLayoutPanel1.Width;
            hScrollBar1.Value = tableLayoutPanel1.Width;
            vScrollBar1.Maximum = tableLayoutPanel1.Height;
            vScrollBar1.Value = tableLayoutPanel1.Height;
            loadColoresAluminio();
            backgroundWorker1.RunWorkerAsync();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            var modulos = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();

            if(modulos != null)
            {
                Bitmap bm = new Bitmap(tableLayoutPanel1.Width, tableLayoutPanel1.Height);
                tableLayoutPanel1.DrawToBitmap(bm, new Rectangle(0, 0, tableLayoutPanel1.Width, tableLayoutPanel1.Height));
                Bitmap gm_2 = new Bitmap(bm, 120, 105);
                modulos.pic = constants.imageToByte(gm_2);
                modulos.dir = 3;
                bm = null;
                gm_2 = null;
                modulos.new_desing = setNewDesing();
                cotizaciones.SaveChanges();
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).reloadModulos();
                }
                if (Application.OpenForms["edit_expresss"] != null)
                {
                    ((edit_expresss)Application.OpenForms["edit_expresss"]).reloadALL();
                }
                this.Close();
            }
        }

        private string setNewDesing()
        {
            string r = string.Empty;
            r = tableLayoutPanel1.RowStyles.Count + ":" + tableLayoutPanel1.ColumnStyles.Count + ",";
            string c = string.Empty;
            foreach(ColumnStyle x in tableLayoutPanel1.ColumnStyles)
            {
                if (c.Length > 0)
                {
                    c = c + ":" + x.Width;
                }
                else
                {
                    c = x.Width.ToString();
                }
            }
            r = r + c + ",";
            string f = string.Empty;
            foreach (RowStyle x in tableLayoutPanel1.RowStyles)
            {
                if (f.Length > 0)
                {
                    f = f + ":" + x.Height;
                }
                else
                {
                    f = x.Height.ToString();
                }
            }
            r = r + f + ",";
            string i = string.Empty;
            foreach(string x in esquemas_diseño)
            {
                if(i.Length > 0)
                {
                    i = i + ":" + x;
                }
                else
                {
                    i = x;
                }
            }
            r = r + i + ",";
            r = r + (checkBox1.Checked == true ? "t" : "f") + ":";
            r = r + (checkBox2.Checked == true ? "t" : "f") + ":";
            r = r + comboBox3.Text;
            return r;
        }

        private void getNewDesing(string desing)
        {
            string[] r = desing.Split(',');
            if (r.Length == 5)
            {
                string[] n = r[0].Split(':');
                comboBox2.Text = n[0];
                comboBox1.Text = n[1];               
                string[] es = r[3].Split(':');
                esquemas_diseño.Clear();
                foreach (string x in es)
                {
                    esquemas_diseño.Add(x);
                }
                string[] a = r[4].Split(':');
                if (a.Length == 3)
                {
                    checkBox1.Checked = a[0] == "t" ? true : false;
                    checkBox2.Checked = a[1] == "t" ? true : false;
                    comboBox3.Text = a[2];
                }
                string[] c = r[1].Split(':');
                for (int i = 0; i < tableLayoutPanel1.ColumnStyles.Count; i++)
                {
                    tableLayoutPanel1.ColumnStyles[i].Width = constants.stringToInt(c[i]);
                }
                string[] f = r[2].Split(':');
                for (int i = 0; i < tableLayoutPanel1.RowStyles.Count; i++)
                {
                    tableLayoutPanel1.RowStyles[i].Height = constants.stringToInt(f[i]);
                }
                int v = 0;
                foreach (string e in esquemas_diseño)
                {
                    if (File.Exists(constants.folder_resources_dir + "esquemas\\corredizas\\" + e + ".png"))
                    {
                        constants.loadDiseño("esquemas\\corredizas\\", e, tableLayoutPanel1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\puertas\\" + e + ".png"))
                    {
                        constants.loadDiseño("esquemas\\puertas\\", e, tableLayoutPanel1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_abatibles\\" + e + ".png"))
                    {
                        constants.loadDiseño("esquemas\\ventanas_abatibles\\", e, tableLayoutPanel1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_fijas\\" + e + ".png"))
                    {
                        constants.loadDiseño("esquemas\\ventanas_fijas\\", e, tableLayoutPanel1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\templados\\" + e + ".png"))
                    {
                        constants.loadDiseño("esquemas\\templados\\", e, tableLayoutPanel1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\otros\\" + e + ".png"))
                    {
                        constants.loadDiseño("esquemas\\otros\\", e, tableLayoutPanel1);
                    }
                    string[] esquema = constants.leerEsquema(e);
                    int s = constants.stringToInt(esquema[0]);
                    if (s > 1)
                    {
                        if (esquema[2] == "V")
                        {
                            tableLayoutPanel1.SetRowSpan(tableLayoutPanel1.Controls[v], s);
                        }
                        else if (esquema[2] == "H")
                        {
                            tableLayoutPanel1.SetColumnSpan(tableLayoutPanel1.Controls[v], s);
                        }
                        else if (esquema[2] == "VH")
                        {
                            tableLayoutPanel1.SetRowSpan(tableLayoutPanel1.Controls[v], constants.stringToInt(esquema[3]));
                            tableLayoutPanel1.SetColumnSpan(tableLayoutPanel1.Controls[v], constants.stringToInt(esquema[4]));
                        }
                    }
                    if (esquema[1] != "" || esquema[1] != null)
                    {
                        config.Add(esquema[1]);
                    }
                    v++;
                }            
                checkAcabado(acabado);
            }
        }

        //Acabado
        private void loadColoresAluminio()
        {
            listas_entities_pva listas = new listas_entities_pva();

            var colores = from x in listas.colores_aluminio select x;

            if (colores != null)
            {
                comboBox4.Items.Clear();
                foreach (var c in colores)
                {
                    comboBox4.Items.Add(c.clave);
                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tableLayoutPanel1.Controls.Count > 0 && comboBox4.SelectedIndex >= 0)
            {
                acabado = string.Empty;
                for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
                {
                    constants.setBackgroundImg("acabados_especiales", comboBox4.Text, "jpg", tableLayoutPanel1.Controls[i]);
                }
                constants.setBackgroundImg("acabados_especiales", comboBox4.Text, "jpg", tableLayoutPanel1);
            }
            else
            {
                comboBox4.SelectedIndex = -1;
            }
        }
        //

        //Agregar esquema inherente
        private void agregarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeView2.SelectedNode.Nodes.Count == 0)
            {
                try
                {
                    if (tableLayoutPanel1.Controls.Count < (tableLayoutPanel1.ColumnCount * tableLayoutPanel1.RowCount))
                    {
                        PictureBox a1 = new PictureBox();
                        a1.Dock = DockStyle.Fill;
                        a1.InitialImage = Properties.Resources.loading_gif;
                        a1.SizeMode = PictureBoxSizeMode.StretchImage;
                        a1.BackgroundImageLayout = ImageLayout.Stretch;
                        a1.Margin = new Padding(0, 0, 0, 0);
                        if (File.Exists(constants.folder_resources_dir + "esquemas\\corredizas\\" + treeView2.SelectedNode.Text + ".png"))
                        {
                            constants.setImage("esquemas\\corredizas\\", treeView2.SelectedNode.Text, "png", a1);
                        }
                        else if (File.Exists(constants.folder_resources_dir + "esquemas\\puertas\\" + treeView2.SelectedNode.Text + ".png"))
                        {
                            constants.setImage("esquemas\\puertas\\", treeView2.SelectedNode.Text, "png", a1);
                        }
                        else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_abatibles\\" + treeView2.SelectedNode.Text + ".png"))
                        {
                            constants.setImage("esquemas\\ventanas_abatibles\\", treeView2.SelectedNode.Text, "png", a1);
                        }
                        else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_fijas\\" + treeView2.SelectedNode.Text + ".png"))
                        {
                            constants.setImage("esquemas\\ventanas_fijas\\", treeView2.SelectedNode.Text, "png", a1);
                        }
                        else if (File.Exists(constants.folder_resources_dir + "esquemas\\templados\\" + treeView2.SelectedNode.Text + ".png"))
                        {
                            constants.setImage("esquemas\\templados\\", treeView2.SelectedNode.Text, "png", pictureBox1);
                        }
                        else if (File.Exists(constants.folder_resources_dir + "esquemas\\otros\\" + treeView2.SelectedNode.Text + ".png"))
                        {
                            constants.setImage("esquemas\\otros\\", treeView2.SelectedNode.Text, "png", pictureBox1);
                        }
                        if (tableLayoutPanel1.BackgroundImage != null)
                        {
                            a1.BackgroundImage = tableLayoutPanel1.BackgroundImage;
                        }
                        tableLayoutPanel1.Controls.Add(a1);
                        esquemas_diseño.Add(treeView2.SelectedNode.Text);
                        string[] esquema = constants.leerEsquema(treeView2.SelectedNode.Text);
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
                        checkAcabado(acabado);
                    }
                    else
                    {
                        MessageBox.Show("[Error]: no queda mas espacio dentro del panel.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception)
                {
                    esquemas_diseño.RemoveAt(tableLayoutPanel1.Controls.Count - 1);
                    tableLayoutPanel1.Controls.RemoveAt(tableLayoutPanel1.Controls.Count - 1);
                    MessageBox.Show("[Error]: no queda mas espacio dentro del panel.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void changeStyleRow(SizeType size, float num)
        {          
            if (tableLayoutPanel1.RowCount > 0)
            {
                tableLayoutPanel1.RowStyles.Clear();
                for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
                {
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(size, num));                  
                }                
            }
        }

        private void changeStyleColumn(SizeType size, float num)
        {
            if (tableLayoutPanel1.ColumnCount > 0)
            {
                tableLayoutPanel1.ColumnStyles.Clear();
                for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
                {
                    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(size, num));                  
                }
            }          
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                comboBox5.Enabled = true;
                comboBox6.Enabled = true;
                comboBox5.Text = "0";
                comboBox6.Text = "0";
                changeStyleRow(SizeType.Absolute, (tableLayoutPanel1.Height - 20) / tableLayoutPanel1.RowCount);
                changeStyleColumn(SizeType.Absolute, (tableLayoutPanel1.Width - 20) / tableLayoutPanel1.ColumnCount);
            }
            else
            {
                comboBox5.Enabled = false;
                comboBox6.Enabled = false;
                changeStyleRow(SizeType.Percent, 100 / tableLayoutPanel1.RowCount);
                changeStyleColumn(SizeType.Percent, 100 / tableLayoutPanel1.ColumnCount);
            }
        }
    }
}
