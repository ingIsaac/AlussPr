using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace cristales_pva
{
    public partial class crear_modulo : Form
    {
        private int module_id = 0;
        cotizaciones_local cotizaciones = new cotizaciones_local();
        localDateBaseEntities3 articulos = new localDateBaseEntities3();
        List<string> items = new List<string>();
        List<string> esquemas = new List<string>();
        string autor = string.Empty;
        int errors = 0;

        public crear_modulo(int module_id)
        {
            InitializeComponent();
            dataGridView1.CellClick += DataGridView1_CellClick1;
            dataGridView1.CellLeave += DataGridView1_CellLeave;
            datagridviewNE1.CellClick += DatagridviewNE1_CellClick;
            dataGridView6.CellEndEdit += DataGridView6_CellEndEdit;
            dataGridView6.CellClick += DataGridView6_CellClick1;
            dataGridView6.CellLeave += DataGridView6_CellLeave;
            dataGridView6.DataError += DataGridView6_DataError;
            dataGridView2.CellClick += DataGridView2_CellClick;
            treeView1.NodeMouseClick += TreeView1_NodeMouseClick;
            treeView1.AfterSelect += TreeView1_AfterSelect;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            backgroundWorker2.RunWorkerCompleted += BackgroundWorker2_RunWorkerCompleted;
            backgroundWorker3.WorkerReportsProgress = true;
            backgroundWorker3.ProgressChanged += BackgroundWorker3_ProgressChanged;
            backgroundWorker3.RunWorkerCompleted += BackgroundWorker3_RunWorkerCompleted;
            checkBox3.Click += CheckBox3_Click;
            checkBox4.Click += CheckBox4_Click;
            textBox5.KeyDown += TextBox5_KeyDown;
            this.module_id = module_id;
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
            contextMenuStrip2.Opening += ContextMenuStrip2_Opening;
            contextMenuStrip3.Opening += ContextMenuStrip3_Opening;
            contextMenuStrip4.Opening += ContextMenuStrip4_Opening;
            contextMenuStrip5.Opening += ContextMenuStrip5_Opening;
            contextMenuStrip6.Opening += ContextMenuStrip6_Opening;
            loadAll();
        }

        private void ContextMenuStrip6_Opening(object sender, CancelEventArgs e)
        {
            if(dataGridView6.RowCount == 0)
            {
                e.Cancel = true;
            }
        }

        private void ContextMenuStrip5_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridView2.RowCount == 0)
            {
                e.Cancel = true;
            }
        }

        private void ContextMenuStrip4_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridView6.RowCount == 0)
            {
                e.Cancel = true;
            }
        }

        private void ContextMenuStrip3_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridView1.RowCount == 0)
            {
                e.Cancel = true;
            }
        }

        private void ContextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if(dataGridView4.RowCount == 0)
            {
                e.Cancel = true;
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if(datagridviewNE1.RowCount == 0)
            {
                e.Cancel = true;
            }
        }

        private void TextBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                listas_entities_pva listas = new listas_entities_pva();
                switch (comboBox3.SelectedIndex)
                {
                    case 0:
                        var costo_corte = from x in listas.lista_costo_corte_e_instalado
                                          where x.articulo.StartsWith(textBox5.Text) || x.clave.StartsWith(textBox5.Text)
                                          orderby x.articulo ascending
                                          select new
                                          {
                                              Clave = x.clave,
                                              Artículo = x.articulo,
                                              Proveedor = x.proveedor,
                                              Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                              Costo_Instalado = "$" + x.costo_instalado
                                          };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = costo_corte.ToList();
                        break;
                    case 1:
                        var perfiles = from x in listas.perfiles
                                       where x.articulo.StartsWith(textBox5.Text) || x.clave.StartsWith(textBox5.Text)
                                       orderby x.articulo ascending
                                       select new
                                       {
                                           Id = x.id,
                                           Clave = x.clave,
                                           Artículo = x.articulo,
                                           Linea = x.linea,
                                           Proveedor = x.proveedor,
                                           Largo = x.largo + " m",
                                           Crudo = "$" + x.crudo,
                                           Blanco = "$" + x.blanco,
                                           Hueso = "$" + x.hueso,
                                           Champagne = "$" + x.champagne,
                                           Gris = "$" + x.gris,
                                           Negro = "$" + x.negro,
                                           Brillante = "$" + x.brillante,
                                           Natural = "$" + x.natural_1,
                                           Madera = "$" + x.madera,
                                           Chocolate = "$" + x.chocolate,
                                           Acero_Inox = "$" + x.acero_inox,
                                           Bronce = "$" + x.bronce
                                       };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = perfiles.ToList();
                        break;
                    case 2:
                        var herrajes = from x in listas.herrajes
                                       where x.articulo.StartsWith(textBox5.Text) || x.clave.StartsWith(textBox5.Text)
                                       orderby x.articulo ascending
                                       select new
                                       {
                                           Id = x.id,
                                           Clave = x.clave,
                                           Artículo = x.articulo,
                                           Linea = x.linea,
                                           Proveedor = x.proveedor,
                                           Caracteristicas = x.caracteristicas,
                                           Color = x.color,
                                           Precio = "$" + x.precio
                                       };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = herrajes.ToList();
                        break;
                    case 3:
                        var otros = from x in listas.otros
                                    where x.articulo.StartsWith(textBox5.Text) || x.clave.StartsWith(textBox5.Text)
                                    orderby x.articulo ascending
                                    select new
                                    {
                                        Id = x.id,
                                        Clave = x.clave,
                                        Artículo = x.articulo,
                                        Linea = x.linea,
                                        Proveedor = x.proveedor,
                                        Caracteristicas = x.caracteristicas,
                                        Color = x.color,
                                        Precio = "$" + x.precio
                                    };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = otros.ToList();
                        break;
                    default: break;
                }
                listas.Dispose();
            }
        }

        private void DatagridviewNE1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox12.Text = "";
            int id = constants.stringToInt(datagridviewNE1.CurrentRow.Cells[11].Value.ToString());
            if (id > 0)
            {
                LeerDiseño(id);
            }
        }

        private void CheckBox3_Click(object sender, EventArgs e)
        {
            if(autor != constants.user)
            {
                if(checkBox3.Checked == true)
                {
                    checkBox3.Checked = false;
                }
                else
                {
                    checkBox3.Checked = true;
                }
                MessageBox.Show("[Error] solo el autor del módulo puede conceder o restringir el acceso.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if(checkBox3.Checked == true)
                {
                    pictureBox3.Image = Properties.Resources.Lock_Lock_icon;
                }
                else
                {
                    pictureBox3.Image = null;
                }
            }
        }

        private void DataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            LeerDiseño(constants.stringToInt(dataGridView2.CurrentRow.Cells[0].Value.ToString()));
        }

        private void DataGridView6_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
           if(e.Exception is ArgumentException)
            {
                MessageBox.Show("[Error] sección no válida.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView6[e.ColumnIndex, e.RowIndex].Value = "";
            }
        }

        private void DataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
        }

        private void DataGridView1_CellClick1(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
        }

        //componentes color
        private void DataGridView6_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView6.RowCount > 0)
            {           
                int seccion = getSeccion(dataGridView6.CurrentRow.Cells[6].Value.ToString());
                if (seccion >= 0 && seccion <= tableLayoutPanel1.Controls.Count)
                {
                    if (seccion == 0)
                    {
                        tableLayoutPanel1.BackColor = Color.Red;
                    }
                    else
                    {
                        tableLayoutPanel1.Controls[seccion - 1].BackColor = Color.Red;
                    }
                }             
            }
        }

        private void DataGridView6_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView6.RowCount > 0)
            {             
                int seccion = getSeccion(dataGridView6.CurrentRow.Cells[6].Value.ToString());
                if (seccion >= 0 && seccion <= tableLayoutPanel1.Controls.Count)
                {
                    if (seccion == 0)
                    {
                        tableLayoutPanel1.BackColor = Color.LightBlue;
                    }
                    else
                    {
                        tableLayoutPanel1.Controls[seccion - 1].BackColor = Color.LightBlue;
                    }
                }
                else
                {
                    tableLayoutPanel1.BackColor = Color.LightBlue;
                    for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
                    {
                        tableLayoutPanel1.Controls[i].BackColor = Color.LightBlue;
                    }
                }            
            }
        }
        //

        //select esquema
        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Node.Nodes.Count == 0)
            {
                int id = getIdFromDiseño(e.Node.Text);
                if (id > 0)
                {
                    textBox12.Text = e.Node.Text;                  
                    LeerDiseño(getIdFromDiseño(e.Node.Text));
                }
            }
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count == 0)
            {
                int id = getIdFromDiseño(e.Node.Text);
                if (id > 0)
                {
                    textBox12.Text = e.Node.Text;
                    LeerDiseño(getIdFromDiseño(e.Node.Text));
                }
            }
        }
        //

        public void loadAll()
        {
            cargarEsquemas();
            setLines();
            foreach (DataGridViewColumn x in dataGridView6.Columns)
            {
                if (x.ReadOnly == true)
                {
                    x.DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
            clearModulos();
            if (module_id != 0)
            {
                modificar(module_id);
            }
        }

        private void countItems()
        {
            richTextBox2.Clear();
            float cristales = 0;
            float perfiles = 0;
            float herrajes = 0;
            float otros = 0;
            foreach(DataGridViewRow x in dataGridView6.Rows)
            {
                if(x.Cells[0].Value.ToString() == "Cristal")
                {
                    cristales = cristales + constants.stringToFloat(x.Cells[4].Value.ToString());
                }
                else if(x.Cells[0].Value.ToString() == "Perfil")
                {
                    perfiles = perfiles + constants.stringToFloat(x.Cells[4].Value.ToString());
                }
                else if (x.Cells[0].Value.ToString() == "Herraje")
                {
                    herrajes = herrajes + constants.stringToFloat(x.Cells[4].Value.ToString());
                }
                else if (x.Cells[0].Value.ToString() == "Otros")
                {
                    otros = otros + constants.stringToFloat(x.Cells[4].Value.ToString());
                }
            }
            richTextBox2.Text = "-Cristales: " + cristales + "\n -Perfiles: " + perfiles + "\n -Herrajes: " + herrajes + "\n -Otros Materiales: " + otros;
            label17.Text = (otros + cristales + perfiles + herrajes).ToString();
        }

        private void DataGridView6_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView6[e.ColumnIndex, e.RowIndex].Value != null)
            {
                setColors();
                countItems();
            }
            else
            {
                dataGridView6[e.ColumnIndex, e.RowIndex].Value = "";
                setColors();
                countItems();
            }
        }

        private void agregarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (comboBox3.SelectedIndex == 0)
                {
                    dataGridView6.Rows.Add("Cristal", "", dataGridView1.CurrentRow.Cells[0].Value, dataGridView1.CurrentRow.Cells[1].Value, 1, "", checkBox2.Checked ? "" : "0");
                }
                else if (comboBox3.SelectedIndex == 1)
                {
                    dataGridView6.Rows.Add("Perfil", dataGridView1.CurrentRow.Cells[0].Value, dataGridView1.CurrentRow.Cells[1].Value, dataGridView1.CurrentRow.Cells[2].Value, 1, "", checkBox2.Checked ? "" : "0");
                }
                else if (comboBox3.SelectedIndex == 2)
                {
                    dataGridView6.Rows.Add("Herraje", dataGridView1.CurrentRow.Cells[0].Value, dataGridView1.CurrentRow.Cells[1].Value, dataGridView1.CurrentRow.Cells[2].Value, 1, "", checkBox2.Checked ? "" : "0");
                }
                else if (comboBox3.SelectedIndex == 3)
                {
                    dataGridView6.Rows.Add("Otros", dataGridView1.CurrentRow.Cells[0].Value, dataGridView1.CurrentRow.Cells[1].Value, dataGridView1.CurrentRow.Cells[2].Value, 1, "", checkBox2.Checked ? "" : "0");
                }
                countItems();
                tabControl1.SelectedTab = tabPage4;
                dataGridView6.FirstDisplayedScrollingRowIndex = dataGridView6.Rows[dataGridView6.RowCount - 1].Index;
            }
        }
        //------------------------------------------------------------

        private void eliminarToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (dataGridView6.RowCount > 0)
            {
                dataGridView6.Rows.RemoveAt(dataGridView6.CurrentRow.Index);
                dataGridView6.Refresh();
                countItems();
                tableLayoutPanel1.BackColor = Color.LightBlue;
                for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
                {
                    tableLayoutPanel1.Controls[i].BackColor = Color.LightBlue;
                }
            }
        }
        //-----------------------------------------------------------

        //atributos de componentes
        private void DataGridView6_CellClick1(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView6.RowCount > 0)
            {
                if (dataGridView6.CurrentCell.OwningColumn.HeaderText == "Ubicación")
                {
                    if (dataGridView6.CurrentRow.Cells[0].Value.ToString() == "Perfil")
                    {                       
                        //---> importante
                        if (dataGridView6.CurrentCell.Value == null)
                        {
                            dataGridView6.CurrentCell.Value = "";
                        }
                        //
                        dataGridView6.CurrentCell.ReadOnly = false;
                        DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                        string u = string.Empty;
                        cell.Items.Clear();
                        cell.Items.AddRange("largo", "alto");
                        foreach(string x in cell.Items)
                        {
                            if(x == dataGridView6.CurrentCell.Value.ToString())
                            {
                                u = dataGridView6.CurrentCell.Value.ToString();
                            }
                        }
                        if (u == string.Empty)
                        {
                            dataGridView6.CurrentCell.Value = "";
                        }
                        cell.Value = u;
                        dataGridView6.CurrentRow.Cells[dataGridView6.CurrentCell.ColumnIndex] = cell;
                        cell.Dispose();
                    }
                    else if(dataGridView6.CurrentRow.Cells[0].Value.ToString() == "Otros")
                    {
                        listas_entities_pva listas = new listas_entities_pva();
                        int id = constants.stringToInt(dataGridView6.CurrentRow.Cells[1].Value.ToString());
                        var otros = (from x in listas.otros where x.id == id select x).SingleOrDefault();

                        if (otros != null)
                        {
                            if ((otros.largo > 0 && otros.alto == 0) || (otros.largo == 0 && otros.alto > 0))
                            {
                                //---> importante
                                if (dataGridView6.CurrentCell.Value == null)
                                {
                                    dataGridView6.CurrentCell.Value = "";
                                }
                                //
                                dataGridView6.CurrentCell.ReadOnly = false;
                                DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                                string u = string.Empty;
                                cell.Items.Clear();
                                cell.Items.AddRange("largo", "alto");
                                foreach (string x in cell.Items)
                                {
                                    if (x == dataGridView6.CurrentCell.Value.ToString())
                                    {
                                        u = dataGridView6.CurrentCell.Value.ToString();
                                    }
                                }
                                if (u == string.Empty)
                                {
                                    dataGridView6.CurrentCell.Value = "";
                                }
                                cell.Value = u;
                                dataGridView6.CurrentRow.Cells[dataGridView6.CurrentCell.ColumnIndex] = cell;
                                cell.Dispose();
                            }                           
                            else
                            {
                                dataGridView6.CurrentCell.ReadOnly = true;
                            }
                        }
                    }
                    else
                    {
                        dataGridView6.CurrentCell.ReadOnly = true;
                    }
                }
                else if(dataGridView6.CurrentCell.OwningColumn.HeaderText == "Sección")
                {
                    if (tableLayoutPanel1.Controls.Count > 0)
                    {
                        setSecciones(dataGridView6.CurrentCell, dataGridView6.CurrentRow);
                    }
                    else
                    {
                        MessageBox.Show("[Error] necesitas incluir un diseño.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                setImageArticulo();                               
            }                          
        }
        //

        private void setSecciones(DataGridViewCell d_cell, DataGridViewRow d_row)
        {
            items.Clear();
            if (checkBox1.Checked || !checkBox2.Checked)
            {
                items.Add("0");
            }
            for (int i = 1; i <= constants.stringToInt(textBox6.Text); i++)
            {
                items.Add(i.ToString());
            }
            //---> importante
            if (d_cell.Value == null)
            {
                d_cell.Value = "";
            }
            //
            DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
            string u = string.Empty;
            cell.Items.Clear();
            cell.Items.AddRange(items.ToArray());
            foreach (string x in cell.Items)
            {
                if (x == d_cell.Value.ToString())
                {
                    u = d_cell.Value.ToString();
                }
            }
            if (u == string.Empty)
            {
                d_cell.Value = "";
            }
            cell.Value = u;
            d_row.Cells[d_cell.ColumnIndex] = cell;
            cell.Dispose();
        }

        public void setFiltros()
        {
            if (comboBox3.SelectedIndex == 0)
            {
                comboBox4.Items.Clear();
                comboBox5.Items.Clear();
                comboBox4.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                comboBox5.Items.AddRange(constants.getCategorias("vidrio").ToArray());
            }
            else if (comboBox3.SelectedIndex == 1)
            {
                comboBox4.Items.Clear();
                comboBox5.Items.Clear();
                comboBox4.Items.AddRange(constants.getProveedores("aluminio").ToArray());
                comboBox5.Items.AddRange(constants.getCategorias("aluminio").ToArray());
            }
            else if (comboBox3.SelectedIndex == 2)
            {
                comboBox4.Items.Clear();
                comboBox5.Items.Clear();
                comboBox4.Items.AddRange(constants.getProveedores("herraje").ToArray());
                comboBox5.Items.AddRange(constants.getCategorias("herraje").ToArray());
            }
            else if (comboBox3.SelectedIndex == 3)
            {
                comboBox4.Items.Clear();
                comboBox5.Items.Clear();
                comboBox4.Items.AddRange(constants.getProveedores("otros").ToArray());
                comboBox5.Items.AddRange(constants.getCategorias("otros").ToArray());
            }           
        }

        //carga productos
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadListas();
        }

        //carga proveedores
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            listas_entities_pva listas = new listas_entities_pva();
            switch (comboBox3.SelectedIndex)
            {
                case 0:
                    if (comboBox5.Text != "")
                    {
                        var filter = from x in listas.lista_costo_corte_e_instalado
                                     where (x.articulo.StartsWith(comboBox5.Text)) && (x.proveedor == comboBox4.Text)
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                         Costo_Instalado = "$" + x.costo_instalado
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.lista_costo_corte_e_instalado
                                     where x.proveedor == comboBox4.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                         Costo_Instalado = "$" + x.costo_instalado
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    break;                             
                case 1:
                    if (comboBox5.Text != "")
                    {
                        var filter = from x in listas.perfiles
                                     where x.linea == comboBox5.Text && x.proveedor == comboBox4.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Largo = x.largo + " m",
                                         Crudo = "$" + x.crudo,
                                         Blanco = "$" + x.blanco,
                                         Hueso = "$" + x.hueso,
                                         Champagne = "$" + x.champagne,
                                         Gris = "$" + x.gris,
                                         Negro = "$" + x.negro,
                                         Brillante = "$" + x.brillante,
                                         Natural = "$" + x.natural_1,
                                         Madera = "$" + x.madera,
                                         Chocolate = "$" + x.chocolate,
                                         Acero_Inox = "$" + x.acero_inox,
                                         Bronce = "$" + x.bronce
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.perfiles
                                     where x.proveedor == comboBox4.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Largo = x.largo + " m",
                                         Crudo = "$" + x.crudo,
                                         Blanco = "$" + x.blanco,
                                         Hueso = "$" + x.hueso,
                                         Champagne = "$" + x.champagne,
                                         Gris = "$" + x.gris,
                                         Negro = "$" + x.negro,
                                         Brillante = "$" + x.brillante,
                                         Natural = "$" + x.natural_1,
                                         Madera = "$" + x.madera,
                                         Chocolate = "$" + x.chocolate,
                                         Acero_Inox = "$" + x.acero_inox,
                                         Bronce = "$" + x.bronce
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    break;
                case 2:
                    if (comboBox5.Text != "")
                    {
                        var filter = from x in listas.herrajes
                                     where x.linea == comboBox5.Text && x.proveedor == comboBox4.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.herrajes
                                     where x.proveedor == comboBox4.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    break;
                case 3:
                    if (comboBox5.Text != "")
                    {
                        var filter = from x in listas.otros
                                     where x.linea == comboBox5.Text && x.proveedor == comboBox4.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.otros
                                     where x.proveedor == comboBox4.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    break;
                default:
                    break;
            }
            listas.Dispose();
        }

        //carga filtros
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            listas_entities_pva listas = new listas_entities_pva();
            switch (comboBox3.SelectedIndex)
            {
                case 0:
                    if (comboBox4.Text != "")
                    {
                        var filter = from x in listas.lista_costo_corte_e_instalado
                                     where (x.articulo.StartsWith(comboBox5.Text)) && (x.proveedor == comboBox4.Text)
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                         Costo_Instalado = "$" + x.costo_instalado
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.lista_costo_corte_e_instalado
                                     where x.articulo.StartsWith(comboBox5.Text)
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                         Costo_Instalado = "$" + x.costo_instalado
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    break;                      
                case 1:
                    if (comboBox4.Text != "")
                    {
                        var filter = from x in listas.perfiles
                                     where x.linea == comboBox5.Text && x.proveedor == comboBox4.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Largo = x.largo + " m",
                                         Crudo = "$" + x.crudo,
                                         Blanco = "$" + x.blanco,
                                         Hueso = "$" + x.hueso,
                                         Champagne = "$" + x.champagne,
                                         Gris = "$" + x.gris,
                                         Negro = "$" + x.negro,
                                         Brillante = "$" + x.brillante,
                                         Natural = "$" + x.natural_1,
                                         Madera = "$" + x.madera,
                                         Chocolate = "$" + x.chocolate,
                                         Acero_Inox = "$" + x.acero_inox,
                                         Bronce = "$" + x.bronce
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.perfiles
                                     where x.linea == comboBox5.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Largo = x.largo + " m",
                                         Crudo = "$" + x.crudo,
                                         Blanco = "$" + x.blanco,
                                         Hueso = "$" + x.hueso,
                                         Champagne = "$" + x.champagne,
                                         Gris = "$" + x.gris,
                                         Negro = "$" + x.negro,
                                         Brillante = "$" + x.brillante,
                                         Natural = "$" + x.natural_1,
                                         Madera = "$" + x.madera,
                                         Chocolate = "$" + x.chocolate,
                                         Acero_Inox = "$" + x.acero_inox,
                                         Bronce = "$" + x.bronce
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    break;
                case 2:
                    if (comboBox4.Text != "")
                    {
                        var filter = from x in listas.herrajes
                                     where x.linea == comboBox5.Text && x.proveedor == comboBox4.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.herrajes
                                     where x.linea == comboBox5.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    break;
                case 3:
                    if (comboBox4.Text != "")
                    {
                        var filter = from x in listas.otros
                                     where x.linea == comboBox5.Text && x.proveedor == comboBox4.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.otros
                                     where x.linea == comboBox5.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = filter.ToList();
                    }
                    break;              
                default:
                    break;
            }
            listas.Dispose();
        }

        //Buscar producto
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            listas_entities_pva listas = new listas_entities_pva();
            switch (comboBox3.SelectedIndex)
            {
                case 0:
                    var costo_corte = from x in listas.lista_costo_corte_e_instalado
                                      where x.articulo.StartsWith(textBox5.Text) || x.clave.StartsWith(textBox5.Text)
                                      orderby x.articulo ascending
                                      select new
                                      {
                                          Clave = x.clave,
                                          Artículo = x.articulo,
                                          Proveedor = x.proveedor,
                                          Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                          Costo_Instalado = "$" + x.costo_instalado
                                      };
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = costo_corte.ToList();
                    break;                
                case 1:
                    var perfiles = from x in listas.perfiles
                                   where x.articulo.StartsWith(textBox5.Text) || x.clave.StartsWith(textBox5.Text)
                                   orderby x.articulo ascending
                                   select new
                                    {
                                        Id = x.id,
                                        Clave = x.clave,
                                        Artículo = x.articulo,
                                        Linea = x.linea,
                                        Proveedor = x.proveedor,
                                        Largo = x.largo + " m",
                                        Crudo = "$" + x.crudo,
                                        Blanco = "$" + x.blanco,
                                        Hueso = "$" + x.hueso,
                                        Champagne = "$" + x.champagne,
                                        Gris = "$" + x.gris,
                                        Negro = "$" + x.negro,
                                        Brillante = "$" + x.brillante,
                                        Natural = "$" + x.natural_1,
                                        Madera = "$" + x.madera,
                                        Chocolate = "$" + x.chocolate,
                                        Acero_Inox = "$" + x.acero_inox,
                                        Bronce = "$" + x.bronce
                                   };
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = perfiles.ToList();
                    break;
                case 2:
                    var herrajes = from x in listas.herrajes
                                   where x.articulo.StartsWith(textBox5.Text) || x.clave.StartsWith(textBox5.Text)
                                   orderby x.articulo ascending
                                   select new
                                   {
                                       Id = x.id,
                                       Clave = x.clave,
                                       Artículo = x.articulo,
                                       Linea = x.linea,
                                       Proveedor = x.proveedor,
                                       Caracteristicas = x.caracteristicas,
                                       Color = x.color,
                                       Precio = "$" + x.precio
                                   };
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = herrajes.ToList();
                    break;
                case 3:
                    var otros = from x in listas.otros
                                where x.articulo.StartsWith(textBox5.Text) || x.clave.StartsWith(textBox5.Text)
                                orderby x.articulo ascending
                                select new
                                {
                                    Id = x.id,
                                    Clave = x.clave,
                                    Artículo = x.articulo,
                                    Linea = x.linea,
                                    Proveedor = x.proveedor,
                                    Caracteristicas = x.caracteristicas,
                                    Color = x.color,
                                    Precio = "$" + x.precio
                                };
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = otros.ToList();
                    break;               
                default: break;
            }
            listas.Dispose();
        }

        //Carga los productos
        private void loadListas()
        {
            setFiltros();
            listas_entities_pva listas = new listas_entities_pva();
            switch (comboBox3.SelectedIndex)
            {
                case 0:
                    var cristal = from x in listas.lista_costo_corte_e_instalado
                                  orderby x.articulo ascending
                                  select new
                               {
                                   Clave = x.clave,
                                   Artículo = x.articulo,
                                   Proveedor = x.proveedor,
                                   Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                   Costo_Instalado = "$" + x.costo_instalado
                               };
                    if (dataGridView1.InvokeRequired == true)
                    {
                        dataGridView1.Invoke((MethodInvoker)delegate
                        {
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = cristal.ToList();
                        });
                    }
                    else {
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = cristal.ToList();
                    }
                    break;
                case 1:
                    var aluminio = from x in listas.perfiles
                                   orderby x.articulo ascending
                                   select new
                                   {
                                       Id = x.id,
                                       Clave = x.clave,
                                       Artículo = x.articulo,
                                       Linea = x.linea,
                                       Proveedor = x.proveedor,
                                       Largo = x.largo + " m",
                                       Crudo = "$" + x.crudo,
                                       Blanco = "$" + x.blanco,
                                       Hueso = "$" + x.hueso,
                                       Champagne = "$" + x.champagne,
                                       Gris = "$" + x.gris,
                                       Negro = "$" + x.negro,
                                       Brillante = "$" + x.brillante,
                                       Natural = "$" + x.natural_1,
                                       Madera = "$" + x.madera,
                                       Chocolate = "$" + x.chocolate,
                                       Acero_Inox = "$" + x.acero_inox,
                                       Bronce = "$" + x.bronce
                                   };
                    if (dataGridView1.InvokeRequired == true)
                    {
                        dataGridView1.Invoke((MethodInvoker)delegate
                        {
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = aluminio.ToList();
                        });
                    }
                    else {
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = aluminio.ToList();
                    }
                    break;
                case 2:
                    var herraje = from x in listas.herrajes
                                  orderby x.articulo ascending
                                  select new
                               {
                                   Id = x.id,
                                   Clave = x.clave,
                                   Artículo = x.articulo,
                                   Linea = x.linea,
                                   Proveedor = x.proveedor,
                                   Caracteristicas = x.caracteristicas,
                                   Color = x.color,
                                   Precio = "$" + x.precio
                               };
                    if (dataGridView1.InvokeRequired == true)
                    {
                        dataGridView1.Invoke((MethodInvoker)delegate
                        {
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = herraje.ToList();
                        });
                    }
                    else {
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = herraje.ToList();
                    }
                    break;
                case 3:
                    var data = from x in listas.otros
                               orderby x.articulo ascending
                               select new
                               {
                                   Id = x.id,
                                   Clave = x.clave,
                                   Artículo = x.articulo,
                                   Linea = x.linea,
                                   Proveedor = x.proveedor,
                                   Caracteristicas = x.caracteristicas,
                                   Color = x.color,
                                   Precio = "$" + x.precio
                               };
                    if (dataGridView1.InvokeRequired == true)
                    {
                        dataGridView1.Invoke((MethodInvoker)delegate
                        {
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = data.ToList();
                        });
                    }
                    else {
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = data.ToList();
                    }
                    break;
                default:
                    break;
            }
        }

        private void afterSave()
        {           
            textBox1.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
            textBox10.Clear();
            textBox11.Clear();
            pictureBox1.Image = null;
            button6.Visible = false;
            button13.Visible = false;
            label27.Text = string.Empty;
            textBox12.Text = string.Empty;
            label19.Text = string.Empty;
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            pictureBox3.Image = null;
            autor = "";
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            //                   
            comboBox2.SelectedItem = null;
            //
            richTextBox1.Clear();
            richTextBox3.Clear();
            label2.Text = "0000000000";
            clearModulos();
            dataGridView6.Rows.Clear();
            dataGridView1.DataSource = null;
            dataGridView6.DataSource = null;
            countItems();
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.Padding = new Padding(0, 0, 0, 0);
        }

        private void clearDiseño()
        {
            textBox6.Clear();
            label27.Text = string.Empty;
            textBox12.Text = string.Empty;
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.Padding = new Padding(0, 0, 0, 0);
        }

        //productos
        private void setLines()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox1.Items.AddRange(constants.getLineasModulo().ToArray());
            comboBox2.Items.AddRange(constants.getLineasModulo().ToArray());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Crear modulo
        private void articulosModulo(string componente, int id_articulo, string clave, string articulo, float cantidad, string ubicacion, int seccion)
        {
            var module = new modulos_articulos
            {
                componente = componente,
                id_articulo = id_articulo,
                clave = clave,
                articulo = articulo,
                cantidad = Math.Round(cantidad, 2),
                ubicacion = ubicacion,
                seccion = seccion
            };
            articulos.modulos_articulos.Add(module);
            articulos.SaveChanges();
        }

        private void loadArticulosModulo(bool clear=true)
        {
            clearModulos();
            try
            {
                foreach (DataGridViewRow x in dataGridView6.Rows)
                {
                    articulosModulo(x.Cells[0].Value.ToString(), constants.stringToInt(x.Cells[1].Value.ToString()), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), getCount(x.Cells[4].Value.ToString()), x.Cells[5].Value.ToString(), getSeccion(x.Cells[6].Value.ToString()));
                }
                if (clear == true)
                {
                    dataGridView6.Rows.Clear();
                }
            }
            catch (Exception) { };
        }       

        private Boolean checkSecciones()
        {
            bool r = false;
            foreach(DataGridViewRow x in dataGridView6.Rows)
            {
               if(getSeccion(x.Cells[6].Value.ToString()) < 0)
                {
                    r = true;
                    break;
                }
            }
            return r;
        }

        private int getIdFromDiseño(string diseño)
        {
            string buffer = string.Empty;
            int id = 0;
            foreach(char x in diseño)
            {
                if(x == '-')
                {
                    id = constants.stringToInt(buffer);
                    break;
                }
                buffer = buffer + x.ToString();
            }
            return id;
        }

        private bool getItemReady()
        {
            bool r = true;
            listas_entities_pva listas = new listas_entities_pva();
            foreach(DataGridViewRow x in dataGridView6.Rows)
            {
                if(x.Cells[0].Value.ToString() == "Perfil")
                {
                    if(x.Cells[5].Value.ToString() == "" || x.Cells[5].Value == null)
                    {
                        x.DefaultCellStyle.BackColor = Color.Red;
                        r = false;
                        break;
                    }
                }
                else if (x.Cells[0].Value.ToString() == "Otros")
                {
                    int id = constants.stringToInt(x.Cells[1].Value.ToString());
                    var otro = (from v in listas.otros where v.id == id select v).SingleOrDefault();
                    if(otro != null)
                    {
                        if((otro.largo > 0 && otro.alto == 0) || (otro.largo == 0 && otro.alto > 0))
                        {
                            if (x.Cells[5].Value.ToString() == "" || x.Cells[5].Value == null)
                            {
                                x.DefaultCellStyle.BackColor = Color.Red;
                                r = false;
                                break;
                            }
                        }
                    }
                }
            }
            return r;
        }

        private string getModuloName(string name, bool cs)
        {
            string[] r = name.Split('-');
            try {
                if (r.Length > 1 && cs == true)
                {
                    return r[1];
                }
                else
                {
                    return name;
                }
            }
            catch (Exception)
            {
                return name;
            }
        }

        private bool checkInvalidChars(string clave)
        {
            bool r = true;
            foreach (char x in clave)
            {
                if (x == '_')
                {
                    r = false;
                }
                else if (x == '-')
                {
                    r = false;
                }
                else if (x == ',')
                {
                    r = false;
                }
                else if (x == '$')
                {
                    r = false;
                }
                else if (x == ':')
                {
                    r = false;
                }
                else if (x == '@')
                {
                    r = false;
                }
                else if (x == '#')
                {
                    r = false;
                }
                else if (x == ',')
                {
                    r = false;
                }
                else if (x == '.')
                {
                    r = false;
                }
                else if (x == ';')
                {
                    r = false;
                }
                else if (x == '%')
                {
                    r = false;
                }                
            }
            if(clave.Contains("S/M") == true || clave.Contains("C/M") == true)
            {
                r = false;
            }
            return r;
        }

        private string getParametros()
        {
            return constants.stringToInt(textBox8.Text) + "," + constants.stringToInt(textBox9.Text) + "," + constants.stringToInt(textBox10.Text) + "," + constants.stringToInt(textBox11.Text);
        }

        private string getReglas()
        {
            string r = string.Empty;
            if (richTextBox3.Text != "")
            {
                foreach (string x in richTextBox3.Lines)
                {
                    if (x.Length > 0)
                    {
                        r = r + x + "$";                                            
                    }
                }
            }
            return r;
        }

        private string getMosquiteros()
        {
            if(checkBox5.Checked == true)
            {
                if(checkBox4.Checked == true)
                {
                    return " C/M";
                }
                else
                {
                    return " S/M";
                }
            }
            else
            {
                return "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                if (checkInvalidChars(textBox1.Text) == true)
                {
                    if (comboBox2.Text != "")
                    {
                        if (textBox12.Text != "")
                        {
                            if (dataGridView6.RowCount > 0)
                            {
                                if (checkSecciones() == false)
                                {
                                    if (getItemReady() == true)
                                    {
                                        sqlDateBaseManager sql = new sqlDateBaseManager();
                                        if (module_id <= 0)
                                        {
                                            if (sql.findSQLValue("articulo", "articulo", "modulos", textBox1.Text) == false)
                                            {
                                                loadArticulosModulo();
                                                label2.Text = setModuloClave().ToUpper();
                                                sql.createModule(label2.Text, textBox7.Text + textBox1.Text + getMosquiteros(), comboBox2.Text, getCristales(), getPerfiles(), getHerrajes(), getOtros(), constants.stringToInt(textBox6.Text), richTextBox1.Text, constants.user, getIdFromDiseño(textBox12.Text), checkBox2.Checked ? true : false, getParametros(), getReglas(), checkBox3.Checked == true ? true : false);
                                                ((Form1)Application.OpenForms["form1"]).manualUpdater();
                                                MessageBox.Show("Se ha creado el módulo con clave: " + label2.Text, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                afterSave();
                                            }
                                            else
                                            {
                                                MessageBox.Show("[Error] ya existe un artículo con ese nombre.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                        else
                                        {
                                            DialogResult result = MessageBox.Show("¿Deseas sobrescribir esté módulo?", constants.msg_box_caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                                            if (result == DialogResult.OK)
                                            {
                                                loadArticulosModulo(false);
                                                sql.updateModule(module_id, textBox7.Text + textBox1.Text + getMosquiteros(), comboBox2.Text, getCristales(), getPerfiles(), getHerrajes(), getOtros(), constants.stringToInt(textBox6.Text), richTextBox1.Text, getIdFromDiseño(textBox12.Text), checkBox2.Checked ? true : false, getParametros(), getReglas(), checkBox3.Checked == true ? true : false);
                                                tabControl1.SelectedTab = tabPage4;
                                                countItems();
                                                setColors();
                                                ((Form1)Application.OpenForms["form1"]).manualUpdater();
                                                MessageBox.Show("Se ha actualizado el módulo con clave: " + label2.Text, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("[Error] existe uno o más componentes sin ubicación.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("[Error] existe uno o más artículos sin sección.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("[Error] necesitas incluir algunos artículos al diseño.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] necesitas incluir un diseño.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] necesitas incluir una linea a esté módulo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("[Error] el nombre incluye caracteres no válidos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] necesitas nombrar este módulo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }                      
        }
        //

        private string getPerfiles()
        {
            string r = "";           

            var data = (from x in articulos.modulos_articulos select x).ToArray();
            for(int i = 0; i < data.Length; i++)
            {
                if (data[i].componente == "Perfil")
                {
                    r = r + data[i].clave + ":" + data[i].cantidad + "-" + data[i].ubicacion + "$" + data[i].seccion + ",";
                }
            }

            return r;       
        }

        private string getCristales()
        {
            string r = "";

            var data = (from x in articulos.modulos_articulos select x).ToArray();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].componente == "Cristal")
                {
                    r = r + data[i].clave + ":" + data[i].cantidad + "$" + data[i].seccion + ",";
                }
            }

            return r;
        }

        private string getHerrajes()
        {
            string r = "";

            var data = (from x in articulos.modulos_articulos select x).ToArray();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].componente == "Herraje")
                {
                    r = r + data[i].clave + ":" + data[i].cantidad + "$" + data[i].seccion + ",";
                }
            }

            return r;
        }

        private string getOtros()
        {
            string r = "";

            var data = (from x in articulos.modulos_articulos select x).ToArray();
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].componente == "Otros")
                {
                    r = r + data[i].clave + ":" + data[i].cantidad + "-" + data[i].ubicacion + "$" + data[i].seccion + ",";
                }
            }

            return r;
        }

        private void setImageArticulo()
        {
            if(dataGridView6.CurrentRow.Cells[0].Value.ToString() == "Perfil")
            {
                constants.setImage(constants.getArticuloProveedorPerfiles(constants.stringToInt(dataGridView6.CurrentRow.Cells[1].Value.ToString())), dataGridView6.CurrentRow.Cells[1].Value.ToString(), "png", pictureBox1);
            }
            else if (dataGridView6.CurrentRow.Cells[0].Value.ToString() == "Cristal")
            {
                constants.setImage(constants.getArticuloProveedorCristales(dataGridView6.CurrentRow.Cells[2].Value.ToString()), dataGridView6.CurrentRow.Cells[2].Value.ToString(), "png", pictureBox1);
            }
            else if (dataGridView6.CurrentRow.Cells[0].Value.ToString() == "Herraje")
            {
                constants.setImage(constants.getArticuloProveedorHerrajes(constants.stringToInt(dataGridView6.CurrentRow.Cells[1].Value.ToString())), dataGridView6.CurrentRow.Cells[1].Value.ToString(), "png", pictureBox1);
            }
            else if (dataGridView6.CurrentRow.Cells[0].Value.ToString() == "Otros")
            {
                constants.setImage(constants.getArticuloProveedorOtros(constants.stringToInt(dataGridView6.CurrentRow.Cells[1].Value.ToString())), dataGridView6.CurrentRow.Cells[1].Value.ToString(), "png", pictureBox1);
            }
        }

        private string setModuloClave()
        {
            string s = "";
            sqlDateBaseManager sql = new sqlDateBaseManager();
            Random r = new Random();
            char[] c = textBox1.Text.ToCharArray();
            s = c[0].ToString() + c[1].ToString() + c[2].ToString() + r.Next(1000000, 1999999).ToString();

            while (sql.findSQLValue("clave", "clave", "modulos", s) == true)
            {
                s = c[0].ToString() + c[1].ToString() + c[2].ToString() + r.Next(1000000, 1999999).ToString();
            }
            return s; 
        }

        //modulos
        private void button3_Click(object sender, EventArgs e)
        {
            verListaModulos();
        }

        private void verListaModulos()
        {
            if (backgroundWorker1.IsBusy == false)
            {
                if (backgroundWorker3.IsBusy == false)
                {
                    sqlDateBaseManager sql = new sqlDateBaseManager();
                    if (constants.user_access == 4)
                    {
                        sql.dropTableOnGridViewWithFilter(datagridviewNE1, "modulos", "usuario", constants.user, true);
                    }
                    else
                    {
                        sql.dropTableOnGridView(datagridviewNE1, "modulos");
                    }
                    progressBar1.Visible = true;
                    progressBar1.Value = 0;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if(constants.user_access == 4)
            {
               new sqlDateBaseManager().getModuloFilterUser(datagridviewNE1, "modulos", "articulo", textBox2.Text);
            }
            else
            {
               new sqlDateBaseManager().dropTableOnGridViewWithFilter(datagridviewNE1, "modulos", "articulo", textBox2.Text);
            }
            if (backgroundWorker1.IsBusy == false)
            {
                progressBar1.Visible = true;
                progressBar1.Value = 0;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void deserializeParameters(string parameters)
        {
            string[] param = parameters.Split(',');

            //Reset Texboxes
            textBox8.Text = "0";
            textBox9.Text = "0";
            textBox10.Text = "0";
            textBox11.Text = "0";
            //

            if (param.Length == 4)
            {
                if (constants.stringToInt(param[0]) > 0)
                {
                    textBox8.Text = constants.stringToInt(param[0]).ToString();
                }
                if (constants.stringToInt(param[1]) > 0)
                {
                    textBox9.Text = constants.stringToInt(param[1]).ToString();
                }
                if (constants.stringToInt(param[2]) > 0)
                {
                    textBox10.Text = constants.stringToInt(param[2]).ToString();
                }
                if (constants.stringToInt(param[3]) > 0)
                {
                    textBox11.Text = constants.stringToInt(param[3]).ToString();
                }
            }
        }

        private void modificar(int id)
        {
            listas_entities_pva listas = new listas_entities_pva();

            var modulos = (from x in listas.modulos where x.id == id select x).SingleOrDefault();

            if (modulos != null)
            {
                if (modulos.privado == false || constants.user_access == 6)
                {
                    int diseño_id = (int)modulos.id_diseño;
                    if(modulos.articulo.Contains("C/M") == true || modulos.articulo.Contains("S/M") == true)
                    {
                        checkBox5.Checked = true;
                        if (modulos.articulo.Contains("C/M") == true)
                        {
                            checkBox4.Checked = true;
                        }
                        else
                        {
                            checkBox4.Checked = false;
                        }
                    }
                    else
                    {
                        checkBox5.Checked = false;
                        checkBox4.Checked = false;
                    }                 
                    checkBox3.Checked = modulos.privado == true ? checkBox3.Checked = true : checkBox3.Checked = false;
                    deserializeParameters(modulos.parametros);
                    LeerDiseño(diseño_id);
                    label2.Text = modulos.clave;
                    textBox1.Text = checkBox4.Checked == true ? getModuloName(modulos.articulo, (bool)modulos.cs).Replace(" C/M", "") : getModuloName(modulos.articulo, (bool)modulos.cs).Replace(" S/M", "");
                    textBox6.Text = modulos.secciones.ToString();
                    comboBox2.Text = modulos.linea;
                    autor = modulos.usuario;
                    richTextBox1.Text = modulos.descripcion;
                    string[] reglas = modulos.reglas.Split('$');
                    richTextBox3.Lines = reglas;
                    button6.Visible = true;
                    button13.Visible = true;
                    if (modulos.cs == true)
                    {
                        checkBox2.Checked = true;
                        textBox7.Text = "*(CS)-";
                    }
                    else
                    {
                        checkBox2.Checked = false;
                        textBox7.Clear();
                    }
                    //Delete info ----------------------------------------
                    clearModulos();
                    dataGridView6.Rows.Clear();
                    // ---------------------------------------------------

                    constants.abrirModulo(id, dataGridView6);
                    tabControl1.SelectedTab = tabPage4;
                    countItems();
                    setColors();
                }
                else
                {
                    if(modulos.usuario == constants.user)
                    {
                        int diseño_id = (int)modulos.id_diseño;
                        checkBox3.Checked = modulos.privado == true ? checkBox3.Checked = true : checkBox3.Checked = false;
                        deserializeParameters(modulos.parametros);
                        LeerDiseño(diseño_id);
                        label2.Text = modulos.clave;
                        textBox1.Text = checkBox4.Checked == true ? getModuloName(modulos.articulo, (bool)modulos.cs).Replace(" C/M", "") : getModuloName(modulos.articulo, (bool)modulos.cs).Replace(" S/M", "");
                        textBox6.Text = modulos.secciones.ToString();
                        comboBox2.Text = modulos.linea;
                        autor = modulos.usuario;
                        richTextBox1.Text = modulos.descripcion;
                        string[] reglas = modulos.reglas.Split('$');
                        richTextBox3.Lines = reglas;
                        button6.Visible = true;
                        button13.Visible = true;
                        if (modulos.cs == true)
                        {
                            checkBox2.Checked = true;
                            textBox7.Text = "*(CS)-";
                        }
                        else
                        {
                            checkBox2.Checked = false;
                            textBox7.Clear();
                        }
                        //Delete info ----------------------------------------
                        clearModulos();
                        dataGridView6.Rows.Clear();
                        // ---------------------------------------------------

                        constants.abrirModulo(id, dataGridView6);
                        tabControl1.SelectedTab = tabPage4;
                        countItems();
                        setColors();
                    }
                    else
                    {
                        MessageBox.Show("Este módulo es privado y solo el autor puede hacer uso de el.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }        
        }

        private void copiarArticulos(int id)
        {         
            constants.abrirModulo(id, dataGridView6);
            tabControl1.SelectedTab = tabPage4;
            countItems();
            setColors();          
        }

        private void modificarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                module_id = constants.stringToInt(datagridviewNE1.CurrentRow.Cells[0].Value.ToString());
                modificar(module_id);
            }
        }

        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                try
                {
                    DialogResult r = MessageBox.Show("¿Estás seguro de eliminar esté modulo?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (r == DialogResult.Yes)
                    {
                        string clave = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                        if (clave != label2.Text)
                        {
                            if (constants.user_access == 6)
                            {
                                sqlDateBaseManager sql = new sqlDateBaseManager();
                                sql.deleteModule((int)datagridviewNE1.CurrentRow.Cells[0].Value);
                                sql.dropTableOnGridView(datagridviewNE1, "modulos");
                                MessageBox.Show("El módulo fue eliminado exitosamente.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("[Error] solo un administrador puede ejecutar esta orden.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] no se puede eliminar el módulo mientras esta abierto.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                    MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Lineas
        private void button5_Click(object sender, EventArgs e)
        {
            new sqlDateBaseManager().dropTableOnGridView(dataGridView4, "lineas_modulos");
            foreach(DataGridViewRow x in dataGridView4.Rows)
            {
                if(constants.imageExist("series", x.Cells[1].Value.ToString(), "png") == false)
                {
                    x.DefaultCellStyle.BackColor = Color.Red;
                }
            }
        }
        //

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            new sqlDateBaseManager().dropTableOnGridViewWithFilter(dataGridView4, "lineas_modulos", "linea_modulo", textBox4.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(textBox3.Text != "")
            {
                sqlDateBaseManager sql = new sqlDateBaseManager();

                if (sql.findSQLValue("linea_modulo", "linea_modulo", "lineas_modulos", textBox3.Text) == false)
                {
                    sql.CreateNewLine(textBox3.Text);
                    sql.dropTableOnGridView(dataGridView4, "lineas_modulos");
                    label9.Text = "Se ha agregado: " + textBox3.Text;
                    setLines();
                    ((Form1)Application.OpenForms["form1"]).setFiltros();
                }
                else
                {
                    MessageBox.Show("[Error] ese nombre ya existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] necesitas darle nombre a la linea.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void eliminarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dataGridView4.RowCount > 0)
            {
                try
                {
                    DialogResult r = MessageBox.Show("¿Estás seguro de eliminar está linea?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (r == DialogResult.Yes)
                    {
                        if (constants.user_access == 6)
                        {
                            sqlDateBaseManager sql = new sqlDateBaseManager();
                            sql.deleteLine((int)dataGridView4.CurrentRow.Cells[0].Value);
                            sql.dropTableOnGridView(dataGridView4, "lineas_modulos");
                            setLines();
                            MessageBox.Show("La linea fue eliminada exitosamente.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("[Error] solo un administrador puede ejecutar esta orden.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                    MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void crear_modulo_FormClosed(object sender, FormClosedEventArgs e)
        {
            ((Form1)Application.OpenForms["form1"]).Enabled = true;
            if (constants.updater_form_close == true)
            {
                new loading_form().ShowDialog();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            module_id = 0;
            afterSave();
            button6.Visible = false;
        }     

        private void seccionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadArticulosModulo();
            var data = (from x in articulos.modulos_articulos orderby x.seccion descending select x);

            if (data != null)
            {
                foreach (var c in data)
                {
                    dataGridView6.Rows.Add(c.componente, c.id_articulo, c.clave, c.articulo, c.cantidad, c.ubicacion, c.seccion);
                }
                setChooseColors(true);
            }
        }

        private void componentesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadArticulosModulo();
            var data = (from x in articulos.modulos_articulos orderby x.componente descending select x);

            if (data != null)
            {
                foreach (var c in data)
                {
                    dataGridView6.Rows.Add(c.componente, c.id_articulo, c.clave, c.articulo, c.cantidad, c.ubicacion, c.seccion);
                }
                setChooseColors(false);
            }
        }
        //

        //cargar Esquemas
        private void cargarEsquemas()
        {
            listas_entities_pva listas = new listas_entities_pva();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add("Abatibles");
            treeView1.Nodes.Add("Corredizas");
            treeView1.Nodes.Add("Fijos");
            treeView1.Nodes.Add("Puertas");
            treeView1.Nodes.Add("Templados");
            treeView1.Nodes.Add("Otros");

            var esquemas = (from x in listas.esquemas select x);
            if(esquemas != null)
            {
                foreach(var x in esquemas)
                {
                    if (x.grupo == "abatible")
                    {
                        treeView1.Nodes[0].Nodes.Add(x.id.ToString() + "-" + x.nombre);
                    }
                    else if (x.grupo == "corrediza")
                    {
                        treeView1.Nodes[1].Nodes.Add(x.id.ToString() + "-" + x.nombre);
                    }
                    else if (x.grupo == "fijo")
                    {
                        treeView1.Nodes[2].Nodes.Add(x.id.ToString() + "-" + x.nombre);
                    }
                    else if (x.grupo == "puerta")
                    {
                        treeView1.Nodes[3].Nodes.Add(x.id.ToString() + "-" + x.nombre);
                    }
                    else if (x.grupo == "templados")
                    {
                        treeView1.Nodes[4].Nodes.Add(x.id.ToString() + "-" + x.nombre);
                    }
                    else if (x.grupo == "otros")
                    {
                        treeView1.Nodes[5].Nodes.Add(x.id.ToString() + "-" + x.nombre);
                    }
                }
            }
        }
        //

        private void setColors()
        {
            try
            {
                foreach (DataGridViewRow x in dataGridView6.Rows)
                {
                    if (x.Cells[3].Value.ToString() != "" && x.Cells[3].Value != null)
                    {
                        if (checkBox2.Checked)
                        {
                            switch (x.Cells[6].Value.ToString())
                            {
                                case "0":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#B9EEB7"); // green
                                    break;
                                case "1":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#D1B7EE"); // purple
                                    break;
                                case "2":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#C6E2FF"); //blue
                                    break;
                                case "3":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#EEB7B9"); //pink
                                    break;
                                case "4":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#eec000"); //orange
                                    break;
                                case "5":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#FD3C3C"); //red
                                    break;
                                case "6":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#b5beff"); //grey
                                    break;
                                case "7":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#f1a66a"); //skin
                                    break;
                                default:
                                    x.DefaultCellStyle.BackColor = constants.getColor("#a54657"); //black_pink
                                    break;
                            }
                        }
                        else
                        {
                            switch (x.Cells[0].Value.ToString())
                            {
                                case "Perfil":
                                    x.DefaultCellStyle.BackColor = Color.LightGray;
                                    break;
                                case "Cristal":
                                    x.DefaultCellStyle.BackColor = Color.LightBlue;
                                    break;
                                case "Herraje":
                                    x.DefaultCellStyle.BackColor = Color.LightYellow;
                                    break;
                                case "Otros":
                                    x.DefaultCellStyle.BackColor = Color.LightGreen;
                                    break;
                                default: break;
                            }
                        }
                    }
                    else
                    {
                        x.DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }
            catch (Exception) { }          
        }

        private void setChooseColors(bool secciones)
        {
            try
            {
                foreach (DataGridViewRow x in dataGridView6.Rows)
                {
                    if (x.Cells[3].Value.ToString() != "" && x.Cells[3].Value != null)
                    {
                        if (secciones == true)
                        {
                            switch (x.Cells[6].Value.ToString())
                            {
                                case "0":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#B9EEB7"); // green
                                    break;
                                case "1":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#D1B7EE"); // purple
                                    break;
                                case "2":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#C6E2FF"); //blue
                                    break;
                                case "3":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#EEB7B9"); //pink
                                    break;
                                case "4":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#eec000"); //orange
                                    break;
                                case "5":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#FD3C3C"); //red
                                    break;
                                case "6":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#b5beff"); //grey
                                    break;
                                case "7":
                                    x.DefaultCellStyle.BackColor = constants.getColor("#f1a66a"); //skin
                                    break;
                                default:
                                    x.DefaultCellStyle.BackColor = constants.getColor("#a54657"); //black_pink
                                    break;
                            }
                        }
                        else
                        {
                            switch (x.Cells[0].Value.ToString())
                            {
                                case "Perfil":
                                    x.DefaultCellStyle.BackColor = Color.LightGray;
                                    break;
                                case "Cristal":
                                    x.DefaultCellStyle.BackColor = Color.LightBlue;
                                    break;
                                case "Herraje":
                                    x.DefaultCellStyle.BackColor = Color.LightYellow;
                                    break;
                                case "Otros":
                                    x.DefaultCellStyle.BackColor = Color.LightGreen;
                                    break;
                                default: break;
                            }
                        }
                    }
                    else
                    {
                        x.DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }
            catch (Exception) { }
        }

        private void clearModulos()
        {
            articulos = new localDateBaseEntities3();
            articulos.Database.ExecuteSqlCommand("TRUNCATE TABLE modulos_articulos");
            articulos.Database.ExecuteSqlCommand("DBCC CHECKIDENT (modulos_articulos, RESEED, 1)");
        }

        //Actualizar diseños
        private void button8_Click(object sender, EventArgs e)
        {
            new sqlDateBaseManager().dropTableOnGridView(dataGridView2, "esquemas");
        }

        //Leer diseño
        private void LeerDiseño(int id_diseño)
        {
            listas_entities_pva listas = new listas_entities_pva();
            var diseño = (from x in listas.esquemas where x.id == id_diseño select x).SingleOrDefault();
            if(diseño != null)
            {
                tableLayoutPanel1.Controls.Clear();
                textBox12.Text = diseño.id + "-" + diseño.nombre;
                label27.Text = "Columnas: " + diseño.columnas + " / " + "Filas: " + diseño.filas;
                getEsquemasFromDiseño(diseño.esquemas);
                tableLayoutPanel1.RowCount = (int)diseño.filas;
                tableLayoutPanel1.ColumnCount = (int)diseño.columnas;
                label19.Text = "Diseño: (" + diseño.diseño + ")";
                foreach (string e in esquemas)
                {
                    if (File.Exists(constants.folder_resources_dir + "esquemas\\corredizas\\" + e + ".png"))
                    {
                        constants.loadDiseño("esquemas\\corredizas\\", e, tableLayoutPanel1);
                    }
                    else if(File.Exists(constants.folder_resources_dir + "esquemas\\puertas\\" + e + ".png"))
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
                }
                if (diseño.marco == true)
                {
                    if(diseño.grupo == "puerta")
                    {
                        tableLayoutPanel1.Padding = new Padding(10, 10, 10, 0);
                    }
                    else
                    {
                        tableLayoutPanel1.Padding = new Padding(10, 10, 10, 10);
                    }
                    checkBox1.Checked = true;
                    checkBox2.Checked = false;
                    checkBox2.Enabled = true;
                }
                else
                {
                    tableLayoutPanel1.Padding = new Padding(0, 0, 0, 0);
                    checkBox1.Checked = false;
                    checkBox2.Checked = true;
                    checkBox2.Enabled = false;
                }
                tableLayoutPanel1.RowStyles.Clear();
                for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
                {
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100 / tableLayoutPanel1.RowCount));
                }
                tableLayoutPanel1.ColumnStyles.Clear();
                for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
                {
                    ColumnStyle sty = new ColumnStyle(SizeType.Percent, 100 / tableLayoutPanel1.ColumnCount);
                    tableLayoutPanel1.ColumnStyles.Add(sty);
                }
                if (!checkBox2.Checked)
                {
                    textBox6.Clear();
                }
                else
                {
                    textBox6.Text = tableLayoutPanel1.Controls.Count.ToString();
                }
            }
        }

        private void getEsquemasFromDiseño(string diseño_esquemas)
        {
            esquemas.Clear();
            string buffer = string.Empty;
            foreach (char x in diseño_esquemas)
            {
                if (x == ',')
                {
                    esquemas.Add(buffer);
                    buffer = string.Empty;
                    continue;
                }
                buffer = buffer + x.ToString();
            }
        }
        //

        //Eliminar diseño
        private void borrarDiseñoArbol(string diseño)
        {
            try
            {
                foreach (TreeNode x in treeView1.Nodes)
                {
                    foreach (TreeNode v in treeView1.Nodes[x.Index].Nodes)
                    {
                        if (v.Text == diseño)
                        {
                            v.Remove();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
        }

        private void eliminarToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (dataGridView2.RowCount > 0)
            {
                try
                {
                    DialogResult r = MessageBox.Show("¿Estás seguro de eliminar esté diseño de apertura?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (r == DialogResult.Yes)
                    {
                        if (constants.user_access == 6)
                        {
                            string id_diseño = dataGridView2.CurrentRow.Cells[0].Value.ToString();
                            if (File.Exists(constants.folder_resources_dir + "\\modulos\\" + id_diseño + ".jpg") == true)
                            {
                                File.Delete(constants.folder_resources_dir + "\\modulos\\" + id_diseño + ".jpg");
                            }
                            sqlDateBaseManager sql = new sqlDateBaseManager();
                            sql.borrarEsquema(constants.stringToInt(dataGridView2.CurrentRow.Cells[0].Value.ToString()));
                            borrarDiseñoArbol(dataGridView2.CurrentRow.Cells[0].Value.ToString() + "-" + dataGridView2.CurrentRow.Cells[1].Value.ToString());
                            sql.dropTableOnGridView(dataGridView2, "esquemas");
                            clearDiseño();
                            MessageBox.Show("El diseño de apertura fue eliminado exitosamente.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("[Error] solo un administrador puede ejecutar esta orden.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                    MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private int getSeccion(string seccion)
        {
            try
            {
                return int.Parse(seccion);
            }
            catch (Exception)
            {
                return -1;
            }
        }    

        private float getCount(string count)
        {
            try
            {
                return (float)Math.Round(float.Parse(count), 2);
            }
            catch (Exception)
            {
                return 1;
            }
        }        

        private void button11_Click(object sender, EventArgs e)
        {
            new loading_form().ShowDialog();
            loadAll();
        }

        //Clonar
        private void button13_Click(object sender, EventArgs e)
        {
            button13.Visible = false;
            autor = "";
            label2.Text = "0000000000";
            button6.Visible = false;
            module_id = 0;
        }

        //Diseño
        private void button10_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["diseño"] == null)
            {
                if (constants.user_access >= 5)
                {
                    new diseño().Show();
                }
                else
                {
                    MessageBox.Show("[Error] solo un usuario con privilegios de grado (5) puede acceder a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Application.OpenForms["diseño"].Select();
                Application.OpenForms["diseño"].WindowState = FormWindowState.Normal;
            }
        }

        //reset button
        private void button9_Click(object sender, EventArgs e)
        {
            if(comboBox3.Text != "")
            {
                loadListas();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox2.Checked)
            {
                textBox6.Clear();
                textBox7.Clear();
                foreach (DataGridViewRow x in dataGridView6.Rows)
                {
                    setSecciones(x.Cells[6], x);
                    x.Cells[6].Value = "0";
                }
            }
            else
            {
                textBox6.Text = tableLayoutPanel1.Controls.Count.ToString();
                textBox7.Text = "*(CS)-";
                foreach (DataGridViewRow x in dataGridView6.Rows)
                {
                    setSecciones(x.Cells[6], x);
                    x.Cells[6].Value = "";
                }
            }
        }

        //check diseño-modulo relacion
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            int rows = datagridviewNE1.RowCount;
            int c = 0;
            errors = 0;
            if (rows > 0)
            {
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    if (sql.getLostDiseño(constants.stringToInt(x.Cells[11].Value.ToString())) == false)
                    {
                        x.DefaultCellStyle.BackColor = Color.Red;
                        errors++;
                    }
                    c++;
                    backgroundWorker1.ReportProgress((c * 100) / rows);
                }
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label21.Text = "Se encontraron: " + datagridviewNE1.RowCount + " módulos / con: " + errors + " errores.";
            progressBar1.Visible = false;
            progressBar1.Value = 100;
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        //------------------------------------------------->

        ~crear_modulo()
        {

        }

        //Exportar datos
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            constants.ExportToExcelFile(dataGridView6);
        }

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox2.Visible = false;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (dataGridView6.RowCount > 0)
            {
                if (backgroundWorker2.IsBusy == false)
                {
                    pictureBox2.Visible = true;
                    backgroundWorker2.RunWorkerAsync();
                }
            }
            else
            {
                MessageBox.Show("[Error] no existen datos para exportar.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        //imprimir desglose
        private void button14_Click(object sender, EventArgs e)
        {
            modulo_data md = new modulo_data();

            foreach (DataGridViewRow x in dataGridView6.Rows)
            {
                DataRow row = md.Tables["modulo_data"].NewRow();
                row[0] = x.Cells[0].Value;
                row[1] = x.Cells[1].Value;
                row[2] = x.Cells[2].Value;
                row[3] = x.Cells[3].Value;
                row[4] = x.Cells[4].Value;
                row[5] = x.Cells[5].Value;
                row[6] = x.Cells[6].Value;
                md.Tables["modulo_data"].Rows.Add(row);
            }
            createModuloPic(md);
            string m = string.Empty;
            if(textBox13.Text != string.Empty)
            {
                m = " " + textBox13.Text;
            }
            new modulo_data_form(md, label2.Text, textBox7.Text + textBox1.Text + m, comboBox2.Text, "Largo: 1000 mm - Alto: 1000 mm", autor).ShowDialog();
            md.Dispose();
        }

        //Buscar modulo con filtros
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == false)
            {
                sqlDateBaseManager sql = new sqlDateBaseManager();
                if(constants.user_access == 4)
                {
                    sql.getModuloFilterUser(datagridviewNE1, "modulos", "linea", comboBox1.Text);
                }
                else
                {
                    sql.dropTableOnGridViewWithFilter(datagridviewNE1, "modulos", "linea", comboBox1.Text, true);
                }
                progressBar1.Visible = true;
                progressBar1.Value = 0;
                backgroundWorker1.RunWorkerAsync();
            }
        }
        //

        //Crear pic modulo
        private void createModuloPic(modulo_data data)
        {           
            Bitmap bm = new Bitmap(tableLayoutPanel1.Width, tableLayoutPanel1.Height);
            tableLayoutPanel1.DrawToBitmap(bm, new Rectangle(0, 0, tableLayoutPanel1.Width, tableLayoutPanel1.Height));
            Bitmap gm_2 = new Bitmap(bm, 120, 105);
            data.Tables["img_modulo"].Rows.Clear();
            DataRow row = data.Tables["img_modulo"].NewRow();
            row[0] = constants.imageToByte(gm_2);
            data.Tables["img_modulo"].Rows.Add(row);          
            bm.Dispose();
            gm_2.Dispose();          
        }
        //

        //Copiar
        private void copiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView6.RowCount > 0)
            {
                dataGridView6.Rows.Add(dataGridView6.CurrentRow.Cells[0].Value.ToString(), dataGridView6.CurrentRow.Cells[1].Value.ToString(), dataGridView6.CurrentRow.Cells[2].Value.ToString(), dataGridView6.CurrentRow.Cells[3].Value.ToString(), 1, "", checkBox2.Checked ? "" : "0");
                countItems();
                tabControl1.SelectedTab = tabPage4;
            }
        }

        //Copiar articulos
        private void copiarArticulosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                copiarArticulos(constants.stringToInt(datagridviewNE1.CurrentRow.Cells[0].Value.ToString()));
            }
        }

        //Eliminar Todos
        private void eliminarTodosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView6.RowCount > 0)
            {
                dataGridView6.Rows.Clear();
                dataGridView6.Refresh();
                countItems();
                tableLayoutPanel1.BackColor = Color.LightBlue;
                for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
                {
                    tableLayoutPanel1.Controls[i].BackColor = Color.LightBlue;
                }
            }
        }

        //Buscar esquema de diseño por grupo
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            new sqlDateBaseManager().dropTableOnGridViewWithFilter(dataGridView2, "esquemas", "grupo", comboBox6.Text);
        }

        //Instrucciones disponibles
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox7.SelectedIndex == 0)
            {
                richTextBox3.Text = richTextBox3.Text + "#D,LIMITE(MIN-MAX),LARGO,ALTO,CLAVE(..:),CANTIDAD,COMPONENTE,UBICACIÓN(OPCIONAL),Z_LARGO(OPCIONAL),FLOOR(OPCIONAL[TRUE-FALSE])\n";
            }
            else if (comboBox7.SelectedIndex == 1)
            {
                richTextBox3.Text = richTextBox3.Text + "#C,CLAVE_X,CLAVE_Y,CANTIDAD_Y,COMPONENTE,CANTIDAD_Z(OPCIONAL),UBICACIÓN(OPCIONAL)\n";
            }
            else if (comboBox7.SelectedIndex == 2)
            {
                richTextBox3.Text = richTextBox3.Text + "#E,CLAVE_X(..:),CLAVE_Y,NUEVA_CLAVE_Y,COMPONENTE,CANTIDAD(OPCIONAL),UBICACIÓN(OPCIONAL)\n";
            }
            else if (comboBox7.SelectedIndex == 3)
            {
                richTextBox3.Text = richTextBox3.Text + "#F,CLAVE,CANTIDAD,ID_DISEÑO,COMPONENTE,UBICACIÓN(OPCIONAL)\n";
            }
            else if (comboBox7.SelectedIndex == 4)
            {
                richTextBox3.Text = richTextBox3.Text + "#G,CLAVE_X,CLAVE_Y,CANTIDAD_Y,NUEVA_CLAVE_Y,COMPONENTE,CANTIDAD_Z(OPCIONAL),UBICACIÓN(OPCIONAL),CAMBIAR_CANT_Y(OPCIONAL[TRUE-FALSE])\n";
            }
        }

        //Borrar instrucciones
        private void button15_Click(object sender, EventArgs e)
        {
            richTextBox3.Clear();
        }

        //analisis estructural
        private void button16_Click(object sender, EventArgs e)
        {
            if(datagridviewNE1.Rows.Count > 0)
            {
                if(backgroundWorker1.IsBusy == false)
                {
                    if(backgroundWorker3.IsBusy == false)
                    {
                        progressBar1.Visible = true;
                        progressBar1.Value = 0;
                        backgroundWorker3.RunWorkerAsync();
                    }
                }
            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            int rows = datagridviewNE1.RowCount;
            int c = 0;
            errors = 0;
            if (rows > 0)
            {
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    if (initAnalisisEstructural(x.Cells[4].Value.ToString(), x.Cells[5].Value.ToString(), x.Cells[6].Value.ToString(), x.Cells[7].Value.ToString()) == true)
                    {
                        x.DefaultCellStyle.BackColor = Color.Red;
                        errors++;
                    }
                    else
                    {
                        x.DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    c++;
                    backgroundWorker3.ReportProgress((c * 100) / rows);
                }
            }
        }

        private void BackgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label21.Text = "Se encontraron: " + datagridviewNE1.RowCount + " módulos / con: " + errors + " errores estructurales.";
            progressBar1.Visible = false;
            progressBar1.Value = 100;
        }

        private void BackgroundWorker3_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }   

        private bool initAnalisisEstructural(string cristales, string aluminio, string herrajes, string otros)
        {
            bool error = false;
            listas_entities_pva listas = new listas_entities_pva();
            string[] componentes = cristales.Split(',');
            string[] clave = null;
            string z = string.Empty;

            foreach (string x in componentes)
            {
                if (x != "")
                {
                    clave = x.Split(':');

                    if (clave.Length > 0)
                    {
                        z = clave[0];
                        var cristal = (from v in listas.lista_costo_corte_e_instalado where v.clave == z select v).SingleOrDefault();

                        if (cristal == null)
                        {
                            error = true;
                        }
                    }
                }
            }

            componentes = aluminio.Split(',');

            foreach (string x in componentes)
            {
                if (x != "")
                {
                    clave = x.Split(':');

                    if (clave.Length > 0)
                    {
                        z = clave[0];
                        var perfil = (from v in listas.perfiles where v.clave == z select v).SingleOrDefault();

                        if (perfil == null)
                        {
                            error = true;
                        }
                    }
                }
            }

            componentes = herrajes.Split(',');

            foreach (string x in componentes)
            {
                if (x != "")
                {
                    clave = x.Split(':');

                    if (clave.Length > 0)
                    {
                        z = clave[0];
                        var herraje = (from v in listas.herrajes where v.clave == z select v).SingleOrDefault();

                        if (herraje == null)
                        {
                            error = true;
                        }
                    }
                }
            }

            componentes = otros.Split(',');

            foreach (string x in componentes)
            {
                if (x != "")
                {
                    clave = x.Split(':');

                    if (clave.Length > 0)
                    {
                        z = clave[0];
                        var otro = (from v in listas.otros where v.clave == z select v).SingleOrDefault();

                        if (otro == null)
                        {
                            error = true;
                        }
                    }
                }
            }
            return error;
        }

        //mosquitero
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                textBox13.Text = "S/M";
            }
            else
            {
                textBox13.Text = "";
                checkBox4.Checked = false;
            }
        }

        //incluir mosquitero
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                if (checkBox5.Checked)
                {
                    textBox13.Text = "C/M";
                }
            }
            else
            {
                if (checkBox5.Checked)
                {
                    textBox13.Text = "S/M";
                }
            }
        }

        private void CheckBox4_Click(object sender, EventArgs e)
        {
            if (!checkBox5.Checked)
            {
                checkBox4.Checked = false;
            }
        }
        //

        //Boton organizar
        private void button7_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                contextMenuStrip6.Show(MousePosition);
            }
            else
            {
                loadArticulosModulo();
                var data = (from x in articulos.modulos_articulos orderby x.componente descending select x);

                if (data != null)
                {
                    foreach (var c in data)
                    {
                        dataGridView6.Rows.Add(c.componente, c.id_articulo, c.clave, c.articulo, c.cantidad, c.ubicacion, c.seccion);
                    }
                    setColors();
                }
            }
        }
        //
    }
}
