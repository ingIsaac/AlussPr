using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;

namespace cristales_pva
{
    public partial class admin_panel : Form
    {
        int[] precios_costos = new int[] { 0, 5, 7, 8};
        int[] instalado = new int[] { 0, 5, 7, 8};
        int[] hojas = new int[] { 0, 4, 6, 7};
        int[] acabados = new int[] { 0, 4};
        int[] perfiles = new int[] { 0, 1, 18};
        int[] herrajes = new int[] { 0, 1, 8};
        int[] otros = new int[] { 0, 1, 10};
        int[] colores = new int[] { 0, 1, 6};
        List<int> find_next = new List<int>();

        // tablas
        System.Data.DataTable tem_table = new System.Data.DataTable();
        System.Data.DataTable data_column = new System.Data.DataTable();
        System.Data.DataTable new_data = new System.Data.DataTable();
        //

        //SQL object
        sqlDateBaseManager sql;
        //

        //Temporales
        Bitmap pic;
        List<int> precios_costos_list = new List<int>();
        List<int> instalado_list = new List<int>();
        List<int> hojas_list = new List<int>();
        List<int> acabados_list = new List<int>();
        List<int> perfiles_list = new List<int>();
        List<int> herrajes_list = new List<int>();
        List<int> otros_list = new List<int>();
        List<int> colores_list = new List<int>();
        //

        //Others
        int category_id = 0;
        int proveedor_id = 0;
        string value_filter = string.Empty;
        bool ordenado = false;
        bool permiso = false;
        //

        public void setPermiso(bool permiso)
        {
            this.permiso = permiso;
        }

        public admin_panel()
        {
            InitializeComponent();
            this.KeyPreview = true;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker3.WorkerReportsProgress = true;

            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            backgroundWorker2.ProgressChanged += BackgroundWorker2_ProgressChanged;
            backgroundWorker2.RunWorkerCompleted += BackgroundWorker2_RunWorkerCompleted;

            //datagridview_1 events
            datagridviewNE1.CellClick += datagridviewNE1_CellClick;
            datagridviewNE1.CellLeave += datagridviewNE1_CellLeave;
            datagridviewNE1.CellContextMenuStripNeeded += datagridviewNE1_CellContextMenuStripNeeded;
            datagridviewNE1.DataError += DatagridviewNE1_DataError;
            datagridviewNE1.EditingControlShowing += DatagridviewNE1_EditingControlShowing;
            //

            backgroundWorker3.ProgressChanged += BackgroundWorker3_ProgressChanged;
            backgroundWorker3.RunWorkerCompleted += BackgroundWorker3_RunWorkerCompleted;
            backgroundWorker4.ProgressChanged += BackgroundWorker4_ProgressChanged;
            backgroundWorker4.RunWorkerCompleted += BackgroundWorker4_RunWorkerCompleted;
            dataGridView4.CellContextMenuStripNeeded += DataGridView4_CellContextMenuStripNeeded;
            dataGridView5.CellContextMenuStripNeeded += DataGridView5_CellContextMenuStripNeeded;
            dataGridView6.EditingControlShowing += DataGridView6_EditingControlShowing;
            dataGridView6.CellClick += DataGridView6_CellClick;
            tabPage2.Enter += TabPage2_Enter;
            tabPage3.Enter += TabPage3_Enter;
            textBox4.KeyDown += TextBox4_KeyDown;
            //columnas tabla_temporal tem_table            
            tem_table.Columns.Add("clave");
            tem_table.Columns.Add("articulo");
            tem_table.Columns.Add("linea");
            tem_table.Columns.Add("lista");
            tem_table.Columns.Add("fecha");
            //-------

            //columna tabla de %
            data_column.Columns.Add("data");
            //-------                 
        }

        private void TextBox4_KeyDown(object sender, KeyEventArgs e)
        {
            bool found = false;
            if(e.KeyData == Keys.Enter)
            {
                if (textBox4.Text != "" && comboBox5.Text != "")
                {
                    find_next.Clear();
                    foreach (DataGridViewRow x in datagridviewNE1.Rows)
                    {
                        foreach (DataGridViewCell v in x.Cells)
                        {
                            if (v.OwningColumn.HeaderText == comboBox5.Text)
                            {
                                if (v.Value.ToString().Equals(textBox4.Text) == true)
                                {
                                    v.Selected = true;
                                    found = true;
                                    datagridviewNE1.FirstDisplayedScrollingRowIndex = x.Index;
                                    find_next.Add(x.Index);
                                    break;
                                }
                                else if (v.Value.ToString().StartsWith(textBox4.Text) == true)
                                {
                                    v.Selected = true;
                                    found = true;
                                    datagridviewNE1.FirstDisplayedScrollingRowIndex = x.Index;
                                    find_next.Add(x.Index);
                                    break;
                                }
                                else
                                {
                                    v.Selected = false;
                                }
                            }
                        }
                        if(found == true)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    datagridviewNE1.ClearSelection();
                }
            }
        }

        //Set ONLY articulos to uppercase
        private void DatagridviewNE1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (datagridviewNE1.CurrentCell.OwningColumn.HeaderText == "articulo")
            {
                if (e.Control is TextBox)
                {
                    ((TextBox)(e.Control)).CharacterCasing = CharacterCasing.Upper;
                }
            }
            else
            {
                if (e.Control is TextBox)
                {
                    ((TextBox)(e.Control)).CharacterCasing = CharacterCasing.Normal;
                }
            }
        }
        //

        //set visto
        private void setVisto(bool visto, int row, int cell)
        {
            if(visto == true)
            {
                datagridviewNE1.Rows[row].Cells[cell].Style.BackColor = Color.Yellow;
            }
            else
            {
                datagridviewNE1.Rows[row].Cells[cell].Style.BackColor = Color.LightGreen;
            }
        }

        private void DatagridviewNE1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if(e.Exception is FormatException)
            {
                MessageBox.Show("[Error] dato no válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(e.Exception is ArgumentException)
            {
                MessageBox.Show("[Error] argumento de lista no válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                datagridviewNE1[e.ColumnIndex, e.RowIndex].Value = "";
            }
        }

        private void DataGridView6_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView6.CurrentRow.DefaultCellStyle.BackColor == Color.Red)
            {
                dataGridView6.CurrentRow.DefaultCellStyle.BackColor = SystemColors.Window;
            }
        }

        //New item uppercase
        private void DataGridView6_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if(e.Control is TextBox)
            {
                ((TextBox)(e.Control)).CharacterCasing = CharacterCasing.Upper;
            }
        }
        //

        private void TabPage3_Enter(object sender, EventArgs e)
        {
            new sqlDateBaseManager().dropTableOnGridView(dataGridView3, "historial_login");
        }

        private void TabPage2_Enter(object sender, EventArgs e)
        {
            new sqlDateBaseManager().dropTableOnGridView(dataGridView2, "historial_actualizacion");
        }

