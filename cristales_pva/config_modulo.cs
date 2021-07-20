using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;

namespace cristales_pva
{
    public partial class config_modulo : Form
    {
        List<string> esquemas = new List<string>();
        List<string> dimensions = new List<string>();
        List<string> claves_cristales = new List<string>();
        List<string> claves_otros = new List<string>();
        List<string> claves_herrajes = new List<string>();
        List<string> claves_perfiles = new List<string>();
        List<string> new_items = new List<string>();
        List<string> new_costos = new List<string>();
        string[] reglas;

        TableLayoutPanel panel;

        int marco_width = 0, marco_height = 0;
        int c = 1;
        int seccion_e = -1, height = 0, width = 0;
        int id = -1;
        int largo_total = 0;
        int alto_total = 0;
        int id_cotizacion = -1;
        bool cs = false;
        int rows = 0;
        int columns = 0;
        int diseño_id = 0;
        float peso_aluminio = 0;

        public config_modulo(int module_id)
        {
            InitializeComponent();
            checkBox11.Click += CheckBox11_Click;
            checkBox12.Click += CheckBox12_Click;
            checkBox13.Click += CheckBox13_Click;
            checkBox14.Click += CheckBox14_Click;
            dataGridView1.CellClick += DataGridView1_CellClick;
            hScrollBar1.ValueChanged += HScrollBar1_ValueChanged;
            dataGridView1.CellLeave += DataGridView1_CellLeave;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            dataGridView2.CellClick += DataGridView2_CellClick;
            dataGridView2.CellLeave += DataGridView2_CellLeave;
            dataGridView2.CellEndEdit += DataGridView2_CellEndEdit;
            dataGridView3.CellClick += DataGridView3_CellClick;
            dataGridView3.CellLeave += DataGridView3_CellLeave;
            dataGridView3.CellEndEdit += DataGridView3_CellEndEdit;
            dataGridView4.CellClick += DataGridView4_CellClick;
            dataGridView4.CellLeave += DataGridView4_CellLeave;
            dataGridView4.CellEndEdit += DataGridView4_CellEndEdit;
            dataGridView5.CellEndEdit += DataGridView5_CellEndEdit;
            dataGridView5.CellClick += DataGridView5_CellClick;
            dataGridView5.CellLeave += DataGridView5_CellLeave;
            textBox1.TextChanged += TextBox1_TextChanged;
            textBox2.TextChanged += TextBox2_TextChanged;
            checkBox10.Click += CheckBox10_Click;
            checkBox8.Click += CheckBox8_Click;
            checkBox19.Click += CheckBox19_Click;
            checkBox17.Click += CheckBox17_Click;
            contextMenuStrip4.Opening += ContextMenuStrip4_Opening;
            contextMenuStrip5.Opening += ContextMenuStrip5_Opening;
            contextMenuStrip6.Opening += ContextMenuStrip6_Opening;
            contextMenuStrip7.Opening += ContextMenuStrip7_Opening;
            Shown += Config_modulo_Shown;
            this.FormClosing += Config_modulo_FormClosing;
            dataGridView5.CellContextMenuStripNeeded += DataGridView5_CellContextMenuStripNeeded;
            SizeChanged += Config_modulo_SizeChanged;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            //bloquear parametros---------------->
            if(constants.user_access <= 1 && constants.permitir_cp == false)
            {
                textBox5.Enabled = false;
                textBox6.Enabled = false;
                textBox3.Enabled = false;
                textBox7.Enabled = false;
                checkBox8.Checked = false;
            }
            else
            {
                checkBox8.Checked = true;
            }
            //------------------------------------>
            if (constants.enable_rules == true)
            {
                checkBox10.Checked = true;
            }
            else
            {
                checkBox10.Checked = false;
            }
            resetTextBox();
            loadConfigParemeters();
            scrollParameters();
            loadColoresAluminio();
            startSession(module_id, constants.id_articulo_cotizacion);
            comboBox1.SelectedIndex = comboBox1.SelectedIndex;
            comboBox3.SelectedIndex = comboBox3.SelectedIndex;
            checkBox21.Checked = constants.siempre_permitir_ac;
            checkBox16.Checked = constants.ajustar_medidas_aut;
            if (checkBox21.Checked)
            {
                checkBox6.Checked = true;
                checkBox7.Checked = true;
                checkBox15.Checked = true;
                checkBox2.Checked = true;
                checkBox4.Checked = true;
                checkBox3.Checked = true;
                checkBox5.Checked = true;
            }
            else
            {
                checkBox6.Checked = false;
                checkBox7.Checked = false;
                checkBox15.Checked = false;
                checkBox2.Checked = false;
                checkBox4.Checked = false;
                checkBox3.Checked = false;
                checkBox5.Checked = false;
            }
            textBox14.KeyDown += TextBox14_KeyDown;
            textBox14.Leave += TextBox14_Leave;    
            textBox14.Text = constants.lim_sm.ToString();
            this.KeyPreview = true;
            this.KeyDown += Config_modulo_KeyDown;
            reloadSecciones();
        }

        private void Config_modulo_Shown(object sender, EventArgs e)
        {
            constants.checkTasaCero(this);
        }

