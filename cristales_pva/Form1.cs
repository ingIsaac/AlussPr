using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Drawing;
using System.Net.Sockets;

namespace cristales_pva
{
    public partial class Form1 : Form
    {
        listas_entities_pva listas;

        // crystales datos temporales
        private float _m2 = 0, _instalado = 0, _hoja = 0;
        private string clave_tem;
        private int id_tem = -1;        
        private int opened_module = 0;
        private bool module_alive = true;
        private int p = 0;
        private bool resize = false;
        private int page = 0;
        private int pages = 0;
        private bool enable_iva = true;
        public string descripcion = string.Empty;
        List<string> esquemas = new List<string>();
        // -------->

        //otros
        bool modulos_config;
        // -->>>>>>>

        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker2.DoWork += BackgroundWorker2_DoWork;
            backgroundWorker3.RunWorkerCompleted += BackgroundWorker3_RunWorkerCompleted;
            this.SizeChanged += Form1_SizeChanged;
            this.FormClosing += Form1_FormClosing;

            //grid 1-------
            datagridviewNE2.CellClick += datagridviewNE2_CellClick;
            datagridviewNE2.CellEnter += DatagridviewNE2_CellEnter;
            datagridviewNE2.KeyDown += DatagridviewNE2_KeyDown;
            //-------------

            //grid 3-------
            datagridviewNE1.CellClick += datagridviewNE1_CellClick;
            datagridviewNE1.CellLeave += datagridviewNE1_CellLeave;
            //-------------

            //Treeviewer ----
            treeView1.NodeMouseClick += TreeView1_NodeMouseClick;
            //---------------

            //Acabados
            datagridviewNE3.CellClick += DatagridviewNE3_CellClick;
            datagridviewNE3.CellEndEdit += DatagridviewNE3_CellEndEdit;
            datagridviewNE3.RowsAdded += DatagridviewNE3_RowsAdded;
            //--------------