        //tira de color en grid....
        private void datagridviewNE1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                if (backgroundWorker1.IsBusy == false)
                {
                    foreach (DataGridViewCell x in datagridviewNE1.CurrentRow.Cells)
                    {
                        if (x.ReadOnly == false)
                        {
                            x.Style.BackColor = Color.White;
                        }
                    }
                }
            }
        }

        private void datagridviewNE1_CellClick(object sender, DataGridViewCellEventArgs e)
        {           
            if (datagridviewNE1.Rows.Count > 0)
            {
                if (backgroundWorker1.IsBusy == false)
                {
                    //---> importante
                    if (datagridviewNE1.CurrentCell.Value == null)
                    {
                        datagridviewNE1.CurrentCell.Value = "";
                    }
                    //
                    label11.Text = datagridviewNE1.CurrentRow.Cells[0].Value.ToString();
                    if(comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
                    {
                        constants.setImage(constants.getArticuloProveedorCristales(label11.Text),label11.Text, "png", pictureBox1);
                    }
                    else if(comboBox1.SelectedIndex == 4)
                    {
                        constants.setImage(constants.getArticuloProveedorPerfiles(constants.stringToInt(label11.Text)), label11.Text, "png", pictureBox1);
                    }
                    else if (comboBox1.SelectedIndex == 5)
                    {
                        constants.setImage(constants.getArticuloProveedorHerrajes(constants.stringToInt(label11.Text)), label11.Text, "png", pictureBox1);
                    }
                    else if (comboBox1.SelectedIndex == 6)
                    {
                        constants.setImage(constants.getArticuloProveedorOtros(constants.stringToInt(label11.Text)), label11.Text, "png", pictureBox1);
                    }
                    else if (comboBox1.SelectedIndex == 7)
                    {
                        constants.setImage("acabados_especiales", label12.Text, "jpg", pictureBox1);                        
                    }
                    foreach (DataGridViewCell x in datagridviewNE1.CurrentRow.Cells)
                    {
                        if (x.ReadOnly == false)
                        {
                            x.Style.BackColor = Color.LightGray;
                        }
                        if(x.OwningColumn.HeaderText == "clave")
                        {
                            label12.Text = x.Value.ToString();
                        }
                        if (x.OwningColumn.HeaderText == "articulo")
                        {
                            label14.Text = x.Value.ToString();
                        }
                    }
                    if (datagridviewNE1.CurrentCell.OwningColumn.HeaderText == "proveedor")
                    {                         
                        DataGridViewComboBoxCell cb = new DataGridViewComboBoxCell();
                        cb.Sorted = true;
                        string u = string.Empty;
                        cb.Items.Clear();
                        switch (comboBox1.SelectedIndex)
                        {
                            case 0:
                                cb.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                                break;
                            case 1:
                                cb.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                                break;
                            case 2:
                                cb.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                                break;
                            case 4:
                                cb.Items.AddRange(constants.getProveedores("aluminio").ToArray());
                                break;
                            case 5:
                                cb.Items.AddRange(constants.getProveedores("herraje").ToArray());
                                break;
                            case 6:
                                cb.Items.AddRange(constants.getProveedores("otros").ToArray());
                                break;
                            case 7:
                                cb.Items.AddRange(constants.getProveedores("colores_aluminio").ToArray());
                                break;
                            default: break;
                        }                        
                        foreach(string x in cb.Items)
                        {
                            if(x == datagridviewNE1.CurrentCell.Value.ToString())
                            {
                                u = datagridviewNE1.CurrentCell.Value.ToString();
                            }
                        }
                        if (u == string.Empty)
                        {
                            datagridviewNE1.CurrentCell.Value = "";
                        }
                        cb.Value = u;
                        datagridviewNE1.CurrentRow.Cells[datagridviewNE1.CurrentCell.ColumnIndex] = cb;
                        cb.Dispose();                          
                    }
                    if (datagridviewNE1.CurrentCell.OwningColumn.HeaderText == "linea")
                    {                      
                        DataGridViewComboBoxCell cb = new DataGridViewComboBoxCell();
                        cb.Sorted = true;
                        string u = string.Empty;
                        cb.Items.Clear();
                        switch (comboBox1.SelectedIndex)
                        {
                            case 0:
                                cb.Items.AddRange(constants.getCategorias("vidrio").ToArray());
                                break;
                            case 1:
                                cb.Items.AddRange(constants.getCategorias("vidrio").ToArray());
                                break;
                            case 2:
                                cb.Items.AddRange(constants.getCategorias("vidrio").ToArray());
                                break;
                            case 4:
                                cb.Items.AddRange(constants.getCategorias("aluminio").ToArray());
                                break;
                            case 5:
                                cb.Items.AddRange(constants.getCategorias("herraje").ToArray());
                                break;
                            case 6:
                                cb.Items.AddRange(constants.getCategorias("otros").ToArray());
                                break;
                            default: break;
                        }
                        foreach(string x in cb.Items)
                        {
                            if(x == datagridviewNE1.CurrentCell.Value.ToString())
                            {
                                u = datagridviewNE1.CurrentCell.Value.ToString();
                            }
                        }
                        if (u == string.Empty)
                        {
                            datagridviewNE1.CurrentCell.Value = "";
                        }
                        cb.Value = u;
                        datagridviewNE1.CurrentRow.Cells[datagridviewNE1.CurrentCell.ColumnIndex] = cb;
                        cb.Dispose();                         
                    }
                }
            }           
        }
        //ends tira de color en grid....

        private void admin_panel_Load(object sender, EventArgs e)
        {
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Maximum = 100;
            checkBox1.Checked = true;           
        }

        //proceso de update ---------------------------------------------------------------------------------------------------------------------------
        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar1.Value = 100;
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel2.Text = "";
            toolStripStatusLabel1.Text = "Listo!";
            ((Control)tabPage1).Enabled = true;
        }

        private void BackgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
            toolStripStatusLabel2.Text = " " + e.ProgressPercentage + "%";
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {           
            if (sql.setServerConnection() == true)
            {
                backgroundWorker2.ReportProgress(0);
                subirLista();
            }
            else
            {
                toolStripStatusLabel1.Text = "(Sin Conexión)";
            }
        }

        private void subirLista()
        {
            if (comboBox1.SelectedIndex == 0)
            {
                for (int v = 0; v < precios_costos_list.Count; v++)
                {
                    sql.updateListaCosto(datagridviewNE1.Rows[precios_costos_list[v]].Cells[0].Value.ToString(), datagridviewNE1.Rows[precios_costos_list[v]].Cells[1].Value.ToString(), constants.stringToFloat(datagridviewNE1.Rows[precios_costos_list[v]].Cells[2].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[precios_costos_list[v]].Cells[3].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[precios_costos_list[v]].Cells[4].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[precios_costos_list[v]].Cells[5].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[precios_costos_list[v]].Cells[6].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[precios_costos_list[v]].Cells[7].Value.ToString()), datagridviewNE1.Rows[precios_costos_list[v]].Cells[8].Value.ToString(), datagridviewNE1.Rows[precios_costos_list[v]].Cells[9].Value.ToString());
                    setVisto(false, precios_costos_list[v], 8);
                    backgroundWorker2.ReportProgress(((v + 1) * 100) / precios_costos_list.Count);
                    toolStripStatusLabel1.Text = "Subiendo Registro #" + (v + 1) + " de " + precios_costos_list.Count;                  
                }
                if (checkBox3.Checked == true)
                {
                    toolStripStatusLabel1.Text = "Actualizando Historial...";
                    crearHistorial();
                }
                tem_table.Clear();
                precios_costos_list.Clear();
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                for (int v = 0; v < instalado_list.Count; v++)
                {
                    sql.updateListaInstalado(datagridviewNE1.Rows[instalado_list[v]].Cells[0].Value.ToString(), datagridviewNE1.Rows[instalado_list[v]].Cells[1].Value.ToString(), constants.stringToFloat(datagridviewNE1.Rows[instalado_list[v]].Cells[2].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[instalado_list[v]].Cells[3].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[instalado_list[v]].Cells[4].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[instalado_list[v]].Cells[5].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[instalado_list[v]].Cells[6].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[instalado_list[v]].Cells[7].Value.ToString()), datagridviewNE1.Rows[instalado_list[v]].Cells[8].Value.ToString(), datagridviewNE1.Rows[instalado_list[v]].Cells[9].Value.ToString());
                    setVisto(false, instalado_list[v], 8);
                    backgroundWorker2.ReportProgress(((v + 1) * 100) / instalado_list.Count);
                    toolStripStatusLabel1.Text = "Subiendo Registro #" + (v + 1) + " de " + instalado_list.Count;                  
                }
                if (checkBox3.Checked == true)
                {
                    toolStripStatusLabel1.Text = "Actualizando Historial...";
                    crearHistorial();
                }
                tem_table.Clear();
                instalado_list.Clear();
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                for (int v = 0; v < hojas_list.Count; v++)
                {
                    sql.updateListaHojas(datagridviewNE1.Rows[hojas_list[v]].Cells[0].Value.ToString(), datagridviewNE1.Rows[hojas_list[v]].Cells[1].Value.ToString(), constants.stringToFloat(datagridviewNE1.Rows[hojas_list[v]].Cells[2].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[hojas_list[v]].Cells[3].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[hojas_list[v]].Cells[4].Value.ToString()), stringToFloat(datagridviewNE1.Rows[hojas_list[v]].Cells[5].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[hojas_list[v]].Cells[6].Value.ToString()), datagridviewNE1.Rows[hojas_list[v]].Cells[7].Value.ToString(), datagridviewNE1.Rows[hojas_list[v]].Cells[8].Value.ToString());
                    setVisto(false, hojas_list[v], 7);
                    backgroundWorker2.ReportProgress(((v + 1) * 100) / hojas_list.Count);
                    toolStripStatusLabel1.Text = "Subiendo Registro #" + (v + 1) + " de " + hojas_list.Count;                   
                }
                if (checkBox3.Checked == true)
                {
                    toolStripStatusLabel1.Text = "Actualizando Historial...";
                    crearHistorial();
                }
                tem_table.Clear();
                hojas_list.Clear();
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                for (int v = 0; v < acabados_list.Count; v++)
                {
                    sql.updateListaAcabados(datagridviewNE1.Rows[acabados_list[v]].Cells[0].Value.ToString(), datagridviewNE1.Rows[acabados_list[v]].Cells[1].Value.ToString(), constants.stringToFloat(datagridviewNE1.Rows[acabados_list[v]].Cells[2].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[acabados_list[v]].Cells[3].Value.ToString()), datagridviewNE1.Rows[acabados_list[v]].Cells[4].Value.ToString());
                    setVisto(false, acabados_list[v], 4);
                    backgroundWorker2.ReportProgress(((v + 1) * 100) / acabados_list.Count);
                    toolStripStatusLabel1.Text = "Subiendo Registro #" + (v + 1) + " de " + acabados_list.Count;                  
                }
                if (checkBox3.Checked == true)
                {
                    toolStripStatusLabel1.Text = "Actualizando Historial...";
                    crearHistorial();
                }
                tem_table.Clear();
                acabados_list.Clear();
            }
            else if (comboBox1.SelectedIndex == 4)
            {
                for (int v = 0; v < perfiles_list.Count; v++)
                {
                    sql.updateListaAluminio((int)datagridviewNE1.Rows[perfiles_list[v]].Cells[0].Value, datagridviewNE1.Rows[perfiles_list[v]].Cells[2].Value.ToString(), datagridviewNE1.Rows[perfiles_list[v]].Cells[3].Value.ToString(), datagridviewNE1.Rows[perfiles_list[v]].Cells[4].Value.ToString(), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[5].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[6].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[7].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[8].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[9].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[10].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[11].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[12].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[13].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[14].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[15].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[16].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[17].Value.ToString()), datagridviewNE1.Rows[perfiles_list[v]].Cells[18].Value.ToString(), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[19].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[20].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[perfiles_list[v]].Cells[21].Value.ToString()));
                    setVisto(false, perfiles_list[v], 18);
                    backgroundWorker2.ReportProgress(((v + 1) * 100) / perfiles_list.Count);
                    toolStripStatusLabel1.Text = "Subiendo Registro #" + (v + 1) + " de " + perfiles_list.Count;                  
                }
                if (checkBox3.Checked == true)
                {
                    toolStripStatusLabel1.Text = "Actualizando Historial...";
                    crearHistorial();
                }
                tem_table.Clear();
                perfiles_list.Clear();
            }
            else if (comboBox1.SelectedIndex == 5)
            {
                for (int v = 0; v < herrajes_list.Count; v++)
                {
                    sql.updateListaHerrajes((int)datagridviewNE1.Rows[herrajes_list[v]].Cells[0].Value, datagridviewNE1.Rows[herrajes_list[v]].Cells[2].Value.ToString(), datagridviewNE1.Rows[herrajes_list[v]].Cells[3].Value.ToString(), datagridviewNE1.Rows[herrajes_list[v]].Cells[4].Value.ToString(), datagridviewNE1.Rows[herrajes_list[v]].Cells[5].Value.ToString(), datagridviewNE1.Rows[herrajes_list[v]].Cells[6].Value.ToString(), constants.stringToFloat(datagridviewNE1.Rows[herrajes_list[v]].Cells[7].Value.ToString()), datagridviewNE1.Rows[herrajes_list[v]].Cells[8].Value.ToString());
                    setVisto(false, herrajes_list[v], 8);
                    backgroundWorker2.ReportProgress(((v + 1) * 100) / herrajes_list.Count);
                    toolStripStatusLabel1.Text = "Subiendo Registro #" + (v + 1) + " de " + herrajes_list.Count;                   
                }
                if (checkBox3.Checked == true)
                {
                    toolStripStatusLabel1.Text = "Actualizando Historial...";
                    crearHistorial();
                }
                tem_table.Clear();
                herrajes_list.Clear();
            }
            else if (comboBox1.SelectedIndex == 6)
            {
                for (int v = 0; v < otros_list.Count; v++)
                {
                    sql.updateListaOtros((int)datagridviewNE1.Rows[otros_list[v]].Cells[0].Value, datagridviewNE1.Rows[otros_list[v]].Cells[2].Value.ToString(), datagridviewNE1.Rows[otros_list[v]].Cells[3].Value.ToString(), datagridviewNE1.Rows[otros_list[v]].Cells[4].Value.ToString(), datagridviewNE1.Rows[otros_list[v]].Cells[5].Value.ToString(), datagridviewNE1.Rows[otros_list[v]].Cells[6].Value.ToString(), constants.stringToFloat(datagridviewNE1.Rows[otros_list[v]].Cells[9].Value.ToString()), datagridviewNE1.Rows[otros_list[v]].Cells[10].Value.ToString(), constants.stringToFloat(datagridviewNE1.Rows[otros_list[v]].Cells[7].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[otros_list[v]].Cells[8].Value.ToString()));
                    setVisto(false, otros_list[v], 10);
                    backgroundWorker2.ReportProgress(((v + 1) * 100) / otros_list.Count);
                    toolStripStatusLabel1.Text = "Subiendo Registro #" + (v + 1) + " de " + otros_list.Count;
                }
                if (checkBox3.Checked == true)
                {
                    toolStripStatusLabel1.Text = "Actualizando Historial...";
                    crearHistorial();
                }
                tem_table.Clear();
                otros_list.Clear();
            }
            else if (comboBox1.SelectedIndex == 7)
            {
                for (int v = 0; v < colores_list.Count; v++)
                {
                    sql.updateListaColores((int)datagridviewNE1.Rows[colores_list[v]].Cells[0].Value, datagridviewNE1.Rows[colores_list[v]].Cells[2].Value.ToString(), datagridviewNE1.Rows[colores_list[v]].Cells[3].Value.ToString(), constants.stringToFloat(datagridviewNE1.Rows[colores_list[v]].Cells[4].Value.ToString()), constants.stringToFloat(datagridviewNE1.Rows[colores_list[v]].Cells[5].Value.ToString()), datagridviewNE1.Rows[colores_list[v]].Cells[6].Value.ToString());
                    setVisto(false, colores_list[v], 6);
                    backgroundWorker2.ReportProgress(((v + 1) * 100) / colores_list.Count);
                    toolStripStatusLabel1.Text = "Subiendo Registro #" + (v + 1) + " de " + colores_list.Count;
                }
                if (checkBox3.Checked == true)
                {
                    toolStripStatusLabel1.Text = "Actualizando Historial...";
                    crearHistorial();
                }
                tem_table.Clear();
                otros_list.Clear();
            }
        }
        
        //end proceso de update ------------------------------------------------------------------------------------------------------------------------------------      
        private void descargarListaUpdate(DataGridView table)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                precios_costos_list.Clear();
                if (ordenado == false)
                {
                    sql.dropTableOnGridView(table, "costo_corte_precio");
                }
                else
                {
                    if (comboBox8.Text != "")
                    {
                        sql.dropTableOnGridView(table, "costo_corte_precio", true, "articulo", value_filter);                       
                    }
                    else if (comboBox9.Text != "")
                    {
                        sql.dropTableOnGridView(table, "costo_corte_precio", true, "proveedor", value_filter);                      
                    }
                }
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                instalado_list.Clear();
                if (ordenado == false)
                {
                    sql.dropTableOnGridView(table, "instalado");
                }
                else
                {
                    if (comboBox8.Text != "")
                    {
                        sql.dropTableOnGridView(table, "instalado", true, "articulo", value_filter);
                    }
                    else if (comboBox9.Text != "")
                    {
                        sql.dropTableOnGridView(table, "instalado", true, "proveedor", value_filter);
                    }
                }
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                hojas_list.Clear();
                if (ordenado == false)
                {
                    sql.dropTableOnGridView(table, "hojas");
                }
                else
                {
                    if (comboBox8.Text != "")
                    {
                        sql.dropTableOnGridView(table, "hojas", true, "articulo", value_filter);
                    }
                    else if (comboBox9.Text != "")
                    {
                        sql.dropTableOnGridView(table, "hojas", true, "proveedor", value_filter);
                    }
                }
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                acabados_list.Clear();
                sql.dropTableOnGridView(table, "acabados");
            }
            else if (comboBox1.SelectedIndex == 4)
            {
                perfiles_list.Clear();
                if (ordenado == false)
                {
                    sql.dropTableOnGridView(table, "perfiles");
                }
                else
                {
                    if (comboBox8.Text != "")
                    {
                        sql.dropTableOnGridView(table, "perfiles", true, "linea", value_filter);
                    }
                    else if (comboBox9.Text != "")
                    {
                        sql.dropTableOnGridView(table, "perfiles", true, "proveedor", value_filter);
                    }
                }
            }
            else if (comboBox1.SelectedIndex == 5)
            {
                herrajes_list.Clear();
                if (ordenado == false)
                {
                    sql.dropTableOnGridView(table, "herrajes");
                }
                else
                {
                    if (comboBox8.Text != "")
                    {
                        sql.dropTableOnGridView(table, "herrajes", true, "linea", value_filter);
                    }
                    else if (comboBox9.Text != "")
                    {
                        sql.dropTableOnGridView(table, "herrajes", true, "proveedor", value_filter);
                    }
                }
            }
            else if (comboBox1.SelectedIndex == 6)
            {
                otros_list.Clear();
                if (ordenado == false)
                {
                    sql.dropTableOnGridView(table, "otros");
                }
                else
                {
                    if (comboBox8.Text != "")
                    {
                        sql.dropTableOnGridView(table, "otros", true, "linea", value_filter);
                    }
                    else if(comboBox9.Text != "")
                    {
                        sql.dropTableOnGridView(table, "otros", true, "proveedor", value_filter);
                    }
                }
            }
            else if (comboBox1.SelectedIndex == 7)
            {
                colores_list.Clear();
                if (ordenado == false)
                {
                    sql.dropTableOnGridView(table, "colores_aluminio");
                }
                else
                {
                    if (comboBox9.Text != "")
                    {
                        sql.dropTableOnGridView(table, "colores_aluminio", true, "proveedor", value_filter);
                    }
                }
            }           
        }
        //ends proceso de descarga ----------------------------------------------------------------------------------------------------------------------

        //Ejecuta la descarga de la base de datos
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label11.Text = string.Empty;
            label12.Text = string.Empty;
            label14.Text = string.Empty;
            comboBox2.Text = "";
            comboBox6.Text = "";
            comboBox7.Text = "";
            pictureBox1.Image = null;
            pic = null;
            executeLoad();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sql.setServerConnection() == true)
            {
                datagridviewNE1.DataSource = null;
                //descarga los datos desde el sql server
                toolStripStatusLabel1.Text = "Descargando Datos...";
                backgroundWorker1.ReportProgress(0);
                descargarListaUpdate(datagridviewNE1);
                backgroundWorker1.ReportProgress(75);
                //Actualiza los calculos de las listas
                toolStripStatusLabel1.Text = "Actualizando Resultados...";
                refreshCalculos();
                backgroundWorker1.ReportProgress(85);
                //configura la tabla (readOnly, colors)
                toolStripStatusLabel1.Text = "Configurando Tabla...";
                clearDatagridConfig(datagridviewNE1);
                setDatagridConfig();
                backgroundWorker1.ReportProgress(100);
            }
            else
            {
                toolStripStatusLabel1.Text = "(Sin Conexión)";
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabel1.Text = "Listo!";
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = "";
            toolStripStatusLabel2.Text = "";
            ((Control)tabPage1).Enabled = true;
            label22.Text = "# Registros en " + comboBox1.Text + " : (" + datagridviewNE1.RowCount + ")";
            if (datagridviewNE1.ColumnCount > 0)
            {
                try
                {
                    string[] lista = new string[datagridviewNE1.ColumnCount];
                    comboBox2.Items.Clear();
                    for (int i = 0; i < datagridviewNE1.ColumnCount; i++)
                    {
                        lista[i] = datagridviewNE1.Columns[i].HeaderText;
                    }
                    comboBox2.Items.AddRange(lista);
                    comboBox6.Items.Clear();
                    comboBox7.Items.Clear();
                    comboBox8.Items.Clear();
                    comboBox9.Items.Clear();
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            comboBox6.Items.AddRange(constants.getCategorias("vidrio").ToArray());
                            comboBox7.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                            comboBox8.Items.AddRange(constants.getCategorias("vidrio").ToArray());
                            comboBox9.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                            break;
                        case 1:
                            comboBox6.Items.AddRange(constants.getCategorias("vidrio").ToArray());
                            comboBox7.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                            comboBox8.Items.AddRange(constants.getCategorias("vidrio").ToArray());
                            comboBox9.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                            break;
                        case 2:
                            comboBox6.Items.AddRange(constants.getCategorias("vidrio").ToArray());
                            comboBox7.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                            comboBox8.Items.AddRange(constants.getCategorias("vidrio").ToArray());
                            comboBox9.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                            break;
                        case 4:
                            comboBox6.Items.AddRange(constants.getCategorias("aluminio").ToArray());
                            comboBox7.Items.AddRange(constants.getProveedores("aluminio").ToArray());
                            comboBox8.Items.AddRange(constants.getCategorias("aluminio").ToArray());
                            comboBox9.Items.AddRange(constants.getProveedores("aluminio").ToArray());
                            break;
                        case 5:
                            comboBox6.Items.AddRange(constants.getCategorias("herraje").ToArray());
                            comboBox7.Items.AddRange(constants.getProveedores("herraje").ToArray());
                            comboBox8.Items.AddRange(constants.getCategorias("herraje").ToArray());
                            comboBox9.Items.AddRange(constants.getProveedores("herraje").ToArray());
                            break;
                        case 6:
                            comboBox6.Items.AddRange(constants.getCategorias("otros").ToArray());
                            comboBox7.Items.AddRange(constants.getProveedores("otros").ToArray());
                            comboBox8.Items.AddRange(constants.getCategorias("otros").ToArray());
                            comboBox9.Items.AddRange(constants.getProveedores("otros").ToArray());
                            break;
                        case 7:
                            comboBox7.Items.AddRange(constants.getProveedores("colores_aluminio").ToArray());
                            comboBox9.Items.AddRange(constants.getProveedores("colores_aluminio").ToArray());
                            break;
                        default: break;
                    }                  
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                }
            }
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
            toolStripStatusLabel2.Text = " " + e.ProgressPercentage + "%";
        }

        private void executeLoad()
        {
            if (constants.local == false)
            {
                if (backgroundWorker2.IsBusy == false)
                {
                    if (backgroundWorker1.IsBusy == false)
                    {
                        sql = new sqlDateBaseManager();
                        toolStripProgressBar1.Visible = true;
                        ((Control)tabPage1).Enabled = false;
                        tem_table.Clear();
                        toolStripStatusLabel1.Text = "Conectando...";
                        toolStripProgressBar1.Value = 0;
                        ordenado = false;
                        comboBox8.SelectedIndex = -1;
                        comboBox9.SelectedIndex = -1;
                        backgroundWorker1.RunWorkerAsync();                 
                    }
                }
                else
                {
                    MessageBox.Show("[Error] no se puede ejecutar este proceso mientras se esta actualizando una lista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible conectarse al servidor.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //termina la ejecucion

        //Nueva config del grid
        private void setDatagridConfig()
        {
            if (comboBox1.SelectedIndex == 0)
            {
                for (int i = 0; i < precios_costos.Length; i++)
                {
                    datagridviewNE1.Columns[precios_costos[i]].ReadOnly = true;
                    datagridviewNE1.Columns[precios_costos[i]].DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                for (int i = 0; i < instalado.Length; i++)
                {
                    datagridviewNE1.Columns[instalado[i]].ReadOnly = true;
                    datagridviewNE1.Columns[instalado[i]].DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                for (int i = 0; i < hojas.Length; i++)
                {
                    datagridviewNE1.Columns[hojas[i]].ReadOnly = true;
                    datagridviewNE1.Columns[hojas[i]].DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                for (int i = 0; i < acabados.Length; i++)
                {
                    datagridviewNE1.Columns[acabados[i]].ReadOnly = true;
                    datagridviewNE1.Columns[acabados[i]].DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
            else if (comboBox1.SelectedIndex == 4)
            {
                for (int i = 0; i < perfiles.Length; i++)
                {
                    datagridviewNE1.Columns[perfiles[i]].ReadOnly = true;
                    datagridviewNE1.Columns[perfiles[i]].DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
            else if (comboBox1.SelectedIndex == 5)
            {
                for (int i = 0; i < herrajes.Length; i++)
                {
                    datagridviewNE1.Columns[herrajes[i]].ReadOnly = true;
                    datagridviewNE1.Columns[herrajes[i]].DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
            else if (comboBox1.SelectedIndex == 6)
            {
                for (int i = 0; i < otros.Length; i++)
                {
                    datagridviewNE1.Columns[otros[i]].ReadOnly = true;
                    datagridviewNE1.Columns[otros[i]].DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
            else if (comboBox1.SelectedIndex == 7)
            {
                for (int i = 0; i < colores.Length; i++)
                {
                    datagridviewNE1.Columns[colores[i]].ReadOnly = true;
                    datagridviewNE1.Columns[colores[i]].DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
        }
        //termina config de grid

        //borra config de grid
        private void clearDatagridConfig(DataGridView grid)
        {
            for(int i = 0; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].ReadOnly = false;
                grid.Columns[i].DefaultCellStyle.BackColor = Color.White;
            }
            grid.Columns["fecha"].DisplayIndex = grid.Columns.Count-1;
            if(comboBox1.SelectedIndex == 4)
            {
                grid.Columns["kg_peso_lineal"].DisplayIndex = 7;
            }      
        }
        //borra config de grid

        //edicion en el grid
        private void datagridviewNE1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                if (datagridviewNE1.CurrentCell.ReadOnly == false)
                {
                    try
                    {
                        if (comboBox1.SelectedIndex == 0)
                        {
                            string costo_vidrio = datagridviewNE1.CurrentRow.Cells[2].Value.ToString();
                            string p_desperdicio = datagridviewNE1.CurrentRow.Cells[3].Value.ToString();
                            string p_mano_obra = datagridviewNE1.CurrentRow.Cells[4].Value.ToString();

                            datagridviewNE1.CurrentRow.Cells[5].Value = getCostoCorte(stringToFloat(costo_vidrio), stringToFloat(p_desperdicio), stringToFloat(p_mano_obra));

                            string costo_corte = datagridviewNE1.CurrentRow.Cells[5].Value.ToString();
                            string p_utilidad = datagridviewNE1.CurrentRow.Cells[6].Value.ToString();

                            datagridviewNE1.CurrentRow.Cells[7].Value = getPrecioCorte(stringToFloat(costo_corte), stringToFloat(p_utilidad));
                            datagridviewNE1.CurrentRow.Cells[8].Value = DateTime.Today.ToString("dd/MM/yyyy");
                            //historial ---> Lista General de Costo Corte y Precio Venta
                            if (noRepeatIndex(datagridviewNE1.CurrentRow.Index, precios_costos_list) == false)
                            {
                                System.Data.DataRow row = tem_table.NewRow();
                                row[0] = datagridviewNE1.CurrentRow.Cells[0].Value.ToString();
                                row[1] = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                                row[2] = "n/a";
                                row[3] = comboBox1.Text;
                                row[4] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                tem_table.Rows.Add(row);
                                precios_costos_list.Add(datagridviewNE1.CurrentRow.Index);
                                setVisto(true, datagridviewNE1.CurrentRow.Index, 8);
                            }
                        }
                        else if (comboBox1.SelectedIndex == 1)
                        {
                            string mano_obra_insta = datagridviewNE1.CurrentRow.Cells[2].Value.ToString();
                            string materiales = datagridviewNE1.CurrentRow.Cells[3].Value.ToString();
                            string flete = datagridviewNE1.CurrentRow.Cells[4].Value.ToString();

                            datagridviewNE1.CurrentRow.Cells[5].Value = getCostoCorteInstalado(stringToFloat(sql.getSingleSQLValue("costo_corte_precio", "costo_corte", "clave", datagridviewNE1.CurrentRow.Cells[0].Value.ToString(), 0)), stringToFloat(mano_obra_insta), stringToFloat(materiales), stringToFloat(flete));

                            string utilidad_insta = datagridviewNE1.CurrentRow.Cells[6].Value.ToString();

                            datagridviewNE1.CurrentRow.Cells[7].Value = getPrecioCorte(stringToFloat(datagridviewNE1.CurrentRow.Cells[5].Value.ToString()), stringToFloat(utilidad_insta));
                            datagridviewNE1.CurrentRow.Cells[8].Value = DateTime.Today.ToString("dd/MM/yyyy");
                            //historial ---> Lista General de Costo y Precios de Instalado
                            if (noRepeatIndex(datagridviewNE1.CurrentRow.Index, instalado_list) == false)
                            {
                                System.Data.DataRow row = tem_table.NewRow();
                                row[0] = datagridviewNE1.CurrentRow.Cells[0].Value.ToString();
                                row[1] = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                                row[2] = "n/a";
                                row[3] = comboBox1.Text;
                                row[4] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                tem_table.Rows.Add(row);
                                instalado_list.Add(datagridviewNE1.CurrentRow.Index);
                                setVisto(true, datagridviewNE1.CurrentRow.Index, 8);
                            }
                        }
                        else if (comboBox1.SelectedIndex == 2)
                        {
                            string largo = datagridviewNE1.CurrentRow.Cells[2].Value.ToString();
                            string alto = datagridviewNE1.CurrentRow.Cells[3].Value.ToString();

                            datagridviewNE1.CurrentRow.Cells[4].Value = getCostoHoja(stringToFloat(sql.getSingleSQLValue("costo_corte_precio", "precio_proveedor", "clave", datagridviewNE1.CurrentRow.Cells[0].Value.ToString(), 0)), stringToFloat(largo), stringToFloat(alto));

                            string utilidad_hoja = datagridviewNE1.CurrentRow.Cells[5].Value.ToString();

                            datagridviewNE1.CurrentRow.Cells[6].Value = getPrecioHoja(stringToFloat(datagridviewNE1.CurrentRow.Cells[4].Value.ToString()), stringToFloat(utilidad_hoja));
                            datagridviewNE1.CurrentRow.Cells[7].Value = DateTime.Today.ToString("dd/MM/yyyy");
                            //historial ---> Lista General de Costo y Precios de Hojas
                            if (noRepeatIndex(datagridviewNE1.CurrentRow.Index, hojas_list) == false)
                            {
                                System.Data.DataRow row = tem_table.NewRow();
                                row[0] = datagridviewNE1.CurrentRow.Cells[0].Value.ToString();
                                row[1] = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                                row[2] = "n/a";
                                row[3] = comboBox1.Text;
                                row[4] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                tem_table.Rows.Add(row);
                                hojas_list.Add(datagridviewNE1.CurrentRow.Index);
                                setVisto(true, datagridviewNE1.CurrentRow.Index, 7);
                            }
                        }
                        else if (comboBox1.SelectedIndex == 3)
                        {
                            datagridviewNE1.CurrentRow.Cells[4].Value = DateTime.Today.ToString("dd/MM/yyyy");
                            //historial ---> Acabados
                            if (noRepeatIndex(datagridviewNE1.CurrentRow.Index, acabados_list) == false)
                            {
                                System.Data.DataRow row = tem_table.NewRow();
                                row[0] = datagridviewNE1.CurrentRow.Cells[0].Value.ToString();
                                row[1] = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                                row[2] = "n/a";
                                row[3] = comboBox1.Text;
                                row[4] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                tem_table.Rows.Add(row);
                                acabados_list.Add(datagridviewNE1.CurrentRow.Index);
                                setVisto(true, datagridviewNE1.CurrentRow.Index, 4);
                            }
                        }
                        else if (comboBox1.SelectedIndex == 4)
                        {
                            datagridviewNE1.CurrentRow.Cells[18].Value = DateTime.Today.ToString("dd/MM/yyyy");
                            //historial ---> Perfiles
                            if (noRepeatIndex(datagridviewNE1.CurrentRow.Index, perfiles_list) == false)
                            {
                                System.Data.DataRow row = tem_table.NewRow();
                                row[0] = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                                row[1] = datagridviewNE1.CurrentRow.Cells[2].Value.ToString();
                                row[2] = datagridviewNE1.CurrentRow.Cells[3].Value.ToString();
                                row[3] = comboBox1.Text;
                                row[4] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                tem_table.Rows.Add(row);
                                perfiles_list.Add(datagridviewNE1.CurrentRow.Index);
                                setVisto(true, datagridviewNE1.CurrentRow.Index, 18);
                            }
                        }
                        else if (comboBox1.SelectedIndex == 5)
                        {
                            datagridviewNE1.CurrentRow.Cells[8].Value = DateTime.Today.ToString("dd/MM/yyyy");
                            //historial ---> Herrajes
                            if (noRepeatIndex(datagridviewNE1.CurrentRow.Index, herrajes_list) == false)
                            {
                                System.Data.DataRow row = tem_table.NewRow();
                                row[0] = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                                row[1] = datagridviewNE1.CurrentRow.Cells[2].Value.ToString();
                                row[2] = datagridviewNE1.CurrentRow.Cells[3].Value.ToString();
                                row[3] = comboBox1.Text;
                                row[4] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                tem_table.Rows.Add(row);
                                herrajes_list.Add(datagridviewNE1.CurrentRow.Index);
                                setVisto(true, datagridviewNE1.CurrentRow.Index, 8);
                            }
                        }
                        else if (comboBox1.SelectedIndex == 6)
                        {
                            datagridviewNE1.CurrentRow.Cells[10].Value = DateTime.Today.ToString("dd/MM/yyyy");
                            //historial ---> Otros
                            if (noRepeatIndex(datagridviewNE1.CurrentRow.Index, otros_list) == false)
                            {
                                System.Data.DataRow row = tem_table.NewRow();
                                row[0] = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                                row[1] = datagridviewNE1.CurrentRow.Cells[2].Value.ToString();
                                row[2] = datagridviewNE1.CurrentRow.Cells[3].Value.ToString();
                                row[3] = comboBox1.Text;
                                row[4] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                tem_table.Rows.Add(row);
                                otros_list.Add(datagridviewNE1.CurrentRow.Index);
                                setVisto(true, datagridviewNE1.CurrentRow.Index, 10);
                            }
                        }
                        else if (comboBox1.SelectedIndex == 7)
                        {
                            datagridviewNE1.CurrentRow.Cells[6].Value = DateTime.Today.ToString("dd/MM/yyyy");
                            //historial ---> colores_aluminio
                            if (noRepeatIndex(datagridviewNE1.CurrentRow.Index, colores_list) == false)
                            {
                                System.Data.DataRow row = tem_table.NewRow();
                                row[0] = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                                row[1] = datagridviewNE1.CurrentRow.Cells[2].Value.ToString();
                                row[2] = "n/a";
                                row[3] = comboBox1.Text;
                                row[4] = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                tem_table.Rows.Add(row);
                                colores_list.Add(datagridviewNE1.CurrentRow.Index);
                                setVisto(true, datagridviewNE1.CurrentRow.Index, 6);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("[Error] dato ingresado no válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }                                         
        }

        private Boolean noRepeatIndex(int index, List<int> list)
        {
            Boolean r = false;
            foreach(int x in list)
            {
                if(x == index)
                {
                    r = true;
                    break;
                }
            }
            return r;
        }
        
        //termina edicion en el grid

        //refresh de calculos por lista en load--------------------------------------------------------------------------------------------------

        //refresh para todos los datos
        private void refreshCalculos()
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    RefreshPreciosCostosCalculos();
                    break;
                case 1:
                    RefreshInstaladoCalculos();
                    break;
                case 2:
                    RefreshHojasCalculos();
                    break;
                default:
                    break;
            }
        }
        // ---->

        private void RefreshPreciosCostosCalculos()
        {
            foreach (DataGridViewRow x in datagridviewNE1.Rows)
            {
                string costo_vidrio = x.Cells[2].Value.ToString();
                string p_desperdicio = x.Cells[3].Value.ToString();
                string p_mano_obra = x.Cells[4].Value.ToString();

                x.Cells[5].Value = getCostoCorte(stringToFloat(costo_vidrio), stringToFloat(p_desperdicio), stringToFloat(p_mano_obra));

                string costo_corte = x.Cells[5].Value.ToString();
                string p_utilidad = x.Cells[6].Value.ToString();
                x.Cells[7].Value = getPrecioCorte(stringToFloat(costo_corte), stringToFloat(p_utilidad));
            }                   
        }

        private void RefreshInstaladoCalculos()
        {
            foreach (DataGridViewRow x in datagridviewNE1.Rows)
            {
                string mano_obra_insta = x.Cells[2].Value.ToString();
                string materiales = x.Cells[3].Value.ToString();
                string flete = x.Cells[4].Value.ToString();

                x.Cells[5].Value = getCostoCorteInstalado(stringToFloat(sql.getSingleSQLValue("costo_corte_precio", "costo_corte", "clave", x.Cells[0].Value.ToString(), 0)), stringToFloat(mano_obra_insta), stringToFloat(materiales), stringToFloat(flete));

                string utilidad_insta = x.Cells[6].Value.ToString();

                x.Cells[7].Value = getPrecioCorte(stringToFloat(x.Cells[5].Value.ToString()), stringToFloat(utilidad_insta));
            }
        }

        private void RefreshHojasCalculos()
        {
            foreach (DataGridViewRow x in datagridviewNE1.Rows)
            {
                string largo = x.Cells[2].Value.ToString();
                string alto = x.Cells[3].Value.ToString();

               x.Cells[4].Value = getCostoHoja(stringToFloat(sql.getSingleSQLValue("costo_corte_precio", "precio_proveedor", "clave", x.Cells[0].Value.ToString(), 0)), stringToFloat(largo), stringToFloat(alto));

                string utilidad_hoja = x.Cells[5].Value.ToString();

                x.Cells[6].Value = getPrecioHoja(stringToFloat(x.Cells[4].Value.ToString()), stringToFloat(utilidad_hoja));
            }
        }
        //ends refresh de calculos por lista en load-----------------------------------------------------------------------------------------------------

        private float stringToFloat(string num)
        {
            float r = 0;
            if (float.TryParse(num, out r) == true)
            {
                return r;
            }
            else
            {
                return 0;
            }
        }

        //calculos dentro del grid------------------------------------------------------------------------------------------------------------------
        private double getPrecioCorte(float costo_corte, float p_utilidad)
        {
            try
            {
                return Math.Round((costo_corte * (p_utilidad + 1)), 2);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private double getCostoCorte(float costo_vidrio, float mano_obra, float desperdicio)
        {
            try {
                return Math.Round(costo_vidrio * (desperdicio + 1) * (mano_obra + 1), 2);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private double getCostoCorteInstalado(float costo_corte, float mano_obra_insta, float materiales, float flete)
        {
            try
            {
                return Math.Round((costo_corte * (mano_obra_insta + 1) * (materiales + 1)) + flete, 2);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private double getPrecioInstalado(float costo_corte_insta, float utilidad_insta)
        {
            try
            {
                return Math.Round((costo_corte_insta * (utilidad_insta + 1)), 2);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private double getCostoHoja(float costo_vidrio, float largo, float alto)
        {
            try
            {
                return Math.Round(costo_vidrio * largo * alto, 2);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private double getPrecioHoja(float costo_hoja, float utilidad_hoja)
        {
            try
            {
                return Math.Round((costo_hoja * (utilidad_hoja + 1)), 2);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        //terminan calculos dentro del grid ---------------------------------------------------------------------------------------------------

        //ejecuta proceso de carga y update a la Base de datos
        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == false)
            {
                if(backgroundWorker2.IsBusy == false)
                {
                    toolStripProgressBar1.Visible = true;
                    ((Control)tabPage1).Enabled = false;
                    toolStripStatusLabel1.Text = "Conectando...";
                    toolStripProgressBar1.Value = 0;
                    backgroundWorker2.RunWorkerAsync();
                }
            }
            else
            {
                MessageBox.Show("[Error] no se puede ejecutar este proceso mientras se esta descargando una lista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //termina carga             

        //Exportar a Excel
        private void button3_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                DialogResult result;
                result = MessageBox.Show("¿Deseas exportar " + comboBox1.Text + " a un archivo Excel?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (backgroundWorker3.IsBusy == false)
                    {
                        ((Control)tabPage1).Enabled = false;
                        toolStripStatusLabel1.Text = "Exportando: " + comboBox1.Text;
                        toolStripProgressBar1.Visible = true;
                        toolStripProgressBar1.Value = 0;
                        backgroundWorker3.RunWorkerAsync();
                    }
                }
            }
            else
            {
                MessageBox.Show("[Error] necesitas seleccionar una lista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker3.ReportProgress(10);
            constants.ExportToExcelFile(datagridviewNE1);
            backgroundWorker3.ReportProgress(100);
        }

        private void BackgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = "";
            toolStripStatusLabel2.Text = "";
            ((Control)tabPage1).Enabled = true;
        }

        private void BackgroundWorker3_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
            toolStripStatusLabel2.Text = " " + e.ProgressPercentage + "%";
        }
        //ends exportar a Excel

        //Pegar en el Grid ------------------------------
        private void datagridviewNE1_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                if (datagridviewNE1.CurrentCell.OwningColumn.HeaderText != "proveedor" && datagridviewNE1.CurrentCell.OwningColumn.HeaderText != "linea")
                {
                    contextMenuStrip1.Show(MousePosition);
                }
            }
        }

        private void pegadoEspecialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            constants.PasteOnGrid(datagridviewNE1);
        }
        //Ends pegar en el Grid  ------------------------

        //historial
        private void crearHistorial()
        {           
            foreach(System.Data.DataRow row in tem_table.Rows)
            {
                int rows = sql.countSQLRows("historial_actualizacion");
                if (rows >= 50)
                {
                    sql.deleteHistory();
                }                                
                sql.insertHistory(rows+1, row[0].ToString(), row[1].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString());                
            }
        }
        //ends historial

        //Opcion de ajuste de columnas
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                datagridviewNE1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                datagridviewNE1.Refresh();
                dataGridView2.Refresh();
                dataGridView3.Refresh();
            }
            else
            {
                datagridviewNE1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                datagridviewNE1.Refresh();
                dataGridView2.Refresh();
                dataGridView3.Refresh();
            }
        }
        //ends opciones    

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            data_column.Clear();
            foreach (DataGridViewRow i in datagridviewNE1.Rows)
            {
                foreach (DataGridViewCell x in i.Cells)
                {
                    if (x.ColumnIndex == datagridviewNE1.Columns[comboBox2.Text].Index)
                    {
                        System.Data.DataRow row = data_column.NewRow();
                        row[0] = x.Value;
                        data_column.Rows.Add(row);
                    }
                }
            }
        }
        //

        //Refresh 
        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
                if (constants.local == false)
                {
                    if (backgroundWorker2.IsBusy == false)
                    {
                        if (backgroundWorker4.IsBusy == false)
                        {
                            if (backgroundWorker1.IsBusy == false)
                            {
                                toolStripProgressBar1.Visible = true;
                                ((Control)tabPage1).Enabled = false;
                                tem_table.Clear();
                                ordenado = false;
                                comboBox8.SelectedIndex = -1;
                                comboBox9.SelectedIndex = -1;
                                backgroundWorker1.RunWorkerAsync();
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] no se puede ejecutar este proceso mientras se estan ingresando nuevo artículos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] no se puede ejecutar este proceso mientras se esta actualizando una lista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("[Error] se ha ingresado de manera local, no es posible conectarse al servidor.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] debes seleccionar una tabla primero.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        //Categorias y proveedores
        private void button6_Click(object sender, EventArgs e)
        {
            new sqlDateBaseManager().dropTableOnGridView(dataGridView4, "categorias");
            textBox2.Clear();
            comboBox3.SelectedIndex = -1;
            label8.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new sqlDateBaseManager().dropTableOnGridView(dataGridView5, "proveedores");
            textBox3.Clear();
            comboBox4.SelectedIndex = -1;
            label9.Text = "";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            sqlDateBaseManager sql_2 = new sqlDateBaseManager();
            if(textBox2.Text != "")
            {
                if (comboBox3.Text != "")
                {
                    if (category_id > 0)
                    {
                        sql_2.updateCategoria(category_id, textBox2.Text.ToLower(), comboBox3.Text.ToLower());
                        label8.Text = "Se ha modificado: " + textBox2.Text.ToLower();
                        category_id = 0;
                        button9.Visible = false;
                    }
                    else
                    {
                        if (sql_2.getCategoriaRep(textBox2.Text.ToLower(), comboBox3.Text.ToLower()) == false)
                        {
                            sql_2.createCategoria(textBox2.Text.ToLower(), comboBox3.Text.ToLower());
                            label8.Text = "Se ha agregado: " + textBox2.Text.ToLower();
                        }
                        else
                        {
                            MessageBox.Show("[Error] ya existe una categoría con ese nombre.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    sql_2.dropTableOnGridView(dataGridView4, "categorias");
                }
                else
                {
                    MessageBox.Show("[Error] debes seleccionar un grupo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] debes nombrar la categoría.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            sqlDateBaseManager sql_2 = new sqlDateBaseManager();
            if (textBox3.Text != "")
            {
                if (comboBox4.Text != "")
                {
                    if(proveedor_id > 0)
                    {
                        sql_2.updateProveedor(proveedor_id, textBox3.Text.ToLower(), comboBox4.Text.ToLower());
                        label9.Text = "Se ha modificado: " + textBox3.Text.ToLower();
                        proveedor_id = 0;
                        button10.Visible = false;
                        if (Directory.Exists(Application.StartupPath + "\\pics\\" + textBox3.Text.ToLower()) == false)
                        {
                            Directory.CreateDirectory(Application.StartupPath + "\\pics\\" + textBox3.Text.ToLower());
                        }
                    }
                    else
                    {
                        if (sql_2.getProveedorRep(textBox3.Text.ToLower(), comboBox4.Text.ToLower()) == false)
                        {
                            sql_2.createProveedor(textBox3.Text.ToLower(), comboBox4.Text.ToLower());
                            label9.Text = "Se ha agregado: " + textBox3.Text.ToLower();
                            if(Directory.Exists(Application.StartupPath + "\\pics\\" + textBox3.Text.ToLower()) == false)
                            {
                                Directory.CreateDirectory(Application.StartupPath + "\\pics\\" + textBox3.Text.ToLower());
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] ya existe proveedor con ese nombre.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    sql_2.dropTableOnGridView(dataGridView5, "proveedores");
                }
                else
                {
                    MessageBox.Show("[Error] debes seleccionar un grupo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] debes nombrar la categoría.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DataGridView4_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            contextMenuStrip2.Show(MousePosition);
        }

        private void DataGridView5_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            contextMenuStrip3.Show(MousePosition);
        }    

        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sqlDateBaseManager sql_2 = new sqlDateBaseManager();
            try
            {
                DialogResult r = MessageBox.Show("¿Estás seguro de eliminar está categoría?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (r == DialogResult.Yes)
                {
                    new delete_password().ShowDialog();
                    if (permiso == true)
                    {
                        sql_2.deleteCategoria((int)dataGridView4.CurrentRow.Cells[0].Value);
                        sql_2.dropTableOnGridView(dataGridView4, "categorias");
                        permiso = false;
                    }
                }
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void eliminarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            sqlDateBaseManager sql_2 = new sqlDateBaseManager();
            try
            {
                DialogResult r = MessageBox.Show("¿Estás seguro de eliminar esté proveedor?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (r == DialogResult.Yes)
                {
                    new delete_password().ShowDialog();
                    if (permiso == true)
                    {
                        sql_2.deleteProveedor((int)dataGridView5.CurrentRow.Cells[0].Value);
                        sql_2.dropTableOnGridView(dataGridView5, "proveedores");
                        permiso = false;
                    }
                }
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void modificarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            category_id = (int)dataGridView4.CurrentRow.Cells[0].Value;
            textBox2.Text = dataGridView4.CurrentRow.Cells[1].Value.ToString();
            comboBox3.Text = dataGridView4.CurrentRow.Cells[2].Value.ToString();
            button9.Visible = true;
        }

        private void modificarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            proveedor_id = (int)dataGridView5.CurrentRow.Cells[0].Value;
            textBox3.Text = dataGridView5.CurrentRow.Cells[1].Value.ToString();
            comboBox4.Text = dataGridView5.CurrentRow.Cells[2].Value.ToString();
            button10.Visible = true;
        }     

        private void button9_Click(object sender, EventArgs e)
        {
            button9.Visible = false;
            category_id = 0;
            resetFields(true);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            button10.Visible = false;
            proveedor_id = 0;
            resetFields(false);
        }

        private void resetFields(bool category)
        {
            if(category == true)
            {
                textBox2.Text = string.Empty;
                comboBox3.Text = string.Empty;
            }
            else
            {
                textBox3.Text = string.Empty;
                comboBox4.Text = string.Empty;
            }
        }
        //-----------------------------------------------------------------------------

        //Reload when close
        private void admin_panel_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (constants.updater_form_close == true)
            {
                new loading_form().ShowDialog();
            }
        }
        //

        ~admin_panel()
        {
            tem_table.Dispose();
        }

        private void BackgroundWorker4_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
            toolStripStatusLabel2.Text = " " + e.ProgressPercentage + "%";
        }

        //Añadir nuevo articulo
        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            toolStripStatusLabel1.Text = "Conectando...";
            int items = dataGridView6.RowCount-1;
            int c = 0;
            if (sql.setServerConnection() == true)
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    foreach (DataGridViewRow x in dataGridView6.Rows)
                    {
                        if ((x.Cells[0].Value.ToString() != null || x.Cells[0].Value.ToString() != "") && (x.Cells[1].Value.ToString() != null || x.Cells[1].Value.ToString() != ""))
                        {
                            if (sql.findSQLValue("clave", "clave", "costo_corte_precio", x.Cells[0].Value.ToString()) == false)
                            {
                                sql.insertListaCosto(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), 0, 0, 0, 0, 0, 0, DateTime.Today.ToString("dd/MM/yyyy"), "");
                                x.DefaultCellStyle.BackColor = Color.LightGreen;                              
                            }
                            else
                            {
                                MessageBox.Show("[Error] la clave " + x.Cells[0].Value.ToString() + " ya existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                x.DefaultCellStyle.BackColor = Color.Red;
                            }
                            c++;
                            backgroundWorker4.ReportProgress((int)((100 / items) * c));
                        }                                       
                    }
                }
                else if (comboBox1.SelectedIndex == 1)
                {
                    foreach (DataGridViewRow x in dataGridView6.Rows)
                    {
                        if ((x.Cells[0].Value.ToString() != null || x.Cells[0].Value.ToString() != "") && (x.Cells[1].Value.ToString() != null || x.Cells[1].Value.ToString() != ""))
                        {
                            if (sql.findSQLValue("clave", "clave", "instalado", x.Cells[0].Value.ToString()) == false)
                            {                            
                                sql.insertListaInstalado(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), 0, 0, 0, 0, 0, 0, DateTime.Today.ToString("dd/MM/yyyy"), "");
                                x.DefaultCellStyle.BackColor = Color.LightGreen;                              
                            }
                            else
                            {
                                MessageBox.Show("[Error] la clave " + x.Cells[0].Value.ToString() + " ya existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                x.DefaultCellStyle.BackColor = Color.Red;
                            }
                            c++;
                            backgroundWorker4.ReportProgress((int)((100 / items) * c));
                        }                       
                    }
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    foreach (DataGridViewRow x in dataGridView6.Rows)
                    {
                        if ((x.Cells[0].Value.ToString() != null || x.Cells[0].Value.ToString() != "") && (x.Cells[1].Value.ToString() != null || x.Cells[1].Value.ToString() != ""))
                        {
                            if (sql.findSQLValue("clave", "clave", "hojas", x.Cells[0].Value.ToString()) == false)
                            {                              
                                sql.insertListaHojas(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), 0, 0, 0, 0, 0, DateTime.Today.ToString("dd/MM/yyyy"), "");
                                x.DefaultCellStyle.BackColor = Color.LightGreen;                             
                            }
                            else
                            {
                                MessageBox.Show("[Error] la clave " + x.Cells[0].Value.ToString() + " ya existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                x.DefaultCellStyle.BackColor = Color.Red;
                            }
                            c++;
                            backgroundWorker4.ReportProgress((int)((100 / items) * c));
                        }
                    }
                }
                else if (comboBox1.SelectedIndex == 3)
                {
                    foreach (DataGridViewRow x in dataGridView6.Rows)
                    {
                        if ((x.Cells[0].Value.ToString() != null || x.Cells[0].Value.ToString() != "") && (x.Cells[1].Value.ToString() != null || x.Cells[1].Value.ToString() != ""))
                        {
                            if (sql.findSQLValue("clave", "clave", "acabados", x.Cells[0].Value.ToString()) == false)
                            {                               
                                sql.insertListaAcabados(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), 0, 0, DateTime.Today.ToString("dd/MM/yyyy"));
                                x.DefaultCellStyle.BackColor = Color.LightGreen;                               
                            }
                            else
                            {
                                MessageBox.Show("[Error] la clave " + x.Cells[0].Value.ToString() + " ya existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                x.DefaultCellStyle.BackColor = Color.Red;
                            }
                            c++;
                            backgroundWorker4.ReportProgress((int)((100 / items) * c));
                        }
                    }
                }
                else if (comboBox1.SelectedIndex == 4)
                {
                    foreach (DataGridViewRow x in dataGridView6.Rows)
                    {
                        if ((x.Cells[0].Value.ToString() != null || x.Cells[0].Value.ToString() != "") && (x.Cells[1].Value.ToString() != null || x.Cells[1].Value.ToString() != ""))
                        {
                            if (sql.findSQLValue("clave", "clave", "perfiles", x.Cells[0].Value.ToString()) == false)
                            {                               
                                sql.insertListaAluminio(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), "", "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, DateTime.Today.ToString("dd/MM/yyyy"), 0, 0, 0);
                                x.DefaultCellStyle.BackColor = Color.LightGreen;                                
                            }
                            else
                            {
                                MessageBox.Show("[Error] la clave " + x.Cells[0].Value.ToString() + " ya existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                x.DefaultCellStyle.BackColor = Color.Red;
                            }
                            c++;
                            backgroundWorker4.ReportProgress((int)((100 / items) * c));
                        }
                    }
                }
                else if (comboBox1.SelectedIndex == 5)
                {
                    foreach (DataGridViewRow x in dataGridView6.Rows)
                    {
                        if ((x.Cells[0].Value.ToString() != null || x.Cells[0].Value.ToString() != "") && (x.Cells[1].Value.ToString() != null || x.Cells[1].Value.ToString() != ""))
                        {
                            if (sql.findSQLValue("clave", "clave", "herrajes", x.Cells[0].Value.ToString()) == false)
                            {                              
                                sql.insertListaHerrajes(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), "", "", "", "", 0, DateTime.Today.ToString("dd/MM/yyyy"));
                                x.DefaultCellStyle.BackColor = Color.LightGreen;                            
                            }
                            else
                            {
                                MessageBox.Show("[Error] la clave " + x.Cells[0].Value.ToString() + " ya existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                x.DefaultCellStyle.BackColor = Color.Red;
                            }
                            c++;
                            backgroundWorker4.ReportProgress((int)((100 / items) * c));
                        }
                    }
                }
                else if (comboBox1.SelectedIndex == 6)
                {
                    foreach (DataGridViewRow x in dataGridView6.Rows)
                    {
                        if ((x.Cells[0].Value.ToString() != null || x.Cells[0].Value.ToString() != "") && (x.Cells[1].Value.ToString() != null || x.Cells[1].Value.ToString() != ""))
                        {
                            if (sql.findSQLValue("clave", "clave", "otros", x.Cells[0].Value.ToString()) == false)
                            {                              
                                sql.insertListaOtros(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), "", "", "", "", 0, DateTime.Today.ToString("dd/MM/yyyy"), 0, 0);
                                x.DefaultCellStyle.BackColor = Color.LightGreen;                               
                            }
                            else
                            {
                                MessageBox.Show("[Error] la clave " + x.Cells[0].Value.ToString() + " ya existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                x.DefaultCellStyle.BackColor = Color.Red;
                            }
                            c++;
                            backgroundWorker4.ReportProgress((int)((100 / items) * c));
                        }
                    }
                }
                else if (comboBox1.SelectedIndex == 7)
                {
                    foreach (DataGridViewRow x in dataGridView6.Rows)
                    {
                        if ((x.Cells[0].Value.ToString() != null || x.Cells[0].Value.ToString() != "") && (x.Cells[1].Value.ToString() != null || x.Cells[1].Value.ToString() != ""))
                        {
                            if (sql.findSQLValue("clave", "clave", "colores_aluminio", x.Cells[0].Value.ToString()) == false)
                            {                                
                                sql.insertListaColores(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), "", 0, 0, DateTime.Today.ToString("dd/MM/yyyy"));
                                x.DefaultCellStyle.BackColor = Color.LightGreen;                              
                            }
                            else
                            {
                                MessageBox.Show("[Error] la clave " + x.Cells[0].Value.ToString() + " ya existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                x.DefaultCellStyle.BackColor = Color.Red;
                            }
                            c++;
                            backgroundWorker4.ReportProgress((int)((100 / items) * c));
                        }
                    }
                }                
            }
            else
            {
                toolStripStatusLabel1.Text = "(Sin Conexión)";
            }
        }

        private void BackgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabel1.Text = "Listo!";
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = "";
            toolStripStatusLabel2.Text = "";
            ((Control)tabPage1).Enabled = true;
            toolStripProgressBar1.Visible = false;
            //Delete completed
            int c = 0;
            foreach (DataGridViewRow x in dataGridView6.Rows)
            {
                if (x.DefaultCellStyle.BackColor == Color.LightGreen)
                {
                    c++;
                }
            }
            while(c > 0)
            {
                foreach (DataGridViewRow x in dataGridView6.Rows)
                {
                    if (x.DefaultCellStyle.BackColor == Color.LightGreen)
                    {
                        dataGridView6.Rows.Remove(x);
                        c--;
                    }
                }
            }
            //Reload         
            if (backgroundWorker2.IsBusy == false)
            {
                if (backgroundWorker1.IsBusy == false)
                {
                    toolStripProgressBar1.Visible = true;
                    ((Control)tabPage1).Enabled = false;
                    tem_table.Clear();
                    toolStripStatusLabel1.Text = "Conectando...";
                    toolStripProgressBar1.Value = 0;
                    ordenado = false;
                    comboBox8.SelectedIndex = -1;
                    comboBox9.SelectedIndex = -1;
                    backgroundWorker1.RunWorkerAsync();
                }
            }            
            //--->
        }

        private bool checkInvalidChars(string clave)
        {
            bool r = true;
            foreach(char x in clave)
            {
                if(x == '_')
                {
                    r = false;
                }
                else if(x == '-')
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
                else
                {
                    r = true;
                }
            }
            return r;
        }

        private bool checkNewValues()
        {
            bool r = false;
            foreach (DataGridViewRow x in dataGridView6.Rows)
            {
                if (x.Index < dataGridView6.RowCount - 1)
                {
                    if (x.Cells[0].Value != null)
                    {
                        if (checkInvalidChars(x.Cells[0].Value.ToString()) == true)
                        {
                            if (x.Cells[0].Value.ToString().Length < 3 || x.Cells[0].Value.ToString().Length > 10)
                            {
                                MessageBox.Show("[Error] La clave " + x.Cells[0].Value + " no es válida, solo se pueden usar claves de 3 o hasta 10 caracteres.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                x.DefaultCellStyle.BackColor = Color.Red;
                                r = true;
                                break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] la clave incluye caracteres no válidos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            x.DefaultCellStyle.BackColor = Color.Red;
                            r = true;
                            break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] dato no válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        x.DefaultCellStyle.BackColor = Color.Red;
                        r = true;
                        break;
                    }
                    //------------------------------------------------------>
                    if (x.Cells[1].Value != null)
                    {
                        if (checkInvalidChars(x.Cells[1].Value.ToString()) == true)
                        {
                            if (x.Cells[1].Value.ToString().Length < 3 || x.Cells[1].Value.ToString().Length > 40)
                            {
                                MessageBox.Show("[Error] El nombre de artículo " + x.Cells[1].Value + " no es válido, solo se pueden usar nombres de 3 o hasta 40 caracteres.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                x.DefaultCellStyle.BackColor = Color.Red;
                                r = true;
                                break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] el nombre de artículo incluye caracteres no válidos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            x.DefaultCellStyle.BackColor = Color.Red;
                            r = true;
                            break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] dato no válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        x.DefaultCellStyle.BackColor = Color.Red;
                        r = true;
                        break;
                    }
                }
            }
            return r;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
                if (dataGridView6.RowCount-1 > 0)
                {
                    if (backgroundWorker1.IsBusy == false)
                    {
                        if (backgroundWorker2.IsBusy == false)
                        {
                            if (backgroundWorker4.IsBusy == false)
                            {
                                if (checkNewValues() == false)
                                {
                                    toolStripProgressBar1.Visible = true;
                                    ((Control)tabPage1).Enabled = false;
                                    backgroundWorker4.RunWorkerAsync();
                                }                              
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] no se puede ejecutar este proceso mientras se esta actualizando una lista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] no se puede ejecutar este proceso mientras se esta descargando una lista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("[Error] no existen artículos nuevos que ingresar.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] necesitas seleccionar una lista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void eliminarRegistroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                DialogResult r = DialogResult.None;

                if (checkBox2.Checked)
                {
                    r = MessageBox.Show("¿Estás seguro de eliminar esté registro?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                else
                {
                    r = DialogResult.Yes;
                }

                if (r == DialogResult.Yes)
                {
                    new delete_password().ShowDialog();
                    if (permiso == true)
                    {
                        try
                        {
                            if (comboBox1.SelectedIndex == 0)
                            {
                                sql.deleteSQLValue("costo_corte_precio", "clave", datagridviewNE1.CurrentRow.Cells[0].Value.ToString());
                            }
                            else if (comboBox1.SelectedIndex == 1)
                            {
                                sql.deleteSQLValue("instalado", "clave", datagridviewNE1.CurrentRow.Cells[0].Value.ToString());
                            }
                            else if (comboBox1.SelectedIndex == 2)
                            {
                                sql.deleteSQLValue("hojas", "clave", datagridviewNE1.CurrentRow.Cells[0].Value.ToString());
                            }
                            else if (comboBox1.SelectedIndex == 3)
                            {
                                sql.deleteSQLValue("acabados", "clave", datagridviewNE1.CurrentRow.Cells[0].Value.ToString());
                            }
                            else if (comboBox1.SelectedIndex == 4)
                            {
                                sql.deleteSQLValue("perfiles", "id", datagridviewNE1.CurrentRow.Cells[0].Value.ToString());
                            }
                            else if (comboBox1.SelectedIndex == 5)
                            {
                                sql.deleteSQLValue("herrajes", "id", datagridviewNE1.CurrentRow.Cells[0].Value.ToString());
                            }
                            else if (comboBox1.SelectedIndex == 6)
                            {
                                sql.deleteSQLValue("otros", "id", datagridviewNE1.CurrentRow.Cells[0].Value.ToString());
                            }
                            else if (comboBox1.SelectedIndex == 7)
                            {
                                sql.deleteSQLValue("colores_aluminio", "id", datagridviewNE1.CurrentRow.Cells[0].Value.ToString());
                                if (sql.findSQLValue("clave", "clave", "colores_imagenes", datagridviewNE1.CurrentRow.Cells[1].Value.ToString()) == false)
                                {
                                    sql.deleteSQLValue("colores_imagenes", "clave", datagridviewNE1.CurrentRow.Cells[1].Value.ToString());
                                }
                            }
                            datagridviewNE1.Rows.Remove(datagridviewNE1.CurrentRow);
                            label11.Text = string.Empty;
                            label12.Text = string.Empty;
                            label14.Text = string.Empty;
                            permiso = false;
                        }
                        catch (Exception err)
                        {
                            constants.errorLog(err.ToString());
                        }
                    }
                }    
            }        
        }

        //Quitar todos los nuevos items
        private void button12_Click(object sender, EventArgs e)
        {
            dataGridView6.Rows.Clear();
        }

        //Quitar nuevo item
        private void eliminarToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (dataGridView6.RowCount > 0)
            {
                try
                {
                    dataGridView6.Rows.Remove(dataGridView6.CurrentRow);
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                }
            }
        }

        private bool getSearchParameters(DataGridViewRow row, string parameter)
        {
            bool r = false;
            foreach(DataGridViewCell v in row.Cells)
            {
                if(v.Value.ToString() == parameter)
                {
                    r = true;
                }
            }
            return r;
        }

        //Editar cambios en un rango de columna dada   
        private void button2_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    if (comboBox6.Text != "" && comboBox7.Text != "")
                    {
                        if (getSearchParameters(x, comboBox6.Text) == true && getSearchParameters(x, comboBox7.Text) == true)
                        {
                            foreach (DataGridViewCell c in x.Cells)
                            {
                                if (c.OwningColumn.HeaderText == comboBox2.Text && c.OwningColumn.ReadOnly != true)
                                {
                                    float per = stringToFloat(textBox1.Text);
                                    if (constants.isFloat(c.Value.ToString()) == true || constants.isInteger(c.Value.ToString()) == true)
                                    {
                                        datagridviewNE1.CurrentCell = c;
                                        datagridviewNE1.BeginEdit(true);                              
                                        c.Value = Math.Round(per > 0 ? stringToFloat(data_column.Rows[x.Index].ItemArray[0].ToString()) * ((per / 100) + 1) : stringToFloat(data_column.Rows[x.Index].ItemArray[0].ToString()) / ((Math.Abs(per) / 100) + 1), 2);
                                        datagridviewNE1.EndEdit();
                                    }
                                }
                            }
                        }
                    }
                    else if (comboBox6.Text != "" && comboBox7.Text == "")
                    {
                        if (getSearchParameters(x, comboBox6.Text) == true)
                        {
                            foreach (DataGridViewCell c in x.Cells)
                            {
                                if (c.OwningColumn.HeaderText == comboBox2.Text && c.OwningColumn.ReadOnly != true)
                                {
                                    float per = stringToFloat(textBox1.Text);
                                    if (constants.isFloat(c.Value.ToString()) == true || constants.isInteger(c.Value.ToString()) == true)
                                    {
                                        datagridviewNE1.CurrentCell = c;
                                        datagridviewNE1.BeginEdit(true);
                                        c.Value = Math.Round(per > 0 ? stringToFloat(data_column.Rows[x.Index].ItemArray[0].ToString()) * ((per / 100) + 1) : stringToFloat(data_column.Rows[x.Index].ItemArray[0].ToString()) / ((Math.Abs(per) / 100) + 1), 2);
                                        datagridviewNE1.EndEdit();
                                    }
                                }
                            }
                        }
                    }
                    else if (comboBox6.Text == "" && comboBox7.Text != "")
                    {
                        if (getSearchParameters(x, comboBox7.Text) == true)
                        {
                            foreach (DataGridViewCell c in x.Cells)
                            {
                                if (c.OwningColumn.HeaderText == comboBox2.Text && c.OwningColumn.ReadOnly != true)
                                {
                                    float per = stringToFloat(textBox1.Text);
                                    if (constants.isFloat(c.Value.ToString()) == true || constants.isInteger(c.Value.ToString()) == true)
                                    {
                                        datagridviewNE1.CurrentCell = c;
                                        datagridviewNE1.BeginEdit(true);
                                        c.Value = Math.Round(per > 0 ? stringToFloat(data_column.Rows[x.Index].ItemArray[0].ToString()) * ((per / 100) + 1) : stringToFloat(data_column.Rows[x.Index].ItemArray[0].ToString()) / ((Math.Abs(per) / 100) + 1), 2);
                                        datagridviewNE1.EndEdit();
                                    }
                                }
                            }
                        }
                    }
                    else if (comboBox6.Text == "" && comboBox7.Text == "")
                    {
                        foreach (DataGridViewCell c in x.Cells)
                        {
                            if (c.OwningColumn.HeaderText == comboBox2.Text && c.OwningColumn.ReadOnly != true)
                            {
                                float per = stringToFloat(textBox1.Text);
                                if (constants.isFloat(c.Value.ToString()) == true || constants.isInteger(c.Value.ToString()) == true)
                                {
                                    datagridviewNE1.CurrentCell = c;
                                    datagridviewNE1.BeginEdit(true);
                                    c.Value = Math.Round(per > 0 ? stringToFloat(data_column.Rows[x.Index].ItemArray[0].ToString()) * ((per / 100) + 1) : stringToFloat(data_column.Rows[x.Index].ItemArray[0].ToString()) / ((Math.Abs(per) / 100) + 1), 2);
                                    datagridviewNE1.EndEdit();
                                }
                            }
                        }
                    }
                }
                refreshCalculos();
            }
        }

        //search picture
        private void button14_Click(object sender, EventArgs e)
        {
            if (label11.Text != "")
            {
                if (comboBox1.SelectedIndex != 7)
                {
                    openFileDialog1.Title = "Selecciona una imagen para el artículo";
                    openFileDialog1.Filter = "Image files (*.png) | *.png";
                }
                else
                {
                    openFileDialog1.Title = "Selecciona un color para el acabado";
                    openFileDialog1.Filter = "Image files (*.jpg, *.jpeg) | *.jpg; *.jpeg";
                }

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bm = new Bitmap(openFileDialog1.FileName);
                    pic = new Bitmap(bm, 60, 60);
                    pictureBox1.Image = pic;
                }               
            }
        }

        //save new picture
        private void button13_Click(object sender, EventArgs e)
        {
            if(label11.Text != "")
            {
                if (comboBox1.SelectedIndex != 7)
                {
                    string proveedor = string.Empty;
                    if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
                    {
                        proveedor = constants.getArticuloProveedorCristales(label11.Text);
                    }
                    else if (comboBox1.SelectedIndex == 4)
                    {
                        proveedor = constants.getArticuloProveedorPerfiles(constants.stringToInt(label11.Text));
                    }
                    else if (comboBox1.SelectedIndex == 5)
                    {
                        proveedor = constants.getArticuloProveedorHerrajes(constants.stringToInt(label11.Text));
                    }
                    else if (comboBox1.SelectedIndex == 6)
                    {
                        proveedor = constants.getArticuloProveedorOtros(constants.stringToInt(label11.Text));
                    }

                    if (proveedor != string.Empty)
                    {
                        if (Directory.Exists(Application.StartupPath + "\\pics\\" + proveedor))
                        {
                            if (pic != null)
                            {
                                try
                                {
                                    pic.Save(Application.StartupPath + "\\pics\\" + proveedor + "\\" + label11.Text + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                }
                                catch (Exception) { }
                                pic = null;
                                MessageBox.Show("Se le ha asignado una nueva imagen.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("[Error] no se ha seleccionado una nueva imagen.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] la carpeta del proveedor no existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] aún no se le asigna proveedor a dicho artículo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (Directory.Exists(Application.StartupPath + "\\pics\\acabados_especiales"))
                    {
                        if (pic != null)
                        {
                            try
                            {
                                pic.Save(Application.StartupPath + "\\pics\\acabados_especiales\\" + label12.Text + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                            catch (Exception) { }
                            if (sql.findSQLValue("clave", "clave", "colores_imagenes", label12.Text) == false)
                            {
                                sql.insertColoresImagenes(label12.Text, pic);
                            }
                            else
                            {
                                sql.updateColoresImagenes(label12.Text, pic);
                            }
                            pic = null;
                            MessageBox.Show("Se le ha asignado una nueva imagen.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("[Error] no se ha seleccionado una nueva imagen.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] la carpeta de acabados especiales no existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        //Reload
        private void button15_Click(object sender, EventArgs e)
        {
            new loading_form().ShowDialog();
        }

        private void pegadoEspecialToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            constants.PasteOnGrid(dataGridView6, true, true);
        }

        //Buscar
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            bool found = false;
            if (textBox4.Text != "" && comboBox5.Text != "")
            {
                find_next.Clear();
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    foreach (DataGridViewCell v in x.Cells)
                    {
                        if (v.OwningColumn.HeaderText == comboBox5.Text)
                        {
                            if (v.Value.ToString().Equals(textBox4.Text) == true)
                            {
                                v.Selected = true;
                                found = true;
                                find_next.Add(x.Index);
                                datagridviewNE1.FirstDisplayedScrollingRowIndex = x.Index;
                                break;
                            }
                            else if (v.Value.ToString().StartsWith(textBox4.Text) == true)
                            {
                                v.Selected = true;
                                found = true;
                                find_next.Add(x.Index);
                                datagridviewNE1.FirstDisplayedScrollingRowIndex = x.Index;
                                break;
                            }
                            else
                            {
                                v.Selected = false;
                            }
                        }
                    }
                    if(found == true)
                    {
                        break;
                    }
                }
            }
            else
            {
                datagridviewNE1.ClearSelection();
            }
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox8.SelectedIndex >= 0)
            {
                if (comboBox1.Text != "")
                {
                    if (constants.local == false)
                    {
                        if (backgroundWorker2.IsBusy == false)
                        {
                            if (backgroundWorker4.IsBusy == false)
                            {
                                if (backgroundWorker1.IsBusy == false)
                                {
                                    comboBox9.SelectedIndex = -1;
                                    toolStripProgressBar1.Visible = true;
                                    ((Control)tabPage1).Enabled = false;
                                    tem_table.Clear();
                                    value_filter = comboBox8.Text;
                                    ordenado = true;
                                    backgroundWorker1.RunWorkerAsync();
                                }
                            }
                            else
                            {
                                MessageBox.Show("[Error] no se puede ejecutar este proceso mientras se estan ingresando nuevo artículos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] no se puede ejecutar este proceso mientras se esta actualizando una lista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] se ha ingresado de manera local, no es posible conectarse al servidor.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("[Error] debes seleccionar una tabla primero.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox9.SelectedIndex >= 0)
            {
                if (comboBox1.Text != "")
                {
                    if (constants.local == false)
                    {
                        if (backgroundWorker2.IsBusy == false)
                        {
                            if (backgroundWorker4.IsBusy == false)
                            {
                                if (backgroundWorker1.IsBusy == false)
                                {
                                    comboBox8.SelectedIndex = -1;
                                    toolStripProgressBar1.Visible = true;
                                    ((Control)tabPage1).Enabled = false;
                                    tem_table.Clear();
                                    value_filter = comboBox9.Text;
                                    ordenado = true;
                                    backgroundWorker1.RunWorkerAsync();
                                }
                            }
                            else
                            {
                                MessageBox.Show("[Error] no se puede ejecutar este proceso mientras se estan ingresando nuevo artículos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] no se puede ejecutar este proceso mientras se esta actualizando una lista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] se ha ingresado de manera local, no es posible conectarse al servidor.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("[Error] debes seleccionar una tabla primero.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            new sqlDateBaseManager().dropTableOnGridViewWithFilter(dataGridView4, "categorias", "categoria", textBox5.Text);
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            new sqlDateBaseManager().dropTableOnGridViewWithFilter(dataGridView5, "proveedores", "proveedor", textBox6.Text);
        }

        //ToUpper
        private void toUpperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!datagridviewNE1.CurrentCell.ReadOnly)
            {
                datagridviewNE1.CurrentCell.Selected = true;
                datagridviewNE1.BeginEdit(true);
                datagridviewNE1.CurrentCell.Value = datagridviewNE1.CurrentCell.Value.ToString().ToUpper();
                datagridviewNE1.EndEdit();
            }
        }

        //ToLower
        private void toLowerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!datagridviewNE1.CurrentCell.ReadOnly)
            {
                datagridviewNE1.CurrentCell.Selected = true;
                datagridviewNE1.BeginEdit(true);
                datagridviewNE1.CurrentCell.Value = datagridviewNE1.CurrentCell.Value.ToString().ToLower();
                datagridviewNE1.EndEdit();
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Toda la lista se actualizara al dia de hoy. ¿Estás seguro de proceder?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(r == DialogResult.Yes)
            {
                if(datagridviewNE1.RowCount <= 100)
                {
                    foreach(DataGridViewRow x in datagridviewNE1.Rows)
                    {
                        foreach (DataGridViewCell v in x.Cells)
                        {
                            if (v.OwningColumn.HeaderText == "articulo")
                            {
                                v.Selected = true;
                                datagridviewNE1.BeginEdit(true);
                                datagridviewNE1.EndEdit();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("[Error] el máximo número de registros para realizar está acción es de 100 registros.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Añadir el incremento o decremento
        private void añadirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.stringToFloat(datagridviewNE1.CurrentCell.Value.ToString()) > 0)
            {
                datagridviewNE1.BeginEdit(true);
                float per = constants.stringToFloat(textBox1.Text);
                datagridviewNE1.CurrentCell.Value = Math.Round(per > 0 ? stringToFloat(datagridviewNE1.CurrentCell.Value.ToString()) * ((per / 100) + 1) : stringToFloat(datagridviewNE1.CurrentCell.Value.ToString()) / ((Math.Abs(per) / 100) + 1), 2);
                datagridviewNE1.EndEdit();
            }
        }

        private bool getNext(int row)
        {
            bool r = false;
            foreach(int x in find_next)
            {
                if(x == row)
                {
                    r = true;
                }
            }
            return r;
        }

        //find next
        private void button17_Click(object sender, EventArgs e)
        {
            bool found = false;
            if (textBox4.Text != "" && comboBox5.Text != "")
            {
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    foreach (DataGridViewCell v in x.Cells)
                    {
                        if (getNext(x.Index) == false)
                        {
                            if (v.OwningColumn.HeaderText == comboBox5.Text)
                            {
                                if (v.Value.ToString().Equals(textBox4.Text) == true)
                                {
                                    v.Selected = true;
                                    found = true;
                                    datagridviewNE1.FirstDisplayedScrollingRowIndex = x.Index;
                                    find_next.Add(x.Index);
                                    break;
                                }
                                else if (v.Value.ToString().StartsWith(textBox4.Text) == true)
                                {
                                    v.Selected = true;
                                    found = true;
                                    datagridviewNE1.FirstDisplayedScrollingRowIndex = x.Index;
                                    find_next.Add(x.Index);
                                    break;
                                }
                                else
                                {
                                    v.Selected = false;
                                }
                            }
                        }
                    }
                    if (found == true)
                    {
                        break;
                    }                  
                }
                if(found == false)
                {
                    find_next.Clear();
                    MessageBox.Show("No existen mas resultados de búsqueda.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
