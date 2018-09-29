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
    public partial class change_colors : Form
    {
        List<string> esquemas = new List<string>();
        cotizaciones_local cotizacion = new cotizaciones_local();
        int id;

        public change_colors(int id=0, string articulo="", string largo="", string alto="")
        {
            InitializeComponent();
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            tableLayoutPanel1.BackgroundImageLayout = ImageLayout.Stretch;
            backgroundWorker1.WorkerSupportsCancellation = true;
            FormClosing += Change_colors_FormClosing;
            backgroundWorker2.RunWorkerCompleted += BackgroundWorker2_RunWorkerCompleted;
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.ProgressChanged += BackgroundWorker2_ProgressChanged;
            backgroundWorker3.RunWorkerCompleted += BackgroundWorker3_RunWorkerCompleted;
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Image = Properties.Resources.wizard_icon_128;
            this.id = id;
            if (articulo != "")
            {
                label4.Text = articulo;
            }
            if(id > 0)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;             
                pictureBox1.Image = null;
                loadDesing(id);
                groupBox3.Enabled = false;
                groupBox6.Enabled = true;
                label17.Text = "<- " + largo + " mm";
                label18.Text = "<- " + alto + " mm";
            }
            loadLineas();
            if (constants.user_access <= 1 && constants.permitir_cp == false)
            {
                checkBox3.Enabled = false;
            }  
        }

        private void loadLineas()
        {
            localDateBaseEntities3 lineas = new localDateBaseEntities3();

            var l = from x in lineas.lineas_modulos select x;

            comboBox5.Items.Clear();

            foreach (var v in l)
            {
                comboBox5.Items.Add(v.linea_modulo);
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

        private void loadDesing(int id)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            var m = (from x in cotizacion.modulos_cotizaciones where x.id == id && x.sub_folio == constants.sub_folio select x).SingleOrDefault();

            if (m != null)
            {
                listas_entities_pva listas = new listas_entities_pva();
                int modulo_id = (int)m.modulo_id;
                var modulos = (from x in listas.modulos where x.id == modulo_id select x).SingleOrDefault();

                if (modulos != null)
                {
                    int id_diseño = (int)modulos.id_diseño;
                    var diseño = (from x in listas.esquemas where x.id == id_diseño select x).SingleOrDefault();

                    if (diseño != null)
                    {
                        tableLayoutPanel1.Controls.Clear();
                        tableLayoutPanel1.RowCount = (int)diseño.filas;
                        tableLayoutPanel1.ColumnCount = (int)diseño.columnas;
                        getEsquemasFromDiseño(diseño.esquemas);
                        foreach (string es in esquemas)
                        {
                            if (File.Exists(constants.folder_resources_dir + "esquemas\\corredizas\\" + es + ".png"))
                            {
                                constants.loadDiseño("esquemas\\corredizas\\", es, tableLayoutPanel1);
                            }
                            else if (File.Exists(constants.folder_resources_dir + "esquemas\\puertas\\" + es + ".png"))
                            {
                                constants.loadDiseño("esquemas\\puertas\\", es, tableLayoutPanel1);
                            }
                            else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_abatibles\\" + es + ".png"))
                            {
                                constants.loadDiseño("esquemas\\ventanas_abatibles\\", es, tableLayoutPanel1);
                            }
                            else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_fijas\\" + es + ".png"))
                            {
                                constants.loadDiseño("esquemas\\ventanas_fijas\\", es, tableLayoutPanel1);
                            }
                            else if (File.Exists(constants.folder_resources_dir + "esquemas\\templados\\" + es + ".png"))
                            {
                                constants.loadDiseño("esquemas\\templados\\", es, tableLayoutPanel1);
                            }
                            else if (File.Exists(constants.folder_resources_dir + "esquemas\\otros\\" + es + ".png"))
                            {
                                constants.loadDiseño("esquemas\\otros\\", es, tableLayoutPanel1);
                            }
                        }
                        if (diseño.marco == true)
                        {
                            if (diseño.grupo == "puerta")
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
                        checkAcabado(m.acabado_perfil);
                    }
                }
                else
                {
                    if(m.new_desing != "")
                    {
                        int d = constants.stringToInt(m.new_desing);
                        if (d > 0)
                        {
                            var diseño = (from x in listas.esquemas where x.id == d select x).SingleOrDefault();

                            if (diseño != null)
                            {
                                tableLayoutPanel1.Controls.Clear();
                                tableLayoutPanel1.RowCount = (int)diseño.filas;
                                tableLayoutPanel1.ColumnCount = (int)diseño.columnas;
                                getEsquemasFromDiseño(diseño.esquemas);
                                foreach (string es in esquemas)
                                {
                                    if (File.Exists(constants.folder_resources_dir + "esquemas\\corredizas\\" + es + ".png"))
                                    {
                                        constants.loadDiseño("esquemas\\corredizas\\", es, tableLayoutPanel1);
                                    }
                                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\puertas\\" + es + ".png"))
                                    {
                                        constants.loadDiseño("esquemas\\puertas\\", es, tableLayoutPanel1);
                                    }
                                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_abatibles\\" + es + ".png"))
                                    {
                                        constants.loadDiseño("esquemas\\ventanas_abatibles\\", es, tableLayoutPanel1);
                                    }
                                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_fijas\\" + es + ".png"))
                                    {
                                        constants.loadDiseño("esquemas\\ventanas_fijas\\", es, tableLayoutPanel1);
                                    }
                                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\templados\\" + es + ".png"))
                                    {
                                        constants.loadDiseño("esquemas\\templados\\", es, tableLayoutPanel1);
                                    }
                                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\otros\\" + es + ".png"))
                                    {
                                        constants.loadDiseño("esquemas\\otros\\", es, tableLayoutPanel1);
                                    }
                                }
                                if (diseño.marco == true)
                                {
                                    if (diseño.grupo == "puerta")
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
                                checkAcabado(m.acabado_perfil);
                            }                          
                        }
                        else
                        {
                            getNewDesing(m.new_desing);
                            checkAcabado(m.acabado_perfil);                          
                        }
                    }                   
                }
            }
        }

        private bool isLoaded()
        {
            bool r = true;
            foreach(PictureBox x in tableLayoutPanel1.Controls)
            {
               if(x.ImageLocation == null)
                {
                    r = false;
                    break;
                }
            }
            return r;
        }

        private void BackgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 0;
            backgroundWorker3.RunWorkerAsync();
        }

        private void Change_colors_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(backgroundWorker1.IsBusy == true && backgroundWorker2.IsBusy == true && backgroundWorker3.IsBusy == true)
            {
                e.Cancel = true;
            }
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 0;
            backgroundWorker2.RunWorkerAsync();
        }

        private void change_colors_Load(object sender, EventArgs e)
        {
            loadColoresAluminio();
        }

        private void loadColoresAluminio()
        {
            listas_entities_pva listas = new listas_entities_pva();
            var colores = from x in listas.colores_aluminio select x;

            if (colores != null)
            {
                comboBox2.Items.Clear();
                foreach (var c in colores)
                {
                    comboBox2.Items.Add(c.clave);
                }
            }
        }

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

        private void button2_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text == "" && comboBox2.Text == "" && textBox1.Text == "" && !checkBox3.Checked && textBox7.Text == "" && textBox8.Text == "")
            {
                MessageBox.Show(this, "[Error] necesitas añadir al menos una opción.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);             
            }
            else if (comboBox3.Text != "" && comboBox4.Text == "" && textBox2.Text == "")
            {
                MessageBox.Show(this, "[Error] necesitas especificar el filtro seleccionado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if(id <= 0 && checkBox4.Checked && comboBox5.Text != "" && comboBox3.Text != "Linea" && comboBox4.Text == "")
            {
                MessageBox.Show(this, "[Error] necesitas seleccionar la linea que será afectada.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {              
                if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false && backgroundWorker3.IsBusy == false)
                {
                    tableLayoutPanel1.Controls.Clear();
                    pictureBox1.Visible = false;
                    progressBar1.Value = 0;
                    progressBar1.Visible = true;
                    backgroundWorker1.RunWorkerAsync();
                }
            }       
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            listas_entities_pva listas = new listas_entities_pva();
            var modulos_c = (IQueryable<modulos_cotizaciones>)null;
            int modulos_count = 0;
            int id = this.id;

            if(id > 0)
            {
                var m_id = (from x in cotizacion.modulos_cotizaciones where x.id == id && x.sub_folio == constants.sub_folio select x).SingleOrDefault();

                if(m_id != null)
                {
                    label4.Text = m_id.articulo;

                    if (m_id.modulo_id == -1)
                    {
                        modulos_c = (from x in cotizacion.modulos_cotizaciones where x.merge_id == id && x.sub_folio == constants.sub_folio select x);
                        modulos_count = modulos_c.Count();                                                                                       
                    }
                    else
                    {                        
                        modulos_c = (from x in cotizacion.modulos_cotizaciones where x.id == id && x.sub_folio == constants.sub_folio select x);
                        modulos_count = modulos_c.Count();
                    }

                    if (checkBox4.Checked)
                    {
                        if (comboBox5.Text != string.Empty)
                        {
                            changeLine(comboBox5.Text, modulos_c);
                        }
                    }
                }         
            }
            else
            {
                string filter = comboBox3.Text;
                if (filter == "Orden")
                {
                    string[] or = textBox2.Text.Split('-');
                    if (or.Length == 2)
                    {
                        int frm = constants.stringToInt(or[0]);
                        int to = constants.stringToInt(or[1]);
                        var no_u = from x in cotizacion.modulos_cotizaciones where x.modulo_id > 0 && x.merge_id < 0 && x.sub_folio == constants.sub_folio && (x.orden >= frm && x.orden <= to) select x;

                        var u_m = from x in cotizacion.modulos_cotizaciones where x.modulo_id == -1 && x.sub_folio == constants.sub_folio && (x.orden >= frm && x.orden <= to) select x;
                        var m_m = from x in cotizacion.modulos_cotizaciones where u_m.Any(v => v.id == x.merge_id) select x;

                        modulos_c = no_u.Concat(m_m);
                        modulos_count = modulos_c.Count();
                    }
                }
                else if(filter == "Linea")
                {
                    string linea = comboBox4.Text;
                    var no_u = from x in cotizacion.modulos_cotizaciones where x.modulo_id > 0 && x.merge_id < 0 && x.sub_folio == constants.sub_folio && x.linea == linea select x;

                    var u_m = from x in cotizacion.modulos_cotizaciones where x.modulo_id == -1 && x.sub_folio == constants.sub_folio && x.linea == linea select x;
                    var m_m = from x in cotizacion.modulos_cotizaciones where u_m.Any(v => v.id == x.merge_id) select x;

                    modulos_c = no_u.Concat(m_m);
                    modulos_count = modulos_c.Count();

                    if (checkBox4.Checked)
                    {
                        if (comboBox5.Text != string.Empty)
                        {
                            u_m = from x in cotizacion.modulos_cotizaciones where x.modulo_id == -1 && x.sub_folio == constants.sub_folio && x.linea == linea select x;

                            foreach (var y in u_m)
                            {
                                y.linea = comboBox5.Text;
                            }

                            changeLine(comboBox5.Text, modulos_c);

                            linea = comboBox5.Text;
                            no_u = from x in cotizacion.modulos_cotizaciones where x.modulo_id > 0 && x.merge_id < 0 && x.sub_folio == constants.sub_folio && x.linea == linea select x;

                            u_m = from x in cotizacion.modulos_cotizaciones where x.modulo_id == -1 && x.sub_folio == constants.sub_folio && x.linea == linea select x;
                            m_m = from x in cotizacion.modulos_cotizaciones where u_m.Any(v => v.id == x.merge_id) select x;

                            modulos_c = no_u.Concat(m_m);
                            modulos_count = modulos_c.Count();
                        }
                    }
                }
                else if(filter == "Ubicación")
                {
                    string ub = comboBox4.Text;
                    var no_u = from x in cotizacion.modulos_cotizaciones where x.modulo_id > 0 && x.merge_id < 0 && x.sub_folio == constants.sub_folio && x.ubicacion == ub select x;

                    var u_m = from x in cotizacion.modulos_cotizaciones where x.modulo_id == -1 && x.sub_folio == constants.sub_folio && x.ubicacion == ub select x;
                    var m_m = from x in cotizacion.modulos_cotizaciones where u_m.Any(v => v.id == x.merge_id) select x;

                    modulos_c = no_u.Concat(m_m);
                    modulos_count = modulos_c.Count();
                }
                else
                {
                    modulos_c = (from x in cotizacion.modulos_cotizaciones where x.modulo_id > 0 && x.sub_folio == constants.sub_folio select x);
                    modulos_count = modulos_c.Count();
                }
            }
         
            progressBar1.Maximum = modulos_count;
            int c = 0;
            int id_diseño = 0;
            int modulo_id = 0;
            string new_acabado = comboBox1.Text != "" ? comboBox1.Text : comboBox2.Text;
            string acabado_op = constants.IASetAcabado(new_acabado);
            string new_cristal = textBox1.Text;
            string p = string.Empty;
            string g = string.Empty;
            string[] n = null;
            string[] k = null;
            bool error = false;
            int c_1 = 0;
            int c_2 = 0;
            int[] parmeters = null;

            if (checkBox3.Checked)
            {
                parmeters = setNewParameters();
            }

            if (modulos_c != null)
            {
                foreach (var v in modulos_c)
                {
                    if (backgroundWorker1.CancellationPending == false)
                    {
                        modulo_id = (int)v.modulo_id;
                        cambiarMedidas(cotizacion, v.id);
                        var modulo = (from x in listas.modulos where x.id == modulo_id select x).SingleOrDefault();
                        label4.Text = "Cargando... " + v.articulo;
                        if (checkBox3.Checked)
                        {
                            if (parmeters != null)
                            {
                                v.mano_obra = parmeters[2];
                                v.utilidad = parmeters[3];
                                v.flete = parmeters[1];
                                v.desperdicio = parmeters[0];
                            }
                        }
                        if (modulo != null)
                        {                          
                            if (v.new_desing != "" && modulo_id >= 0)
                            {
                                id_diseño = constants.stringToInt(v.new_desing);
                            }
                            else
                            {
                                id_diseño = (int)modulo.id_diseño;
                            }

                            error = false;

                            var diseño = (from x in listas.esquemas where x.id == id_diseño select x).SingleOrDefault();

                            if (diseño != null)
                            {
                                tableLayoutPanel1.Controls.Clear();
                                tableLayoutPanel1.RowCount = (int)diseño.filas;
                                tableLayoutPanel1.ColumnCount = (int)diseño.columnas;
                                getEsquemasFromDiseño(diseño.esquemas);
                                foreach (string es in esquemas)
                                {
                                    if (File.Exists(constants.folder_resources_dir + "esquemas\\corredizas\\" + es + ".png"))
                                    {
                                        constants.loadDiseño("esquemas\\corredizas\\", es, tableLayoutPanel1);
                                    }
                                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\puertas\\" + es + ".png"))
                                    {
                                        constants.loadDiseño("esquemas\\puertas\\", es, tableLayoutPanel1);
                                    }
                                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_abatibles\\" + es + ".png"))
                                    {
                                        constants.loadDiseño("esquemas\\ventanas_abatibles\\", es, tableLayoutPanel1);
                                    }
                                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_fijas\\" + es + ".png"))
                                    {
                                        constants.loadDiseño("esquemas\\ventanas_fijas\\", es, tableLayoutPanel1);
                                    }
                                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\templados\\" + es + ".png"))
                                    {
                                        constants.loadDiseño("esquemas\\templados\\", es, tableLayoutPanel1);
                                    }
                                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\otros\\" + es + ".png"))
                                    {
                                        constants.loadDiseño("esquemas\\otros\\", es, tableLayoutPanel1);
                                    }
                                }
                                if (diseño.marco == true)
                                {
                                    if (diseño.grupo == "puerta")
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

                                
                                c_1 = 0;
                                c_2 = 0;

                                //Perfiles
                                if (new_acabado != "")
                                {
                                    n = v.claves_perfiles.Split(',');
                                    if (n.Length > 0)
                                    {
                                        for (int i = 0; i < n.Length - 1; i++)
                                        {
                                            k = n[i].Split('-');
                                            if (k.Length >= 2)
                                            {
                                                if (constants.stringToFloat(k[1]) > 0)
                                                {
                                                    if (checkPefilesAcabados(k[0], new_acabado) == true)
                                                    {
                                                        c_1++;
                                                        p = p + k[0] + "-" + k[1] + "-" + new_acabado + ",";
                                                    }
                                                    else if (checkPefilesAcabados(k[0], acabado_op) == true && checkBox2.Checked)
                                                    {
                                                        c_2++;
                                                        p = p + k[0] + "-" + k[1] + "-" + acabado_op + ",";
                                                    }
                                                    else
                                                    {
                                                        if (checkBox1.Checked == true)
                                                        {
                                                            error = false;
                                                            p = p + k[0] + "-" + k[1] + "-" + k[2] + ",";
                                                        }
                                                        else
                                                        {
                                                            error = true;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    p = p + k[0] + "-" + k[1] + "-" + k[2] + ",";
                                                }
                                            }
                                        }
                                    }
                                }
                                //end perfiles

                                n = null;
                                k = null;

                                //Cristales               
                                if (new_cristal != "")
                                {
                                    n = v.claves_cristales.Split(',');
                                    if (n.Length > 0)
                                    {
                                        for (int i = 0; i < n.Length - 1; i++)
                                        {
                                            k = n[i].Split('-');
                                            if (k.Length > 0)
                                            {
                                                if (k.Length > 1)
                                                {
                                                    g = g + new_cristal + "-" + k[1] + ",";
                                                }
                                                else
                                                {
                                                    g = g + new_cristal + ",";
                                                }
                                            }
                                        }
                                        if (g != "")
                                        {
                                            v.claves_cristales = g;
                                        }
                                    }
                                }
                                //end cristales

                                g = "";
                                n = null;
                                k = null;

                                //news
                                n = v.news.Split(';');
                                if (n.Length > 0)
                                {
                                    for (int i = 0; i < n.Length - 1; i++)
                                    {
                                        k = n[i].Split(',');
                                        if (k.Length == 6 || k.Length == 4)
                                        {
                                            if (k[0] == "1")
                                            {
                                                if (new_acabado != "")
                                                {
                                                    if (constants.stringToFloat(k[2]) > 0)
                                                    {
                                                        if (checkPefilesAcabados(k[1], new_acabado) == true)
                                                        {
                                                            c_1++;
                                                            g = g + "1," + k[1] + "," + k[2] + "," + k[3] + "," + k[4] + "," + (new_acabado != "" ? new_acabado : k[5]) + ";";
                                                        }
                                                        else if (checkPefilesAcabados(k[1], acabado_op) == true && checkBox2.Checked)
                                                        {
                                                            c_2++;
                                                            g = g + "1," + k[1] + "," + k[2] + "," + k[3] + "," + k[4] + "," + (acabado_op != "" ? acabado_op : k[5]) + ";";
                                                        }
                                                        else
                                                        {
                                                            if (checkBox1.Checked == true)
                                                            {
                                                                error = false;
                                                                g = g + "1," + k[1] + "," + k[2] + "," + k[3] + "," + k[4] + "," + k[5] + ";";
                                                            }
                                                            else
                                                            {
                                                                error = true;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        g = g + "1," + k[1] + "," + k[2] + "," + k[3] + "," + k[4] + "," + k[5] + ";";
                                                    }
                                                }
                                                else
                                                {
                                                    g = g + "1," + k[1] + "," + k[2] + "," + k[3] + "," + k[4] + "," + k[5] + ";";
                                                }
                                            }
                                            else if (k[0] == "2")
                                            {
                                                g = g + "2," + (new_cristal != "" ? new_cristal : k[1]) + "," + k[2] + "," + "," + k[4] + "," + ";";
                                            }
                                            else if (k[0] == "3")
                                            {
                                                g = g + "3," + k[1] + "," + k[2] + "," + "," + k[4] + "," + ";";
                                            }
                                            else if (k[0] == "4")
                                            {
                                                g = g + "4," + k[1] + "," + k[2] + "," + k[3] + "," + k[4] + "," + ";";
                                            }
                                            else if (k[0] == "5")
                                            {
                                                g = g + "5," + k[1] + "," + k[2] + "," + k[3] + ";";
                                            }
                                        }
                                    }
                                    if (g != "" && error == false)
                                    {
                                        v.news = g;
                                    }
                                }
                                //

                                //check esquemas cargados
                                do
                                {
                                    System.Threading.Thread.Sleep(500);
                                }
                                while (isLoaded() == false);
                                c++;
                                backgroundWorker1.ReportProgress(c);
                                if (error == true && checkBox1.Checked == false)
                                {
                                    MessageBox.Show(this, "[Error] el módulo con ID: " + v.modulo_id + " no es compatible con esté acabado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else
                                {
                                    if (new_acabado != "" || acabado_op != "")
                                    {
                                        if (p != "")
                                        {
                                            v.claves_perfiles = p;
                                            v.acabado_perfil = c_1 >= c_2 ? new_acabado : acabado_op;
                                            v.pic = constants.imageToByte(createModuloPic());
                                        }
                                    }
                                }

                                n = null;
                                k = null;
                                p = "";
                                g = "";
                            }
                            else
                            {
                                if (v.new_desing != "" && modulo_id >= 0)
                                {
                                    getNewDesing(v.new_desing);

                                    c_1 = 0;
                                    c_2 = 0;

                                    //Perfiles
                                    if (new_acabado != "")
                                    {
                                        n = v.claves_perfiles.Split(',');
                                        if (n.Length > 0)
                                        {
                                            for (int i = 0; i < n.Length - 1; i++)
                                            {
                                                k = n[i].Split('-');
                                                if (k.Length >= 2)
                                                {
                                                    if (constants.stringToFloat(k[1]) > 0)
                                                    {
                                                        if (checkPefilesAcabados(k[0], new_acabado) == true)
                                                        {
                                                            c_1++;
                                                            p = p + k[0] + "-" + k[1] + "-" + new_acabado + ",";
                                                        }
                                                        else if (checkPefilesAcabados(k[0], acabado_op) == true && checkBox2.Checked)
                                                        {
                                                            c_2++;
                                                            p = p + k[0] + "-" + k[1] + "-" + acabado_op + ",";
                                                        }
                                                        else
                                                        {
                                                            if (checkBox1.Checked == true)
                                                            {
                                                                error = false;
                                                                p = p + k[0] + "-" + k[1] + "-" + k[2] + ",";
                                                            }
                                                            else
                                                            {
                                                                error = true;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        p = p + k[0] + "-" + k[1] + "-" + k[2] + ",";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //end perfiles

                                    n = null;
                                    k = null;

                                    //Cristales               
                                    if (new_cristal != "")
                                    {
                                        n = v.claves_cristales.Split(',');
                                        if (n.Length > 0)
                                        {
                                            for (int i = 0; i < n.Length - 1; i++)
                                            {
                                                k = n[i].Split('-');
                                                if (k.Length > 0)
                                                {
                                                    if (k.Length > 1)
                                                    {
                                                        g = g + new_cristal + "-" + k[1] + ",";
                                                    }
                                                    else
                                                    {
                                                        g = g + new_cristal + ",";
                                                    }
                                                }
                                            }
                                            if (g != "")
                                            {
                                                v.claves_cristales = g;
                                            }
                                        }
                                    }
                                    //end cristales

                                    g = "";
                                    n = null;
                                    k = null;

                                    //news
                                    n = v.news.Split(';');
                                    if (n.Length > 0)
                                    {
                                        for (int i = 0; i < n.Length - 1; i++)
                                        {
                                            k = n[i].Split(',');
                                            if (k.Length == 6 || k.Length == 4)
                                            {
                                                if (k[0] == "1")
                                                {
                                                    if (new_acabado != "")
                                                    {
                                                        if (constants.stringToFloat(k[2]) > 0)
                                                        {
                                                            if (checkPefilesAcabados(k[1], new_acabado) == true)
                                                            {
                                                                c_1++;
                                                                g = g + "1," + k[1] + "," + k[2] + "," + k[3] + "," + k[4] + "," + (new_acabado != "" ? new_acabado : k[5]) + ";";
                                                            }
                                                            else if (checkPefilesAcabados(k[1], acabado_op) == true && checkBox2.Checked)
                                                            {
                                                                c_2++;
                                                                g = g + "1," + k[1] + "," + k[2] + "," + k[3] + "," + k[4] + "," + (acabado_op != "" ? acabado_op : k[5]) + ";";
                                                            }
                                                            else
                                                            {
                                                                if (checkBox1.Checked == true)
                                                                {
                                                                    error = false;
                                                                    g = g + "1," + k[1] + "," + k[2] + "," + k[3] + "," + k[4] + "," + k[5] + ";";
                                                                }
                                                                else
                                                                {
                                                                    error = true;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            g = g + "1," + k[1] + "," + k[2] + "," + k[3] + "," + k[4] + "," + k[5] + ";";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        g = g + "1," + k[1] + "," + k[2] + "," + k[3] + "," + k[4] + "," + k[5] + ";";
                                                    }
                                                }
                                                else if (k[0] == "2")
                                                {
                                                    g = g + "2," + (new_cristal != "" ? new_cristal : k[1]) + "," + k[2] + "," + "," + k[4] + "," + ";";
                                                }
                                                else if (k[0] == "3")
                                                {
                                                    g = g + "3," + k[1] + "," + k[2] + "," + "," + k[4] + "," + ";";
                                                }
                                                else if (k[0] == "4")
                                                {
                                                    g = g + "4," + k[1] + "," + k[2] + "," + k[3] + "," + k[4] + "," + ";";
                                                }
                                                else if (k[0] == "5")
                                                {
                                                    g = g + "5," + k[1] + "," + k[2] + "," + k[3] + ";";
                                                }
                                            }
                                        }
                                        if (g != "" && error == false)
                                        {
                                            v.news = g;
                                        }
                                    }
                                    //

                                    //check esquemas cargados
                                    do
                                    {
                                        System.Threading.Thread.Sleep(500);
                                    }
                                    while (isLoaded() == false);
                                    c++;
                                    backgroundWorker1.ReportProgress(c);
                                    if (error == true && checkBox1.Checked == false)
                                    {
                                        MessageBox.Show(this, "[Error] el módulo con ID: " + v.modulo_id + " no es compatible con esté acabado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        if (new_acabado != "" || acabado_op != "")
                                        {
                                            if (p != "")
                                            {
                                                v.claves_perfiles = p;
                                                v.acabado_perfil = c_1 >= c_2 ? new_acabado : acabado_op;
                                                v.pic = constants.imageToByte(createModuloPic());
                                            }
                                        }
                                    }

                                    n = null;
                                    k = null;
                                    p = "";
                                    g = "";
                                }
                            }                           
                        }
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }     
            if (error == false)
            {
                cotizacion.SaveChanges();
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

        private void setNewEsquema(bool s)
        {
            if (s == true)
            {                
                changeStyleRow(SizeType.Absolute, (tableLayoutPanel1.Height - 20) / tableLayoutPanel1.RowCount);
                changeStyleColumn(SizeType.Absolute, (tableLayoutPanel1.Width - 20) / tableLayoutPanel1.ColumnCount);
            }
            else
            {                
                changeStyleRow(SizeType.Percent, 100 / tableLayoutPanel1.RowCount);
                changeStyleColumn(SizeType.Percent, 100 / tableLayoutPanel1.ColumnCount);
            }
        }

        private float getProportion(string[] m)
        {
            int r = 0;
            foreach(string x in m)
            {
                r = r + constants.stringToInt(x);
            }
            return r;
        }

        private void getNewDesing(string desing)
        {
            string[] r = desing.Split(',');
            if (r.Length == 5)
            {
                tableLayoutPanel1.Controls.Clear();
                int m = 0;
                string[] n = r[0].Split(':');
                tableLayoutPanel1.RowCount = constants.stringToInt(n[0]);
                tableLayoutPanel1.ColumnCount = constants.stringToInt(n[1]);
                string[] es = r[3].Split(':');
                esquemas.Clear();
                foreach (string x in es)
                {
                    esquemas.Add(x);
                }
                string[] a = r[4].Split(':');
                if (a.Length == 3)
                {
                    if (a[0] == "t")
                    {
                        if (a[2] == "puerta")
                        {
                            tableLayoutPanel1.Padding = new Padding(10, 10, 10, 0);
                        }
                        else
                        {
                            tableLayoutPanel1.Padding = new Padding(10, 10, 10, 10);
                        }
                        m = 30;
                    }
                    else
                    {
                        tableLayoutPanel1.Padding = new Padding(0, 0, 0, 0);
                        m = 5;
                    }
                    if(a[1] == "t")
                    {
                        setNewEsquema(true);
                    }
                    else
                    {
                        setNewEsquema(false);
                    }
                }
                string[] c = r[1].Split(':');
                string[] f = r[2].Split(':');
                float y = 0;
                int width = 0;
                int height = 0;
                float p = getProportion(c);
                for (int i = 0; i < tableLayoutPanel1.ColumnStyles.Count; i++)
                {
                    y = (constants.stringToFloat(c[i]) / p);
                    width = (int)((tableLayoutPanel1.Width-m) * y);
                    tableLayoutPanel1.ColumnStyles[i].Width = width;
                }
                p = getProportion(f);
                for (int i = 0; i < tableLayoutPanel1.RowStyles.Count; i++)
                {
                    y = (constants.stringToFloat(f[i]) / p);
                    height = (int)((tableLayoutPanel1.Height-m) * y);
                    tableLayoutPanel1.RowStyles[i].Height = height;
                }
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
            }
        }

        //Crear pic modulo
        private Bitmap createModuloPic()
        {          
            if (constants.mostrar_acabado == false)
            {
                clearBackground();
            }           
            Bitmap bm = new Bitmap(tableLayoutPanel1.Width, tableLayoutPanel1.Height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                tableLayoutPanel1.DrawToBitmap(bm, new Rectangle(0, 0, tableLayoutPanel1.Width, tableLayoutPanel1.Height));
            }
            Bitmap pic = new Bitmap(bm, 120, 105);
            bm = null;
            return pic;
        }

        //borrar panel image background
        private void clearBackground()
        {
            foreach (Control x in tableLayoutPanel1.Controls)
            {
                x.BackgroundImage = null;
                x.BackColor = Color.LightBlue;
            }
            tableLayoutPanel1.BackgroundImage = null;
            tableLayoutPanel1.BackColor = Color.LightBlue;
        }
        //  

        private bool checkPefilesAcabados(string clave, string acabado)
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
                {
                    if (tableLayoutPanel1.Controls[i] != pictureBox1)
                    {
                        constants.setBackgroundImg("acabados_perfil", comboBox1.Text, "jpg", tableLayoutPanel1.Controls[i]);
                    }
                }
                constants.setBackgroundImg("acabados_perfil", comboBox1.Text, "jpg", tableLayoutPanel1);
                constants.setBackgroundImg("acabados_perfil", comboBox1.Text, "jpg", pictureBox2);
                label5.Text = comboBox1.Text;
                if (button6.Visible == false)
                {
                    button6.Visible = true;
                }
                comboBox2.SelectedIndex = -1;
            }         
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != -1)
            {
                for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
                {
                    if (tableLayoutPanel1.Controls[i] != pictureBox1)
                    {
                        constants.setBackgroundImg("acabados_especiales", comboBox2.Text, "jpg", tableLayoutPanel1.Controls[i]);
                    }
                }
                constants.setBackgroundImg("acabados_especiales", comboBox2.Text, "jpg", tableLayoutPanel1);
                constants.setBackgroundImg("acabados_especiales", comboBox2.Text, "jpg", pictureBox2);
                label5.Text = comboBox2.Text;
                if (button6.Visible == false)
                {
                    button6.Visible = true;
                }
                comboBox1.SelectedIndex = -1;
            }        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new colores(true, -1, true, true).ShowDialog();
        }

        public void setColorAnodizado(string color)
        {
            comboBox2.Text = color;
        }

        public void setCristal(string clave)
        {
            textBox1.Text = clave;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new config_items(-1, 2, "", -1, true, true).ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        //detener
        private void button5_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        //cargar esquemas unificados
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            listas_entities_pva listas = new listas_entities_pva();
            var modulos_c = (IQueryable<modulos_cotizaciones>)null;
            int modulos_count = 0;
            int id = this.id;

            if (id > 0)
            {                              
                modulos_c = (from x in cotizacion.modulos_cotizaciones where x.id == id && x.sub_folio == constants.sub_folio select x);
                modulos_count = (from x in cotizacion.modulos_cotizaciones where x.id == id && x.sub_folio == constants.sub_folio select x).Count();                         
            }
            else
            {
                string filter = comboBox3.Text;
                if (filter == "Orden")
                {
                    string[] or = textBox2.Text.Split('-');
                    if (or.Length == 2)
                    {
                        int frm = constants.stringToInt(or[0]);
                        int to = constants.stringToInt(or[1]);
                        modulos_c = (from x in cotizacion.modulos_cotizaciones where x.modulo_id == -1 && x.sub_folio == constants.sub_folio && (x.orden >= frm && x.orden <= to) select x);
                        modulos_count = modulos_c.Count();
                    }
                }
                else if (filter == "Linea")
                {
                    string linea = comboBox4.Text;
                    if (checkBox4.Checked)
                    {
                        linea = comboBox5.Text;
                    }
                    modulos_c = (from x in cotizacion.modulos_cotizaciones where x.modulo_id == -1 && x.sub_folio == constants.sub_folio && x.linea == linea select x);
                    modulos_count = modulos_c.Count();
                }
                else if (filter == "Ubicación")
                {
                    string ub = comboBox4.Text;
                    modulos_c = (from x in cotizacion.modulos_cotizaciones where x.modulo_id == -1 && x.sub_folio == constants.sub_folio && x.ubicacion == ub select x);
                    modulos_count = modulos_c.Count();
                }
                else
                {
                    modulos_c = (from x in cotizacion.modulos_cotizaciones where x.modulo_id == -1 && x.sub_folio == constants.sub_folio select x);
                    modulos_count = modulos_c.Count();
                }           
            }

            int id_diseño = 0;
            progressBar1.Maximum = modulos_count;
            int c = 0;
            string new_acabado = comboBox1.Text != "" ? comboBox1.Text : comboBox2.Text;
            string acabado_op = constants.IASetAcabado(new_acabado);

            if (modulos_c != null)
            {
                foreach (var v in modulos_c)
                {
                    int k = v.id;
                    int j = (from x in cotizacion.modulos_cotizaciones where x.merge_id == k select x).Count();
                    if (j > 0)
                    {
                        label4.Text = "Cargando conceptos unificados...";

                        if (checkAcabados((int)v.concept_id, new_acabado, acabado_op) == true)
                        {
                            if (v.new_desing != "" && v.new_desing != null)
                            {
                                id_diseño = constants.stringToInt(v.new_desing);

                                var diseño = (from x in listas.esquemas where x.id == id_diseño select x).SingleOrDefault();

                                if (diseño != null)
                                {
                                    tableLayoutPanel1.Controls.Clear();
                                    tableLayoutPanel1.RowCount = (int)diseño.filas;
                                    tableLayoutPanel1.ColumnCount = (int)diseño.columnas;
                                    getEsquemasFromDiseño(diseño.esquemas);
                                    foreach (string es in esquemas)
                                    {
                                        if (File.Exists(constants.folder_resources_dir + "esquemas\\corredizas\\" + es + ".png"))
                                        {
                                            constants.loadDiseño("esquemas\\corredizas\\", es, tableLayoutPanel1);
                                        }
                                        else if (File.Exists(constants.folder_resources_dir + "esquemas\\puertas\\" + es + ".png"))
                                        {
                                            constants.loadDiseño("esquemas\\puertas\\", es, tableLayoutPanel1);
                                        }
                                        else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_abatibles\\" + es + ".png"))
                                        {
                                            constants.loadDiseño("esquemas\\ventanas_abatibles\\", es, tableLayoutPanel1);
                                        }
                                        else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_fijas\\" + es + ".png"))
                                        {
                                            constants.loadDiseño("esquemas\\ventanas_fijas\\", es, tableLayoutPanel1);
                                        }
                                        else if (File.Exists(constants.folder_resources_dir + "esquemas\\templados\\" + es + ".png"))
                                        {
                                            constants.loadDiseño("esquemas\\templados\\", es, tableLayoutPanel1);
                                        }
                                        else if (File.Exists(constants.folder_resources_dir + "esquemas\\otros\\" + es + ".png"))
                                        {
                                            constants.loadDiseño("esquemas\\otros\\", es, tableLayoutPanel1);
                                        }
                                    }
                                    if (diseño.marco == true)
                                    {
                                        if (diseño.grupo == "puerta")
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

                                    //check esquemas cargados
                                    do
                                    {
                                        System.Threading.Thread.Sleep(500);
                                    }
                                    while (isLoaded() == false);
                                    c++;
                                    backgroundWorker2.ReportProgress(c);
                                    v.pic = constants.imageToByte(createModuloPic());
                                }
                                else
                                {
                                    getNewDesing(v.new_desing);
                                    //check esquemas cargados
                                    do
                                    {
                                        System.Threading.Thread.Sleep(500);
                                    }
                                    while (isLoaded() == false);
                                    c++;
                                    backgroundWorker2.ReportProgress(c);
                                    v.pic = constants.imageToByte(createModuloPic());
                                }
                            }
                        }
                    }
                }
            }
            cotizacion.SaveChanges();
        }

        private bool checkAcabados(int merged_id, string acabado, string acabado_op)
        {
            bool r = true;
            var modulos_c = (from x in cotizacion.modulos_cotizaciones where x.merge_id == merged_id && x.sub_folio == constants.sub_folio select x);
            foreach (var v in modulos_c)
            {
                if(v.acabado_perfil != acabado && v.acabado_perfil != acabado_op)
                {
                    r = false;
                    break;
                }
            }
            return r;
        }

        //proceso completado
        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            label4.Text = "Actualizando Datos...";
            constants.errors_Open.Clear();
            constants.reloadPreciosCotizaciones(constants.sub_folio);
            ((Form1)Application.OpenForms["Form1"]).reloadAll();                
        }

        private void BackgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Application.OpenForms["articulos_cotizacion"] != null)
            {
                ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL();
            }          
            MessageBox.Show(this, "Se ha completado el proceso.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.Image = Properties.Resources.wizard_icon_128;
            progressBar1.Visible = false;
            label4.Text = "";
            if (id > 0)
            {
                this.Close();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            button6.Visible = false;
            label5.Text = "";
            pictureBox2.BackgroundImage = null;
            tableLayoutPanel1.BackgroundImage = null;
            foreach(Control x in tableLayoutPanel1.Controls)
            {
                x.BackgroundImage = null;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            comboBox4.Enabled = true;
            textBox2.Enabled = true;
            comboBox4.Items.Clear();

            if (comboBox3.Text == "Orden")
            {
                comboBox4.Enabled = false;
            }
            else if(comboBox3.Text == "Linea")
            {
                textBox2.Enabled = false;
                var h = (from x in cotizaciones.modulos_cotizaciones where x.modulo_id > 0 && x.sub_folio == constants.sub_folio select x);

                if(h != null)
                {
                    foreach(var y in h)
                    {
                        if(comboBox4.Items.Contains(y.linea) == false) 
                        {
                            if (y.linea != string.Empty)
                            {
                                comboBox4.Items.Add(y.linea);
                            }
                        }
                    }
                }             
            }
            else if(comboBox3.Text == "Ubicación")
            {
                textBox2.Enabled = false;
                var h = (from x in cotizaciones.modulos_cotizaciones where x.modulo_id > 0 && x.sub_folio == constants.sub_folio select x);

                if (h != null)
                {
                    foreach (var y in h)
                    {
                        if (comboBox4.Items.Contains(y.ubicacion) == false)
                        {
                            if (y.ubicacion != string.Empty)
                            {
                                comboBox4.Items.Add(y.ubicacion);
                            }
                        }
                    }
                }
            }
            else
            {
                comboBox4.Enabled = false;
                textBox2.Enabled = false;
            }          
        }
        //

        //set new parameters
        private int[] setNewParameters()
        {
            return new int[4] {constants.stringToInt(textBox3.Text), constants.stringToInt(textBox4.Text), constants.stringToInt(textBox5.Text), constants.stringToInt(textBox6.Text)};
        }

        //habilitar parametros
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                groupBox4.Enabled = true;
            }
            else
            {
                groupBox4.Enabled = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            comboBox3.SelectedIndex = -1;
            comboBox4.SelectedIndex = -1;
        }

        private void changeLine(string new_line, IQueryable<modulos_cotizaciones> w)
        {
            listas_entities_pva listas = new listas_entities_pva();
            string clave = string.Empty;
            string[] p_dim = null;
            string[,] dimensiones = null;
            int c = 0;
            int y = 0;
            if (w != null)
            {
                foreach (var m in w)
                {
                    int m_id = (int)m.modulo_id;
                    int id = (int)(from x in listas.modulos where x.id == m_id select x.id_diseño).SingleOrDefault();

                    var modulos = (from x in listas.modulos where x.linea == new_line && x.id_diseño == id select x).FirstOrDefault();

                    if (modulos != null)
                    {
                        m.linea = new_line;
                        m.articulo = modulos.articulo;
                        m.modulo_id = modulos.id;
                        m.news = "";
                        p_dim = m.dimensiones.Split(',');
                        
                        if (p_dim.Length > 0)
                        {
                            dimensiones = new string[(p_dim.Length / 2), 2];
                            y = 0;
                            c = 0;
                            for (int i = 0; i < p_dim.Length - 1; i++)
                            {
                                dimensiones[y, c] = p_dim[i];
                                c++;
                                if (c == 2)
                                {
                                    c = 0;
                                    y++;
                                }
                            }
                        }

                        string[] k = m.clave.Split('-');
                        if(k.Length > 1)
                        {
                            m.clave = modulos.clave + "-" + k[1];
                        }
                        else
                        {
                            m.clave = modulos.clave;
                        }

                        string buffer = string.Empty;

                        string[] perfiles = modulos.id_aluminio.Split(',');

                        foreach (string v in perfiles)
                        {
                            if (v.Length > 0)
                            {
                                string[] p = v.Split(new char[] { ':', '-', '$' }, StringSplitOptions.RemoveEmptyEntries);
                                if (p.Length > 1)
                                {
                                    clave = p[0];
                                    var perfil = (from x in listas.perfiles where x.clave == clave select x).SingleOrDefault();
                                    if(perfil != null)
                                    {
                                        if(perfil.linea == "celosia" || perfil.linea == "duelas" || perfil.linea == "lama")
                                        {
                                            if(p[2] == "largo")
                                            {
                                                if(constants.stringToInt(p[3]) > 0)
                                                {
                                                    p[1] = Math.Floor((float)(constants.stringToFloat(dimensiones[constants.stringToInt(p[3]), 1]) / perfil.ancho_perfil)).ToString();
                                                    buffer = buffer + p[0] + "-" + p[1] + "-" + ",";
                                                }
                                                else
                                                {
                                                    p[1] = Math.Floor((float)(m.alto / perfil.ancho_perfil)).ToString();
                                                    buffer = buffer + p[0] + "-" + p[1] + "-" + ",";
                                                }
                                            }
                                            else if(p[2] == "alto")
                                            {
                                                if (constants.stringToInt(p[3]) > 0)
                                                {
                                                    p[1] = Math.Floor((float)(constants.stringToFloat(dimensiones[constants.stringToInt(p[3]), 0]) / perfil.ancho_perfil)).ToString();
                                                    buffer = buffer + p[0] + "-" + p[1] + "-" + ",";
                                                }
                                                else
                                                {
                                                    p[1] = Math.Floor((float)(m.largo / perfil.ancho_perfil)).ToString();
                                                    buffer = buffer + p[0] + "-" + p[1] + "-" + ",";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            buffer = buffer + p[0] + "-" + p[1] + "-" + ",";
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("[Error] perfil no identificado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                        }

                        m.claves_perfiles = buffer;
                        buffer = "";

                        string[] herrajes = modulos.id_herraje.Split(',');

                        foreach (string v in herrajes)
                        {
                            if (v.Length > 0)
                            {
                                string[] p = v.Split(new char[] { ':', '-', '$' }, StringSplitOptions.RemoveEmptyEntries);
                                if (p.Length > 1)
                                {
                                    buffer = buffer + p[0] + "-" + p[1] + ",";
                                }
                            }
                        }

                        m.claves_herrajes = buffer;
                        buffer = "";

                        string[] otros = modulos.id_otros.Split(',');

                        foreach (string v in otros)
                        {
                            if (v.Length > 0)
                            {
                                string[] p = v.Split(new char[] { ':', '-', '$' }, StringSplitOptions.RemoveEmptyEntries);
                                if (p.Length > 1)
                                {
                                    buffer = buffer + p[0] + "-" + p[1] + ",";
                                }
                            }
                        }

                        m.claves_otros = buffer;
                        buffer = "";

                        string[] vidrio = modulos.clave_vidrio.Split(',');

                        foreach (string v in vidrio)
                        {
                            if (v.Length > 0)
                            {
                                string[] p = v.Split(new char[] { ':', '-', '$' }, StringSplitOptions.RemoveEmptyEntries);
                                if (p.Length > 1)
                                {
                                    buffer = buffer + p[0] + "-" + p[1] + ",";
                                }
                            }
                        }

                        m.claves_cristales = buffer;
                        buffer = "";
                    }
                    else
                    {
                        MessageBox.Show(this, "[Error] el módulo con ID: " + m.id + " no es compatible con la linea: " + new_line + ".", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                cotizacion.SaveChanges();
            }         
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                groupBox5.Enabled = true;
            }
            else
            {
                groupBox5.Enabled = false;
            }
        }

        private string getPerfiles(string perfiles, listas_entities_pva listas, float largo, float alto, int m_id)
        {
            string r = string.Empty;
            string[] _u = perfiles.Split(',');
            string[] _p = null;
            string clave = string.Empty;
            float perfil_ancho = 0;
            float cant = 0;
            string c_clave = string.Empty;
            string buffer = string.Empty;
            string dir = string.Empty;

            foreach(string x in _u)
            {
                _p = x.Split('-');
                if(_p.Length == 3)
                {
                    clave = _p[0];
                    var perfil = (from v in listas.perfiles where v.clave == clave select v).SingleOrDefault();
                    if (perfil != null)
                    {
                        if(perfil.linea == "celosia" || perfil.linea == "duelas" || perfil.linea == "lama")
                        {
                            perfil_ancho = (float)perfil.ancho_perfil;
                            cant = constants.stringToFloat(_p[1]);
                            if (perfil_ancho > 0 && cant > 0)
                            {
                                var _m = (from y in listas.modulos where y.id == m_id select y).SingleOrDefault();                              

                                if (_m != null)
                                {
                                    foreach (char alm in _m.id_aluminio)
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
                                            buffer = "";

                                            if (clave == c_clave)
                                            {
                                                if (dir == "largo")
                                                {
                                                    r = r + _p[0] + "-" + Math.Floor(alto / perfil_ancho) + "-" + _p[2] + ",";
                                                }
                                                else if (dir == "alto")
                                                {
                                                    r = r + _p[0] + "-" + Math.Floor(largo / perfil_ancho) + "-" + _p[2] + ",";
                                                }
                                            }
                                        }
                                    }                                  
                                }                              
                            }
                            else
                            {
                                r = r + x + ",";
                            }
                        }
                        else
                        {
                            r = r + x + ",";
                        }
                    }
                }
            }
            return r;
        }

        private string getNewPerfiles(string perfiles, listas_entities_pva listas, float largo, float alto, int m_id)
        {
            string r = string.Empty;
            string[] _u = perfiles.Split(';');
            string[] _p = null;
            string clave = string.Empty;
            float perfil_ancho = 0;
            float cant = 0;
            string dir = string.Empty;

            foreach (string x in _u)
            {
                _p = x.Split(',');
                if (_p.Length == 4 || _p.Length == 6)
                {
                    if (_p[0] == "1")
                    {
                        clave = _p[1];
                        var perfil = (from v in listas.perfiles where v.clave == clave select v).SingleOrDefault();
                        if (perfil != null)
                        {
                            if (perfil.linea == "celosia" || perfil.linea == "duelas" || perfil.linea == "lama")
                            {
                                perfil_ancho = (float)perfil.ancho_perfil;
                                cant = constants.stringToFloat(_p[2]);
                                dir = _p[3];

                                if (perfil_ancho > 0 && cant > 0)
                                {                                   
                                    if (dir == "largo")
                                    {
                                        r = r + _p[0] + "," + _p[1] + "," + Math.Floor(alto / perfil_ancho) + "," + _p[3] + "," + _p[4] + "," + _p[5] + ";";
                                    }
                                    else if (dir == "alto")
                                    {
                                        r = r + _p[0] + "," + _p[1] + "," + Math.Floor(largo / perfil_ancho) + "," + _p[3] + "," + _p[4] + "," + _p[5] + ";";
                                    }
                                }
                                else
                                {
                                    r = r + x + ";";
                                }
                            }
                            else
                            {
                                r = r + x + ";";
                            }
                        }
                    }
                    else
                    {
                        r = r + x + ";";
                    }
                }               
            }
            return r;
        }

        private void cambiarMedidas(cotizaciones_local cotizacion, int id)
        {
            if (textBox7.Text != "" || textBox8.Text != "")
            {
                var modulo = (from x in cotizacion.modulos_cotizaciones where x.id == id && x.sub_folio == constants.sub_folio select x).SingleOrDefault();

                if (modulo != null)
                {
                    string[] r = null;
                    float largo = 0;
                    float alto = 0;
                    int m_id = 0;
                    listas_entities_pva listas = new listas_entities_pva();

                    if (modulo.merge_id > 0)
                    {
                        int merged_id = (int)modulo.merge_id;
                        var concept = (from x in cotizacion.modulos_cotizaciones where x.id == merged_id && x.sub_folio == constants.sub_folio select x).SingleOrDefault();
                        if (concept != null)
                        {                           
                            r = modulo.dimensiones.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            m_id = (int)modulo.modulo_id;
                            var _l = (from x in listas.modulos where x.id == m_id select x).SingleOrDefault();

                            if (_l != null)
                            {
                                if (r.Length == 2 && _l.reglas == string.Empty)
                                {
                                    float p_l = ((float)modulo.largo / (float)concept.largo);
                                    float p_a = ((float)modulo.alto / (float)concept.alto);
                                    largo = constants.stringToInt(textBox7.Text);
                                    alto = constants.stringToInt(textBox8.Text);

                                    modulo.dimensiones = (largo > 0 ? (int)(largo * p_l) : modulo.largo) + "," + (alto > 0 ? (int)(alto * p_a) : modulo.alto) + ",";

                                    if (largo > 0)
                                    {
                                        modulo.largo = (int)(largo * p_l);
                                    }
                                    if (alto > 0)
                                    {
                                        modulo.alto = (int)(alto * p_a);
                                    }

                                    modulo.claves_perfiles = getPerfiles(modulo.claves_perfiles, listas, largo * p_l, alto * p_a, (int)modulo.modulo_id);
                                    if (modulo.news.Length > 0)
                                    {
                                        modulo.news = getNewPerfiles(modulo.news, listas, largo * p_l, alto * p_a, (int)modulo.modulo_id);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(this, "[Error] el módulo con ID: " + concept.id + " tiene artículos los cuales no se pueden modificar por este medio.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }                        
                        }
                    }
                    else
                    {
                        r = modulo.dimensiones.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        m_id = (int)modulo.modulo_id;
                        var _l = (from x in listas.modulos where x.id == m_id select x).SingleOrDefault();

                        if (_l != null)
                        {
                            if (r.Length == 2 && _l.reglas == string.Empty)
                            {
                                largo = constants.stringToInt(textBox7.Text);
                                alto = constants.stringToInt(textBox8.Text);

                                modulo.dimensiones = (largo > 0 ? (int)largo : modulo.largo) + "," + (alto > 0 ? (int)alto : modulo.alto) + ",";

                                if (largo > 0)
                                {
                                    modulo.largo = (int)largo;
                                }
                                if (alto > 0)
                                {
                                    modulo.alto = (int)alto;
                                }

                                modulo.claves_perfiles = getPerfiles(modulo.claves_perfiles, listas, largo, alto, (int)modulo.modulo_id);
                                if (modulo.news.Length > 0)
                                {
                                    modulo.news = getNewPerfiles(modulo.news, listas, largo, alto, (int)modulo.modulo_id);
                                }
                            }
                            else
                            {
                                MessageBox.Show(this, "[Error] el módulo con ID: " + modulo.id + " no se pueden modificar por este medio.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }                
                }
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.SelectedIndex >= 0)
            {
                DialogResult r = MessageBox.Show(this, "Se eliminarán los costos adicionales y nuevos materiales incluidos en " + (id > 0 ? "esta partida" : "estas partidas") + ". ¿Desea continuar?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (r == DialogResult.No)
                {
                    comboBox5.SelectedIndex = -1;
                }
            }
        }
    }
}
