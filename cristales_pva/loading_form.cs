using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;
using System.IO;

namespace cristales_pva
{
    public partial class loading_form : Form
    {
        listas_entities_pva listas;

        public loading_form()
        {
            InitializeComponent();
            progressBar1.Maximum = 100;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            this.FormClosing += Loading_form_FormClosing;
            backgroundWorker2.RunWorkerCompleted += BackgroundWorker2_RunWorkerCompleted;
        }

        private void Loading_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                e.Cancel = true;
            }
        }

        //inserter ------------------------------------------------------------------------------------------------------------------>
        private void insertListaCostoCorteInstalado(string clave, string articulo, float costo_corte_m2, float costo_instalado, string proveedor, string moneda)
        {
            listas = new listas_entities_pva();
            float tc = constants.tc;

            try
            {
                if (moneda != "MXN")
                {
                    var k = new lista_costo_corte_e_instalado()
                    {
                        clave = clave,
                        articulo = articulo,
                        costo_corte_m2 = Math.Round(costo_corte_m2 * tc, 2),
                        costo_instalado = Math.Round(costo_instalado * tc, 2),
                        proveedor = proveedor,
                        moneda = moneda
                    };
                    listas.lista_costo_corte_e_instalado.Add(k);
                    listas.SaveChanges();
                }
                else
                {
                    var k = new lista_costo_corte_e_instalado()
                    {
                        clave = clave,
                        articulo = articulo,
                        costo_corte_m2 = Math.Round(costo_corte_m2, 2),
                        costo_instalado = Math.Round(costo_instalado, 2),
                        proveedor = proveedor,
                        moneda = moneda
                    };
                    listas.lista_costo_corte_e_instalado.Add(k);
                    listas.SaveChanges();
                }
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertListaPrecioCorteInstalado(string clave, string articulo, float precio_corte_m2, float precio_instalado, string proveedor, string moneda)
        {
            listas = new listas_entities_pva();
            float tc = constants.tc;

            try
            {
                if (moneda != "MXN")
                {
                    var k = new lista_precio_corte_e_instalado()
                    {
                        clave = clave,
                        articulo = articulo,
                        precio_venta_corte_m2 = Math.Round(precio_corte_m2 * tc, 2),
                        precio_venta_instalado = Math.Round(precio_instalado * tc, 2),
                        proveedor = proveedor,
                        moneda = moneda
                    };
                    listas.lista_precio_corte_e_instalado.Add(k);
                    listas.SaveChanges();
                }
                else
                {
                    var k = new lista_precio_corte_e_instalado()
                    {
                        clave = clave,
                        articulo = articulo,
                        precio_venta_corte_m2 = Math.Round(precio_corte_m2, 2),
                        precio_venta_instalado = Math.Round(precio_instalado, 2),
                        proveedor = proveedor,
                        moneda = moneda
                    };
                    listas.lista_precio_corte_e_instalado.Add(k);
                    listas.SaveChanges();
                }
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertListaPrecioHojas(string clave, string articulo, float largo, float alto, float precio_hoja, string proveedor, string moneda)
        {
            listas = new listas_entities_pva();
            float tc = constants.tc;

            try
            {
                if (moneda != "MXN")
                {
                    var k = new lista_precios_hojas()
                    {
                        clave = clave,
                        articulo = articulo,
                        largo = Math.Round(largo, 2),
                        alto = Math.Round(alto, 2),
                        precio_hoja = Math.Round(precio_hoja * tc, 2),
                        proveedor = proveedor,
                        moneda = moneda
                    };
                    listas.lista_precios_hojas.Add(k);
                    listas.SaveChanges();
                }
                else
                {
                    var k = new lista_precios_hojas()
                    {
                        clave = clave,
                        articulo = articulo,
                        largo = Math.Round(largo, 2),
                        alto = Math.Round(alto, 2),
                        precio_hoja = Math.Round(precio_hoja, 2),
                        proveedor = proveedor,
                        moneda = moneda
                    };
                    listas.lista_precios_hojas.Add(k);
                    listas.SaveChanges();
                }
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertListaAcabados(string clave, string acabado, float neto_recto, float neto_curvo)
        {
            listas = new listas_entities_pva();
            try
            {
                var k = new acabado()
                {
                    clave = clave,
                    acabado1 = acabado,
                    neto_recto = Math.Round(neto_recto, 2),
                    neto_curvo = Math.Round(neto_curvo, 2)
                };
                listas.acabados.Add(k);
                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertListaPerfilesCuprum(int id, string clave, string articulo, string linea, string proveedor, float largo, float ancho, float per_a, float crudo, float blanco, float hueso, float champagne, float gris, float negro, float brillante, float natural, float madera, float peso, float chocolate, float acero_inox, float bronce, string moneda)
        {
            listas = new listas_entities_pva();
            float tc = constants.tc;

            try
            {
                if (moneda != "MXN")
                {
                    var k = new perfile()
                    {
                        id = id,
                        clave = clave,
                        articulo = articulo,
                        linea = linea,
                        proveedor = proveedor,
                        largo = Math.Round(largo, 2),
                        ancho_perfil = Math.Round(ancho, 2),
                        perimetro_dm2_ml = Math.Round(per_a, 2),
                        crudo = Math.Round(crudo * tc, 2),
                        blanco = Math.Round(blanco * tc, 2),
                        hueso = Math.Round(hueso * tc, 2),
                        champagne = Math.Round(champagne * tc, 2),
                        gris = Math.Round(gris * tc, 2),
                        negro = Math.Round(negro * tc, 2),
                        brillante = Math.Round(brillante * tc, 2),
                        natural_1 = Math.Round(natural * tc, 2),
                        madera = Math.Round(madera * tc, 2),
                        chocolate = Math.Round(chocolate * tc, 2),
                        acero_inox = Math.Round(acero_inox * tc, 2),
                        bronce = Math.Round(bronce * tc, 2),
                        kg_peso_lineal = Math.Round(peso, 2),
                        moneda = moneda
                    };
                    listas.perfiles.Add(k);
                    listas.SaveChanges();
                }
                else
                {
                    var k = new perfile()
                    {
                        id = id,
                        clave = clave,
                        articulo = articulo,
                        linea = linea,
                        proveedor = proveedor,
                        largo = Math.Round(largo, 2),
                        ancho_perfil = Math.Round(ancho, 2),
                        perimetro_dm2_ml = Math.Round(per_a, 2),
                        crudo = Math.Round(crudo, 2),
                        blanco = Math.Round(blanco, 2),
                        hueso = Math.Round(hueso, 2),
                        champagne = Math.Round(champagne, 2),
                        gris = Math.Round(gris, 2),
                        negro = Math.Round(negro, 2),
                        brillante = Math.Round(brillante, 2),
                        natural_1 = Math.Round(natural, 2),
                        madera = Math.Round(madera, 2),
                        chocolate = Math.Round(chocolate, 2),
                        acero_inox = Math.Round(acero_inox, 2),
                        bronce = Math.Round(bronce, 2),
                        kg_peso_lineal = Math.Round(peso, 2),
                        moneda = moneda
                    };
                    listas.perfiles.Add(k);
                    listas.SaveChanges();
                }
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertListaHerrajes(int id, string clave, string articulo, string proveedor, string linea, string caracteristicas, string color, float precio, string moneda)
        {
            listas = new listas_entities_pva();
            float tc = constants.tc;

            try
            {
                if (moneda != "MXN")
                {
                    var k = new herraje()
                    {
                        id = id,
                        clave = clave,
                        articulo = articulo,
                        proveedor = proveedor,
                        linea = linea,
                        caracteristicas = caracteristicas,
                        color = color,
                        precio = Math.Round(precio * tc, 2),
                        moneda = moneda
                    };
                    listas.herrajes.Add(k);
                    listas.SaveChanges();
                }
                else
                {
                    var k = new herraje()
                    {
                        id = id,
                        clave = clave,
                        articulo = articulo,
                        proveedor = proveedor,
                        linea = linea,
                        caracteristicas = caracteristicas,
                        color = color,
                        precio = Math.Round(precio, 2),
                        moneda = moneda
                    };
                    listas.herrajes.Add(k);
                    listas.SaveChanges();
                }
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertListaOtros(int id, string clave, string articulo, string proveedor, string linea, string caracteristicas, string color, float largo, float alto, float precio, string moneda)
        {
            listas = new listas_entities_pva();
            float tc = constants.tc;

            try
            {
                if (moneda != "MXN")
                {
                    var k = new otro()
                    {
                        id = id,
                        clave = clave,
                        articulo = articulo,
                        proveedor = proveedor,
                        linea = linea,
                        caracteristicas = caracteristicas,
                        color = color,
                        largo = largo,
                        alto = alto,
                        precio = Math.Round(precio * tc, 2),
                        moneda = moneda
                    };
                    listas.otros.Add(k);
                    listas.SaveChanges();
                }
                else
                {
                    var k = new otro()
                    {
                        id = id,
                        clave = clave,
                        articulo = articulo,
                        proveedor = proveedor,
                        linea = linea,
                        caracteristicas = caracteristicas,
                        color = color,
                        largo = largo,
                        alto = alto,
                        precio = Math.Round(precio, 2),
                        moneda = moneda
                    };
                    listas.otros.Add(k);
                    listas.SaveChanges();
                }
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertListaModulos(int id, string clave, string articulo, string linea, string clave_vidrio, string id_aluminio, string id_herrajes, string id_otros, int secciones, string descripcion, string usuario, int id_diseño, bool cs, string parametros, string reglas, bool privado)
        {
            listas = new listas_entities_pva();
            try
            {
                var k = new modulo()
                {
                    id = id,
                    clave = clave,
                    articulo = articulo,                   
                    linea = linea,
                    clave_vidrio = clave_vidrio,
                    id_aluminio = id_aluminio,
                    id_herraje = id_herrajes,
                    id_otros = id_otros,
                    secciones = secciones,
                    descripcion = descripcion,
                    usuario = usuario,
                    id_diseño = id_diseño,
                    cs = cs,
                    parametros = parametros,
                    reglas = reglas,
                    privado = privado
                };
                listas.modulos.Add(k);
                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertCategorias(int id, string categoria, string grupo)
        {
            localDateBaseEntities3 cate = new localDateBaseEntities3();
            try
            {
                var k = new categoria()
                {
                    Id = id,
                    categoria1 = categoria,
                    grupo = grupo
                };
                cate.categorias.Add(k);
                cate.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertProveedores(int id, string proveedor, string grupo)
        {
            localDateBaseEntities3 prove = new localDateBaseEntities3();
            try
            {
                var k = new proveedore()
                {
                    Id = id,
                    proveedor = proveedor,
                    grupo = grupo
                };
                prove.proveedores.Add(k);
                prove.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertLineasModulos(int id, string linea_modulo)
        {
            localDateBaseEntities3 lineas = new localDateBaseEntities3();
            try
            {
                var k = new lineas_modulos()
                {
                    id = id,
                    linea_modulo = linea_modulo
                };
                lineas.lineas_modulos.Add(k);
                lineas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertEsquemas(int id, string nombre, int filas, int columnas, string diseño, string esquemas, bool marco, string grupo)
        {
            listas = new listas_entities_pva();
            try
            {
                var k = new esquema()
                {
                    id = id,
                    nombre = nombre,
                    filas = filas,
                    columnas = columnas,
                    diseño = diseño,
                    esquemas = esquemas,
                    marco = marco,
                    grupo = grupo
                };
                listas.esquemas.Add(k);
                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertColoresAluminio(int id, string clave, string color, string proveedor, float precio, float costo_extra)
        {
            listas = new listas_entities_pva();
            try
            {
                var k = new colores_aluminio()
                {
                    id = id,
                    clave = clave,
                    color = color,
                    proveedor = proveedor,
                    precio = Math.Round(precio, 2),
                    costo_extra_ml = Math.Round(costo_extra, 2)
                };
                listas.colores_aluminio.Add(k);
                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void insertPaquetes(int id, string clave, string items, string type, string articulo)
        {
            listas = new listas_entities_pva();
            try
            {
                var k = new paquete()
                {
                    id = id,
                    comp_clave = clave,
                    comp_items = items,
                    comp_type = type,
                    comp_articulo = articulo               
                };
                listas.paquetes.Add(k);
                listas.SaveChanges();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        public void checkUpdates()
        {
            label1.Text = "Buscando actualizaciones...";
            sqlDateBaseManager sql = new sqlDateBaseManager();
            string version = sql.getNewVersionUpdate();
            constants.db_version = version;
            string v = version;
            string h = constants.version;
            string[] n = v.Split('.');
            v = "";
            foreach(string g in n)
            {
                v = v + g;
            }
            n = h.Split('.');
            h = "";
            foreach(string k in n)
            {
                h = h + k;
            }
            backgroundWorker1.ReportProgress(100);
            if (constants.stringToInt(h) < constants.stringToInt(v))
            {
                new update(version).ShowDialog(this);
            }
        }

        public void insertTablesToLocalDB()
        {
            DataTable t1;
            DataTable t2;
            optimizeLocalDataBase();
            sqlDateBaseManager sql = new sqlDateBaseManager();
            int i;
            label1.Text = "Actualizando Base de Datos Local...";

            backgroundWorker1.ReportProgress(0);

            t1 = sql.createDataTableFromSQLTable("hojas");

            for (i = 0; i < t1.Rows.Count; i++)
            {
                if (t1.Rows[i][0] != null && t1.Rows[i][0].ToString() != "")
                {                   
                    insertListaPrecioHojas(t1.Rows[i][0].ToString(), t1.Rows[i][1].ToString(), constants.stringToFloat(t1.Rows[i][2].ToString()), constants.stringToFloat(t1.Rows[i][3].ToString()), constants.stringToFloat(t1.Rows[i][6].ToString()), t1.Rows[i][8].ToString(), t1.Rows[i][9].ToString());
                }
            }

            backgroundWorker1.ReportProgress(10);

            t1.Reset();
            t1 = sql.createDataTableFromSQLTable("acabados");

            for (i = 0; i < t1.Rows.Count; i++)
            {
                if (t1.Rows[i][0] != null && t1.Rows[i][0].ToString() != "")
                {                   
                    insertListaAcabados(t1.Rows[i][0].ToString(), t1.Rows[i][1].ToString(), constants.stringToFloat(t1.Rows[i][2].ToString()), constants.stringToFloat(t1.Rows[i][3].ToString()));
                }
            }

            backgroundWorker1.ReportProgress(20);

            t1.Reset();
            t1 = sql.createDataTableFromSQLTable("perfiles");

            for (i = 0; i < t1.Rows.Count; i++)
            {
                if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                {                   
                    insertListaPerfilesCuprum((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString(), t1.Rows[i][3].ToString(), t1.Rows[i][4].ToString(),
                    constants.stringToFloat(t1.Rows[i][5].ToString()), constants.stringToFloat(t1.Rows[i][6].ToString()), constants.stringToFloat(t1.Rows[i][7].ToString()), constants.stringToFloat(t1.Rows[i][8].ToString()), constants.stringToFloat(t1.Rows[i][9].ToString()), constants.stringToFloat(t1.Rows[i][10].ToString()), constants.stringToFloat(t1.Rows[i][11].ToString()), constants.stringToFloat(t1.Rows[i][12].ToString()), constants.stringToFloat(t1.Rows[i][13].ToString()), constants.stringToFloat(t1.Rows[i][14].ToString()), constants.stringToFloat(t1.Rows[i][15].ToString()), constants.stringToFloat(t1.Rows[i][16].ToString()), constants.stringToFloat(t1.Rows[i][17].ToString()), constants.stringToFloat(t1.Rows[i][19].ToString()), constants.stringToFloat(t1.Rows[i][20].ToString()), constants.stringToFloat(t1.Rows[i][21].ToString()), t1.Rows[i][23].ToString());
                }
            }

            backgroundWorker1.ReportProgress(40);

            t1.Reset();
            t1 = sql.createDataTableFromSQLTable("herrajes");

            for (i = 0; i < t1.Rows.Count; i++)
            {
                if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                {
                    insertListaHerrajes((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString(), t1.Rows[i][3].ToString(), t1.Rows[i][4].ToString(), t1.Rows[i][5].ToString(), t1.Rows[i][6].ToString(), constants.stringToFloat(t1.Rows[i][7].ToString()), t1.Rows[i][9].ToString());
                }
            }

            backgroundWorker1.ReportProgress(50);

            t1.Reset();
            t1 = sql.createDataTableFromSQLTable("otros");

            for (i = 0; i < t1.Rows.Count; i++)
            {
                if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                {
                    insertListaOtros((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString(), t1.Rows[i][3].ToString(), t1.Rows[i][4].ToString(), t1.Rows[i][5].ToString(), t1.Rows[i][6].ToString(), constants.stringToFloat(t1.Rows[i][7].ToString()), constants.stringToFloat(t1.Rows[i][8].ToString()), constants.stringToFloat(t1.Rows[i][9].ToString()), t1.Rows[i][11].ToString());
                }
            }

            backgroundWorker1.ReportProgress(60);

            t1.Reset();
            t1 = sql.createDataTableFromSQLTable("modulos");

            for (i = 0; i < t1.Rows.Count; i++)
            {
                if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                {
                    insertListaModulos((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString(), t1.Rows[i][3].ToString(), t1.Rows[i][4].ToString(), t1.Rows[i][5].ToString(), t1.Rows[i][6].ToString(), t1.Rows[i][7].ToString(), (int)t1.Rows[i][8], t1.Rows[i][9].ToString(), t1.Rows[i][10].ToString(), (int)t1.Rows[i][11], (bool)t1.Rows[i][12], t1.Rows[i][13].ToString(), t1.Rows[i][14].ToString(), (bool)t1.Rows[i][15]);
                }
            }

            backgroundWorker1.ReportProgress(70);

            t1.Reset();
            t1 = sql.createDataTableFromSQLTable("categorias");

            for (i = 0; i < t1.Rows.Count; i++)
            {
                if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                {
                    insertCategorias((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString());
                }
            }

            t1.Reset();
            t1 = sql.createDataTableFromSQLTable("proveedores");

            for (i = 0; i < t1.Rows.Count; i++)
            {
                if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                {
                    insertProveedores((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString());
                    loadProveedoresFolders(t1.Rows[i][1].ToString());
                }
            }

            t1.Reset();
            t1 = sql.createDataTableFromSQLTable("lineas_modulos");

            for (i = 0; i < t1.Rows.Count; i++)
            {
                if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                {
                    insertLineasModulos((int)t1.Rows[i][0], t1.Rows[i][1].ToString());
                }
            }

            t1.Reset();
            t1 = sql.createDataTableFromSQLTable("esquemas");

            for (i = 0; i < t1.Rows.Count; i++)
            {
                if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                {
                    insertEsquemas((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), (int)t1.Rows[i][2], (int)t1.Rows[i][3], t1.Rows[i][4].ToString(), t1.Rows[i][5].ToString(), (bool)t1.Rows[i][6], t1.Rows[i][7].ToString());
                }
            }

            t1.Reset();
            t1 = sql.createDataTableFromSQLTable("colores_aluminio");

            for (i = 0; i < t1.Rows.Count; i++)
            {
                if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                {
                    insertColoresAluminio((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString(), t1.Rows[i][3].ToString(), constants.stringToFloat(t1.Rows[i][4].ToString()), constants.stringToFloat(t1.Rows[i][5].ToString()));
                    loadColores(t1.Rows[i][1].ToString());
                }
            }

            t1.Reset();
            t1 = sql.createDataTableFromSQLTable("paquetes");

            for (i = 0; i < t1.Rows.Count; i++)
            {
                if (t1.Rows[i][1].ToString() != null && t1.Rows[i][1].ToString() != "")
                {
                    insertPaquetes((int)t1.Rows[i][0], t1.Rows[i][1].ToString(), t1.Rows[i][2].ToString(), t1.Rows[i][3].ToString(), t1.Rows[i][4].ToString());
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
                    insertListaCostoCorteInstalado(t1.Rows[i][0].ToString(), t1.Rows[i][1].ToString(), constants.stringToFloat(t1.Rows[i][5].ToString()), getPrecioInstalado(t2, t1.Rows[i][0].ToString()), t1.Rows[i][9].ToString(), t1.Rows[i][10].ToString());
                    insertListaPrecioCorteInstalado(t1.Rows[i][0].ToString(), t1.Rows[i][1].ToString(), constants.stringToFloat(t1.Rows[i][7].ToString()), getPrecioInstalado(t2, t1.Rows[i][0].ToString()), t1.Rows[i][9].ToString(), t1.Rows[i][10].ToString());
                }
            }

            backgroundWorker1.ReportProgress(100);

            t1.Dispose();
            t2.Dispose();
        }

        private float getPrecioInstalado(DataTable table, string clave)
        {
            float r = 0;
            foreach(DataRow x in table.Rows)
            {
                if(x[0] != null)
                {
                    if(x[0].ToString() == clave)
                    {
                        r = constants.stringToFloat(x[7].ToString());
                    }
                }
            }
            return r;
        }
        //ends inserters ----------------------------------------------------------------------------------------------------------------->     

        //cargar carpetas de proveedores
        private void loadProveedoresFolders(string proveedor)
        {
            if (Directory.Exists(Application.StartupPath + "\\pics\\" + proveedor) == false)
            {
                label3.Text = ">Creando folder de proveedor: " + proveedor + " ...";
                Directory.CreateDirectory(Application.StartupPath + "\\pics\\" + proveedor);
            }
        }
        //

        //cargar colores
        private void loadColores(string clave)
        {
            if (File.Exists(constants.folder_resources_dir + "\\acabados_especiales\\" + clave + ".jpg") == false)
            {
                sqlDateBaseManager sql = new sqlDateBaseManager();
                Image img = sql.getColorImg(clave);
                if (img != null)
                {                   
                    try
                    {
                        Bitmap bm = new Bitmap(img, 60, 60);
                        label3.Text = ">Descargando nuevo color: " + clave + " ...";
                        bm.Save(constants.folder_resources_dir + "\\acabados_especiales\\" + clave + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    catch (Exception) { }
                }
            }
        }
        //

        //optimizador de tabla local
        public void optimizeLocalDataBase()
        {
            listas = new listas_entities_pva();
            try
            {
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE lista_costo_corte_e_instalado");
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE lista_precio_corte_e_instalado");
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE lista_precios_hojas");
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE acabados");
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE perfiles");
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE herrajes");
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE otros");
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE modulos");
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE esquemas");
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE categorias");
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE proveedores");
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE lineas_modulos");
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE colores_aluminio");
                listas.Database.ExecuteSqlCommand("TRUNCATE TABLE paquetes");
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }
        //ends optimizador

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           if(backgroundWorker2.IsBusy == false)
            {
                label2.Text = "";
                label3.Text = "";
                label1.Text = "Configurando Datos...";
                progressBar1.Value = 100;
                backgroundWorker2.RunWorkerAsync();               
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label2.Text = "(" + e.ProgressPercentage + "%)";
        }

        private void loading_form_Load(object sender, EventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if (sql.setServerConnection() == true)
            {
                ((Form1)Application.OpenForms["Form1"]).Enabled = false;
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                Close();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (constants.local == false)
            {
                //TC --------------------------------------------------------------------------->
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
                // ---------------------------------------------------------------------------->
                if (constants.logged == false)
                {
                    constants.logged = true;
                    checkUpdates();
                    constants.downloadPropiedadesModel();
                    constants.loadPropiedadesModel();                                  
                    if (constants.optimizar_inicio == true)
                    {
                        insertTablesToLocalDB();
                    }                                   
                }
                else
                {
                    constants.downloadPropiedadesModel();
                    constants.loadPropiedadesModel();
                    insertTablesToLocalDB();                    
                }              
            }
        }       

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

        private int stringToInt(string num)
        {
            int r = 0;
            if (int.TryParse(num, out r) == true)
            {
                return r;
            }
            else
            {
                return 0;
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            ((Form1)Application.OpenForms["Form1"]).reloadPrecios();
            ((Form1)Application.OpenForms["Form1"]).seleccionarPastaña();
            if (constants.tipo_cotizacion > 0)
            {
                ((Form1)Application.OpenForms["Form1"]).refreshNewArticulo();
            }            
        }

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Text = "listo.";          
            ((Form1)Application.OpenForms["Form1"]).Enabled = true;
            ((Form1)Application.OpenForms["Form1"]).setTCLabel(constants.tc);
            this.Close();
        }

        ~loading_form()
        {

        }
    }
}
