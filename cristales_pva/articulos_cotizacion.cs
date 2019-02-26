using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace cristales_pva
{
    public partial class articulos_cotizacion : Form
    {
        bool reload = false;
        int row = -1;

        public articulos_cotizacion()
        {
            InitializeComponent();
            datagridviewNE1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            datagridviewNE1.CellClick += DatagridviewNE1_CellClick;
            datagridviewNE1.CellLeave += DatagridviewNE1_CellLeave;
            datagridviewNE1.CellEndEdit += DatagridviewNE1_CellEndEdit;
            datagridviewNE1.EditingControlShowing += DatagridviewNE1_EditingControlShowing;
            datagridviewNE1.DataBindingComplete += DatagridviewNE1_DataBindingComplete;
            datagridviewNE1.SortCompare += DatagridviewNE1_SortCompare;
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
            backgroundWorker2.RunWorkerCompleted += BackgroundWorker2_RunWorkerCompleted;
            backgroundWorker3.RunWorkerCompleted += BackgroundWorker3_RunWorkerCompleted;
            this.FormClosed += Articulos_cotizacion_FormClosed;
            textBox1.KeyDown += TextBox1_KeyDown;
            loadFontSize();
            if (Application.OpenForms["edit_express"] != null)
            {
                Application.OpenForms["edit_express"].Close();
            }          
            loadALL();
        }

        private void DatagridviewNE1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            try
            {
                e.SortResult = System.String.Compare(e.CellValue1.ToString(), e.CellValue2.ToString());
                e.Handled = true;
            }
            catch (Exception)
            {
                //do nothing
            }
        }

        private void loadtitles()
        {
            string m = constants.nombre_cotizacion;
            if (constants.nombre_proyecto != string.Empty)
            {
                m = m + " - " + constants.nombre_proyecto;
            }
            if(constants.getSubfoliotitle(constants.sub_folio) != string.Empty)
            {
                if(m.Length > 0)
                {
                    m = m + " - " + constants.getSubfoliotitle(constants.sub_folio);
                }
            }
            label7.Text = m;
        }

        private void DatagridviewNE1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                string[] y = null;
                foreach (string x in constants.save_onEdit)
                {
                    if (datagridviewNE1.Rows.Count > 0)
                    {
                        foreach (DataGridViewRow v in datagridviewNE1.Rows)
                        {
                            y = x.Split('-');
                            if (y.Length == 2)
                            {
                                if (constants.stringToInt(y[1]) == constants.tipo_cotizacion)
                                {
                                    if ((int)v.Cells[0].Value == constants.stringToInt(y[0]))
                                    {
                                        v.Cells[0].Style.BackColor = Color.LightGreen;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //do nohitng
            }
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                switch (constants.tipo_cotizacion)
                {
                    case 1:
                        this.Text = "Cristales Cotizados";
                        constants.loadCotizacionesLocales("cristales", datagridviewNE1, false, true, textBox1.Text);
                        break;
                    case 2:
                        this.Text = "Aluminio Cotizados";
                        constants.loadCotizacionesLocales("aluminio", datagridviewNE1, false, true, textBox1.Text);
                        break;
                    case 3:
                        this.Text = "Herrajes Cotizados";
                        constants.loadCotizacionesLocales("herrajes", datagridviewNE1, false, true, textBox1.Text);
                        break;
                    case 4:
                        this.Text = "Otros Materiales Cotizados";
                        constants.loadCotizacionesLocales("otros", datagridviewNE1, false, true, textBox1.Text);
                        break;
                    case 5:
                        this.Text = "Módulos Cotizados";
                        constants.loadCotizacionesLocales("modulos", datagridviewNE1, true, true, textBox1.Text);
                        break;
                    default: break;
                }
                label2.Text = "Se encontrarón: (" + datagridviewNE1.RowCount + ") partidas.";
            }
        }

        public void checkErrorsModulos()
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    foreach (int v in constants.errors_Open)
                    {
                        if (v == constants.stringToInt(x.Cells[0].Value.ToString()))
                        {
                            x.Cells[0].Style.BackColor = Color.Red;
                        }                       
                    }
                }
            }
        }

        public void loadALL()
        {
            try
            {
                datagridviewNE1.DataSource = null;
                datagridviewNE1.Rows.Clear();
                datagridviewNE1.Columns.Clear();
                switch (constants.tipo_cotizacion)
                {
                    case 1:
                        this.Text = "Cristales Cotizados";
                        constants.loadCotizacionesLocales("cristales", datagridviewNE1);
                        button7.Visible = false;
                        button8.Visible = false;
                        break;
                    case 2:
                        this.Text = "Aluminio Cotizados";
                        constants.loadCotizacionesLocales("aluminio", datagridviewNE1);
                        button7.Visible = false;
                        button8.Visible = false;
                        break;
                    case 3:
                        this.Text = "Herrajes Cotizados";
                        constants.loadCotizacionesLocales("herrajes", datagridviewNE1);
                        button7.Visible = false;
                        button8.Visible = false;
                        break;
                    case 4:
                        this.Text = "Otros Materiales Cotizados";
                        constants.loadCotizacionesLocales("otros", datagridviewNE1);
                        button7.Visible = false;
                        button8.Visible = false;
                        break;
                    case 5:
                        this.Text = "Módulos Cotizados";
                        constants.loadCotizacionesLocales("modulos", datagridviewNE1, true);
                        checkErrorsModulos();
                        button7.Visible = true;
                        button8.Visible = true;
                        break;
                    default: break;
                }
                label2.Text = "Se encontrarón: (" + datagridviewNE1.RowCount + ") partidas.";
                selectRow();
                string[] r = ((Form1)Application.OpenForms["form1"]).getTotalAndDescount();
                label3.Text = "$ " + r[0] + " (-" + r[1] + "%)";
                label5.Text = "Sub-Folio: " + constants.sub_folio;
                label6.Text = constants.sub_folio.ToString();
                loadtitles();
                if (constants.tipo_cotizacion == 5)
                {
                    string[] y = null;
                    foreach (string x in constants.save_onEdit)
                    {
                        if (datagridviewNE1.Rows.Count > 0)
                        {
                            foreach (DataGridViewRow v in datagridviewNE1.Rows)
                            {
                                y = x.Split('-');
                                if (y.Length == 2)
                                {
                                    if (constants.stringToInt(y[1]) == constants.tipo_cotizacion)
                                    {
                                        if ((int)v.Cells[0].Value == constants.stringToInt(y[0]))
                                        {
                                            v.Cells[0].Style.BackColor = Color.LightGreen;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }           
            }
            catch (Exception) { }        
        }

        //Set ONLY articulos to uppercase
        private void DatagridviewNE1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (datagridviewNE1.CurrentCell.OwningColumn.HeaderText == "Artículo" || datagridviewNE1.CurrentCell.OwningColumn.HeaderText == "Ubicación")
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

        public void reloadModulos()
        {
            constants.loadCotizacionesLocales("modulos", datagridviewNE1, true);
            label2.Text = "Se encontrarón: (" + datagridviewNE1.RowCount + ") partidas.";
            selectRow();
        }

        private void Articulos_cotizacion_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms["change_colors"] != null)
            {
                Application.OpenForms["change_colors"].Close();
            }
            if (backgroundWorker1.IsBusy == false)
            {
                this.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
            }           
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (constants.tipo_cotizacion != 5)
            {
                contextMenuStrip1.Items[2].Visible = false;
                contextMenuStrip1.Items[3].Visible = false;
                contextMenuStrip1.Items[4].Visible = false;
                contextMenuStrip1.Items[5].Visible = false;
                contextMenuStrip1.Items[7].Visible = false;
                if (datagridviewNE1.RowCount > 0)
                {
                    contextMenuStrip1.Items[0].Visible = true;
                    contextMenuStrip1.Items[1].Visible = true;
                    contextMenuStrip1.Items[6].Visible = true;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                if (datagridviewNE1.RowCount > 0)
                {
                    contextMenuStrip1.Items[0].Visible = true;
                    contextMenuStrip1.Items[1].Visible = true;
                    contextMenuStrip1.Items[5].Visible = true;
                    contextMenuStrip1.Items[6].Visible = true;
                    if (datagridviewNE1.CurrentRow.Cells[5].Value.ToString() == "-1")
                    {
                        contextMenuStrip1.Items[3].Visible = true;
                    }
                    else
                    {
                        contextMenuStrip1.Items[3].Visible = false;
                    }
                    if (datagridviewNE1.CurrentRow.Cells[5].Value.ToString() != "-2")
                    {
                        contextMenuStrip1.Items[7].Visible = true;
                    }
                    else
                    {
                        contextMenuStrip1.Items[7].Visible = false;
                    }
                }
                else
                {
                    contextMenuStrip1.Items[0].Visible = false;
                    contextMenuStrip1.Items[1].Visible = false;
                    contextMenuStrip1.Items[5].Visible = false;
                    contextMenuStrip1.Items[6].Visible = false;
                    contextMenuStrip1.Items[7].Visible = false;
                }
                contextMenuStrip1.Items[2].Visible = true;               
                contextMenuStrip1.Items[4].Visible = true;
            }
        }

        private void DatagridviewNE1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (constants.tipo_cotizacion == 5)
            {
                if (datagridviewNE1.RowCount > 0)
                {                 
                    if (backgroundWorker2.IsBusy == false)
                    {
                        pictureBox1.Visible = true;
                        object[] arguments = new object[3];
                        if (datagridviewNE1.CurrentCell.ColumnIndex == 1)
                        {
                            if (datagridviewNE1.CurrentCell.Value != null)
                            {
                                if (constants.stringToInt(datagridviewNE1.CurrentCell.Value.ToString()) > 0)
                                {
                                    datagridviewNE1.CurrentCell.Value = constants.stringToInt(datagridviewNE1.CurrentCell.Value.ToString());
                                }
                                else
                                {
                                    datagridviewNE1.CurrentCell.Value = 0;
                                }                               
                                arguments[0] = 0;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                            else
                            {
                                datagridviewNE1.CurrentCell.Value = 0;
                                arguments[0] = 0;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                            //Sort
                        }
                        if (datagridviewNE1.CurrentCell.ColumnIndex == 6)
                        {
                            if(datagridviewNE1.CurrentCell.Value != null)
                            {
                                arguments[0] = 1;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                            else
                            {
                                datagridviewNE1.CurrentCell.Value = "";
                                arguments[0] = 1;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                        }
                        else if(datagridviewNE1.CurrentCell.ColumnIndex == 7)
                        {
                            if (datagridviewNE1.CurrentCell.Value != null)
                            {
                                arguments[0] = 2;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                            else
                            {
                                datagridviewNE1.CurrentCell.Value = "";
                                arguments[0] = 2;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                        }
                        else if (datagridviewNE1.CurrentCell.ColumnIndex == 8)
                        {
                            if (datagridviewNE1.CurrentCell.Value != null)
                            {
                                arguments[0] = 3;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                            else
                            {
                                datagridviewNE1.CurrentCell.Value = "";
                                arguments[0] = 3;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                        }
                        else if (datagridviewNE1.CurrentCell.ColumnIndex == 9)
                        {
                            if (datagridviewNE1.CurrentCell.Value != null)
                            {
                                arguments[0] = 4;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                            else
                            {
                                datagridviewNE1.CurrentCell.Value = "";
                                arguments[0] = 4;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                        }
                        else if (datagridviewNE1.CurrentCell.ColumnIndex == 10)
                        {
                            if (datagridviewNE1.CurrentCell.Value != null)
                            {
                                arguments[0] = 5;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                            else
                            {
                                datagridviewNE1.CurrentCell.Value = "";
                                arguments[0] = 5;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                        }
                        else if (datagridviewNE1.CurrentCell.ColumnIndex == 11)
                        {
                            if (datagridviewNE1.CurrentCell.Value != null)
                            {
                                arguments[0] = 6;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                            else
                            {
                                datagridviewNE1.CurrentCell.Value = "";
                                arguments[0] = 6;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                        }
                        else if (datagridviewNE1.CurrentCell.ColumnIndex == 12)
                        {
                            if (datagridviewNE1.CurrentCell.Value != null)
                            {
                                arguments[0] = 7;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                            else
                            {
                                datagridviewNE1.CurrentCell.Value = "";
                                arguments[0] = 7;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                        }
                        else if (datagridviewNE1.CurrentCell.ColumnIndex == 13)
                        {
                            if (datagridviewNE1.CurrentCell.Value != null)
                            {
                                if (constants.stringToInt(datagridviewNE1.CurrentCell.Value.ToString()) > 0)
                                {
                                    datagridviewNE1.CurrentCell.Value = constants.stringToInt(datagridviewNE1.CurrentCell.Value.ToString());
                                }
                                else
                                {
                                    datagridviewNE1.CurrentCell.Value = 0;
                                }
                                arguments[0] = 8;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                            else
                            {
                                datagridviewNE1.CurrentCell.Value = 0;
                                arguments[0] = 8;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                        }
                        else if (datagridviewNE1.CurrentCell.ColumnIndex == 14)
                        {
                            if (datagridviewNE1.CurrentCell.Value != null)
                            {
                                if (constants.stringToInt(datagridviewNE1.CurrentCell.Value.ToString()) > 0)
                                {
                                    datagridviewNE1.CurrentCell.Value = constants.stringToInt(datagridviewNE1.CurrentCell.Value.ToString());
                                }
                                else
                                {
                                    datagridviewNE1.CurrentCell.Value = 0;
                                }
                                arguments[0] = 9;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                            else
                            {
                                datagridviewNE1.CurrentCell.Value = 0;
                                arguments[0] = 9;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                        }
                        else if (datagridviewNE1.CurrentCell.ColumnIndex == 15)
                        {
                            if (datagridviewNE1.CurrentCell.Value != null)
                            {
                                if (constants.stringToInt(datagridviewNE1.CurrentCell.Value.ToString()) > 0)
                                {
                                    datagridviewNE1.CurrentCell.Value = constants.stringToInt(datagridviewNE1.CurrentCell.Value.ToString());
                                }
                                else
                                {
                                    datagridviewNE1.CurrentCell.Value = 0;
                                }
                                arguments[0] = 10;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                            else
                            {
                                datagridviewNE1.CurrentCell.Value = 0;
                                arguments[0] = 10;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                        }
                        else if (datagridviewNE1.CurrentCell.ColumnIndex == 16)
                        {
                            if (datagridviewNE1.CurrentCell.Value != null)
                            {
                                if (constants.stringToFloat(datagridviewNE1.CurrentCell.Value.ToString()) > 0)
                                {
                                    datagridviewNE1.CurrentCell.Value = constants.stringToFloat(datagridviewNE1.CurrentCell.Value.ToString());
                                }
                                else
                                {
                                    datagridviewNE1.CurrentCell.Value = 0;
                                }
                                arguments[0] = 11;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                            else
                            {
                                datagridviewNE1.CurrentCell.Value = 0;
                                arguments[0] = 11;
                                arguments[1] = datagridviewNE1.CurrentRow.Cells[0].Value;
                                arguments[2] = datagridviewNE1.CurrentCell.Value;
                                backgroundWorker2.RunWorkerAsync(arguments);
                            }
                        }
                    }
                }               
            }
        }      

        private void DatagridviewNE1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                datagridviewNE1.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
        }

        private void DatagridviewNE1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                row = datagridviewNE1.CurrentRow.Index;
                datagridviewNE1.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;  
                if(constants.tipo_cotizacion == 5)
                {                 
                    if(datagridviewNE1.CurrentCell.ColumnIndex == 2)
                    {
                        if (Application.OpenForms["cambiar_imagen"] == null)
                        {
                            new cambiar_imagen((int)datagridviewNE1.CurrentRow.Cells[0].Value).ShowDialog();
                        }
                    }                   
                }
            }
        }

        //editar
        private void editarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.tipo_cotizacion == 5)
            {
                if (datagridviewNE1.CurrentRow.Cells[5].Value.ToString() != "-1")
                {
                    this.WindowState = FormWindowState.Minimized;
                }
            }
            else
            {
                this.WindowState = FormWindowState.Minimized;
            }
            ((Form1)Application.OpenForms["Form1"]).setArticuloCotizadoToEdit((int)datagridviewNE1.CurrentRow.Cells[0].Value);            
        }

        //eliminar
        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((Form1)Application.OpenForms["Form1"]).eliminarArticuloCotizado((int)datagridviewNE1.CurrentRow.Cells[0].Value);
            loadALL();
        }     

        //Unificar conceptos
        private void añadirToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (constants.tipo_cotizacion == 5)
            {
                cotizaciones_local cotizaciones = new cotizaciones_local();
                try
                {
                    var modulo_p = new modulos_cotizaciones
                    {
                        folio = 00000,
                        modulo_id = -1,
                        descripcion = "",
                        mano_obra = 0,
                        dimensiones = "",
                        largo = 0,
                        alto = 0,
                        acabado_perfil = "",
                        claves_cristales = "",
                        cantidad = 1,
                        flete = 0,
                        desperdicio = 0,
                        utilidad = 0,
                        articulo = "",
                        linea = "",
                        diseño = "",
                        clave = "md",
                        total = 0,
                        claves_otros = "",
                        claves_herrajes = "",
                        ubicacion = "",
                        claves_perfiles = "",
                        pic = constants.imageToByte(Properties.Resources.new_concepto),
                        merge_id = -1,
                        concept_id = 0,
                        sub_folio = constants.sub_folio,
                        dir = 0,
                        news = "",
                        new_desing = "",
                        orden = constants.getCountPartidas()
                    };
                    cotizaciones.modulos_cotizaciones.Add(modulo_p);
                    cotizaciones.SaveChanges();
                    ((Form1)Application.OpenForms["Form1"]).countCotizacionesArticulo();
                    ((Form1)Application.OpenForms["Form1"]).loadCountArticulos();
                    constants.loadCotizacionesLocales("modulos", datagridviewNE1, true);
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                    MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Añadir a concepto
        private void removerToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                if (Application.OpenForms["merge_items"] == null)
                {
                    new merge_items(false, true, (int)datagridviewNE1.CurrentRow.Cells[0].Value).Show();
                }
                else
                {
                    Application.OpenForms["merge_items"].Select();
                }
            }
        }

        //Nuevo Concepto
        private void nuevoConceptoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.tipo_cotizacion == 5)
            {
                cotizaciones_local cotizaciones = new cotizaciones_local();
                try
                {
                    var modulo_p = new modulos_cotizaciones
                    {
                        folio = 00000,
                        modulo_id = -2,
                        descripcion = "",
                        mano_obra = 0,
                        dimensiones = "",
                        largo = 0,
                        alto = 0,
                        acabado_perfil = "",
                        claves_cristales = "",
                        cantidad = 1,
                        flete = 0,
                        desperdicio = 0,
                        utilidad = 0,
                        articulo = "",
                        linea = "",
                        diseño = "",
                        clave = "md",
                        total = 0,
                        claves_otros = "",
                        claves_herrajes = "",
                        ubicacion = "",
                        claves_perfiles = "",
                        pic = constants.imageToByte(Properties.Resources.concepto_extra),
                        merge_id = -1,
                        concept_id = -1,
                        sub_folio = constants.sub_folio,
                        dir = 0,
                        news = "",
                        new_desing = "",
                        orden = constants.getCountPartidas()
                    };
                    cotizaciones.modulos_cotizaciones.Add(modulo_p);
                    cotizaciones.SaveChanges();
                    ((Form1)Application.OpenForms["Form1"]).countCotizacionesArticulo();
                    ((Form1)Application.OpenForms["Form1"]).loadCountArticulos();
                    constants.loadCotizacionesLocales("modulos", datagridviewNE1, true);
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                    MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Buscar
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            switch (constants.tipo_cotizacion)
            {
                case 1:
                    this.Text = "Cristales Cotizados";
                    constants.loadCotizacionesLocales("cristales", datagridviewNE1, false, true, textBox1.Text);
                    break;
                case 2:
                    this.Text = "Aluminio Cotizados";
                    constants.loadCotizacionesLocales("aluminio", datagridviewNE1, false, true, textBox1.Text);
                    break;
                case 3:
                    this.Text = "Herrajes Cotizados";
                    constants.loadCotizacionesLocales("herrajes", datagridviewNE1, false, true, textBox1.Text);
                    break;
                case 4:
                    this.Text = "Otros Materiales Cotizados";
                    constants.loadCotizacionesLocales("otros", datagridviewNE1, false, true, textBox1.Text);
                    break;
                case 5:
                    this.Text = "Módulos Cotizados";
                    constants.loadCotizacionesLocales("modulos", datagridviewNE1, true, true, textBox1.Text);
                    break;
                default: break;
            }
            label2.Text = "Se encontrarón: (" + datagridviewNE1.RowCount + ") partidas.";
        }

        //Reload all
        private void button1_Click(object sender, EventArgs e)
        {
            loadALL();
        }

        //copybox
        private void copiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((Form1)Application.OpenForms["Form1"]).openCopybox((int)datagridviewNE1.CurrentRow.Cells[0].Value);
        }

        //duplicar concepto
        private void duplicarConceptoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resetRowSelect();
            constants.duplicarConcepto(constants.tipo_cotizacion, (int)datagridviewNE1.CurrentRow.Cells[0].Value);
            ((Form1)Application.OpenForms["Form1"]).reloadAll();
            loadALL();
        }

        //cargar al cerrar
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (backgroundWorker1.IsBusy == false)
            {
                constants.errors_Open.Clear();
                constants.reloadPreciosCotizaciones();
                ((Form1)Application.OpenForms["Form1"]).reloadAll();
            }
        }

        //cargar al modificar
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] a = e.Argument as object[];

            cotizaciones_local cotizaciones = new cotizaciones_local();

            reload = false;
            int id = (int)a[1];

            var concepto = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();

            if (concepto != null)
            {
                switch ((int)a[0])
                {
                    case 0:
                        concepto.orden = constants.stringToInt(a[2].ToString());
                        break;
                    case 1:
                        concepto.articulo = a[2].ToString();
                        break;
                    case 2:
                        concepto.linea = a[2].ToString();
                        break;
                    case 3:
                        concepto.diseño = a[2].ToString();
                        break;
                    case 4:
                        concepto.descripcion = a[2].ToString();
                        break;
                    case 5:
                        concepto.ubicacion = a[2].ToString();
                        break;
                    case 6:
                        concepto.claves_cristales = a[2].ToString();
                        break;
                    case 7:
                        concepto.acabado_perfil = a[2].ToString();
                        break;
                    case 8:
                        concepto.largo = constants.stringToInt(a[2].ToString());
                        break;
                    case 9:
                        concepto.alto = constants.stringToInt(a[2].ToString());
                        break;
                    case 10:                       
                        concepto.cantidad = constants.stringToInt(a[2].ToString());
                        if (concepto.modulo_id != -2)
                        {
                            concepto.total = Math.Round((float)concepto.total * constants.stringToInt(a[2].ToString()), 2);
                            reload = true;
                        }
                        break;
                    case 11:
                        concepto.total = Math.Round(constants.stringToFloat(a[2].ToString()), 2);
                        reload = true;
                        break;
                    default:
                        break;
                }
                cotizaciones.SaveChanges();
                constants.errors_Open.Clear();
                if (reload == true)
                {
                    constants.reloadPreciosCotizaciones();
                    ((Form1)Application.OpenForms["Form1"]).reloadAll();
                }
                else
                {
                    ((Form1)Application.OpenForms["Form1"]).reloadCotizacion();
                }
                //Sort
                if((int)a[0] == 0)
                {
                    datagridviewNE1.Sort(datagridviewNE1.Columns[1], ListSortDirection.Ascending);
                }
            }
        }

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
            if(reload == true)
            {
                loadALL();
            }           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //Imprimir
        private void button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            if (Application.OpenForms["load_report"] == null && Application.OpenForms["reportes"] == null)
            {
                string[] r = ((Form1)Application.OpenForms["form1"]).getDesglose();
                new load_report(r[0], r[1], r[2]).Show();
            }
            else
            {
                if (Application.OpenForms["reportes"] != null)
                {
                    if (Application.OpenForms["reportes"].WindowState == FormWindowState.Minimized)
                    {
                        Application.OpenForms["reportes"].WindowState = FormWindowState.Maximized;
                    }
                    else
                    {
                        Application.OpenForms["reportes"].Select();
                    }
                }
            }
        }

        //Exportar
        private void button4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            if (Application.OpenForms["load_report"] == null && Application.OpenForms["reportes"] == null)
            {
                string[] r = ((Form1)Application.OpenForms["form1"]).getDesglose();
                new load_report(r[0], r[1], r[2]).Show();
            }
            else
            {
                if (Application.OpenForms["reportes"] != null)
                {
                    if (Application.OpenForms["reportes"].WindowState == FormWindowState.Minimized)
                    {
                        Application.OpenForms["reportes"].WindowState = FormWindowState.Maximized;
                    }
                    else
                    {
                        Application.OpenForms["reportes"].Select();
                    }
                }
            }
        }

        //Abrir
        private void button5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            if (constants.local == false)
            {
                new buscar_cotizacion().ShowDialog();
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {
                    Application.OpenForms["articulos_cotizacion"].Select();
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Guardar
        private void button6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            if (constants.local == false)
            {
                if (datagridviewNE1.RowCount > 0)
                {
                    new guardar_cotizacion().ShowDialog();
                }
                else
                {
                    MessageBox.Show("Necesitas ingresar algunos artículos primero.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //set subfolio
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                ((Form1)Application.OpenForms["form1"]).setSubFolio(comboBox1.Text);
                label5.Text = "Sub-Folio: " + constants.sub_folio;
                label6.Text = constants.sub_folio.ToString();
                comboBox1.SelectedIndex = -1;
            }
        }

        //acabado rapido
        private void button7_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                resetRowSelect();
                if (Application.OpenForms["change_colors"] == null)
                {
                    change_colors colors = new change_colors();
                    colors.ShowDialog();
                    colors.Select();
                }
                else
                {
                    if (Application.OpenForms["change_colors"].WindowState == FormWindowState.Minimized)
                    {
                        Application.OpenForms["change_colors"].WindowState = FormWindowState.Maximized;
                    }
                    else
                    {
                        Application.OpenForms["change_colors"].Select();
                    }
                }
            }
            else
            {
                MessageBox.Show("[Error] no hay ningún artículo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void resetRowSelect()
        {
            row = -1;
        }

        private void acabadosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            change_colors colors = new change_colors((int)datagridviewNE1.CurrentRow.Cells[0].Value, datagridviewNE1.CurrentRow.Cells[6].Value.ToString(), datagridviewNE1.CurrentRow.Cells[13].Value.ToString(), datagridviewNE1.CurrentRow.Cells[14].Value.ToString());
            if (Application.OpenForms["change_colors"] == null)
            {
                colors.ShowDialog();
                colors.Select();
            }
            else
            {
                Application.OpenForms["change_colors"].Close();
                colors.ShowDialog();
                colors.Select();
            }
        }

        private void selectRow()
        {
            if (datagridviewNE1.RowCount > 0)
            {
                if (row >= 0)
                {
                    try
                    {
                        datagridviewNE1.FirstDisplayedScrollingRowIndex = row;
                        datagridviewNE1.Rows[row].Selected = true;
                        datagridviewNE1.CurrentCell = datagridviewNE1.Rows[row].Cells[0];
                    }
                    catch (Exception)
                    {
                        resetRowSelect();
                    }
                }
            }
        }

        //Analiticas
        private void button8_Click(object sender, EventArgs e)
        {
            if(Application.OpenForms["analiticas"] == null)
            {
               if(backgroundWorker3.IsBusy == false)
                {
                    pictureBox1.Visible = true;
                    backgroundWorker3.RunWorkerAsync();
                }
            }
            else
            {
                if(Application.OpenForms["analiticas"].WindowState == FormWindowState.Minimized)
                {
                    Application.OpenForms["analiticas"].WindowState = FormWindowState.Normal;
                }
                Application.OpenForms["analiticas"].Select();
            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
        }

        private void BackgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string[] r = ((Form1)Application.OpenForms["form1"]).getDesglose();
            new analiticas(constants.nombre_cotizacion, constants.nombre_proyecto, constants.folio_abierto.ToString(), constants.stringToFloat(r[0]), constants.stringToFloat(r[1]), constants.stringToFloat(r[2]), constants.desc_cotizacion, constants.desc_cant, constants.utilidad_cotizacion).Show();
            pictureBox1.Visible = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                datagridviewNE1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                datagridviewNE1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        private void articulos_cotizacion_Load(object sender, EventArgs e)
        {
            if (constants.tipo_cotizacion == 0)
            {
                MessageBox.Show("No se ha seleccionado un tipo de artículo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    constants.tipo_cotizacion = 1;
                    loadALL();
                    break;
                case 1:
                    constants.tipo_cotizacion = 2;
                    loadALL();
                    break;
                case 2:
                    constants.tipo_cotizacion = 3;
                    loadALL();
                    break;
                case 3:
                    constants.tipo_cotizacion = 4;
                    loadALL();
                    break;
                case 4:
                    constants.tipo_cotizacion = 5;
                    loadALL();
                    break;
                default:
                    break;
            }
            comboBox2.SelectedIndex = -1;
        }

        //Font Size
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            float font_size = constants.stringToFloat(comboBox3.Text);
            datagridviewNE1.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", font_size, GraphicsUnit.Pixel);
            datagridviewNE1.Refresh();

            if (constants.fsconfig != font_size)
            {
                try
                {
                    XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                    var mv = from x in opciones_xml.Descendants("Opciones") select x;

                    foreach (XElement x in mv)
                    {
                        x.SetElementValue("FSCONFIG", comboBox3.Text);
                    }
                    opciones_xml.Save(constants.opciones_xml);
                    constants.fsconfig = font_size;
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                    MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void loadFontSize()
        {
            comboBox3.Text = constants.fsconfig.ToString();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            new subfolio_title().ShowDialog();
        }
    }
}
