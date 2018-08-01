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
    public partial class buscar_cotizacion : Form
    {
        private string org_search = string.Empty;
        int c = 0;
        int div = 25;
        int paginas = 1;
        bool LastPage = true;
        bool load = false;

        public buscar_cotizacion()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;

            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.RunWorkerCompleted += BackgroundWorker2_RunWorkerCompleted;

            backgroundWorker3.WorkerReportsProgress = true;
            backgroundWorker3.RunWorkerCompleted += BackgroundWorker3_RunWorkerCompleted;
            backgroundWorker4.RunWorkerCompleted += BackgroundWorker4_RunWorkerCompleted;
            datagridviewNE1.CellContextMenuStripNeeded += DataGridView1_CellContextMenuStripNeeded;
            datagridviewNE1.CellClick += DataGridView1_CellClick;
            datagridviewNE1.CellLeave += DataGridView1_CellLeave;
            monthCalendar1.DateSelected += MonthCalendar1_DateSelected;
            textBox1.KeyPress += TextBox1_KeyPress;
            this.FormClosing += Buscar_cotizacion_FormClosing;
            setYears();
            comboBox2.Text = getMesName(DateTime.Now.Month.ToString());
            comboBox3.Text = DateTime.Now.Year.ToString();
        }

        private void setYears()
        {
            for(int i = 2017; i <= DateTime.Today.Year; i++)
            {
                comboBox3.Items.Add(i);
            }
        }

        private void Buscar_cotizacion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(backgroundWorker1.IsBusy || backgroundWorker2.IsBusy || backgroundWorker3.IsBusy || backgroundWorker4.IsBusy)
            {
                e.Cancel = true;
            }
        }

        private void DataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                datagridviewNE1.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                datagridviewNE1.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
        }

        private void MonthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false && backgroundWorker4.IsBusy == false)
            {
                label3.Text = "Buscando...";
                textBox2.Text = "0";
                pictureBox1.Visible = true;
                label3.Visible = true;
                datagridviewNE1.Enabled = false;
                backgroundWorker1.RunWorkerAsync(e.Start.ToString("dd/MM/yyyy"));
            }
        }

        private void DataGridView1_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                contextMenuStrip1.Show(MousePosition);
            }
        }

        private void buscar_cotizacion_Load(object sender, EventArgs e)
        {
            if(constants.user_access == 6)
            {
                checkBox1.Visible = true;
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Visible = false;
                checkBox1.Checked = false;
            }
            if (constants.licencia != "DEMO")
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
            else
            {
                if (constants.org_name != string.Empty)
                {
                    comboBox1.Items.Add(constants.org_name);
                    comboBox1.Text = constants.org_name;
                }
            }
            org_search = constants.org_name;
            pictureBox1.Visible = false;
            label3.Visible = false;
            load = true;
            reloadBusqueda();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if(e.Argument != null)
            {
                iniciarBusquedaFechada(e.Argument.ToString(), LastPage);
            }
            else
            {
                iniciarBusqueda(LastPage);
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label3.Text = "";
            pictureBox1.Visible = false;
            label3.Visible = false;
            datagridviewNE1.Enabled = true;
            if(datagridviewNE1.Rows.Count > 0)
            {
                datagridviewNE1.FirstDisplayedScrollingRowIndex = datagridviewNE1.Rows.Count - 1;
            }
        }

        private void iniciarBusqueda(bool lastPage)
        {
            datagridviewNE1.Columns.Clear();
            int offset = 0;
            int p = constants.stringToInt(textBox2.Text);
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if (checkBox1.Checked == false)
            {
                if (textBox1.Text == "")
                {
                    c = sql.countCotizaciones(org_search, false, "", false);
                    paginas = (int)Math.Ceiling((float)c / div);
                    label4.Text = "Páginas: " + (paginas);
                    label2.Text = "Total de registros en (" + constants.user + "): " + c;
                    if (lastPage == false)
                    {
                        p = p > 0 ? p - 1 : 0;
                        offset = p;
                    }
                    else
                    {
                        if (paginas > 0)
                        {
                            offset = paginas - 1;
                        }
                        else
                        {
                            offset = 0;
                        }
                        textBox2.Text = paginas.ToString();
                    }
                    sql.dropTableCotizacionesOnGridView(datagridviewNE1, org_search, false, "", false, offset * div, div);
                }
                else
                {
                    c = sql.countCotizaciones(org_search, false, textBox1.Text, true);
                    paginas = (int)Math.Ceiling((float)c / div);
                    label4.Text = "Páginas: " + (paginas);
                    label2.Text = "Total de registros en (" + constants.user + "): " + c;
                    if (lastPage == false)
                    {
                        p = p > 0 ? p - 1 : 0;
                        offset = p;
                    }
                    else
                    {
                        if (paginas > 0)
                        {
                            offset = paginas - 1;
                        }
                        else
                        {
                            offset = 0;
                        }
                        textBox2.Text = paginas.ToString();
                    }
                    sql.dropTableCotizacionesOnGridView(datagridviewNE1, org_search, false, textBox1.Text, true, offset * div, div);
                }
            }
            else
            {
                if (textBox1.Text == "")
                {
                    c = sql.countCotizaciones(org_search, true, "", false);
                    paginas = (int)Math.Ceiling((float)c / div);
                    label4.Text = "Páginas: " + (paginas);
                    label2.Text = "Total de registros: " + c;
                    if (lastPage == false)
                    {
                        p = p > 0 ? p - 1 : 0;
                        offset = p;
                    }
                    else
                    {
                        if (paginas > 0)
                        {
                            offset = paginas - 1;
                        }
                        else
                        {
                            offset = 0;
                        }
                        textBox2.Text = paginas.ToString();
                    }
                    sql.dropTableCotizacionesOnGridView(datagridviewNE1, org_search, true, "", false, offset * div, div);
                }
                else
                {
                    c = sql.countCotizaciones(org_search, true, textBox1.Text, true);
                    paginas = (int)Math.Ceiling((float)c / div);
                    label4.Text = "Páginas: " + (paginas);
                    label2.Text = "Total de registros: " + c;
                    if (lastPage == false)
                    {
                        p = p > 0 ? p - 1 : 0;
                        offset = p;
                    }
                    else
                    {
                        if (paginas > 0)
                        {
                            offset = paginas - 1;
                        }
                        else
                        {
                            offset = 0;
                        }
                        textBox2.Text = paginas.ToString();
                    }
                    sql.dropTableCotizacionesOnGridView(datagridviewNE1, org_search, true, textBox1.Text, true, offset * div, div);
                }
            }
            setIndicador();
        }

        private void iniciarBusquedaFechada(string date, bool lastPage)
        {
            datagridviewNE1.Columns.Clear();
            int offset = 0;
            int p = constants.stringToInt(textBox2.Text);
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if (checkBox1.Checked == false)
            {
                c = sql.countCotizacionesFecha(date, org_search);
                paginas = (int)Math.Ceiling((float)c / div);
                label4.Text = "Páginas: " + (paginas);
                label2.Text = "Total de registros en (" + constants.user + "): " + c;
                if (lastPage == false)
                {
                    p = p > 0 ? p - 1 : 0;
                    offset = p;
                }
                else
                {
                    if (paginas > 0)
                    {
                        offset = paginas - 1;
                    }
                    else
                    {
                        offset = 0;
                    }
                    textBox2.Text = paginas.ToString();
                }
                sql.dropTableCotizacionesOnGridViewFromDate(datagridviewNE1, date, org_search, false, offset * div, div);
            }
            else
            {
                c = sql.countCotizacionesFecha(date, org_search, true);
                paginas = (int)Math.Ceiling((float)c / div);
                label4.Text = "Páginas: " + (paginas);
                label2.Text = "Total de registros: " + c;
                if (lastPage == false)
                {
                    p = p > 0 ? p - 1 : 0;
                    offset = p;
                }
                else
                {
                    if (paginas > 0)
                    {
                        offset = paginas - 1;
                    }
                    else
                    {
                        offset = 0;
                    }
                    textBox2.Text = paginas.ToString();
                }
                sql.dropTableCotizacionesOnGridViewFromDate(datagridviewNE1, date, org_search, true, offset * div, div);
            }
            setIndicador();
        }

        private void setIndicador()
        {
            foreach(DataGridViewRow x in datagridviewNE1.Rows)
            {
                if(x.Cells[6].Value.ToString() == "Registrada")
                {
                    x.Cells[6].Style.BackColor = Color.LightGreen;
                }
                else
                {
                    x.Cells[6].Style.BackColor = Color.Red;
                }
            }          
        }

        //BOTON BUSCAR -------------------------------->
        private void button1_Click(object sender, EventArgs e)
        {
            buscarCotizacion();     
        }
        //

        private void buscarCotizacion()
        {
            if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false && backgroundWorker4.IsBusy == false)
            {
                label3.Text = "Buscando...";
                LastPage = true;
                textBox2.Text = "0";
                pictureBox1.Visible = true;
                label3.Visible = true;
                datagridviewNE1.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        //ELIMINAR COTIZACION ------------------------->
        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false && backgroundWorker4.IsBusy == false)
            {
                DialogResult r = MessageBox.Show(this, "¿Estás seguro de eliminar de forma permanente esta cotización?", constants.msg_box_caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (r == DialogResult.Yes)
                {
                    new confirm_password().ShowDialog();
                }
            }
        }

        public void confirmarEliminacion()
        {
            label3.Text = "Eliminando Cotización...";
            pictureBox1.Visible = true;
            label3.Visible = true;
            datagridviewNE1.Enabled = false;
            constants.folio_eliminacion = (int)datagridviewNE1.CurrentRow.Cells[1].Value;
            backgroundWorker2.RunWorkerAsync();
        }
        //

        //ABRIR COTIZACION ---------------------------------------------------------------------------------------------------------------------->
        private void verToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false && backgroundWorker4.IsBusy == false)
            {
                if (constants.cotizacion_proceso == true)
                {
                    DialogResult r = MessageBox.Show(this, "Existe una cotización en progreso. ¿Desea guardarla?", constants.msg_box_caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if(r == DialogResult.Yes)
                    {
                        new guardar_cotizacion().ShowDialog();
                    }
                    else if(r == DialogResult.No)
                    {
                        label3.Text = "Cargando...";
                        pictureBox1.Visible = true;
                        label3.Visible = true;
                        datagridviewNE1.Enabled = false;
                        constants.sub_folio = 1;
                        ((Form1)Application.OpenForms["form1"]).setSubFolioLabel();
                        constants.folio_abierto = (int)datagridviewNE1.CurrentRow.Cells[1].Value;
                        constants.nombre_cotizacion = datagridviewNE1.CurrentRow.Cells[2].Value.ToString();
                        constants.nombre_proyecto = datagridviewNE1.CurrentRow.Cells[5].Value.ToString();
                        constants.fecha_cotizacion = datagridviewNE1.CurrentRow.Cells[3].Value.ToString();
                        constants.autor_cotizacion = datagridviewNE1.CurrentRow.Cells[4].Value.ToString();
                        sqlDateBaseManager sql = new sqlDateBaseManager();
                        constants.desc_cotizacion = constants.stringToFloat(sql.getSingleSQLValue("cotizaciones", "descuento", "folio", constants.folio_abierto.ToString(), 0));
                        constants.utilidad_cotizacion = constants.stringToFloat(sql.getSingleSQLValue("cotizaciones", "utilidad", "folio", constants.folio_abierto.ToString(), 0));
                        constants.setClienteToPropiedades(constants.folio_abierto, constants.nombre_cotizacion, constants.nombre_proyecto, constants.desc_cotizacion, constants.utilidad_cotizacion);
                        constants.iva_desglosado = sql.getIvaDesglosado(constants.folio_abierto);
                        //cerrar ventanas
                        if (Application.OpenForms["registro_presupuesto"] != null)
                        {
                            Application.OpenForms["registro_presupuesto"].Close();
                        }
                        if (Application.OpenForms["articulos_cotizacion"] != null)
                        {
                            Application.OpenForms["articulos_cotizacion"].Close();
                        }
                        if (Application.OpenForms["config_modulo"] != null)
                        {
                            Application.OpenForms["config_modulo"].Close();
                        }
                        //
                        backgroundWorker3.RunWorkerAsync();
                    }
                    else if(r == DialogResult.Cancel)
                    {
                        //DONOTHING
                    }                   
                }
                else
                {
                    label3.Text = "Cargando...";
                    pictureBox1.Visible = true;
                    label3.Visible = true;
                    datagridviewNE1.Enabled = false;
                    constants.sub_folio = 1;
                    ((Form1)Application.OpenForms["form1"]).setSubFolioLabel();
                    constants.folio_abierto = (int)datagridviewNE1.CurrentRow.Cells[1].Value;
                    constants.nombre_cotizacion = datagridviewNE1.CurrentRow.Cells[2].Value.ToString();
                    constants.nombre_proyecto = datagridviewNE1.CurrentRow.Cells[5].Value.ToString();
                    constants.fecha_cotizacion = datagridviewNE1.CurrentRow.Cells[3].Value.ToString();
                    constants.autor_cotizacion = datagridviewNE1.CurrentRow.Cells[4].Value.ToString();
                    sqlDateBaseManager sql = new sqlDateBaseManager();
                    constants.desc_cotizacion = constants.stringToFloat(sql.getSingleSQLValue("cotizaciones", "descuento", "folio", constants.folio_abierto.ToString(), 0));
                    constants.utilidad_cotizacion = constants.stringToFloat(sql.getSingleSQLValue("cotizaciones", "utilidad", "folio", constants.folio_abierto.ToString(), 0));
                    constants.setClienteToPropiedades(constants.folio_abierto, constants.nombre_cotizacion, constants.nombre_proyecto, constants.desc_cotizacion, constants.utilidad_cotizacion);
                    constants.iva_desglosado = sql.getIvaDesglosado(constants.folio_abierto);
                    //cerrar ventanas
                    if (Application.OpenForms["registro_presupuesto"] != null)
                    {
                        Application.OpenForms["registro_presupuesto"].Close();
                    }
                    if (Application.OpenForms["articulos_cotizacion"] != null)
                    {
                        Application.OpenForms["articulos_cotizacion"].Close();
                    }
                    if (Application.OpenForms["config_modulo"] != null)
                    {
                        Application.OpenForms["config_modulo"].Close();
                    }
                    //
                    backgroundWorker3.RunWorkerAsync();
                }
            }
        }
        //

        //Proceso de eliminacion ------------------------------------------------------------------------------------------------------------->
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            sql.deleteCotizacion("cotizaciones", constants.folio_eliminacion);
            sql.deleteCotizacion("cristales_cotizados", constants.folio_eliminacion);
            sql.deleteCotizacion("aluminio_cotizado", constants.folio_eliminacion);
            sql.deleteCotizacion("herrajes_cotizados", constants.folio_eliminacion);
            sql.deleteCotizacion("otros_cotizaciones", constants.folio_eliminacion);
            sql.deleteCotizacion("modulos_cotizaciones", constants.folio_eliminacion);
            sql.deleteRegistroPresupuestos(constants.folio_eliminacion);
            sql.deleteProduccionTable(constants.folio_eliminacion);
            ((Form1)Application.OpenForms["Form1"]).ifDeleted();
            iniciarBusqueda(true);
        }       

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label3.Text = "";
            pictureBox1.Visible = false;
            label3.Visible = false;
            datagridviewNE1.Enabled = true;
            constants.folio_eliminacion = -1;
            iniciarBusqueda(true);
        }
        //

        //Proceso para abrir --------------------------------------------------------------------------------------------------------------->
        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            constants.clearCotizacionesLocales();           
            sql.dropTableArticulosCotizacionesToLocal("cristales_cotizados", constants.folio_abierto);
            sql.dropTableArticulosCotizacionesToLocal("aluminio_cotizado", constants.folio_abierto);
            sql.dropTableArticulosCotizacionesToLocal("herrajes_cotizados", constants.folio_abierto);
            sql.dropTableArticulosCotizacionesToLocal("otros_cotizaciones", constants.folio_abierto);
            sql.dropTableArticulosCotizacionesToLocal("modulos_cotizaciones", constants.folio_abierto);
            //verificar si esta contizacion esta registrada en presupuestos
            ((Form1)Application.OpenForms["Form1"]).verificarRegistro();
            //
        }
        //

        private void BackgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {            
            if (constants.cotizacion_error != true)
            {
                label3.Text = "Abriendo Cotización...";
                if (constants.ac_cotizacion == true && constants.p_ac == true)
                {
                    DialogResult r = MessageBox.Show(this, "¿Deseas actualizar los precios de está cotización?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (r == DialogResult.Yes)
                    {
                        constants.reload_precios = true;
                    }
                    else if(r == DialogResult.No)
                    {
                        constants.reload_precios = false;
                    }
                }
                backgroundWorker4.RunWorkerAsync();
            }
            else
            {
                pictureBox1.Visible = false;
                label3.Visible = false;
                datagridviewNE1.Enabled = true;
                MessageBox.Show(this, "[Error] no se pudo abrir esta cotización, intenta de nuevo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(monthCalendar1.Visible == true)
            {
                monthCalendar1.Visible = false;
            }
            else
            {
                monthCalendar1.Visible = true;
            }
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false && backgroundWorker4.IsBusy == false)
                {
                    label3.Text = "Buscando...";
                    LastPage = true;
                    textBox2.Text = "0";
                    pictureBox1.Visible = true;
                    label3.Visible = true;
                    datagridviewNE1.Enabled = false;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false && backgroundWorker4.IsBusy == false)
            {
                label3.Text = "Buscando...";
                LastPage = true;
                textBox2.Text = "0";
                pictureBox1.Visible = true;
                label3.Visible = true;
                datagridviewNE1.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            org_search = comboBox1.Text;
            label5.Text = org_search;
            buscarCotizacion();
        }

        //abriendo cotizacion
        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            constants.deleteFilasBorradasFromLocalDB();
            if (constants.ac_cotizacion == true && constants.reload_precios == true)
            {
                constants.errors_Open.Clear();
                for (int i = 1; i < 5; i++)
                {
                    constants.reloadPreciosCotizaciones(i);
                }
            }
            constants.reloadCotizaciones();
            ((Form1)Application.OpenForms["Form1"]).setFolioLabel();
            ((Form1)Application.OpenForms["Form1"]).seleccionarPastaña();
            ((Form1)Application.OpenForms["Form1"]).resetSubfolio();
            ((Form1)Application.OpenForms["Form1"]).refreshNewArticulo();
        }

        private void BackgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label3.Text = "";
            pictureBox1.Visible = false;
            label3.Visible = false;
            datagridviewNE1.Enabled = true;
            if (Application.OpenForms["articulos_cotizacion"] == null)
            {
                articulos_cotizacion articulos = new articulos_cotizacion();
                articulos.Show();
                if (constants.maximizar_ventanas == true)
                {
                    articulos.WindowState = FormWindowState.Maximized;
                }
                articulos.Select();
            }
            else
            {
                if (constants.maximizar_ventanas == true)
                {
                    Application.OpenForms["articulos_cotizacion"].WindowState = FormWindowState.Maximized;
                }
                else
                {
                    Application.OpenForms["articulos_cotizacion"].WindowState = FormWindowState.Normal;
                }
                Application.OpenForms["articulos_cotizacion"].Select();
            }
            Close();
        }

        private void gotoLastPage()
        {
            if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false && backgroundWorker4.IsBusy == false)
            {
                label3.Text = "Buscando...";
                LastPage = true;
                pictureBox1.Visible = true;
                label3.Visible = true;
                datagridviewNE1.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void gotoFirstPage()
        {
            if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false && backgroundWorker4.IsBusy == false)
            {
                label3.Text = "Buscando...";
                LastPage = false;
                textBox2.Text = paginas > 0 ? "1" : "0";
                pictureBox1.Visible = true;
                label3.Visible = true;
                datagridviewNE1.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        //page up
        private void button4_Click(object sender, EventArgs e)
        {
            int page = constants.stringToInt(textBox2.Text);
            if (page < paginas)
            {
                if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false && backgroundWorker4.IsBusy == false)
                {
                    label3.Text = "Buscando...";
                    LastPage = false;
                    textBox2.Text = (page + 1).ToString();
                    pictureBox1.Visible = true;
                    label3.Visible = true;
                    datagridviewNE1.Enabled = false;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        //page down
        private void button5_Click(object sender, EventArgs e)
        {
            int page = constants.stringToInt(textBox2.Text);
            if (page > 1)
            {
                if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false && backgroundWorker4.IsBusy == false)
                {
                    label3.Text = "Buscando...";
                    LastPage = false;
                    textBox2.Text = (page - 1).ToString();
                    pictureBox1.Visible = true;
                    label3.Visible = true;
                    datagridviewNE1.Enabled = false;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            gotoFirstPage();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            gotoLastPage();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (load == true)
            {
                if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false && backgroundWorker4.IsBusy == false)
                {
                    label3.Text = "Buscando...";
                    textBox2.Text = "0";
                    pictureBox1.Visible = true;
                    label3.Visible = true;
                    datagridviewNE1.Enabled = false;
                    backgroundWorker1.RunWorkerAsync(getMesInt(comboBox2.Text) + "/" + comboBox3.Text);
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (load == true)
            {
                if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false && backgroundWorker4.IsBusy == false)
                {
                    label3.Text = "Buscando...";
                    textBox2.Text = "0";
                    pictureBox1.Visible = true;
                    label3.Visible = true;
                    datagridviewNE1.Enabled = false;
                    backgroundWorker1.RunWorkerAsync(getMesInt(comboBox2.Text) + "/" + comboBox3.Text);
                }
            }
        }

        private string getMesInt(string mes)
        {
            switch (mes)
            {
                case "Enero":
                    return "01";
                case "Febrero":
                    return "02";
                case "Marzo":
                    return "03";
                case "Abril":
                    return "04";
                case "Mayo":
                    return "05";
                case "Junio":
                    return "06";
                case "Julio":
                    return "07";
                case "Agosto":
                    return "08";
                case "Septiembre":
                    return "09";
                case "Octubre":
                    return "10";
                case "Noviembre":
                    return "11";
                case "Diciembre":
                    return "12";
                default:
                    return "";
            }
        }

        private string getMesName(string mes)
        {
            switch (mes)
            {
                case "1":
                    return "Enero";
                case "2":
                    return "Febrero";
                case "3":
                    return "Marzo";
                case "4":
                    return "Abril";
                case "5":
                    return "Mayo";
                case "6":
                    return "Junio";
                case "7":
                    return "Julio";
                case "8":
                    return "Agosto";
                case "9":
                    return "Septiembre";
                case "10":
                    return "Octubre";
                case "11":
                    return "Noviembre";
                case "12":
                    return "Diciembre";
                default:
                    return "";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            reloadBusqueda();
        }

        private void reloadBusqueda()
        {
            if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false && backgroundWorker4.IsBusy == false)
            {
                label3.Text = "Buscando...";
                textBox2.Text = "0";
                pictureBox1.Visible = true;
                label3.Visible = true;
                datagridviewNE1.Enabled = false;
                backgroundWorker1.RunWorkerAsync(getMesInt(comboBox2.Text) + "/" + comboBox3.Text);
            }
        }
    }
}
