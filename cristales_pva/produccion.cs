using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace cristales_pva
{
    public partial class produccion : Form
    {
        int folio;

        public produccion()
        {
            InitializeComponent();
            reportViewer1.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;
            reportViewer1.LocalReport.EnableExternalImages = true;
            reportViewer1.ZoomMode = ZoomMode.PageWidth;
            string display = "Producción";
            if (constants.nombre_cotizacion != string.Empty)
            {
                display = display + " - " + constants.nombre_cotizacion;
            }        
            //------------------------>
            if (constants.nombre_proyecto != string.Empty)
            {
                display = display + " - " + constants.nombre_proyecto;
            }
            //------------------------>                    
            reportViewer1.LocalReport.DisplayName = display;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker2.WorkerReportsProgress = true;
            ////
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            backgroundWorker2.RunWorkerCompleted += BackgroundWorker2_RunWorkerCompleted;
            ////
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker2.ProgressChanged += BackgroundWorker2_ProgressChanged;
            this.folio = constants.folio_abierto;
            this.Text = this.Text + " - Sub-Folio: " + constants.sub_folio;
            this.Shown += Produccion_Shown;
        }

        private void Produccion_Shown(object sender, EventArgs e)
        {
            if (!backgroundWorker2.IsBusy && !backgroundWorker1.IsBusy)
            {
                progressBar1.Visible = true;
                progressBar1.Value = 0;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            reportViewer1.LocalReport.ReleaseSandboxAppDomain();
            reportViewer1.LocalReport.Dispose();
        }

        private void loadReporte()
        {
            if (reportViewer1.InvokeRequired == true)
            {
                reportViewer1.Invoke((MethodInvoker)delegate
                {
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("Image", constants.getExternalImage("header")));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("user", constants.user));                   
                    try
                    {
                        cotizaciones_local cotizaciones = new cotizaciones_local();
                        cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE datos_reporte");
                        cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (datos_reporte, RESEED, 1)");
                        cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE produccion_t");
                        cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (filas_borradas, RESEED, 1)");

                        loadData();

                        var reporte = new datos_reporte()
                        {
                            Cliente = constants.nombre_cotizacion,
                            Nombre_Proyecto = constants.nombre_proyecto,
                            Fecha = DateTime.Today.ToString("dd/MM/yyyy"),
                            Folio = folio.ToString(),
                        };
                        cotizaciones.datos_reporte.Add(reporte);
                        cotizaciones.SaveChanges();

                        var datos = (from x in cotizaciones.datos_reporte select x);
                        var datos_2 = (from x in cotizaciones.produccion_t select x);
                        datos_reporteBindingSource.DataSource = datos.ToList();
                        produccion_tBindingSource.DataSource = datos_2.ToList();
                        this.datos_reporteTableAdapter.Fill(this.reportes_dataSet.datos_reporte);
                        this.produccion_tTableAdapter.Fill(this.reportes_dataSet.produccion_t);                       
                        this.reportViewer1.LocalReport.Refresh();
                        this.reportViewer1.RefreshReport();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.ToString());
                    }
                });
            }
        }

        private void reloadAll()
        {
            if (!backgroundWorker2.IsBusy && !backgroundWorker1.IsBusy)
            {
                progressBar1.Visible = true;
                progressBar1.Value = 0;
                datagridviewNE1.Rows.Clear();
                backgroundWorker1.RunWorkerAsync();
            }
        }

        //Cargar Todo
        private void loadAll()
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            listas_entities_pva listas = new listas_entities_pva();

            var modulos = from x in cotizaciones.modulos_cotizaciones where x.sub_folio == constants.sub_folio && x.merge_id <= 0 orderby x.orden ascending select x;

            if(modulos != null)
            {
                int c = 0;
                int count = modulos.Count();
                progressBar1.Maximum = modulos.Count();
                sqlDateBaseManager sql = new sqlDateBaseManager();
                DataTable table = sql.selectProduccionTable(folio);

                //Vars
                string[] y = null;
                string descripcion = string.Empty;
                string[] items = new string[] {"No asignado", "Vista por fuera", "Vista por dentro", "Apertura por fuera", "Apertura por dentro", "Apertura por ambos lados"};
                bool t = false;
                List<string> cris_list = new List<string>();
                List<string[]> herrajes_l = new List<string[]>();
                string c_clave = string.Empty;
                string buffer = string.Empty;
                bool n_c = false;
                string cristales = string.Empty;
                string[] _k = null;
                string herrajes = string.Empty;
                string[] _x = null;
              
                foreach (var x in modulos)
                {
                    try {
                        //obtener claves cristales new
                        string[] news = x.news.Split(';');
                        foreach (string k in news)
                        {
                            string[] o = k.Split(',');
                            if (o[0] == "2")
                            {
                                n_c = false;
                                foreach (string p in cris_list)
                                {
                                    if (p == o[1])
                                    {
                                        n_c = true;
                                        break;
                                    }
                                }

                                if (n_c == false)
                                {
                                    cris_list.Add(o[1]);
                                }
                            }
                        }
                        //obtener claves cristales (no-repetir)
                        foreach (char cri in x.claves_cristales)
                        {
                            if (cri != ',')
                            {
                                if (cri != '-' && t == false)
                                {
                                    buffer = buffer + cri.ToString();
                                }
                                else
                                {
                                    t = true;
                                }
                            }
                            else
                            {
                                t = false;
                                n_c = false;
                                c_clave = buffer;
                                buffer = string.Empty;
                                foreach (string p in cris_list)
                                {
                                    if (p == c_clave)
                                    {
                                        n_c = true;
                                        break;
                                    }
                                }

                                if (n_c == false)
                                {
                                    cris_list.Add(c_clave);
                                }
                            }
                        }
                        //--------------------------------------------------------->

                        //obtener nombre de cristales
                        foreach (string p in cris_list)
                        {
                            var g = (from n in listas.lista_precio_corte_e_instalado where n.clave == p select n).SingleOrDefault();

                            if (g != null)
                            {
                                if (cristales.Length == 0)
                                {
                                    cristales = g.articulo;
                                }
                                else
                                {
                                    cristales = cristales + "\n" + g.articulo;
                                }
                            }
                        }
                        //-------------------------------------------------------------->

                        //Color del anodizado
                        string acabado = x.acabado_perfil;
                        string clr = "";
                        var colores = (from p in listas.colores_aluminio where p.clave == acabado select p).SingleOrDefault();

                        if (colores != null)
                        {
                            clr = " - " + colores.color;
                        }
                        //

                        string mosquitero = string.Empty;
                        string elevacion = string.Empty;

                        if (x.articulo.Contains("C/M") == true)
                        {
                            mosquitero = "\nIncluye Mosquitero.";
                        }
                        else if (x.articulo.Contains("S/M") == true)
                        {
                            mosquitero = "\nSin Mosquitero.";
                        }

                        if (acabado != "")
                        {
                            acabado = "\n-Acabado: " + x.acabado_perfil + clr;
                        }
                        else
                        {
                            acabado = "";
                        }

                        if (x.diseño != "CM")
                        {
                            if (cristales.Length > 0)
                            {
                                descripcion = "-Linea: " + x.linea + acabado + "\n-Cristal:\n" + cristales + "\n-Descripción:\n" + x.descripcion + mosquitero;
                            }
                            else
                            {
                                descripcion = "-Linea: " + x.linea + acabado + "\n-Descripción:\n" + x.descripcion + mosquitero;
                            }
                        }
                        else
                        {
                            string[] k = x.claves_otros.Split(',');
                            foreach (string r in k)
                            {
                                string[] z = r.Split('-');
                                if (z.Length == 2)
                                {
                                    string cl = z[0];
                                    float cant = constants.stringToFloat(z[1]);
                                    var ot = (from p in listas.otros where p.clave == cl select p).SingleOrDefault();

                                    if (ot != null)
                                    {
                                        if (ot.linea == "motores" && cant > 0)
                                        {
                                            elevacion = "\nCortina Motorizada.";
                                            break;
                                        }
                                        else
                                        {
                                            elevacion = "\nCortina Manual.";
                                        }
                                    }
                                }
                            }
                            string[] modelo = x.articulo.Split('-');
                            if (modelo.Length > 1)
                            {
                                descripcion = "-Linea: " + x.linea + "\n-Modelo: " + modelo[1] + acabado + "\n-Descripción:\n" + x.descripcion + mosquitero + elevacion;
                            }
                            else
                            {
                                descripcion = "-Linea: " + x.linea + "\n-Modelo: " + x.articulo + acabado + "\n-Descripción:\n" + x.descripcion + mosquitero + elevacion;
                            }
                        }

                        if (x.modulo_id == -2)
                        {
                            descripcion = x.descripcion;
                        }
                    }
                    catch (Exception err)
                    {
                        constants.errorLog(err.ToString());
                        MessageBox.Show(this, "[Error] se ha presentado un problema al cargar datos, puede que la información no se haya cargado correctamente.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    //ADD
                    //----------------------------------------------------------------------------------------------------------------------->
                    y = x.clave.Split('-');
                    try {
                        //get Herrajes ------------------------------------------------------------------------------------------------------>
                        if (!checkBox1.Checked)
                        {
                            if (x.modulo_id == -1)
                            {
                                int q = x.id;
                                var merged = from n in cotizaciones.modulos_cotizaciones where n.merge_id == q && n.sub_folio == constants.sub_folio select n;
                                foreach (var n in merged)
                                {
                                    _k = n.claves_herrajes.Split(',');                                  
                                    foreach (string m in _k.Where(s => s != string.Empty))
                                    {
                                        _x = m.Split('-');
                                        if (_x.Length >= 2)
                                        {
                                            if (constants.stringToFloat(_x[1]) > 0)
                                            {
                                                if (herrajes_l.FirstOrDefault(s => s[0] == _x[0]) == null)
                                                {
                                                    herrajes_l.Add(new string[] { _x[0], _x[1] });
                                                }
                                                else
                                                {
                                                    herrajes_l.FirstOrDefault(s => s[0] == _x[0])[1] = (constants.stringToFloat(herrajes_l.FirstOrDefault(s => s[0] == _x[0])[1].ToString()) + constants.stringToFloat(_x[1])).ToString();
                                                }
                                            }
                                        }
                                    }
                                    if(n.news.Length > 0)
                                    {
                                        _k = n.news.Split(';');
                                        foreach (string m in _k.Where(s => s != string.Empty))
                                        {
                                            _x = m.Split(',');
                                            if (_x.Length > 0)
                                            {
                                                if (_x[0] == "3")
                                                {
                                                    if (constants.stringToFloat(_x[2]) > 0)
                                                    {
                                                        if (herrajes_l.FirstOrDefault(s => s[0] == _x[1]) == null)
                                                        {
                                                            herrajes_l.Add(new string[] { _x[1], _x[2] });
                                                        }
                                                        else
                                                        {
                                                            herrajes_l.FirstOrDefault(s => s[0] == _x[1])[1] = (constants.stringToFloat(herrajes_l.FirstOrDefault(s => s[0] == _x[1])[1].ToString()) + constants.stringToFloat(_x[2])).ToString();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (x.modulo_id > 0)
                            {
                                _k = x.claves_herrajes.Split(',');
                                foreach (string m in _k.Where(s => s != string.Empty))
                                {
                                    _x = m.Split('-');
                                    if (_x.Length >= 2)
                                    {
                                        if (constants.stringToFloat(_x[1]) > 0)
                                        {
                                            if (herrajes_l.FirstOrDefault(s => s[0] == _x[0]) == null)
                                            {
                                                herrajes_l.Add(new string[] { _x[0], _x[1] });
                                            }
                                            else
                                            {
                                                herrajes_l.FirstOrDefault(s => s[0] == _x[0])[1] = (constants.stringToFloat(herrajes_l.FirstOrDefault(s => s[0] == _x[0])[1].ToString()) + constants.stringToFloat(_x[1])).ToString();
                                            }
                                        }
                                    }
                                }
                                if (x.news.Length > 0)
                                {
                                    _k = x.news.Split(';');
                                    foreach (string m in _k.Where(s => s != string.Empty))
                                    {
                                        _x = m.Split(',');
                                        if (_x.Length > 0)
                                        {
                                            if (_x[0] == "3")
                                            {
                                                if (constants.stringToFloat(_x[2]) > 0)
                                                {
                                                    if (herrajes_l.FirstOrDefault(s => s[0] == _x[1]) == null)
                                                    {
                                                        herrajes_l.Add(new string[] { _x[1], _x[2] });
                                                    }
                                                    else
                                                    {
                                                        herrajes_l.FirstOrDefault(s => s[0] == _x[1])[1] = (constants.stringToFloat(herrajes_l.FirstOrDefault(s => s[0] == _x[1])[1].ToString()) + constants.stringToFloat(_x[2])).ToString();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            _x = null;

                            foreach (string[] n in herrajes_l)
                            {
                                string clave = n[0];
                                var l = (from r in listas.herrajes where r.clave == clave select r).SingleOrDefault();
                                if (l != null)
                                {
                                    herrajes = herrajes + "-" + clave + " : " + l.articulo + " #" + n[1] + "\n";
                                }
                            }
                        }
                        //---------------------------------------------------------------------------------------------------------------------------->

                        if (y.Length == 2)
                        {
                            int _v = constants.stringToInt(y[1]);
                            var get_match = (from v in table.AsEnumerable() where v.Field<int>("m_id") == _v select v).FirstOrDefault();

                            if (get_match != null)
                            {
                                _k = null;
                                _k = get_match.Field<string>("parameters").Split(',');
                                if (_k.Length == 2)
                                {
                                    setDataTable(get_match.Field<int>("id"), x.pic, _v, "L: " + x.largo + "\nA: " + x.alto, (float)x.cantidad, x.ubicacion, descripcion, items.FirstOrDefault(s => s == _k[0]) != null ? _k[0] : "No asignado", items.FirstOrDefault(s => s == _k[1]) != null ? _k[1] : "No asignado", get_match.Field<string>("observaciones"), herrajes);
                                }
                                else
                                {
                                    setDataTable(get_match.Field<int>("id"), x.pic, _v, "L: " + x.largo + "\nA: " + x.alto, (float)x.cantidad, x.ubicacion, descripcion, "No asignado", "No asignado", get_match.Field<string>("observaciones"), herrajes);
                                }
                            }
                            else
                            {
                                setDataTable(-1, x.pic, _v, "L: " + x.largo + "\nA: " + x.alto, (float)x.cantidad, x.ubicacion, descripcion, "No asignado", "No asignado", "", herrajes);
                            }
                        }
                        else
                        {
                            setDataTable(-1, x.pic, -1, "L: " + x.largo + "\nA: " + x.alto, (float)x.cantidad, x.ubicacion, descripcion, "No asignado", "No asignado", "", herrajes);
                        }
                    }
                    catch (Exception err)
                    {
                        constants.errorLog(err.ToString());
                        MessageBox.Show(this, "[Error] se ha presentado un problema al cargar datos, intente de nuevo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    cris_list.Clear();
                    herrajes_l.Clear();
                    herrajes = string.Empty;
                    cristales = string.Empty;
                    descripcion = string.Empty;
                    c++;
                    backgroundWorker1.ReportProgress(c);
                }
                //End loop
                checkRows();
            }
        }
     
        private byte[] setDimensionsImage(byte[] pic, string largo, string alto)
        {
            byte[] r = pic;
            int h_increase = 20;
            int w_increase = 80;
            Image img = constants.byteToImage(r);
            Bitmap bm = new Bitmap(img.Width + w_increase, img.Height + h_increase);
            bm.SetResolution(90, 90);       
            using (Graphics gp = Graphics.FromImage(bm))
            {
                gp.Clear(Color.White);
                gp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gp.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                gp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gp.DrawImageUnscaled(img, 0, 0);
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                //Lines
                gp.DrawLine(new Pen(Brushes.Black), new PointF(img.Width + 4, 0), new PointF(img.Width + 4, img.Height));
                gp.DrawLine(new Pen(Brushes.Black), new PointF(0, img.Height + 4), new PointF(img.Width, img.Height + 4));
                //Text
                gp.DrawString(largo + " mm", new Font("Arial", 10), Brushes.Black, new RectangleF(0, img.Height + 5, img.Width, h_increase), sf);
                gp.DrawString(alto + " mm", new Font("Arial", 10), Brushes.Black, new RectangleF(img.Width + 5, 0, w_increase, img.Height), sf);
                r = constants.imageToByte(bm);
            }
            img = null;
            bm = null;
            return r;
        }

        private void setDataTable(int id, byte[] pic, int modulo_id, string medidas, float cantidad, string ubicacion, string descripcion, string vista, string apertura, string observaciones, string herrajes)
        {
            if (datagridviewNE1.InvokeRequired)
            {
                datagridviewNE1.Invoke((MethodInvoker)delegate {
                    datagridviewNE1.Rows.Add(id, pic, modulo_id, medidas, cantidad, ubicacion, descripcion, vista, apertura, observaciones, herrajes);
                });
            }
        }

        //imprimir
        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        //eliminar
        private void button3_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker2.IsBusy && !backgroundWorker1.IsBusy)
            {
                if (datagridviewNE1.RowCount > 0)
                {
                    sqlDateBaseManager sql = new sqlDateBaseManager();

                    if (sql.getFolioProduccion(folio))
                    {
                        if (MessageBox.Show(this, "Se eliminarán todas las ordenes ligadas a esté folio, incluyendo sub-folios. ¿Desea continuar?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            sql.deleteProduccionTable(folio, true, this);
                            reloadAll();
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "[Error] no se encontro ninguna orden ligada a esté folio.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        //abrir ------------------------------------------------------------------------------------>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            loadAll();
            //Report Load
            loadReporte();
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
        }
        //------------------------------------------------------------------------------------------>

        //Guardar ---------------------------------------------------------------------------------->
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            int c = 0;
            int id = 0;
            sqlDateBaseManager sql = new sqlDateBaseManager();
            foreach(DataGridViewRow x in datagridviewNE1.Rows)
            {
                id = constants.stringToInt(x.Cells[0].Value.ToString());
                if (!sql.getProduccionID(id))
                {
                    sql.insertProduccionTable(constants.stringToInt(x.Cells[2].Value.ToString()), x.Cells[7].Value.ToString() + "," + x.Cells[8].Value.ToString(), x.Cells[9].Value.ToString(), folio);
                }
                else
                {
                    sql.updateProduccionTable(id, x.Cells[7].Value.ToString() + "," + x.Cells[8].Value.ToString(), x.Cells[9].Value.ToString(), constants.stringToInt(x.Cells[2].Value.ToString()));
                }
                c++;
                backgroundWorker2.ReportProgress(c);
            }          
        }

        private void BackgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            pictureBox1.Visible = false;
            reloadAll();
            MessageBox.Show(this, "Se ha guardado exitosamente la orden ligada al folio: " + folio + ".", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker2.IsBusy && !backgroundWorker1.IsBusy)
            {
                pictureBox1.Visible = true;
                progressBar1.Visible = true;
                progressBar1.Value = 0;
                progressBar1.Maximum = datagridviewNE1.RowCount;
                backgroundWorker2.RunWorkerAsync();
            }
        }
        //------------------------------------------------------------------------------------------>

        //reload
        private void button4_Click(object sender, EventArgs e)
        {
            reloadAll();
        }

        //Set Data to Report
        private void setData(int id, byte[] pic, string descripcion, string observaciones, string ubicacion, string medidas, string vista, string apertura, string aluminio, string cristal, string otros, string herrajes, int cantidad)
        {
            try
            {
                cotizaciones_local data = new cotizaciones_local();               
                produccion_t p = new produccion_t()
                {
                    id = id,
                    pic = pic,
                    ubicacion = ubicacion,
                    descripcion = descripcion,
                    observaciones = observaciones,
                    medidas = medidas,
                    vista = vista,
                    apertura = apertura,
                    aluminio = aluminio,
                    cristal = cristal,
                    otros = otros,
                    herrajes = herrajes,
                    cantidad = cantidad
                };
                data.produccion_t.Add(p);
                data.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void loadData()
        {
            if (datagridviewNE1.RowCount > 0)
            {
                string[] y = null;
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    if (x.Cells[0].Value.ToString() != "-1")
                    {
                        y = System.Text.RegularExpressions.Regex.Split(x.Cells[3].Value.ToString(), @"(\d+)");
                        if (y.Length == 5)
                        {
                            setData(constants.stringToInt(x.Cells[0].Value.ToString()), setDimensionsImage((byte[])x.Cells[1].Value, y[1], y[3]), x.Cells[6].Value.ToString(), x.Cells[9].Value.ToString(), x.Cells[5].Value.ToString(), x.Cells[3].Value.ToString(), x.Cells[7].Value.ToString(), x.Cells[8].Value.ToString(), "", "", "", x.Cells[10].Value.ToString(), constants.stringToInt(x.Cells[4].Value.ToString()));
                        }                   
                    }
                }
            }
        }

        private void checkRows()
        {
            if (datagridviewNE1.InvokeRequired)
            {
                datagridviewNE1.Invoke((MethodInvoker)delegate
                {
                    foreach (DataGridViewRow x in datagridviewNE1.Rows)
                    {
                        if (x.Cells[0].Value.ToString() == "-1" || x.Cells[2].Value.ToString() == "-1")
                        {
                            x.DefaultCellStyle.BackColor = Color.Yellow;
                        }
                    }
                });            
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            reloadAll();
        }
    }
}