        private void Config_modulo_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.F5)
            {
                if (Application.OpenForms["edit_expresss"] == null)
                {
                    new edit_expresss().Show();
                }
                else
                {
                    Application.OpenForms["edit_expresss"].Select();
                    Application.OpenForms["edit_expresss"].WindowState = FormWindowState.Normal;
                }
            }
            else if(e.KeyData == Keys.F6)
            {
                if (Application.OpenForms["new_articulo"] == null)
                {
                    new new_articulo(new_costos).Show(this);
                }
                else
                {
                    if (Application.OpenForms["new_articulo"].WindowState == FormWindowState.Minimized)
                    {
                        Application.OpenForms["new_articulo"].WindowState = FormWindowState.Normal;
                    }
                    Application.OpenForms["new_articulo"].Select();
                }
            }
            else if(e.KeyData == Keys.F1)
            {
                button1.Focus();
                agregar();
            }
        }

        private void setLimitSM(float lim)
        {
            constants.lim_sm = lim;
            try
            {
                XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                var spac = from x in opciones_xml.Descendants("Opciones") select x;

                foreach (XElement x in spac)
                {
                    x.SetElementValue("LIM_SM", lim.ToString());
                }
                opciones_xml.Save(constants.opciones_xml);
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TextBox14_Leave(object sender, EventArgs e)
        {
            setLimitSM(constants.stringToFloat(textBox14.Text));
            checkWeight();
        }

        //lim sistemas manual
        private void TextBox14_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                setLimitSM(constants.stringToFloat(textBox14.Text));
                checkWeight();
            }
        }

        private bool checkIsAvailableManual()
        {
            bool r = false;
            string clave = string.Empty;
            listas_entities_pva listas = new listas_entities_pva();

            foreach (DataGridViewRow x in dataGridView4.Rows)
            {
                clave = x.Cells[2].Value.ToString();
                var otros = (from v in listas.otros where v.clave == clave select v).SingleOrDefault();

                if (otros != null)
                {
                    if (otros.linea == "carrete")
                    {
                        r = true;
                        break;
                    }                   
                }
            }
            return r;
        }

        private void CheckBox17_Click(object sender, EventArgs e)
        {
            if (checkBox17.Checked)
            {
                if (checkIsAvailableManual() == false)
                {
                    checkBox17.Checked = false;
                    MessageBox.Show("[Error] La cortina no incluye un sistema de elevación manual.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ContextMenuStrip7_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridView4.RowCount > 0)
            {
                if (dataGridView4.CurrentRow.Cells[0].Style.BackColor == Color.Yellow)
                {
                    contextMenuStrip7.Show(MousePosition);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void ContextMenuStrip6_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridView3.RowCount > 0)
            {
                if (dataGridView3.CurrentRow.Cells[0].Style.BackColor == Color.Yellow)
                {
                    contextMenuStrip6.Show(MousePosition);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void ContextMenuStrip5_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridView2.RowCount > 0)
            {
                if (dataGridView2.CurrentRow.Cells[0].Style.BackColor == Color.Yellow)
                {
                    contextMenuStrip5.Show(MousePosition);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void ContextMenuStrip4_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (dataGridView1.CurrentRow.Cells[0].Style.BackColor == Color.Yellow)
                {
                    contextMenuStrip4.Show(MousePosition);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        //Remove new Items
        private void removerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (dataGridView1.CurrentRow.Cells[0].Style.BackColor == Color.Yellow)
                {
                    dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
                    calcularCostoModulo();
                }
            }
        }

        private void removerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.RowCount > 0)
            {
                if (dataGridView2.CurrentRow.Cells[0].Style.BackColor == Color.Yellow)
                {
                    dataGridView2.Rows.Remove(dataGridView2.CurrentRow);
                    calcularCostoModulo();
                }
            }
        }

        private void removerToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (dataGridView3.RowCount > 0)
            {
                if (dataGridView3.CurrentRow.Cells[0].Style.BackColor == Color.Yellow)
                {
                    dataGridView3.Rows.Remove(dataGridView3.CurrentRow);
                    calcularCostoModulo();
                }
            }
        }

        private void removerToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (dataGridView4.RowCount > 0)
            {
                if (dataGridView4.CurrentRow.Cells[0].Style.BackColor == Color.Yellow)
                {
                    dataGridView4.Rows.Remove(dataGridView4.CurrentRow);
                    calcularCostoModulo();
                }
            }
        }           
        //----------------------------------------------------------------------->

        //set parameters
        public void setNewParameters(string desperdicio, string flete, string mano_obra, string utlidad)
        {
            textBox5.Text = desperdicio;
            textBox6.Text = flete;
            textBox3.Text = mano_obra;
            textBox7.Text = utlidad;
        }

        //habilitar reglas
        private void CheckBox10_Click(object sender, EventArgs e)
        {
            if (checkBox10.Checked)
            {
                checkBox10.Checked = false;
                constants.enable_rules = false;
            }
            else
            {
                checkBox10.Checked = true;
                constants.enable_rules = true;
            }

            try
            {
                XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                var mv = from x in opciones_xml.Descendants("Opciones") select x;

                foreach (XElement x in mv)
                {
                    x.SetElementValue("ERL", constants.enable_rules == true ? "true" : "false");
                }
                opciones_xml.Save(constants.opciones_xml);
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadFactory(int id_cotizacion)
        {
            if(id_cotizacion <= -1)
            {
                //Acabado
                if (constants.factory_acabado_perfil != string.Empty)
                {
                    string[] a = constants.factory_acabado_perfil.Split(',');
                    if (a.Length == 2)
                    {
                        if (a[0] == "0")
                        {
                            comboBox1.Text = a[1];
                        }
                        else if (a[0] == "1")
                        {
                            comboBox3.Text = a[1];
                        }
                    }
                }
                //Cristales
                if(constants.factory_cristal != string.Empty)
                {
                    string[] a = constants.factory_cristal.Split(',');
                    if(a.Length == 2)
                    {
                        foreach (DataGridViewRow x in dataGridView2.Rows)
                        {
                            setNewCristal(x.Index, a[0], a[1]);
                        }
                    }
                }
            }
        }

        private void startSession(int module_id, int id_cotizacion, bool load=true)
        {
            loadParameters(module_id);
            loadFactory(id_cotizacion);         
            loadOnEdit(id_cotizacion, load);
        }

        private void resetTextBox()
        {
            loadConfigParemeters();
            textBox1.Text = "0";
            textBox2.Text = "0";
            textBox8.Text = "";
            richTextBox1.Text = "";
            textBox4.Text = "1";          
            textBox13.Text = "";
            label12.Text = "0.00";
        }

        public void resetSession(int module_id, int id_cotizacion, bool load=true)
        {          
            tableLayoutPanel1.Visible = false;
            resetTextBox();
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            dataGridView4.Rows.Clear();
            dataGridView5.Rows.Clear();
            dataGridView6.Rows.Clear();          
            comboBox1.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            checkBox17.Checked = false;
            checkBox18.Checked = false;
            pictureBox2.Image = null;
            label37.Text = "";
            this.Text = "";
            label29.Text = "...";
            label30.Text = "...";
            label25.Text = "";
            label43.Text = "";
            panel.Controls.Clear();
            panel.BackColor = Color.White;
            tableLayoutPanel1.Controls.Clear();
            panel = null;
            esquemas.Clear();
            dimensions.Clear();
            claves_cristales.Clear();
            claves_otros.Clear();
            claves_herrajes.Clear();
            claves_perfiles.Clear();           
            new_items.Clear();
            new_costos.Clear();            
            marco_width = 0;
            marco_height = 0;
            c = 1;
            height = 0;
            width = 0;
            id = -1;
            largo_total = 0;
            alto_total = 0;
            cs = false;
            reglas = null;
            rows = 0;
            columns = 0;
            diseño_id = 0;
            seccion_e = -1;
            peso_aluminio = 0;
            checkBox11.Checked = true;
            checkBox12.Checked = true;
            checkBox13.Checked = true;
            checkBox14.Checked = true;
            checkBox9.Checked = false;
            setNewClaveModulo(module_id);
            startSession(module_id, id_cotizacion, load);
            tableLayoutPanel1.Visible = true;
        }

        //Refrescar clave al intercambiar modulo
        private void setNewClaveModulo(int modulo_id)
        {
            string[] c = label6.Text.Split('-');
            listas_entities_pva listas = new listas_entities_pva();
           
            var modulos = (from x in listas.modulos where x.id == modulo_id select x).SingleOrDefault();
            if(modulos != null)
            {
                if(c.Length == 2)
                {
                    label6.Text = modulos.clave + "-" + c[1];
                }
                else
                {
                    label6.Text = modulos.clave;
                }
            }          
        }      

        //Cerrar edit express si esta cierra
        private void Config_modulo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Application.OpenForms["edit_expresss"] != null)
            {
                Application.OpenForms["edit_expresss"].Close();
            }
            //------------------------------------------------------------------------>
            if (Application.OpenForms["new_articulo"] != null)
            {
                Application.OpenForms["new_articulo"].Close();
            }          
        }

        //Recargar modulo original
        private void reloadModulo(int module_id)
        {
            textBox1.Text = "0";
            textBox2.Text = "0";
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            dataGridView4.Rows.Clear();
            dataGridView5.Rows.Clear();
            dataGridView6.Rows.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            tableLayoutPanel1.Controls.Clear();
            pictureBox2.Image = null;
            label37.Text = "";
            checkBox9.Checked = false;
            loadParameters(module_id);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            reloadModulo(id);
            calcularCostoModulo();
            recountItems();
            richTextBox1.Clear();
        }
        //

        //Crear pic modulo
        private Bitmap createModuloPic()
        {
            if (constants.mostrar_acabado == false)
            {
                clearBackground();
            }

            Bitmap _img = null;

            try
            {
                tableLayoutPanel1.Enabled = false;
                Panel p = new Panel();
                splitContainer1.Panel2.Controls.Add(p);
                p.Show();
                p.Dock = DockStyle.Fill;
                p.BackColor = Color.White;
                p.BringToFront();

                panel.Dock = DockStyle.Fill;
                foreach (Control x in panel.Controls)
                {
                    x.Width = (panel.Width - getMarcoWidth()) / columns;
                    x.Height = (panel.Height - getMarcoHeight()) / rows;
                }

                Bitmap bm = new Bitmap(panel.Width, panel.Height);
                panel.DrawToBitmap(bm, new Rectangle(0, 0, panel.Width, panel.Height));
                panel.Dock = DockStyle.None;

                //Reset --------------------------------------------------->
                foreach (Control x in panel.Controls)
                {
                    x.Width = (panel.Width - getMarcoWidth()) / columns;
                    x.Height = (panel.Height - getMarcoHeight()) / rows;
                }
                //--------------------------------------------------------->

                p.Dispose();
                tableLayoutPanel1.Enabled = true;

                _img = new Bitmap(bm, 120, 105);
                bm = null;
            }
            catch (Exception)
            {
                //Do nothing
            }
            return _img;
        }
        //

        //Dibujar medidas
        private void drawDimesions(Control ctl)
        {
            ctl.Paint += new PaintEventHandler((sender, e) =>
            {
                e.Graphics.Clear(Color.White);
                Pen pen = new Pen(Color.Black, 1);             
                e.Graphics.DrawLine(pen, new Point(panel.Width + 3, 0), new Point(panel.Width + 3, ctl.Height + 3));
                e.Graphics.DrawLine(pen, new Point(0, panel.Height + 3), new Point(ctl.Width + 3, panel.Height + 3));                
            });           
        }
        // ------------------------------------------------------------------------------------------------------------------------------->

        private void checkAcabadoSelectivo()
        {
            string x = string.Empty;
            bool y = false;
            foreach(DataGridViewRow z in dataGridView1.Rows)
            {
                if (y == false)
                {
                    if (z.Cells[7].Value.ToString() != "")
                    {
                        x = z.Cells[7].Value.ToString();
                        y = true;
                    }
                }
                else
                {
                    if (z.Cells[7].Value.ToString() != "")
                    {
                        if (z.Cells[7].Value.ToString() != x)
                        {
                            checkBox9.Checked = true;
                        }
                    }
                }           
            }
        }
        
        private void DataGridView5_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (dataGridView5.RowCount > 1)
            {
                if (dataGridView5.CurrentCell.ColumnIndex == 2 || dataGridView5.CurrentCell.ColumnIndex == 3)
                {
                    contextMenuStrip1.Show(MousePosition);
                }
            }
        }

        private void loadColoresAluminio()
        {
            listas_entities_pva listas = new listas_entities_pva();

            var colores = from x in listas.colores_aluminio select x;

            if(colores != null)
            {
                comboBox3.Items.Clear();
                foreach(var c in colores)
                {
                    comboBox3.Items.Add(c.clave);
                }
            }
        }

        public void autoEscalar()
        {
            if (panel.Width > (tableLayoutPanel1.Width - 10))
            {
                while(panel.Width > (tableLayoutPanel1.Width - 10))
                {
                    if (hScrollBar1.Value > 1)
                    {
                        hScrollBar1.Value = hScrollBar1.Value - 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if(panel.Height > (tableLayoutPanel1.Height - 10))
            {
                while (panel.Height > (tableLayoutPanel1.Height - 10))
                {
                    if (hScrollBar1.Value > 1)
                    {
                        hScrollBar1.Value = hScrollBar1.Value - 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public void getColorAluminio(string clave)
        {
            comboBox3.SelectedIndex = -1;
            comboBox3.Text = clave;
        }

        public void getColorAluminoManual(string clave, int index, string type)
        {
            if (constants.stringToFloat(dataGridView1.Rows[index].Cells[4].Value.ToString()) > 0)
            {
                dataGridView1.Rows[index].Cells[7].Value = clave;
                string s = getMostAcabado();
                if (type == "lista")
                {
                    comboBox1.Text = s;
                }
                else if (type == "pa")
                {
                    comboBox3.Text = s;
                }
                checkPefilesAcabados();
                calcularCostoModulo();
            }
            for (int i = c; i <= constants.stringToInt(label9.Text); i++)
            {
                getSeccionesReady(i);
            }
        }

        private void loadParameters(int module_id)
        {          
            loadModulo(module_id);
            secciones();
            label10.Text = countArticulos(0).ToString();
            checkDimensions(false);
            calcularCostoModulo();
            id = module_id;
            if(label7.Text.Contains("C/M") == true)
            {
                checkBox19.Checked = true;
            }
            else
            {
                checkBox19.Checked = false;
            }
        }   

        private void recountItems()
        {
            label10.Text = countArticulos(0).ToString();
            if (dataGridView5.RowCount > 0) {
                foreach (DataGridViewRow x in dataGridView5.Rows)
                {
                    x.Cells[1].Value = countArticulos(0, constants.stringToInt(x.Cells[0].Value.ToString()));
                }
            }
        }

        private void loadOnEdit(int id_cotizacion, bool load=true)
        {
            this.id_cotizacion = id_cotizacion;
            bool motor = false;

            if (id_cotizacion > -1)
            {
                cotizaciones_local cotizacion = new cotizaciones_local();
                listas_entities_pva listas = new listas_entities_pva();

                var modulo = (from x in cotizacion.modulos_cotizaciones where x.id == id_cotizacion select x).SingleOrDefault();
                if (modulo != null)
                {
                    richTextBox1.Text = modulo.descripcion;
                    textBox3.Text = modulo.mano_obra.ToString();
                    textBox4.Text = modulo.cantidad.ToString();
                    textBox5.Text = modulo.desperdicio.ToString();
                    textBox6.Text = modulo.flete.ToString();
                    textBox7.Text = modulo.utilidad.ToString();
                    textBox8.Text = modulo.ubicacion;
                    //Only if load new one--------------------------------->
                    if (load)
                    {
                        label6.Text = modulo.clave;
                    }
                    //----------------------------------------------------->
                    if (modulo.folio > 0)
                    {
                        label38.Text = "Folio: " + modulo.folio.ToString();
                    }
                    Text = modulo.articulo + (id_cotizacion > -1 ? " - " + id_cotizacion : "");
                    string clave = string.Empty;
                    string buffer = string.Empty;
                    int c = 0;
                    string[] v;

                    //Carga las dimensiones del modulo guardado
                    foreach (char x in modulo.dimensiones)
                    {
                        if (x == ',')
                        {
                            dimensions.Add(buffer);
                            buffer = string.Empty;
                            continue;
                        }
                        buffer = buffer + x.ToString();
                    }
                    //

                    if (modulo.claves_cristales.Length > 0)
                    {
                        checkBox12.Checked = true;
                        dataGridView2.Enabled = true;
                        foreach (char x in modulo.claves_cristales)
                        {
                            if (x == ',')
                            {
                                claves_cristales.Add(buffer);
                                buffer = string.Empty;
                                continue;
                            }
                            buffer = buffer + x.ToString();
                        }
                    }
                    else
                    {
                        checkBox12.Checked = false;
                        dataGridView2.Enabled = false;
                    }

                    if (modulo.claves_otros.Length > 0)
                    {
                        checkBox14.Checked = true;
                        dataGridView4.Enabled = true;
                        foreach (char x in modulo.claves_otros)
                        {
                            if (x == ',')
                            {
                                claves_otros.Add(buffer);
                                buffer = string.Empty;
                                continue;
                            }
                            buffer = buffer + x.ToString();
                        }
                    }
                    else
                    {
                        checkBox14.Checked = false;
                        dataGridView4.Enabled = false;
                    }

                    if (modulo.claves_herrajes.Length > 0)
                    {
                        checkBox13.Checked = true;
                        dataGridView3.Enabled = true;
                        foreach (char x in modulo.claves_herrajes)
                        {
                            if (x == ',')
                            {
                                claves_herrajes.Add(buffer);
                                buffer = string.Empty;
                                continue;
                            }
                            buffer = buffer + x.ToString();
                        }
                    }
                    else
                    {
                        checkBox13.Checked = false;
                        dataGridView3.Enabled = false;
                    }

                    if (modulo.claves_perfiles.Length > 0)
                    {
                        checkBox11.Checked = true;
                        dataGridView1.Enabled = true;
                        foreach (char x in modulo.claves_perfiles)
                        {
                            if (x == ',')
                            {
                                claves_perfiles.Add(buffer);
                                buffer = string.Empty;
                                continue;
                            }
                            buffer = buffer + x.ToString();
                        }
                    }
                    else
                    {
                        checkBox11.Checked = false;
                        dataGridView1.Enabled = true;
                    }

                    //new items
                    if (modulo.news.Length > 0)
                    {
                        foreach (char x in modulo.news)
                        {
                            if (x == ';')
                            {
                                new_items.Add(buffer);
                                buffer = string.Empty;
                                continue;
                            }
                            buffer = buffer + x.ToString();
                        }
                        string[] t = null;
                        foreach(string x in new_items)
                        {
                            t = x.Split(',');
                            if(t.Length > 0)
                            {
                                if(t[0] == "1")
                                {
                                    if (!checkBox11.Checked)
                                    {
                                        checkBox11.Checked = true;
                                        dataGridView1.Enabled = true;
                                    }
                                }
                                //-------------------------------->
                                if (t[0] == "2")
                                {
                                    if (!checkBox12.Checked)
                                    {
                                        checkBox12.Checked = true;
                                        dataGridView2.Enabled = true;
                                    }
                                }
                                //-------------------------------->
                                if (t[0] == "3")
                                {
                                    if (!checkBox13.Checked)
                                    {
                                        checkBox13.Checked = true;
                                        dataGridView3.Enabled = true;
                                    }
                                }
                                //-------------------------------->
                                if (t[0] == "4")
                                {
                                    if (!checkBox14.Checked)
                                    {
                                        checkBox14.Checked = true;
                                        dataGridView4.Enabled = true;
                                    }
                                }
                                //-------------------------------->
                                if (t[0] == "5")
                                {
                                    if (t.Length == 4)
                                    {
                                        new_costos.Add(x);
                                    }
                                }
                            }
                        }
                        displayNewCosto();
                    }
                    //    

                    if (dimensions.Count > 2)
                    {
                        for (int i = 0; i < dimensions.Count; i++)
                        {
                            if (c <= (dataGridView5.RowCount - 1))
                            {
                                if (i % 2 == 0)
                                {
                                    dataGridView5.Rows[c].Cells[2].Value = dimensions[i];
                                }
                                else
                                {
                                    dataGridView5.Rows[c].Cells[3].Value = dimensions[i];
                                    c++;
                                }
                            }                       
                        }
                    }
                    else if (dimensions.Count == 2)
                    {
                        textBox1.Text = dimensions[0];
                        textBox2.Text = dimensions[1];
                    }

                    //Only if load new one--------------------------------->
                    if (dataGridView2.RowCount == claves_cristales.Count && load)
                    {
                        for (int i = 0; i < claves_cristales.Count; i++)
                        {
                            v = claves_cristales[i].Split('-');
                            if (v.Length > 0)
                            {
                                clave = v[0];
                            }
                            else
                            {
                                clave = claves_cristales[i];
                            }

                            var cristal = (from x in listas.lista_costo_corte_e_instalado where x.clave == clave select x).SingleOrDefault();

                            if (cristal != null)
                            {
                                dataGridView2.Rows[i].Cells[1].Value = cristal.clave;
                                dataGridView2.Rows[i].Cells[2].Value = cristal.articulo;
                                if (v.Length > 1)
                                {
                                    dataGridView2.Rows[i].Cells[3].Value = v[1];
                                }
                            }
                        }
                    }

                    clave = "";

                    //Only if load new one--------------------------------->
                    if (dataGridView4.RowCount == claves_otros.Count && load)
                    {
                        for (int i = 0; i < claves_otros.Count; i++)
                        {
                            v = claves_otros[i].Split('-');
                            clave = v[0];
                            var otros = (from x in listas.otros where x.clave == clave select x).SingleOrDefault();

                            if (otros != null)
                            {
                                dataGridView4.Rows[i].Cells[1].Value = otros.id;
                                dataGridView4.Rows[i].Cells[2].Value = clave;
                                dataGridView4.Rows[i].Cells[3].Value = otros.articulo;
                                if (v.Length > 1)
                                {
                                    dataGridView4.Rows[i].Cells[4].Value = v[1];
                                    //Check sistema de elevacion
                                    if (modulo.diseño == "CM")
                                    {
                                        if (otros.linea == "motores")
                                        {
                                            if (constants.stringToFloat(v[1]) > 0)
                                            {
                                                motor = true;
                                            }
                                        }
                                    }
                                }
                                dataGridView4.Rows[i].Cells[7].Value = otros.color;                              
                            }
                        }
                    }

                    clave = "";
                    //Check sistema de elevacion
                    if(motor == false)
                    {
                        checkBox18.Checked = false;
                        checkBox17.Checked = true;
                    }
                    else
                    {
                        checkBox18.Checked = true;
                        checkBox17.Checked = false;
                    }

                    //Only if load new one--------------------------------->
                    if (dataGridView3.RowCount == claves_herrajes.Count && load)
                    {
                        for (int i = 0; i < claves_herrajes.Count; i++)
                        {
                            v = claves_herrajes[i].Split('-');
                            clave = v[0];
                            var herrajes = (from x in listas.herrajes where x.clave == clave select x).SingleOrDefault();

                            if (herrajes != null)
                            {
                                dataGridView3.Rows[i].Cells[1].Value = herrajes.id;
                                dataGridView3.Rows[i].Cells[2].Value = clave;
                                dataGridView3.Rows[i].Cells[3].Value = herrajes.articulo;
                                if (v.Length > 1)
                                {
                                    dataGridView3.Rows[i].Cells[4].Value = v[1];
                                }
                                dataGridView3.Rows[i].Cells[6].Value = herrajes.color;
                            }
                        }
                    }

                    clave = "";

                    //Only if load new one--------------------------------->
                    if (dataGridView1.RowCount == claves_perfiles.Count && load)
                    {
                        for (int i = 0; i < claves_perfiles.Count; i++)
                        {
                            v = claves_perfiles[i].Split('-');
                            clave = v[0];
                            var perfiles = (from x in listas.perfiles where x.clave == clave select x).SingleOrDefault();

                            if (perfiles != null)
                            {
                                dataGridView1.Rows[i].Cells[1].Value = perfiles.id;
                                dataGridView1.Rows[i].Cells[2].Value = clave;
                                dataGridView1.Rows[i].Cells[3].Value = perfiles.articulo;
                                //Cantidad                      
                                if (v.Length > 1)
                                {
                                    dataGridView1.Rows[i].Cells[4].Value = v[1];
                                    if (constants.stringToFloat(v[1]) > 0)
                                    {
                                        //Acabado
                                        if (v.Length == 3)
                                        {
                                            dataGridView1.Rows[i].Cells[7].Value = v[2];
                                        }
                                    }
                                }
                            }
                        }                      
                    }

                    //new items
                    foreach (string x in new_items)
                    {
                        string[] n = x.Split(',');
                        if (n[0] != "5")
                        {
                            setNewItem(constants.stringToInt(n[0]), n[1], n[2], n[3], n[4], n[5]);
                        }
                    }

                    //check acabados selectivo
                    checkAcabadoSelectivo();
                    comboBox1.Text = modulo.acabado_perfil;
                    comboBox3.Text = modulo.acabado_perfil;
                }
            }                            
            checkDimensions();
            calcularCostoModulo();
            recountItems();
        }

        private void DataGridView5_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            deSelectSeccion(dataGridView5, 0);
        }

        private void DataGridView5_CellClick(object sender, DataGridViewCellEventArgs e)
        {         
            selectSeccion(dataGridView5, 0);
            label25.Text = "Largo (mm): " + dataGridView5.CurrentRow.Cells[2].Value + "\n" + "Alto (mm): " + dataGridView5.CurrentRow.Cells[3].Value;
        }

        private void Config_modulo_SizeChanged(object sender, EventArgs e)
        {
            checkDimensions(false);
            if (this.WindowState == FormWindowState.Minimized)
            {
                if (Application.OpenForms["edit_expresss"] != null)
                {
                    Application.OpenForms["edit_expresss"].WindowState = FormWindowState.Minimized;
                }
            }          
        }

        private void checkDimensions(bool checkInstruction=true)
        {
            if (panel != null)
            {
                int sum_w = 0;
                int sum_h = 0;
                float mts = 0;

                foreach (DataGridViewRow x in dataGridView5.Rows)
                {
                    seccion_e = constants.stringToInt(x.Cells[0].Value.ToString());
                    height = constants.stringToInt(x.Cells[3].Value.ToString());
                    width = constants.stringToInt(x.Cells[2].Value.ToString());

                    if (checkBox1.Checked == true)
                    {
                        if (seccion_e == 0)
                        {
                            panel.Width = (width / (hScrollBar1.Maximum - hScrollBar1.Value)) + getMarcoWidth();
                            panel.Height = (height / (hScrollBar1.Maximum - hScrollBar1.Value)) + getMarcoHeight();
                            label20.Text = height + " mm";
                            label21.Text = width + " mm";
                            largo_total = width;
                            alto_total = height;
                            if (cs == false)
                            {
                                for (int i = 0; i < panel.Controls.Count; i++)
                                {
                                    panel.Controls[i].Width = (width / panel.ColumnCount) / (hScrollBar1.Maximum - hScrollBar1.Value);
                                    panel.Controls[i].Height = (height / panel.RowCount) / (hScrollBar1.Maximum - hScrollBar1.Value);
                                }
                            }
                        }
                        else
                        {
                            panel.Controls[seccion_e - 1].Width = width / (hScrollBar1.Maximum - hScrollBar1.Value);
                            panel.Controls[seccion_e - 1].Height = height / (hScrollBar1.Maximum - hScrollBar1.Value);
                            if (panel.ColumnCount > 1)
                            {
                                if (panel.GetPositionFromControl(panel.Controls[seccion_e - 1]).Row == 0)
                                {
                                    sum_w = sum_w + width;
                                }
                            }
                            else
                            {
                                sum_w = width;
                            }
                            if (panel.RowCount > 1)
                            {
                                if (panel.GetPositionFromControl(panel.Controls[seccion_e - 1]).Column == 0)
                                {
                                    sum_h = sum_h + height;
                                }
                            }
                            else
                            {
                                sum_h = height;
                            }
                            label29.Text = sum_h + " mm";
                            label30.Text = sum_w + " mm";
                        }
                    }
                    else
                    {
                        if (cs == false)
                        {
                            for (int i = 0; i < panel.Controls.Count; i++)
                            {
                                panel.Controls[i].Width = (width / panel.ColumnCount) / (hScrollBar1.Maximum - hScrollBar1.Value);
                                panel.Controls[i].Height = (height / panel.RowCount) / (hScrollBar1.Maximum - hScrollBar1.Value);
                            }
                        }
                        else
                        {
                            panel.Controls[seccion_e - 1].Width = width / (hScrollBar1.Maximum - hScrollBar1.Value);
                            panel.Controls[seccion_e - 1].Height = height / (hScrollBar1.Maximum - hScrollBar1.Value);
                            panel.Width = ((width * panel.ColumnCount) / (hScrollBar1.Maximum - hScrollBar1.Value)) + getMarcoWidth();
                            panel.Height = ((height * panel.RowCount) / (hScrollBar1.Maximum - hScrollBar1.Value)) + getMarcoHeight();
                            if (constants.stringToFloat(textBox1.Text) > 0 && constants.stringToFloat(textBox2.Text) > 0)
                            {
                                label20.Text = constants.stringToFloat(textBox2.Text) + " mm";
                                label21.Text = constants.stringToFloat(textBox1.Text) + " mm";
                                largo_total = constants.stringToInt(textBox1.Text);
                                alto_total = constants.stringToInt(textBox2.Text);
                            }
                            else
                            {
                                if (panel.ColumnCount > 1)
                                {
                                    if (panel.GetPositionFromControl(panel.Controls[seccion_e - 1]).Row == 0)
                                    {
                                        sum_w = sum_w + width;
                                    }
                                }
                                else
                                {
                                    sum_w = width;
                                }
                                if (panel.RowCount > 1)
                                {
                                    if (panel.GetPositionFromControl(panel.Controls[seccion_e - 1]).Column == 0)
                                    {
                                        sum_h = sum_h + height;
                                    }
                                }
                                else
                                {
                                    sum_h = height;
                                }
                                label20.Text = sum_h + " mm";
                                label21.Text = sum_w + " mm";
                                largo_total = sum_w;
                                alto_total = sum_h;
                            }
                        }
                    }
                }
                if (checkBox1.Checked == true)
                {
                    if (sum_w > largo_total || sum_w < largo_total)
                    {
                        label30.ForeColor = Color.Red;
                    }
                    else
                    {
                        label30.ForeColor = Color.Green;
                    }
                    if (sum_h > alto_total || sum_h < alto_total)
                    {
                        label29.ForeColor = Color.Red;
                    }
                    else
                    {
                        label29.ForeColor = Color.Green;
                    }
                }               
                drawDimesions(tableLayoutPanel1);
                autoEscalar();
                mts = mts + (float)(Math.Round((float)largo_total / 1000, 2) * Math.Round((float)alto_total / 1000, 2));
                label40.Text = "Superficie: " + Math.Round(mts, 2).ToString("0.00") + " m2";
            }
        }        

        private void checkWeight()
        {
            if (label18.Text == "CM")
            {
                if (checkBox17.Checked)
                {
                    if (peso_aluminio > constants.lim_sm)
                    {
                        MessageBox.Show("[Error] El peso de la cortina no es el indicado para un sistema de elevación manual.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        label42.ForeColor = Color.Red;
                    }
                    else
                    {
                        label42.ForeColor = Color.Green;
                    }

                    string clave = string.Empty;
                    listas_entities_pva listas = new listas_entities_pva();

                    foreach (DataGridViewRow x in dataGridView4.Rows)
                    {
                        clave = x.Cells[2].Value.ToString();
                        var otros = (from v in listas.otros where v.clave == clave select v).SingleOrDefault();

                        if (otros != null)
                        {
                            if (otros.linea == "carrete" || otros.linea == "motores")
                            {
                                x.Cells[4].Value = "0";
                            }
                        }
                    }

                    foreach (DataGridViewRow x in dataGridView4.Rows)
                    {
                        clave = x.Cells[2].Value.ToString();
                        var otros = (from v in listas.otros where v.clave == clave select v).SingleOrDefault();

                        if (otros != null)
                        {
                            if (otros.linea == "carrete")
                            {
                                x.Cells[4].Value = "1";
                            }
                        }
                    }
                }
                else if (checkBox18.Checked)
                {
                    validMotor();
                }
            }
        }

        private bool checkWeightBool()
        {
            bool r = true;
            if (label18.Text == "CM")
            {
                if (checkBox18.Checked)
                {
                    if (peso_aluminio > getMotor())
                    {
                        label42.ForeColor = Color.Red;
                        return false;
                    }
                    else
                    {
                        label42.ForeColor = Color.Green;
                    }
                }
                else if(checkBox17.Checked)
                {
                    if(peso_aluminio > constants.lim_sm)
                    {
                        label42.ForeColor = Color.Red;
                        return false;
                    }
                    else
                    {
                        label42.ForeColor = Color.Green;
                    }
                }
            }
            return r;
        }

        private bool checkSelectionBoolCortinas()
        {
            bool r = true;
            if (label18.Text == "CM")
            {
               if(!checkBox17.Checked && !checkBox18.Checked)
                {
                    r = false;
                }
            }
            return r;
        }

        private float getMotor()
        {
            listas_entities_pva listas = new listas_entities_pva();
            string clave = string.Empty;
            float cant = 0;
            float r = 0;          
            foreach (DataGridViewRow x in dataGridView4.Rows)
            {
                if (constants.stringToFloat(x.Cells[4].Value.ToString()) > 0)
                {
                    clave = x.Cells[2].Value.ToString();
                    cant = constants.stringToFloat(x.Cells[4].Value.ToString());
                    var otros = (from v in listas.otros where v.clave == clave select v).SingleOrDefault();
                    if (otros != null)
                    {
                        if (otros.linea == "motores")
                        {
                            r = constants.stringToFloat(otros.caracteristicas.ToString()) * cant;
                        }
                    }
                }
            }     
            return r;
        }

        private void calcularTramosLargos(listas_entities_pva listas)
        {
            if (dataGridView1.RowCount > 0 && dataGridView5.RowCount > 0)
            {
                int id = 0;
                peso_aluminio = 0;

                foreach (DataGridViewRow x in dataGridView1.Rows)
                {
                    id = constants.stringToInt(x.Cells[1].Value.ToString());
                    var perfiles = (from v in listas.perfiles where v.id == id select v).SingleOrDefault();

                    if (perfiles != null)
                    {
                        if (perfiles.linea == "celosia" || perfiles.linea == "duelas" || perfiles.linea == "lama")
                        {
                            int seccion = constants.stringToInt(x.Cells[6].Value.ToString());
                            if (!checkBox1.Checked)
                            {
                                seccion = -1;
                            }
                            float count = constants.stringToFloat(x.Cells[4].Value.ToString());
                            if ((int)perfiles.ancho_perfil > 0)
                            {
                                if (count >= 0)
                                {
                                    if (x.Cells[5].Value.ToString() == "largo")
                                    {
                                        if (seccion > 0)
                                        {
                                            x.Cells[4].Value = Math.Floor((float)(constants.stringToFloat(dataGridView5.Rows[seccion].Cells[3].Value.ToString()) / perfiles.ancho_perfil));
                                            peso_aluminio = peso_aluminio + (constants.stringToFloat(x.Cells[4].Value.ToString()) * (float)perfiles.kg_peso_lineal) * (constants.stringToFloat(dataGridView5.Rows[seccion].Cells[2].Value.ToString())/1000f);
                                        }
                                        else
                                        {
                                            x.Cells[4].Value = Math.Floor((float)(alto_total / perfiles.ancho_perfil));
                                            peso_aluminio = peso_aluminio + (constants.stringToFloat(x.Cells[4].Value.ToString()) * (float)perfiles.kg_peso_lineal) * (largo_total/1000f);
                                        }
                                    }
                                    else if (x.Cells[5].Value.ToString() == "alto")
                                    {
                                        if (seccion > 0)
                                        {
                                            x.Cells[4].Value = Math.Floor((float)(constants.stringToFloat(dataGridView5.Rows[seccion].Cells[2].Value.ToString()) / perfiles.ancho_perfil));
                                            peso_aluminio = peso_aluminio + (constants.stringToFloat(x.Cells[4].Value.ToString()) * (float)perfiles.kg_peso_lineal) * (constants.stringToFloat(dataGridView5.Rows[seccion].Cells[2].Value.ToString())/1000f);
                                        }
                                        else
                                        {
                                            x.Cells[4].Value = Math.Floor((float)(largo_total / perfiles.ancho_perfil));
                                            peso_aluminio = peso_aluminio + (constants.stringToFloat(x.Cells[4].Value.ToString()) * (float)perfiles.kg_peso_lineal) * (largo_total/1000f);
                                        }
                                    }
                                    getInstruction(x.Cells[2].Value.ToString(), constants.stringToFloat(x.Cells[4].Value.ToString()));
                                    checkAcabados();
                                }
                            }
                            else
                            {
                                MessageBox.Show("[Error] a el perfil con clave: " + perfiles.clave + " no se le indicó el ancho correspondiente.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private void DataGridView4_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            deSelectSeccion(dataGridView4, 6);
        }

        private void DataGridView3_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            deSelectSeccion(dataGridView3, 5);
        }

        private void DataGridView2_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            deSelectSeccion(dataGridView2, 4);
        }

        private void DataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            deSelectSeccion(dataGridView1, 6);
        }

        private void deSelectSeccion(DataGridView table, int cell)
        {
            if (table.RowCount > 0)
            {
                int seccion = constants.stringToInt(table.CurrentRow.Cells[cell].Value.ToString());
                if (comboBox1.Text != "" || comboBox3.Text != "")
                {
                    if (seccion >= 0)
                    {
                        if (seccion > 0)
                        {
                            constants.setBackgroundImg(comboBox1.Text != "" ? "acabados_perfil" : "acabados_especiales", comboBox1.Text != "" ? comboBox1.Text : comboBox3.Text, "jpg", panel.Controls[seccion - 1]);
                        }
                        else
                        {
                            constants.setBackgroundImg(comboBox1.Text != "" ? "acabados_perfil" : "acabados_especiales", comboBox1.Text != "" ? comboBox1.Text : comboBox3.Text, "jpg", panel);
                        }
                    }
                }
                else
                {
                    if (seccion > 0)
                    {
                        panel.Controls[seccion - 1].BackColor = Color.LightBlue;
                    }
                    else
                    {
                        panel.BackColor = Color.LightBlue;
                    }
                }
            }
        }

        private void selectSeccion(DataGridView table, int cell)
        {
            if (table.RowCount > 0)
            {
                int seccion = constants.stringToInt(table.CurrentRow.Cells[cell].Value.ToString());
                if (seccion >= 0)
                {
                    if (seccion > 0)
                    {
                        panel.Controls[seccion - 1].BackgroundImage = null;
                        panel.Controls[seccion - 1].BackColor = Color.Red;
                    }
                    else
                    {
                        panel.BackgroundImage = null;
                        panel.BackColor = Color.Red;
                    }
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

        private void loadDiseño(int id_diseño, listas_entities_pva lista)
        {
            var diseño = (from x in lista.esquemas where x.id == id_diseño select x).SingleOrDefault();

            if(diseño != null)
            {
                diseño_id = diseño.id;        
                label18.Text = diseño.diseño;
                if(diseño.diseño == "CM")
                {
                    checkBox17.Visible = true;
                    checkBox18.Visible = true;
                    label42.Visible = true;
                    label45.Visible = true;
                    textBox14.Visible = true;
                }
                else
                {
                    checkBox17.Visible = false;
                    checkBox18.Visible = false;
                    label42.Visible = false;
                    label45.Visible = false;
                    textBox14.Visible = false;
                }
                label41.Text = "Columnas: " + diseño.columnas + " / " + "Filas: " + diseño.filas;
                panel = new TableLayoutPanel();
                tableLayoutPanel1.Controls.Add(panel);
                panel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
                panel.BackColor = Color.LightBlue;
                panel.Controls.Clear();
                panel.RowCount = (int)diseño.filas;
                panel.ColumnCount = (int)diseño.columnas;
                panel.BackgroundImageLayout = ImageLayout.Stretch;               
                rows = (int)diseño.filas;
                columns = (int)diseño.columnas;
                getEsquemasFromDiseño(diseño.esquemas);
                foreach (string e in esquemas)
                {
                    if (File.Exists(constants.folder_resources_dir + "esquemas\\corredizas\\" + e + ".png"))
                    {
                        constants.loadDiseñoOnDimension("esquemas\\corredizas\\", e, panel);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\puertas\\" + e + ".png"))
                    {
                        constants.loadDiseñoOnDimension("esquemas\\puertas\\", e, panel);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_abatibles\\" + e + ".png"))
                    {
                        constants.loadDiseñoOnDimension("esquemas\\ventanas_abatibles\\", e, panel);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_fijas\\" + e + ".png"))
                    {
                        constants.loadDiseñoOnDimension("esquemas\\ventanas_fijas\\", e, panel);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\templados\\" + e + ".png"))
                    {
                        constants.loadDiseñoOnDimension("esquemas\\templados\\", e, panel);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\otros\\" + e + ".png"))
                    {
                        constants.loadDiseñoOnDimension("esquemas\\otros\\", e, panel);
                    }
                }
                if (diseño.marco == true)
                {
                    if (diseño.grupo == "puerta")
                    {
                        panel.Padding = new Padding(10, 10, 10, 0);
                        marco_width = 20;
                        marco_height = 10;
                    }
                    else
                    {
                        panel.Padding = new Padding(10, 10, 10, 10);
                        marco_width = 20;
                        marco_height = 20;
                    }
                    checkBox1.Checked = true;
                    c = 0;
                }
                else
                {                      
                    checkBox1.Checked = false;
                    c = 1;
                    panel.BackColor = Color.Transparent;
                }
                panel.RowStyles.Clear();
                for (int i = 0; i < panel.RowCount; i++)
                {
                    panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                }
                panel.ColumnStyles.Clear();
                for (int i = 0; i < panel.ColumnCount; i++)
                {
                    panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                }              
            }                         
        }

        private void scrollParameters()
        {           
            label19.Text = "x -" + (hScrollBar1.Maximum - hScrollBar1.Value);
        }

        private void ajustGlobalDimension()
        {
            if (checkBox16.Checked)
            {
                if (dataGridView5.RowCount > 0)
                {
                    if (dataGridView5.Rows[0].Cells[0].Value != null)
                    {
                        if (dataGridView5.Rows[0].Cells[0].Value.ToString() == "0")
                        {
                            if (dataGridView5.CurrentRow.Index != 0)
                            {
                                int g_largo = 0;
                                int g_alto = 0;
                                if (dataGridView5.CurrentCell.OwningColumn.Index == 2)
                                {
                                    foreach (DataGridViewRow x in dataGridView5.Rows)
                                    {
                                        if (x.Index != 0)
                                        {
                                            g_largo = g_largo + constants.stringToInt(x.Cells[2].Value.ToString());
                                        }
                                    }
                                    dataGridView5.Rows[0].Cells[2].Value = (int)(g_largo / panel.RowCount);
                                }
                                else if (dataGridView5.CurrentCell.OwningColumn.Index == 3)
                                {
                                    foreach (DataGridViewRow x in dataGridView5.Rows)
                                    {
                                        if (x.Index != 0)
                                        {
                                            g_alto = g_alto + constants.stringToInt(x.Cells[3].Value.ToString());
                                        }
                                    }
                                    dataGridView5.Rows[0].Cells[3].Value = (int)(g_alto / panel.ColumnCount);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DataGridView5_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView5.CurrentCell.Value != null)
            {
                if (constants.stringToInt(dataGridView5.CurrentCell.Value.ToString()) > 0)
                {
                    ajustGlobalDimension();
                    checkDimensions();
                    calcularCostoModulo();
                }
                else
                {
                    dataGridView5.CurrentCell.Value = "0";
                }
            }
            else
            {
                dataGridView5.CurrentCell.Value = "0";
            }
        }

        private void getSeccionesReady(int seccion)
        {
            bool ready = true;
            if (checkBox11.Checked)
            {
                foreach (DataGridViewRow x in dataGridView1.Rows)
                {
                    if (constants.stringToFloat(x.Cells[4].Value.ToString()) > 0)
                    {
                        if (constants.stringToInt(x.Cells[6].Value.ToString()) == seccion && x.Cells[7].Value.ToString() == "")
                        {
                            ready = false;
                            break;
                        }
                    }
                }
            }

            if (checkBox12.Checked)
            {
                foreach (DataGridViewRow x in dataGridView2.Rows)
                {
                    if (constants.stringToFloat(x.Cells[3].Value.ToString()) > 0)
                    {
                        if (constants.stringToInt(x.Cells[4].Value.ToString()) == seccion && x.Cells[1].Value.ToString() == "")
                        {
                            ready = false;
                            break;
                        }
                    }
                }
            }

            if (checkBox13.Checked)
            {
                foreach (DataGridViewRow x in dataGridView3.Rows)
                {
                    if (constants.stringToFloat(x.Cells[4].Value.ToString()) > 0)
                    {
                        if (constants.stringToInt(x.Cells[5].Value.ToString()) == seccion && x.Cells[2].Value.ToString() == "")
                        {
                            ready = false;
                            break;
                        }
                    }
                }
            }

            if (checkBox14.Checked)
            {
                foreach (DataGridViewRow x in dataGridView4.Rows)
                {
                    if (constants.stringToFloat(x.Cells[4].Value.ToString()) > 0)
                    {
                        if (constants.stringToInt(x.Cells[6].Value.ToString()) == seccion && x.Cells[2].Value.ToString() == "")
                        {
                            ready = false;
                            break;
                        }
                    }
                }
            }

            foreach (DataGridViewRow x in dataGridView5.Rows)
            {
                if (constants.stringToInt(x.Cells[0].Value.ToString()) == seccion)
                {
                    if (ready == true)
                    {
                        x.DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        x.DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }                         
        }        

        public void setNewCristal(int index, string clave, string articulo)
        {
            dataGridView2.Rows[index].Cells[1].Value = clave;
            dataGridView2.Rows[index].Cells[2].Value = articulo;
            getInstruction(clave);
            getSeccionesReady(constants.stringToInt(dataGridView2.Rows[index].Cells[4].Value.ToString()));
            calcularCostoModulo();
        }

        public void setNewHerraje(int index, int id, string clave, string articulo, string color)
        {
            dataGridView3.Rows[index].Cells[1].Value = id;
            dataGridView3.Rows[index].Cells[2].Value = clave;
            dataGridView3.Rows[index].Cells[3].Value = articulo;
            dataGridView3.Rows[index].Cells[6].Value = color;
            getInstruction(clave);
            getSeccionesReady(constants.stringToInt(dataGridView3.CurrentRow.Cells[5].Value.ToString()));
            calcularCostoModulo();
        }

        public void setNewOtros(int index, int id, string clave, string articulo, string color)
        {
            dataGridView4.Rows[index].Cells[1].Value = id;
            dataGridView4.Rows[index].Cells[2].Value = clave;
            dataGridView4.Rows[index].Cells[3].Value = articulo;
            dataGridView4.Rows[index].Cells[7].Value = color;
            getInstruction(clave);
            getSeccionesReady(constants.stringToInt(dataGridView4.CurrentRow.Cells[6].Value.ToString()));
            calcularCostoModulo();
        }

        public void setNewPerfiles(int index, int id, string clave, string articulo)
        {
            dataGridView1.Rows[index].Cells[1].Value = id;
            dataGridView1.Rows[index].Cells[2].Value = clave;
            dataGridView1.Rows[index].Cells[3].Value = articulo;
            getInstruction(clave);
            getSeccionesReady(constants.stringToInt(dataGridView1.CurrentRow.Cells[6].Value.ToString()));
            calcularCostoModulo();
            //Reload Acabado
            if(comboBox1.Text != string.Empty)
            {
                int s = comboBox1.SelectedIndex;
                comboBox1.SelectedIndex = -1;
                comboBox1.SelectedIndex = s;
            }
            else if(comboBox3.Text != string.Empty)
            {
                int s = comboBox3.SelectedIndex;
                comboBox3.SelectedIndex = -1;
                comboBox3.SelectedIndex = s;
            }
        }

        private void DataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView4.RowCount > 0)
            {
                if (dataGridView4.CurrentCell.OwningColumn.HeaderText == "Artículo")
                {
                    new config_items(dataGridView4.CurrentRow.Index, 1, "", constants.stringToInt(dataGridView4.CurrentRow.Cells[1].Value.ToString()), checkBox5.Checked).ShowDialog();
                }
                comboBoxSelection(dataGridView4);
                selectSeccion(dataGridView4, 6);
            }
        }

        private void DataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView3.RowCount > 0)
            {              
                if (dataGridView3.CurrentCell.OwningColumn.HeaderText == "Artículo")
                {
                    new config_items(dataGridView3.CurrentRow.Index, 0, "", constants.stringToInt(dataGridView3.CurrentRow.Cells[1].Value.ToString()), checkBox4.Checked).ShowDialog();
                }
                comboBoxSelection(dataGridView3);
                selectSeccion(dataGridView3, 5);
            }
        }

        private void DataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.RowCount > 0)
            {
                if (dataGridView2.CurrentCell.OwningColumn.HeaderText == "Artículo")
                {
                    new config_items(dataGridView2.CurrentRow.Index, 2, dataGridView2.CurrentRow.Cells[1].Value.ToString(), -1).ShowDialog();
                }
                comboBoxSelection(dataGridView2);
                selectSeccion(dataGridView2, 4);
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.RowCount > 0)
            {
                if (checkBox7.Checked == true)
                {
                    if (dataGridView1.CurrentCell.OwningColumn.HeaderText == "Artículo")
                    {
                        new config_items(dataGridView1.CurrentRow.Index, 3, dataGridView1.CurrentRow.Cells[1].Value.ToString(), -1).ShowDialog();
                    }
                }
                if (checkBox9.Checked == true)
                {
                    if (dataGridView1.CurrentCell.OwningColumn.HeaderText == "Acabado")
                    {
                        contextMenuStrip3.Show(MousePosition);
                    }
                }
                comboBoxSelection(dataGridView1);
                selectSeccion(dataGridView1, 6);
            }
        }

        private void reloadSecciones()
        {
            for (int i = c; i <= constants.stringToInt(label9.Text); i++)
            {
                getSeccionesReady(i);
            }
        }

        private void secciones()
        {
            for(int i = c; i <= constants.stringToInt(label9.Text); i++)
            {
                if (i == 0)
                {
                    dataGridView5.Rows.Add(i, countArticulos(0, i), 1000 * panel.ColumnCount, 1000 * panel.RowCount);
                }
                else
                {
                    if(panel.GetColumnSpan(panel.Controls[i-1]) > 0 && panel.GetRowSpan(panel.Controls[i - 1]) > 0)
                    {
                        dataGridView5.Rows.Add(i, countArticulos(0, i), 1000 * panel.GetColumnSpan(panel.Controls[i - 1]), 1000 * panel.GetRowSpan(panel.Controls[i - 1]));                       
                    }
                    else if(panel.GetColumnSpan(panel.Controls[i - 1]) > 0 && panel.GetRowSpan(panel.Controls[i - 1]) <= 0)
                    {
                        dataGridView5.Rows.Add(i, countArticulos(0, i), 1000 * panel.GetColumnSpan(panel.Controls[i - 1]), 1000);
                    }
                    else if (panel.GetColumnSpan(panel.Controls[i - 1]) <= 0 && panel.GetRowSpan(panel.Controls[i - 1]) > 0)
                    {
                        dataGridView5.Rows.Add(i, countArticulos(0, i), 1000, 1000 * panel.GetRowSpan(panel.Controls[i - 1]));
                    }
                    else
                    {
                        dataGridView5.Rows.Add(i, countArticulos(0, i), 1000, 1000);
                    }
                }
                getSeccionesReady(i);
            }
        }

        private float countArticulos(int articulo, int seccion=-1)
        {
            float perfiles = 0, cristales = 0, herrajes = 0, otros = 0;

            foreach(DataGridViewRow x in dataGridView1.Rows)
            {
                if (seccion >= 0)
                {
                    if (seccion == constants.stringToInt(x.Cells[6].Value.ToString()))
                    {
                        perfiles = perfiles + constants.stringToFloat(x.Cells[4].Value.ToString());
                    }
                }
                else
                {
                    perfiles = perfiles + constants.stringToFloat(x.Cells[4].Value.ToString());
                }
            }

            foreach (DataGridViewRow x in dataGridView2.Rows)
            {
                if (seccion >= 0)
                {
                    if(seccion == constants.stringToInt(x.Cells[4].Value.ToString()))
                    {
                        cristales = cristales + constants.stringToFloat(x.Cells[3].Value.ToString());
                    }
                }
                else
                {
                    cristales = cristales + constants.stringToFloat(x.Cells[3].Value.ToString());
                }
            }

            foreach (DataGridViewRow x in dataGridView3.Rows)
            {
                if(seccion >= 0)
                {
                    if(seccion == constants.stringToInt(x.Cells[5].Value.ToString()))
                    {
                        herrajes = herrajes + constants.stringToFloat(x.Cells[4].Value.ToString());
                    }
                }
                else
                {
                    herrajes = herrajes + constants.stringToFloat(x.Cells[4].Value.ToString());
                }
            }

            foreach (DataGridViewRow x in dataGridView4.Rows)
            {
                if(seccion >= 0)
                {
                    if(seccion == constants.stringToInt(x.Cells[6].Value.ToString()))
                    {
                        otros = otros + constants.stringToFloat(x.Cells[4].Value.ToString());
                    }
                }
                else
                {
                    otros = otros + constants.stringToFloat(x.Cells[4].Value.ToString());
                }
            }

            switch (articulo)
            {
                case 0:
                    return perfiles + cristales + herrajes + otros;
                case 1:
                    return perfiles;
                case 2:
                    return cristales;
                case 3:
                    return herrajes;
                case 4:
                    return otros;
                default:
                    return 0;
            }
        }

        private void deserializeParameters(string parameters)
        {
            string[] param = parameters.Split(',');

            if (param.Length == 4)
            {
                if (constants.stringToInt(param[0]) > 0 || constants.stringToInt(param[1]) > 0 || constants.stringToInt(param[2]) > 0 || constants.stringToInt(param[3]) > 0)
                {
                    if (constants.user_access != 6)
                    {
                        //BLOQUEAR PARAMETROS
                        textBox5.Enabled = false;
                        textBox6.Enabled = false;
                        textBox3.Enabled = false;
                        textBox7.Enabled = false;
                        checkBox8.Checked = false;
                        //
                    }
                    else
                    {
                        checkBox8.Checked = true;
                    }
                }
                if (constants.stringToInt(param[0]) > 0)
                {
                    textBox5.Text = constants.stringToInt(param[0]).ToString();
                }
                if (constants.stringToInt(param[1]) > 0)
                {
                    textBox6.Text = constants.stringToInt(param[1]).ToString();
                }
                if (constants.stringToInt(param[2]) > 0)
                {
                    textBox3.Text = constants.stringToInt(param[2]).ToString();
                }
                if (constants.stringToInt(param[3]) > 0)
                {
                    textBox7.Text = constants.stringToInt(param[3]).ToString();
                }
            }
        }   

        private void loadModulo(int module_id)
        {
            listas_entities_pva listas = new listas_entities_pva();

            var modulo = (from x in listas.modulos where x.id == module_id select x).SingleOrDefault();

            if(modulo != null)
            {
                deserializeParameters(modulo.parametros);
                label6.Text = id_cotizacion > -1 ? label6.Text : modulo.clave;
                label7.Text = modulo.articulo;
                label8.Text = modulo.linea;
                getLineaCompleta(modulo.linea);
                label9.Text = modulo.secciones.ToString();
                loadDiseño((int)modulo.id_diseño, listas);
                cs = (bool)modulo.cs;                
                float count = 0;
                string buffer = "";
                string dir = "";
                string c_clave = "";
                int seccion = -1;
                getReglas(modulo.reglas);
                Text = modulo.articulo;
                if (modulo.linea == "CANCEL BAÑO")
                {
                    constants.setImage("series", "CB", "png", pictureBox3);
                }
                else
                {
                    constants.setImage("series", modulo.linea, "png", pictureBox3);
                }

                //aluminio
                foreach (char alm in modulo.id_aluminio)
                {
                    if (alm != ',')
                    {
                        if (alm == ':')
                        {
                            c_clave = buffer;
                            buffer = "";
                            continue;
                        }
                        if (alm == '-')
                        {
                            count = constants.stringToFloat(buffer);
                            buffer = "";
                            continue;
                        }
                        if (alm == '$')
                        {
                            dir = buffer;
                            buffer = "";
                            continue;
                        }
                        buffer = buffer + alm.ToString();
                    }
                    else
                    {
                        seccion = constants.stringToInt(buffer);
                        buffer = "";

                        var aluminio = (from c in listas.perfiles where c.clave == c_clave select c).SingleOrDefault();

                        if (aluminio != null)
                        {
                            dataGridView1.Rows.Add("Perfil", aluminio.id, c_clave, aluminio.articulo, count, dir, seccion, "");
                        }
                    }
                }
                //             

                buffer = "";
                count = 0;
                seccion = -1;
                c_clave = "";

                //cristales
                foreach (char cri in modulo.clave_vidrio)
                {
                    if (cri != ',')
                    {
                        if (cri == ':')
                        {
                            c_clave = buffer;
                            buffer = "";
                            continue;
                        }
                        if (cri == '$')
                        {
                            count = constants.stringToFloat(buffer);
                            buffer = "";
                            continue;
                        }
                        buffer = buffer + cri.ToString();
                    }
                    else
                    {
                        seccion = constants.stringToInt(buffer);
                        buffer = "";

                        var cristal = (from c in listas.lista_costo_corte_e_instalado where c.clave == c_clave select c).SingleOrDefault();

                        if (cristal != null)
                        {
                            dataGridView2.Rows.Add("Cristal", c_clave, cristal.articulo, count, seccion);
                        }
                    }
                }
                //

                buffer = "";
                count = 0;
                id = 0;
                seccion = -1;
                c_clave = "";

                //herrajes
                foreach (char h in modulo.id_herraje)
                {
                    if (h != ',')
                    {
                        if (h == ':')
                        {
                            c_clave = buffer;
                            buffer = "";
                            continue;
                        }
                        if (h == '$')
                        {
                            count = constants.stringToFloat(buffer);
                            buffer = "";
                            continue;
                        }
                        buffer = buffer + h.ToString();
                    }
                    else
                    {
                        seccion = constants.stringToInt(buffer);
                        buffer = "";

                        var herraje = (from c in listas.herrajes where c.clave == c_clave select c).SingleOrDefault();

                        if (herraje != null)
                        {
                            dataGridView3.Rows.Add("Herraje", herraje.id, c_clave, herraje.articulo, count, seccion, herraje.color);
                        }
                    }
                }
                //

                buffer = "";
                count = 0;
                id = 0;
                dir = "";
                seccion = -1;
                c_clave = "";

                //otros
                foreach (char o in modulo.id_otros)
                {
                    if (o != ',')
                    {
                        if (o == ':')
                        {
                            c_clave = buffer;
                            buffer = "";
                            continue;
                        }
                        if (o == '-')
                        {
                            count = constants.stringToFloat(buffer);
                            buffer = "";
                            continue;
                        }
                        if (o == '$')
                        {
                            dir = buffer;
                            buffer = "";
                            continue;
                        }
                        buffer = buffer + o.ToString();
                    }
                    else
                    {
                        seccion = constants.stringToInt(buffer);
                        buffer = "";

                        var otros = (from c in listas.otros where c.clave == c_clave select c).SingleOrDefault();

                        if (otros != null)
                        {
                            dataGridView4.Rows.Add("Otros", otros.id, c_clave, otros.articulo, count, dir, seccion, otros.color);
                        }
                    }
                }
            }
            //Check  
            if (dataGridView1.RowCount == 0)
            {
                checkBox11.Checked = false;
            }           
            //----------------------------------->
            if (dataGridView2.RowCount == 0)
            {
                checkBox12.Checked = false;
            }            
            //----------------------------------->
            if (dataGridView3.RowCount == 0)
            {
                checkBox13.Checked = false;
            }            
            //----------------------------------->
            if (dataGridView4.RowCount == 0)
            {
                checkBox14.Checked = false;
            }            
        }      

        private float[] getMedidas(int seccion)
        {
            float[] r = new float[2];
            foreach(DataGridViewRow x in dataGridView5.Rows)
            {
                if(constants.stringToInt(x.Cells[0].Value.ToString()) == seccion)
                {
                    r[0] = constants.stringToFloat(x.Cells[2].Value.ToString()) / 1000f;
                    r[1] = constants.stringToFloat(x.Cells[3].Value.ToString()) / 1000f;                    
                    break;
                }
            }
            return r;
        }

        private void acomodarDesglose()
        {
            dataGridView6.Sort(dataGridView6.Columns[0], ListSortDirection.Ascending);
            foreach(DataGridViewRow x in dataGridView6.Rows)
            {
                if(x.Cells[0].Value.ToString() == "Perfil")
                {
                    x.DefaultCellStyle.BackColor = Color.LightGray;
                }
                else if(x.Cells[0].Value.ToString() == "Cristal")
                {
                    x.DefaultCellStyle.BackColor = Color.LightBlue;
                }
                else if (x.Cells[0].Value.ToString() == "Herraje")
                {
                    x.DefaultCellStyle.BackColor = Color.LightYellow;
                }
                else if (x.Cells[0].Value.ToString() == "Otros")
                {
                    x.DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }
        }

        private void calcularCostoModulo()
        {
            listas_entities_pva listas = new listas_entities_pva();
            float tot_aluminio = 0, tot_vidrio = 0, tot_herraje = 0, tot_otros = 0, total = 0;
            float desperdicio = constants.stringToFloat(textBox5.Text) / 100f;
            float flete = constants.stringToFloat(textBox6.Text) / 100f;
            float utilidad = constants.stringToFloat(textBox7.Text) / 100f;
            float mano_obra = constants.stringToFloat(textBox3.Text) / 100f;
            float cantidad = constants.stringToFloat(textBox4.Text);
            float ext = 0;
            float count = 0;
            float pf = 0;
            float cr = 0;
            float hrr = 0;
            float otr = 0;

            //borrar desglose anterior
            dataGridView6.Rows.Clear();
            calcularTramosLargos(listas);
            checkWeight();

            for (int i = c; i <= constants.stringToInt(label9.Text); i++)
            {
                //Perfiles
                if (checkBox11.Checked)
                {
                    foreach (DataGridViewRow x in dataGridView1.Rows)
                    {
                        if (constants.stringToInt(x.Cells[6].Value.ToString()) == i)
                        {
                            int perfil_id = constants.stringToInt(x.Cells[1].Value.ToString());
                            var perfil = (from v in listas.perfiles where v.id == perfil_id select v).SingleOrDefault();

                            if (perfil != null)
                            {
                                count = constants.stringToFloat(x.Cells[4].Value.ToString());
                                string clr = x.Cells[7].Value.ToString();
                                var color = (from u in listas.colores_aluminio where u.clave == clr select u).SingleOrDefault();

                                if (color == null)
                                {
                                    if (clr == "crudo")
                                    {
                                        pf = (float)perfil.crudo;
                                        if (x.Cells[5].Value.ToString() == "largo")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.crudo / perfil.largo) * getMedidas(i)[0]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[0]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                        else if (x.Cells[5].Value.ToString() == "alto")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.crudo / perfil.largo) * getMedidas(i)[1]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                    }
                                    else if (clr == "blanco")
                                    {
                                        pf = (float)perfil.blanco;
                                        if (x.Cells[5].Value.ToString() == "largo")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.blanco / perfil.largo) * getMedidas(i)[0]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[0]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                        else if (x.Cells[5].Value.ToString() == "alto")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.blanco / perfil.largo) * getMedidas(i)[1]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                    }
                                    else if (clr == "hueso")
                                    {
                                        pf = (float)perfil.hueso;
                                        if (x.Cells[5].Value.ToString() == "largo")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.hueso / perfil.largo) * getMedidas(i)[0]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[0]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                        else if (x.Cells[5].Value.ToString() == "alto")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.hueso / perfil.largo) * getMedidas(i)[1]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                    }
                                    else if (clr == "champagne")
                                    {
                                        pf = (float)perfil.champagne;
                                        if (x.Cells[5].Value.ToString() == "largo")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.champagne / perfil.largo) * getMedidas(i)[0]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[0]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                        else if (x.Cells[5].Value.ToString() == "alto")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.champagne / perfil.largo) * getMedidas(i)[1]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                    }
                                    else if (clr == "gris")
                                    {
                                        pf = (float)perfil.gris;
                                        if (x.Cells[5].Value.ToString() == "largo")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.gris / perfil.largo) * getMedidas(i)[0]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[0]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                        else if (x.Cells[5].Value.ToString() == "alto")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.gris / perfil.largo) * getMedidas(i)[1]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                    }
                                    else if (clr == "negro")
                                    {
                                        pf = (float)perfil.negro;
                                        if (x.Cells[5].Value.ToString() == "largo")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.negro / perfil.largo) * getMedidas(i)[0]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[0]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                        else if (x.Cells[5].Value.ToString() == "alto")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.negro / perfil.largo) * getMedidas(i)[1]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                    }
                                    else if (clr == "brillante")
                                    {
                                        pf = (float)perfil.brillante;
                                        if (x.Cells[5].Value.ToString() == "largo")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.brillante / perfil.largo) * getMedidas(i)[0]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[0]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                        else if (x.Cells[5].Value.ToString() == "alto")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.brillante / perfil.largo) * getMedidas(i)[1]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                    }
                                    else if (clr == "natural")
                                    {
                                        pf = (float)perfil.natural_1;
                                        if (x.Cells[5].Value.ToString() == "largo")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.natural_1 / perfil.largo) * getMedidas(i)[0]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[0]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                        else if (x.Cells[5].Value.ToString() == "alto")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.natural_1 / perfil.largo) * getMedidas(i)[1]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                    }
                                    else if (clr == "madera")
                                    {
                                        pf = (float)perfil.madera;
                                        if (x.Cells[5].Value.ToString() == "largo")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.madera / perfil.largo) * getMedidas(i)[0]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[0]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                        else if (x.Cells[5].Value.ToString() == "alto")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.madera / perfil.largo) * getMedidas(i)[1]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                    }
                                    else if (clr == "chocolate")
                                    {
                                        pf = (float)perfil.chocolate;
                                        if (x.Cells[5].Value.ToString() == "largo")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.chocolate / perfil.largo) * getMedidas(i)[0]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[0]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                        else if (x.Cells[5].Value.ToString() == "alto")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.chocolate / perfil.largo) * getMedidas(i)[1]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                    }
                                    else if (clr == "acero_inox")
                                    {
                                        pf = (float)perfil.acero_inox;
                                        if (x.Cells[5].Value.ToString() == "largo")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.acero_inox / perfil.largo) * getMedidas(i)[0]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[0]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                        else if (x.Cells[5].Value.ToString() == "alto")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.acero_inox / perfil.largo) * getMedidas(i)[1]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                    }
                                    else if (clr == "bronce")
                                    {
                                        pf = (float)perfil.bronce;
                                        if (x.Cells[5].Value.ToString() == "largo")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.bronce / perfil.largo) * getMedidas(i)[0]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[0]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                        else if (x.Cells[5].Value.ToString() == "alto")
                                        {
                                            tot_aluminio = tot_aluminio + (float)(((perfil.bronce / perfil.largo) * getMedidas(i)[1]) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), Math.Round((float)(pf / perfil.largo), 2), Math.Round((float)(((pf / perfil.largo) * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                    }
                                }
                                else
                                {
                                    pf = (float)perfil.crudo;
                                    if (x.Cells[5].Value.ToString() == "largo")
                                    {
                                        ext = (float)(getMedidas(i)[0] * color.costo_extra_ml);
                                        tot_aluminio = tot_aluminio + (float)((((perfil.crudo / perfil.largo) * getMedidas(i)[0]) + (((getMedidas(i)[0] * (perfil.perimetro_dm2_ml / 100) * (color.precio)) + ext))) * count);
                                        dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", Math.Round((double)((pf / perfil.largo) + ((((perfil.perimetro_dm2_ml / 100) * (color.precio)) + color.costo_extra_ml))), 2), Math.Round((double)((((pf / perfil.largo) * getMedidas(i)[0]) + (((getMedidas(i)[0] * (perfil.perimetro_dm2_ml / 100) * (color.precio)) + ext))) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                    }
                                    else if (x.Cells[5].Value.ToString() == "alto")
                                    {
                                        ext = (float)(getMedidas(i)[1] * color.costo_extra_ml);
                                        tot_aluminio = tot_aluminio + (float)((((perfil.crudo / perfil.largo) * getMedidas(i)[1]) + (((getMedidas(i)[1] * (perfil.perimetro_dm2_ml / 100) * (color.precio)) + ext))) * count);
                                        dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), Math.Round((double)((pf / perfil.largo) + ((((perfil.perimetro_dm2_ml / 100) * (color.precio)) + color.costo_extra_ml))), 2), Math.Round((double)((((pf / perfil.largo) * getMedidas(i)[1]) + (((getMedidas(i)[1] * (perfil.perimetro_dm2_ml / 100) * (color.precio)) + ext))) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                    }
                                }
                            }
                        }
                    }
                }
                //                

                //Herrajes
                if (checkBox13.Checked)
                {
                    foreach (DataGridViewRow x in dataGridView3.Rows)
                    {
                        if (constants.stringToInt(x.Cells[5].Value.ToString()) == i)
                        {
                            int herraje_id = constants.stringToInt(x.Cells[1].Value.ToString());
                            var herraje = (from v in listas.herrajes where v.id == herraje_id select v).SingleOrDefault();

                            if (herraje != null)
                            {
                                hrr = (float)Math.Round((float)herraje.precio, 2);
                                tot_herraje = tot_herraje + (float)(herraje.precio * constants.stringToFloat(x.Cells[4].Value.ToString()));
                                dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", "", hrr, Math.Round((float)((hrr) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                            }
                        }
                    }
                }
                //

                //Otros materiales
                if (checkBox14.Checked)
                {
                    foreach (DataGridViewRow x in dataGridView4.Rows)
                    {
                        if (constants.stringToInt(x.Cells[6].Value.ToString()) == i)
                        {
                            int otro_id = constants.stringToInt(x.Cells[1].Value.ToString());
                            var otro = (from v in listas.otros where v.id == otro_id select v).SingleOrDefault();

                            if (otro != null)
                            {
                                count = constants.stringToFloat(x.Cells[4].Value.ToString());
                                otr = (float)Math.Round((float)otro.precio, 2);
                                if (otro.largo > 0 && otro.alto <= 0)
                                {
                                    if (x.Cells[5].Value.ToString() == "largo")
                                    {
                                        tot_otros = tot_otros + (float)(otro.precio * getMedidas(i)[0] * count);
                                        dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", otr, Math.Round((float)((otr) * getMedidas(i)[0] * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                    }
                                    else if (x.Cells[5].Value.ToString() == "alto")
                                    {
                                        tot_otros = tot_otros + (float)(otro.precio * getMedidas(i)[1] * count);
                                        dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), otr, Math.Round((float)((otr) * getMedidas(i)[1] * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                    }
                                }
                                else if (otro.largo <= 0 && otro.alto > 0)
                                {
                                    if (x.Cells[5].Value.ToString() == "largo")
                                    {
                                        tot_otros = tot_otros + (float)(otro.precio * getMedidas(i)[0] * count);
                                        dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), "", otr, Math.Round((float)((otr) * getMedidas(i)[0] * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                    }
                                    else if (x.Cells[5].Value.ToString() == "alto")
                                    {
                                        tot_otros = tot_otros + (float)(otro.precio * getMedidas(i)[1] * count);
                                        dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", (getMedidas(i)[1] * 1000).ToString(), otr, Math.Round((float)((otr) * getMedidas(i)[1] * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                    }
                                }
                                else if (otro.largo > 0 && otro.alto > 0)
                                {
                                    if (cs == false)
                                    {
                                        if (rows > 0 && columns > 0)
                                        {
                                            tot_otros = tot_otros + (float)((otro.precio * (getMedidas(i)[0] / columns) * (getMedidas(i)[1] / rows)) * count);
                                            dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), Math.Round(((getMedidas(i)[0] / columns) * 1000), 2).ToString(), Math.Round(((getMedidas(i)[1] / rows) * 1000), 2).ToString(), otr, Math.Round((float)((otr) * (getMedidas(i)[0] / columns) * (getMedidas(i)[1] / rows) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                        }
                                    }
                                    else
                                    {
                                        tot_otros = tot_otros + (float)(otro.precio * getMedidas(i)[0] * getMedidas(i)[1] * count);
                                        dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), (getMedidas(i)[1] * 1000).ToString(), otr, Math.Round((float)((otr) * getMedidas(i)[0] * getMedidas(i)[1] * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                    }
                                }
                                else
                                {
                                    tot_otros = tot_otros + (float)(otro.precio * count);
                                    dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[4].Value.ToString(), "", "", otr, Math.Round((float)((otr) * constants.stringToFloat(x.Cells[4].Value.ToString())), 2));
                                }
                            }
                        }
                    }
                }
                //

                //cristales
                if (checkBox12.Checked)
                {
                    foreach (DataGridViewRow x in dataGridView2.Rows)
                    {
                        if (constants.stringToInt(x.Cells[4].Value.ToString()) == i)
                        {
                            string cristal_clave = x.Cells[1].Value.ToString();
                            var cristal = (from v in listas.lista_costo_corte_e_instalado where v.clave == cristal_clave select v).SingleOrDefault();

                            if (cristal != null)
                            {
                                cr = (float)Math.Round((float)cristal.costo_corte_m2, 2);
                                if (cs == false)
                                {
                                    if (rows > 0 && columns > 0)
                                    {
                                        tot_vidrio = tot_vidrio + (float)((cristal.costo_corte_m2 * (getMedidas(i)[0] / columns) * (getMedidas(i)[1] / rows)) * constants.stringToFloat(x.Cells[3].Value.ToString()));
                                        dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), "", x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), Math.Round(((getMedidas(i)[0] / columns) * 1000), 2).ToString(), Math.Round(((getMedidas(i)[1] / rows) * 1000), 2).ToString(), cr, Math.Round((float)(((cr) * (getMedidas(i)[0] / columns) * (getMedidas(i)[1] / rows)) * constants.stringToFloat(x.Cells[3].Value.ToString())), 2));
                                    }
                                }
                                else
                                {
                                    tot_vidrio = tot_vidrio + (float)((cristal.costo_corte_m2 * getMedidas(i)[0] * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[3].Value.ToString()));
                                    dataGridView6.Rows.Add(x.Cells[0].Value.ToString(), "", x.Cells[1].Value.ToString(), x.Cells[2].Value.ToString(), x.Cells[3].Value.ToString(), (getMedidas(i)[0] * 1000).ToString(), (getMedidas(i)[1] * 1000).ToString(), cr, Math.Round((float)(((cr) * getMedidas(i)[0] * getMedidas(i)[1]) * constants.stringToFloat(x.Cells[3].Value.ToString())), 2));
                                }
                            }
                        }
                    }
                }
                //
            }
            //Resultados
            //---> Desglose
            int t_cant = constants.stringToInt(textBox4.Text);            
            foreach (DataGridViewRow x in dataGridView6.Rows)
            {
                x.Cells[4].Value = Math.Round(constants.stringToFloat(x.Cells[4].Value.ToString()) * t_cant, 2);
                x.Cells[8].Value = Math.Round(constants.stringToFloat(x.Cells[8].Value.ToString()) * t_cant, 2);
            }    

            total = (tot_aluminio + (tot_aluminio * desperdicio));
            textBox9.Text = "$" + Math.Round(total * t_cant, 2).ToString("0.00");
            total = total + tot_herraje + tot_otros + tot_vidrio + getTotalNewCostos(new_costos);
            textBox10.Text = "$" + Math.Round(tot_herraje * t_cant, 2).ToString("0.00");
            textBox11.Text = "$" + Math.Round(tot_vidrio * t_cant, 2).ToString("0.00");
            textBox12.Text = "$" + Math.Round(tot_otros * t_cant, 2).ToString("0.00") + " + $" + Math.Round(getTotalNewCostos(new_costos) * t_cant, 2);
            //--------------------------------->
            total = total + (total * flete);
            total = total + (total * mano_obra);
            total = total + (total * utilidad);
            label42.Text = "Peso: " + Math.Round(peso_aluminio, 2) + " KG.";
            if (((dataGridView1.RowCount == 0 || arePerfiles() == false) && (comboBox1.Text == "" || comboBox3.Text == "")) || (dataGridView1.RowCount > 0 && (comboBox1.Text != "" || comboBox3.Text != "")))
            {
                float _tot = getTotalNewCostosTotal(new_costos);
                float t_tot = (total * cantidad) + _tot;
                //Tasa Cero
                if (constants.tasa_cero)
                {
                    if (!constants.iva_desglosado)
                    {
                        //Get IVA
                        float _iva = constants.getPropiedadesModel();
                        float _ut = t_tot / (utilidad + 1);
                        float _nt = t_tot - _ut;
                        t_tot = (_ut * _iva) + _nt;
                    }
                }
                //----------------------------------------------------->
                textBox13.Text = Math.Round(t_tot, 2).ToString("0.00");
                label12.Text = Math.Round(t_tot * constants.iva, 2).ToString("n");
                label44.Text = _tot > 0 ? "+ ($" + _tot + ")" : string.Empty;
            }
            else
            {
                textBox13.Text = "0.00";
                label12.Text = "0.00";
            }
            if (label18.Text == "CM")
            {
                if (checkBox17.Checked == false && checkBox18.Checked == false)
                {
                    textBox13.Text = "0.00";
                    label12.Text = "0.00";
                }
            }
            acomodarDesglose();
            //
        }        

        private float getTotalNewCostos(List<string> new_costos)
        {
            float r = 0;
            string[] t = null;
            foreach(string x in new_costos)
            {
                t = x.Split(',');
                if(t.Length == 4)
                {
                    if (t[3] != "Total")
                    {
                        r = constants.stringToFloat(t[2]) + r;
                    }
                }
            }
            return r;
        }

        private float getTotalNewCostosTotal(List<string> new_costos)
        {
            float r = 0;
            string[] t = null;
            foreach (string x in new_costos)
            {
                t = x.Split(',');
                if (t.Length == 4)
                {
                    if (t[3] == "Total")
                    {
                        r = constants.stringToFloat(t[2]) + r;
                    }
                }
            }
            return r;
        }

        //SACS
        private bool IACheckPefilesAcabados(string clave, string acabado)
        {
            bool r = true;

            if (acabado != "")
            {
                listas_entities_pva lista = new listas_entities_pva();

                var perfil = (from u in lista.perfiles where u.clave == clave select u).SingleOrDefault();
                var color = (from u in lista.colores_aluminio where u.clave == acabado select u).SingleOrDefault();

                if (color == null)
                {
                    if (perfil != null)
                    {
                        if (acabado == "crudo")
                        {
                            if (perfil.crudo <= 0)
                            {
                                r = false;
                            }
                        }
                        else if (acabado == "blanco")
                        {
                            if (perfil.blanco <= 0)
                            {
                                r = false;
                            }
                        }
                        else if (acabado == "hueso")
                        {
                            if (perfil.hueso <= 0)
                            {
                                r = false;
                            }
                        }
                        else if (acabado == "champagne")
                        {
                            if (perfil.champagne <= 0)
                            {
                                r = false;
                            }
                        }
                        else if (acabado == "gris")
                        {
                            if (perfil.gris <= 0)
                            {
                                r = false;
                            }
                        }
                        else if (acabado == "negro")
                        {
                            if (perfil.negro <= 0)
                            {
                                r = false;
                            }
                        }
                        else if (acabado == "brillante")
                        {
                            if (perfil.brillante <= 0)
                            {
                                r = false;
                            }
                        }
                        else if (acabado == "natural")
                        {
                            if (perfil.natural_1 <= 0)
                            {
                                r = false;
                            }
                        }
                        else if (acabado == "madera")
                        {
                            if (perfil.madera <= 0)
                            {
                                r = false;
                            }
                        }
                        else if (acabado == "chocolate")
                        {
                            if (perfil.chocolate <= 0)
                            {
                                r = false;
                            }
                        }
                        else if (acabado == "acero_inox")
                        {
                            if (perfil.acero_inox <= 0)
                            {
                                r = false;
                            }
                        }
                        else if (acabado == "bronce")
                        {
                            if (perfil.bronce <= 0)
                            {
                                r = false;
                            }
                        }
                    }
                }
                else
                {
                    if (perfil != null)
                    {
                        if (perfil.perimetro_dm2_ml <= 0)
                        {
                            r = false;
                        }
                        else if (perfil.crudo <= 0)
                        {
                            r = false;
                        }
                    }
                }
            }
            else
            {
                r = false;
            }
            return r;
        }

        //Normal checking
        private void checkPefilesAcabados()
        {
            int id = 0;
            string acabado = string.Empty;
            string result_1 = string.Empty;
            string result_2 = string.Empty;
            string result_3 = string.Empty;

            listas_entities_pva lista = new listas_entities_pva();

            foreach (DataGridViewRow x in dataGridView1.Rows)
            {
                if (constants.stringToFloat(x.Cells[4].Value.ToString()) > 0)
                {
                    id = constants.stringToInt(x.Cells[1].Value.ToString());
                    acabado = x.Cells[7].Value.ToString();

                    var perfil = (from u in lista.perfiles where u.id == id select u).SingleOrDefault();
                    var color = (from u in lista.colores_aluminio where u.clave == acabado select u).SingleOrDefault();

                    if (color == null)
                    {
                        if (perfil != null)
                        {
                            if (acabado == "crudo")
                            {
                                if (perfil.crudo <= 0)
                                {
                                    if (checkBox9.Checked == true)
                                    {
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        result_1 = perfil.id.ToString();
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    x.Cells[7].Style.BackColor = Color.LightGreen;
                                }
                            }
                            else if (acabado == "blanco")
                            {
                                if (perfil.blanco <= 0)
                                {
                                    if (checkBox9.Checked == true)
                                    {
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        result_1 = perfil.id.ToString();
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    x.Cells[7].Style.BackColor = Color.LightGreen;
                                }
                            }
                            else if (acabado == "hueso")
                            {
                                if (perfil.hueso <= 0)
                                {
                                    if (checkBox9.Checked == true)
                                    {
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        result_1 = perfil.id.ToString();
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    x.Cells[7].Style.BackColor = Color.LightGreen;
                                }
                            }
                            else if (acabado == "champagne")
                            {
                                if (perfil.champagne <= 0)
                                {
                                    if (checkBox9.Checked == true)
                                    {
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        result_1 = perfil.id.ToString();
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    x.Cells[7].Style.BackColor = Color.LightGreen;
                                }
                            }
                            else if (acabado == "gris")
                            {
                                if (perfil.gris <= 0)
                                {
                                    if (checkBox9.Checked == true)
                                    {
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        result_1 = perfil.id.ToString();
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    x.Cells[7].Style.BackColor = Color.LightGreen;
                                }
                            }
                            else if (acabado == "negro")
                            {
                                if (perfil.negro <= 0)
                                {
                                    if (checkBox9.Checked == true)
                                    {
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        result_1 = perfil.id.ToString();
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    x.Cells[7].Style.BackColor = Color.LightGreen;
                                }
                            }
                            else if (acabado == "brillante")
                            {
                                if (perfil.brillante <= 0)
                                {
                                    if (checkBox9.Checked == true)
                                    {
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        result_1 = perfil.id.ToString();
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    x.Cells[7].Style.BackColor = Color.LightGreen;
                                }
                            }
                            else if (acabado == "natural")
                            {
                                if (perfil.natural_1 <= 0)
                                {
                                    if (checkBox9.Checked == true)
                                    {
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        result_1 = perfil.id.ToString();
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    x.Cells[7].Style.BackColor = Color.LightGreen;
                                }
                            }
                            else if (acabado == "madera")
                            {
                                if (perfil.madera <= 0)
                                {
                                    if (checkBox9.Checked == true)
                                    {
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        result_1 = perfil.id.ToString();
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    x.Cells[7].Style.BackColor = Color.LightGreen;
                                }
                            }
                            else if (acabado == "chocolate")
                            {
                                if (perfil.chocolate <= 0)
                                {
                                    if (checkBox9.Checked == true)
                                    {
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        result_1 = perfil.id.ToString();
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    x.Cells[7].Style.BackColor = Color.LightGreen;
                                }
                            }
                            else if (acabado == "acero_inox")
                            {
                                if (perfil.acero_inox <= 0)
                                {
                                    if (checkBox9.Checked == true)
                                    {
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        result_1 = perfil.id.ToString();
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    x.Cells[7].Style.BackColor = Color.LightGreen;
                                }
                            }
                            else if (acabado == "bronce")
                            {
                                if (perfil.bronce <= 0)
                                {
                                    if (checkBox9.Checked == true)
                                    {
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                    else
                                    {
                                        result_1 = perfil.id.ToString();
                                        x.Cells[7].Style.BackColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    x.Cells[7].Style.BackColor = Color.LightGreen;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (perfil != null)
                        {
                            if (perfil.perimetro_dm2_ml <= 0)
                            {
                                if (checkBox9.Checked == true)
                                {
                                    x.Cells[7].Style.BackColor = Color.Red;
                                }
                                else
                                {
                                    result_2 = perfil.id.ToString();
                                    x.Cells[7].Style.BackColor = Color.Red;
                                }
                            }
                            else if (perfil.crudo <= 0)
                            {
                                if (checkBox9.Checked == true)
                                {
                                    x.Cells[7].Style.BackColor = Color.Red;
                                }
                                else
                                {
                                    result_3 = perfil.id.ToString();
                                    x.Cells[7].Style.BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                x.Cells[7].Style.BackColor = Color.LightGreen;
                            }
                        }
                    }
                }
            }
            //------------------------------------------------------------------------------------------>
            if(result_1.Length > 0)
            {
                MessageBox.Show("El artículo con el ID " + result_1 + " no tiene disponible esté acabado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if(result_2.Length > 0)
            {
                MessageBox.Show("El artículo con el ID " + result_2 + " no tiene disponible un perímetro anodizable.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if(result_3.Length > 0)
            {
                MessageBox.Show("El artículo con el ID " + result_3 + " no tiene disponible un acabado en crudo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }          
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            setAcabadoLista();
        }

        private void setAcabadoLista()
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                if (dataGridView1.RowCount > 0 && arePerfiles() == true)
                {
                    comboBox3.SelectedIndex = -1;
                    label37.Text = comboBox1.Text.ToUpper();
                    if (checkBox9.Checked == false)
                    {
                        if (checkBox20.Checked)
                        {
                            //using SACS
                            string acabado_op = constants.IASetAcabado(comboBox1.Text);

                            foreach (DataGridViewRow x in dataGridView1.Rows)
                            {
                                if (constants.stringToFloat(x.Cells[4].Value.ToString()) > 0)
                                {
                                    if (IACheckPefilesAcabados(x.Cells[2].Value.ToString(), comboBox1.Text) == true)
                                    {
                                        x.Cells[7].Value = comboBox1.Text;
                                    }
                                    else if (IACheckPefilesAcabados(x.Cells[2].Value.ToString(), acabado_op) == true)
                                    {
                                        x.Cells[7].Value = acabado_op;
                                    }
                                    else
                                    {
                                        x.Cells[7].Value = comboBox1.Text;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (DataGridViewRow x in dataGridView1.Rows)
                            {
                                if (constants.stringToFloat(x.Cells[4].Value.ToString()) > 0)
                                {
                                    x.Cells[7].Value = comboBox1.Text;
                                }
                            }
                        }
                    }
                    for (int i = c; i <= constants.stringToInt(label9.Text); i++)
                    {
                        getSeccionesReady(i);
                    }
                    for (int i = 0; i < panel.Controls.Count; i++)
                    {
                        constants.setBackgroundImg("acabados_perfil", comboBox1.Text, "jpg", panel.Controls[i]);
                    }
                    calcularCostoModulo();
                    if (checkBox1.Checked == true)
                    {
                        constants.setBackgroundImg("acabados_perfil", comboBox1.Text, "jpg", panel);
                    }
                    constants.setImage("acabados_perfil", comboBox1.Text, "jpg", pictureBox2);
                    checkPefilesAcabados();
                }
                else
                {
                    comboBox1.SelectedIndex = -1;
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataGridView3.RowCount > 0)
            {
                foreach (DataGridViewRow x in dataGridView3.Rows)
                {
                    x.Cells[7].Value = comboBox2.Text;
                }

                for (int i = c; i <= constants.stringToInt(label9.Text); i++)
                {
                    getSeccionesReady(i);
                }
                calcularCostoModulo();
            }
        }

        private string getDimensions()
        {
            string r = string.Empty;
            foreach (DataGridViewRow x in dataGridView5.Rows)
            {
                   r =  r + x.Cells[2].Value.ToString() + "," + x.Cells[3].Value.ToString() + ",";
            }           
            return r;
        }

        private string getClavesCristal()
        {
            string r = string.Empty;
            if (checkBox12.Checked)
            {
                foreach (DataGridViewRow x in dataGridView2.Rows)
                {
                    if (x.Cells[0].Style.BackColor != Color.Yellow)
                    {
                        r = r + x.Cells[1].Value.ToString() + "-" + x.Cells[3].Value.ToString() + ",";                       
                    }
                }
            }
            return r;
        }

        private string getClavesPerfiles()
        {
            string r = string.Empty;
            if (checkBox11.Checked)
            {
                foreach (DataGridViewRow x in dataGridView1.Rows)
                {
                    if (x.Cells[0].Style.BackColor != Color.Yellow)
                    {
                        r = r + x.Cells[2].Value.ToString() + "-" + x.Cells[4].Value.ToString() + "-" + x.Cells[7].Value.ToString() + ",";
                    }
                }
            }
            return r;
        }

        private string getClavesHerrajes()
        {
            string r = string.Empty;
            if (checkBox13.Checked)
            {
                foreach (DataGridViewRow x in dataGridView3.Rows)
                {
                    if (x.Cells[0].Style.BackColor != Color.Yellow)
                    {
                        r = r + x.Cells[2].Value.ToString() + "-" + x.Cells[4].Value.ToString() + ",";
                    }
                }
            }
            return r;
        }

        private string getClavesOtros()
        {
            string r = string.Empty;
            if (checkBox14.Checked)
            {
                foreach (DataGridViewRow x in dataGridView4.Rows)
                {
                    if (x.Cells[0].Style.BackColor != Color.Yellow)
                    {
                        r = r + x.Cells[2].Value.ToString() + "-" + x.Cells[4].Value.ToString() + ",";
                    }
                }
            }
            return r;
        }

        private string getNewItems()
        {
            string r = string.Empty;
            foreach(DataGridViewRow x in dataGridView1.Rows)
            {
                if(x.Cells[0].Style.BackColor == Color.Yellow)
                {
                    r = r + "1," + x.Cells[2].Value.ToString() + "," + x.Cells[4].Value.ToString() + "," + x.Cells[5].Value.ToString() + "," + x.Cells[6].Value.ToString() + "," + x.Cells[7].Value.ToString() + ";";
                }
            }
            foreach (DataGridViewRow x in dataGridView2.Rows)
            {
                if (x.Cells[0].Style.BackColor == Color.Yellow)
                {
                    r = r + "2," + x.Cells[1].Value.ToString() + "," + x.Cells[3].Value.ToString() + "," + "," + x.Cells[4].Value.ToString() + "," + ";";
                }
            }
            foreach (DataGridViewRow x in dataGridView3.Rows)
            {
                if (x.Cells[0].Style.BackColor == Color.Yellow)
                {
                    r = r + "3," + x.Cells[2].Value.ToString() + "," + x.Cells[4].Value.ToString() + "," + "," + x.Cells[5].Value.ToString() + "," + ";";
                }
            }
            foreach (DataGridViewRow x in dataGridView4.Rows)
            {
                if (x.Cells[0].Style.BackColor == Color.Yellow)
                {
                    r = r + "4," + x.Cells[2].Value.ToString() + "," + x.Cells[4].Value.ToString() + "," + x.Cells[5].Value.ToString() + "," + x.Cells[6].Value.ToString() + "," + ";";
                }
            }
            foreach(string x in new_costos)
            {
                r = r + x + ";";
            }
            return r;
        }

        private bool getAllReady()
        {
            bool r = true;
            foreach(DataGridViewRow x in dataGridView5.Rows)
            {
                if(x.DefaultCellStyle.BackColor == Color.Red)
                {
                    r = false;
                }
            }           
            return r;
        }

        private string getMostAcabado()
        {
            string r = string.Empty;
            int tot = 0;
            List<string> acabados = new List<string>();
            foreach (DataGridViewRow x in dataGridView1.Rows)
            {
                acabados.Add(x.Cells[7].Value.ToString());
            }
            var d = acabados.GroupBy(c => c);
            foreach(var g in d)
            {
                if(g.Count() > tot)
                {
                    tot = g.Count();
                    r = g.Key;
                }
            }
            return r;
        }

        private bool arePerfiles()
        {
            bool r = false;
            if(checkBox11.Checked)
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    foreach (DataGridViewRow x in dataGridView1.Rows)
                    {
                        if (constants.stringToFloat(x.Cells[4].Value.ToString()) > 0)
                        {
                            r = true;
                            break;
                        }
                    }
                }
            }          
            return r;
        }


        private bool checkIsPerfiles()
        {
            bool r;
            if (dataGridView1.RowCount > 0 && arePerfiles() == true)
            {
                if ((comboBox1.Text != "" && comboBox3.Text != "") || (comboBox1.Text != "" && comboBox3.Text == "") || (comboBox1.Text == "" && comboBox3.Text != ""))
                {
                    r = true;
                }
                else
                {
                    r = false;
                }
            }
            else
            {
                r = true;
            }
            return r;
        }

        private bool getAllReadyPerfiles()
        {
            bool r = true;
            foreach (DataGridViewRow x in dataGridView1.Rows)
            {
                if (x.Cells[7].Style.BackColor == Color.Red)
                {
                    r = false;
                }
            }
            return r;
        }

        private bool getSeccionesOK()
        {
            bool r = true;
            foreach (DataGridViewRow x in dataGridView1.Rows)
            {
                if (x.Cells[6].Value.ToString() == "")
                {
                    r = false;
                    break;
                }
            }
            foreach (DataGridViewRow x in dataGridView2.Rows)
            {
                if (x.Cells[4].Value.ToString() == "")
                {
                    r = false;
                    break;
                }
            }
            foreach (DataGridViewRow x in dataGridView3.Rows)
            {
                if (x.Cells[5].Value.ToString() == "")
                {
                    r = false;
                    break;
                }
            }
            foreach (DataGridViewRow x in dataGridView4.Rows)
            {
                if (x.Cells[6].Value.ToString() == "")
                {
                    r = false;
                    break;
                }
            }
            return r;
        }

        //Agregar
        private void button1_Click(object sender, EventArgs e)
        {
            agregar();
        }

        private void agregar()
        {
            if (getAllReady() == true)
            {
                if (getSeccionesOK() == true)
                {
                    if (panel.Width < (tableLayoutPanel1.Width - 5) && panel.Height < (tableLayoutPanel1.Height - 5))
                    {
                        if (checkIsPerfiles() == true)
                        {
                            if (getAllReadyPerfiles() == true)
                            {
                                if (checkWeightBool() == true)
                                {
                                    if (checkSelectionBoolCortinas() == true)
                                    {
                                        cotizaciones_local cotizacion = new cotizaciones_local();

                                        if (constants.getArticuloIdLocalDB(5, id_cotizacion) == false)
                                        {
                                            try
                                            {
                                                var modulo = new modulos_cotizaciones
                                                {
                                                    folio = 00000,
                                                    modulo_id = id,
                                                    descripcion = richTextBox1.Text,
                                                    mano_obra = constants.stringToFloat(textBox3.Text),
                                                    dimensiones = getDimensions(),
                                                    largo = largo_total,
                                                    alto = alto_total,
                                                    acabado_perfil = comboBox3.SelectedIndex >= 0 ? comboBox3.Text : comboBox1.Text,
                                                    claves_cristales = getClavesCristal(),
                                                    cantidad = constants.stringToInt(textBox4.Text),
                                                    flete = constants.stringToFloat(textBox6.Text),
                                                    desperdicio = constants.stringToFloat(textBox5.Text),
                                                    utilidad = constants.stringToFloat(textBox7.Text),
                                                    articulo = label7.Text,
                                                    linea = label8.Text,
                                                    diseño = label18.Text,
                                                    clave = label6.Text,
                                                    total = Math.Round(constants.stringToFloat(textBox13.Text), 2),
                                                    claves_otros = getClavesOtros(),
                                                    claves_herrajes = getClavesHerrajes(),
                                                    ubicacion = textBox8.Text,
                                                    claves_perfiles = getClavesPerfiles(),
                                                    pic = constants.imageToByte(createModuloPic()),
                                                    merge_id = -1,
                                                    concept_id = -1,
                                                    sub_folio = constants.sub_folio,
                                                    dir = 0,
                                                    news = getNewItems(),
                                                    new_desing = "",
                                                    orden = constants.getCountPartidas()
                                                };
                                                cotizacion.modulos_cotizaciones.Add(modulo);
                                                cotizacion.SaveChanges();
                                            }
                                            catch (Exception err)
                                            {
                                                constants.errorLog(err.ToString());
                                                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                            ((Form1)Application.OpenForms["form1"]).refreshNewArticulo(5);
                                            constants.count_modulos++;
                                            ((Form1)Application.OpenForms["form1"]).loadCountArticulos();
                                            if (Application.OpenForms["articulos_cotizacion"] != null)
                                            {
                                                ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).resetRowSelect();
                                            }
                                            //Close
                                            if (Application.OpenForms["edit_expresss"] == null)
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
                                                Close();
                                            }
                                        }
                                        else
                                        {
                                            DialogResult r = MessageBox.Show("Esté artículo ya esta incluido. ¿Desea guardar los cambios?.", constants.msg_box_caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                                            if (r == DialogResult.Yes)
                                            {
                                                try
                                                {
                                                    var ud = (from x in cotizacion.modulos_cotizaciones where x.id == id_cotizacion select x).SingleOrDefault();
                                                    if (ud != null)
                                                    {
                                                        ud.modulo_id = id;
                                                        ud.descripcion = richTextBox1.Text;
                                                        ud.mano_obra = constants.stringToFloat(textBox3.Text);
                                                        ud.dimensiones = getDimensions();
                                                        ud.largo = largo_total;
                                                        ud.alto = alto_total;
                                                        ud.acabado_perfil = comboBox3.SelectedIndex >= 0 ? comboBox3.Text : comboBox1.Text;
                                                        ud.claves_cristales = getClavesCristal();
                                                        ud.cantidad = constants.stringToInt(textBox4.Text);
                                                        ud.flete = constants.stringToFloat(textBox6.Text);
                                                        ud.desperdicio = constants.stringToFloat(textBox5.Text);
                                                        ud.utilidad = constants.stringToFloat(textBox7.Text);
                                                        ud.articulo = label7.Text;
                                                        ud.linea = label8.Text;
                                                        ud.diseño = label18.Text;
                                                        ud.clave = label6.Text;
                                                        ud.total = Math.Round(constants.stringToFloat(textBox13.Text), 2);
                                                        ud.claves_otros = getClavesOtros();
                                                        ud.claves_herrajes = getClavesHerrajes();
                                                        ud.ubicacion = textBox8.Text;
                                                        ud.claves_perfiles = getClavesPerfiles();
                                                        ud.pic = ud.new_desing == "" ? constants.imageToByte(createModuloPic()) : ud.pic;
                                                        ud.news = getNewItems();
                                                        cotizacion.SaveChanges();
                                                        if (ud.merge_id > 0)
                                                        {
                                                            constants.updateModuloPersonalizado((int)ud.merge_id);
                                                        }
                                                        constants.errors_Open.RemoveAll(x => x == id_cotizacion);
                                                        constants.errors_Open.RemoveAll(x => x == ud.merge_id);
                                                    }
                                                }
                                                catch (Exception err)
                                                {
                                                    constants.errorLog(err.ToString());
                                                    MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                                ((Form1)Application.OpenForms["form1"]).refreshNewArticulo(5);
                                                ((Form1)Application.OpenForms["form1"]).setEditImage(false, false);
                                                constants.id_articulo_cotizacion = -1;
                                                //Close
                                                if (Application.OpenForms["edit_expresss"] == null)
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
                                                    Close();
                                                }
                                            }
                                            else if (r == DialogResult.No)
                                            {
                                                DialogResult r2 = MessageBox.Show("¿Desea generar un nuevo concepto a partir de estos datos?.", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                                if (r2 == DialogResult.Yes)
                                                {
                                                    constants.id_articulo_cotizacion = -1;
                                                    id_cotizacion = -1;
                                                    try
                                                    {
                                                        var modulo = new modulos_cotizaciones
                                                        {
                                                            folio = 00000,
                                                            modulo_id = id,
                                                            descripcion = richTextBox1.Text,
                                                            mano_obra = constants.stringToFloat(textBox3.Text),
                                                            dimensiones = getDimensions(),
                                                            largo = largo_total,
                                                            alto = alto_total,
                                                            acabado_perfil = comboBox3.SelectedIndex >= 0 ? comboBox3.Text : comboBox1.Text,
                                                            claves_cristales = getClavesCristal(),
                                                            cantidad = constants.stringToInt(textBox4.Text),
                                                            flete = constants.stringToFloat(textBox6.Text),
                                                            desperdicio = constants.stringToFloat(textBox5.Text),
                                                            utilidad = constants.stringToFloat(textBox7.Text),
                                                            articulo = label7.Text,
                                                            linea = label8.Text,
                                                            diseño = label18.Text,
                                                            clave = label6.Text,
                                                            total = Math.Round(constants.stringToFloat(textBox13.Text), 2),
                                                            claves_otros = getClavesOtros(),
                                                            claves_herrajes = getClavesHerrajes(),
                                                            ubicacion = textBox8.Text,
                                                            claves_perfiles = getClavesPerfiles(),
                                                            pic = constants.imageToByte(createModuloPic()),
                                                            merge_id = -1,
                                                            concept_id = -1,
                                                            sub_folio = constants.sub_folio,
                                                            dir = 0,
                                                            news = getNewItems(),
                                                            new_desing = "",
                                                            orden = constants.getCountPartidas()
                                                        };
                                                        cotizacion.modulos_cotizaciones.Add(modulo);
                                                        cotizacion.SaveChanges();
                                                    }
                                                    catch (Exception err)
                                                    {
                                                        constants.errorLog(err.ToString());
                                                        MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    }
                                                    ((Form1)Application.OpenForms["form1"]).refreshNewArticulo(5);
                                                    constants.count_modulos++;
                                                    ((Form1)Application.OpenForms["form1"]).loadCountArticulos();
                                                    ((Form1)Application.OpenForms["form1"]).setEditImage(false, false);
                                                    if (Application.OpenForms["articulos_cotizacion"] != null)
                                                    {
                                                        ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).resetRowSelect();
                                                    }
                                                    //Close
                                                    if (Application.OpenForms["edit_expresss"] == null)
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
                                                        Close();
                                                    }
                                                }
                                            }
                                        }
                                        ((Form1)Application.OpenForms["form1"]).calcularTotalesCotizacion();
                                        constants.cotizacion_proceso = true;
                                        constants.cotizacion_guardada = false;
                                        if (Application.OpenForms["articulos_cotizacion"] != null)
                                        {
                                            ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL();
                                        }
                                        if (Application.OpenForms["edit_expresss"] != null)
                                        {
                                            ((edit_expresss)Application.OpenForms["edit_expresss"]).reloadALL(id_cotizacion);
                                            Application.OpenForms["edit_expresss"].Select();
                                            Application.OpenForms["edit_expresss"].WindowState = FormWindowState.Normal;
                                            ((edit_expresss)Application.OpenForms["edit_expresss"]).selectEdited();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("[Error] Se debe seleccionar un sistema de elevación para la cortina.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("[Error] El sistema de elevación seleccionado no es el adecuado para está cortina.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("[Error] existen acabados no compatibles en el aluminio.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] necesitas añadir un acabado al aluminio.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] Las dimensiones de esté módulo se han salido de la escala.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("[Error] existen artículos sin sección.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] existen secciones sin configurar.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //cantidad
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            calcularCostoModulo();
        }

        //desperdicio
        private void textBox5_TextChanged(object sender, EventArgs e)
        {            
            calcularCostoModulo();
        }

        //flete
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            calcularCostoModulo();
        }

        //utilidad
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            calcularCostoModulo();
        }

        //Ajustar calculo
        private void ajustarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float d = constants.stringToFloat(dataGridView5.CurrentCell.Value.ToString()); ;
            float total = 0;
            int column = dataGridView5.CurrentCell.ColumnIndex;
            int row = dataGridView5.CurrentCell.RowIndex;
            float ajustado = 0;
            int indi = 0;

            if (checkBox1.Checked == true)
            {
                total = constants.stringToFloat(dataGridView5.Rows[0].Cells[column].Value.ToString());
            }
            else
            {
                indi = -1;
                if(column == 2)
                {
                    total = constants.stringToFloat(textBox1.Text);
                }
                else
                {
                    total = constants.stringToFloat(textBox2.Text);
                }
            }

            ajustado = total - d;
            ajustado = ajustado / (dataGridView5.RowCount - 2);
            if (ajustado > 0)
            {
                foreach (DataGridViewRow x in dataGridView5.Rows)
                {
                    if (x.Index != row)
                    {
                        if (x.Index > indi)
                        {
                            x.Cells[column].Value = Math.Round(ajustado);
                        }
                    }
                }
            }
            checkDimensions();
            calcularCostoModulo();
        }

        private void elementosFaltantesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string seccion_p = dataGridView5.CurrentRow.Cells[0].Value.ToString();

            foreach (DataGridViewRow x in dataGridView4.Rows)
            {
                if (x.Cells[6].Value.ToString() == seccion_p)
                {
                    if (x.Cells[2].Value.ToString() == "")
                    {
                        tabControl1.SelectedTab = tabPage4;
                        x.Cells[2].Selected = true;
                        break;
                    }
                }
            }

            foreach (DataGridViewRow x in dataGridView3.Rows)
            {
                if (x.Cells[5].Value.ToString() == seccion_p)
                {
                    if (x.Cells[6].Value.ToString() == "")
                    {
                        tabControl1.SelectedTab = tabPage3;
                        x.Cells[6].Selected = true;
                        break;
                    }
                }
            }          

            foreach (DataGridViewRow x in dataGridView2.Rows)
            {
                if (x.Cells[4].Value.ToString() == seccion_p)
                {
                    if (x.Cells[1].Value.ToString() == "")
                    {
                        tabControl1.SelectedTab = tabPage2;
                        x.Cells[1].Selected = true;
                        break;
                    }
                }
            }

            foreach (DataGridViewRow x in dataGridView1.Rows)
            {
                if (x.Cells[6].Value.ToString() == seccion_p)
                {
                    if (x.Cells[7].Value.ToString() == "")
                    {
                        tabControl1.SelectedTab = tabPage1;
                        x.Cells[7].Selected = true;
                        break;
                    }
                }
            }
        }

        private void loadConfigParemeters()
        {
            string[] parametros = new string[] { };
            try
            {
                XDocument opciones = XDocument.Load(constants.opciones_xml);

                var op = (from x in opciones.Descendants("Opciones") select x.Element("CM")).SingleOrDefault();

                if (op != null)
                {
                    parametros = op.Value.Split(',');
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }

            if (parametros.Length == 4)
            {
                textBox5.Text = constants.stringToFloat(parametros[0]).ToString();
                textBox6.Text = constants.stringToFloat(parametros[1]).ToString();
                textBox3.Text = constants.stringToFloat(parametros[2]).ToString();
                textBox7.Text = constants.stringToFloat(parametros[3]).ToString();
            }
        }      

        private void button2_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["cm_opciones"] == null)
            {
                if(constants.user_access <= 1)
                {
                    MessageBox.Show("Este usuario no cuenta con los privilegios para acceder a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    new cm_opciones().ShowDialog();
                }
            }
        }

        //Permitir ajuste de cantidades CRISTALES
        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox15.Checked == true)
            {
                dataGridView2.Columns[3].DefaultCellStyle.BackColor = Color.FromArgb(192, 192, 255);
                dataGridView2.Columns[3].ReadOnly = false;
            }
            else
            {
                dataGridView2.Columns[3].DefaultCellStyle.BackColor = Color.White;
                dataGridView2.Columns[3].ReadOnly = true;
            }
        }

        private void DataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2[e.ColumnIndex, e.RowIndex].Value != null)
            {
                getInstruction(dataGridView2.CurrentRow.Cells[1].Value.ToString(), constants.stringToFloat(dataGridView2.CurrentRow.Cells[3].Value.ToString()));
                calcularCostoModulo();
                recountItems();
            }
            else
            {
                if (dataGridView2[e.ColumnIndex, e.RowIndex].OwningColumn.HeaderText == "Cantidad")
                {
                    if (dataGridView2[e.ColumnIndex, e.RowIndex].Value == null)
                    {
                        dataGridView2[e.ColumnIndex, e.RowIndex].Value = "0";
                    }                   
                }
                //---------------------------------------------------------------------------------->
                if (dataGridView2[e.ColumnIndex, e.RowIndex].OwningColumn.HeaderText != "Cantidad")
                {
                    dataGridView2[e.ColumnIndex, e.RowIndex].Value = "";
                }
                getInstruction(dataGridView2.CurrentRow.Cells[1].Value.ToString(), constants.stringToFloat(dataGridView2.CurrentRow.Cells[3].Value.ToString()));
                calcularCostoModulo();
            }
        }
        //---------------------------------------------------------------------------------------------------------->

        //Permitir ajuste de cantidades HERRAJES  
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox2.Checked == true)
            {
                dataGridView3.Columns[4].DefaultCellStyle.BackColor = Color.FromArgb(192,192,255);
                dataGridView3.Columns[4].ReadOnly = false;
            }
            else
            {
                dataGridView3.Columns[4].DefaultCellStyle.BackColor = Color.White;
                dataGridView3.Columns[4].ReadOnly = true;
            }
        }

        private void DataGridView3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView3[e.ColumnIndex, e.RowIndex].Value != null)
            {
                getInstruction(dataGridView3.CurrentRow.Cells[2].Value.ToString(), constants.stringToFloat(dataGridView3.CurrentRow.Cells[4].Value.ToString()));
                calcularCostoModulo();
                recountItems();
            }
            else
            {
                if (dataGridView3[e.ColumnIndex, e.RowIndex].OwningColumn.HeaderText == "Cantidad")
                {
                    if (dataGridView3[e.ColumnIndex, e.RowIndex].Value == null)
                    {
                        dataGridView3[e.ColumnIndex, e.RowIndex].Value = "0";
                    }
                }
                //---------------------------------------------------------------------------------->
                if (dataGridView3[e.ColumnIndex, e.RowIndex].OwningColumn.HeaderText != "Cantidad")
                {
                    dataGridView3[e.ColumnIndex, e.RowIndex].Value = "";
                }
                getInstruction(dataGridView3.CurrentRow.Cells[2].Value.ToString(), constants.stringToFloat(dataGridView3.CurrentRow.Cells[4].Value.ToString()));
                calcularCostoModulo();
            }
        }
        //---------------------------------------------------------------------------------------------------------->

        //Permitir ajuste de cantidades OTROS     
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                dataGridView4.Columns[4].DefaultCellStyle.BackColor = Color.FromArgb(192, 192, 255);
                dataGridView4.Columns[4].ReadOnly = false;
            }
            else
            {
                dataGridView4.Columns[4].DefaultCellStyle.BackColor = Color.White;
                dataGridView4.Columns[4].ReadOnly = true;
            }
        }

        private void DataGridView4_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView4[e.ColumnIndex, e.RowIndex].Value != null)
            {
                getInstruction(dataGridView4.CurrentRow.Cells[2].Value.ToString(), constants.stringToFloat(dataGridView4.CurrentRow.Cells[4].Value.ToString()));
                calcularCostoModulo();
                recountItems();
            }
            else
            {
                if (dataGridView4[e.ColumnIndex, e.RowIndex].OwningColumn.HeaderText == "Cantidad")
                {
                    if (dataGridView4[e.ColumnIndex, e.RowIndex].Value == null)
                    {
                        dataGridView4[e.ColumnIndex, e.RowIndex].Value = "0";
                    }
                }
                //---------------------------------------------------------------------------------->
                if (dataGridView4[e.ColumnIndex, e.RowIndex].OwningColumn.HeaderText != "Cantidad")
                {
                    dataGridView4[e.ColumnIndex, e.RowIndex].Value = "";
                }
                getInstruction(dataGridView4.CurrentRow.Cells[2].Value.ToString(), constants.stringToFloat(dataGridView4.CurrentRow.Cells[4].Value.ToString()));
                calcularCostoModulo();
            }
        }
        //---------------------------------------------------------------------------------------------------------->        

        //Permitir ajuste de cantidades PERFILES
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked == true)
            {
                dataGridView1.Columns[4].ReadOnly = false;
                dataGridView1.Columns[4].DefaultCellStyle.BackColor = Color.FromArgb(192, 192, 255);
            }
            else
            {
                dataGridView1.Columns[4].ReadOnly = true;
                dataGridView1.Columns[4].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                dataGridView1.Columns[3].DefaultCellStyle.BackColor = Color.FromArgb(192, 192, 255);
            }
            else
            {
                dataGridView1.Columns[3].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1[e.ColumnIndex, e.RowIndex].Value != null)
            {
                if (constants.stringToFloat(dataGridView1.CurrentRow.Cells[4].Value.ToString()) <= 0)
                {
                    dataGridView1.CurrentRow.Cells[7].Value = "";
                    dataGridView1.CurrentRow.Cells[7].Style.BackColor = Color.White;
                }
                else
                {
                    //Aplicador de acabado automatico --------------------------------------->
                    if(dataGridView1.CurrentRow.Cells[7].Value.ToString() == "")
                    {
                        if(comboBox1.Text != "")
                        {
                            setAcabadoLista();
                        }
                        else if(comboBox3.Text != "")
                        {
                            setAcabadoAnodizado();
                        }
                    }
                    //----------------------------------------------------------------------->
                }
                checkAcabados();
                getInstruction(dataGridView1.CurrentRow.Cells[2].Value.ToString(), constants.stringToFloat(dataGridView1.CurrentRow.Cells[4].Value.ToString()));
                calcularCostoModulo();
                recountItems();
            }
            else
            {
                if(dataGridView1[e.ColumnIndex, e.RowIndex].OwningColumn.HeaderText == "Cantidad")
                {
                    if(dataGridView1[e.ColumnIndex, e.RowIndex].Value == null)
                    {
                        dataGridView1[e.ColumnIndex, e.RowIndex].Value = "0";
                    }                    
                }
                //---------------------------------------------------------------------------------->
                if (constants.stringToFloat(dataGridView1.CurrentRow.Cells[4].Value.ToString()) <= 0)
                {
                    dataGridView1.CurrentRow.Cells[7].Value = "";
                    dataGridView1.CurrentRow.Cells[7].Style.BackColor = Color.White;
                }
                else
                {
                    //Aplicador de acabado automatico --------------------------------------->
                    if (dataGridView1.CurrentRow.Cells[7].Value.ToString() == "")
                    {
                        if (comboBox1.Text != "")
                        {
                            setAcabadoLista();
                        }
                        else if (comboBox3.Text != "")
                        {
                            setAcabadoAnodizado();
                        }
                    }
                    //----------------------------------------------------------------------->
                }
                checkAcabados();
                if (dataGridView1[e.ColumnIndex, e.RowIndex].OwningColumn.HeaderText != "Cantidad")
                {
                    dataGridView1[e.ColumnIndex, e.RowIndex].Value = "";
                }
                getInstruction(dataGridView1.CurrentRow.Cells[2].Value.ToString(), constants.stringToFloat(dataGridView1.CurrentRow.Cells[4].Value.ToString()));
                calcularCostoModulo();
            }
        }

        private void checkAcabados()
        {
            bool r = false;
            foreach(DataGridViewRow x in dataGridView1.Rows)
            {
                if(x.Cells[7].Value.ToString() != "")
                {
                    r = true;
                }
                else
                {
                    x.Cells[7].Style.BackColor = Color.White;                  
                }
            }
            if(r == false)
            {
                comboBox1.SelectedIndex = -1;
                comboBox3.SelectedIndex = -1;
                pictureBox2.Image = null;
                label37.Text = "";
                for (int i = 0; i < panel.Controls.Count; i++)
                {
                    panel.Controls[i].BackgroundImage = null;
                }
                if (checkBox1.Checked == true)
                {
                    panel.BackgroundImage = null;
                }
            }          
            for (int i = c; i <= constants.stringToInt(label9.Text); i++)
            {
                getSeccionesReady(i);
            }
        }
        //---------------------------------------------------------------------------------------------------------->        

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            setAcabadoAnodizado();
        }

        private void setAcabadoAnodizado()
        {
            if (comboBox3.SelectedIndex >= 0)
            {
                if (dataGridView1.RowCount > 0 && arePerfiles() == true)
                {
                    comboBox1.SelectedIndex = -1;
                    string clr = comboBox3.Text;
                    listas_entities_pva listas = new listas_entities_pva();
                    var color = (from x in listas.colores_aluminio where x.clave == clr select x).SingleOrDefault();
                    if (color != null)
                    {
                        label37.Text = color.color.ToUpper();
                    }
                    if (checkBox9.Checked == false)
                    {
                        if (checkBox20.Checked)
                        {
                            //using SACS
                            string acabado_op = constants.IASetAcabado(comboBox3.Text);

                            foreach (DataGridViewRow x in dataGridView1.Rows)
                            {
                                if (constants.stringToFloat(x.Cells[4].Value.ToString()) > 0)
                                {
                                    if (IACheckPefilesAcabados(x.Cells[2].Value.ToString(), comboBox3.Text) == true)
                                    {
                                        x.Cells[7].Value = comboBox3.Text;
                                    }
                                    else if (IACheckPefilesAcabados(x.Cells[2].Value.ToString(), acabado_op) == true)
                                    {
                                        x.Cells[7].Value = acabado_op;
                                    }
                                    else
                                    {
                                        x.Cells[7].Value = comboBox3.Text;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (DataGridViewRow x in dataGridView1.Rows)
                            {
                                if (constants.stringToFloat(x.Cells[4].Value.ToString()) > 0)
                                {
                                    x.Cells[7].Value = comboBox3.Text;
                                }
                            }
                        }
                    }
                    for (int i = c; i <= constants.stringToInt(label9.Text); i++)
                    {
                        getSeccionesReady(i);
                    }
                    for (int i = 0; i < panel.Controls.Count; i++)
                    {
                        constants.setBackgroundImg("acabados_especiales", comboBox3.Text, "jpg", panel.Controls[i]);
                    }
                    calcularCostoModulo();
                    if (checkBox1.Checked == true)
                    {
                        constants.setBackgroundImg("acabados_especiales", comboBox3.Text, "jpg", panel);
                    }
                    constants.setImage("acabados_especiales", comboBox3.Text, "jpg", pictureBox2);
                    checkPefilesAcabados();
                }
                else
                {
                    comboBox3.SelectedIndex = -1;
                }
            }
        }

        //exportar datos
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            constants.ExportToExcelFile(dataGridView6);
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView6.RowCount > 0)
            {
                if (backgroundWorker1.IsBusy == false)
                {
                    pictureBox1.Visible = true;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            else
            {
                MessageBox.Show("[Error] no existen datos para exportar.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        //Imprimir datos
        private void button4_Click(object sender, EventArgs e)
        {
            if (panel.Width > (tableLayoutPanel1.Width - 5) || panel.Height > (tableLayoutPanel1.Height - 5))
            {
                MessageBox.Show("[Error] Las dimensiones de esté módulo se han salido de la escala.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {       
                modulo_data md = new modulo_data();
                foreach (DataGridViewRow x in dataGridView6.Rows)
                {
                    DataRow row = md.Tables[1].NewRow();
                    row[0] = x.Cells[0].Value;
                    row[1] = x.Cells[1].Value;
                    row[2] = x.Cells[2].Value;
                    row[3] = x.Cells[3].Value;
                    row[4] = x.Cells[4].Value;
                    row[5] = x.Cells[5].Value;
                    row[6] = x.Cells[6].Value;
                    row[7] = x.Cells[7].Value;
                    row[8] = x.Cells[8].Value;
                    md.Tables[1].Rows.Add(row);
                }
                createModuloPic(md);
                new modulo_precios(md, label6.Text, label7.Text, label8.Text, label37.Text, textBox4.Text, textBox5.Text, textBox6.Text, textBox3.Text, textBox7.Text, label12.Text, "Largo: " + label21.Text + " - " + "Alto: " + label20.Text, textBox9.Text, textBox10.Text, textBox12.Text, textBox11.Text, textBox13.Text, label44.Text, "Largo mm", "Alto mm", textBox8.Text).ShowDialog();
                md.Dispose();
            }       
        }

        //Crear pic modulo
        private void createModuloPic(modulo_data data)
        {
            Bitmap bm = new Bitmap(panel.Width, panel.Height);
            panel.DrawToBitmap(bm, new Rectangle(0, 0, panel.Width, panel.Height));
            Bitmap gm_2 = new Bitmap(bm, 120, 105);
            data.Tables["img_modulo"].Rows.Clear();
            DataRow row = data.Tables["img_modulo"].NewRow();
            row[0] = constants.imageToByte(gm_2);
            data.Tables["img_modulo"].Rows.Add(row);
            bm = null;
            gm_2 = null;
        }
        //

        private void button5_Click(object sender, EventArgs e)
        {
            new colores().ShowDialog();
        }
        //---------------------------------------------------------------------------------------------------------->

        //mano de obra
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            calcularCostoModulo();
        }

        //Edit List
        private void button7_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["edit_expresss"] == null)
            {
                new edit_expresss().Show();
            }
            else
            {
                Application.OpenForms["edit_expresss"].Select();
                Application.OpenForms["edit_expresss"].WindowState = FormWindowState.Normal;
            }
        }

        //Alto
        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            if (constants.stringToFloat(textBox2.Text) > 0)
            {
                dataGridView5.Columns[3].ReadOnly = true;
                if (constants.stringToFloat(textBox2.Text) > 30000)
                {
                    MessageBox.Show("[Error] Se ha superado el valor máximo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Text = "0";
                }
            }
            else
            {
                dataGridView5.Columns[3].ReadOnly = false;
                textBox2.Text = "";
            }
            foreach (DataGridViewRow x in dataGridView5.Rows)
            {
                if (constants.stringToFloat(textBox2.Text) != 0)
                {
                    if (x.Index == 0)
                    {
                        if (checkBox1.Checked == true)
                        {
                            x.Cells[3].Value = constants.stringToInt(textBox2.Text);
                        }
                        else
                        {
                            if (panel.GetRowSpan(panel.Controls[((int)x.Cells[0].Value) - 1]) == 1)
                            {
                                x.Cells[3].Value = constants.stringToInt(textBox2.Text) / panel.RowCount;
                            }
                            else
                            {
                                x.Cells[3].Value = constants.stringToInt(textBox2.Text) / (panel.RowCount / panel.GetRowSpan(panel.Controls[((int)x.Cells[0].Value) - 1]));
                            }
                        }
                    }
                    else
                    {
                        if (panel.GetRowSpan(panel.Controls[((int)x.Cells[0].Value) - 1]) == 1)
                        {
                            x.Cells[3].Value = constants.stringToInt(textBox2.Text) / panel.RowCount;
                        }
                        else
                        {
                            x.Cells[3].Value = constants.stringToInt(textBox2.Text) / (panel.RowCount / panel.GetRowSpan(panel.Controls[((int)x.Cells[0].Value) - 1]));
                        }
                    }
                }
                else
                {
                    if (x.Index == 0)
                    {
                        if (checkBox1.Checked == true)
                        {
                            x.Cells[3].Value = 1000 * panel.RowCount;
                        }
                        else
                        {
                            x.Cells[3].Value = 1000;
                        }
                    }
                    else
                    {
                        x.Cells[3].Value = 1000;
                    }
                }
            }
            checkDimensions();
            calcularCostoModulo();
        }

        //Largo
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (constants.stringToFloat(textBox1.Text) > 0)
            {
                dataGridView5.Columns[2].ReadOnly = true;
                if (constants.stringToFloat(textBox1.Text) > 90000)
                {
                    MessageBox.Show("[Error] Se ha superado el valor máximo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Text = "0";
                }
            }
            else
            {
                dataGridView5.Columns[2].ReadOnly = false;
                textBox1.Text = "";
            }
            foreach (DataGridViewRow x in dataGridView5.Rows)
            {
                if (constants.stringToFloat(textBox1.Text) != 0)
                {
                    if (x.Index == 0)
                    {
                        if (checkBox1.Checked == true)
                        {
                            x.Cells[2].Value = constants.stringToInt(textBox1.Text);
                        }
                        else
                        {
                            if (panel.GetColumnSpan(panel.Controls[((int)x.Cells[0].Value) - 1]) == 1)
                            {
                                x.Cells[2].Value = constants.stringToInt(textBox1.Text) / panel.ColumnCount;
                            }
                            else
                            {
                                x.Cells[2].Value = constants.stringToInt(textBox1.Text) / (panel.ColumnCount / panel.GetColumnSpan(panel.Controls[((int)x.Cells[0].Value) - 1]));
                            }
                        }
                    }
                    else
                    {
                        if (panel.GetColumnSpan(panel.Controls[((int)x.Cells[0].Value) - 1]) == 1)
                        {
                            x.Cells[2].Value = constants.stringToInt(textBox1.Text) / panel.ColumnCount;
                        }
                        else
                        {
                            x.Cells[2].Value = constants.stringToInt(textBox1.Text) / (panel.ColumnCount / panel.GetColumnSpan(panel.Controls[((int)x.Cells[0].Value) - 1]));
                        }
                    }
                }
                else
                {
                    if (x.Index == 0)
                    {
                        if (checkBox1.Checked == true)
                        {
                            x.Cells[2].Value = 1000 * panel.ColumnCount;
                        }
                        else
                        {
                            x.Cells[2].Value = 1000;
                        }
                    }
                    else
                    {
                        x.Cells[2].Value = 1000;
                    }
                }
            }
            checkDimensions();
            calcularCostoModulo();
        }

        private void HScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow x in dataGridView5.Rows)
            {
                seccion_e = constants.stringToInt(x.Cells[0].Value.ToString());
                height = constants.stringToInt(x.Cells[3].Value.ToString());
                width = constants.stringToInt(x.Cells[2].Value.ToString());
                if (hScrollBar1.Value < hScrollBar1.Maximum)
                {
                    if (checkBox1.Checked == true)
                    {
                        if (seccion_e > 0)
                        {
                            panel.Controls[seccion_e - 1].Width = width / (hScrollBar1.Maximum - hScrollBar1.Value);
                            panel.Controls[seccion_e - 1].Height = height / (hScrollBar1.Maximum - hScrollBar1.Value);
                        }
                        else
                        {
                            panel.Width = (width / (hScrollBar1.Maximum - hScrollBar1.Value)) + getMarcoWidth();
                            panel.Height = (height / (hScrollBar1.Maximum - hScrollBar1.Value)) + getMarcoHeight();
                            if (cs == false)
                            {
                                for (int i = 0; i < panel.Controls.Count; i++)
                                {
                                    panel.Controls[i].Width = (width / panel.ColumnCount) / (hScrollBar1.Maximum - hScrollBar1.Value);
                                    panel.Controls[i].Height = (height / panel.RowCount) / (hScrollBar1.Maximum - hScrollBar1.Value);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (cs == false)
                        {
                            for (int i = 0; i < panel.Controls.Count; i++)
                            {
                                panel.Controls[i].Width = (width / panel.ColumnCount) / (hScrollBar1.Maximum - hScrollBar1.Value);
                                panel.Controls[i].Height = (height / panel.RowCount) / (hScrollBar1.Maximum - hScrollBar1.Value);
                            }
                        }
                        else
                        {
                            panel.Controls[seccion_e - 1].Width = width / (hScrollBar1.Maximum - hScrollBar1.Value);
                            panel.Controls[seccion_e - 1].Height = height / (hScrollBar1.Maximum - hScrollBar1.Value);
                            panel.Width = ((width * panel.ColumnCount) / (hScrollBar1.Maximum - hScrollBar1.Value)) + getMarcoWidth();
                            panel.Height = ((height * panel.RowCount) / (hScrollBar1.Maximum - hScrollBar1.Value)) + getMarcoHeight();
                        }
                    }
                }
            }
            label19.Text = "x -" + (hScrollBar1.Maximum - hScrollBar1.Value);
        }

        private int getMarcoWidth()
        {
            return marco_width;
        }

        private int getMarcoHeight()
        {
            return marco_height;
        }

        //borrar panel image background
        private void clearBackground()
        {
            foreach (Control x in panel.Controls)
            {
                x.BackgroundImage = null;
                x.BackColor = Color.LightBlue;
            }
            panel.BackgroundImage = null;
            panel.BackColor = Color.LightBlue;
        }        

        //Reglas
        private void getReglas(string reglas)
        {
            if (reglas.Length > 0)
            {
                reglas = reglas.ToUpper();
                this.reglas = null;
                this.reglas = reglas.Split('$');
            }
        }

        //cambiar parametros ------------------------------------------------------------>      
        private void CheckBox8_Click(object sender, EventArgs e)
        {
            if (constants.user_access == 6 || constants.permitir_cp == true)
            {
                if (checkBox8.Checked)
                {
                    textBox5.Enabled = false;
                    textBox6.Enabled = false;
                    textBox3.Enabled = false;
                    textBox7.Enabled = false;
                    checkBox8.Checked = false;
                }
                else
                {
                    textBox5.Enabled = true;
                    textBox6.Enabled = true;
                    textBox3.Enabled = true;
                    textBox7.Enabled = true;
                    checkBox8.Checked = true;
                }
            }
            else
            {
                if (checkBox8.Checked)
                {
                    textBox5.Enabled = false;
                    textBox6.Enabled = false;
                    textBox3.Enabled = false;
                    textBox7.Enabled = false;
                    checkBox8.Checked = false;
                    new delete_password(false).ShowDialog();
                }
                else
                {
                    new delete_password(false).ShowDialog();
                }
            }
        }

        public void permitirCambioParametros(bool access)
        {
            if (access == true)
            {
                constants.permitir_cp = true;
                checkBox8.Checked = true;
                textBox5.Enabled = true;
                textBox6.Enabled = true;
                textBox3.Enabled = true;
                textBox7.Enabled = true;
            }
            else
            {
                checkBox8.Checked = false;
                textBox5.Enabled = false;
                textBox6.Enabled = false;
                textBox3.Enabled = false;
                textBox7.Enabled = false;
            }
        }

        private void acabadoListaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new colores(false, dataGridView1.CurrentRow.Index).ShowDialog();
        }

        private void acabadoPAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new colores(true, dataGridView1.CurrentRow.Index).ShowDialog();
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox9.Checked)
            {
                comboBox1.Enabled = false;
                comboBox3.Enabled = false;
                button5.Enabled = false;
            }
            else
            {
                comboBox1.Enabled = true;
                comboBox3.Enabled = true;
                button5.Enabled = true;
            }
        }
        //------------------------------------------------------------------------------------->

        //Habilitar componentes
        private void CheckBox14_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("¿Desea habilitar/deshabilitar esté componente?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {
                if (checkBox14.Checked)
                {
                    if (!checkNewItems(dataGridView4))
                    {
                        checkBox14.Checked = false;
                        dataGridView4.Enabled = false;
                        reloadSecciones();
                        calcularCostoModulo();
                    }
                    else
                    {
                        MessageBox.Show("[Error] Se deben de eliminar los nuevos artículos para deshabilitar dicho componente.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    checkBox14.Checked = true;
                    dataGridView4.Enabled = true;
                    reloadSecciones();
                    calcularCostoModulo();
                }
            }
        }

        private void CheckBox13_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("¿Desea habilitar/deshabilitar esté componente?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {
                if (checkBox13.Checked)
                {
                    if (!checkNewItems(dataGridView3))
                    {
                        checkBox13.Checked = false;
                        dataGridView3.Enabled = false;
                        reloadSecciones();
                        calcularCostoModulo();
                    }
                    else
                    {
                        MessageBox.Show("[Error] Se deben de eliminar los nuevos artículos para deshabilitar dicho componente.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    checkBox13.Checked = true;
                    dataGridView3.Enabled = true;
                    reloadSecciones();
                    calcularCostoModulo();
                }
            }
        }

        private void CheckBox12_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("¿Desea habilitar/deshabilitar esté componente?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {
                if (checkBox12.Checked)
                {
                    if (!checkNewItems(dataGridView2))
                    {
                        checkBox12.Checked = false;
                        dataGridView2.Enabled = false;
                        reloadSecciones();
                        calcularCostoModulo();
                    }
                    else
                    {
                        MessageBox.Show("[Error] Se deben de eliminar los nuevos artículos para deshabilitar dicho componente.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    checkBox12.Checked = true;
                    dataGridView2.Enabled = true;
                    reloadSecciones();
                    calcularCostoModulo();
                }
            }
        }

        private void CheckBox11_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("¿Desea habilitar/deshabilitar esté componente?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {             
                if (checkBox11.Checked)
                {
                    if (!checkNewItems(dataGridView1))
                    {
                        checkBox11.Checked = false;
                        dataGridView1.Enabled = false;
                        reloadSecciones();
                        calcularCostoModulo();
                    }
                    else
                    {
                        MessageBox.Show("[Error] Se deben de eliminar los nuevos artículos para deshabilitar dicho componente.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    checkBox11.Checked = true;
                    dataGridView1.Enabled = true;
                    reloadSecciones();
                    calcularCostoModulo();
                }
            }
        }
        // --------------------------------------------------------------------------------->

        private bool checkNewItems(DataGridView table)
        {
            bool r = false;
            foreach(DataGridViewRow x in table.Rows)
            {
                if(x.Cells[0].Style.BackColor == Color.Yellow)
                {
                    r = true;
                    break;
                }
            }
            return r;
        }

        //add new item
        private void button8_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["new_articulo"] == null)
            {
                new new_articulo(new_costos).Show(this);
            }
            else
            {
                if (Application.OpenForms["new_articulo"].WindowState == FormWindowState.Minimized)
                {
                    Application.OpenForms["new_articulo"].WindowState = FormWindowState.Normal;
                }
                Application.OpenForms["new_articulo"].Select();
            }
        }

        //Obtener las instrucciones
        private void getInstruction(string x_clave="", float x_cant=0)
        {
            if (constants.enable_rules == true)
            {
                if (reglas != null)
                {
                    string[] instruc;
                    foreach (string x in reglas)
                    {
                        if (x.Length > 0)
                        {
                            instruc = x.Split(',');
                            if (instruc.Length > 0)
                            {
                                if (instruc[0] == "#D")
                                {
                                    if (instruc.Length == 7)
                                    {
                                        string[] n_claves = instruc[4].Split(':');
                                        if (n_claves.Length > 0)
                                        {
                                            foreach (string n in n_claves)
                                            {
                                                setNewCountAtDimension(instruc[1], constants.stringToFloat(instruc[2]), constants.stringToFloat(instruc[3]), n, constants.stringToFloat(instruc[5]), instruc[6]);
                                            }
                                        }
                                        else
                                        {
                                            setNewCountAtDimension(instruc[1], constants.stringToFloat(instruc[2]), constants.stringToFloat(instruc[3]), instruc[4], constants.stringToFloat(instruc[5]), instruc[6]);
                                        }
                                    }
                                    else if (instruc.Length == 8)
                                    {
                                        string[] n_claves = instruc[4].Split(':');
                                        if (n_claves.Length > 0)
                                        {
                                            foreach (string n in n_claves)
                                            {
                                                setNewCountAtDimension(instruc[1], constants.stringToFloat(instruc[2]), constants.stringToFloat(instruc[3]), n, constants.stringToFloat(instruc[5]), instruc[6], instruc[7]);
                                            }
                                        }
                                        else
                                        {
                                            setNewCountAtDimension(instruc[1], constants.stringToFloat(instruc[2]), constants.stringToFloat(instruc[3]), instruc[4], constants.stringToFloat(instruc[5]), instruc[6], instruc[7]);
                                        }
                                    }
                                    else if (instruc.Length == 10)
                                    {
                                        string[] n_claves = instruc[4].Split(':');
                                        if (n_claves.Length > 0)
                                        {
                                            foreach (string n in n_claves)
                                            {
                                                setNewCountAtDimension(instruc[1], constants.stringToFloat(instruc[2]), constants.stringToFloat(instruc[3]), n, constants.stringToFloat(instruc[5]), instruc[6], instruc[7], constants.stringToFloat(instruc[8]), instruc[9]);
                                            }
                                        }
                                        else
                                        {
                                            setNewCountAtDimension(instruc[1], constants.stringToFloat(instruc[2]), constants.stringToFloat(instruc[3]), instruc[4], constants.stringToFloat(instruc[5]), instruc[6], instruc[7], constants.stringToFloat(instruc[8]), instruc[9]);
                                        }
                                    }
                                }
                                else if (instruc[0] == "#C")
                                {
                                    if (instruc.Length == 5)
                                    {
                                        if (instruc[1] == x_clave)
                                        {
                                            setNewCountAtChangeCount(instruc[1], x_cant, instruc[2], constants.stringToFloat(instruc[3]), instruc[4]);
                                        }
                                    }
                                    else if (instruc.Length == 6)
                                    {
                                        if (instruc[1] == x_clave)
                                        {
                                            setNewCountAtChangeCount(instruc[1], x_cant, instruc[2], constants.stringToFloat(instruc[3]), instruc[4], constants.stringToFloat(instruc[5]));
                                        }
                                    }
                                    else if (instruc.Length == 7)
                                    {
                                        if (instruc[1] == x_clave)
                                        {
                                            setNewCountAtChangeCount(instruc[1], x_cant, instruc[2], constants.stringToFloat(instruc[3]), instruc[4], constants.stringToFloat(instruc[5]), instruc[6]);
                                        }
                                    }                                 
                                }                               
                                else if (instruc[0] == "#E")
                                {
                                    if (instruc.Length == 5)
                                    {
                                        string[] n_claves = instruc[1].Split(':');
                                        if (n_claves.Length > 0)
                                        {
                                            bool not = n_claves[0] == "NOT" ? true : false;
                                            if (not == false)
                                            {
                                                foreach (string n in n_claves)
                                                {
                                                    if (n == x_clave)
                                                    {
                                                        onChangeItem(n, instruc[2], instruc[3], instruc[4]);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (string n in n_claves)
                                                {
                                                    if (n != "NOT")
                                                    {
                                                        if (n != x_clave)
                                                        {
                                                            onChangeItem(n, instruc[2], instruc[3], instruc[4]);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            onChangeItem(instruc[1], instruc[2], instruc[3], instruc[4]);
                                        }
                                    }
                                    else if (instruc.Length == 6)
                                    {
                                        string[] n_claves = instruc[1].Split(':');
                                        if (n_claves.Length > 0)
                                        {
                                            bool not = n_claves[0] == "NOT" ? true : false;
                                            if (not == false)
                                            {
                                                foreach (string n in n_claves)
                                                {
                                                    if (n == x_clave)
                                                    {
                                                        onChangeItem(n, instruc[2], instruc[3], instruc[4], constants.stringToFloat(instruc[5]));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (string n in n_claves)
                                                {
                                                    if (n != "NOT")
                                                    {
                                                        if (n != x_clave)
                                                        {
                                                            onChangeItem(n, instruc[2], instruc[3], instruc[4], constants.stringToFloat(instruc[5]));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            onChangeItem(instruc[1], instruc[2], instruc[3], instruc[4], constants.stringToFloat(instruc[5]));
                                        }
                                    }
                                    else if (instruc.Length == 7)
                                    {
                                        string[] n_claves = instruc[1].Split(':');
                                        if (n_claves.Length > 0)
                                        {
                                            bool not = n_claves[0] == "NOT" ? true : false;
                                            if (not == false)
                                            {
                                                foreach (string n in n_claves)
                                                {
                                                    if (n == x_clave)
                                                    {
                                                        onChangeItem(n, instruc[2], instruc[3], instruc[4], constants.stringToFloat(instruc[5]), instruc[6]);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (string n in n_claves)
                                                {
                                                    if (n != "NOT")
                                                    {
                                                        if (n != x_clave)
                                                        {
                                                            onChangeItem(n, instruc[2], instruc[3], instruc[4], constants.stringToFloat(instruc[5]), instruc[6]);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            onChangeItem(instruc[1], instruc[2], instruc[3], instruc[4], constants.stringToFloat(instruc[5]), instruc[6]);
                                        }
                                    }
                                }
                                else if (instruc[0] == "#F")
                                {
                                    if (instruc.Length == 5)
                                    {
                                        setNewImageOnChangeCount(instruc[1], constants.stringToFloat(instruc[2]), constants.stringToInt(instruc[3]), instruc[4]);                                       
                                    }
                                    else if (instruc.Length == 6)
                                    {
                                        setNewImageOnChangeCount(instruc[1], constants.stringToFloat(instruc[2]), constants.stringToInt(instruc[3]), instruc[4], instruc[5]);
                                    }
                                }
                                else if (instruc[0] == "#G")
                                {
                                    if (instruc.Length == 6)
                                    {
                                        if (instruc[1] == x_clave)
                                        {
                                            changeItemOnCount(instruc[1], x_cant, instruc[2], constants.stringToFloat(instruc[3]), instruc[4], instruc[5]);
                                        }
                                    }
                                    else if (instruc.Length == 7)
                                    {
                                        if (instruc[1] == x_clave)
                                        {
                                            changeItemOnCount(instruc[1], x_cant, instruc[2], constants.stringToFloat(instruc[3]), instruc[4], instruc[5], constants.stringToFloat(instruc[6]));
                                        }
                                    }
                                    else if (instruc.Length == 8)
                                    {
                                        if (instruc[1] == x_clave)
                                        {
                                            changeItemOnCount(instruc[1], x_cant, instruc[2], constants.stringToFloat(instruc[3]), instruc[4], instruc[5], constants.stringToFloat(instruc[6]), instruc[7]);
                                        }
                                    }
                                    else if (instruc.Length == 9)
                                    {
                                        if (instruc[1] == x_clave)
                                        {
                                            changeItemOnCount(instruc[1], x_cant, instruc[2], constants.stringToFloat(instruc[3]), instruc[4], instruc[5], constants.stringToFloat(instruc[6]), instruc[7], instruc[8]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //finishing
                    recountItems();                   
                }
            }
        }

        //1ra instruccion
        private void setNewCountAtDimension(string limit, float largo, float alto, string clave, float count, string componente, string dim = "", float k = 0, string floor = "")
        {          
            dim = dim.ToLower();
            float z = 1;
            float a_t = alto_total;
            float l_t = largo_total;
            if (largo <= 0)
            {
                l_t = 0;
            }
            if (alto <= 0)
            {
                a_t = 0;
            }
            if (k > 0)
            {
                if (floor == "TRUE")
                {
                    z = (float)Math.Floor(largo_total / k);
                }
                else if (floor == "FALSE")
                {
                    z = (float)Math.Ceiling(largo_total / k);
                }
            }          
            if (componente == "PERFIL")
            {         
                foreach (DataGridViewRow x in dataGridView1.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (dim != "")
                        {
                            if (x.Cells[2].Value.ToString() == clave && x.Cells[5].Value.ToString() == dim)
                            {
                                if (limit == "MAX")
                                {
                                    if (l_t >= largo && a_t >= alto)
                                    {
                                        x.Cells[4].Value = (count < 0 ? constants.stringToFloat(x.Cells[4].Value.ToString()) : count) * z;
                                    }
                                }
                                else if (limit == "MIN")
                                {
                                    if (l_t <= largo && a_t <= alto)
                                    {
                                        x.Cells[4].Value = (count < 0 ? constants.stringToFloat(x.Cells[4].Value.ToString()) : count) * z;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (x.Cells[2].Value.ToString() == clave)
                            {
                                if (limit == "MAX")
                                {
                                    if (l_t >= largo && a_t >= alto)
                                    {
                                        x.Cells[4].Value = (count < 0 ? constants.stringToFloat(x.Cells[4].Value.ToString()) : count) * z;
                                    }
                                }
                                else if (limit == "MIN")
                                {
                                    if (l_t <= largo && a_t <= alto)
                                    {
                                        x.Cells[4].Value = (count < 0 ? constants.stringToFloat(x.Cells[4].Value.ToString()) : count) * z;
                                    }
                                }
                            }
                        }
                    }                   
                }
            }
            else if (componente == "CRISTAL")
            {
                foreach (DataGridViewRow x in dataGridView2.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (x.Cells[1].Value.ToString() == clave)
                        {
                            if (limit == "MAX")
                            {
                                if (l_t >= largo && a_t >= alto)
                                {
                                    x.Cells[3].Value = (count < 0 ? constants.stringToFloat(x.Cells[3].Value.ToString()) : count) * z;
                                }
                            }
                            else if (limit == "MIN")
                            {
                                if (l_t <= largo && a_t <= alto)
                                {
                                    x.Cells[3].Value = (count < 0 ? constants.stringToFloat(x.Cells[3].Value.ToString()) : count) * z;
                                }
                            }
                        }
                    }
                }
            }
            else if (componente == "HERRAJE")
            {
                foreach (DataGridViewRow x in dataGridView3.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (x.Cells[2].Value.ToString() == clave)
                        {
                            if (limit == "MAX")
                            {
                                if (l_t >= largo && a_t >= alto)
                                {
                                    x.Cells[4].Value = (count < 0 ? constants.stringToFloat(x.Cells[4].Value.ToString()) : count) * z;
                                }
                            }
                            else if (limit == "MIN")
                            {
                                if (l_t <= largo && a_t <= alto)
                                {
                                    x.Cells[4].Value = (count < 0 ? constants.stringToFloat(x.Cells[4].Value.ToString()) : count) * z;
                                }
                            }
                        }
                    }
                }
            }
            else if (componente == "OTROS")
            {
                foreach (DataGridViewRow x in dataGridView4.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (dim != "")
                        {
                            if (x.Cells[2].Value.ToString() == clave && x.Cells[5].Value.ToString() == dim)
                            {
                                if (limit == "MAX")
                                {
                                    if (l_t >= largo && a_t >= alto)
                                    {
                                        x.Cells[4].Value = (count < 0 ? constants.stringToFloat(x.Cells[4].Value.ToString()) : count) * z;
                                    }
                                }
                                else if (limit == "MIN")
                                {
                                    if (l_t <= largo && a_t <= alto)
                                    {
                                        x.Cells[4].Value = (count < 0 ? constants.stringToFloat(x.Cells[4].Value.ToString()) : count) * z;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (x.Cells[2].Value.ToString() == clave)
                            {
                                if (limit == "MAX")
                                {
                                    if (l_t >= largo && a_t >= alto)
                                    {
                                        x.Cells[4].Value = (count < 0 ? constants.stringToFloat(x.Cells[4].Value.ToString()) : count) * z;
                                    }
                                }
                                else if (limit == "MIN")
                                {
                                    if (l_t <= largo && a_t <= alto)
                                    {
                                        x.Cells[4].Value = (count < 0 ? constants.stringToFloat(x.Cells[4].Value.ToString()) : count) * z;
                                    }
                                }
                            }
                        }
                    }
                }
            }         
        }

        //2da instruccion
        private void setNewCountAtChangeCount(string x_clave, float x_cant, string y_clave, float y_cant, string componente, float z_cant = 0, string dim = "")
        {
            dim = dim.ToLower();          
            if (componente == "PERFIL")
            {
                foreach (DataGridViewRow x in dataGridView1.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (dim != "")
                        {
                            if (x.Cells[2].Value.ToString() == y_clave && x.Cells[5].Value.ToString() == dim)
                            {
                                x.Cells[4].Value = (x_cant * y_cant) + z_cant;
                            }
                        }
                        else
                        {
                            if (x.Cells[2].Value.ToString() == y_clave)
                            {
                                x.Cells[4].Value = (x_cant * y_cant) + z_cant;
                            }
                        }
                    }
                }
            }
            else if (componente == "CRISTAL")
            {
                foreach (DataGridViewRow x in dataGridView2.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (x.Cells[1].Value.ToString() == y_clave)
                        {
                            x.Cells[3].Value = (x_cant * y_cant) + z_cant;
                        }
                    }
                }
            }
            else if (componente == "HERRAJE")
            {
                foreach (DataGridViewRow x in dataGridView3.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (x.Cells[2].Value.ToString() == y_clave)
                        {
                            x.Cells[4].Value = (x_cant * y_cant) + z_cant;
                        }
                    }
                }
            }
            else if (componente == "OTROS")
            {
                foreach (DataGridViewRow x in dataGridView4.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (dim != "")
                        {
                            if (x.Cells[2].Value.ToString() == y_clave && x.Cells[5].Value.ToString() == dim)
                            {
                                x.Cells[4].Value = (x_cant * y_cant) + z_cant;
                            }
                        }
                        else
                        {
                            if (x.Cells[2].Value.ToString() == y_clave)
                            {
                                x.Cells[4].Value = (x_cant * y_cant) + z_cant;
                            }
                        }
                    }
                }
            }
        }

        //3ra instruccion
        private void onChangeItem(string x_clave, string y_clave, string new_y_clave, string componente, float cantidad=-1, string dim="")
        {
            listas_entities_pva listas = new listas_entities_pva();
            dim = dim.ToLower();
            if (componente == "PERFIL")
            {
                foreach (DataGridViewRow x in dataGridView1.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (dim != "")
                        {
                            if (x.Cells[2].Value.ToString() == y_clave && x.Cells[5].Value.ToString() == dim)
                            {
                                var perfiles = (from v in listas.perfiles where v.clave == new_y_clave select v).SingleOrDefault();

                                if (perfiles != null)
                                {
                                    dataGridView1.BeginEdit(true);
                                    x.Cells[1].Value = perfiles.id;
                                    x.Cells[2].Value = perfiles.clave;
                                    x.Cells[3].Value = perfiles.articulo;
                                    if (cantidad >= 0)
                                    {
                                        x.Cells[4].Value = cantidad;
                                    }
                                    dataGridView1.EndEdit();
                                }
                            }
                        }
                        else
                        {
                            if (x.Cells[2].Value.ToString() == y_clave)
                            {
                                var perfiles = (from v in listas.perfiles where v.clave == new_y_clave select v).SingleOrDefault();

                                if (perfiles != null)
                                {
                                    dataGridView1.BeginEdit(true);
                                    x.Cells[1].Value = perfiles.id;
                                    x.Cells[2].Value = perfiles.clave;
                                    x.Cells[3].Value = perfiles.articulo;
                                    if (cantidad >= 0)
                                    {
                                        x.Cells[4].Value = cantidad;
                                    }
                                    dataGridView1.EndEdit();
                                }
                            }
                        }
                    }
                }
            }
            else if (componente == "HERRAJE")
            {
                foreach (DataGridViewRow x in dataGridView3.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (x.Cells[2].Value.ToString() == y_clave)
                        {
                            var herrajes = (from v in listas.herrajes where v.clave == new_y_clave select v).SingleOrDefault();

                            if (herrajes != null)
                            {
                                dataGridView3.BeginEdit(true);
                                x.Cells[1].Value = herrajes.id;
                                x.Cells[2].Value = herrajes.clave;
                                x.Cells[3].Value = herrajes.articulo;
                                if (cantidad >= 0)
                                {
                                    x.Cells[4].Value = cantidad;
                                }
                                x.Cells[6].Value = herrajes.color;
                                dataGridView3.EndEdit();
                            }
                        }
                    }
                }
            }
            else if (componente == "CRISTALES")
            {
                foreach (DataGridViewRow x in dataGridView2.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (x.Cells[1].Value.ToString() == y_clave)
                        {
                            var cristales = (from v in listas.lista_costo_corte_e_instalado where v.clave == new_y_clave select v).SingleOrDefault();

                            if (cristales != null)
                            {
                                dataGridView2.BeginEdit(true);
                                x.Cells[1].Value = cristales.clave;
                                x.Cells[2].Value = cristales.articulo;
                                if (cantidad >= 0)
                                {
                                    x.Cells[3].Value = cantidad;
                                }
                                dataGridView2.EndEdit();
                            }
                        }
                    }
                }
            }
            else if (componente == "OTROS")
            {
                foreach (DataGridViewRow x in dataGridView4.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (dim != "")
                        {
                            if (x.Cells[2].Value.ToString() == y_clave && x.Cells[5].Value.ToString() == dim)
                            {
                                var otros = (from v in listas.otros where v.clave == new_y_clave select v).SingleOrDefault();

                                if (otros != null)
                                {
                                    dataGridView4.BeginEdit(true);
                                    x.Cells[1].Value = otros.id;
                                    x.Cells[2].Value = otros.clave;
                                    x.Cells[3].Value = otros.articulo;
                                    if (cantidad >= 0)
                                    {
                                        x.Cells[4].Value = cantidad;
                                    }
                                    x.Cells[7].Value = otros.color;
                                    dataGridView4.EndEdit();
                                }
                            }
                        }
                        else
                        {
                            if (x.Cells[2].Value.ToString() == y_clave)
                            {
                                var otros = (from v in listas.otros where v.clave == new_y_clave select v).SingleOrDefault();

                                if (otros != null)
                                {
                                    dataGridView4.BeginEdit(true);
                                    x.Cells[1].Value = otros.id;
                                    x.Cells[2].Value = otros.clave;
                                    x.Cells[3].Value = otros.articulo;
                                    if (cantidad >= 0)
                                    {
                                        x.Cells[4].Value = cantidad;
                                    }
                                    x.Cells[7].Value = otros.color;
                                    dataGridView4.EndEdit();
                                }
                            }
                        }
                    }
                }
            }
        }

        //4ta instruccion
        private void setNewImageOnChangeCount(string clave, float cantidad, int id_diseño, string componente, string dim="")
        {
            listas_entities_pva listas = new listas_entities_pva();
            dim = dim.ToLower();
            if (componente == "PERFIL")
            {
                foreach (DataGridViewRow x in dataGridView1.Rows)
                {
                    if (dim != "")
                    {
                        if (x.Cells[2].Value.ToString() == clave && x.Cells[5].Value.ToString() == dim)
                        {
                            if (constants.stringToFloat(x.Cells[4].Value.ToString()) == cantidad)
                            {
                                panel = null;
                                tableLayoutPanel1.Controls.Clear();
                                loadDiseño(id_diseño, listas);
                                checkDimensions(false);
                            }
                        }
                    }
                    else
                    {
                        if (x.Cells[2].Value.ToString() == clave)
                        {
                            if (constants.stringToFloat(x.Cells[4].Value.ToString()) == cantidad)
                            {
                                panel = null;
                                tableLayoutPanel1.Controls.Clear();
                                loadDiseño(id_diseño, listas);
                                checkDimensions(false);
                            }
                        }
                    }
                }
            }
            else if (componente == "HERRAJE")
            {
                foreach (DataGridViewRow x in dataGridView3.Rows)
                {
                    if (x.Cells[2].Value.ToString() == clave)
                    {
                        if (constants.stringToFloat(x.Cells[4].Value.ToString()) == cantidad)
                        {
                            panel = null;
                            tableLayoutPanel1.Controls.Clear();
                            loadDiseño(id_diseño, listas);
                            checkDimensions(false);
                        }
                    }
                }
            }
            else if (componente == "CRISTALES")
            {
                foreach (DataGridViewRow x in dataGridView2.Rows)
                {
                    if (x.Cells[1].Value.ToString() == clave)
                    {
                        if (constants.stringToFloat(x.Cells[3].Value.ToString()) == cantidad)
                        {
                            panel = null;
                            tableLayoutPanel1.Controls.Clear();
                            loadDiseño(id_diseño, listas);
                            checkDimensions(false);
                        }
                    }
                }
            }
            else if (componente == "OTROS")
            {
                foreach (DataGridViewRow x in dataGridView4.Rows)
                {
                    if (dim != "")
                    {
                        if (x.Cells[2].Value.ToString() == clave && x.Cells[5].Value.ToString() == dim)
                        {
                            if (constants.stringToFloat(x.Cells[4].Value.ToString()) == cantidad)
                            {
                                panel = null;
                                tableLayoutPanel1.Controls.Clear();
                                loadDiseño(id_diseño, listas);
                                checkDimensions(false);
                            }
                        }
                    }
                    else
                    {
                        if (x.Cells[2].Value.ToString() == clave)
                        {
                            if (constants.stringToFloat(x.Cells[4].Value.ToString()) == cantidad)
                            {
                                panel = null;
                                tableLayoutPanel1.Controls.Clear();
                                loadDiseño(id_diseño, listas);
                                checkDimensions(false);
                            }
                        }
                    }
                }
            }
            //Cargar el acabado de la cortina
            foreach(Control x in panel.Controls)
            {
                if (comboBox1.Text != "")
                {
                    constants.setBackgroundImg("acabados_perfil", comboBox1.Text, "jpg", x);
                }
                else if(comboBox3.Text != "")
                {
                    constants.setBackgroundImg("acabados_especiales", comboBox3.Text, "jpg", x);
                }
            }
            if (checkBox1.Checked == true)
            {
                if (comboBox1.Text != "")
                {
                    constants.setBackgroundImg("acabados_perfil", comboBox1.Text, "jpg", panel);
                }
                else if(comboBox3.Text != "")
                {
                    constants.setBackgroundImg("acabados_especiales", comboBox3.Text, "jpg", panel);
                }
            }
            //--------------------------------->
        }

        //5ta instruccion
        private void changeItemOnCount(string x_clave, float x_cant, string y_clave, float y_cant, string new_y_clave, string componente, float z_cant = 0, string dim = "", string changeCount_y = "TRUE")
        {
            listas_entities_pva listas = new listas_entities_pva();
            dim = dim.ToLower();
            if (componente == "PERFIL")
            {
                foreach (DataGridViewRow x in dataGridView1.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (dim != "")
                        {
                            if (x.Cells[2].Value.ToString() == y_clave && x.Cells[5].Value.ToString() == dim)
                            {
                                var perfiles = (from v in listas.perfiles where v.clave == new_y_clave select v).SingleOrDefault();

                                if (perfiles != null)
                                {
                                    x.Cells[1].Value = perfiles.id;
                                    x.Cells[2].Value = perfiles.clave;
                                    x.Cells[3].Value = perfiles.articulo;
                                    if (changeCount_y == "TRUE")
                                    {
                                        x.Cells[4].Value = (x_cant * y_cant) + z_cant;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (x.Cells[2].Value.ToString() == y_clave)
                            {
                                var perfiles = (from v in listas.perfiles where v.clave == new_y_clave select v).SingleOrDefault();

                                if (perfiles != null)
                                {
                                    x.Cells[1].Value = perfiles.id;
                                    x.Cells[2].Value = perfiles.clave;
                                    x.Cells[3].Value = perfiles.articulo;
                                    if (changeCount_y == "TRUE")
                                    {
                                        x.Cells[4].Value = (x_cant * y_cant) + z_cant;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (componente == "CRISTAL")
            {
                foreach (DataGridViewRow x in dataGridView2.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (x.Cells[1].Value.ToString() == y_clave)
                        {
                            var cristales = (from v in listas.lista_costo_corte_e_instalado where v.clave == new_y_clave select v).SingleOrDefault();

                            if (cristales != null)
                            {
                                x.Cells[1].Value = cristales.clave;
                                x.Cells[2].Value = cristales.articulo;
                                if (changeCount_y == "TRUE")
                                {
                                    x.Cells[3].Value = (x_cant * y_cant) + z_cant;
                                }
                            }
                        }
                    }
                }
            }
            else if (componente == "HERRAJE")
            {
                foreach (DataGridViewRow x in dataGridView3.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (x.Cells[2].Value.ToString() == y_clave)
                        {
                            var herrajes = (from v in listas.herrajes where v.clave == new_y_clave select v).SingleOrDefault();

                            if (herrajes != null)
                            {
                                x.Cells[1].Value = herrajes.id;
                                x.Cells[2].Value = herrajes.clave;
                                x.Cells[3].Value = herrajes.articulo;
                                if (changeCount_y == "TRUE")
                                {
                                    x.Cells[4].Value = (x_cant * y_cant) + z_cant;
                                }
                                x.Cells[6].Value = herrajes.color;
                            }
                        }
                    }
                }
            }
            else if (componente == "OTROS")
            {
                foreach (DataGridViewRow x in dataGridView4.Rows)
                {
                    if (x.Cells.Count > 0)
                    {
                        if (dim != "")
                        {
                            if (x.Cells[2].Value.ToString() == y_clave && x.Cells[5].Value.ToString() == dim)
                            {
                                var otros = (from v in listas.otros where v.clave == new_y_clave select v).SingleOrDefault();

                                if (otros != null)
                                {
                                    x.Cells[1].Value = otros.id;
                                    x.Cells[2].Value = otros.clave;
                                    x.Cells[3].Value = otros.articulo;
                                    if (changeCount_y == "TRUE")
                                    {
                                        x.Cells[4].Value = (x_cant * y_cant) + z_cant;
                                    }
                                    x.Cells[7].Value = otros.color;
                                }
                            }
                        }
                        else
                        {
                            if (x.Cells[2].Value.ToString() == y_clave)
                            {
                                var otros = (from v in listas.otros where v.clave == new_y_clave select v).SingleOrDefault();

                                if (otros != null)
                                {
                                    x.Cells[1].Value = otros.id;
                                    x.Cells[2].Value = otros.clave;
                                    x.Cells[3].Value = otros.articulo;
                                    if (changeCount_y == "TRUE")
                                    {
                                        x.Cells[4].Value = (x_cant * y_cant) + z_cant;
                                    }
                                    x.Cells[7].Value = otros.color;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void setNewItem(int concept, string clave, string cantidad="0", string ubicacion="", string seccion="", string acabado="")
        {
            listas_entities_pva listas = new listas_entities_pva();
            switch (concept)
            {
                case 1:
                    var perfil = (from x in listas.perfiles where x.clave == clave select x).SingleOrDefault();
                    if (perfil != null)
                    {
                        dataGridView1.Rows.Add("Perfil", perfil.id, clave, perfil.articulo, cantidad, ubicacion, seccion, acabado);
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0].Style.BackColor = Color.Yellow;
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[5].Style.BackColor = Color.FromArgb(192, 192, 255);
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[6].Style.BackColor = Color.FromArgb(192, 192, 255);
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[5].ReadOnly = false;
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[6].ReadOnly = false;
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
                        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Index;                        
                    }
                    break;
                case 2:
                    var cristal = (from x in listas.lista_costo_corte_e_instalado where x.clave == clave select x).SingleOrDefault();
                    if (cristal != null)
                    {
                        dataGridView2.Rows.Add("Cristal", clave, cristal.articulo, cantidad, seccion);
                        dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[0].Style.BackColor = Color.Yellow;
                        dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[4].Style.BackColor = Color.FromArgb(192, 192, 255);
                        dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[4].ReadOnly = false;
                        dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[4].Selected = true;
                        dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView2.Rows[dataGridView2.Rows.Count - 1].Index;                       
                    }
                    break;
                case 3:
                    var herraje = (from c in listas.herrajes where c.clave == clave select c).SingleOrDefault();
                    if (herraje != null)
                    {
                        dataGridView3.Rows.Add("Herraje", herraje.id, clave, herraje.articulo, cantidad, seccion, herraje.color);
                        dataGridView3.Rows[dataGridView3.Rows.Count - 1].Cells[0].Style.BackColor = Color.Yellow;
                        dataGridView3.Rows[dataGridView3.Rows.Count - 1].Cells[5].Style.BackColor = Color.FromArgb(192, 192, 255);
                        dataGridView3.Rows[dataGridView3.Rows.Count - 1].Cells[5].ReadOnly = false;
                        dataGridView3.Rows[dataGridView3.Rows.Count - 1].Selected = true;
                        dataGridView3.FirstDisplayedScrollingRowIndex = dataGridView3.Rows[dataGridView3.Rows.Count - 1].Index;                     
                    }
                    break;
                case 4:
                    var otros = (from c in listas.otros where c.clave == clave select c).SingleOrDefault();
                    if (otros != null)
                    {
                        dataGridView4.Rows.Add("Otros", otros.id, clave, otros.articulo, cantidad, ubicacion, seccion, otros.color);
                        dataGridView4.Rows[dataGridView4.Rows.Count - 1].Cells[0].Style.BackColor = Color.Yellow;
                        dataGridView4.Rows[dataGridView4.Rows.Count - 1].Cells[5].Style.BackColor = Color.FromArgb(192, 192, 255);
                        dataGridView4.Rows[dataGridView4.Rows.Count - 1].Cells[6].Style.BackColor = Color.FromArgb(192, 192, 255);
                        dataGridView4.Rows[dataGridView4.Rows.Count - 1].Cells[5].ReadOnly = false;
                        dataGridView4.Rows[dataGridView4.Rows.Count - 1].Cells[6].ReadOnly = false;
                        dataGridView4.Rows[dataGridView4.Rows.Count - 1].Selected = true;
                        dataGridView4.FirstDisplayedScrollingRowIndex = dataGridView4.Rows[dataGridView4.Rows.Count - 1].Index;                        
                    }
                    break;
                default:
                    break;
            }          
        }

        private void comboBoxSelection(DataGridView datagrid)
        {
            if (datagrid.CurrentCell.OwningColumn.HeaderText == "Ubicación" && datagrid.CurrentRow.Cells[0].Style.BackColor == Color.Yellow)
            {
                if (datagrid.CurrentRow.Cells[0].Value.ToString() == "Perfil")
                {
                    //---> importante
                    if (datagrid.CurrentCell.Value == null)
                    {
                        datagrid.CurrentCell.Value = "";
                    }
                    //
                    datagrid.CurrentCell.ReadOnly = false;
                    DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                    string u = string.Empty;
                    cell.Items.Clear();
                    cell.Items.AddRange("largo", "alto");
                    foreach (string x in cell.Items)
                    {
                        if (x == datagrid.CurrentCell.Value.ToString())
                        {
                            u = datagrid.CurrentCell.Value.ToString();
                        }
                    }
                    if (u == string.Empty)
                    {
                        datagrid.CurrentCell.Value = "";
                    }
                    cell.Value = u;
                    datagrid.CurrentRow.Cells[datagrid.CurrentCell.ColumnIndex] = cell;
                    cell.Dispose();
                }
                else if (datagrid.CurrentRow.Cells[0].Value.ToString() == "Otros")
                {
                    listas_entities_pva listas = new listas_entities_pva();
                    int id = constants.stringToInt(datagrid.CurrentRow.Cells[1].Value.ToString());
                    var otros = (from x in listas.otros where x.id == id select x).SingleOrDefault();

                    if (otros != null)
                    {
                        if ((otros.largo > 0 && otros.alto == 0) || (otros.largo == 0 && otros.alto > 0))
                        {
                            //---> importante
                            if (datagrid.CurrentCell.Value == null)
                            {
                                datagrid.CurrentCell.Value = "";
                            }
                            //
                            datagrid.CurrentCell.ReadOnly = false;
                            DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                            string u = string.Empty;
                            cell.Items.Clear();
                            cell.Items.AddRange("largo", "alto");
                            foreach (string x in cell.Items)
                            {
                                if (x == datagrid.CurrentCell.Value.ToString())
                                {
                                    u = datagrid.CurrentCell.Value.ToString();
                                }
                            }
                            if (u == string.Empty)
                            {
                                datagrid.CurrentCell.Value = "";
                            }
                            cell.Value = u;
                            datagrid.CurrentRow.Cells[datagrid.CurrentCell.ColumnIndex] = cell;
                            cell.Dispose();
                        }
                        else
                        {
                            datagrid.CurrentCell.ReadOnly = true;
                        }
                    }
                }
                else
                {
                    datagrid.CurrentCell.ReadOnly = true;
                }
            }
            else if (datagrid.CurrentCell.OwningColumn.HeaderText == "Sección" && datagrid.CurrentRow.Cells[0].Style.BackColor == Color.Yellow)
            {
                setSecciones(datagrid.CurrentCell, datagrid.CurrentRow);               
            }
        }

        private void setSecciones(DataGridViewCell d_cell, DataGridViewRow d_row)
        {
            List<string> secciones = new List<string>();
            foreach (DataGridViewRow x in dataGridView5.Rows)
            {
                secciones.Add(x.Cells[0].Value.ToString());
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
            cell.Items.AddRange(secciones.ToArray());
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

        public void selectTab(int concept)
        {
            switch (concept)
            {
                case 1:
                    tabControl1.SelectedTab = tabPage1;
                    break;
                case 2:
                    tabControl1.SelectedTab = tabPage2;
                    break;
                case 3:
                    tabControl1.SelectedTab = tabPage3;
                    break;
                case 4:
                    tabControl1.SelectedTab = tabPage4;
                    break;
                default: break;
            }
        }

        //incluir mosquiteros
        private void CheckBox19_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Se restablecerán los componentes predeterminados a este módulo. ¿Desea continuar?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {
                int m_id = getModuloMosquiteros(label7.Text, label7.Text.Contains("S/M") == true ? "C/M" : label7.Text.Contains("C/M") == true ? "S/M" : "");
                if (checkBox19.Checked)
                {
                    if (m_id > 0)
                    {
                        resetSession(m_id, id_cotizacion, false);
                        richTextBox1.Clear();
                    }
                    else
                    {
                        checkBox19.Checked = true;
                        MessageBox.Show("[Error] no existe dicho diseño de apertura con mosquitero.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (m_id > 0)
                    {                       
                        resetSession(m_id, id_cotizacion, false);
                        richTextBox1.Clear();
                    }
                    else
                    {
                        checkBox19.Checked = false;
                        MessageBox.Show("[Error] no existe dicho diseño de apertura sin mosquitero.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                if (checkBox19.Checked)
                {
                    checkBox19.Checked = true;
                }
                else
                {
                    checkBox19.Checked = false;
                }
            }          
        }

        private int getModuloMosquiteros(string a_name, string c)
        {
            if (c != "")
            {
                listas_entities_pva listas = new listas_entities_pva();
                string linea = label8.Text;
                if (c == "C/M")
                {
                    a_name = a_name.Replace("S/M", "") + c;
                }
                else if(c == "S/M")
                {
                    a_name = a_name.Replace("C/M", "") + c;
                }
                var modulos = (from x in listas.modulos where x.linea == linea && x.articulo == a_name select x).SingleOrDefault();

                if (modulos != null)
                {
                    return modulos.id;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        //siempre permitir el ajuste de los componentes
        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox21.Checked)
            {
                checkBox6.Checked = true;
                checkBox7.Checked = true;
                checkBox15.Checked = true;
                checkBox2.Checked = true;
                checkBox4.Checked = true;
                checkBox3.Checked = true;
                checkBox5.Checked = true;
                constants.siempre_permitir_ac = true;
            }
            else
            {
                checkBox6.Checked = false;
                checkBox7.Checked = false;
                checkBox15.Checked = false;
                checkBox2.Checked = false;
                checkBox4.Checked = false;
                checkBox3.Checked = false;
                checkBox5.Checked = false;
                constants.siempre_permitir_ac = false;
            }

            try
            {
                XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                var spac = from x in opciones_xml.Descendants("Opciones") select x;

                foreach (XElement x in spac)
                {
                    x.SetElementValue("SPAC", constants.siempre_permitir_ac == true ? "true" : "false");
                }
                opciones_xml.Save(constants.opciones_xml);
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Cortinas
        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            setSistManual();
        }

        private void setSistManual()
        {
            if (label18.Text == "CM")
            {
                if (checkBox17.Checked)
                {
                    if (peso_aluminio <= constants.lim_sm)
                    {
                        checkBox18.Checked = false;
                        string clave = string.Empty;
                        listas_entities_pva listas = new listas_entities_pva();

                        foreach (DataGridViewRow x in dataGridView4.Rows)
                        {
                            clave = x.Cells[2].Value.ToString();
                            var otros = (from v in listas.otros where v.clave == clave select v).SingleOrDefault();

                            if (otros != null)
                            {
                                if (otros.linea == "carrete" || otros.linea == "motores")
                                {
                                    x.Cells[4].Value = "0";
                                }
                            }
                        }

                        foreach (DataGridViewRow x in dataGridView4.Rows)
                        {
                            clave = x.Cells[2].Value.ToString();
                            var otros = (from v in listas.otros where v.clave == clave select v).SingleOrDefault();

                            if (otros != null)
                            {
                                if (otros.linea == "carrete")
                                {
                                    x.Cells[4].Value = "1";
                                }
                            }
                        }
                        calcularCostoModulo();
                        label42.ForeColor = Color.Green;
                    }
                    else
                    {
                        MessageBox.Show("[Error] El peso de la cortina no es el indicado para un sistema de elevación manual.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        checkBox17.Checked = false;
                    }
                }
                else
                {
                    calcularCostoModulo();
                }
            }
        }

        private void checkBox18_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox18.Checked)
            {             
                checkBox17.Checked = false;
                validMotor();
                calcularCostoModulo();
            }
            else
            {
                calcularCostoModulo();
            }         
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            if (!constants.isFloat(textBox14.Text))
            {
                textBox14.Clear();
            }         
        }

        private void config_modulo_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
            textBox1.Select();
        }

        private void validMotor()
        {
            if (label18.Text == "CM")
            {
                if (dataGridView4.RowCount > 0)
                {
                    string clave = string.Empty;
                    listas_entities_pva listas = new listas_entities_pva();

                    //Put all to 0
                    foreach (DataGridViewRow x in dataGridView4.Rows)
                    {
                        clave = x.Cells[2].Value.ToString();
                        var otros = (from v in listas.otros where v.clave == clave select v).SingleOrDefault();

                        if (otros != null)
                        {
                            if (otros.linea == "carrete" || otros.linea == "motores")
                            {
                                x.Cells[4].Value = "0";
                            }
                        }
                    }

                    bool error = true;

                    //Set motor
                    foreach (DataGridViewRow x in dataGridView4.Rows)
                    {
                        clave = x.Cells[2].Value.ToString();
                        var otros = (from v in listas.otros where v.clave == clave select v).SingleOrDefault();
                        float peso_motor = 0;

                        if (otros != null)
                        {
                            if (otros.linea == "motores")
                            {
                                peso_motor = constants.stringToFloat(otros.caracteristicas.ToString());
                                if (peso_motor >= 100)
                                {
                                    if (peso_aluminio < peso_motor)
                                    {
                                        x.Cells[4].Value = "1";
                                        getInstruction(x.Cells[2].Value.ToString(), 1);
                                        error = false;
                                        break;
                                    }
                                    else
                                    {
                                        x.Cells[4].Value = Math.Ceiling(peso_aluminio / peso_motor);
                                        getInstruction(x.Cells[2].Value.ToString(), 1);
                                        error = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (peso_aluminio < peso_motor)
                                    {
                                        x.Cells[4].Value = "1";
                                        getInstruction(x.Cells[2].Value.ToString(), 1);
                                        error = false;
                                        break;
                                    }
                                }                               
                            }
                        }
                    }

                    if (error == true)
                    {
                        MessageBox.Show("[Error] No hay motor que se ajuste al peso de la cortina.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        checkBox18.Checked = false;
                        label42.ForeColor = Color.Red;
                    }
                    else
                    {
                        label42.ForeColor = Color.Green;
                    }
                }
            }      
        }
        ////---------------------------------------------------------------------------------------------------------->

        public void setNewCosto(List<string> new_costos)
        {
            this.new_costos = new_costos;
            displayNewCosto();
            calcularCostoModulo();
        }

        private void displayNewCosto()
        {
            if (new_costos.Count > 0)
            {
                label43.Text = "*Nota: este artículo incluye costos adicionales.";
            }
            else
            {
                label43.Text = string.Empty;
            }
        }

        //Variaciones
        private void button10_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["variaciones"] == null)
            {
                new variaciones(label8.Text).Show(this);
            }
            else
            {
                if (Application.OpenForms["variaciones"].WindowState == FormWindowState.Minimized)
                {
                    Application.OpenForms["variaciones"].WindowState = FormWindowState.Normal;
                }
                Application.OpenForms["variaciones"].Select();
            }
        }

        //Tipos de Apertura
        private void getLineaCompleta(string linea)
        {
            listas_entities_pva listas = new listas_entities_pva();

            var modulos = (from x in listas.modulos where x.linea == linea orderby x.articulo ascending select x.articulo);

            if (modulos != null)
            {
                comboBox4.Items.Clear();
                foreach (string x in modulos)
                {
                    comboBox4.Items.Add(x);
                }
            }
        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox16.Checked)
            {
                constants.ajustar_medidas_aut = true;
            }
            else
            {
                constants.ajustar_medidas_aut = false;
            }
            try
            {
                XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                var ajma = from x in opciones_xml.Descendants("Opciones") select x;

                foreach (XElement x in ajma)
                {
                    x.SetElementValue("AJMA", constants.ajustar_medidas_aut == true ? "true" : "false");
                }
                opciones_xml.Save(constants.opciones_xml);
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Cambiar Tipo de Apertura de la misma linea...
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.SelectedIndex > -1)
            {
                DialogResult r = MessageBox.Show("Se restablecerán los componentes predeterminados a este módulo. ¿Desea continuar?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (r == DialogResult.Yes)
                {
                    listas_entities_pva listas = new listas_entities_pva();
                    string m = comboBox4.Text;
                    var modulo = (from x in listas.modulos where x.articulo == m select x).SingleOrDefault();
                    if (modulo != null)
                    {
                        int _id = modulo.id;
                        resetSession(_id, id_cotizacion, false);
                        richTextBox1.Clear();
                    }
                }
                else
                {
                    comboBox4.SelectedIndex = -1;
                }
            }
        }

        public void setComponentEnable(int component)
        {
            if (component == 1)
            {
                if (dataGridView1.RowCount > 0)
                {
                    if (!checkBox11.Checked)
                    {
                        checkBox11.Checked = true;
                        dataGridView1.Enabled = true;
                    }
                }
            }
            else if(component == 2)
            {
                if (dataGridView2.RowCount > 0)
                {
                    if (!checkBox12.Checked)
                    {
                        checkBox12.Checked = true;
                        dataGridView2.Enabled = true;
                    }
                }
            }
            else if (component == 3)
            {
                if (dataGridView3.RowCount > 0)
                {
                    if (!checkBox13.Checked)
                    {
                        checkBox13.Checked = true;
                        dataGridView3.Enabled = true;
                    }
                }
            }
            else if (component == 4)
            {
                if (dataGridView4.RowCount > 0)
                {
                    if (!checkBox14.Checked)
                    {
                        checkBox14.Checked = true;
                        dataGridView4.Enabled = true;
                    }
                }
            }
        }

        public void loadVariaciones(string cambios, string nuevos, string descripcion)
        {
            try
            {
                listas_entities_pva listas = new listas_entities_pva();
                int cp = -1;
                string clave = string.Empty;
                string[] c = cambios.Split(',');
                string[] p = null;
                bool error = false;
                bool p_error = false;

                foreach (string x in c)
                {
                    p = x.Split(':');
                    if (p.Length == 4)
                    {
                        cp = constants.stringToInt(p[0]);
                        if (cp == 1)
                        {
                            p_error = false;
                            foreach (DataGridViewRow r in dataGridView1.Rows)
                            {
                                if (r.Cells[2].Value.ToString() == p[3])
                                {
                                    clave = p[1];
                                    var perfiles = (from n in listas.perfiles where n.clave == clave select n).SingleOrDefault();
                                    if (perfiles != null)
                                    {
                                        p_error = true;
                                        r.Cells[1].Value = perfiles.id;
                                        r.Cells[2].Value = clave;
                                        r.Cells[3].Value = perfiles.articulo;
                                        if (p[2] != "-1")
                                        {
                                            r.Cells[4].Value = p[2];
                                        }
                                        getInstruction(clave);
                                        getSeccionesReady(constants.stringToInt(r.Cells[6].Value.ToString()));
                                    }
                                }
                            }
                            if (!p_error)
                            {
                                error = true;
                            }
                        }
                        else if (cp == 2)
                        {
                            p_error = false;
                            foreach (DataGridViewRow r in dataGridView2.Rows)
                            {
                                if (r.Cells[2].Value.ToString() == p[3])
                                {
                                    clave = p[1];
                                    var cristales = (from n in listas.lista_costo_corte_e_instalado where n.clave == clave select n).SingleOrDefault();
                                    if (cristales != null)
                                    {
                                        p_error = true;
                                        r.Cells[1].Value = clave;
                                        r.Cells[2].Value = cristales.articulo;
                                        if (p[2] != "-1")
                                        {
                                            r.Cells[3].Value = p[2];
                                        }
                                        getInstruction(clave);
                                        getSeccionesReady(constants.stringToInt(r.Cells[4].Value.ToString()));
                                    }
                                }
                            }
                            if (!p_error)
                            {
                                error = true;
                            }
                        }
                        else if (cp == 3)
                        {
                            p_error = false;
                            foreach (DataGridViewRow r in dataGridView3.Rows)
                            {
                                if (r.Cells[2].Value.ToString() == p[3])
                                {
                                    clave = p[1];
                                    var herrajes = (from n in listas.herrajes where n.clave == clave select n).SingleOrDefault();
                                    if (herrajes != null)
                                    {
                                        p_error = true;
                                        r.Cells[1].Value = herrajes.id;
                                        r.Cells[2].Value = clave;
                                        r.Cells[3].Value = herrajes.articulo;
                                        if (p[2] != "-1")
                                        {
                                            r.Cells[4].Value = p[2];
                                        }
                                        getInstruction(clave);
                                        getSeccionesReady(constants.stringToInt(r.Cells[5].Value.ToString()));
                                    }
                                }
                            }
                            if (!p_error)
                            {
                                error = true;
                            }
                        }
                        else if (cp == 4)
                        {
                            p_error = false;
                            foreach (DataGridViewRow r in dataGridView4.Rows)
                            {
                                if (r.Cells[2].Value.ToString() == p[3])
                                {
                                    clave = p[1];
                                    var otros = (from n in listas.otros where n.clave == clave select n).SingleOrDefault();
                                    if (otros != null)
                                    {
                                        p_error = true;
                                        r.Cells[1].Value = otros.id;
                                        r.Cells[2].Value = clave;
                                        r.Cells[3].Value = otros.articulo;
                                        if (p[2] != "-1")
                                        {
                                            r.Cells[4].Value = p[2];
                                        }
                                        getInstruction(clave);
                                        getSeccionesReady(constants.stringToInt(r.Cells[6].Value.ToString()));
                                    }
                                }
                            }
                            if (!p_error)
                            {
                                error = true;
                            }
                        }
                    }
                }
                            
                //Nuevos
                cp = -1;
                clave = string.Empty;
                c = nuevos.Split(',');
                p = null;
                foreach (string x in c)
                {
                    p = x.Split(':');
                    if (p.Length == 4)
                    {
                        setNewItem(constants.stringToInt(p[0]), p[1], p[2] == "-1" ? "0" : p[2]);
                    }
                }
                //End   
                if (!error)
                {
                    if (richTextBox1.TextLength > 0)
                    {
                        if (!richTextBox1.Text.Contains(descripcion.ToUpper()))
                        {
                            richTextBox1.Text = richTextBox1.Text + "\n-" + descripcion.ToUpper();
                        }
                    }
                    else
                    {
                        richTextBox1.Text = "-" + descripcion.ToUpper();
                    }
                }
                else
                {
                    MessageBox.Show("[Error] al parecer este módulo no es 100% compatible con esta variación.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                calcularCostoModulo();
            }
            catch(Exception)
            {
                MessageBox.Show("[Error] se produjo un error al cargar la variación.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
