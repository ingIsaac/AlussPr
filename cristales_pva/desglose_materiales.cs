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
    public partial class desglose_materiales : Form
    {
        List<string> claves_cristales = new List<string>();
        List<string> claves_otros = new List<string>();
        List<string> claves_herrajes = new List<string>();
        List<string> claves_perfiles = new List<string>();
        List<string> new_items = new List<string>();

        public desglose_materiales()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if(datagridviewNE1.CurrentRow.Cells[0].Value.ToString() != "Perfil")
            {
                e.Cancel = true;
            }
        }

        private void desglose_materiales_Load(object sender, EventArgs e)
        {
            cargarMateriales();       
        }      

        private void cargarTabla()
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            var data = from x in cotizaciones.materiales_modulos
                       orderby x.concepto ascending
                       select new
                       {
                           Componente = x.concepto == 1 ? "Perfil" : x.concepto == 2 ? "Cristal" : x.concepto == 3 ? "Herraje" : x.concepto == 4 ? "Otros" : "n/a",
                           Clave = x.clave,
                           Artículo = x.articulo,
                           Acabado = x.acabado,
                           Cantidad = Math.Round((float)x.cantidad, 2),
                           Metros_Lineales = x.metros_lineales > 0 ? Math.Round((float)(x.metros_lineales / 1000), 2) + " m" : "",
                           Metros_Cuadrados = x.metros_cuadrados > 0 ? Math.Round((float)x.metros_cuadrados / 1000, 2) + " m2" : "",
                           Perfiles = x.concepto == 1 ? Math.Round(Math.Round((float)(x.metros_lineales / 1000), 2) / (float)x.largo_perfil, 2) + "/" + x.largo_perfil : ""
                       };
            if (data != null)
            {
                if (datagridviewNE1.InvokeRequired == true)
                {
                    datagridviewNE1.Invoke((MethodInvoker)delegate
                    {
                        datagridviewNE1.DataSource = data.ToList();
                        if (datagridviewNE1.RowCount <= 0)
                        {
                            datagridviewNE1.DataSource = null;
                        }
                        else
                        {
                            acomodarDesglose(datagridviewNE1);
                        }
                    });
                }
                else
                {
                    datagridviewNE1.DataSource = data.ToList();
                    if (datagridviewNE1.RowCount <= 0)
                    {
                        datagridviewNE1.DataSource = null;
                    }
                    else
                    {
                        acomodarDesglose(datagridviewNE1);
                    }
                }
            }
        }

        private void acomodarDesglose(DataGridView data)
        {
            foreach (DataGridViewRow x in data.Rows)
            {
                if (x.Cells[0].Value.ToString() == "Perfil")
                {
                    x.DefaultCellStyle.BackColor = Color.LightGray;
                }
                else if (x.Cells[0].Value.ToString() == "Cristal")
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
            data.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            data.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            data.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            data.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            data.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            data.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
        }

        private void cargarMateriales()
        {
            if(backgroundWorker1.IsBusy == false)
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void insertarMateriales(int concepto, string clave, string articulo, float cant, float m_lineales, float m_cuadrados, string acabado="", float largo_perfil=0)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            try
            {
                var p = new materiales_modulos()
                {
                    concepto = concepto,
                    clave = clave,
                    articulo = articulo,
                    cantidad = cant,
                    metros_lineales = m_lineales,
                    metros_cuadrados = m_cuadrados,
                    acabado = acabado,
                    largo_perfil = largo_perfil
                };
                cotizaciones.materiales_modulos.Add(p);
                cotizaciones.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void updateMateriales(string c_clave, float cant, float m_lineales, float m_cuadrados, string acabado="")
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            try
            {
                if (acabado != "")
                {
                    var materiales = (from x in cotizaciones.materiales_modulos where x.clave == c_clave && x.acabado == acabado select x).SingleOrDefault();
                    if (materiales != null)
                    {
                        materiales.cantidad = materiales.cantidad + cant;
                        materiales.metros_lineales = materiales.metros_lineales + m_lineales;
                        materiales.metros_cuadrados = materiales.metros_cuadrados + m_cuadrados;
                    }
                    cotizaciones.SaveChanges();
                }
                else
                {
                    var materiales = (from x in cotizaciones.materiales_modulos where x.clave == c_clave select x).SingleOrDefault();
                    if (materiales != null)
                    {
                        materiales.cantidad = materiales.cantidad + cant;
                        materiales.metros_lineales = materiales.metros_lineales + m_lineales;
                        materiales.metros_cuadrados = materiales.metros_cuadrados + m_cuadrados;
                    }
                    cotizaciones.SaveChanges();
                }
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertarCortes(string clave, string articulo, int modulo_id, string partida, float longitud_corte, float tramo_perfil, string acabado, float cantidad)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            try
            {
                var p = new corte()
                {
                     clave = clave,
                     articulo = articulo,
                     modulo_id = modulo_id,
                     partida = partida,
                     longitud_corte = longitud_corte,
                     tramo_perfil = tramo_perfil,
                     acabado = acabado,
                     cantidad = cantidad
                };
                cotizaciones.cortes.Add(p);
                cotizaciones.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            listas_entities_pva listas = new listas_entities_pva();
            int modulo_id = 0;
            int esquema_id = 0;
            string buffer = string.Empty;
            string c_clave = string.Empty;
            float count = 0;
            string dir = string.Empty;
            string acabado = string.Empty;
            int seccion = -1;
            int m_c = 0;
            int columns = 0;
            int rows = 0;
            int v_count = 1;
            cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE materiales_modulos");
            cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (materiales_modulos, RESEED, 1)");
            cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE cortes");
            cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (cortes, RESEED, 1)");

            var modulos = from x in cotizaciones.modulos_cotizaciones where x.modulo_id > 0 && x.sub_folio == constants.sub_folio select x;
            
            if (modulos != null)
            {
                int counter = 0;
                int modulos_cant = modulos.Count();
                progressBar1.Maximum = modulos_cant;
                progressBar1.Value = 0;

                foreach (var v in modulos)
                {
                    v_count = (int)v.cantidad;
                    counter++;
                    backgroundWorker1.ReportProgress(counter);
                    label1.Text = "Desglosando: " + v.articulo;
                    modulo_id = (int)v.modulo_id;
                    var modulo = (from x in listas.modulos where x.id == modulo_id select x).SingleOrDefault();

                    if (modulo != null)
                    {
                        esquema_id = (int)modulo.id_diseño;
                        var esquema = (from x in listas.esquemas where x.id == esquema_id select x).SingleOrDefault();

                        string[] d_s = v.dimensiones.Split(',');
                        int[] d_num = new int[d_s.Length - 1];

                        for (int i = 0; i < d_num.Length; i++)
                        {
                            d_num[i] = int.Parse(d_s[i]);
                        }

                        int c = 0;

                        if (esquema != null)
                        {
                            columns = (int)esquema.columnas;
                            rows = (int)esquema.filas;
                            if (esquema.marco == false)
                            {
                                c = 1;
                            }
                        }

                        int[,] arr = new int[((int)modulo.secciones + 1) + c, 2];

                        for (int i = 0; i < d_num.Length; i++)
                        {
                            if (i % 2 == 0)
                            {
                                arr[c, 0] = d_num[i];
                            }
                            else
                            {
                                arr[c, 1] = d_num[i];
                                c++;
                            }
                        }

                        claves_perfiles.Clear();
                        foreach (char x in v.claves_perfiles)
                        {
                            if (x == ',')
                            {
                                claves_perfiles.Add(buffer);
                                buffer = string.Empty;
                                continue;
                            }
                            buffer = buffer + x.ToString();
                        }

                        buffer = "";
                        count = 0;
                        dir = "";
                        seccion = -1;
                        c_clave = "";
                        acabado = "";
                        m_c = 0;

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
                                    count = constants.stringToFloat(buffer) * v_count;
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

                                string[] y = claves_perfiles[m_c].Split('-');
                                if (y.Length > 0)
                                {
                                    c_clave = y[0];
                                    if (y.Length > 1)
                                    {
                                        count = constants.stringToFloat(y[1]) * v_count;
                                        if (y.Length == 3)
                                        {
                                            acabado = y[2];
                                        }
                                    }
                                }
                                var perfiles = (from x in listas.perfiles where x.clave == c_clave select x).SingleOrDefault();

                                if (perfiles != null)
                                {
                                    if(acabado == "")
                                    {
                                        acabado = v.acabado_perfil;
                                    }

                                    var materiales = (from x in cotizaciones.materiales_modulos where x.clave == c_clave && x.acabado == acabado select x).SingleOrDefault();

                                    if (count > 0)
                                    {
                                        if (materiales == null)
                                        {
                                            insertarMateriales(1, c_clave, perfiles.articulo, count, dir == "largo" ? arr[seccion, 0] * count : arr[seccion, 1] * count, 0, acabado, (float)perfiles.largo);
                                        }
                                        else
                                        {
                                            updateMateriales(c_clave, count, dir == "largo" ? arr[seccion, 0] * count : arr[seccion, 1] * count, 0, acabado);
                                        }
                                        ///Cortes
                                        insertarCortes(c_clave, perfiles.articulo, modulo_id, v.ubicacion, dir == "largo" ? arr[seccion, 0] * count : arr[seccion, 1], (float)perfiles.largo, acabado, count);
                                    }
                                }

                                m_c++;
                            }
                        }
                        //

                        buffer = "";
                        count = 0;
                        seccion = -1;
                        c_clave = "";
                        m_c = 0;

                        claves_cristales.Clear();
                        foreach (char x in v.claves_cristales)
                        {
                            if (x == ',')
                            {
                                claves_cristales.Add(buffer);
                                buffer = string.Empty;
                                continue;
                            }
                            buffer = buffer + x.ToString();
                        }

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
                                    count = constants.stringToFloat(buffer) * v_count;
                                    buffer = "";
                                    continue;
                                }
                                buffer = buffer + cri.ToString();
                            }
                            else
                            {
                                seccion = constants.stringToInt(buffer);
                                buffer = "";
                               
                                string[] y = claves_cristales[m_c].Split('-');
                                if (y.Length > 0)
                                {
                                    c_clave = y[0];
                                    if (y.Length > 1)
                                    {
                                        count = constants.stringToFloat(y[1]) * v_count;
                                    }
                                }
                                var cristal = (from x in listas.lista_costo_corte_e_instalado where x.clave == c_clave select x).SingleOrDefault();

                                if (cristal != null)
                                {
                                    var materiales = (from x in cotizaciones.materiales_modulos where x.clave == c_clave select x).SingleOrDefault();

                                    if (count > 0)
                                    {
                                        if (materiales == null)
                                        {
                                            if (modulo.cs == true)
                                            {
                                                insertarMateriales(2, c_clave, cristal.articulo, count, 0, ((arr[seccion, 0] * arr[seccion, 1]) * count) / 1000);
                                            }
                                            else
                                            {
                                                insertarMateriales(2, c_clave, cristal.articulo, count, 0, (((arr[seccion, 0] / columns) * (arr[seccion, 1] / rows)) * count) / 1000);
                                            }
                                        }
                                        else
                                        {
                                            if (modulo.cs == true)
                                            {
                                                updateMateriales(c_clave, count, 0, ((arr[seccion, 0] * arr[seccion, 1]) * count) / 1000);
                                            }
                                            else
                                            {
                                                updateMateriales(c_clave, count, 0, (((arr[seccion, 0] / columns) * (arr[seccion, 1] / rows)) * count) / 1000);
                                            }
                                        }
                                    }
                                }

                                m_c++;
                            }
                        }
                        //

                        buffer = "";
                        count = 0;
                        seccion = -1;
                        c_clave = "";
                        m_c = 0;

                        claves_herrajes.Clear();
                        foreach (char x in v.claves_herrajes)
                        {
                            if (x == ',')
                            {
                                claves_herrajes.Add(buffer);
                                buffer = string.Empty;
                                continue;
                            }
                            buffer = buffer + x.ToString();
                        }

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
                                    count = constants.stringToFloat(buffer) * v_count;
                                    buffer = "";
                                    continue;
                                }
                                buffer = buffer + h.ToString();
                            }
                            else
                            {
                                seccion = constants.stringToInt(buffer);
                                buffer = "";

                                string[] y = claves_herrajes[m_c].Split('-');
                                if (y.Length > 0)
                                {
                                    c_clave = y[0];
                                    if (y.Length > 1)
                                    {
                                        count = constants.stringToFloat(y[1]) * v_count;
                                    }
                                }
                                var herrajes = (from s in listas.herrajes where s.clave == c_clave select s).SingleOrDefault();

                                if (herrajes != null)
                                {
                                    var materiales = (from x in cotizaciones.materiales_modulos where x.clave == c_clave select x).SingleOrDefault();

                                    if (count > 0)
                                    {
                                        if (materiales == null)
                                        {
                                            insertarMateriales(3, c_clave, herrajes.articulo, count, 0, 0);
                                        }
                                        else
                                        {
                                            updateMateriales(c_clave, count, 0, 0);
                                        }
                                    }
                                }

                                m_c++;
                            }
                        }
                        //

                        buffer = "";
                        count = 0;
                        dir = "";
                        seccion = -1;
                        c_clave = "";
                        m_c = 0;

                        claves_otros.Clear();
                        foreach (char x in v.claves_otros)
                        {
                            if (x == ',')
                            {
                                claves_otros.Add(buffer);
                                buffer = string.Empty;
                                continue;
                            }
                            buffer = buffer + x.ToString();
                        }

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
                                    count = constants.stringToFloat(buffer) * v_count;
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
                               
                                string[] y = claves_otros[m_c].Split('-');
                                if (y.Length > 0)
                                {
                                    c_clave = y[0];
                                    if (y.Length > 1)
                                    {
                                        count = constants.stringToFloat(y[1]) * v_count;
                                    }
                                }
                                var otros = (from x in listas.otros where x.clave == c_clave select x).SingleOrDefault();

                                if (otros != null)
                                {
                                    var materiales = (from x in cotizaciones.materiales_modulos where x.clave == c_clave select x).SingleOrDefault();

                                    if (count > 0)
                                    {
                                        if (materiales == null)
                                        {
                                            if (modulo.cs == true)
                                            {
                                                insertarMateriales(4, c_clave, otros.articulo, count, dir == "largo" ? arr[seccion, 0] * count : dir == "alto" ? arr[seccion, 1] * count : 0, otros.largo <= 0 ? 0 : otros.alto <= 0 ? 0 : ((arr[seccion, 0] * arr[seccion, 1]) * count) / 1000);
                                            }
                                            else
                                            {
                                                insertarMateriales(4, c_clave, otros.articulo, count, dir == "largo" ? arr[seccion, 0] * count : dir == "alto" ? arr[seccion, 1] * count : 0, otros.largo <= 0 ? 0 : otros.alto <= 0 ? 0 : (((arr[seccion, 0] / columns) * (arr[seccion, 1] / rows)) * count) / 1000);
                                            }
                                        }
                                        else
                                        {
                                            if (modulo.cs == true)
                                            {
                                                updateMateriales(c_clave, count, dir == "largo" ? arr[seccion, 0] * count : dir == "alto" ? arr[seccion, 1] * count : 0, otros.largo <= 0 ? 0 : otros.alto <= 0 ? 0 : ((arr[seccion, 0] * arr[seccion, 1]) * count) / 1000);
                                            }
                                            else
                                            {
                                                updateMateriales(c_clave, count, dir == "largo" ? arr[seccion, 0] * count : dir == "alto" ? arr[seccion, 1] * count : 0, otros.largo <= 0 ? 0 : otros.alto <= 0 ? 0 : (((arr[seccion, 0] / columns) * (arr[seccion, 1] / rows)) * count) / 1000);
                                            }
                                        }
                                    }
                                }

                                m_c++;
                            }
                        }
                        //

                        count = 0;
                        dir = "";
                        seccion = -1;
                        c_clave = "";

                        new_items.Clear();
                        foreach (char x in v.news)
                        {
                            if (x == ';')
                            {
                                new_items.Add(buffer);
                                buffer = string.Empty;
                                continue;
                            }
                            buffer = buffer + x.ToString();
                        }

                        //news
                        foreach (string n in new_items)
                        {
                            string[] nw = n.Split(',');
                            int concept = constants.stringToInt(nw[0]);
                            c_clave = nw[1];
                            count = constants.stringToFloat(nw[2]) * v_count;
                            dir = nw[3];
                            seccion = constants.stringToInt(nw[4]);
                            acabado = nw[5];

                            if (concept == 1)
                            {
                                var perfiles = (from x in listas.perfiles where x.clave == c_clave select x).SingleOrDefault();

                                if (perfiles != null)
                                {
                                    if (acabado == "")
                                    {
                                        acabado = v.acabado_perfil;
                                    }

                                    var materiales = (from x in cotizaciones.materiales_modulos where x.clave == c_clave && x.acabado == acabado select x).SingleOrDefault();

                                    if (count > 0)
                                    {
                                        if (materiales == null)
                                        {
                                            insertarMateriales(1, c_clave, perfiles.articulo, count, dir == "largo" ? arr[seccion, 0] * count : arr[seccion, 1] * count, 0, acabado, (float)perfiles.largo);
                                        }
                                        else
                                        {
                                            updateMateriales(c_clave, count, dir == "largo" ? arr[seccion, 0] * count : arr[seccion, 1] * count, 0, acabado);
                                        }
                                        ///Cortes
                                        insertarCortes(c_clave, perfiles.articulo, modulo_id, v.ubicacion, dir == "largo" ? arr[seccion, 0] * count : arr[seccion, 1], (float)perfiles.largo, acabado, count);
                                    }
                                }
                            }
                            else if (concept == 2)
                            {
                                var cristal = (from x in listas.lista_costo_corte_e_instalado where x.clave == c_clave select x).SingleOrDefault();

                                if (cristal != null)
                                {
                                    var materiales = (from x in cotizaciones.materiales_modulos where x.clave == c_clave select x).SingleOrDefault();

                                    if (count > 0)
                                    {
                                        if (materiales == null)
                                        {
                                            if (modulo.cs == true)
                                            {
                                                insertarMateriales(2, c_clave, cristal.articulo, count, 0, ((arr[seccion, 0] * arr[seccion, 1]) * count) / 1000);
                                            }
                                            else
                                            {
                                                insertarMateriales(2, c_clave, cristal.articulo, count, 0, (((arr[seccion, 0] / columns) * (arr[seccion, 1] / rows)) * count) / 1000);
                                            }
                                        }
                                        else
                                        {
                                            if (modulo.cs == true)
                                            {
                                                updateMateriales(c_clave, count, 0, ((arr[seccion, 0] * arr[seccion, 1]) * count) / 1000);
                                            }
                                            else
                                            {
                                                updateMateriales(c_clave, count, 0, (((arr[seccion, 0] / columns) * (arr[seccion, 1] / rows)) * count) / 1000);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (concept == 3)
                            {
                                var herrajes = (from s in listas.herrajes where s.clave == c_clave select s).SingleOrDefault();

                                if (herrajes != null)
                                {
                                    var materiales = (from x in cotizaciones.materiales_modulos where x.clave == c_clave select x).SingleOrDefault();

                                    if (count > 0)
                                    {
                                        if (materiales == null)
                                        {
                                            insertarMateriales(3, c_clave, herrajes.articulo, count, 0, 0);
                                        }
                                        else
                                        {
                                            updateMateriales(c_clave, count, 0, 0);
                                        }
                                    }
                                }
                            }
                            else if (concept == 4)
                            {
                                var otros = (from x in listas.otros where x.clave == c_clave select x).SingleOrDefault();

                                if (otros != null)
                                {
                                    var materiales = (from x in cotizaciones.materiales_modulos where x.clave == c_clave select x).SingleOrDefault();

                                    if (count > 0)
                                    {
                                        if (materiales == null)
                                        {
                                            if (modulo.cs == true)
                                            {
                                                insertarMateriales(4, c_clave, otros.articulo, count, dir == "largo" ? arr[seccion, 0] * count : dir == "alto" ? arr[seccion, 1] * count : 0, otros.largo <= 0 ? 0 : otros.alto <= 0 ? 0 : ((arr[seccion, 0] * arr[seccion, 1]) * count) / 1000);
                                            }
                                            else
                                            {
                                                insertarMateriales(4, c_clave, otros.articulo, count, dir == "largo" ? arr[seccion, 0] * count : dir == "alto" ? arr[seccion, 1] * count : 0, otros.largo <= 0 ? 0 : otros.alto <= 0 ? 0 : (((arr[seccion, 0] / columns) * (arr[seccion, 1] / rows)) * count) / 1000);
                                            }
                                        }
                                        else
                                        {
                                            if (modulo.cs == true)
                                            {
                                                updateMateriales(c_clave, count, dir == "largo" ? arr[seccion, 0] * count : dir == "alto" ? arr[seccion, 1] * count : 0, otros.largo <= 0 ? 0 : otros.alto <= 0 ? 0 : ((arr[seccion, 0] * arr[seccion, 1]) * count) / 1000);
                                            }
                                            else
                                            {
                                                updateMateriales(c_clave, count, dir == "largo" ? arr[seccion, 0] * count : dir == "alto" ? arr[seccion, 1] * count : 0, otros.largo <= 0 ? 0 : otros.alto <= 0 ? 0 : (((arr[seccion, 0] / columns) * (arr[seccion, 1] / rows)) * count) / 1000);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                     //-----------------------> END
                    }
                }
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Enabled = true;
            progressBar1.Visible = false;
            cargarTabla();
            label1.Text = "Se encontrarón: (" + datagridviewNE1.Rows.Count + ") tipos de materiales.";
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["desglose"] == null)
            {
                new desglose().Show();
            }
        }

        private void verCortesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                if (Application.OpenForms["cortes"] == null)
                {
                    new cortes(datagridviewNE1.CurrentRow.Cells[1].Value.ToString(),datagridviewNE1.CurrentRow.Cells[3].Value.ToString()).Show();
                }
                else
                {
                    Application.OpenForms["cortes"].Select();
                    Application.OpenForms["cortes"].WindowState = FormWindowState.Normal;
                }
            }
        }
    }
}