            //Others ----
            contextMenuStrip2.Opening += ContextMenuStrip2_Opening;
            backgroundWorker4.RunWorkerCompleted += BackgroundWorker4_RunWorkerCompleted;
            tabPage14.Enter += TabPage14_Enter;
            hScrollBar1.Minimum = 1;
            checkBox3.Click += CheckBox3_Click;
            textBox1.KeyDown += TextBox1_KeyDown;
            constants.login_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private void DatagridviewNE2_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE2.Rows.Count > 0)
            {
                setArticuloToVer();
            }
        }

        private void DatagridviewNE2_KeyDown(object sender, KeyEventArgs e)
        {
            if (comboBox1.SelectedIndex == 6)
            {
                if (e.KeyData == Keys.Enter)
                {
                    e.Handled = true;
                    abrirConfigModulo();
                }
            }
            else if(comboBox1.SelectedIndex == 5)
            {
                if (e.KeyData == Keys.Enter)
                {
                    e.Handled = true;
                    //set focus
                    textBox30.Focus();
                    //
                }
            }
            else if(comboBox1.SelectedIndex == 4)
            {
                if (e.KeyData == Keys.Enter)
                {
                    e.Handled = true;
                    //set focus
                    textBox33.Focus();
                    //
                }
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                if (e.KeyData == Keys.Enter)
                {
                    e.Handled = true;
                    //set focus
                    textBox7.Focus();
                    //
                }
            }
            else if (comboBox1.SelectedIndex == 2 || comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 0)
            {
                if (e.KeyData == Keys.Enter)
                {
                    e.Handled = true;
                    //set focus
                    textBox5.Focus();
                    //
                }
            }           
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                listas = new listas_entities_pva();
                textBox9.Clear();

                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        var costo_corte = from x in listas.lista_costo_corte_e_instalado
                                          where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                          orderby x.articulo ascending
                                          select new
                                          {
                                              Clave = x.clave,
                                              Artículo = x.articulo,
                                              Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                              Costo_Instalado = "$" + x.costo_instalado
                                          };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = costo_corte.ToList();
                        break;
                    case 1:
                        var precio_corte = from x in listas.lista_precio_corte_e_instalado
                                           where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                           orderby x.articulo ascending
                                           select new
                                           {
                                               Clave = x.clave,
                                               Artículo = x.articulo,
                                               Precio_Corte_m2 = "$" + x.precio_venta_corte_m2,
                                               Precio_Instalado = "$" + x.precio_venta_instalado
                                           };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = precio_corte.ToList();
                        break;
                    case 2:
                        var hojas = from x in listas.lista_precios_hojas
                                    where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                    orderby x.articulo ascending
                                    select new
                                    {
                                        Clave = x.clave,
                                        Artículo = x.articulo,
                                        Largo = x.largo + "m",
                                        Alto = x.alto + "m",
                                        Precio_Hoja = "$" + x.precio_hoja
                                    };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = hojas.ToList();
                        break;
                    case 3:
                        var perfiles = from x in listas.perfiles
                                       where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                       orderby x.articulo ascending
                                       select new
                                       {
                                           Id = x.id,
                                           Clave = x.clave,
                                           Artículo = x.articulo,
                                           Linea = x.linea,
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
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = perfiles.ToList();
                        break;
                    case 4:
                        var herrajes = from x in listas.herrajes
                                       where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                       orderby x.articulo ascending
                                       select new
                                       {
                                           Id = x.id,
                                           Clave = x.clave,
                                           Artículo = x.articulo,
                                           Proveedor = x.proveedor,
                                           Linea = x.linea,
                                           Color = x.color,
                                           Precio = "$" + x.precio
                                       };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = herrajes.ToList();
                        break;
                    case 5:
                        var otros = from x in listas.otros
                                    where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                    orderby x.articulo ascending
                                    select new
                                    {
                                        Id = x.id,
                                        Clave = x.clave,
                                        Artículo = x.articulo,
                                        Proveedor = x.proveedor,
                                        Linea = x.linea,
                                        Color = x.color,
                                        Precio = "$" + x.precio
                                    };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = otros.ToList();
                        break;
                    case 6:
                        var modulos = from x in listas.modulos
                                      join y in listas.esquemas on x.id_diseño equals y.id
                                      where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                      orderby x.articulo ascending
                                      select new
                                      {
                                          Id = x.id,
                                          Clave = x.clave,
                                          Diseño = y.diseño,
                                          Artículo = x.articulo,
                                          Linea = x.linea,
                                          Descripcion = x.descripcion
                                      };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = modulos.ToList();
                        setDiseñoColorGrid();
                        break;
                    default: break;
                }
                listas.Dispose();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            guardarCerrar(e);
        }

        private void guardarCerrar(FormClosingEventArgs e=null)
        {
            if (sumCounter() > 0 || constants.nombre_cotizacion != "")
            {
                if (constants.cotizacion_guardada == false && constants.local == false)
                {
                    DialogResult r = MessageBox.Show("Existe una operación en curso.\n\n ¿Desea guardarla?", constants.msg_box_caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (r == DialogResult.Yes)
                    {
                        new guardar_cotizacion().ShowDialog();
                    }
                    else if (r == DialogResult.No)
                    {
                        //do nothing
                    }
                    else if (r == DialogResult.Cancel)
                    {
                        if (e != null)
                        {
                            e.Cancel = true;
                        }
                    }
                }
            }
        }

        public void checkErrorsModulos()
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    foreach(int v in constants.errors_Open)
                    {
                        if(v == constants.stringToInt(x.Cells[0].Value.ToString()))
                        {
                            x.DefaultCellStyle.BackColor = Color.Red;
                        }                      
                    }
                }
            }
        }

        private void ContextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if(constants.tipo_cotizacion != 5)
            {
                contextMenuStrip2.Items[2].Visible = false;
                contextMenuStrip2.Items[3].Visible = false;
                contextMenuStrip2.Items[4].Visible = false;
                contextMenuStrip2.Items[5].Visible = false;
                if (datagridviewNE1.RowCount > 0)
                {
                    contextMenuStrip2.Items[0].Visible = true;
                    contextMenuStrip2.Items[1].Visible = true;
                    contextMenuStrip2.Items[6].Visible = true;
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
                    contextMenuStrip2.Items[0].Visible = true;
                    contextMenuStrip2.Items[1].Visible = true;
                    contextMenuStrip2.Items[5].Visible = true;
                    contextMenuStrip2.Items[6].Visible = true;
                    if (datagridviewNE1.CurrentRow.Cells[5].Value.ToString() == "-1")
                    {
                        contextMenuStrip2.Items[3].Visible = true;
                    }
                    else
                    {
                        contextMenuStrip2.Items[3].Visible = false;
                    }
                }
                else
                {
                    contextMenuStrip2.Items[0].Visible = false;
                    contextMenuStrip2.Items[1].Visible = false;
                    contextMenuStrip2.Items[3].Visible = false;
                    contextMenuStrip2.Items[5].Visible = false;
                    contextMenuStrip2.Items[6].Visible = false;
                }
                contextMenuStrip2.Items[2].Visible = true;              
                contextMenuStrip2.Items[4].Visible = true;
            }
        }

        private void DatagridviewNE3_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            foreach(DataGridViewCell x in datagridviewNE3.Rows[e.RowIndex].Cells)
            {
                x.Value = "";
            }
        }

        private void DatagridviewNE3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE3[e.ColumnIndex, e.RowIndex].Value != null)
            {
                validarAcabado();
            }
            else
            {
                datagridviewNE3[e.ColumnIndex, e.RowIndex].Value = "";
                validarAcabado();
            }
        }

        public void setAcabado(string clave, int index)
        {
            datagridviewNE3.Rows[index].Cells[0].Value = clave;
            validarAcabado();
        }

        public void validarAcabado()
        {
            if (comboBox1.SelectedIndex == 2)
            {
                calculoGeneralCristales("_hoja");
            }
            else
            {
                if (checkBox1.Checked == true)
                {
                    calculoGeneralCristales("_m2");
                }
                else
                {
                    calculoGeneralCristales("_instalado");
                }
            }
        }

        private void DatagridviewNE3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE3.CurrentCell.OwningColumn.HeaderText == "Acabado")
            {
                new acabados(datagridviewNE3.CurrentRow.Index).ShowDialog();
            }            
            else if (datagridviewNE3.CurrentCell.OwningColumn.HeaderText == "Tipo")
            {             
                datagridviewNE3.CurrentCell.ReadOnly = false;
                //---> importante
                if (datagridviewNE3.CurrentCell.Value == null)
                {
                    datagridviewNE3.CurrentCell.Value = "";
                }
                //
                DataGridViewComboBoxCell cb = new DataGridViewComboBoxCell();
                string u = string.Empty;
                cb.Items.Clear();
                cb.Items.Add("recto");
                cb.Items.Add("curvo");
                cb.Items.Add("n/a");
                foreach (string x in cb.Items)
                {
                    if (x == datagridviewNE3.CurrentCell.Value.ToString())
                    {
                        u = datagridviewNE3.CurrentCell.Value.ToString();
                    }
                }
                if (u == string.Empty)
                {
                    datagridviewNE3.CurrentCell.Value = "";
                }
                cb.Value = u;
                datagridviewNE3.CurrentRow.Cells[datagridviewNE3.CurrentCell.ColumnIndex] = cb;
                cb.Dispose();
            }                         
        }

        private float calcularAcabados()
        {
            float r = 0;
            if (datagridviewNE3.Rows.Count > 1)
            {
                string clave = string.Empty;
                string tipo = string.Empty;
                float largo = 0;
                float alto = 0;
                int cantidad = 0;
                listas_entities_pva listas = new listas_entities_pva();
                foreach (DataGridViewRow x in datagridviewNE3.Rows)
                {                  
                    clave = x.Cells[0].Value.ToString();
                    tipo = x.Cells[1].Value.ToString();
                    largo = stringToFloat(x.Cells[2].Value.ToString());
                    alto = stringToFloat(x.Cells[3].Value.ToString());
                    cantidad = constants.stringToInt(x.Cells[4].Value.ToString());

                    var acabado = (from v in listas.acabados where v.clave == clave select v).SingleOrDefault();

                    if (acabado != null)
                    {
                        if (tipo == "recto")
                        {
                            if (largo > 0 && alto > 0)
                            {
                                r = (float)acabado.neto_recto * ((largo / 1000) * 2) * ((alto / 1000) * 2);
                            }
                            else if (largo > 0 && alto == 0)
                            {
                                r = (float)acabado.neto_recto * ((largo / 1000) * 2) * ((stringToFloat(textBox3.Text) / 1000) * 2);
                            }
                            else if (largo == 0 && alto > 0)
                            {
                                r = (float)acabado.neto_recto * (((stringToFloat(textBox2.Text) / 1000) * 2) * (alto / 1000) * 2);
                            }
                            else if (largo == 0 && alto == 0)
                            {
                                r = (float)acabado.neto_recto * ((stringToFloat(textBox2.Text) / 1000) * 2) * ((stringToFloat(textBox3.Text) / 1000) * 2);
                            }
                            else
                            {
                                r = 0;
                            }
                        }
                        else if (tipo == "curvo")
                        {
                            if (largo > 0 && alto > 0)
                            {
                                r = (float)acabado.neto_curvo * ((largo / 1000) * 2) * ((alto / 1000) * 2);
                            }
                            else if (largo > 0 && alto == 0)
                            {
                                r = (float)acabado.neto_curvo * ((largo / 1000) * 2) * ((stringToFloat(textBox3.Text) / 1000) * 2);
                            }
                            else if (largo == 0 && alto > 0)
                            {
                                r = (float)acabado.neto_curvo * (((stringToFloat(textBox2.Text) / 1000) * 2) * (alto / 1000) * 2);
                            }
                            else if (largo == 0 && alto == 0)
                            {
                                r = (float)acabado.neto_curvo * ((stringToFloat(textBox2.Text) / 1000) * 2) * ((stringToFloat(textBox3.Text) / 1000) * 2);
                            }
                            else
                            {
                                r = 0;
                            }
                        }
                        else if (tipo == "n/a")
                        {
                            if (cantidad > 0)
                            {
                                r = (float)acabado.neto_recto * cantidad;
                            }
                            else
                            {
                                if (largo > 0 && alto > 0)
                                {
                                    r = (float)acabado.neto_recto * (largo / 1000) * (alto / 1000);
                                }
                                else if (largo > 0 && alto == 0)
                                {
                                    r = (float)acabado.neto_recto * (largo / 1000) * (stringToFloat(textBox3.Text) / 1000);
                                }
                                else if (largo == 0 && alto > 0)
                                {
                                    r = (float)acabado.neto_recto * (stringToFloat(textBox2.Text) / 1000) * (alto / 1000);
                                }
                                else if (largo == 0 && alto == 0)
                                {
                                    r = (float)acabado.neto_recto * (stringToFloat(textBox2.Text) / 1000) * (stringToFloat(textBox3.Text) / 1000);
                                }
                                else
                                {
                                    r = 0;
                                }
                            }
                        }
                    }
                }              
            }          
          return r;
        }

        private void createAcabadoRow(string acabado)
        {
            if (acabado != "")
            {
                datagridviewNE3.Rows.Add();
                string[] desglose = acabado.Split(',');
                datagridviewNE3.Rows[datagridviewNE3.Rows.Count - 2].SetValues(desglose[0], desglose[1], desglose[2], desglose[3], desglose[4]);
            }
        }

        private string getAcabadoFromRow(string acabado)
        {
            string r = string.Empty;
            foreach(DataGridViewRow x in datagridviewNE3.Rows)
            {
                if (x.Cells[0].Value.ToString().StartsWith(acabado))
                {
                    r = x.Cells[0].Value + "," + x.Cells[1].Value + "," + x.Cells[2].Value + "," + x.Cells[3].Value + "," + x.Cells[4].Value;
                }               
            }
            return r;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Process[] p = System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            if(p.Length > 1)
            {
                MessageBox.Show("[Error] solo puede existir una instancia de este programa.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }            
            new Form2(false).ShowDialog();
            if (constants.error == true)
            {
                Environment.Exit(0);
            }
            WindowState = FormWindowState.Maximized;
            toolStripProgressBar1.Visible = false;
            //Client version
            label39.Text = "v." + constants.version;
            //
            tabControl1.SelectedTab = tabPage15;
            checkBox5.Checked = constants.iva_desglosado;
            checkBox5.Enabled = constants.permitir_ajuste_iva;
            setModoLIVA();
            constants.setFolioStart();           
            if (constants.folio_abierto > 0)
            {
                label22.Text = constants.folio_abierto.ToString();
                textBox4.Text = constants.desc_cotizacion.ToString();
                textBox28.Text = constants.utilidad_cotizacion.ToString();
                checkBox5.Checked = constants.iva_desglosado;
                toolStripStatusLabel3.Text = "     [Cliente: " + constants.nombre_cotizacion + "]   [Proyecto: " + constants.nombre_proyecto + "]";
                toolStripStatusLabel3.Text = toolStripStatusLabel3.Text.ToUpper();
                toolStripStatusLabel3.ForeColor = System.Drawing.Color.Blue;
            }
            setTextCaptionForm();
            loadIntervalo();
            if (constants.local == false)
            {
                new loading_form().ShowDialog();
                checkConnection();
                connectionChecker();
                updater();
            }
            else
            {
                toolStripStatusLabel1.Text = "Status: Local";
            }    
            if(constants.enable_cs == true)
            {
                checkBox3.Checked = true;
            } 
            treeView1.Nodes[0].Expand();
            constants.loadPropiedadesModel();
            reloadIVA();          
            disableTabPage();           
            if (constants.getAlertVigencia(new sqlDateBaseManager().getvigenciaTienda(constants.org_name)))
            {
                MessageBox.Show("La fecha de expiración esta próxima, pónganse en contacto con el proveedor del sistema.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if(constants.licencia == "DEMO")
            {
                Text = Text + " | VERSIÓN DE PRUEBA"; 
            }
            //Changelog
            loadChangelog();
            //Select control
            comboBox1.Focus();
            //Image List
            setModueTreeImages();
        }

        private void setModueTreeImages()
        {
            imageList1.Images.Add(Properties.Resources.green_tree);
            imageList1.Images.Add(Properties.Resources.red_tree);
            treeView1.Nodes[0].Nodes[0].ImageIndex = 1;
            treeView1.Nodes[0].Nodes[1].ImageIndex = 1;
            treeView1.Nodes[0].Nodes[2].ImageIndex = 1;
            treeView1.Nodes[0].Nodes[3].ImageIndex = 1;
            treeView1.Nodes[0].Nodes[4].ImageIndex = 1;
        }

        private void loadChangelog()
        {
            if (constants.local == false)
            {
                List<string> list = new sqlDateBaseManager().getChangelog();
                if (list.Count > 0)
                {
                    richTextBox4.Lines = list.ToArray();
                }
            }
        }

        public void permitirAjusteIVA(bool r)
        {
            checkBox5.Enabled = r;
        }

        public void ReloadData()
        {
            if (constants.error == true)
            {
                Environment.Exit(0);
            }            
            constants.setFolioStart();
            if (constants.folio_abierto > 0)
            {
                label22.Text = constants.folio_abierto.ToString();
                textBox4.Text = constants.desc_cotizacion.ToString();
                textBox28.Text = constants.utilidad_cotizacion.ToString();
                checkBox5.Checked = constants.iva_desglosado;
                toolStripStatusLabel3.Text = "     [Cliente: " + constants.nombre_cotizacion + "]   [Proyecto: " + constants.nombre_proyecto + "]";
                toolStripStatusLabel3.Text = toolStripStatusLabel3.Text.ToUpper();
                toolStripStatusLabel3.ForeColor = System.Drawing.Color.Blue;
            }
            setTextCaptionForm();
            tabControl1.SelectedTab = tabPage15;
            disableTabPage();
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if (Application.OpenForms[i].Name != "Form1")
                {
                    Application.OpenForms[i].Close();
                }
            }
        }

        private void loadIntervalo()
        {
            try
            {
                XDocument propiedades_xml = XDocument.Load(constants.propiedades_xml);

                var intervalo = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("UDT")).SingleOrDefault();

                if(intervalo != null)
                {
                    constants.updater_interval = constants.stringToInt(intervalo.Value);                                  
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
        }

        public void reloadIVA()
        {
            checkBox5.Text = "IVA (" + ((constants.getPropiedadesModel() - 1) * 100) + "%):";
        }

        public void reloadAll()
        {
            countCotizacionesArticulo();
            loadCountArticulos();
            calcularTotalesCotizacion();         
            refreshNewArticulo();
            constants.checkErrorsOnLoad();
            constants.cotizacion_guardada = false;
        }

        public void reloadCotizacion()
        {
            refreshNewArticulo();
            constants.checkErrorsOnLoad();
            constants.cotizacion_guardada = false;
        }

        public void reloadPreciosCotizaciones()
        {
            if (constants.ac_cotizacion == true && constants.reload_precios == true)
            {
                constants.errors_Open.Clear();
                for (int i = 1; i < 5; i++)
                {
                    constants.reloadPreciosCotizaciones(i);
                }
            }
        }

        private void checkConnection()
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            sql.setServerConnection();
        }

        private void disableTabPage()
        {
            ((Control)tabPage1).Enabled = false;
            ((Control)tabPage2).Enabled = false;
            ((Control)tabPage3).Enabled = false;
            ((Control)tabPage4).Enabled = false;
            ((Control)tabPage5).Enabled = false;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            checkConnection();
            if (constants.update_later == true)
            {
                if (Application.OpenForms["update"] == null)
                {
                    new update(constants.db_version).ShowDialog();
                }
                else
                {
                    Application.OpenForms["update"].Select();
                }
            }
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel2.Image = null;          
            if(constants.iva_desglosado == true)
            {
                constants.iva = constants.getPropiedadesModel();
            }
            setTCLabel(constants.tc);
        }

        public void reloadPrecios()
        {           
            if (comboBox1.Text != "")
            {
                page = 0;
                loadListaFromLocal();
            }
            reloadPreciosCotizaciones();
            reloadAll();
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Maximum = 100;
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        public void setTextCaptionForm()
        {                    
            if (constants.local == true)
            {
                Text = constants.app_name + " <" + constants.org_name + "> - LOCAL";
            }
            else
            {
                Text = constants.app_name + " <" + constants.org_name + "> - " + constants.user;
            }
        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                new Form2(true).ShowDialog();
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, para iniciar sesión nuevamente ocupas reiniciar el programa.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panelDeAdministradorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                if (constants.user_access >= 5)
                {
                    if (Application.OpenForms["admin_panel"] == null)
                    {
                        admin_panel admin_panel = new admin_panel();
                        admin_panel.Show();
                        if(constants.maximizar_ventanas == true)
                        {
                            admin_panel.WindowState = FormWindowState.Maximized;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("[Error] solo un usuario con privilegios de grado (5) puede acceder a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string getEspesorVidrio()
        {
            string r = string.Empty;
            foreach(char x in constants.getClave(label9.Text))
            {
                if(char.IsDigit(x) == true)
                {
                    r = r + x.ToString();
                }
            }
            return r;
        }

        // Inicia filter ----------------------------------------------------------------------------------------------------------------------------

        //filter items
        public void setFiltros()
        {
            if (comboBox1.SelectedIndex == 0 || comboBox1.SelectedIndex == 1 || comboBox1.SelectedIndex == 2)
            {
                comboBox2.Items.Clear();
                comboBox6.Items.Clear();                

                comboBox6.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                comboBox2.Items.AddRange(constants.getCategorias("vidrio").ToArray());
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                comboBox2.Items.Clear();
                comboBox6.Items.Clear();               

                comboBox6.Items.AddRange(constants.getProveedores("aluminio").ToArray());
                comboBox2.Items.AddRange(constants.getCategorias("aluminio").ToArray());
            }
            else if (comboBox1.SelectedIndex == 4)
            {
                comboBox2.Items.Clear();
                comboBox6.Items.Clear();                

                comboBox6.Items.AddRange(constants.getProveedores("herraje").ToArray());
                comboBox2.Items.AddRange(constants.getCategorias("herraje").ToArray());
            }
            else if (comboBox1.SelectedIndex == 5)
            {
                comboBox2.Items.Clear();
                comboBox6.Items.Clear();               

                comboBox6.Items.AddRange(constants.getProveedores("otros").ToArray());
                comboBox2.Items.AddRange(constants.getCategorias("otros").ToArray());
            }
            else if (comboBox1.SelectedIndex == 6)
            {
                comboBox2.Items.Clear();
                comboBox6.Items.Clear();                             

                comboBox6.Items.AddRange(constants.getProveedores("modulo").ToArray());
                comboBox2.Items.AddRange(constants.getLineasModulo().ToArray());
            }
        }

        //filter code
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            listas = new listas_entities_pva();
            textBox9.Clear();

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    if (comboBox2.Text != "")
                    {
                        var filter = from x in listas.lista_costo_corte_e_instalado
                                     where (x.articulo.StartsWith(comboBox2.Text) && (x.proveedor == comboBox6.Text))
                                     orderby x.articulo ascending
                                     select new
                                        {
                                            Clave = x.clave,
                                            Artículo = x.articulo,
                                            Proveedor = x.proveedor,
                                            Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                            Costo_Instalado = "$" + x.costo_instalado
                                        };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }
                    else 
                    {
                        var filter = from x in listas.lista_costo_corte_e_instalado
                                     where x.proveedor == comboBox6.Text
                                     orderby x.articulo ascending
                                     select new
                                        {
                                            Clave = x.clave,
                                            Artículo = x.articulo,
                                            Proveedor = x.proveedor,
                                            Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                            Costo_Instalado = "$" + x.costo_instalado
                                        };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                   
                    break;
                case 1:
                    if (comboBox2.Text != "")
                    {
                        var filter = from x in listas.lista_precio_corte_e_instalado
                                     where (x.articulo.StartsWith(comboBox2.Text) && x.proveedor == comboBox6.Text)
                                     orderby x.articulo ascending
                                     select new
                                        {
                                            Clave = x.clave,
                                            Artículo = x.articulo,
                                            Proveedor = x.proveedor,
                                            Precio_Corte_m2 = "$" + x.precio_venta_corte_m2,
                                            Precio_Instalado = "$" + x.precio_venta_instalado
                                        };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }
                    else 
                    {
                        var filter = from x in listas.lista_precio_corte_e_instalado
                                     where x.proveedor == comboBox6.Text
                                     orderby x.articulo ascending
                                     select new
                                        {
                                            Clave = x.clave,
                                            Artículo = x.articulo,
                                            Proveedor = x.proveedor,
                                            Precio_Corte_m2 = "$" + x.precio_venta_corte_m2,
                                            Precio_Instalado = "$" + x.precio_venta_instalado
                                        };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                  
                    break;
                case 2:
                    if (comboBox2.Text != "")
                    {
                        var filter = from x in listas.lista_precios_hojas
                                     where (x.articulo.StartsWith(comboBox2.Text) && x.proveedor == comboBox6.Text)
                                     orderby x.articulo ascending
                                     select new
                                        {
                                            Clave = x.clave,
                                            Artículo = x.articulo,
                                            Proveedor = x.proveedor,
                                            Largo = x.largo + " m",
                                            Alto = x.alto + " m",
                                            Precio_Hoja = "$" + x.precio_hoja
                                        };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                   
                    else 
                    {
                        var filter = from x in listas.lista_precios_hojas
                                     where x.proveedor == comboBox6.Text
                                     orderby x.articulo ascending
                                     select new
                                        {
                                            Clave = x.clave,
                                            Artículo = x.articulo,
                                            Proveedor = x.proveedor,
                                            Largo = x.largo + " m",
                                            Alto = x.alto + " m",
                                            Precio_Hoja = "$" + x.precio_hoja
                                        };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                      
                    break;
                case 3:
                    if (comboBox2.Text != "")
                    {
                        var filter = from x in listas.perfiles
                                     where x.linea == comboBox2.Text && x.proveedor == comboBox6.Text
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
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                   
                    else
                    {
                        var filter = from x in listas.perfiles
                                     where x.proveedor == comboBox6.Text
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
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                  
                    break;
                case 4:
                    if (comboBox2.Text != "")
                    {
                        var filter = from x in listas.herrajes
                                     where x.linea == comboBox2.Text && x.proveedor == comboBox6.Text
                                     orderby x.articulo ascending
                                     select new
                                        {
                                            Id = x.id,
                                            Clave = x.clave,
                                            Artículo = x.articulo,
                                            Linea = x.linea,
                                            Proveedor = x.proveedor,
                                            Color = x.color,
                                            Precio = "$" + x.precio
                                        };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                   
                    else
                    {
                        var filter = from x in listas.herrajes
                                     where x.proveedor == comboBox6.Text
                                     orderby x.articulo ascending
                                     select new
                                        {
                                            Id = x.id,
                                            Clave = x.clave,
                                            Artículo = x.articulo,
                                            Linea = x.linea,
                                            Proveedor = x.proveedor,
                                            Color = x.color,
                                            Precio = "$" + x.precio
                                        };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                          
                    break;
                case 5:
                    if (comboBox2.Text != "")
                    {
                        var filter = from x in listas.otros
                                     where x.linea == comboBox2.Text && x.proveedor == comboBox6.Text
                                     orderby x.articulo ascending
                                     select new
                                            {
                                                Id = x.id,
                                                Clave = x.clave,
                                                Artículo = x.articulo,
                                                Linea = x.linea,
                                                Proveedor = x.proveedor,
                                                Color = x.color,
                                                Precio = "$" + x.precio
                                            };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                   
                    else
                    {
                        var filter = from x in listas.otros
                                     where x.proveedor == comboBox6.Text
                                     orderby x.articulo ascending
                                     select new
                                            {
                                                Id = x.id,
                                                Clave = x.clave,
                                                Artículo = x.articulo,
                                                Linea = x.linea,
                                                Proveedor = x.proveedor,
                                                Color = x.color,
                                                Precio = "$" + x.precio
                                            };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                        setDiseñoColorGrid();
                    }
                    break;
                default:
                    break;
            }
            listas.Dispose();
            if (datagridviewNE2.RowCount > 0)
            {
                datagridviewNE2.Select();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listas = new listas_entities_pva();
            textBox9.Clear();

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    if (comboBox6.Text != "")
                    {
                        var filter = from x in listas.lista_costo_corte_e_instalado
                                     where (x.articulo.StartsWith(comboBox2.Text)) && (x.proveedor == comboBox6.Text)
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                         Costo_Instalado = "$" + x.costo_instalado
                                     };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                  
                    else
                    {
                        var filter = from x in listas.lista_costo_corte_e_instalado
                                     where x.articulo.StartsWith(comboBox2.Text)
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                         Costo_Instalado = "$" + x.costo_instalado
                                     };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                  
                    break;
                case 1:
                    if (comboBox6.Text != "")
                    {
                        var filter = from x in listas.lista_precio_corte_e_instalado
                                     where (x.articulo.StartsWith(comboBox2.Text)) && (x.proveedor == comboBox6.Text)
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Precio_Corte_m2 = "$" + x.precio_venta_corte_m2,
                                         Precio_Instalado = "$" + x.precio_venta_instalado
                                     };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                   
                    else
                    {
                        var filter = from x in listas.lista_precio_corte_e_instalado
                                     where x.articulo.StartsWith(comboBox2.Text)
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Precio_Corte_m2 = "$" + x.precio_venta_corte_m2,
                                         Precio_Instalado = "$" + x.precio_venta_instalado
                                     };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                 
                    break;
                case 2:
                    if (comboBox6.Text != "")
                    {
                        var filter = from x in listas.lista_precios_hojas
                                     where (x.articulo.StartsWith(comboBox2.Text)) && (x.proveedor == comboBox6.Text)
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Largo = x.largo + " m",
                                         Alto = x.alto + " m",
                                         Precio_Hoja = "$" + x.precio_hoja
                                     };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                   
                    else
                    {
                        var filter = from x in listas.lista_precios_hojas
                                     where x.articulo.StartsWith(comboBox2.Text)
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Largo = x.largo + " m",
                                         Alto = x.alto + " m",
                                         Precio_Hoja = "$" + x.precio_hoja
                                     };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                    
                    break;
                case 3:
                    if (comboBox6.Text != "")
                    {
                        var filter = from x in listas.perfiles
                                     where x.linea == comboBox2.Text && x.proveedor == comboBox6.Text
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
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                  
                    else
                    {
                        var filter = from x in listas.perfiles
                                     where x.linea == comboBox2.Text
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
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                    
                    break;
                case 4:
                    if (comboBox6.Text != "")
                    {
                        var filter = from x in listas.herrajes
                                     where x.linea == comboBox2.Text && x.proveedor == comboBox6.Text
                                     orderby x.articulo ascending
                                     select new
                                       {
                                           Id = x.id,
                                           Clave = x.clave,
                                           Artículo = x.articulo,
                                           Linea = x.linea,
                                           Proveedor = x.proveedor,
                                           Color = x.color,
                                           Precio = "$" + x.precio
                                       };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                   
                    else
                    {
                        var filter = from x in listas.herrajes
                                     where x.linea == comboBox2.Text
                                     orderby x.articulo ascending
                                     select new
                                       {
                                           Id = x.id,
                                           Clave = x.clave,
                                           Artículo = x.articulo,
                                           Linea = x.linea,
                                           Proveedor = x.proveedor,
                                           Color = x.color,
                                           Precio = "$" + x.precio
                                       };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                  
                    break;
                case 5:
                    if (comboBox6.Text != "")
                    {
                        var filter = from x in listas.otros
                                     where x.linea == comboBox2.Text && x.proveedor == comboBox6.Text
                                     orderby x.articulo ascending
                                     select new
                                        {
                                            Id = x.id,
                                            Clave = x.clave,
                                            Artículo = x.articulo,
                                            Linea = x.linea,
                                            Proveedor = x.proveedor,
                                            Color = x.color,
                                            Precio = "$" + x.precio
                                        };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                   
                    else
                    {
                        var filter = from x in listas.otros
                                     where x.linea == comboBox2.Text
                                     orderby x.articulo ascending
                                     select new
                                        {
                                            Id = x.id,
                                            Clave = x.clave,
                                            Artículo = x.articulo,
                                            Linea = x.linea,
                                            Proveedor = x.proveedor,
                                            Color = x.color,
                                            Precio = "$" + x.precio
                                        };
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = filter.ToList();
                    }                  
                    break;
                case 6:                   
                    var filter_a = from x in listas.modulos
                                   join y in listas.esquemas on x.id_diseño equals y.id
                                   where x.linea == comboBox2.Text
                                   orderby x.articulo ascending
                                   select new
                                        {
                                            Id = x.id,
                                            Clave = x.clave,
                                            Diseño = y.diseño,
                                            Artículo = x.articulo,                                              
                                            Linea = x.linea,
                                            Descripcion = x.descripcion                                              
                                        };
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = filter_a.ToList();
                    setDiseñoColorGrid();
                    break;
                default:
                    break;
            }
            listas.Dispose();
            if(datagridviewNE2.RowCount > 0)
            {
                datagridviewNE2.Select();
            }
        }

        // buscar
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            page = 0;
            loadListaFromLocal();           
        }
        //

        //cargar listas
        public void loadListaFromLocal()
        {
            setFiltros();
            listas = new listas_entities_pva();

            if (comboBox1.SelectedIndex == 0)
            {
                pages = (from x in listas.lista_costo_corte_e_instalado select x).Count() / 50;
                var data = from x in listas.lista_costo_corte_e_instalado orderby x.articulo ascending
                           select new
                           {
                               Clave = x.clave,
                               Artículo = x.articulo,
                               Proveedor = x.proveedor,
                               Costo_Corte_m2 = "$" + x.costo_corte_m2,
                               Costo_Instalado = "$" + x.costo_instalado
                           };
                if (datagridviewNE2.InvokeRequired == true)
                {
                    datagridviewNE2.Invoke((MethodInvoker)delegate
                    {
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                        textBox9.Text = (page + 1) + "/" + (pages + 1);
                    });
                }
                else {
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                    textBox9.Text = (page + 1) + "/" + (pages + 1);
                }
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                pages = (from x in listas.lista_precio_corte_e_instalado select x).Count() / 50;
                var data = from x in listas.lista_precio_corte_e_instalado orderby x.articulo ascending
                           select new
                           {
                               Clave = x.clave,
                               Artículo = x.articulo,
                               Proveedor = x.proveedor,
                               Precio_Corte_m2 = "$" + x.precio_venta_corte_m2,
                               Precio_Instalado = "$" + x.precio_venta_instalado
                           };
                if (datagridviewNE2.InvokeRequired == true)
                {
                    datagridviewNE2.Invoke((MethodInvoker)delegate
                    {
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                        textBox9.Text = (page + 1) + "/" + (pages + 1);
                    });
                }
                else {
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                    textBox9.Text = (page + 1) + "/" + (pages + 1);
                }
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                pages = (from x in listas.lista_precios_hojas select x).Count() / 50;
                var data = from x in listas.lista_precios_hojas orderby x.articulo ascending
                           select new
                           {
                               Clave = x.clave,
                               Artículo = x.articulo,
                               Proveedor = x.proveedor,
                               Largo = x.largo + " m",
                               Alto = x.alto + " m",
                               Precio_Hoja = "$" + x.precio_hoja
                           };
                if (datagridviewNE2.InvokeRequired == true)
                {
                    datagridviewNE2.Invoke((MethodInvoker)delegate
                    {
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                        textBox9.Text = (page + 1) + "/" + (pages + 1);
                    });
                }
                else {
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                    textBox9.Text = (page + 1) + "/" + (pages + 1);
                }
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                pages = (from x in listas.perfiles select x).Count() / 50;
                var data = from x in listas.perfiles orderby x.articulo ascending
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
                if (datagridviewNE2.InvokeRequired == true)
                {
                    datagridviewNE2.Invoke((MethodInvoker)delegate
                    {
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                        textBox9.Text = (page + 1) + "/" + (pages + 1);
                    });
                }
                else {
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                    textBox9.Text = (page + 1) + "/" + (pages + 1);
                }
            }
            else if (comboBox1.SelectedIndex == 4)
            {
                pages = (from x in listas.herrajes select x).Count() / 50;
                var data = from x in listas.herrajes orderby x.articulo ascending
                           select new
                           {
                               Id = x.id,
                               Clave = x.clave,
                               Artículo = x.articulo,
                               Linea = x.linea,
                               Proveedor = x.proveedor,
                               Color = x.color,
                               Precio = "$" + x.precio
                           };
                if (datagridviewNE2.InvokeRequired == true)
                {
                    datagridviewNE2.Invoke((MethodInvoker)delegate
                    {
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                        textBox9.Text = (page + 1) + "/" + (pages + 1);
                    });
                }
                else {
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                    textBox9.Text = (page + 1) + "/" + (pages + 1);
                }
            }
            else if (comboBox1.SelectedIndex == 5)
            {
                pages = (from x in listas.otros select x).Count() / 50;
                var data = from x in listas.otros orderby x.articulo ascending
                           select new
                           {
                               Id = x.id,
                               Clave = x.clave,
                               Artículo = x.articulo,
                               Linea = x.linea,
                               Proveedor = x.proveedor,
                               Color = x.color,
                               Precio = "$" + x.precio
                           };
                if (datagridviewNE2.InvokeRequired == true)
                {
                    datagridviewNE2.Invoke((MethodInvoker)delegate
                    {
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                        textBox9.Text = (page + 1) + "/" + (pages + 1);
                    });
                }
                else {
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                    textBox9.Text = (page + 1) + "/" + (pages + 1);
                }
            }
            else if (comboBox1.SelectedIndex == 6)
            {
                pages = (from x in listas.modulos select x).Count() / 50;
                var data = from x in listas.modulos join y in listas.esquemas on x.id_diseño equals y.id
                            orderby x.articulo ascending select new
                            {
                                Id = x.id,
                                Clave = x.clave,
                                Diseño = y.diseño,
                                Artículo = x.articulo,
                                Linea = x.linea,
                                Descripcion = x.descripcion
                            };
                if (datagridviewNE2.InvokeRequired == true)
                {
                    datagridviewNE2.Invoke((MethodInvoker)delegate
                    {
                        datagridviewNE2.DataSource = null;
                        datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                        textBox9.Text = (page + 1) + "/" + (pages + 1);
                        setDiseñoColorGrid();
                    });
                }
                else {
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = data.Skip(50 * page).Take(50).ToList();
                    textBox9.Text = (page + 1) + "/" + (pages + 1);
                    setDiseñoColorGrid();
                }
            }
            if (datagridviewNE2.RowCount > 0)
            {
                datagridviewNE2.Select();
            }
        }

        private void setDiseñoColorGrid()
        {
            if (datagridviewNE2.ColumnCount > 0)
            {
                foreach (DataGridViewColumn x in datagridviewNE2.Columns)
                {
                    if (x.HeaderText == "Diseño")
                    {
                        x.DefaultCellStyle.BackColor = Color.LightGreen;
                    }                    
                }
            }
        }

        // ends filter ----------------------------------------------------------------------------------------------------------------------

        // Inicia load ----------------------------------------------------------------------------------------------------------------------
        //barra de carga
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {            
            constants.downloadPropiedadesModel();
            constants.loadPropiedadesModel();
            //TC
            sqlDateBaseManager sql = new sqlDateBaseManager();
            float tc = sql.getTC();
            constants.setPropiedadesXML(tc, sql.getCostoAluminioKG());
            if (tc <= 0)
            {
                tc = constants.getTCFromXML();
            }
            constants.tc = tc;
            if (constants.enable_c_tc && constants.folio_abierto > 0)
            {
                float c_tc = sql.getCotizacionTC(constants.folio_abierto);
                if (c_tc > 0)
                {
                    constants.tc = c_tc;
                }
            }
            //----------------------------------------------------------------->
            updateTablesToLocalDB();
            constants.reloadUserItems(this);          
            reloadPrecios();
            reloadIVA();           
        }
        //end barra de carga

        public void setToolStripStatus(string status, Boolean icon)
        {
            toolStripStatusLabel1.Text = status;
            if (icon == true)
            {
                toolStripStatusLabel1.Image = Properties.Resources.green_light;
            }
            else
            {
                toolStripStatusLabel1.Image = Properties.Resources.red_light;
            }
        }

        private void connectionChecker()
        {
            System.Timers.Timer timer = new System.Timers.Timer(1 * 60 * 1000);
            timer.Elapsed += runConnectionChecker;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
        }

        private void runConnectionChecker(object sender, System.Timers.ElapsedEventArgs e)
        {        
            if (backgroundWorker2.IsBusy == false)
            {
                backgroundWorker2.RunWorkerAsync();
            }
        }

        private void BackgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            new sqlDateBaseManager().checkConnection();
            constants.setConnectionToLoginServer(constants.user + " - " + constants.org_name);
        }

        private void updater()
        {
            if (constants.updater_enable == true)
            {
                System.Timers.Timer timer = new System.Timers.Timer(constants.updater_interval * 60 * 1000);
                timer.Elapsed += runUpdater;
                timer.AutoReset = true;
                timer.Enabled = true;
                timer.Start();
            }
        }

        private void runUpdater(object sender, System.Timers.ElapsedEventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if (sql.setServerConnection() == true)
            {
                if (sql.getConnectionState() == ConnectionState.Closed)
                {
                    if (Application.OpenForms["admin_panel"] == null)
                    {
                        if (backgroundWorker1.IsBusy == false)
                        {
                            if (backgroundWorker2.IsBusy == false)
                            {
                                if (backgroundWorker3.IsBusy == false)
                                {
                                    toolStripStatusLabel2.Image = Properties.Resources.Counterclockwise_arrow_icon;
                                    toolStripProgressBar1.Visible = true;
                                    toolStripStatusLabel1.Text = "Actualizando...";
                                    backgroundWorker1.RunWorkerAsync();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void manualUpdater()
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if (sql.setServerConnection() == true)
            {
                if (backgroundWorker1.IsBusy == false)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }    

        //updaters ------------------------------------------------------------------------------------------------------------------>
        private void updateListaCostoCorteInstalado(string clave, string articulo, float costo_corte_m2, float costo_instalado, string proveedor, string moneda)
        {
            listas = new listas_entities_pva();
            try
            {
                var new_data = (from x in listas.lista_costo_corte_e_instalado where x.clave == clave select x).SingleOrDefault();
                float tc = constants.tc;

                if (new_data != null)
                {
                    new_data.articulo = articulo;
                    if (moneda != "MXN")
                    {
                        new_data.costo_corte_m2 = Math.Round(costo_corte_m2 * tc, 2);
                        new_data.costo_instalado = Math.Round(costo_instalado * tc, 2);
                    }
                    else
                    {
                        new_data.costo_corte_m2 = Math.Round(costo_corte_m2, 2);
                        new_data.costo_instalado = Math.Round(costo_instalado, 2);
                    }
                    new_data.proveedor = proveedor;
                    new_data.moneda = moneda;
                }

                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updateListaPrecioCorteInstalado(string clave, string articulo, float precio_corte_m2, float precio_instalado, string proveedor, string moneda)
        {
            listas = new listas_entities_pva();
            try
            {
                var new_data = (from x in listas.lista_precio_corte_e_instalado where x.clave == clave select x).SingleOrDefault();
                float tc = constants.tc;

                if (new_data != null)
                {
                    new_data.articulo = articulo;
                    if (moneda != "MXN")
                    {
                        new_data.precio_venta_corte_m2 = Math.Round(precio_corte_m2 * tc, 2);
                        new_data.precio_venta_instalado = Math.Round(precio_instalado * tc, 2);
                    }
                    else
                    {
                        new_data.precio_venta_corte_m2 = Math.Round(precio_corte_m2, 2);
                        new_data.precio_venta_instalado = Math.Round(precio_instalado, 2);
                    }                   
                    new_data.proveedor = proveedor;
                    new_data.moneda = moneda;
                }

                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updateListaPrecioHojas(string clave, string articulo, float largo, float alto, float precio_hoja, string proveedor, string moneda)
        {
            listas = new listas_entities_pva();
            try
            {
                var new_data = (from x in listas.lista_precios_hojas where x.clave == clave select x).SingleOrDefault();
                float tc = constants.tc;

                if (new_data != null)
                {
                    new_data.articulo = articulo;
                    new_data.largo = Math.Round(largo, 2);
                    new_data.alto = Math.Round(alto, 2);
                    if (moneda != "MXN")
                    {
                        new_data.precio_hoja = Math.Round(precio_hoja * tc, 2);
                    }
                    else
                    {
                        new_data.precio_hoja = Math.Round(precio_hoja, 2);
                    }
                    new_data.proveedor = proveedor;
                    new_data.moneda = moneda;
                }

                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updateListaAcabados(string clave, string acabado, float neto_recto, float neto_curvo)
        {
            listas = new listas_entities_pva();
            try
            {
                var new_data = (from x in listas.acabados where x.clave == clave select x).SingleOrDefault();

                if (new_data != null)
                {
                    new_data.acabado1 = acabado;
                    new_data.neto_recto = Math.Round(neto_recto, 2);
                    new_data.neto_curvo = Math.Round(neto_curvo, 2);
                }

                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updateListaPerfilesCuprum(int id, string clave, string articulo, string linea, string proveedor, float largo, float ancho, float per_a, float crudo, float blanco, float hueso, float champagne, float gris, float negro, float brillante, float natural, float madera, float peso, float chocolate, float acero_inox, float bronce, string moneda)
        {
            listas = new listas_entities_pva();
            try
            {
                var new_data = (from x in listas.perfiles where x.id == id select x).SingleOrDefault();
                float tc = constants.tc;

                if (new_data != null)
                {
                    new_data.articulo = articulo;
                    new_data.linea = linea;
                    new_data.proveedor = proveedor;
                    new_data.largo = Math.Round(largo, 2);
                    new_data.ancho_perfil = Math.Round(ancho, 2);
                    new_data.perimetro_dm2_ml = Math.Round(per_a, 2);
                    if (moneda != "MXN")
                    {
                        new_data.crudo = Math.Round(crudo * tc, 2);
                        new_data.blanco = Math.Round(blanco * tc, 2);
                        new_data.hueso = Math.Round(hueso * tc, 2);
                        new_data.champagne = Math.Round(champagne * tc, 2);
                        new_data.gris = Math.Round(gris * tc, 2);
                        new_data.negro = Math.Round(negro * tc, 2);
                        new_data.brillante = Math.Round(brillante * tc, 2);
                        new_data.natural_1 = Math.Round(natural * tc, 2);
                        new_data.madera = Math.Round(madera * tc, 2);
                        new_data.chocolate = Math.Round(chocolate * tc, 2);
                        new_data.acero_inox = Math.Round(acero_inox * tc, 2);
                        new_data.bronce = Math.Round(bronce * tc, 2);
                    }
                    else
                    {
                        new_data.crudo = Math.Round(crudo, 2);
                        new_data.blanco = Math.Round(blanco, 2);
                        new_data.hueso = Math.Round(hueso, 2);
                        new_data.champagne = Math.Round(champagne, 2);
                        new_data.gris = Math.Round(gris, 2);
                        new_data.negro = Math.Round(negro, 2);
                        new_data.brillante = Math.Round(brillante, 2);
                        new_data.natural_1 = Math.Round(natural, 2);
                        new_data.madera = Math.Round(madera, 2);
                        new_data.chocolate = Math.Round(chocolate, 2);
                        new_data.acero_inox = Math.Round(acero_inox, 2);
                        new_data.bronce = Math.Round(bronce, 2);
                    }
                    new_data.kg_peso_lineal = Math.Round(peso, 2);
                    new_data.moneda = moneda;
                }

                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updateListaHerrajes(int id, string clave, string articulo, string proveedor, string linea, string caracteristicas, string color, float precio, string moneda)
        {
            listas = new listas_entities_pva();
            try
            {
                var new_data = (from x in listas.herrajes where x.id == id select x).SingleOrDefault();
                float tc = constants.tc;

                if (new_data != null)
                {
                    new_data.articulo = articulo;
                    new_data.proveedor = proveedor;
                    new_data.linea = linea;
                    new_data.caracteristicas = caracteristicas;
                    new_data.color = color;
                    if (moneda != "MXN")
                    {
                        new_data.precio = Math.Round(precio * tc, 2);
                    }
                    else
                    {
                        new_data.precio = Math.Round(precio, 2);
                    }
                    new_data.moneda = moneda;
                }

                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updateListaOtros(int id, string clave, string articulo, string proveedor, string linea, string caracteristicas, string color, float largo, float alto, float precio, string moneda)
        {
            listas = new listas_entities_pva();
            try
            {
                var new_data = (from x in listas.otros where x.id == id select x).SingleOrDefault();
                float tc = constants.tc;

                if (new_data != null)
                {
                    new_data.articulo = articulo;
                    new_data.proveedor = proveedor;
                    new_data.linea = linea;
                    new_data.caracteristicas = caracteristicas;
                    new_data.color = color;
                    new_data.largo = largo;
                    new_data.alto = alto;
                    if (moneda != "MXN")
                    {
                        new_data.precio = Math.Round(precio * tc, 2);
                    }
                    else
                    {
                        new_data.precio = Math.Round(precio, 2);
                    }
                    new_data.moneda = moneda;
                }

                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updateListaModulos(int id, string clave, string articulo, string linea, string clave_vidrio, string id_aluminio, string id_herraje, string id_otros, int secciones, string descripcion, string usuario, int id_diseño, bool cs, string parametros, string reglas, bool privado)
        {
            listas = new listas_entities_pva();
            try
            {
                var new_data = (from x in listas.modulos where x.id == id select x).SingleOrDefault();

                if (new_data != null)
                {
                    new_data.articulo = articulo;
                    new_data.linea = linea;
                    new_data.clave_vidrio = clave_vidrio;
                    new_data.id_aluminio = id_aluminio;
                    new_data.id_herraje = id_herraje;
                    new_data.id_otros = id_otros;
                    new_data.secciones = secciones;
                    new_data.descripcion = descripcion;
                    new_data.usuario = usuario;
                    new_data.id_diseño = id_diseño;
                    new_data.cs = cs;
                    new_data.parametros = parametros;
                    new_data.reglas = reglas;
                    new_data.privado = privado;
                }

                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updateCategorias(int id, string categoria, string grupo)
        {
            localDateBaseEntities3 cate = new localDateBaseEntities3();
            try
            {
                var new_data = (from x in cate.categorias where x.Id == id select x).SingleOrDefault();

                if (new_data != null)
                {
                    new_data.categoria1 = categoria;
                    new_data.grupo = grupo;
                }

                cate.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updateProveedores(int id, string proveedor, string grupo)
        {
            localDateBaseEntities3 prove = new localDateBaseEntities3();
            try
            {
                var new_data = (from x in prove.proveedores where x.Id == id select x).SingleOrDefault();

                if (new_data != null)
                {
                    new_data.proveedor = proveedor;
                    new_data.grupo = grupo;
                }

                prove.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updateLineasModulos(int id, string linea_modulo)
        {
            localDateBaseEntities3 linea = new localDateBaseEntities3();
            try
            {
                var new_data = (from x in linea.lineas_modulos where x.id == id select x).SingleOrDefault();

                if (new_data != null)
                {
                    new_data.linea_modulo = linea_modulo;
                }

                linea.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updateEsquemas(int id, string nombre, int filas, int columnas, string diseño, string esquemas, bool marco, string grupo)
        {
            listas = new listas_entities_pva();
            try
            {
                var new_data = (from x in listas.esquemas where x.id == id select x).SingleOrDefault();

                if (new_data != null)
                {
                    new_data.nombre = nombre;
                    new_data.filas = filas;
                    new_data.columnas = columnas;
                    new_data.diseño = diseño;
                    new_data.esquemas = esquemas;
                    new_data.marco = marco;
                    new_data.grupo = grupo;
                }

                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updateColoresAluminio(int id, string color, string proveedor, float precio, float costo_extra)
        {
            listas = new listas_entities_pva();
            try
            {
                var new_data = (from x in listas.colores_aluminio where x.id == id select x).SingleOrDefault();

                if (new_data != null)
                {
                    new_data.color = color;
                    new_data.proveedor = proveedor;
                    new_data.precio = Math.Round(precio, 2);
                    new_data.costo_extra_ml = Math.Round(costo_extra, 2);
                }

                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updatePaquetes(int id, string clave, string items, string type, string articulo)
        {
            listas = new listas_entities_pva();
            try
            {
                var new_data = (from x in listas.paquetes where x.id == id select x).SingleOrDefault();

                if (new_data != null)
                {
                    new_data.comp_items = items;
                    new_data.comp_type = type;
                    new_data.comp_articulo = articulo;                  
                }

                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        public void updateTablesToLocalDB()
        {
            try
            {
                DataTable t1;
                DataTable t2;
                sqlDateBaseManager sql = new sqlDateBaseManager();
                int i;

                t1 = sql.createDataTableFromSQLTable("hojas");

                for (i = 0; i < t1.Rows.Count; i++)
                {
                    if (t1.Rows[i][0] != null && t1.Rows[i][0].ToString() != "")
                    {
                        updateListaPrecioHojas(t1.Rows[i][0].ToString(), t1.Rows[i][1].ToString(), constants.stringToFloat(t1.Rows[i][2].ToString()), constants.stringToFloat(t1.Rows[i][3].ToString()), constants.stringToFloat(t1.Rows[i][6].ToString()), t1.Rows[i][8].ToString(), t1.Rows[i][9].ToString());
                    }
                }

                backgroundWorker1.ReportProgress(10);

                t1.Reset();
                t1 = sql.createDataTableFromSQLTable("acabados");

                for (i = 0; i < t1.Rows.Count; i++)
                {
                    if (t1.Rows[i][0] != null && t1.Rows[i][0].ToString() != "")
                    {
                        updateListaAcabados(t1.Rows[i][0].ToString(), t1.Rows[i][1].ToString(), constants.stringToFloat(t1.Rows[i][2].ToString()), constants.stringToFloat(t1.Rows[i][3].ToString()));
                    }
                }

                backgroundWorker1.ReportProgress(20);

                t1.Reset();
                t1 = sql.createDataTableFromSQLTable("perfiles");

                for (i = 0; i < t1.Rows.Count; i++)
                {
                    if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                    {
                        updateListaPerfilesCuprum((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString(), t1.Rows[i][3].ToString(), t1.Rows[i][4].ToString(), constants.stringToFloat(t1.Rows[i][5].ToString()),
                        constants.stringToFloat(t1.Rows[i][6].ToString()), constants.stringToFloat(t1.Rows[i][7].ToString()), constants.stringToFloat(t1.Rows[i][8].ToString()), constants.stringToFloat(t1.Rows[i][9].ToString()), constants.stringToFloat(t1.Rows[i][10].ToString()), constants.stringToFloat(t1.Rows[i][11].ToString()), constants.stringToFloat(t1.Rows[i][12].ToString()), constants.stringToFloat(t1.Rows[i][13].ToString()), constants.stringToFloat(t1.Rows[i][14].ToString()), constants.stringToFloat(t1.Rows[i][15].ToString()), constants.stringToFloat(t1.Rows[i][16].ToString()), constants.stringToFloat(t1.Rows[i][17].ToString()), constants.stringToFloat(t1.Rows[i][19].ToString()), constants.stringToFloat(t1.Rows[i][20].ToString()), constants.stringToFloat(t1.Rows[i][21].ToString()), t1.Rows[i][23].ToString());
                    }
                }

                backgroundWorker1.ReportProgress(40);

                t1.Reset();
                t1 = sql.createDataTableFromSQLTable("herrajes");

                for (i = 0; i < t1.Rows.Count; i++)
                {
                    if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                    {
                        updateListaHerrajes((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString(), t1.Rows[i][3].ToString(), t1.Rows[i][4].ToString(), t1.Rows[i][5].ToString(), t1.Rows[i][6].ToString(), constants.stringToFloat(t1.Rows[i][7].ToString()), t1.Rows[i][9].ToString());
                    }
                }

                backgroundWorker1.ReportProgress(50);

                t1.Reset();
                t1 = sql.createDataTableFromSQLTable("otros");

                for (i = 0; i < t1.Rows.Count; i++)
                {
                    if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                    {
                        updateListaOtros((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString(), t1.Rows[i][3].ToString(), t1.Rows[i][4].ToString(), t1.Rows[i][5].ToString(), t1.Rows[i][6].ToString(), constants.stringToFloat(t1.Rows[i][7].ToString()), constants.stringToFloat(t1.Rows[i][8].ToString()), constants.stringToFloat(t1.Rows[i][9].ToString()), t1.Rows[i][11].ToString());
                    }
                }

                t1.Reset();
                t1 = sql.createDataTableFromSQLTable("colores_aluminio");

                for (i = 0; i < t1.Rows.Count; i++)
                {
                    if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                    {
                        updateColoresAluminio((int)t1.Rows[i][0], t1.Rows[i][2].ToString(), t1.Rows[i][3].ToString(), constants.stringToFloat(t1.Rows[i][4].ToString()), constants.stringToFloat(t1.Rows[i][5].ToString()));
                    }
                }

                backgroundWorker1.ReportProgress(60);

                t1.Reset();
                t1 = sql.createDataTableFromSQLTable("modulos");

                for (i = 0; i < t1.Rows.Count; i++)
                {
                    if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                    {
                        updateListaModulos((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString(), t1.Rows[i][3].ToString(), t1.Rows[i][4].ToString(), t1.Rows[i][5].ToString(), t1.Rows[i][6].ToString(), t1.Rows[i][7].ToString(), (int)t1.Rows[i][8], t1.Rows[i][9].ToString(), t1.Rows[i][10].ToString(), (int)t1.Rows[i][11], (bool)t1.Rows[i][12], t1.Rows[i][13].ToString(), t1.Rows[i][14].ToString(), (bool)t1.Rows[i][15]);
                    }
                }

                backgroundWorker1.ReportProgress(70);

                t1.Reset();
                t1 = sql.createDataTableFromSQLTable("categorias");

                for (i = 0; i < t1.Rows.Count; i++)
                {
                    if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                    {
                        updateCategorias((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString());
                    }
                }

                t1.Reset();
                t1 = sql.createDataTableFromSQLTable("proveedores");

                for (i = 0; i < t1.Rows.Count; i++)
                {
                    if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                    {
                        updateProveedores((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString());
                    }
                }

                t1.Reset();
                t1 = sql.createDataTableFromSQLTable("lineas_modulos");

                for (i = 0; i < t1.Rows.Count; i++)
                {
                    if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                    {
                        updateLineasModulos((int)t1.Rows[i][0], t1.Rows[i][1].ToString());
                    }
                }

                t1.Reset();
                t1 = sql.createDataTableFromSQLTable("esquemas");

                for (i = 0; i < t1.Rows.Count; i++)
                {
                    if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                    {
                        updateEsquemas((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), (int)t1.Rows[i][2], (int)t1.Rows[i][3], t1.Rows[i][4].ToString(), t1.Rows[i][5].ToString(), (bool)t1.Rows[i][6], t1.Rows[i][7].ToString());
                    }
                }

                t1.Reset();
                t1 = sql.createDataTableFromSQLTable("paquetes");

                for (i = 0; i < t1.Rows.Count; i++)
                {
                    if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                    {
                        updatePaquetes((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString(), t1.Rows[i][3].ToString(), t1.Rows[i][4].ToString());
                    }
                }

                backgroundWorker1.ReportProgress(80);

                t1.Reset();
                t1 = sql.createDataTableFromSQLTable("costo_corte_precio");
                t2 = sql.createDataTableFromSQLTable("instalado");

                for (i = 0; i <= t1.Rows.Count; i++)
                {
                    if (t1.Rows[i][0] != null && t1.Rows[i][0].ToString() != "")
                    {
                        updateListaCostoCorteInstalado(t1.Rows[i][0].ToString(), t1.Rows[i][1].ToString(), constants.stringToFloat(t1.Rows[i][5].ToString()), getPrecioInstalado(t2, t1.Rows[i][0].ToString()), t1.Rows[i][9].ToString(), t1.Rows[i][10].ToString());
                        updateListaPrecioCorteInstalado(t1.Rows[i][0].ToString(), t1.Rows[i][1].ToString(), constants.stringToFloat(t1.Rows[i][7].ToString()), getPrecioInstalado(t2, t1.Rows[i][0].ToString()), t1.Rows[i][9].ToString(), t1.Rows[i][10].ToString());
                    }
                }

                backgroundWorker1.ReportProgress(100);

                t1.Dispose();
                t2.Dispose();
            }
            catch (Exception) { }
        }

        private float getPrecioInstalado(DataTable table, string clave)
        {
            float r = 0;
            foreach (DataRow x in table.Rows)
            {
                if (x[0] != null)
                {
                    if (x[0].ToString() == clave)
                    {
                        r = constants.stringToFloat(x[7].ToString());
                    }
                }
            }
            return r;
        }
        //ends updaters ----------------------------------------------------------------------------------------------------------------->

        //tools ----------------------------------------->
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
        //   

        private void optimizarBaseDeDatosLocalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                if (MessageBox.Show("Se volverá a cargar la información desde la base de datos. ¿Desea proceder?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    new loading_form().ShowDialog();
                }                      
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible conectarse al servidor.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //ends load ----------------------------------------------------------------------------------------------------------------------

        // Inicia Cristales --------------------------------------------------------------------------------------------------------------
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listas = new listas_entities_pva();
            textBox9.Clear();

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    var costo_corte = from x in listas.lista_costo_corte_e_instalado
                                      where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                      orderby x.articulo ascending
                                      select new
                                      {
                                          Clave = x.clave,
                                          Artículo = x.articulo,
                                          Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                          Costo_Instalado = "$" + x.costo_instalado
                                      };
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = costo_corte.ToList();
                    break;
                case 1:
                    var precio_corte = from x in listas.lista_precio_corte_e_instalado
                                       where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                       orderby x.articulo ascending
                                       select new
                                       {
                                           Clave = x.clave,
                                           Artículo = x.articulo,
                                           Precio_Corte_m2 = "$" + x.precio_venta_corte_m2,
                                           Precio_Instalado = "$" + x.precio_venta_instalado
                                       };
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = precio_corte.ToList();
                    break;
                case 2:
                    var hojas = from x in listas.lista_precios_hojas
                                where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                orderby x.articulo ascending
                                select new
                                {
                                    Clave = x.clave,
                                    Artículo = x.articulo,
                                    Largo = x.largo + "m",
                                    Alto = x.alto + "m",
                                    Precio_Hoja = "$" + x.precio_hoja
                                };
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = hojas.ToList();
                    break;
                case 3:
                    var perfiles = from x in listas.perfiles
                                          where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                   orderby x.articulo ascending
                                   select new
                                          {
                                              Id = x.id,
                                              Clave = x.clave,
                                              Artículo = x.articulo,
                                              Linea = x.linea,
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
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = perfiles.ToList();
                    break;
                case 4:
                    var herrajes = from x in listas.herrajes
                                   where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                   orderby x.articulo ascending
                                   select new
                                   {
                                       Id = x.id,
                                       Clave = x.clave,
                                       Artículo = x.articulo,
                                       Proveedor = x.proveedor,
                                       Linea = x.linea,
                                       Color = x.color,
                                       Precio = "$" + x.precio
                                   };
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = herrajes.ToList();
                    break;
                case 5:
                    var otros = from x in listas.otros
                                where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                orderby x.articulo ascending
                                select new
                                {
                                    Id = x.id,
                                    Clave = x.clave,
                                    Artículo = x.articulo,
                                    Proveedor = x.proveedor,
                                    Linea = x.linea,
                                    Color = x.color,
                                    Precio = "$" + x.precio
                                };
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = otros.ToList();
                    break;
                case 6:                   
                    var modulos = from x in listas.modulos
                                  join y in listas.esquemas on x.id_diseño equals y.id
                                  where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                  orderby x.articulo ascending
                                  select new
                                  {
                                     Id = x.id,
                                     Clave = x.clave,
                                     Diseño = y.diseño,
                                     Artículo = x.articulo,
                                     Linea = x.linea,
                                     Descripcion = x.descripcion
                                  };
                    datagridviewNE2.DataSource = null;
                    datagridviewNE2.DataSource = modulos.ToList();
                    setDiseñoColorGrid();
                    break;
                default: break;
            }
            listas.Dispose();
        }      

        private void datagridviewNE2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE2.Rows.Count > 0)
            {              
                setArticuloToVer();
            }
        }
        //ENDS DATAGRID 1 LISTAS
           
        //Calculos de cristales
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
                calculoGeneralCristales("_instalado");
            }
            else
            {
                checkBox1.Checked = true;
                calculoGeneralCristales("_m2");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox2.Checked = false;
                calculoGeneralCristales("_m2");
            }
            else
            {
                checkBox2.Checked = true;
                calculoGeneralCristales("_instalado");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox2.Text) == true)
            {
                if (float.Parse(textBox2.Text) >= 0)
                {
                    if (checkBox1.Checked == true)
                    {
                        calculoGeneralCristales("_m2");
                    }
                    else
                    {
                        calculoGeneralCristales("_instalado");
                    }
                }
                else
                {
                    textBox2.Text = "";
                    label17.Text = p.ToString("0.00");
                    label41.Text = p.ToString("0.00");
                }
            }
            else
            {
                textBox2.Text = "";
                label17.Text = p.ToString("0.00");
                label41.Text = p.ToString("0.00");
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox3.Text) == true)
            {
                if (float.Parse(textBox2.Text) >= 0)
                {
                    if (checkBox1.Checked == true)
                    {
                        calculoGeneralCristales("_m2");
                    }
                    else
                    {
                        calculoGeneralCristales("_instalado");
                    }
                }
                else
                {
                    textBox3.Text = "";
                    label17.Text = p.ToString("0.00");
                    label41.Text = p.ToString("0.00");
                }
            }
            else
            {
                textBox3.Text = "";
                label17.Text = p.ToString("0.00");
                label41.Text = p.ToString("0.00");
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox5.Text) == true)
            {
                if (float.Parse(textBox5.Text) >= 0)
                {
                    if (comboBox1.SelectedIndex == 2)
                    {
                        calculoGeneralCristales("_hoja");
                    }
                    else {
                        if (checkBox1.Checked == true)
                        {
                            calculoGeneralCristales("_m2");
                        }
                        else
                        {
                            calculoGeneralCristales("_instalado");
                        }
                    }
                }
                else
                {
                    textBox5.Text = "";
                    label17.Text = p.ToString("0.00");
                    label41.Text = p.ToString("0.00");
                }
            }
            else
            {
                textBox5.Text = "";
                label17.Text = p.ToString("0.00");
                label41.Text = p.ToString("0.00");
            }
        }

        private float precioMinimo()
        {
            if (checkBox39.Checked == true)
            {
                return 1.0f;
            }
            else
            {
                return 1.5f;
            }
        }      

        private void calculoGeneralCristales(string tipo)
        {
            listas_entities_pva lista = new listas_entities_pva();
            var acabados_vidrio = (from x in lista.acabados where x.clave == clave_tem select x).SingleOrDefault();
            float filo_muerto = 1;
            if (checkBox4.Checked)
            {
                filo_muerto = (constants.stringToFloat(textBox10.Text) / 100) + 1;
            }
            if (tipo == "_m2")
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    label17.Text = Math.Round((((_m2 * (stringToFloat(textBox2.Text) / 1000) * (stringToFloat(textBox3.Text) / 1000) * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) + (((_m2 * (stringToFloat(textBox2.Text) / 1000) * (stringToFloat(textBox3.Text) / 1000) * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) * (stringToFloat(textBox26.Text) / 100))) * constants.iva, 2).ToString("0.00");
                    label41.Text = Math.Round(((_m2 * (stringToFloat(textBox2.Text) / 1000) * (stringToFloat(textBox3.Text) / 1000) * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) + (((_m2 * (stringToFloat(textBox2.Text) / 1000) * (stringToFloat(textBox3.Text) / 1000) * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) * (stringToFloat(textBox26.Text) / 100)), 2).ToString("0.00");
                }
                else if(comboBox1.SelectedIndex == 1)
                {
                    label17.Text = Math.Round((((_m2 * precioMinimo() * (stringToFloat(textBox2.Text) / 1000) * (stringToFloat(textBox3.Text) / 1000) * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) + (((_m2 * precioMinimo() * (stringToFloat(textBox2.Text) / 1000) * (stringToFloat(textBox3.Text) / 1000) * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) * (stringToFloat(textBox26.Text) / 100))) * constants.iva, 2).ToString("0.00");
                    label41.Text = Math.Round(((_m2 * precioMinimo() * (stringToFloat(textBox2.Text) / 1000) * (stringToFloat(textBox3.Text) / 1000) * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) + (((_m2 * precioMinimo() * (stringToFloat(textBox2.Text) / 1000) * (stringToFloat(textBox3.Text) / 1000) * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) * (stringToFloat(textBox26.Text) / 100)), 2).ToString("0.00");
                }
            }
            else if (tipo == "_instalado")
            {
                label17.Text = Math.Round((((_instalado * (stringToFloat(textBox2.Text) / 1000) * (stringToFloat(textBox3.Text) / 1000) * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) + (((_instalado * (stringToFloat(textBox2.Text) / 1000) * (stringToFloat(textBox3.Text) / 1000) * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) * (stringToFloat(textBox26.Text) / 100))) * constants.iva, 2).ToString("0.00");
                label41.Text = Math.Round(((_instalado * (stringToFloat(textBox2.Text) / 1000) * (stringToFloat(textBox3.Text) / 1000) * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) + (((_instalado * (stringToFloat(textBox2.Text) / 1000) * (stringToFloat(textBox3.Text) / 1000) * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) * (stringToFloat(textBox26.Text) / 100)), 2).ToString("0.00");
            }
            else if (tipo == "_hoja")
            {
                label17.Text = Math.Round((((_hoja * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) + (((_hoja * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) * (stringToFloat(textBox26.Text) / 100))) * constants.iva, 2).ToString("0.00");
                label41.Text = Math.Round(((_hoja * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) + (((_hoja * stringToFloat(textBox5.Text) * filo_muerto) + calcularAcabados()) * (stringToFloat(textBox26.Text) / 100)), 2).ToString("0.00");
            }
        }

        //Descuento cristales
        private void textBox26_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox26.Text) == true)
            {              
                if (comboBox1.SelectedIndex == 2)
                {
                    calculoGeneralCristales("_hoja");
                }
                else
                {
                    if (checkBox1.Checked == true)
                    {
                        calculoGeneralCristales("_m2");
                    }
                    else
                    {
                        calculoGeneralCristales("_instalado");
                    }
                }              
            }
            else
            {
                textBox26.Text = "";
                label17.Text = p.ToString("0.00");
                label41.Text = p.ToString("0.00");
            }
        }
        //

        //poner articulo para 'ver'
        private void setArticuloToVer()
        {
            try
            {
                string clave;
                int id;
                string[] colores = null;
                constants.id_articulo_cotizacion = -1;
                listas = new listas_entities_pva();
                setEditImage(false, false);

                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        disableTabPage();
                        ((Control)tabPage1).Enabled = true;
                        tabControl1.SelectedTab = tabPage1;
                        clave = datagridviewNE2.CurrentRow.Cells[0].Value.ToString();
                        clave_tem = clave;
                        datagridviewNE3.Rows.Clear();

                        var costos = (from x in listas.lista_costo_corte_e_instalado where x.clave == clave select x).SingleOrDefault();

                        if (costos != null)
                        {
                            this.descripcion = string.Empty;
                            checkBox39.Enabled = false;
                            label7.Text = comboBox1.Text;
                            label8.Text = costos.articulo;
                            label9.Text = costos.clave;
                            label85.Text = costos.proveedor;
                            checkBox1.Text = "Costo Corte m2";
                            checkBox2.Text = "Costo Instalado";

                            textBox2.Text = "1000";
                            textBox3.Text = "1000";
                            textBox5.Text = "1";
                            textBox26.Text = constants.desc_cristales.ToString();
                            
                            textBox2.ReadOnly = false;
                            textBox3.ReadOnly = false;
                            checkBox1.Checked = true;
                            checkBox1.Visible = true;
                            checkBox2.Visible = true;
                            checkBox4.Checked = false;
                            textBox10.Text = "20";

                            _m2 = (float)costos.costo_corte_m2;
                            _instalado = (float)costos.costo_instalado;

                            label17.Text = Math.Round((float)(_m2 * constants.iva), 2).ToString();
                            label41.Text = _m2.ToString();
                          
                            constants.setImage(costos.proveedor, costos.clave, "png", pictureBox1);

                            //Calcular
                            calculoGeneralCristales("_m2");
                        }
                        break;
                    case 1:
                        disableTabPage();
                        ((Control)tabPage1).Enabled = true;
                        tabControl1.SelectedTab = tabPage1;
                        clave = datagridviewNE2.CurrentRow.Cells[0].Value.ToString();
                        clave_tem = clave;
                        datagridviewNE3.Rows.Clear();

                        var precios = (from x in listas.lista_precio_corte_e_instalado where x.clave == clave select x).SingleOrDefault();

                        if (precios != null)
                        {
                            this.descripcion = string.Empty;
                            checkBox39.Enabled = true;
                            label7.Text = comboBox1.Text;
                            label8.Text = precios.articulo;
                            label9.Text = precios.clave;
                            label85.Text = precios.proveedor;
                            checkBox1.Text = "Precio Corte m2";
                            checkBox2.Text = "Precio Instalado";

                            textBox2.Text = "1000";
                            textBox3.Text = "1000";
                            textBox5.Text = "1";
                            textBox26.Text = constants.desc_cristales.ToString();
                           
                            textBox2.ReadOnly = false;
                            textBox3.ReadOnly = false;
                            checkBox1.Checked = true;
                            checkBox1.Visible = true;
                            checkBox2.Visible = true;
                            checkBox4.Checked = false;
                            textBox10.Text = "20";

                            _m2 = (float)precios.precio_venta_corte_m2;
                            _instalado = (float)precios.precio_venta_instalado;

                            label17.Text = Math.Round((float)(_m2 * constants.iva), 2).ToString();
                            label41.Text = _m2.ToString();
                            
                            constants.setImage(precios.proveedor, precios.clave, "png", pictureBox1);

                            //Calcular
                            calculoGeneralCristales("_m2");
                        }
                        break;
                    case 2:
                        disableTabPage();
                        ((Control)tabPage1).Enabled = true;
                        tabControl1.SelectedTab = tabPage1;
                        clave = datagridviewNE2.CurrentRow.Cells[0].Value.ToString();
                        clave_tem = clave;
                        datagridviewNE3.Rows.Clear();

                        var hojas = (from x in listas.lista_precios_hojas where x.clave == clave select x).SingleOrDefault();

                        if (hojas != null)
                        {
                            this.descripcion = string.Empty;
                            checkBox39.Enabled = false;
                            label7.Text = comboBox1.Text;
                            label8.Text = hojas.articulo;
                            label9.Text = hojas.clave;
                            label85.Text = hojas.proveedor;
                            checkBox1.Text = "Precio Corte m2";
                            checkBox2.Text = "Precio Instalado";

                            textBox5.Text = "1";
                            textBox26.Text = constants.desc_cristales.ToString();
                           
                            textBox2.Text = Math.Round((float)hojas.largo * 1000, 2).ToString();
                            textBox3.Text = Math.Round((float)hojas.alto * 1000, 2).ToString();
                            textBox2.ReadOnly = true;
                            textBox3.ReadOnly = true;
                            checkBox1.Visible = false;
                            checkBox2.Visible = false;
                            checkBox4.Checked = false;
                            textBox10.Text = "20";

                            _hoja = (float)hojas.precio_hoja;

                            label17.Text = Math.Round((float)(_hoja * constants.iva), 2).ToString();
                            label41.Text = _hoja.ToString();

                            constants.setImage(hojas.proveedor, hojas.clave, "png", pictureBox1);

                            //Calcular
                            calculoGeneralCristales("_hoja");
                        }
                        break;
                    case 3:
                        disableTabPage();
                        ((Control)tabPage3).Enabled = true;
                        tabControl1.SelectedTab = tabPage3;
                        id = (int)datagridviewNE2.CurrentRow.Cells[0].Value;
                        id_tem = id;

                        var perfiles = (from x in listas.perfiles where x.id == id select x).SingleOrDefault();

                        if (perfiles != null)
                        {
                            this.descripcion = string.Empty;
                            label28.Text = comboBox1.Text;
                            label29.Text = perfiles.clave;
                            label30.Text = perfiles.articulo;
                            label31.Text = perfiles.linea;
                            label87.Text = perfiles.proveedor;
                            label111.Text = perfiles.largo.ToString();

                            textBox6.Text = "1000";
                            textBox7.Text = "1";
                            label38.Text = "0.00";
                            label104.Text = "0.00";
                            label34.Text = p.ToString("0.00");
                            textBox8.Text = "";
                            textBox27.Text = constants.desc_aluminio.ToString();
                           
                            constants.setImage(perfiles.proveedor, perfiles.id.ToString(), "jpg", pictureBox2);
                            constants.setImage(perfiles.proveedor, perfiles.proveedor, "jpg", pictureBox3);

                            comboBox3.Items.Clear();
                            if (perfiles.crudo > 0)
                            {
                                comboBox3.Items.Add("crudo");
                            }
                            if (perfiles.blanco > 0)
                            {
                                comboBox3.Items.Add("blanco");
                            }
                            if (perfiles.hueso > 0)
                            {
                                comboBox3.Items.Add("hueso");
                            }
                            if (perfiles.champagne > 0)
                            {
                                comboBox3.Items.Add("champagne");
                            }
                            if (perfiles.gris > 0)
                            {
                                comboBox3.Items.Add("gris");
                            }
                            if (perfiles.brillante > 0)
                            {
                                comboBox3.Items.Add("brillante");
                            }
                            if (perfiles.natural_1 > 0)
                            {
                                comboBox3.Items.Add("natural");
                            }
                            if (perfiles.madera > 0)
                            {
                                comboBox3.Items.Add("madera");
                            }
                            if (perfiles.chocolate > 0)
                            {
                                comboBox3.Items.Add("chocolate");
                            }
                            if (perfiles.acero_inox > 0)
                            {
                                comboBox3.Items.Add("acero_inox");
                            }
                            if (perfiles.bronce > 0)
                            {
                                comboBox3.Items.Add("bronce");
                            }
                            var color = from x in listas.colores_aluminio select x;

                            if (color != null)
                            {
                                comboBox4.Items.Clear();
                                foreach (var c in color)
                                {
                                    comboBox4.Items.Add(c.clave);
                                }
                            }
                        }
                        break;
                    case 4:
                        disableTabPage();
                        ((Control)tabPage2).Enabled = true;
                        tabControl1.SelectedTab = tabPage2;
                        id = (int)datagridviewNE2.CurrentRow.Cells[0].Value;
                        id_tem = id;

                        var herrajes = (from x in listas.herrajes where x.id == id select x).SingleOrDefault();

                        if (herrajes != null)
                        {
                            this.descripcion = string.Empty;
                            label55.Text = comboBox1.Text;
                            label60.Text = herrajes.clave;
                            label62.Text = herrajes.articulo;
                            label61.Text = herrajes.proveedor;
                            label59.Text = herrajes.linea;
                            richTextBox1.Text = herrajes.caracteristicas;

                            textBox33.Text = "1";
                            label63.Text = p.ToString("0.00");
                            label43.Text = p.ToString("0.00");
                            textBox29.Text = constants.desc_herrajes.ToString();
                           
                            comboBox8.Items.Clear();
                            if (herrajes.color != "")
                            {
                                colores = herrajes.color.Split(',');
                                if (colores.Length > 0)
                                {
                                    comboBox8.Items.Clear();
                                    foreach (string n in colores)
                                    {
                                        comboBox8.Items.Add(n);
                                    }
                                }
                                else
                                {
                                    comboBox8.Items.Add(herrajes.color);
                                }
                                if (comboBox8.Items.Count > 0)
                                {
                                    comboBox8.SelectedIndex = 0;
                                }
                            }

                            constants.setImage(herrajes.proveedor, herrajes.id.ToString(), "jpg", pictureBox4);
                            constants.setImage(herrajes.proveedor, herrajes.proveedor, "jpg", pictureBox5);

                            //Calcular
                            calculoHerrajes();
                        }
                        break;
                    case 5:
                        disableTabPage();
                        ((Control)tabPage4).Enabled = true;
                        tabControl1.SelectedTab = tabPage4;
                        id = (int)datagridviewNE2.CurrentRow.Cells[0].Value;
                        id_tem = id;

                        var otros = (from x in listas.otros where x.id == id select x).SingleOrDefault();

                        if (otros != null)
                        {
                            this.descripcion = string.Empty;
                            label75.Text = comboBox1.Text;
                            label76.Text = otros.clave;
                            label77.Text = otros.articulo;
                            label78.Text = otros.proveedor;
                            label79.Text = otros.linea;
                            richTextBox2.Text = otros.caracteristicas;
                            textBox39.Enabled = false;
                            textBox37.Enabled = false;
                            richTextBox2.ReadOnly = true;
                            richTextBox2.Enabled = false;
                            
                            constants.setImage(otros.proveedor, otros.id.ToString(), "jpg", pictureBox6);
                            constants.setImage(otros.proveedor, otros.proveedor, "jpg", pictureBox7);

                            textBox30.Text = "1";
                            label74.Text = p.ToString("0.00");
                            label106.Text = p.ToString("0.00");
                            textBox39.Text = "";

                            comboBox9.Items.Clear();
                            if (otros.color != "")
                            {
                                colores = otros.color.Split(',');
                                if (colores.Length > 0)
                                {
                                    comboBox9.Items.Clear();
                                    foreach (string n in colores)
                                    {
                                        comboBox9.Items.Add(n);
                                    }
                                }
                                else
                                {
                                    comboBox9.Items.Add(otros.color);
                                }
                                if (comboBox9.Items.Count > 0)
                                {
                                    comboBox9.SelectedIndex = 0;
                                }
                            }

                            textBox39.Text = "1000";
                            textBox37.Text = "1000";
                            textBox31.Text = constants.desc_otros.ToString();

                            if (otros.linea == "concepto-extra")
                            {
                                richTextBox2.Enabled = true;
                                richTextBox2.ReadOnly = false;
                            }

                            if (otros.largo > 0)
                            {
                                textBox39.Enabled = true;
                                textBox39.Text = otros.largo.ToString();
                            }

                            if (otros.alto > 0)
                            {
                                textBox37.Enabled = true;
                                textBox37.Text = otros.alto.ToString();
                            }

                            //Calcular
                            calculoOtros();
                        }
                        break;
                    case 6:
                        disableTabPage();
                        ((Control)tabPage5).Enabled = true;
                        tabControl1.SelectedTab = tabPage5;
                        id = (int)datagridviewNE2.CurrentRow.Cells[0].Value;
                        id_tem = id;
                        hScrollBar1.Maximum = panel1.Width;

                        var modulos = (from x in listas.modulos where x.id == id select x).SingleOrDefault();

                        if (modulos != null)
                        {
                            label99.Text = comboBox1.Text;
                            label90.Text = modulos.clave;
                            label92.Text = modulos.articulo;
                            label94.Text = modulos.linea;
                            richTextBox3.Text = modulos.descripcion;
                            opened_module = id;
                            if (modulos.linea == "CANCEL BAÑO")
                            {
                                constants.setImage("series", "CB", "png", pictureBox10);
                            }
                            else
                            {
                                constants.setImage("series", modulos.linea, "png", pictureBox10);
                            }
                            if (modulos.privado == true)
                            {
                                pictureBox11.Image = Properties.Resources.Lock_Lock_icon;
                            }
                            else
                            {
                                pictureBox11.Image = null;
                            }
                            var diseño = (from x in listas.esquemas where x.id == modulos.id_diseño select x).SingleOrDefault();

                            if (diseño != null)
                            {
                                tableLayoutPanel1.Controls.Clear();
                                tableLayoutPanel1.RowCount = (int)diseño.filas;
                                tableLayoutPanel1.ColumnCount = (int)diseño.columnas;
                                getEsquemasFromDiseño(diseño.esquemas);
                                foreach (string e in esquemas)
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
                                }
                                if (diseño.marco == true)
                                {
                                    if (diseño.grupo == "puerta")
                                    {
                                        tableLayoutPanel1.Padding = new Padding(10, 10, 10, 0);
                                        if (Size.Width > 1060)
                                        {
                                            tableLayoutPanel1.Width = hScrollBar1.Maximum / 2;
                                            hScrollBar1.Value = hScrollBar1.Maximum / 2;
                                        }
                                        resize = true;
                                    }
                                    else
                                    {
                                        if (diseño.columnas > 2)
                                        {
                                            tableLayoutPanel1.Padding = new Padding(10, 10, 10, 10);
                                            tableLayoutPanel1.Width = hScrollBar1.Maximum;
                                            hScrollBar1.Value = hScrollBar1.Maximum;
                                            resize = false;
                                        }
                                        else
                                        {
                                            tableLayoutPanel1.Padding = new Padding(10, 10, 10, 10);
                                            if (Size.Width > 1060)
                                            {
                                                tableLayoutPanel1.Width = hScrollBar1.Maximum / 2;
                                                hScrollBar1.Value = hScrollBar1.Maximum / 2;
                                            }
                                            resize = true;
                                        }
                                    }
                                }
                                else
                                {
                                    tableLayoutPanel1.Padding = new Padding(0, 0, 0, 0);
                                    if (diseño.columnas > 2)
                                    {
                                        tableLayoutPanel1.Width = hScrollBar1.Maximum;
                                        hScrollBar1.Value = hScrollBar1.Maximum;
                                        resize = false;
                                    }
                                    else
                                    {
                                        if (Size.Width > 1060)
                                        {
                                            tableLayoutPanel1.Width = hScrollBar1.Maximum / 2;
                                            hScrollBar1.Value = hScrollBar1.Maximum / 2;
                                        }
                                        resize = true;
                                    }
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
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception) { }       
        }
        //

        //cargar esquemas
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

        //este es para cotizaciones
        private void setValuesToEdit(int type)
        {
            try {
                listas = new listas_entities_pva();
                setEditImage(true, true);
                string[] colores = null;

                switch (type)
                {
                    case 1:
                        disableTabPage();
                        ((Control)tabPage1).Enabled = true;
                        tabControl1.SelectedTab = tabPage1;
                        var costos = (from x in listas.lista_costo_corte_e_instalado where x.clave == clave_tem select x).SingleOrDefault();

                        if (costos != null)
                        {
                            this.descripcion = string.Empty;
                            label7.Text = comboBox1.Text;
                            label8.Text = costos.articulo;
                            label9.Text = costos.clave;
                            label85.Text = costos.proveedor;
                            checkBox1.Text = "Costo Corte m2";
                            checkBox2.Text = "Costo Instalado";

                            textBox2.Text = "1000";
                            textBox3.Text = "1000";
                            textBox5.Text = "1";
                            textBox26.Text = constants.desc_cristales.ToString();

                            textBox2.ReadOnly = false;
                            textBox3.ReadOnly = false;
                            checkBox1.Visible = true;
                            checkBox2.Visible = true;
                            checkBox4.Checked = false;
                            textBox10.Text = "20";

                            _m2 = (float)costos.costo_corte_m2;
                            _instalado = (float)costos.costo_instalado;

                            label17.Text = Math.Round((float)(_m2 * constants.iva), 2).ToString();
                            label41.Text = _m2.ToString();
                           
                            constants.setImage(costos.proveedor, costos.clave, "png", pictureBox1);

                            //Calcular
                            calculoGeneralCristales("_m2");
                        }
                        break;
                    case 2:
                        disableTabPage();
                        ((Control)tabPage1).Enabled = true;
                        tabControl1.SelectedTab = tabPage1;
                        var precios = (from x in listas.lista_precio_corte_e_instalado where x.clave == clave_tem select x).SingleOrDefault();

                        if (precios != null)
                        {
                            this.descripcion = string.Empty;
                            label7.Text = comboBox1.Text;
                            label8.Text = precios.articulo;
                            label9.Text = precios.clave;
                            label85.Text = precios.proveedor;
                            checkBox1.Text = "Precio Corte m2";
                            checkBox2.Text = "Precio Instalado";

                            textBox2.Text = "1000";
                            textBox3.Text = "1000";
                            textBox5.Text = "1";
                            textBox26.Text = constants.desc_cristales.ToString();

                            textBox2.ReadOnly = false;
                            textBox3.ReadOnly = false;
                            checkBox1.Visible = true;
                            checkBox2.Visible = true;
                            checkBox4.Checked = false;
                            textBox10.Text = "20";

                            _m2 = (float)precios.precio_venta_corte_m2;
                            _instalado = (float)precios.precio_venta_instalado;

                            label17.Text = Math.Round((float)(_m2 * constants.iva), 2).ToString();
                            label41.Text = _m2.ToString();
                           
                            constants.setImage(precios.proveedor, precios.clave, "png", pictureBox1);

                            //Calcular
                            calculoGeneralCristales("_m2");
                        }
                        break;
                    case 3:
                        disableTabPage();
                        ((Control)tabPage1).Enabled = true;
                        tabControl1.SelectedTab = tabPage1;
                        var hojas = (from x in listas.lista_precios_hojas where x.clave == clave_tem select x).SingleOrDefault();

                        if (hojas != null)
                        {
                            this.descripcion = string.Empty;
                            label7.Text = comboBox1.Text;
                            label8.Text = hojas.articulo;
                            label9.Text = hojas.clave;
                            label85.Text = hojas.proveedor;
                            checkBox1.Text = "Precio Corte m2";
                            checkBox2.Text = "Precio Instalado";

                            textBox5.Text = "1";
                            textBox26.Text = constants.desc_cristales.ToString();

                            textBox2.Text = hojas.largo.ToString();
                            textBox3.Text = hojas.alto.ToString();
                            textBox2.ReadOnly = true;
                            textBox3.ReadOnly = true;
                            checkBox1.Visible = false;
                            checkBox2.Visible = false;
                            checkBox4.Checked = false;
                            textBox10.Text = "20";

                            _hoja = (float)hojas.precio_hoja;

                            label17.Text = Math.Round((float)(_hoja * constants.iva), 2).ToString();
                            label41.Text = _hoja.ToString();
                           
                            constants.setImage(hojas.proveedor, hojas.clave, "png", pictureBox1);

                            //Calcular
                            calculoGeneralCristales("_hoja");
                        }
                        break;
                    case 4:
                        disableTabPage();
                        ((Control)tabPage3).Enabled = true;
                        tabControl1.SelectedTab = tabPage3;

                        var perfiles = (from x in listas.perfiles where x.id == id_tem select x).SingleOrDefault();

                        if (perfiles != null)
                        {
                            this.descripcion = string.Empty;
                            label28.Text = comboBox1.Text;
                            label29.Text = perfiles.clave;
                            label30.Text = perfiles.articulo;
                            label31.Text = perfiles.linea;
                            label87.Text = perfiles.proveedor;
                            label111.Text = perfiles.largo.ToString();

                            textBox6.Text = "1000";
                            textBox7.Text = "1";
                            label34.Text = p.ToString("0.00");
                            label38.Text = "0.00";
                            label104.Text = "0.00";
                            textBox8.Text = "";
                            textBox27.Text = constants.desc_aluminio.ToString();

                            constants.setImage(perfiles.proveedor, perfiles.id.ToString(), "jpg", pictureBox2);
                            constants.setImage(perfiles.proveedor, perfiles.proveedor, "jpg", pictureBox3);

                            comboBox3.Items.Clear();
                            if (perfiles.crudo > 0)
                            {
                                comboBox3.Items.Add("crudo");
                            }
                            if (perfiles.blanco > 0)
                            {
                                comboBox3.Items.Add("blanco");
                            }
                            if (perfiles.hueso > 0)
                            {
                                comboBox3.Items.Add("hueso");
                            }
                            if (perfiles.champagne > 0)
                            {
                                comboBox3.Items.Add("champagne");
                            }
                            if (perfiles.gris > 0)
                            {
                                comboBox3.Items.Add("gris");
                            }
                            if (perfiles.brillante > 0)
                            {
                                comboBox3.Items.Add("brillante");
                            }
                            if (perfiles.natural_1 > 0)
                            {
                                comboBox3.Items.Add("natural");
                            }
                            if (perfiles.madera > 0)
                            {
                                comboBox3.Items.Add("madera");
                            }
                            if (perfiles.chocolate > 0)
                            {
                                comboBox3.Items.Add("chocolate");
                            }
                            if (perfiles.acero_inox > 0)
                            {
                                comboBox3.Items.Add("acero_inox");
                            }
                            if (perfiles.bronce > 0)
                            {
                                comboBox3.Items.Add("bronce");
                            }
                            var color = from x in listas.colores_aluminio select x;

                            if (color != null)
                            {
                                comboBox4.Items.Clear();
                                foreach (var c in color)
                                {
                                    comboBox4.Items.Add(c.clave);
                                }
                            }
                        }
                        break;
                    case 5:
                        disableTabPage();
                        ((Control)tabPage2).Enabled = true;
                        tabControl1.SelectedTab = tabPage2;

                        var herrajes = (from x in listas.herrajes where x.id == id_tem select x).SingleOrDefault();

                        if (herrajes != null)
                        {
                            this.descripcion = string.Empty;
                            label55.Text = comboBox1.Text;
                            label60.Text = herrajes.clave;
                            label62.Text = herrajes.articulo;
                            label61.Text = herrajes.proveedor;
                            label59.Text = herrajes.linea;
                            richTextBox1.Text = herrajes.caracteristicas;

                            textBox33.Text = "1";
                            label63.Text = p.ToString("0.00");
                            label43.Text = p.ToString("0.00");
                            textBox29.Text = constants.desc_herrajes.ToString();

                            comboBox8.Items.Clear();
                            if (herrajes.color != "")
                            {
                                colores = herrajes.color.Split(',');
                                if (colores.Length > 0)
                                {
                                    comboBox8.Items.Clear();
                                    foreach (string n in colores)
                                    {
                                        comboBox8.Items.Add(n);
                                    }
                                }
                                else
                                {
                                    comboBox8.Items.Add(herrajes.color);
                                }
                                if (comboBox8.Items.Count > 0)
                                {
                                    comboBox8.SelectedIndex = 0;
                                }
                            }

                            constants.setImage(herrajes.proveedor, herrajes.id.ToString(), "jpg", pictureBox4);
                            constants.setImage(herrajes.proveedor, herrajes.proveedor, "jpg", pictureBox5);

                            //Calcular
                            calculoHerrajes();
                        }
                        break;
                    case 6:
                        disableTabPage();
                        ((Control)tabPage4).Enabled = true;
                        tabControl1.SelectedTab = tabPage4;

                        var otros = (from x in listas.otros where x.id == id_tem select x).SingleOrDefault();

                        if (otros != null)
                        {
                            this.descripcion = string.Empty;
                            label75.Text = comboBox1.Text;
                            label76.Text = otros.clave;
                            label77.Text = otros.articulo;
                            label78.Text = otros.proveedor;
                            label79.Text = otros.linea;
                            textBox39.Enabled = false;
                            textBox37.Enabled = false;
                            richTextBox2.ReadOnly = true;
                            richTextBox2.Enabled = false;
                            richTextBox2.Text = otros.caracteristicas;
                            constants.setImage(otros.proveedor, otros.id.ToString(), "jpg", pictureBox6);
                            constants.setImage(otros.proveedor, otros.proveedor, "jpg", pictureBox7);

                            textBox30.Text = "1";
                            label74.Text = p.ToString("0.00");
                            label106.Text = p.ToString("0.00");
                            textBox39.Text = "";

                            comboBox9.Items.Clear();
                            if (otros.color != "")
                            {
                                colores = otros.color.Split(',');
                                if (colores.Length > 0)
                                {
                                    comboBox9.Items.Clear();
                                    foreach (string n in colores)
                                    {
                                        comboBox9.Items.Add(n);
                                    }
                                }
                                else
                                {
                                    comboBox9.Items.Add(otros.color);
                                }
                                if (comboBox9.Items.Count > 0)
                                {
                                    comboBox9.SelectedIndex = 0;
                                }
                            }

                            textBox39.Text = "1000";
                            textBox37.Text = "1000";
                            textBox31.Text = constants.desc_otros.ToString();

                            if (otros.linea == "concepto-extra")
                            {
                                richTextBox2.Enabled = true;
                                richTextBox2.ReadOnly = false;
                            }

                            if (otros.largo > 0)
                            {
                                textBox39.Enabled = true;
                                textBox39.Text = otros.largo.ToString();
                            }

                            if (otros.alto > 0)
                            {
                                textBox37.Enabled = true;
                                textBox37.Text = otros.alto.ToString();
                            }

                            //Calcular
                            calculoOtros();
                        }
                        break;
                    case 7:
                        disableTabPage();
                        ((Control)tabPage5).Enabled = true;
                        tabControl1.SelectedTab = tabPage5;
                        hScrollBar1.Maximum = panel1.Width;

                        var modulos = (from x in listas.modulos where x.id == id_tem select x).SingleOrDefault();

                        if (modulos != null)
                        {
                            label99.Text = comboBox1.Text;
                            label90.Text = modulos.clave;
                            label92.Text = modulos.articulo;
                            label94.Text = modulos.linea;
                            richTextBox3.Text = modulos.descripcion;
                            opened_module = id_tem;
                            if (modulos.linea == "CANCEL BAÑO")
                            {
                                constants.setImage("series", "CB", "png", pictureBox10);
                            }
                            else
                            {
                                constants.setImage("series", modulos.linea, "png", pictureBox10);
                            }
                            if (modulos.privado == true)
                            {
                                pictureBox11.Image = Properties.Resources.Lock_Lock_icon;
                            }
                            else
                            {
                                pictureBox11.Image = null;
                            }
                            var diseño = (from x in listas.esquemas where x.id == modulos.id_diseño select x).SingleOrDefault();

                            if (diseño != null)
                            {
                                tableLayoutPanel1.Controls.Clear();
                                tableLayoutPanel1.RowCount = (int)diseño.filas;
                                tableLayoutPanel1.ColumnCount = (int)diseño.columnas;
                                getEsquemasFromDiseño(diseño.esquemas);
                                foreach (string e in esquemas)
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
                                }
                                if (diseño.marco == true)
                                {
                                    if (diseño.grupo == "puerta")
                                    {
                                        tableLayoutPanel1.Padding = new Padding(10, 10, 10, 0);
                                        if (Size.Width > 1060)
                                        {
                                            tableLayoutPanel1.Width = hScrollBar1.Maximum / 2;
                                            hScrollBar1.Value = hScrollBar1.Maximum / 2;
                                        }
                                        resize = true;
                                    }
                                    else
                                    {
                                        if (diseño.columnas > 2)
                                        {
                                            tableLayoutPanel1.Padding = new Padding(10, 10, 10, 10);
                                            tableLayoutPanel1.Width = hScrollBar1.Maximum;
                                            hScrollBar1.Value = hScrollBar1.Maximum;
                                            resize = false;
                                        }
                                        else
                                        {
                                            tableLayoutPanel1.Padding = new Padding(10, 10, 10, 10);
                                            if (Size.Width > 1060)
                                            {
                                                tableLayoutPanel1.Width = hScrollBar1.Maximum / 2;
                                                hScrollBar1.Value = hScrollBar1.Maximum / 2;
                                            }
                                            resize = true;
                                        }
                                    }
                                }
                                else
                                {
                                    tableLayoutPanel1.Padding = new Padding(0, 0, 0, 0);
                                    if (diseño.columnas > 2)
                                    {
                                        tableLayoutPanel1.Width = hScrollBar1.Maximum;
                                        hScrollBar1.Value = hScrollBar1.Maximum;
                                        resize = false;
                                    }
                                    else
                                    {
                                        if (Size.Width > 1060)
                                        {
                                            tableLayoutPanel1.Width = hScrollBar1.Maximum / 2;
                                            hScrollBar1.Value = hScrollBar1.Maximum / 2;
                                        }
                                        resize = true;
                                    }
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
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception) { }
        }
        //ends display de imagen de crital y resets
        //ends Cristales --------------------------------------------------------------------------------------------------------------

        //Aluminio --------------------------------------------------------------------------------------------------------------------
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex >= 0)
            {
                comboBox4.SelectedIndex = -1;
                calculoAluminio();
            }
        }

        //largo
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox6.Text) == true)
            {               
                calculoAluminio();
            }
            else
            {
                textBox6.Text = "";
                label38.Text = p.ToString("0.00");
                label104.Text = p.ToString("0.00");
            }
        }

        //tramos
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox8.Text) == true)
            {
                textBox6.Text = (Math.Round(constants.stringToFloat(label111.Text) * constants.stringToFloat(textBox8.Text), 2) * 1000).ToString();
            }
            else
            {
                textBox6.Text = "";
                textBox8.Text = "";
                label38.Text = p.ToString("0.00");
                label104.Text = p.ToString("0.00");
            }
        }

        //cantidad
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox7.Text) == true)
            {
                calculoAluminio();
            }
            else
            {
                textBox7.Text = "";
                label38.Text = p.ToString("0.00");
                label104.Text = p.ToString("0.00");
            }
        }

        //descuento
        private void textBox27_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox27.Text) == true)
            {
                calculoAluminio();
            }
            else
            {
                textBox27.Text = "";
                label38.Text = p.ToString("0.00");
                label104.Text = p.ToString("0.00");
            }
        }

        //calculo de aluminio
        private void calculoAluminio()
        {
            listas_entities_pva lista = new listas_entities_pva();
            var perfiles = (from x in listas.perfiles where x.id == id_tem select x).SingleOrDefault();
            float largo_original = 1;
            float ext = 0;
            float pf = 0;

            if (perfiles != null)
            {
                if(perfiles.largo > 1)
                {
                    largo_original = (float)perfiles.largo;
                }
                float largo = stringToFloat(textBox6.Text) / 1000;
                float cant = stringToFloat(textBox7.Text);
                float desc = stringToFloat(textBox27.Text) / 100;
                if (comboBox3.Text != "")
                {
                    switch (comboBox3.Text)
                    {
                        case "crudo":
                            pf = (float)perfiles.crudo;
                            label34.Text = Math.Round((double)pf / largo_original, 2).ToString("0.00");
                            label38.Text = Math.Round(((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc)) * constants.iva, 2).ToString("0.00");
                            label104.Text = Math.Round((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc), 2).ToString("0.00");
                            break;
                        case "blanco":
                            pf = (float)perfiles.blanco;
                            label34.Text = Math.Round((double)pf / largo_original, 2).ToString("0.00");
                            label38.Text = Math.Round(((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc)) * constants.iva, 2).ToString("0.00");
                            label104.Text = Math.Round((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc), 2).ToString("0.00");
                            break;
                        case "hueso":
                            pf = (float)perfiles.hueso;
                            label34.Text = (Math.Round((double)pf / largo_original, 2)).ToString("0.00");
                            label38.Text = Math.Round(((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc)) * constants.iva, 2).ToString("0.00");
                            label104.Text = Math.Round((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc), 2).ToString("0.00");
                            break;
                        case "champagne":
                            pf = (float)perfiles.champagne;
                            label34.Text = (Math.Round((double)pf / largo_original, 2)).ToString("0.00");
                            label38.Text = Math.Round(((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc)) * constants.iva, 2).ToString("0.00");
                            label104.Text = Math.Round((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc), 2).ToString("0.00");
                            break;
                        case "gris":
                            pf = (float)perfiles.gris;
                            label34.Text = (Math.Round((double)pf / largo_original, 2)).ToString("0.00");
                            label38.Text = Math.Round(((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc)) * constants.iva, 2).ToString("0.00");
                            label104.Text = Math.Round((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc), 2).ToString("0.00");
                            break;
                        case "negro":
                            pf = (float)perfiles.negro;
                            label34.Text = (Math.Round((double)pf / largo_original, 2)).ToString("0.00");
                            label38.Text = Math.Round(((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc)) * constants.iva, 2).ToString("0.00");
                            label104.Text = Math.Round((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc), 2).ToString("0.00");
                            break;
                        case "brillante":
                            pf = (float)perfiles.brillante;
                            label34.Text = (Math.Round((double)pf / largo_original, 2)).ToString("0.00");
                            label38.Text = Math.Round(((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc)) * constants.iva, 2).ToString("0.00");
                            label104.Text = Math.Round((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc), 2).ToString("0.00");
                            break;
                        case "natural":
                            pf = (float)perfiles.natural_1;
                            label34.Text = (Math.Round((double)pf / largo_original, 2)).ToString("0.00");
                            label38.Text = Math.Round(((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc)) * constants.iva, 2).ToString("0.00");
                            label104.Text = Math.Round((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc), 2).ToString("0.00");
                            break;
                        case "madera":
                            pf = (float)perfiles.madera;
                            label34.Text = (Math.Round((double)pf / largo_original, 2)).ToString("0.00");
                            label38.Text = Math.Round(((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc)) * constants.iva, 2).ToString("0.00");
                            label104.Text = Math.Round((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc), 2).ToString("0.00");
                            break;
                        case "chocolate":
                            pf = (float)perfiles.chocolate;
                            label34.Text = (Math.Round((double)pf / largo_original, 2)).ToString("0.00");
                            label38.Text = Math.Round(((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc)) * constants.iva, 2).ToString("0.00");
                            label104.Text = Math.Round((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc), 2).ToString("0.00");
                            break;
                        case "acero_inox":
                            pf = (float)perfiles.acero_inox;
                            label34.Text = (Math.Round((double)pf / largo_original, 2)).ToString("0.00");
                            label38.Text = Math.Round(((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc)) * constants.iva, 2).ToString("0.00");
                            label104.Text = Math.Round((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc), 2).ToString("0.00");
                            break;
                        case "bronce":
                            pf = (float)perfiles.bronce;
                            label34.Text = (Math.Round((double)pf / largo_original, 2)).ToString("0.00");
                            label38.Text = Math.Round(((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc)) * constants.iva, 2).ToString("0.00");
                            label104.Text = Math.Round((((double)pf / largo_original) * largo * cant) + ((((double)pf / largo_original) * largo * cant) * desc), 2).ToString("0.00");
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if(comboBox4.Text != "")
                    {
                        string c = comboBox4.Text;
                        var color = (from x in listas.colores_aluminio where x.clave == c select x).SingleOrDefault();

                        if (color != null)
                        {
                            pf = (float)perfiles.crudo;
                            ext = (float)(largo_original * color.costo_extra_ml);
                            label34.Text = Math.Round((double)((pf / largo_original) + ((((perfiles.perimetro_dm2_ml/100) * color.precio) + ext) * constants.iva)), 2).ToString("0.00");
                            ext = (float)(largo * color.costo_extra_ml);
                            label38.Text = Math.Round((double)((((pf / largo_original) * largo * cant) + (((largo * (perfiles.perimetro_dm2_ml/100) * (color.precio)) + ext) * constants.iva * cant)) + ((((pf / largo_original) * largo * cant) + (largo * (perfiles.perimetro_dm2_ml/100) * (color.precio) * constants.iva * cant)) * desc)) * constants.iva, 2).ToString("0.00");
                            label104.Text = Math.Round((double)((((pf / largo_original) * largo * cant) + (((largo * (perfiles.perimetro_dm2_ml / 100) * (color.precio)) + ext) * constants.iva * cant)) + ((((pf / largo_original) * largo * cant) + (largo * (perfiles.perimetro_dm2_ml / 100) * (color.precio) * constants.iva * cant)) * desc)), 2).ToString("0.00");
                        }
                    }
                }
            }
        }
        //ends Aluminio ----------------------------------------------------------------------------------------------------------------------------

        //Herrajes y Otros -------------------------------------------------------------------------------------------------------------------------

        private void calculoHerrajes()
        {
            listas_entities_pva listas = new listas_entities_pva();
            var herraje = (from x in listas.herrajes where x.id == id_tem select x).SingleOrDefault();

            if(herraje != null)
            {              
                label63.Text = Math.Round(((stringToFloat(textBox33.Text) * (float)(herraje.precio)) + ((stringToFloat(textBox33.Text) * (float)(herraje.precio)) * (stringToFloat(textBox29.Text) / 100))) * constants.iva, 2).ToString("0.00");
                label43.Text = Math.Round((stringToFloat(textBox33.Text) * (float)(herraje.precio)) + ((stringToFloat(textBox33.Text) * (float)(herraje.precio)) * (stringToFloat(textBox29.Text) / 100)), 2).ToString("0.00");
            }
        }

        //numero herrajes
        private void textBox33_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox33.Text) == true)
            {
                calculoHerrajes();
            }
            else
            {
                textBox33.Text = "";
                label63.Text = p.ToString("0.00");
                label43.Text = p.ToString("0.00");
            }
        }

        //descuento herrajes
        private void textBox29_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox29.Text) == true)
            {
                calculoHerrajes();
            }
            else
            {
                textBox29.Text = "";
                label63.Text = p.ToString("0.00");
                label43.Text = p.ToString("0.00");
            }
        }

        private void calculoOtros()
        {
            listas_entities_pva listas = new listas_entities_pva();
            var costo = (from x in listas.otros where x.id == id_tem select x.precio).SingleOrDefault();

            if (costo != null)
            {
                label74.Text = Math.Round(((stringToFloat(textBox30.Text) * (float)(costo) * (stringToFloat(textBox39.Text) / 1000) * (stringToFloat(textBox37.Text) / 1000)) + ((stringToFloat(textBox30.Text) * (float)(costo) * (stringToFloat(textBox39.Text) / 1000)) * (stringToFloat(textBox37.Text) / 1000) * (stringToFloat(textBox31.Text) / 100))) * constants.iva, 2).ToString("0.00");
                label106.Text = Math.Round((stringToFloat(textBox30.Text) * (float)(costo) * (stringToFloat(textBox39.Text) / 1000) * (stringToFloat(textBox37.Text) / 1000)) + ((stringToFloat(textBox30.Text) * (float)(costo) * (stringToFloat(textBox39.Text) / 1000)) * (stringToFloat(textBox37.Text) / 1000) * (stringToFloat(textBox31.Text) / 100)), 2).ToString("0.00");
            }
        }

        //descuento otros
        private void textBox31_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox31.Text) == true)
            {
                calculoOtros();
            }
            else
            {
                textBox31.Text = "";
                label74.Text = p.ToString("0.00");
                label106.Text = p.ToString("0.00");
            }
        }

        //numero otros
        private void textBox30_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox30.Text) == true)
            {
                calculoOtros();
            }
            else
            {
                textBox30.Text = "";
                label74.Text = p.ToString("0.00");
                label106.Text = p.ToString("0.00");
            }
        }

        //largo otros
        private void textBox39_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox39.Text) == true)
            {
                calculoOtros();
            }
            else
            {
                textBox39.Text = "";
                label74.Text = p.ToString("0.00");
                label106.Text = p.ToString("0.00");
            }
        }

        //ALTO OTROS
        private void textBox37_TextChanged(object sender, EventArgs e)
        {
            if (constants.isInteger(textBox37.Text) == true)
            {
                calculoOtros();
            }
            else
            {
                textBox37.Text = "";
                label74.Text = p.ToString("0.00");
                label106.Text = p.ToString("0.00");
            }
        }       

        //Agregar nueva cotizacion -----------------------------------------------------------------------------------------------------------------

        //Calcular total y subtotal --------------------->
        public void calcularTotalesCotizacion()
        {
            float sum = 0;
            cotizaciones_local cotizaciones = new cotizaciones_local();
            try {
                // -----------------------------------------
                var total_cristales = (from x in cotizaciones.cristales_cotizados where x.total > 0 select x).ToArray();
                for (int i = 0; i < total_cristales.Length; i++)
                {
                    sum = sum + (float)total_cristales[i].total;
                }
                var total_aluminio = (from x in cotizaciones.aluminio_cotizado where x.total > 0 select x).ToArray();
                for (int i = 0; i < total_aluminio.Length; i++)
                {
                    sum = sum + (float)total_aluminio[i].total;
                }
                var total_herrajes = (from x in cotizaciones.herrajes_cotizados where x.total > 0 select x).ToArray();
                for (int i = 0; i < total_herrajes.Length; i++)
                {
                    sum = sum + (float)total_herrajes[i].total;
                }
                var total_otros = (from x in cotizaciones.otros_cotizaciones where x.total > 0 select x).ToArray();
                for (int i = 0; i < total_otros.Length; i++)
                {
                    sum = sum + (float)total_otros[i].total;
                }
                var total_modulos = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == -1 && x.sub_folio == constants.sub_folio && x.total > 0 select x).ToArray();
                for (int i = 0; i < total_modulos.Length; i++)
                {
                    sum = sum + (float)total_modulos[i].total;
                }
                // -----------------------------------------
                //descuentos --->
                constants.desc_cant = (float)Math.Round((sum * (stringToFloat(textBox4.Text) / 100)), 2);
                sum = sum - constants.desc_cant;
                sum = sum + (float)Math.Round((sum * (stringToFloat(textBox28.Text) / 100)), 2);
                //
                label18.Text = Math.Round(sum, 2).ToString();
                label19.Text = Math.Round(this.enable_iva == true ? stringToFloat(label18.Text) * (constants.iva - 1) : 0, 2).ToString();
                label20.Text = Math.Round(stringToFloat(label18.Text) + stringToFloat(label19.Text), 2).ToString("n");
            } catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }
        //

        //Texbox descuento cotizacion
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (constants.isFloat(textBox4.Text) == true)
            {
                constants.desc_cotizacion = constants.stringToFloat(textBox4.Text);
            }
            else
            {
                constants.desc_cotizacion = 0;
                textBox4.Text = "";
            }
            calcularTotalesCotizacion();
        }

        //Texbox utilidad cotizacion
        private void textBox28_TextChanged(object sender, EventArgs e)
        {
            if (constants.isFloat(textBox28.Text) == true)
            {
                constants.utilidad_cotizacion = constants.stringToFloat(textBox28.Text);
            }
            else
            {
                constants.utilidad_cotizacion = 0;
                textBox28.Text = "";
            }
            calcularTotalesCotizacion();
        }
        //

        //Folio  --------------
        public void setFolioLabel()
        {           
            label22.Text = constants.folio_abierto.ToString();
            textBox4.Text = constants.desc_cotizacion.ToString();
            textBox28.Text = constants.utilidad_cotizacion.ToString();
            toolStripStatusLabel3.Text = "     [Cliente: " + constants.nombre_cotizacion + "]   [Proyecto: " + constants.nombre_proyecto + "]";
            toolStripStatusLabel3.Text = toolStripStatusLabel3.Text.ToUpper();
            toolStripStatusLabel3.ForeColor = System.Drawing.Color.Blue;
            checkBox5.Checked = constants.iva_desglosado;
            if (checkBox5.Checked)
            {
                setModoLIVA();
            }
        }
        //

        //Boton guardar cotizacion
        private void button4_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                if (sumCounter() > 0)
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
        //----

        //Boton de nuevo
        private void button8_Click(object sender, EventArgs e)
        {
            if (sumCounter() > 0 || constants.nombre_cotizacion != "")
            {
                if (constants.cotizacion_guardada == false && constants.local == false)
                {
                    DialogResult r = MessageBox.Show("Existe una operación en curso.\n\n ¿Desea guardarla?", constants.msg_box_caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (r == DialogResult.Yes)
                    {
                        new guardar_cotizacion().ShowDialog();
                    }
                    else if (r == DialogResult.No)
                    {
                        borrarCotizacion();
                    }
                    else if (r == DialogResult.Cancel)
                    {
                        //do nothing
                    }
                }
                else
                {
                    borrarCotizacion();
                }
            }                      
        }
        //----

        //Boton de buscar
        private void button5_Click(object sender, EventArgs e)
        {
            if(constants.local == false)
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
        //----

        //Boton cancelar cotizacion
        private void button2_Click(object sender, EventArgs e)
        {
            if (sumCounter() > 0 || constants.nombre_cotizacion != "")
            {
                DialogResult r = MessageBox.Show("¿Estas seguro de cerrar está cotización?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    borrarCotizacion();
                }
            }
        }

        public void borrarCotizacion()
        {
            if (backgroundWorker4.IsBusy == false)
            {
                //Cerrar ventanas
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
                label100.Text = "[Cerrando Cotización...]";
                backgroundWorker4.RunWorkerAsync();
            }
        }

        public void resetDatagridCotizaciones()
        {
            datagridviewNE1.HorizontalScrollingOffset = 0;
            datagridviewNE1.DataSource = null;
            datagridviewNE1.Refresh();
        }
        //----        

        private void datagridviewNE1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                datagridviewNE1.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                datagridviewNE1.EndEdit();
            }
        }

        private void datagridviewNE1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                datagridviewNE1.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                datagridviewNE1.BeginEdit(true);              
            }
        }

        //eliminar articulo del grid
        public void eliminarArticuloCotizado(int id, bool noDialog=false)
        {
            DialogResult r;

            if (noDialog == false)
            {
                r = MessageBox.Show("¿Estás seguro de eliminar esté artículo?", constants.msg_box_caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
            else
            {
                r = DialogResult.OK;
            }

            if (r == DialogResult.OK)
            {
                cotizaciones_local cotizaciones = new cotizaciones_local();
                DataTable table = new DataTable();
                try
                {
                    if (constants.tipo_cotizacion == 1)
                    {
                        var cristales = (from x in cotizaciones.cristales_cotizados where x.id == id select x).SingleOrDefault();
                        if (cristales != null)
                        {
                            cotizaciones.cristales_cotizados.Remove(cristales);
                            cotizaciones.SaveChanges();
                            if (constants.count_cristales > 0)
                            {
                                constants.count_cristales = constants.count_cristales - 1;
                            }
                            if (cristales.folio > 0)
                            {
                                constants.setFilaBorradaOnLocalDB(1, (int)cristales.folio, constants.getIDFromClave(cristales.clave));
                            }
                            if (constants.id_articulo_cotizacion == cristales.id)
                            {
                                constants.id_articulo_cotizacion = -1;
                                setEditImage(false, false);
                            }
                            constants.loadCotizacionesLocales("cristales", datagridviewNE1);
                        }
                    }
                    else if (constants.tipo_cotizacion == 2)
                    {
                        var aluminio = (from x in cotizaciones.aluminio_cotizado where x.id == id select x).SingleOrDefault();
                        if (aluminio != null)
                        {
                            cotizaciones.aluminio_cotizado.Remove(aluminio);
                            cotizaciones.SaveChanges();
                            if (constants.count_aluminio > 0)
                            {
                                constants.count_aluminio = constants.count_aluminio - 1;
                            }
                            if (aluminio.folio > 0)
                            {
                                constants.setFilaBorradaOnLocalDB(2, (int)aluminio.folio, constants.getIDFromClave(aluminio.clave));
                            }
                            if (constants.id_articulo_cotizacion == aluminio.id)
                            {
                                constants.id_articulo_cotizacion = -1;
                                setEditImage(false, false);
                            }
                            constants.loadCotizacionesLocales("aluminio", datagridviewNE1);
                        }
                    }
                    else if (constants.tipo_cotizacion == 3)
                    {
                        var herrajes = (from x in cotizaciones.herrajes_cotizados where x.id == id select x).SingleOrDefault();
                        if (herrajes != null)
                        {
                            cotizaciones.herrajes_cotizados.Remove(herrajes);
                            cotizaciones.SaveChanges();
                            if (constants.count_herrajes > 0)
                            {
                                constants.count_herrajes = constants.count_herrajes - 1;
                            }
                            if (herrajes.folio > 0)
                            {
                                constants.setFilaBorradaOnLocalDB(3, (int)herrajes.folio, constants.getIDFromClave(herrajes.clave));
                            }
                            if (constants.id_articulo_cotizacion == herrajes.id)
                            {
                                constants.id_articulo_cotizacion = -1;
                                setEditImage(false, false);
                            }
                            constants.loadCotizacionesLocales("herrajes", datagridviewNE1);
                        }
                    }
                    else if (constants.tipo_cotizacion == 4)
                    {
                        var otros = (from x in cotizaciones.otros_cotizaciones where x.id == id select x).SingleOrDefault();
                        if (otros != null)
                        {
                            cotizaciones.otros_cotizaciones.Remove(otros);
                            cotizaciones.SaveChanges();
                            if (constants.count_otros > 0)
                            {
                                constants.count_otros = constants.count_otros - 1;
                            }
                            if (otros.folio > 0)
                            {
                                constants.setFilaBorradaOnLocalDB(4, (int)otros.folio, constants.getIDFromClave(otros.clave));
                            }
                            if (constants.id_articulo_cotizacion == otros.id)
                            {
                                constants.id_articulo_cotizacion = -1;
                                setEditImage(false, false);
                            }
                            constants.loadCotizacionesLocales("otros", datagridviewNE1);
                        }
                    }
                    else if (constants.tipo_cotizacion == 5)
                    {
                        var modulos = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();
                        if (modulos != null)
                        {
                            cotizaciones.modulos_cotizaciones.Remove(modulos);
                            cotizaciones.SaveChanges();
                            if (constants.count_modulos > 0)
                            {
                                constants.count_modulos = constants.count_modulos - 1;
                            }
                            if (modulos.folio > 0)
                            {
                                constants.setFilaBorradaOnLocalDB(5, (int)modulos.folio, constants.getIDFromClave(modulos.clave));
                            }
                            if (constants.id_articulo_cotizacion == modulos.id)
                            {
                                constants.id_articulo_cotizacion = -1;
                                setEditImage(false, false);
                            }
                            if (modulos.concept_id <= 0)
                            {
                                if (modulos.merge_id > 0)
                                {
                                    constants.updateModuloPersonalizado((int)modulos.merge_id);
                                }
                            }
                            else
                            {
                                DialogResult re = MessageBox.Show("¿Deseas borrar todos los artículos añadidos?.", constants.msg_box_caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                                int concept = (int)modulos.concept_id;
                                var n = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == concept select x);

                                if (re == DialogResult.Yes)
                                {
                                    foreach (var g in n)
                                    {
                                        if (g != null)
                                        {
                                            eliminarArticuloCotizado(g.id, true);
                                        }
                                    }
                                }
                                else
                                {                                     
                                    foreach (var g in n)
                                    {
                                        if (g != null)
                                        {
                                            g.merge_id = -1;
                                        }
                                    }                                    
                                }
                                cotizaciones.SaveChanges();
                            }
                            constants.loadCotizacionesLocales("modulos", datagridviewNE1);
                        }
                    }
                    countCotizacionesArticulo();
                    loadCountArticulos();
                    calcularTotalesCotizacion();
                    constants.cotizacion_guardada = false;
                    if (Application.OpenForms["articulos_cotizacion"] != null)
                    {
                        ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL();
                    }
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                }
            }
        }

        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            eliminarArticuloCotizado((int)datagridviewNE1.CurrentRow.Cells[0].Value);
        }
        //------------------------------------------------------------------

        //Grid de articulos (pequeño)      

        //carga los articulos de la db local y los cuenta
        public void loadCountArticulos()
        {
            treeView1.BeginUpdate();
            treeView1.Nodes[0].Nodes[0].Text = "Cristales" + " (" + constants.count_cristales + ")";
            treeView1.Nodes[0].Nodes[1].Text = "Aluminio" + " (" + constants.count_aluminio + ")";
            treeView1.Nodes[0].Nodes[2].Text = "Herrajes" + " (" + constants.count_herrajes + ")";
            treeView1.Nodes[0].Nodes[3].Text = "Otros" + " (" + constants.count_otros + ")";
            treeView1.Nodes[0].Nodes[4].Text = "Modulos" + " (" + constants.count_modulos + ")";
            treeView1.EndUpdate();
            treeView1.Nodes[0].Expand();
        }
        //

        public void resetCountArticulos()
        {
            constants.count_cristales = 0;
            constants.count_aluminio = 0;
            constants.count_herrajes = 0;
            constants.count_otros = 0;
            constants.count_modulos = 0;
            countCotizacionesArticulo();
            loadCountArticulos();
        }

        public int sumCounter()
        {
            return constants.count_cristales + constants.count_aluminio + constants.count_herrajes + constants.count_otros + constants.count_modulos;
        }
        //      

        //Arbol de articulos de contadores
        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            constants.id_articulo_cotizacion = -1;
            setEditImage(false, false);
            switch (e.Node.Index)
            {
                case 0:
                    resetDatagridCotizaciones();
                    constants.loadCotizacionesLocales("cristales", datagridviewNE1);
                    constants.tipo_cotizacion = 1;                  
                    break;
                case 1:
                    resetDatagridCotizaciones();
                    constants.loadCotizacionesLocales("aluminio", datagridviewNE1);
                    constants.tipo_cotizacion = 2;
                    break;
                case 2:
                    resetDatagridCotizaciones();
                    constants.loadCotizacionesLocales("herrajes", datagridviewNE1);
                    constants.tipo_cotizacion = 3;
                    break;
                case 3:
                    resetDatagridCotizaciones();
                    constants.loadCotizacionesLocales("otros", datagridviewNE1);
                    constants.tipo_cotizacion = 4;
                    break;
                case 4:
                    resetDatagridCotizaciones();
                    constants.loadCotizacionesLocales("modulos", datagridviewNE1);
                    constants.tipo_cotizacion = 5;
                    break;
                default:
                    break;
            }          
        }
        //        

        public void countCotizacionesArticulo()
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            try
            {
                var c_cristales = (from x in cotizaciones.cristales_cotizados select x).Count();
                constants.count_cristales = c_cristales;

                var c_aluminio = (from x in cotizaciones.aluminio_cotizado select x).Count();
                constants.count_aluminio = c_aluminio;

                var c_herrajes = (from x in cotizaciones.herrajes_cotizados select x).Count();
                constants.count_herrajes = c_herrajes;

                var c_otros = (from x in cotizaciones.otros_cotizaciones select x).Count();
                constants.count_otros = c_otros;   
                
                var c_modulos = (from x in cotizaciones.modulos_cotizaciones where x.merge_id <= 0 && x.sub_folio == constants.sub_folio select x).Count();
                constants.count_modulos = c_modulos;      
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }
        //

        private string getTipo_Venta()
        {
            if (checkBox1.Visible == true)
            {
                if (checkBox1.Checked == true)
                {
                    return "Metro Cuadrado";
                }
                else
                {
                    return "Instalado";
                }
            }
            else
            {
                return "Hoja";
            }
        }

        //crea cotizacion local de cristal
        public void crearCotizacionCristales(string clave,
            string articulo,
            string lista,
            string proveedor,
            float largo,
            float alto,
            float cantidad,
            string tipo_v,
            float descuento,
            float total,
            string canteado = "",
            string biselado = "",
            string desconchado = "",
            string pechoP = "",
            string perfo_media = "",
            string perfo_una = "",
            string perfo_dos = "",
            string grabado = "",
            string esmerilado = "",
            float filo_muerto = 0)
        {
            cotizaciones_local cotizacion = new cotizaciones_local();
            cristales_cotizados cristales = new cristales_cotizados
            {
                folio = 00000,
                clave = clave,
                articulo = articulo,
                lista = lista,
                proveedor = proveedor,
                largo = Math.Round(largo, 2),
                alto = Math.Round(alto, 2),
                canteado = canteado,
                biselado = biselado,
                desconchado = desconchado,
                pecho_paloma = pechoP,
                perforado_media_pulgada = perfo_media,
                perforado_una_pulgada = perfo_una,
                perforado_dos_pulgadas = perfo_dos,
                grabado = grabado,
                esmerilado = esmerilado,
                filo_muerto = filo_muerto,
                cantidad = Math.Round(cantidad, 2),
                tipo_venta = tipo_v,
                descuento = Math.Round(descuento, 2),
                total = Math.Round(total, 2),
                descripcion = this.descripcion
            };
            cotizacion.cristales_cotizados.Add(cristales);
            cotizacion.SaveChanges();
        }

        private string refreshClave(string clave)
        {
            string r = string.Empty;
            foreach(char x in clave)
            {
                if(x == '#' || x == '-')
                {
                    break;
                }
                r = r + x;
            }
            return r;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            constants.id_articulo_cotizacion = -1;
            setEditImage(false, false);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                if (Application.OpenForms["crear_modulo"] == null)
                {
                    if (Application.OpenForms["config_modulo"] == null)
                    {
                        if (constants.user_access >= 5)
                        {
                            if (backgroundWorker3.IsBusy == false)
                            {
                                modulos_config = false;
                                label97.Text = "Cargando...";
                                pictureBox9.Image = Properties.Resources.lg_dual_gear_loading_icon;
                                backgroundWorker3.RunWorkerAsync();
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] solo un usuario con privilegios de grado (4) puede acceder a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] no es posible ingresar a esta característica mientras se configura un módulo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Application.OpenForms["crear_modulo"].Select();
                    if (constants.maximizar_ventanas == true)
                    {
                        Application.OpenForms["crear_modulo"].WindowState = FormWindowState.Maximized;
                    }
                    else
                    {
                        Application.OpenForms["crear_modulo"].WindowState = FormWindowState.Normal;
                    }
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            abrirConfigModulo(); 
        }

        //abrir configuracion de modulo
        public void abrirConfigModulo()
        {
            if (backgroundWorker3.IsBusy == false)
            {
                modulos_config = true;
                label97.Text = "Cargando...";
                pictureBox9.Image = Properties.Resources.lg_dual_gear_loading_icon;
                backgroundWorker3.RunWorkerAsync();
            }
        }

        //Crea cotizacion local de aluminio
        public void crearCotizacionAluminio(string clave,
                string articulo,
                string lista,
                string proveedor,
                string linea,
                float largo_total,
                string acabado,
                float cantidad,
                float descuento,
                float total)
        {
            cotizaciones_local cotizacion = new cotizaciones_local();
            aluminio_cotizado aluminio = new aluminio_cotizado
            {
                folio = 00000,
                clave = clave,
                articulo = articulo,
                lista = lista,
                proveedor = proveedor,
                linea = linea,
                largo_total = largo_total,
                acabado = acabado,
                cantidad = Math.Round(cantidad, 2),
                descuento = Math.Round(descuento, 2),
                total = Math.Round(total, 2),
                descripcion = this.descripcion
            };
            cotizacion.aluminio_cotizado.Add(aluminio);
            cotizacion.SaveChanges();
        }    

        //Crea cotizacion local de herrajes
        public void crearCotizacionHerrajes(string clave,
                string articulo,
                string proveedor,
                string linea,
                string caracteristicas,
                string color,
                float cantidad,
                float descuento,
                float total)
        {
            cotizaciones_local cotizacion = new cotizaciones_local();
            herrajes_cotizados herrajes = new herrajes_cotizados
            {
                folio = 00000,
                clave = clave,
                articulo = articulo,
                proveedor = proveedor,
                linea = linea,
                caracteristicas = caracteristicas,
                color = color,
                cantidad = Math.Round(cantidad, 2),
                descuento = Math.Round(descuento, 2),
                total = Math.Round(total, 2),
                descripcion = this.descripcion
            };
            cotizacion.herrajes_cotizados.Add(herrajes);
            cotizacion.SaveChanges();
        }

        //Crea cotizacion local de otros
        public void crearCotizacioOtros(string clave,
                string articulo,
                string proveedor,
                string linea,
                string caracteristicas,
                string color,
                float cantidad,
                float descuento,
                float largo,
                float alto,
                float total)
        {
            cotizaciones_local cotizacion = new cotizaciones_local();
            otros_cotizaciones otros = new otros_cotizaciones
            {
                folio = 00000,
                clave = clave,
                articulo = articulo,
                proveedor = proveedor,
                linea = linea,
                caracteristicas = caracteristicas,
                color = color,
                cantidad = Math.Round(cantidad, 2),
                descuento = Math.Round(descuento, 2),
                largo = Math.Round(largo, 2),
                alto = Math.Round(alto, 2),
                total = Math.Round(total, 2),
                descripcion = this.descripcion
            };
            cotizacion.otros_cotizaciones.Add(otros);
            cotizacion.SaveChanges();
        }

        //boton agregar en cristales
        private void button1_Click(object sender, EventArgs e)
        {
            if (label9.Text != "")
            {
                if (constants.tipo_cotizacion == 1 && constants.getArticuloIdLocalDB(1, constants.id_articulo_cotizacion) == true)
                {
                    DialogResult r = MessageBox.Show("Esté artículo ya esta incluido. ¿Desea guardar los cambios?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (r == DialogResult.Yes)
                    {
                        updateArticuloIdLocalDB(1, constants.id_articulo_cotizacion);
                        setEditImage(false, false);
                        constants.id_articulo_cotizacion = -1;
                    }
                }
                else
                {                 
                    crearCotizacionCristales(label9.Text, label8.Text, label7.Text, label85.Text, stringToFloat(textBox2.Text), stringToFloat(textBox3.Text), stringToFloat(textBox5.Text),
                    getTipo_Venta(), stringToFloat(textBox26.Text), stringToFloat(label41.Text), getAcabadoFromRow("CP"), getAcabadoFromRow("BR"), getAcabadoFromRow("DESC"),
                    getAcabadoFromRow("PP"), getAcabadoFromRow("PERMEDIA"), getAcabadoFromRow("PERUNA"), getAcabadoFromRow("PERDOS"), getAcabadoFromRow("GRB"), getAcabadoFromRow("ESM"), checkBox4.Checked == true ? constants.stringToFloat(textBox10.Text) : 0);
                    constants.tipo_cotizacion = 1;
                    refreshNewArticulo();
                    constants.count_cristales++;
                    loadCountArticulos();
                }
                this.descripcion = string.Empty;
                calcularTotalesCotizacion();
                constants.cotizacion_proceso = true;
                constants.cotizacion_guardada = false;
            }
            else
            {
                MessageBox.Show("[Error] no existe artículo que agregar.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        //Generar Reporte
        private void generarReporteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["load_report"] == null && Application.OpenForms["reportes"] == null)
            {
                new load_report(label18.Text, label19.Text, label20.Text).Show();
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

        public string[] getDesglose()
        {
            string[] r = new string[3];
            r[0] = label18.Text;
            r[1] = label19.Text;
            r[2] = label20.Text;
            return r;
        }
        //

        //boton agregar en aluminios
        private void button7_Click(object sender, EventArgs e)
        {
            if (label29.Text != "")
            {
                if (comboBox3.Text != "" || comboBox4.Text != "")
                {
                    if (constants.tipo_cotizacion == 2 && constants.getArticuloIdLocalDB(2, constants.id_articulo_cotizacion) == true)
                    {
                        DialogResult r = MessageBox.Show("Esté artículo ya esta incluido. ¿Desea guardar los cambios?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (r == DialogResult.Yes)
                        {
                            updateArticuloIdLocalDB(2, constants.id_articulo_cotizacion);
                            setEditImage(false, false);
                            constants.id_articulo_cotizacion = -1;
                        }
                    }
                    else
                    {
                        crearCotizacionAluminio(label29.Text + "#" + id_tem, label30.Text, label28.Text, label87.Text, label31.Text, stringToFloat(textBox6.Text), comboBox4.Text != "" ? comboBox4.Text : comboBox3.Text, stringToFloat(textBox7.Text), stringToFloat(textBox27.Text), stringToFloat(label104.Text));
                        constants.tipo_cotizacion = 2;
                        refreshNewArticulo();
                        constants.count_aluminio++;
                        loadCountArticulos();
                    }
                    this.descripcion = string.Empty;
                    calcularTotalesCotizacion();
                    constants.cotizacion_proceso = true;
                    constants.cotizacion_guardada = false;
                }
                else
                {
                    MessageBox.Show("[Error] necesitas seleccionar un acabado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] no existe artículo que agregar.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        //boton agregar en herrajes
        private void button9_Click(object sender, EventArgs e)
        {
            if (label60.Text != "")
            {               
                if (constants.tipo_cotizacion == 3 && constants.getArticuloIdLocalDB(3, constants.id_articulo_cotizacion) == true)
                {
                    DialogResult r = MessageBox.Show("Esté artículo ya esta incluido. ¿Desea guardar los cambios?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (r == DialogResult.Yes)
                    {
                        updateArticuloIdLocalDB(3, constants.id_articulo_cotizacion);
                        setEditImage(false, false);
                        constants.id_articulo_cotizacion = -1;
                    }
                }
                else
                {
                    crearCotizacionHerrajes(label60.Text + "#" + id_tem, label62.Text, label61.Text, label59.Text, richTextBox1.Text, comboBox8.Text, stringToFloat(textBox33.Text), stringToFloat(textBox29.Text), stringToFloat(label43.Text));
                    constants.tipo_cotizacion = 3;
                    refreshNewArticulo();
                    constants.count_herrajes++;
                    loadCountArticulos();
                }
                this.descripcion = string.Empty;
                calcularTotalesCotizacion();
                constants.cotizacion_proceso = true;
                constants.cotizacion_guardada = false;                           
            }
            else
            {
                MessageBox.Show("[Error] no existe artículo que agregar.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {
            hScrollBar1.Maximum = panel1.Width;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            reloadLista();
        }
 
        public void reloadLista()
        {
            if (comboBox1.Text != "")
            {
                page = 0;
                loadListaFromLocal();
            }
        }

        //boton agregar en otros
        private void button10_Click(object sender, EventArgs e)
        {
            if (label76.Text != "")
            {                               
                if (constants.tipo_cotizacion == 4 && constants.getArticuloIdLocalDB(4, constants.id_articulo_cotizacion) == true)
                {
                    DialogResult r = MessageBox.Show("Esté artículo ya esta incluido. ¿Desea guardar los cambios?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (r == DialogResult.Yes)
                    {
                        updateArticuloIdLocalDB(4, constants.id_articulo_cotizacion);
                        setEditImage(false, false);
                        constants.id_articulo_cotizacion = -1;
                    }
                }
                else {
                    crearCotizacioOtros(label76.Text + "#" + id_tem, label77.Text, label78.Text, label79.Text, richTextBox2.Text, comboBox9.Text, stringToFloat(textBox30.Text), stringToFloat(textBox31.Text), stringToFloat(textBox39.Text), stringToFloat(textBox37.Text), stringToFloat(label106.Text));
                    constants.tipo_cotizacion = 4;
                    refreshNewArticulo();
                    constants.count_otros++;
                    loadCountArticulos();
                }
                this.descripcion = string.Empty;
                calcularTotalesCotizacion();
                constants.cotizacion_proceso = true;
                constants.cotizacion_guardada = false;                              
            }
            else
            {
                MessageBox.Show("[Error] no existe artículo que agregar.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        //boton editar cotizacion
        public void setArticuloCotizadoToEdit(int id)
        {
            try
            {
                datagridviewNE2.ClearSelection();
                constants.id_articulo_cotizacion = id;
                if (constants.tipo_cotizacion > 0)
                {
                    if (constants.save_onEdit.Contains(id + "-" + constants.tipo_cotizacion) == false)
                    {
                        constants.save_onEdit.Add(id + "-" + constants.tipo_cotizacion);
                    }
                }                          
                setArticuloToEdit(constants.tipo_cotizacion, constants.id_articulo_cotizacion);
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void editarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setArticuloCotizadoToEdit((int)datagridviewNE1.CurrentRow.Cells[0].Value);
        }
        //---------------------------------------------------------------------------------

        //Precio minimo cristales
        private void checkBox39_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {              
                calculoGeneralCristales("_m2");
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.SelectedIndex >= 0)
            {
                listas_entities_pva lista = new listas_entities_pva();
                var perfiles = (from x in listas.perfiles where x.id == id_tem select x).SingleOrDefault();

                if (perfiles != null)
                {
                    if (perfiles.perimetro_dm2_ml > 0)
                    {
                        if (perfiles.crudo > 0)
                        {
                            comboBox3.SelectedIndex = -1;
                            calculoAluminio();
                        }
                        else
                        {
                            comboBox4.SelectedIndex = -1;
                            MessageBox.Show("Esté perfil no tiene disponible un acabado en crudo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        comboBox4.SelectedIndex = -1;
                        MessageBox.Show("Esté perfil no tiene disponible un perímetro anodizable.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        public void setEditImage(Boolean visible, Boolean color)
        {
            pictureBox8.Visible = visible;
            button12.Visible = visible;       
            if (color == false)
            {
                label9.Text = refreshClave(label9.Text);
                label60.Text = refreshClave(label60.Text);
                label29.Text = refreshClave(label29.Text);
                label76.Text = refreshClave(label76.Text);
                // ---------------------------------------->
                tabPage1.Refresh();
                tabPage2.Refresh();
                tabPage3.Refresh();
                tabPage4.Refresh();
                tabPage5.Refresh();
            }
            else
            {
                tabPage1.Refresh();
                tabPage2.Refresh();
                tabPage3.Refresh();
                tabPage4.Refresh();
                tabPage5.Refresh();               
            }
        }
        //

        private void updateArticuloIdLocalDB(int tipo, int id)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            if (tipo == 1)
            {
                var c = (from x in cotizaciones.cristales_cotizados where x.id == id select x).SingleOrDefault();
                if (c != null)
                {
                    c.clave = label9.Text;
                    c.articulo = label8.Text;
                    c.lista = label7.Text;
                    c.proveedor = label85.Text;
                    c.largo = Math.Round(stringToFloat(textBox2.Text), 2);
                    c.alto = Math.Round(stringToFloat(textBox3.Text), 2);
                    c.cantidad = Math.Round(stringToFloat(textBox5.Text), 2);
                    c.tipo_venta = getTipo_Venta();
                    c.descuento = Math.Round(stringToFloat(textBox26.Text), 2);
                    c.total = Math.Round(stringToFloat(label41.Text), 2);
                    c.canteado = getAcabadoFromRow("CP");
                    c.biselado = getAcabadoFromRow("BR");
                    c.desconchado = getAcabadoFromRow("DESC");
                    c.pecho_paloma = getAcabadoFromRow("PP");
                    c.perforado_media_pulgada = getAcabadoFromRow("PERMEDIA");
                    c.perforado_una_pulgada = getAcabadoFromRow("PERUNA");
                    c.perforado_dos_pulgadas = getAcabadoFromRow("PERDOS");
                    c.grabado = getAcabadoFromRow("GRB");
                    c.esmerilado = getAcabadoFromRow("ESM");
                    c.filo_muerto = checkBox4.Checked == true ? constants.stringToFloat(textBox10.Text) : 0;
                    c.descripcion = this.descripcion;
                }
                cotizaciones.SaveChanges();
                constants.loadCotizacionesLocales("cristales", datagridviewNE1);
            }
            else if (tipo == 2)
            {
                var c = (from x in cotizaciones.aluminio_cotizado where x.id == id select x).SingleOrDefault();
                if (c != null)
                {
                    c.clave = label29.Text;
                    c.articulo = label30.Text;
                    c.lista = label28.Text;
                    c.proveedor = label87.Text;
                    c.linea = label31.Text;
                    c.largo_total = stringToFloat(textBox6.Text);
                    c.acabado = comboBox4.Text != "" ? comboBox4.Text : comboBox3.Text;
                    c.cantidad = Math.Round(stringToFloat(textBox7.Text), 2);
                    c.descuento = Math.Round(stringToFloat(textBox27.Text), 2);
                    c.total = Math.Round(stringToFloat(label104.Text), 2);
                    c.descripcion = this.descripcion;
                }
                cotizaciones.SaveChanges();
                constants.loadCotizacionesLocales("aluminio", datagridviewNE1);
            }
            else if (tipo == 3)
            {
                var c = (from x in cotizaciones.herrajes_cotizados where x.id == id select x).SingleOrDefault();
                if (c != null)
                {
                    c.clave = label60.Text;
                    c.articulo = label62.Text;
                    c.proveedor = label61.Text;
                    c.linea = label59.Text;
                    c.caracteristicas = richTextBox1.Text;
                    c.color = comboBox8.Text;
                    c.cantidad = Math.Round(stringToFloat(textBox33.Text), 2);
                    c.descuento = Math.Round(stringToFloat(textBox29.Text), 2);
                    c.total = Math.Round(stringToFloat(label43.Text), 2);
                    c.descripcion = this.descripcion;
                }
                cotizaciones.SaveChanges();
                constants.loadCotizacionesLocales("herrajes", datagridviewNE1);
            }
            else if (tipo == 4)
            {
                var c = (from x in cotizaciones.otros_cotizaciones where x.id == id select x).SingleOrDefault();
                if (c != null)
                {
                    c.clave = label76.Text;
                    c.articulo = label77.Text;
                    c.proveedor = label78.Text;
                    c.linea = label79.Text;
                    c.caracteristicas = richTextBox2.Text;
                    c.color = comboBox9.Text;
                    c.cantidad = Math.Round(stringToFloat(textBox30.Text), 2);
                    c.descuento = Math.Round(stringToFloat(textBox31.Text), 2);
                    c.largo = Math.Round(stringToFloat(textBox39.Text), 2);
                    c.alto = Math.Round(stringToFloat(textBox37.Text), 2);
                    c.total = Math.Round(stringToFloat(label106.Text), 2);
                    c.descripcion = this.descripcion;
                }
                cotizaciones.SaveChanges();
                constants.loadCotizacionesLocales("otros", datagridviewNE1);
            }
        }
        //

        private void setArticuloToEdit(int tipo, int id)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            listas = new listas_entities_pva();

            if (tipo == 1)
            {
                var c = (from x in cotizaciones.cristales_cotizados where x.id == id select x).SingleOrDefault();
                if (c != null)
                {
                    clave_tem = constants.getClave(c.clave);
                    var cr = (from v in listas.lista_costo_corte_e_instalado where v.clave == clave_tem select v).SingleOrDefault();

                    if (cr != null)
                    {
                        setEditImage(true, true);
                        if (c.lista == "Cristal Costo Corte")
                        {
                            setValuesToEdit(1);
                        }
                        else if (c.lista == "Cristal Precio Venta")
                        {
                            setValuesToEdit(2);
                        }
                        else if (c.lista == "Cristal Precio Hojas")
                        {
                            setValuesToEdit(3);
                        }
                        label9.Text = c.clave;
                        label8.Text = c.articulo;
                        label7.Text = c.lista;
                        label85.Text = c.proveedor;
                        textBox2.Text = c.largo.ToString();
                        textBox3.Text = c.alto.ToString();
                        textBox5.Text = c.cantidad.ToString();
                        checkBox4.Checked = c.filo_muerto > 0 ? true : false;
                        this.descripcion = c.descripcion;
                        textBox10.Text = c.filo_muerto > 0 ? c.filo_muerto.ToString() : "20";
                        if (c.tipo_venta == "Metro Cuadrado")
                        {
                            checkBox1.Checked = true;
                            checkBox2.Checked = false;
                        }
                        else if (c.tipo_venta == "Instalado")
                        {
                            checkBox1.Checked = false;
                            checkBox2.Checked = true;
                        }
                        textBox26.Text = c.descuento.ToString();
                        label41.Text = c.total.ToString();
                        datagridviewNE3.Rows.Clear();
                        createAcabadoRow(c.canteado);
                        createAcabadoRow(c.biselado);
                        createAcabadoRow(c.desconchado);
                        createAcabadoRow(c.pecho_paloma);
                        createAcabadoRow(c.perforado_media_pulgada);
                        createAcabadoRow(c.perforado_una_pulgada);
                        createAcabadoRow(c.perforado_dos_pulgadas);
                        createAcabadoRow(c.grabado);
                        createAcabadoRow(c.esmerilado);
                    }
                    else
                    {
                        MessageBox.Show("[Error] es posible que el artículo ya no exista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (tipo == 2)
            {
                var c = (from x in cotizaciones.aluminio_cotizado where x.id == id select x).SingleOrDefault();
                if (c != null)
                {
                    string cl = c.acabado;
                    id_tem = constants.getOriginalIDFromClave(c.clave);
                    var al = (from v in listas.perfiles where v.id == id_tem select v).SingleOrDefault();
                    var color = (from v in listas.colores_aluminio where v.clave == cl select v).SingleOrDefault();

                    if (al != null)
                    {
                        setEditImage(true, true);
                        setValuesToEdit(4);
                        label29.Text = c.clave;
                        label30.Text = c.articulo;
                        label28.Text = c.lista;
                        label87.Text = c.proveedor;
                        label31.Text = c.linea;
                        this.descripcion = c.descripcion;
                        textBox6.Text = c.largo_total.ToString();
                        if (color != null)
                        {
                            comboBox4.Text = c.acabado;
                        }
                        else
                        {
                            comboBox3.Text = c.acabado;
                        }
                        textBox7.Text = c.cantidad.ToString();
                        textBox27.Text = c.descuento.ToString();
                        label104.Text = c.total.ToString();
                    }
                    else
                    {
                        MessageBox.Show("[Error] es posible que el artículo ya no exista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (tipo == 3)
            {
                var c = (from x in cotizaciones.herrajes_cotizados where x.id == id select x).SingleOrDefault();
                if (c != null)
                {
                    id_tem = constants.getOriginalIDFromClave(c.clave);
                    var he = (from v in listas.herrajes where v.id == id_tem select v).SingleOrDefault();

                    if (he != null)
                    {
                        setEditImage(true, true);
                        setValuesToEdit(5);
                        label60.Text = c.clave;
                        label62.Text = c.articulo;
                        label61.Text = c.proveedor;
                        label59.Text = c.linea;
                        label55.Text = "Herrajes";
                        this.descripcion = c.descripcion;
                        richTextBox1.Text = c.caracteristicas;
                        comboBox8.Text = c.color;
                        textBox33.Text = c.cantidad.ToString();
                        textBox29.Text = c.descuento.ToString();
                        label43.Text = c.total.ToString();
                    }
                    else
                    {
                        MessageBox.Show("[Error] es posible que el artículo ya no exista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (tipo == 4)
            {
                var c = (from x in cotizaciones.otros_cotizaciones where x.id == id select x).SingleOrDefault();
                if (c != null)
                {
                    id_tem = constants.getOriginalIDFromClave(c.clave);
                    var ot = (from v in listas.otros where v.id == id_tem select v).SingleOrDefault();

                    if (ot != null)
                    {
                        setEditImage(true, true);
                        setValuesToEdit(6);
                        label76.Text = c.clave;
                        label77.Text = c.articulo;
                        label78.Text = c.proveedor;
                        label79.Text = c.linea;
                        label75.Text = "Otros";
                        richTextBox2.Text = c.caracteristicas;
                        comboBox9.Text = c.color;
                        this.descripcion = c.descripcion;
                        textBox30.Text = c.cantidad.ToString();
                        textBox31.Text = c.descuento.ToString();
                        textBox39.Text = c.largo.ToString();
                        textBox37.Text = c.alto.ToString();
                        label106.Text = c.total.ToString();
                    }
                    else
                    {
                        MessageBox.Show("[Error] es posible que el artículo ya no exista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (tipo == 5)
            {
                var c = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();
                if (c != null)
                {
                    if (c.concept_id <= 0)
                    {
                        id_tem = (int)c.modulo_id;
                        var mod = (from v in listas.modulos where v.id == id_tem select v).SingleOrDefault();

                        if (mod != null)
                        {
                            setEditImage(true, true);
                            setValuesToEdit(7);
                            abrirConfigModulo();
                        }
                        else
                        {
                            MessageBox.Show("[Error] es posible que el artículo ya no exista.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        if (Application.OpenForms["merge_items"] == null)
                        {
                            new merge_items(true, false, id).Show();
                        }
                        else
                        {
                            Application.OpenForms["merge_items"].Select();
                        }
                    }
                }
            }
        }
        //

        //Boton del contructor
        private void button6_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                if (constants.user_access >= 4)
                {
                    if (Application.OpenForms["crear_modulo"] == null)
                    {
                        if (Application.OpenForms["config_modulo"] == null)
                        {
                            this.Enabled = false;
                            crear_modulo modulo = new crear_modulo(0);
                            modulo.Show();
                            if (constants.maximizar_ventanas == true)
                            {
                                modulo.WindowState = FormWindowState.Maximized;
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] no es posible ingresar a esta característica mientras se configura un módulo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        Application.OpenForms["crear_modulo"].Select();
                        if (constants.maximizar_ventanas == true)
                        {
                            Application.OpenForms["crear_modulo"].WindowState = FormWindowState.Maximized;
                        }
                        else
                        {
                            Application.OpenForms["crear_modulo"].WindowState = FormWindowState.Normal;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("[Error] solo un usuario con privilegios de grado (4) puede acceder a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        //refrescar el datagrid de cotizzaciones al agregar articulo nuevo
        public void refreshNewArticulo(int tipo=-1)
        {
            if(tipo > 0)
            {
                constants.tipo_cotizacion = tipo;
            }
            switch (constants.tipo_cotizacion)
            {
                case 1:
                    constants.loadCotizacionesLocales("cristales", datagridviewNE1);
                    treeView1.SelectedNode = treeView1.Nodes[0].Nodes[0];
                    break;
                case 2:
                    constants.loadCotizacionesLocales("aluminio", datagridviewNE1);
                    treeView1.SelectedNode = treeView1.Nodes[0].Nodes[1];
                    break;
                case 3:
                    constants.loadCotizacionesLocales("herrajes", datagridviewNE1);
                    treeView1.SelectedNode = treeView1.Nodes[0].Nodes[2];
                    break;
                case 4:
                    constants.loadCotizacionesLocales("otros", datagridviewNE1);
                    treeView1.SelectedNode = treeView1.Nodes[0].Nodes[3];
                    break;
                case 5:
                    constants.loadCotizacionesLocales("modulos", datagridviewNE1);
                    treeView1.SelectedNode = treeView1.Nodes[0].Nodes[4];
                    break;
                default: break;
            }
        }
        //      

        //abrir clientes
        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                if (Application.OpenForms["clientes"] == null)
                {
                    new clientes().Show();
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        //editar clientes
        private void editarClienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                if (Application.OpenForms["clientes"] == null)
                {
                    clientes cliente = new clientes();
                    cliente.Show();
                    cliente.setEditTab();
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        //Error log
        private void errorLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Application.StartupPath+"\\error_log.txt") == true)
            {
                System.Diagnostics.Process.Start(Application.StartupPath + "\\error_log.txt");
            }
            else
            {
                MessageBox.Show("[Error] el archivo error_log.txt no se encuentra en la carpeta.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        //seleccionar la pestaña con mas número de articulos
        public void seleccionarPastaña()
        {
            if(constants.count_cristales > constants.count_aluminio && constants.count_cristales > constants.count_herrajes && constants.count_cristales > constants.count_otros && constants.count_cristales > constants.count_modulos)
            {
                constants.tipo_cotizacion = 1;
            }
            else if (constants.count_aluminio > constants.count_cristales && constants.count_aluminio > constants.count_herrajes && constants.count_aluminio > constants.count_otros && constants.count_aluminio > constants.count_modulos)
            {
                constants.tipo_cotizacion = 2;
            }
            else if (constants.count_herrajes > constants.count_cristales && constants.count_herrajes > constants.count_aluminio && constants.count_herrajes > constants.count_otros && constants.count_herrajes > constants.count_modulos)
            {
                constants.tipo_cotizacion = 3;
            }
            else if (constants.count_otros > constants.count_cristales && constants.count_otros > constants.count_aluminio && constants.count_otros > constants.count_herrajes && constants.count_otros > constants.count_modulos)
            {
                constants.tipo_cotizacion = 4;
            }
            else if (constants.count_modulos > constants.count_cristales && constants.count_modulos > constants.count_aluminio && constants.count_modulos > constants.count_herrajes && constants.count_modulos > constants.count_otros)
            {
                constants.tipo_cotizacion = 5;
            }
            //si se presenta igualdad
            if (constants.tipo_cotizacion <= 0)
            {
                if (constants.count_modulos > 0)
                {
                    constants.tipo_cotizacion = 5;
                }
                else if (constants.count_cristales > 0)
                {
                    constants.tipo_cotizacion = 1;
                }
                else if(constants.count_aluminio > 0)
                {
                    constants.tipo_cotizacion = 2;
                }
                else if (constants.count_herrajes > 0)
                {
                    constants.tipo_cotizacion = 3;
                }
                else if (constants.count_otros > 0)
                {
                    constants.tipo_cotizacion = 4;
                }                
            }
        }
        //

        //boton guardar % predeterminados
        private void button11_Click(object sender, EventArgs e)
        {
            constants.setPercentageToPropiedades(stringToFloat(textBox32.Text), stringToFloat(textBox34.Text), stringToFloat(textBox35.Text), stringToFloat(textBox36.Text));
            MessageBox.Show("Los porcentajes han sido actualizados.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //

        //load precentajes onClick
        private void TabPage14_Enter(object sender, EventArgs e)
        {
            constants.setPercentageOfArticulos(textBox32, textBox34, textBox35, textBox36);
        }
        // 

        //Resize modulo
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {            
            tableLayoutPanel1.Width = hScrollBar1.Value;          
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            hScrollBar1.Maximum = panel1.Width;
            if (((Control)tabPage5).Enabled == true)
            {
                if (Size.Width <= 1060)
                {
                    tableLayoutPanel1.Width = hScrollBar1.Maximum;
                    hScrollBar1.Value = hScrollBar1.Maximum;
                }
                else
                {
                    if (resize == true)
                    {
                        tableLayoutPanel1.Width = hScrollBar1.Maximum / 2;
                        hScrollBar1.Value = hScrollBar1.Maximum / 2;
                    }
                    else
                    {
                        tableLayoutPanel1.Width = hScrollBar1.Maximum;
                        hScrollBar1.Value = hScrollBar1.Maximum;
                    }
                }                            
            }
        }
        //

        private void removerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(datagridviewNE3.Rows.Count > 1)
            {
                if(datagridviewNE3.CurrentRow.Index != datagridviewNE3.Rows.Count - 1)
                {
                    datagridviewNE3.Rows.RemoveAt(datagridviewNE3.CurrentRow.Index);
                    if (comboBox1.SelectedIndex == 2)
                    {
                        calculoGeneralCristales("_hoja");
                    }
                    else
                    {
                        if (checkBox1.Checked == true)
                        {
                            calculoGeneralCristales("_m2");
                        }
                        else
                        {
                            calculoGeneralCristales("_instalado");
                        }
                    }
                }
            }
        }

        //ver lista grande de articulos de cotizacion
        private void button15_Click(object sender, EventArgs e)
        {
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
        }
        //

        //Personalizar modulo
        private void personalizarMóduloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(constants.tipo_cotizacion == 5)
            {
                cotizaciones_local cotizaciones = new cotizaciones_local();

                try {
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
                    countCotizacionesArticulo();
                    loadCountArticulos();
                    constants.loadCotizacionesLocales("modulos", datagridviewNE1);
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                    MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void setArticuloPersonalizacion(int id, int perso_id, int dir, bool remove=false)
        {           
            cotizaciones_local cotizaciones = new cotizaciones_local();
            try
            {
                Image img = null;
                var concepto = (from x in cotizaciones.modulos_cotizaciones where x.id == perso_id select x).SingleOrDefault();
                var modulo = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();

                if (modulo != null)
                {
                    if (remove == false)
                    {
                        if (concepto != null)
                        {
                            if (concepto.dir != 3)
                            {
                                img = constants.MergeTwoImages(constants.byteToImage(concepto.pic), constants.byteToImage(modulo.pic));                      
                            }
                            modulo.dir = dir;
                            modulo.merge_id = perso_id;
                            concepto.pic = img != null ? constants.imageToByte(img) : concepto.dir == 3 ? concepto.pic : modulo.pic;
                            concepto.concept_id = perso_id;
                            concepto.modulo_id = -1;
                            concepto.articulo = concepto.articulo.Length > 0 ? concepto.articulo + " + " + modulo.articulo : modulo.articulo;
                            concepto.total = Math.Round((((float)concepto.total / (float)concepto.cantidad) + (float)modulo.total) * (float)concepto.cantidad, 2);
                            concepto.linea = concepto.linea.Length <= 0 ? modulo.linea : constants.getLineasInConcept(concepto.linea, modulo.linea) == false ? concepto.linea + "/" + modulo.linea : concepto.linea;
                            concepto.ubicacion = modulo.ubicacion.Length <= 0 ? concepto.ubicacion : concepto.ubicacion.Length <= 0 ? modulo.ubicacion : constants.getUbicacionInConcept(concepto.ubicacion, modulo.ubicacion) == true ? concepto.ubicacion : modulo.ubicacion.Length > 0 ? concepto.ubicacion + "/" + modulo.ubicacion : concepto.ubicacion;
                            concepto.acabado_perfil = concepto.acabado_perfil == "" ? modulo.acabado_perfil : concepto.acabado_perfil;
                            concepto.diseño = concepto.diseño.Length > 0 ? concepto.diseño + "/" + modulo.diseño : modulo.diseño;
                            concepto.claves_cristales = concepto.claves_cristales + modulo.claves_cristales;
                        }
                        cotizaciones.SaveChanges();
                        countCotizacionesArticulo();
                        loadCountArticulos();
                        constants.updateModuloPersonalizado((int)concepto.concept_id);
                    }
                    else
                    {
                        concepto.dir = 0;
                        modulo.merge_id = -1;
                        cotizaciones.SaveChanges();
                        constants.updateModuloPersonalizado((int)concepto.concept_id);
                        countCotizacionesArticulo();
                        loadCountArticulos();
                    }
                    constants.loadCotizacionesLocales("modulos", datagridviewNE1);
                }
            }
            catch (Exception)
            {
                cotizaciones = null;
                cotizaciones = new cotizaciones_local();
                var concepto = (from x in cotizaciones.modulos_cotizaciones where x.id == perso_id select x).SingleOrDefault();
                if (concepto != null)
                {
                    concepto.pic = constants.imageToByte(Properties.Resources.new_concepto);
                }
                cotizaciones.SaveChanges();
                MessageBox.Show("[Error] se detecto un problema durante la integración del nuevo concepto.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Añadir a conceptos
        private void agregarConceptoToolStripMenuItem_Click(object sender, EventArgs e)
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

        //Nuevo concepto
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
                    countCotizacionesArticulo();
                    loadCountArticulos();
                    constants.loadCotizacionesLocales("modulos", datagridviewNE1);
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                    MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //    

        //Load modulos ver-config
        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if(sql.isModuleAlive(opened_module) == false)
            {
                module_alive = false;
            }
            else
            {
                module_alive = true;
            }
        }

        private void BackgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox9.Image = null;
            label97.Text = "";
            if (modulos_config == false)
            {
                if (constants.local == false)
                {
                    listas_entities_pva listas = new listas_entities_pva();
                    var m = (from x in listas.modulos where x.id == opened_module select x).SingleOrDefault();
                    if (m != null)
                    {
                        if (m.privado == false || constants.user_access == 6)
                        {
                            this.Enabled = false;
                            crear_modulo modulo = new crear_modulo(opened_module);
                            modulo.Show();
                            if (constants.maximizar_ventanas == true)
                            {
                                modulo.WindowState = FormWindowState.Maximized;
                            }
                        }
                        else
                        {
                            if (m.usuario == constants.user)
                            {
                                this.Enabled = false;
                                crear_modulo modulo = new crear_modulo(opened_module);
                                modulo.Show();
                                if (constants.maximizar_ventanas == true)
                                {
                                    modulo.WindowState = FormWindowState.Maximized;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Este módulo es privado y solo el autor puede hacer uso de el.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }               
                }
                else
                {
                    MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (module_alive == true)
                {
                    listas_entities_pva listas = new listas_entities_pva();
                    var m = (from x in listas.modulos where x.id == opened_module select x).SingleOrDefault();
                    if (m != null)
                    {
                        if (m.privado == false || constants.user_access == 6)
                        {
                            if (Application.OpenForms["config_modulo"] == null)
                            {
                                config_modulo modulo = new config_modulo(opened_module);
                                modulo.Show();
                                if (constants.maximizar_ventanas == true)
                                {
                                    modulo.WindowState = FormWindowState.Maximized;
                                }
                            }
                            else
                            {
                                ((config_modulo)Application.OpenForms["config_modulo"]).resetSession(opened_module, constants.id_articulo_cotizacion);
                                ((config_modulo)Application.OpenForms["config_modulo"]).Select();
                                //desminimizar
                                if (constants.maximizar_ventanas == true)
                                {
                                    ((config_modulo)Application.OpenForms["config_modulo"]).WindowState = FormWindowState.Maximized;
                                }
                                else
                                {
                                    ((config_modulo)Application.OpenForms["config_modulo"]).WindowState = FormWindowState.Normal;
                                }
                                ((config_modulo)Application.OpenForms["config_modulo"]).Focus();
                            }                        
                        }
                        else
                        {
                            if(m.usuario == constants.user)
                            {
                                if (Application.OpenForms["config_modulo"] == null)
                                {
                                    config_modulo modulo = new config_modulo(opened_module);
                                    modulo.Show();
                                    if (constants.maximizar_ventanas == true)
                                    {
                                        modulo.WindowState = FormWindowState.Maximized;
                                    }
                                }
                                else
                                {
                                    ((config_modulo)Application.OpenForms["config_modulo"]).resetSession(opened_module, constants.id_articulo_cotizacion);
                                    ((config_modulo)Application.OpenForms["config_modulo"]).Select();
                                    //desminimizar
                                    if (constants.maximizar_ventanas == true)
                                    {
                                        ((config_modulo)Application.OpenForms["config_modulo"]).WindowState = FormWindowState.Maximized;
                                    }
                                    else
                                    {
                                        ((config_modulo)Application.OpenForms["config_modulo"]).WindowState = FormWindowState.Normal;
                                    }
                                    ((config_modulo)Application.OpenForms["config_modulo"]).Focus();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Este módulo es privado y solo el autor puede hacer uso de el.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("[Error] el módulo con el ID " + opened_module + " ya no existe.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //---------------------------------------------------------------------->

        //Generar desglose de material     
        private void generarDesgloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["desglose_materiales"] == null)
            {
                new desglose_materiales().Show();
            }
            else
            {
                Application.OpenForms["desglose_materiales"].Select();
                Application.OpenForms["desglose_materiales"].WindowState = FormWindowState.Normal;
            }
        }

        //copybox
        private void button16_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["copy"] == null)
            {
                new copy().Show();
            }
            else
            {
                ((copy)Application.OpenForms["copy"]).Select();
                ((copy)Application.OpenForms["copy"]).WindowState = FormWindowState.Normal;
            }
        }

        public void openCopybox(int id)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            var copy = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();

            if(copy != null)
            {
                var paste = new copybox()
                {
                    modulo_id = copy.modulo_id,
                    descripcion = copy.descripcion,
                    mano_obra = copy.mano_obra,
                    dimensiones = copy.dimensiones,
                    acabado_perfil = copy.acabado_perfil,
                    claves_cristales = copy.claves_cristales,
                    cantidad = copy.cantidad,
                    articulo = copy.articulo,
                    linea = copy.linea,
                    diseño = copy.diseño,
                    clave = constants.getClave(copy.clave),
                    total = copy.total,
                    largo = copy.largo,
                    alto = copy.alto,
                    flete = copy.flete,
                    utilidad = copy.utilidad,
                    desperdicio = copy.desperdicio,
                    claves_otros = copy.claves_otros,
                    claves_herrajes = copy.claves_herrajes,
                    pic = copy.pic,
                    claves_perfiles = copy.claves_perfiles,
                    merge_id = copy.merge_id,
                    concept_id = copy.concept_id,
                    ubicacion = copy.ubicacion,
                    sub_folio = 0,
                    dir = copy.dir,
                    news = copy.news,
                    new_desing = copy.new_desing, 
                    orden = 0     
                };
                cotizaciones.copyboxes.Add(paste);
                cotizaciones.SaveChanges();

                if (copy.modulo_id == -1)
                {
                    int last = 0;
                    var g = (from x in cotizaciones.copyboxes orderby x.id descending select x).FirstOrDefault();

                    if (g != null)
                    {
                        last = g.id;
                    }

                    var last_m = (from x in cotizaciones.copyboxes where x.id == last select x).SingleOrDefault();

                    if (last_m != null)
                    {
                        last_m.concept_id = last;

                        var copy_m = from x in cotizaciones.modulos_cotizaciones where x.merge_id == id select x;

                        if (copy_m != null)
                        {
                            foreach (var v in copy_m)
                            {
                                var paste_m = new copybox()
                                {
                                    modulo_id = v.modulo_id,
                                    descripcion = v.descripcion,
                                    mano_obra = v.mano_obra,
                                    dimensiones = v.dimensiones,
                                    acabado_perfil = v.acabado_perfil,
                                    claves_cristales = v.claves_cristales,
                                    cantidad = v.cantidad,
                                    articulo = v.articulo,
                                    linea = v.linea,
                                    diseño = v.diseño,
                                    clave = constants.getClave(v.clave),
                                    total = v.total,
                                    largo = v.largo,
                                    alto = v.alto,
                                    flete = v.flete,
                                    utilidad = v.utilidad,
                                    desperdicio = v.desperdicio,
                                    claves_otros = v.claves_otros,
                                    claves_herrajes = v.claves_herrajes,
                                    pic = v.pic,
                                    claves_perfiles = v.claves_perfiles,
                                    merge_id = last,
                                    concept_id = v.concept_id,
                                    ubicacion = v.ubicacion,
                                    sub_folio = 0,
                                    dir = v.dir,    
                                    news = v.news,
                                    new_desing = v.new_desing,
                                    orden = 0                                
                                };
                                cotizaciones.copyboxes.Add(paste_m);
                            }
                        }
                    }                 
                }
                cotizaciones.SaveChanges();
                if (Application.OpenForms["copy"] == null)
                {
                    new copy().Show();
                }
                else
                {
                    ((copy)Application.OpenForms["copy"]).loadCopy();
                    ((copy)Application.OpenForms["copy"]).Select();
                    ((copy)Application.OpenForms["copy"]).WindowState = FormWindowState.Normal;
                }
            }          
        }       

        private void copiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openCopybox((int)datagridviewNE1.CurrentRow.Cells[0].Value);
        }

        //Habilitar cs
        private void CheckBox3_Click(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                constants.enable_cs = true;
            }
            else
            {
                constants.enable_cs = false;
            }

            try
            {
                XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                var mv = from x in opciones_xml.Descendants("Opciones") select x;

                foreach (XElement x in mv)
                {
                    x.SetElementValue("ECS", constants.enable_cs == true ? "true" : "false");
                }
                opciones_xml.Save(constants.opciones_xml);
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            constants.loadCotizacionesLocales("modulos", datagridviewNE1);
        }

        //Borrar articulos
        private void button17_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("¿Desea eliminar todos los artículos seleccionados?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {
                cotizaciones_local cotizaciones = new cotizaciones_local();

                if (constants.tipo_cotizacion == 1)
                {
                    var cristales = (from x in cotizaciones.cristales_cotizados select x);

                    foreach (var c in cristales)
                    {
                        cotizaciones.cristales_cotizados.Remove(c);
                        if (c.folio > 0)
                        {
                            constants.setFilaBorradaOnLocalDB(1, (int)c.folio, constants.getIDFromClave(c.clave));
                        }
                    }
                    cotizaciones.SaveChanges();
                    resetDatagridCotizaciones();
                    constants.loadCotizacionesLocales("cristales", datagridviewNE1);
                    resetCountArticulos();
                }
                else if (constants.tipo_cotizacion == 2)
                {
                    var aluminio = (from x in cotizaciones.aluminio_cotizado select x);

                    foreach (var c in aluminio)
                    {
                        cotizaciones.aluminio_cotizado.Remove(c);
                        if (c.folio > 0)
                        {
                            constants.setFilaBorradaOnLocalDB(2, (int)c.folio, constants.getIDFromClave(c.clave));
                        }
                    }
                    cotizaciones.SaveChanges();
                    resetDatagridCotizaciones();
                    constants.loadCotizacionesLocales("aluminio", datagridviewNE1);
                    resetCountArticulos();
                }
                else if (constants.tipo_cotizacion == 3)
                {
                    var herrajes = (from x in cotizaciones.herrajes_cotizados select x);

                    foreach (var c in herrajes)
                    {
                        cotizaciones.herrajes_cotizados.Remove(c);
                        if (c.folio > 0)
                        {
                            constants.setFilaBorradaOnLocalDB(3, (int)c.folio, constants.getIDFromClave(c.clave));
                        }
                    }
                    cotizaciones.SaveChanges();
                    resetDatagridCotizaciones();
                    constants.loadCotizacionesLocales("herrajes", datagridviewNE1);
                    resetCountArticulos();
                }
                else if (constants.tipo_cotizacion == 4)
                {
                    var otros = (from x in cotizaciones.otros_cotizaciones select x);

                    foreach (var c in otros)
                    {
                        cotizaciones.otros_cotizaciones.Remove(c);
                        if (c.folio > 0)
                        {
                            constants.setFilaBorradaOnLocalDB(4, (int)c.folio, constants.getIDFromClave(c.clave));
                        }
                    }
                    cotizaciones.SaveChanges();
                    resetDatagridCotizaciones();
                    constants.loadCotizacionesLocales("otros", datagridviewNE1);
                    resetCountArticulos();
                }
                else if (constants.tipo_cotizacion == 5)
                {
                    var modulos = (from x in cotizaciones.modulos_cotizaciones select x);

                    foreach (var c in modulos)
                    {
                        cotizaciones.modulos_cotizaciones.Remove(c);
                        if (c.folio > 0)
                        {
                            constants.setFilaBorradaOnLocalDB(5, (int)c.folio, constants.getIDFromClave(c.clave));
                        }
                    }
                    cotizaciones.SaveChanges();
                    resetDatagridCotizaciones();
                    constants.loadCotizacionesLocales("modulos", datagridviewNE1);
                    resetCountArticulos();
                }
                if (Application.OpenForms["config_modulo"] != null)
                {
                    Application.OpenForms["config_modulo"].Close();
                }
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL();
                }
                constants.id_articulo_cotizacion = -1;
                setEditImage(false, false);
                calcularTotalesCotizacion();
            }
        }

        //Eliminar articulos sub-folio
        private void button18_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("¿Desea eliminar todos los módulos del SUB-FOLIO seleccionado?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {
                cotizaciones_local cotizaciones = new cotizaciones_local();

                var modulos = (from x in cotizaciones.modulos_cotizaciones where x.sub_folio == constants.sub_folio select x);

                foreach (var c in modulos)
                {
                    cotizaciones.modulos_cotizaciones.Remove(c);
                    if (c.folio > 0)
                    {
                        constants.setFilaBorradaOnLocalDB(5, (int)c.folio, constants.getIDFromClave(c.clave));
                    }
                }
                cotizaciones.SaveChanges();
               
                if (Application.OpenForms["config_modulo"] != null)
                {
                    Application.OpenForms["config_modulo"].Close();
                }
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL();
                }
                constants.id_articulo_cotizacion = -1;
                setEditImage(false, false);
                resetDatagridCotizaciones();
                constants.loadCotizacionesLocales("modulos", datagridviewNE1);
                resetCountArticulos();
                calcularTotalesCotizacion();
            }
        }

        //sub-folios
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.SelectedIndex >= 0)
            {
                if (Application.OpenForms["config_modulo"] != null) { Application.OpenForms["config_modulo"].Close(); }
                constants.id_articulo_cotizacion = -1;
                setEditImage(false, false);
                constants.sub_folio = constants.stringToInt(comboBox5.Text);
                setSubFolioLabel();
                reloadAll();
                constants.loadCotizacionesLocales("modulos", datagridviewNE1);
                if (Application.OpenForms["articulos_cotizacion"] != null) { ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL(); }
            }
        }

        //from articulos cotizados
        public void setSubFolio(string sub_folio)
        {
            comboBox5.Text = sub_folio;
        }

        //set subfolio-label
        public void setSubFolioLabel()
        {
            label113.Text = "Sub-Folio: " + constants.sub_folio.ToString();
        }

        //Registro de presupuesto
        private void resToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                if (Application.OpenForms["registro_presupuesto"] == null)
                {
                    if (constants.folio_abierto > 0)
                    {
                        new registro_presupuesto().Show();
                    }
                    else
                    {
                        MessageBox.Show("[Error] no existe una cotización abierta.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Application.OpenForms["registro_presupuesto"].Select();
                    Application.OpenForms["registro_presupuesto"].WindowState = FormWindowState.Normal;
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void historialDePresupuestosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                if (Application.OpenForms["historial_registros"] == null)
                {
                    historial_registros historial = new historial_registros();
                    historial.Show();
                    if (constants.maximizar_ventanas == true)
                    {
                        historial.WindowState = FormWindowState.Maximized;
                    }
                }
                else
                {
                    Application.OpenForms["historial_registros"].Select();
                    Application.OpenForms["historial_registros"].WindowState = FormWindowState.Normal;
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //resetsubfoliobox
        public void resetSubfolio()
        {
            comboBox5.SelectedIndex = -1;
        }

        //duplicar concepto
        private void duplicarConceptoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            constants.duplicarConcepto(constants.tipo_cotizacion, (int)datagridviewNE1.CurrentRow.Cells[0].Value);
            reloadAll();
            if (Application.OpenForms["articulos_cotizacion"] != null)
            {
                ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL();
            }
        }   
        
        //Cancelar si fue eliminada la cotizacion
        public void ifDeleted()
        {
            if(constants.folio_abierto == constants.folio_eliminacion)
            {
                borrarCotizacion();
            }
        }

        //duplicar items de sub-folio
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox7.SelectedIndex != -1)
            {
                if (constants.stringToInt(comboBox7.Text) != constants.sub_folio)
                {
                    DialogResult r = MessageBox.Show("¿Desea duplicar todos los conceptos desde el sub-folio " + comboBox7.Text + " ?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (r == DialogResult.Yes)
                    {
                        constants.duplicarSubfolio(constants.stringToInt(comboBox7.Text), constants.sub_folio);
                        reloadAll();
                        if (Application.OpenForms["articulos_cotizacion"] != null)
                        {
                            ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL();
                        }
                    }
                    else
                    {
                        comboBox7.SelectedIndex = -1;
                    }
                }
            }
        }

        //page down
        private void button19_Click(object sender, EventArgs e)
        {
            if(page > 0)
            {
                page--;
                loadListaFromLocal();
            }
        }

        //page up
        private void button20_Click(object sender, EventArgs e)
        {
            if (page < pages)
            {
                page++;
                loadListaFromLocal();
            }
        }

        //acabados aluminio anodizado
        private void button21_Click(object sender, EventArgs e)
        {
            new colores(true, -1, true).ShowDialog();
        }

        public void setAcabadoColor(string acabado)
        {
            comboBox4.Text = acabado;
        }

        //filo muerto
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 2)
            {
                calculoGeneralCristales("_hoja");
            }
            else {
                if (checkBox1.Checked == true)
                {
                    calculoGeneralCristales("_m2");
                }
                else
                {
                    calculoGeneralCristales("_instalado");
                }
            }
        }

        //filo muerto
        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                if (comboBox1.SelectedIndex == 2)
                {
                    calculoGeneralCristales("_hoja");
                }
                else {
                    if (checkBox1.Checked == true)
                    {
                        calculoGeneralCristales("_m2");
                    }
                    else
                    {
                        calculoGeneralCristales("_instalado");
                    }
                }
            }
        }

        //verificar cotizacion registrada
        public void verificarRegistro()
        {
            if (new sqlDateBaseManager().verificarRegistro(constants.folio_abierto) == false)
            {
                label108.Text = "*Esta cotización no ha sido registrada.";
            }
            else
            {
                label108.Text = "";
            }
        }

        public void setVerificarRegistro()
        {
            verificarRegistro();
        }

        //descipcion
        private void button22_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["descipcion"] == null)
            {
                new descipcion(this.descripcion).ShowDialog();
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["descipcion"] == null)
            {
                new descipcion(this.descripcion).ShowDialog();
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["descipcion"] == null)
            {
                new descipcion(this.descripcion).ShowDialog();
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["descipcion"] == null)
            {
                new descipcion(this.descripcion).ShowDialog();
            }
        }

        //Descuentos predeterminados
        private void textBox32_TextChanged(object sender, EventArgs e)
        {
            if(constants.isFloat(textBox32.Text) == false)
            {
                textBox32.Text = "";
            }
        }

        private void textBox34_TextChanged(object sender, EventArgs e)
        {
            if (constants.isFloat(textBox34.Text) == false)
            {
                textBox34.Text = "";
            }
        }

        private void textBox35_TextChanged(object sender, EventArgs e)
        {
            if (constants.isFloat(textBox35.Text) == false)
            {
                textBox35.Text = "";
            }
        }

        private void textBox36_TextChanged(object sender, EventArgs e)
        {
            if (constants.isFloat(textBox36.Text) == false)
            {
                textBox36.Text = "";
            }
        }
        //------------------------------------------------------------>

        public void setDescription(string descripcion)
        {
            this.descripcion = descripcion;
        }

        //enviar cotizacion
        private void enviarCotizaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                if (constants.folio_abierto > 0)
                {
                    guardarCerrar();
                    new enviar().ShowDialog();
                }
                else
                {
                    MessageBox.Show("[Error] no existe una cotización abierta.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Borrar cotizacion
        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            try
            {
                constants.save_onEdit.Clear();
                checkBox5.Checked = true;
                textBox4.Text = "0";
                textBox28.Text = "0";
                constants.clearCotizacionesLocales();
                constants.folio_abierto = -1;
                constants.nombre_cotizacion = "";
                constants.nombre_proyecto = "";
                constants.autor_cotizacion = "";
                constants.fecha_cotizacion = "";
                constants.cotizacion_proceso = false;
                constants.cotizacion_guardada = false;
                constants.setClienteToPropiedades();
                constants.deleteFilasBorradasFromLocalDB();
                setModoLIVA();
                resetCountArticulos();
                resetDatagridCotizaciones();
                calcularTotalesCotizacion();
                constants.tipo_cotizacion = 0;
                constants.id_articulo_cotizacion = -1;
                constants.sub_folio = 1;
                setSubFolioLabel();
                comboBox5.SelectedIndex = -1;
                label22.Text = "0000000";
                label108.Text = "";
                toolStripStatusLabel3.Text = string.Empty;
                setEditImage(false, false);
                disableTabPage();
                tabControl1.SelectedTab = tabPage15;
                float tc = constants.getTCFromXML();
                if(constants.tc != tc)
                {
                    constants.changeTC(constants.tc, tc, "USD");
                }             
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        //Borrar completed
        private void BackgroundWorker4_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label100.Text = "";
            ((Form1)Application.OpenForms["Form1"]).setTCLabel(constants.tc);
        }

        //Aríticulos de Usuario
        private void artículosDeUsuarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                if (constants.user_access >= 5)
                {
                    if (Application.OpenForms["user_items"] == null)
                    {
                        user_items user_items = new user_items();
                        user_items.Show();
                        if (constants.maximizar_ventanas == true)
                        {
                            user_items.WindowState = FormWindowState.Maximized;
                        }
                    }
                    else
                    {
                        Application.OpenForms["user_items"].WindowState = FormWindowState.Normal;
                        Application.OpenForms["user_items"].Select();
                    }
                }
                else
                {
                    MessageBox.Show("[Error] solo un usuario con privilegios de grado (5) puede acceder a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Opciones del programa
        private void opcionesDelProgramaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            admin_propiedades p = new admin_propiedades();
            p.ShowDialog();
        }

        //Produccion
        private void ordenDeProducciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                if (constants.folio_abierto > 0)
                {
                    new produccion().ShowDialog();
                }
                else
                {
                    MessageBox.Show("[Error] la cotización debe de estar ligada a un folio para continuar.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        //enable IVA
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                this.enable_iva = true;
                constants.iva = constants.getPropiedadesModel();
                constants.iva_desglosado = true;
            }
            else
            {
                this.enable_iva = false;
                constants.iva = 1;
                constants.iva_desglosado = false;
            }

            calcularTotalesCotizacion();

            try
            {
                XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                var mv = from x in opciones_xml.Descendants("Opciones") select x;

                foreach (XElement x in mv)
                {
                    x.SetElementValue("IVD", checkBox5.Checked == true ? "true" : "false");                   
                }
                opciones_xml.Save(constants.opciones_xml);
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /////----------------------------------------------------------------------------->

        public string[] getTotalAndDescount()
        {
            string[] r = new string[2];
            r[0] = label20.Text;
            r[1] = constants.stringToFloat(textBox4.Text).ToString();
            return r;
        }

        private void habilitarCambioDeParametrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new delete_password(false, true).ShowDialog();
        }

        private void inventariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (constants.local == false)
            {
                if (constants.user_access >= 4)
                {
                    if (Application.OpenForms["inventario"] == null)
                    {
                        inventario inventario = new inventario();
                        inventario.Show();                        
                    }
                    else
                    {
                        if (Application.OpenForms["inventario"].WindowState == FormWindowState.Minimized)
                        {
                            Application.OpenForms["inventario"].WindowState = FormWindowState.Normal;
                        }                        
                        Application.OpenForms["inventario"].Select();
                    }
                }
                else
                {
                    MessageBox.Show("[Error] solo un usuario con privilegios de grado (4) puede acceder a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] se ha ingresado de manera local, no es posible ingresar a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void setModoLIVA()
        {
            if (constants.m_liva)
            {
                if (constants.stringToFloat(textBox28.Text) <= 0)
                {
                    textBox28.Text = "16";
                }
                checkBox5.Checked = false;
                //---------------------------->
                textBox28.Enabled = false;
            }
            else
            {
                textBox28.Text = "0";
                checkBox5.Checked = true;
                textBox28.Enabled = true;
            }
        }

        public void setTCLabel(float tc)
        {
            label15.Text = "Tipo de Cambio (USD): $" + tc + " MXN";
        }      
    }
}