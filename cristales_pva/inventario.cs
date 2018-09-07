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
    public partial class inventario : Form
    {
        //------>
        DataTable data;

        //------>
        List<int> find_next = new List<int>();
        List<string> update_data = new List<string>();

        //------>
        string[] costeo = new string[] { "PZA", "MT2", "JGO", "TRAMO", "MTL" };
        DataTable update_table = new DataTable();

        //------>
        sqlDateBaseManager sql = new sqlDateBaseManager();

        public inventario()
        {
            InitializeComponent();
            datagridviewNE1.RowsAdded += DatagridviewNE1_RowsAdded;
            datagridviewNE1.CellClick += DatagridviewNE1_CellClick;
            datagridviewNE1.EditingControlShowing += DatagridviewNE1_EditingControlShowing;          
            datagridviewNE1.CellEndEdit += DatagridviewNE1_CellEndEdit;
            //------------------------->
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            backgroundWorker2.RunWorkerCompleted += BackgroundWorker2_RunWorkerCompleted;
            backgroundWorker3.RunWorkerCompleted += BackgroundWorker3_RunWorkerCompleted;
            backgroundWorker4.RunWorkerCompleted += BackgroundWorker4_RunWorkerCompleted;
            //--------------------------->
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;

            //Tables
            update_table.Columns.Add("id");
            update_table.Columns.Add("clave");
            update_table.Columns.Add("articulo");
            update_table.Columns.Add("linea");
            update_table.Columns.Add("proveedor");
            update_table.Columns.Add("costeo");
          
            //--------------------------->
            comboBox4.Text = "clave";
            comboBox8.Text = "clave";
            comboBox12.Text = "clave";

            //--------------------------->
            textBox1.KeyDown += TextBox1_KeyDown;
            textBox2.KeyDown += TextBox2_KeyDown;
            textBox9.KeyDown += TextBox9_KeyDown;
        }

        private void reset()
        {
            update_data.Clear();
            update_table.Rows.Clear();
        }

        private void DatagridviewNE1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if(datagridviewNE1.Rows[e.RowIndex].Cells[0].Value == null)
            {
                datagridviewNE1.Rows[e.RowIndex].Cells[0].Value = "-1";
                datagridviewNE1.Rows[e.RowIndex].Cells[1].Value = "";
                datagridviewNE1.Rows[e.RowIndex].Cells[2].Value = "";
                datagridviewNE1.Rows[e.RowIndex].Cells[3].Value = "";
                datagridviewNE1.Rows[e.RowIndex].Cells[4].Value = "";
                datagridviewNE1.Rows[e.RowIndex].Cells[5].Value = "";
                datagridviewNE1.Rows[e.RowIndex].Cells[6].Value = "0";
                //----------------------------------------------------->
                datagridviewNE1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.LightBlue;
                datagridviewNE1.Rows[e.RowIndex].Cells[2].Style.BackColor = Color.LightBlue;
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if(datagridviewNE1.RowCount <= 0)
            {
                e.Cancel = true;
            }
            //------------------------------------------------------------->
            if(datagridviewNE1.CurrentRow.Cells[0].Value.ToString() == "-1")
            {
                contextMenuStrip1.Items[2].Visible = false;
            }
            else
            {
                contextMenuStrip1.Items[2].Visible = true;
            }
        }

        private void DatagridviewNE1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if(datagridviewNE1.RowCount > 0)
            {
                if (datagridviewNE1.CurrentRow.Cells[0].Value.ToString() != "-1")
                {
                    if (datagridviewNE1.CurrentRow.Cells[0].Value != null && datagridviewNE1.CurrentRow.Cells[1].Value != null && datagridviewNE1.CurrentRow.Cells[2].Value != null)
                    {                        
                        setVisto(true, datagridviewNE1.CurrentRow.Index, 0);
                        string clave = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                        if (noRepeatIndex(clave, update_data) == false)
                        {
                            DataRow row = update_table.NewRow();
                            row[0] = datagridviewNE1.CurrentRow.Cells[0].Value.ToString();
                            row[1] = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                            row[2] = datagridviewNE1.CurrentRow.Cells[2].Value.ToString();
                            row[3] = datagridviewNE1.CurrentRow.Cells[3].Value.ToString();
                            row[4] = datagridviewNE1.CurrentRow.Cells[4].Value.ToString();
                            row[5] = datagridviewNE1.CurrentRow.Cells[5].Value.ToString();
                            update_table.Rows.Add(row);
                            update_data.Add(clave);
                        }
                        else
                        {
                            foreach (DataRow x in update_table.Rows)
                            {
                                if (x[1].ToString() == clave)
                                {
                                    x[2] = datagridviewNE1.CurrentRow.Cells[2].Value.ToString();
                                    x[3] = datagridviewNE1.CurrentRow.Cells[3].Value.ToString();
                                    x[4] = datagridviewNE1.CurrentRow.Cells[4].Value.ToString();
                                    x[5] = datagridviewNE1.CurrentRow.Cells[5].Value.ToString();
                                }
                            }
                        }                       
                    }
                }                
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            comboBox6.SelectedIndex = -1;
            comboBox7.SelectedIndex = -1;
            comboBox14.SelectedIndex = -1;
            comboBox13.SelectedIndex = -1;
        }

        private void DatagridviewNE1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                if (datagridviewNE1.CurrentCell.OwningColumn.HeaderText == "artículo" || datagridviewNE1.CurrentCell.OwningColumn.HeaderText == "clave")
                {
                    if (e.Control is TextBox)
                    {
                        ((TextBox)(e.Control)).CharacterCasing = CharacterCasing.Upper;
                    }
                }               
            }
        }

        private void DatagridviewNE1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                if (backgroundWorker1.IsBusy == false)
                {
                    if(datagridviewNE1.CurrentCell.ColumnIndex == 1)
                    {
                        if(datagridviewNE1.CurrentRow.Cells[0].Value.ToString() == "-1")
                        { 
                            if (datagridviewNE1.CurrentCell.ReadOnly)
                            {
                                datagridviewNE1.CurrentCell.ReadOnly = false;
                            }
                        }
                        else
                        {
                            if (!datagridviewNE1.CurrentCell.ReadOnly)
                            {
                                datagridviewNE1.CurrentCell.ReadOnly = true;
                            }
                        }
                    }                  
                    //---> importante
                    if (datagridviewNE1.CurrentCell.Value == null)
                    {
                        datagridviewNE1.CurrentCell.Value = "";
                    }
                    //                   
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
                                cb.Items.AddRange(constants.getProveedores("aluminio").ToArray());
                                break;
                            case 2:
                                cb.Items.AddRange(constants.getProveedores("herraje").ToArray());
                                break;
                            case 3:
                                cb.Items.AddRange(constants.getProveedores("otros").ToArray());
                                break;                           
                            default: break;
                        }
                        foreach (string x in cb.Items)
                        {
                            if (x == datagridviewNE1.CurrentCell.Value.ToString())
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
                                cb.Items.AddRange(constants.getCategorias("aluminio").ToArray());
                                break;
                            case 2:
                                cb.Items.AddRange(constants.getCategorias("herraje").ToArray());
                                break;
                            case 3:
                                cb.Items.AddRange(constants.getCategorias("otros").ToArray());
                                break;
                            default: break;
                        }
                        foreach (string x in cb.Items)
                        {
                            if (x == datagridviewNE1.CurrentCell.Value.ToString())
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
                    if (datagridviewNE1.CurrentCell.OwningColumn.HeaderText == "costeo")
                    {
                        DataGridViewComboBoxCell cb = new DataGridViewComboBoxCell();
                        cb.Sorted = true;
                        string u = string.Empty;
                        cb.Items.Clear();
                        cb.Items.AddRange(costeo);
                        foreach (string x in cb.Items)
                        {
                            if (x == datagridviewNE1.CurrentCell.Value.ToString())
                            {
                                u = datagridviewNE1.CurrentCell.Value.ToString();
                            }
                        }
                        if (u == string.Empty)
                        {
                            datagridviewNE1.CurrentCell.Value = "PZA";
                        }
                        cb.Value = u;
                        datagridviewNE1.CurrentRow.Cells[datagridviewNE1.CurrentCell.ColumnIndex] = cb;
                        cb.Dispose();
                    }                            
                }
            }
        }

        private List<string> getSuggets(string clave)
        {
            List<string> list = new List<string>();
            listas_entities_pva listas = new listas_entities_pva();
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    list = listas.lista_precios_hojas.Where(x => x.clave.StartsWith(clave)).Select(x => x.clave).ToList();
                    break;
                case 1:
                    list = listas.perfiles.Where(x => x.clave.StartsWith(clave)).Select(x => x.clave).ToList();
                    break;
                case 2:
                    list = listas.herrajes.Where(x => x.clave.StartsWith(clave)).Select(x => x.clave).ToList();
                    break;
                case 3:
                    list = listas.otros.Where(x => x.clave.StartsWith(clave)).Select(x => x.clave).ToList();
                    break;
                default: break;
            }
            return list;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            datagridviewNE1.AllowUserToAddRows = true;
            tabControl1.SelectedIndex = 0;
            executeLoad();
        }

        private void executeLoad(datagridviewNE datagridview=null, bool periodo=false)
        {
            if (!backgroundWorker1.IsBusy || !backgroundWorker2.IsBusy || !backgroundWorker3.IsBusy || !backgroundWorker4.IsBusy)
            {
                reset();
                pictureBox1.Visible = true;
                object[] v = new object[] { datagridview, periodo };
                backgroundWorker1.RunWorkerAsync(v);               
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] dat = e.Argument as object[];
            datagridviewNE dt = dat[0] as datagridviewNE;
            bool periodo = (bool)dat[1];
            sqlDateBaseManager sql = new sqlDateBaseManager();
            int tienda_id = sql.getTiendaID(constants.org_name);
            if (tienda_id > 0)
            {
                if (dt == null)
                {                   
                    datagridviewNE1.Rows.Clear();
                    if (comboBox2.Text != "")
                    {
                        data = sql.getInventario(comboBox1.Text, tienda_id, "linea", comboBox2.Text);
                    }
                    else if (comboBox3.Text != "")
                    {
                        data = sql.getInventario(comboBox1.Text, tienda_id, "proveedor", comboBox3.Text);
                    }
                    else
                    {
                        data = sql.getInventario(comboBox1.Text, tienda_id);
                        comboBox2.Items.Clear();
                        comboBox3.Items.Clear();
                        switch (comboBox1.SelectedIndex)
                        {
                            case 0:
                                comboBox2.Items.AddRange(constants.getCategorias("vidrio").ToArray());
                                comboBox3.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                                break;
                            case 1:
                                comboBox2.Items.AddRange(constants.getCategorias("aluminio").ToArray());
                                comboBox3.Items.AddRange(constants.getProveedores("aluminio").ToArray());
                                break;
                            case 2:
                                comboBox2.Items.AddRange(constants.getCategorias("herraje").ToArray());
                                comboBox3.Items.AddRange(constants.getProveedores("herraje").ToArray());
                                break;
                            case 3:
                                comboBox2.Items.AddRange(constants.getCategorias("otros").ToArray());
                                comboBox3.Items.AddRange(constants.getProveedores("otros").ToArray());
                                break;
                            default: break;
                        }
                    }
                    foreach (DataRow x in data.Rows)
                    {
                        datagridviewNE1.Rows.Add(x[0].ToString(), x[1].ToString(), x[2].ToString(), x[3].ToString(), x[4].ToString(), x[6].ToString(), x[7].ToString());
                    }
                    //---------------------------------------------------------------------------->                   
                }
                else if(dt == datagridviewNE2)
                {
                    if (!periodo)
                    {
                        if (comboBox6.Text != "")
                        {
                            sql.getSalidas(constants.stringToInt(textBox3.Text), dt, comboBox5.Text, tienda_id, "linea", comboBox6.Text);
                        }
                        else if (comboBox7.Text != "")
                        {
                            sql.getSalidas(constants.stringToInt(textBox3.Text), dt, comboBox5.Text, tienda_id, "proveedor", comboBox7.Text);
                        }
                        else
                        {
                            sql.getSalidas(constants.stringToInt(textBox3.Text), dt, comboBox5.Text, tienda_id);
                            comboBox6.Items.Clear();
                            comboBox7.Items.Clear();
                            switch (comboBox5.SelectedIndex)
                            {
                                case 0:
                                    comboBox6.Items.AddRange(constants.getCategorias("vidrio").ToArray());
                                    comboBox7.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                                    break;
                                case 1:
                                    comboBox6.Items.AddRange(constants.getCategorias("aluminio").ToArray());
                                    comboBox7.Items.AddRange(constants.getProveedores("aluminio").ToArray());
                                    break;
                                case 2:
                                    comboBox6.Items.AddRange(constants.getCategorias("herraje").ToArray());
                                    comboBox7.Items.AddRange(constants.getProveedores("herraje").ToArray());
                                    break;
                                case 3:
                                    comboBox6.Items.AddRange(constants.getCategorias("otros").ToArray());
                                    comboBox7.Items.AddRange(constants.getProveedores("otros").ToArray());
                                    break;
                                default: break;
                            }
                        }
                    }
                    else
                    {
                        sql.getSalidasPeriodo(constants.stringToInt(textBox3.Text), dt, comboBox5.Text, tienda_id, dateTimePicker1.Value.ToShortDateString());
                    }
                    //---------------------------------------------------------------------------->  
                    foreach (DataGridViewColumn x in datagridviewNE2.Columns)
                    {
                        if (x.HeaderText == "salidas")
                        {
                            x.DefaultCellStyle.BackColor = Color.Khaki;
                        }
                    }
                }
                else if (dt == datagridviewNE3)
                {
                    if (!periodo)
                    {
                        if (comboBox14.Text != "")
                        {
                            sql.getEntradas(constants.stringToInt(textBox8.Text), dt, comboBox15.Text, tienda_id, "linea", comboBox14.Text);
                        }
                        else if (comboBox13.Text != "")
                        {
                            sql.getEntradas(constants.stringToInt(textBox8.Text), dt, comboBox15.Text, tienda_id, "proveedor", comboBox13.Text);
                        }
                        else
                        {
                            sql.getEntradas(constants.stringToInt(textBox8.Text), dt, comboBox15.Text, tienda_id);
                            comboBox14.Items.Clear();
                            comboBox13.Items.Clear();
                            switch (comboBox15.SelectedIndex)
                            {
                                case 0:
                                    comboBox14.Items.AddRange(constants.getCategorias("vidrio").ToArray());
                                    comboBox13.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                                    break;
                                case 1:
                                    comboBox14.Items.AddRange(constants.getCategorias("aluminio").ToArray());
                                    comboBox13.Items.AddRange(constants.getProveedores("aluminio").ToArray());
                                    break;
                                case 2:
                                    comboBox14.Items.AddRange(constants.getCategorias("herraje").ToArray());
                                    comboBox13.Items.AddRange(constants.getProveedores("herraje").ToArray());
                                    break;
                                case 3:
                                    comboBox14.Items.AddRange(constants.getCategorias("otros").ToArray());
                                    comboBox13.Items.AddRange(constants.getProveedores("otros").ToArray());
                                    break;
                                default: break;
                            }
                        }
                    }
                    else
                    {
                        sql.getEntradasPeriodo(constants.stringToInt(textBox8.Text), dt, comboBox15.Text, tienda_id, dateTimePicker4.Value.ToShortDateString());
                    }
                    //----------------------------------------------------------------------------> 
                    foreach(DataGridViewColumn x in datagridviewNE3.Columns)
                    {
                        if(x.HeaderText == "entradas")
                        {
                            x.DefaultCellStyle.BackColor = Color.Khaki;
                        }
                    }                 
                }
                //------------------------->
                countRows(dt);                
            }
            else
            {
                MessageBox.Show(this, "[Error] se ha producido un error, favor de intentar de nuevo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            bool found = false;
            datagridviewNE table = datagridviewNE1;           
            if (table != null)
            {
                if (textBox1.Text != "" && comboBox4.Text != "")
                {
                    find_next.Clear();
                    foreach (DataGridViewRow x in table.Rows)
                    {
                        foreach (DataGridViewCell v in x.Cells)
                        {
                            if (v.OwningColumn.HeaderText == comboBox4.Text)
                            {
                                if (v.Value.ToString().Equals(textBox1.Text) == true)
                                {
                                    v.Selected = true;
                                    found = true;
                                    find_next.Add(x.Index);
                                    table.FirstDisplayedScrollingRowIndex = x.Index;
                                    break;
                                }
                                else if (v.Value.ToString().StartsWith(textBox1.Text) == true)
                                {
                                    v.Selected = true;
                                    found = true;
                                    find_next.Add(x.Index);
                                    table.FirstDisplayedScrollingRowIndex = x.Index;
                                    break;
                                }
                                else
                                {
                                    v.Selected = false;
                                }
                            }
                        }
                        if (found == true)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    table.ClearSelection();
                }
            }
        }

        private bool getNext(int row)
        {
            bool r = false;
            foreach (int x in find_next)
            {
                if (x == row)
                {
                    r = true;
                }
            }
            return r;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            findNext(datagridviewNE1, textBox1, comboBox4);
        }

        //set visto
        private void setVisto(bool visto, int row, int cell)
        {
            if (visto == true)
            {
                datagridviewNE1.Rows[row].Cells[cell].Style.BackColor = Color.Yellow;
            }
            else
            {
                datagridviewNE1.Rows[row].Cells[cell].Style.BackColor = Color.LightGreen;
            }
        }

        private void pegadoEspecialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            constants.PasteOnGrid(datagridviewNE1, true, true);
        }

        private void borrarRegistroToolStripMenuItem_Click(object sender, EventArgs e)
        {        
            if (datagridviewNE1.CurrentRow.Cells[0].Value != null)
            {
                int id = constants.stringToInt(datagridviewNE1.CurrentRow.Cells[0].Value.ToString());
                string clave = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                if (id > 0)
                {
                    if (MessageBox.Show(this, "¿Estas seguro de eliminar este registro de forma permanente?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (!backgroundWorker1.IsBusy || !backgroundWorker2.IsBusy || !backgroundWorker3.IsBusy || !backgroundWorker4.IsBusy)
                        {
                            pictureBox1.Visible = true;
                            backgroundWorker2.RunWorkerAsync(id);
                        }
                    }
                }
                else
                {
                    if (datagridviewNE1.CurrentRow.Index < datagridviewNE1.Rows.Count - 1)
                    {
                        datagridviewNE1.Rows.Remove(datagridviewNE1.CurrentRow);
                    }
                }
            }            
        }

        //Delete
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            int id = (int)e.Argument;
            if (id > 0)
            {
                new sqlDateBaseManager().deleteRegistroInventario(id);
            }
            else
            {
                new sqlDateBaseManager().deleteInventario();
            }
        }

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
            executeLoad();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            executeLoad();
        }

        private Boolean noRepeatIndex(string clave, List<string> list)
        {
            Boolean r = false;
            foreach (string x in list)
            {
                if (x == clave)
                {
                    r = true;
                    break;
                }
            }
            return r;
        }

        //Subir
        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "¿Deseas actualizar el registro?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (!backgroundWorker1.IsBusy || !backgroundWorker2.IsBusy || !backgroundWorker3.IsBusy || !backgroundWorker4.IsBusy)
                {
                    pictureBox1.Visible = true;
                    backgroundWorker3.RunWorkerAsync();
                }
            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            int tienda_id = sql.getTiendaID(constants.org_name);

            if (tienda_id > 0)
            {
                tabControl1.SelectedIndex = 0;
                string clave = string.Empty;
                string articulo = string.Empty;
                if (update_table != null)
                {
                    foreach (DataRow x in update_table.Rows)
                    {
                        sql.updateInventario(constants.stringToInt(x[0].ToString()), x[2].ToString(), x[3].ToString(), x[4].ToString(), x[5].ToString());
                    }
                }
                //-------------------->
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    if (x.Cells[0].Value.ToString() == "-1")
                    {
                        if (x.Cells[1].Value != null && x.Cells[2].Value != null)
                        {
                            clave = x.Cells[1].Value.ToString();
                            articulo = x.Cells[2].Value.ToString();
                            if (clave != "" && articulo != "")
                            {
                                if (sql.findSQLValue("clave", "clave", "inventario", clave) == false)
                                {
                                    sql.newInventario(clave, articulo, x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), comboBox1.Text, x.Cells[5].Value.ToString(), 0, tienda_id);
                                }
                                else
                                {
                                    MessageBox.Show(this, "[Error] la clave " + clave + " ya existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(this, "[Error] se ha producido un error, favor de intentar de nuevo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }           
        }

        private void countRows(datagridviewNE table)
        {            
            if(table == null)
            {
                label12.Text = "Se encontrarón (" + datagridviewNE1.RowCount + ") registros.";
            }
            else if (table == datagridviewNE2)
            {
                label26.Text = "Se encontrarón (" + table.RowCount + ") registros.";
            }
            else if (table == datagridviewNE3)
            {
                label31.Text = "Se encontrarón (" + table.RowCount + ") registros.";
            }
        }

        private void BackgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
            executeLoad();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox2.SelectedIndex >= 0)
            {
                executeLoad();
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex >= 0)
            {
                executeLoad();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (data != null)
            {
                new print_inventarios(data, comboBox1.Text).ShowDialog(this);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            bool found = false;
            datagridviewNE table = datagridviewNE2;
            if (table != null)
            {
                if (textBox2.Text != "" && comboBox8.Text != "")
                {
                    find_next.Clear();
                    foreach (DataGridViewRow x in table.Rows)
                    {
                        foreach (DataGridViewCell v in x.Cells)
                        {
                            if (v.OwningColumn.HeaderText == comboBox8.Text)
                            {
                                if (v.Value.ToString().Equals(textBox2.Text) == true)
                                {
                                    v.Selected = true;
                                    found = true;
                                    find_next.Add(x.Index);
                                    table.FirstDisplayedScrollingRowIndex = x.Index;
                                    break;
                                }
                                else if (v.Value.ToString().StartsWith(textBox2.Text) == true)
                                {
                                    v.Selected = true;
                                    found = true;
                                    find_next.Add(x.Index);
                                    table.FirstDisplayedScrollingRowIndex = x.Index;
                                    break;
                                }
                                else
                                {
                                    v.Selected = false;
                                }
                            }
                        }
                        if (found == true)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    table.ClearSelection();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            findNext(datagridviewNE2, textBox2, comboBox8);
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            executeLoad(datagridviewNE2);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (data != null)
            {
                new print_inventarios(datagridviewNE2.DataSource as DataTable, comboBox5.Text, "salidas").ShowDialog(this);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            executeLoad(datagridviewNE2);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            executeLoad(datagridviewNE2, true);
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox6.SelectedIndex >= 0)
            {
                executeLoad(datagridviewNE2);
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox7.SelectedIndex >= 0)
            {
                executeLoad(datagridviewNE2);
            }
        }

        private void PEPS(string lista, string peps, string clave, string articulo, string linea, string proveedor, string costeo)
        {
            textBox11.Text = lista;
            comboBox9.Text = peps;
            textBox10.Text = clave;
            textBox4.Text = articulo;
            textBox5.Text = linea;
            textBox6.Text = proveedor;
            textBox12.Text = costeo;
            textBox7.Clear();
            tabControl1.SelectedIndex = 3;
            textBox7.Focus();

        }

        private void salidaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(datagridviewNE1.Rows.Count > 0)
            {
                if (constants.stringToFloat(datagridviewNE1.CurrentRow.Cells[6].Value.ToString()) > 0)
                {
                    PEPS(comboBox1.Text, "Salida", datagridviewNE1.CurrentRow.Cells[1].Value.ToString(), datagridviewNE1.CurrentRow.Cells[2].Value.ToString(), datagridviewNE1.CurrentRow.Cells[3].Value.ToString(), datagridviewNE1.CurrentRow.Cells[4].Value.ToString(), datagridviewNE1.CurrentRow.Cells[5].Value.ToString());
                }
                else
                {
                    MessageBox.Show(this, "[Error] no existe suficiente número de existencias.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void entradaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                PEPS(comboBox1.Text, "Entrada", datagridviewNE1.CurrentRow.Cells[1].Value.ToString(), datagridviewNE1.CurrentRow.Cells[2].Value.ToString(), datagridviewNE1.CurrentRow.Cells[3].Value.ToString(), datagridviewNE1.CurrentRow.Cells[4].Value.ToString(), datagridviewNE1.CurrentRow.Cells[5].Value.ToString());
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void resetForm()
        {
            textBox11.Clear();
            comboBox9.SelectedIndex = -1;
            textBox10.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox12.Clear();
            richTextBox1.Clear();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy || !backgroundWorker2.IsBusy || !backgroundWorker3.IsBusy || !backgroundWorker4.IsBusy)
            {
                if (textBox11.Text != "")
                {
                    if (comboBox9.Text != "")
                    {
                        if (textBox10.Text != "")
                        {
                            DialogResult r = MessageBox.Show(this, "Se generara una " + comboBox9.Text + " por una cantidad de " + constants.stringToFloat(textBox7.Text) + " unidades. \n\n ¿Deseas continuar?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (r == DialogResult.Yes)
                            {
                                pictureBox1.Visible = true;
                                backgroundWorker4.RunWorkerAsync();
                            }
                        }
                        else
                        {
                            MessageBox.Show(this, "[Error] se necesita seleccionar un artículo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "[Error] se necesita seleccionar un tipo de entrada/salida.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(this, "[Error] se necesita seleccionar un tipo de lista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void comboBox15_SelectedIndexChanged(object sender, EventArgs e)
        {
            executeLoad(datagridviewNE3);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            executeLoad(datagridviewNE3);
        }

        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox14.SelectedIndex >= 0)
            {
                executeLoad(datagridviewNE3);
            }
        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox13.SelectedIndex >= 0)
            {
                executeLoad(datagridviewNE3);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (data != null)
            {
                new print_inventarios(data, comboBox15.Text, "entradas").ShowDialog(this);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            executeLoad(datagridviewNE3, true);
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            bool found = false;
            datagridviewNE table = datagridviewNE3;
            if (table != null)
            {
                if (textBox9.Text != "" && comboBox12.Text != "")
                {
                    find_next.Clear();
                    foreach (DataGridViewRow x in table.Rows)
                    {
                        foreach (DataGridViewCell v in x.Cells)
                        {
                            if (v.OwningColumn.HeaderText == comboBox12.Text)
                            {
                                if (v.Value.ToString().Equals(textBox9.Text) == true)
                                {
                                    v.Selected = true;
                                    found = true;
                                    find_next.Add(x.Index);
                                    table.FirstDisplayedScrollingRowIndex = x.Index;
                                    break;
                                }
                                else if (v.Value.ToString().StartsWith(textBox9.Text) == true)
                                {
                                    v.Selected = true;
                                    found = true;
                                    find_next.Add(x.Index);
                                    table.FirstDisplayedScrollingRowIndex = x.Index;
                                    break;
                                }
                                else
                                {
                                    v.Selected = false;
                                }
                            }
                        }
                        if (found == true)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    table.ClearSelection();
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            findNext(datagridviewNE3, textBox9, comboBox12);
        }

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            float new_cant = constants.stringToFloat(textBox7.Text);
            float existencias = 0;
            e.Result = false;

            if (comboBox9.Text == "Salida")
            {
                int tienda = sql.getTiendaID(constants.org_name);
                if (tienda > 0)
                {
                    existencias = sql.getExistencia(textBox10.Text);
                    if (existencias >= new_cant)
                    {
                        sql.newSalida(textBox10.Text, textBox4.Text, textBox5.Text, textBox6.Text, textBox11.Text, new_cant, DateTime.Today.ToString("dd/MM/yyyy"), tienda, richTextBox1.Text);
                        sql.updateExistencias(textBox10.Text, -1 * constants.stringToFloat(textBox7.Text));
                        existencias = sql.getExistencia(textBox10.Text);
                        MessageBox.Show(this, "Se ha registrado correctamente. \n\n Se retiraron: (" + new_cant + ") " + textBox12.Text + " al inventario.\n Existencias disponibles para el artículo " + textBox10.Text + " suman: (" + existencias + ") " + textBox12.Text, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Result = true;
                    }
                    else
                    {
                        MessageBox.Show(this, "[Error] no existe suficiente número de existencias.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(this, "[Error] se ha producido un error, favor de intentar de nuevo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                int tienda = sql.getTiendaID(constants.org_name);
                if (tienda > 0)
                {
                    sql.newEntrada(textBox10.Text, textBox4.Text, textBox5.Text, textBox6.Text, textBox11.Text, new_cant, DateTime.Today.ToString("dd/MM/yyyy"), tienda, richTextBox1.Text);
                    sql.updateExistencias(textBox10.Text, constants.stringToFloat(textBox7.Text));
                    existencias = sql.getExistencia(textBox10.Text);
                    MessageBox.Show(this, "Se ha registrado correctamente. \n\n Se añadieron: (" + new_cant + ") " + textBox12.Text + " al inventario.\n Existencias disponibles para el artículo " + textBox10.Text + " suman: (" + existencias + ") " + textBox12.Text, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Result = true;
                }
                else
                {
                    MessageBox.Show(this, "[Error] se ha producido un error, favor de intentar de nuevo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BackgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
            bool s = (bool)e.Result;
            if (s)
            {
                resetForm();
            }
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox9.Text == "Salida")
            {
                pictureBox6.Image = Properties.Resources.salidas_arrow;
            }
            else if(comboBox9.Text == "Entrada")
            {
                pictureBox6.Image = Properties.Resources.entradas_arrow;
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            textBox1.Focus();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            executeLoad(datagridviewNE2, true);
        }

        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {
            executeLoad(datagridviewNE3, true);
        }

        private void TextBox9_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                findNext(datagridviewNE3, textBox9, comboBox12);
            }
        }

        private void TextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                findNext(datagridviewNE2, textBox2, comboBox8);
            }
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                findNext(datagridviewNE1, textBox1, comboBox4);
            }
        }

        private void findNext(datagridviewNE table, TextBox text, ComboBox param)
        {
            bool found = false;
            if (table != null)
            {
                if (text.Text != "" && param.Text != "")
                {
                    foreach (DataGridViewRow x in table.Rows)
                    {
                        foreach (DataGridViewCell v in x.Cells)
                        {
                            if (getNext(x.Index) == false)
                            {
                                if (v.OwningColumn.HeaderText == param.Text)
                                {
                                    if (v.Value.ToString().Equals(text.Text) == true)
                                    {
                                        v.Selected = true;
                                        found = true;
                                        table.FirstDisplayedScrollingRowIndex = x.Index;
                                        find_next.Add(x.Index);
                                        break;
                                    }
                                    else if (v.Value.ToString().StartsWith(text.Text) == true)
                                    {
                                        v.Selected = true;
                                        found = true;
                                        table.FirstDisplayedScrollingRowIndex = x.Index;
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
                    if (found == false)
                    {
                        find_next.Clear();
                        MessageBox.Show(this, "No existen mas resultados de búsqueda.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
    }
}
