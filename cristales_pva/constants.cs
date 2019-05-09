using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Drawing;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using System.Xml;
using System.Data;
using System.Net.Mail;
using Microsoft.Reporting.WinForms;
using System.Net.Sockets;

namespace cristales_pva
{
    static class constants
    {
        public static string app_name = "AlussPr";
        public static Boolean connected = false, error = false, cotizacion_error = false;
        public static Boolean online = true;
        public static string msg_box_caption = string.Empty, org_name = string.Empty;
        public static string user = string.Empty, password = string.Empty;
        public static int user_id;
        public static string mac_address = string.Empty;
        public static int user_access = 0;
        public static Boolean local = false, _false_activation = false, logged = false;
        public static string propiedades_xml = Application.StartupPath + "\\propiedades.xml";
        public static string opciones_xml = Application.StartupPath + "\\opciones.xml";
        public static string updater_xml = Application.StartupPath + "\\data_updater.xml";
        public static string folder_resources_dir = Application.StartupPath + "\\pics\\";
        public static string directorio_xml = Application.StartupPath + "\\directorio.xml";
        public static string release_n = "478";
        public static string ps_dl = "45uT2Q23l";
        public static Socket login_server = null;

        //Properties...
        public static string licencia = string.Empty;
        public static float iva = 1.16f;
        public static float tc = 0;
        public static bool enable_c_tc = true;
        public static int updater_interval = 1;
        public static bool updater_enable = true;
        public static bool updater_form_close = true;
        public static bool maximizar_ventanas = false;
        public static bool optimizar_inicio = true;
        public static bool mostrar_acabado = false;
        public static bool ac_cotizacion = true;
        public static string db_version = string.Empty;
        public static string version = string.Empty;
        public static string server = string.Empty;
        public static string data_base = string.Empty;
        public static string server_user = string.Empty;
        public static string server_password = string.Empty;
        public static string header_reporte = string.Empty;
        public static bool enable_cs = true;
        public static bool enable_rules = true;
        public static float desc_cristales = 0;
        public static float desc_aluminio = 0;
        public static float desc_herrajes = 0;
        public static float desc_otros = 0;
        public static float desc_cant = 0;
        public static bool iva_desglosado = true;
        public static bool permitir_ajuste_iva = true;
        public static bool siempre_permitir_ac = true;
        public static bool ajustar_medidas_aut = true;
        public static bool op1 = false;
        public static bool op2 = false;
        public static bool op3 = false;
        public static bool op4 = false;
        public static bool op5 = false;
        public static bool op6 = false;
        public static bool op7 = false;
        public static bool op8 = false;
        public static bool op9 = false;
        public static bool op10 = false;
        public static bool m_liva = false;
        public static bool ingreso_ac = false;
        public static string user_ac = string.Empty;
        public static string password_ac = string.Empty;
        public static bool reload_precios = true;
        public static bool p_ac = false;
        public static float lim_sm = 15;
        public static int monitor_interval = 1;
        public static float fsconfig = 11;
        public static float costo_aluminio_kg = 0;
        public static bool enable_costo_alum_kg = true;
        public static bool anuncios = true;
        public static string smtp = "smtp.live.com";
        public static int m_port = 587;
        public static int timeout = 10000;

        //Temporales...
        public static int folio_abierto = -1, id_articulo_cotizacion = -1, tipo_cotizacion = 0;
        public static string nombre_cotizacion = string.Empty;
        public static string nombre_proyecto = string.Empty;
        public static string fecha_cotizacion = string.Empty;
        public static string autor_cotizacion = string.Empty;
        public static List<string> subfolio_titles = new List<string>();
        public static float desc_cotizacion = 0;
        public static float utilidad_cotizacion = 0;
        public static int folio_eliminacion = -1;
        public static Boolean cotizacion_proceso = false;
        public static Boolean cotizacion_guardada = false;
        public static int count_cristales = 0, count_aluminio = 0, count_herrajes = 0, count_otros = 0, count_modulos = 0;
        public static bool permitir_cp = false;
        public static int sub_folio = 1;
        public static List<int> errors_Open = new List<int>();
        public static List<string> save_onEdit = new List<string>();
        public static bool update_later = false;
        public static bool user_forbid = false;
        public static string factory_acabado_perfil = string.Empty;
        public static string factory_cristal = string.Empty;

        public static void getSoftwareVersion()
        {
            try
            {
                version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            }
            catch (Exception err)
            {
                errorLog(err.ToString());
                version = "0.0.0.0";
            }
        }

        public static FileVersionInfo getFileInfoVersion()
        {
            try
            {
                return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public static int countRowsSQL(string table)
        {
            return new sqlDateBaseManager().countSQLRows(table);
        }

        public static void setServerCredentials()
        {
            localDateBaseEntities3 local = new localDateBaseEntities3();

            var credenciales = (from x in local.logins where x.id == 1 select x).SingleOrDefault();

            if(credenciales != null)
            {
                server_user = credenciales.usuario;
                server_password = credenciales.contraseña;               
            }           
        }

        public static string getPublicIP()
        {
            string externalip = string.Empty;
            string c = string.Empty;        
            try
            {
                XDocument propiedades = XDocument.Load(constants.propiedades_xml);

                var web_c = (from x in propiedades.Descendants("Propiedades") select x.Element("WEBC")).SingleOrDefault();

                if (web_c != null)
                {
                    c = web_c.Value;
                }
                //
                externalip = new WebClient().DownloadString(c);
            }
            catch (Exception)
            {
                externalip = "0.0.0.0";
            }
            return externalip;
        }

        public static string getMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    if (adapter.GetPhysicalAddress() != null)
                    {
                        sMacAddress = adapter.GetPhysicalAddress().ToString();
                    }
                    else
                    {
                        MessageBox.Show("[Error]: No se puede obtener la mac del equipo, revisé en adaptador de red.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        System.Environment.Exit(0);
                    }
                }
            }            
            return sMacAddress;     
        }

        public static void errorLog(string error)
        {
            try
            {
                string dir = Application.StartupPath + "\\error_log.txt";
                if (!File.Exists(dir))
                {
                    File.Create(dir);
                    TextWriter text = new StreamWriter(dir);
                    text.Write(DateTime.Today.ToString("dd/MM/yyyy") + " | " + DateTime.Now.ToString("HH:mm:ss") + " /////////////-->" + error + "\n");
                    text.Close();
                }
                else
                {
                    TextWriter text = new StreamWriter(dir);
                    text.Write(DateTime.Today.ToString("dd/MM/yyyy") + " | " + DateTime.Now.ToString("HH:mm:ss") + " /////////////-->" + error + "\n");
                    text.Close();
                }
            }
            catch (Exception) { }         
        }

        public static Boolean isInteger(string num)
        {          
            int r;
            return int.TryParse(num, out r);
        }    

        public static Boolean isFloat(string num)
        {
            float r;
            return float.TryParse(num, out r);
        }

        public static Boolean isLong(string num)
        {
            long r;
            return long.TryParse(num, out r);
        }

        public static int stringToInt(string num)
        {
            int r = 0;
            if(int.TryParse(num, out r) == true)
            {
                return r;
            }
            else
            {
                return 0;
            }
        }

        public static float stringToFloat(string num, bool round=false)
        {
            float r = 0;
            if (float.TryParse(num, out r) == true)
            {
                if(round == true)
                {
                    return (float)Math.Round(r, 2);
                }
                else
                {
                    return r;
                }
            }
            else
            {
                return 0;
            }
        }

        public static double stringToDouble(string num, bool round = false)
        {
            double r = 0;
            if (double.TryParse(num, out r) == true)
            {
                if (round == true)
                {
                    return Math.Round(r, 2);
                }
                else
                {
                    return r;
                }
            }
            else
            {
                return 0;
            }
        }

        //tools --------------------------------------------------------------------------------------------------------------
        public static void copyAlltoClipboard(DataGridView table)
        {           
            if (table.InvokeRequired == true)
            {
                table.Invoke((MethodInvoker)delegate
                {
                    table.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                    table.MultiSelect = true;
                    table.SelectAll();
                    DataObject dataObj = table.GetClipboardContent();
                    if (dataObj != null)
                    {
                        Clipboard.SetDataObject(dataObj);
                    }
                    table.MultiSelect = false;
                });
            }
            else {                
                table.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
                table.MultiSelect = true;
                table.SelectAll();
                DataObject dataObj = table.GetClipboardContent();
                if (dataObj != null)
                {
                    Clipboard.SetDataObject(dataObj);
                }
                table.MultiSelect = false;
            }
        }
        //ends tools

        //Export --------------------------------------------------------------------------------------------------------------
        public static void ExportToExcelFile(DataGridView table)
        {
            copyAlltoClipboard(table);
            try {
                Microsoft.Office.Interop.Excel.Application xlexcel;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;
                xlexcel = new Microsoft.Office.Interop.Excel.Application();
                xlexcel.Visible = true;
                xlWorkBook = xlexcel.Workbooks.Add(misValue);
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                Microsoft.Office.Interop.Excel.Range CR = (Microsoft.Office.Interop.Excel.Range)xlWorkSheet.Cells[1, 1];
                CR.Select();
                xlWorkSheet.PasteSpecial(CR, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            }catch(Exception err)
            {
                MessageBox.Show("[Error] fallo al exportar los datos.", msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorLog(err.ToString());
            }
        }
        //ends Export

        //Copy paste code --> de stackoverflow
        public static void PasteOnGrid(DataGridView grid, bool addRow = false, bool toupper=false)
        {
            try { 
                char[] rowSplitter = { '\r', '\n' };
                char[] columnSplitter = { '\t' };

                IDataObject dataInClipboard = Clipboard.GetDataObject();
                string stringInClipboard = (string)dataInClipboard.GetData(DataFormats.Text);

                string[] rowsInClipboard = stringInClipboard.Split(rowSplitter, StringSplitOptions.RemoveEmptyEntries);

                int r = grid.SelectedCells[0].RowIndex;
                int c = grid.SelectedCells[0].ColumnIndex;

                for (int iRow = 0; iRow < rowsInClipboard.Length; iRow++)
                {
                    if (addRow == false)
                    {
                        if (grid.RowCount - 1 >= r + iRow)
                        {
                            string[] valuesInRow = rowsInClipboard[iRow].Split(columnSplitter);

                            for (int iCol = 0; iCol < valuesInRow.Length; iCol++)
                            {
                                if (grid.ColumnCount - 1 >= c + iCol)
                                {
                                    if (!grid.Rows[r + iRow].Cells[c + iCol].ReadOnly)
                                    {
                                        grid.Rows[r + iRow].Cells[c + iCol].Selected = true;
                                        grid.BeginEdit(true);
                                        if (toupper == true)
                                        {
                                            grid.Rows[r + iRow].Cells[c + iCol].Value = valuesInRow[iCol].ToUpper();
                                        }
                                        else
                                        {
                                            grid.Rows[r + iRow].Cells[c + iCol].Value = valuesInRow[iCol];
                                        }
                                        grid.EndEdit();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {                        
                        string[] valuesInRow = rowsInClipboard[iRow].Split(columnSplitter);

                        if (grid.RowCount - 1 <= r + iRow)
                        {
                            grid.Rows.Add();
                        }

                        for (int iCol = 0; iCol < valuesInRow.Length; iCol++)
                        {
                            if (grid.ColumnCount - 1 >= c + iCol)
                            {
                                if (!grid.Rows[r + iRow].Cells[c + iCol].ReadOnly)
                                {
                                    if (toupper == true)
                                    {
                                        grid.Rows[r + iRow].Cells[c + iCol].Value = valuesInRow[iCol].ToUpper();
                                    }
                                    else
                                    {
                                        grid.Rows[r + iRow].Cells[c + iCol].Value = valuesInRow[iCol];
                                    }
                                }
                            }
                        }
                    }
                }
                //-------->
            }
            catch (Exception)
            {
                MessageBox.Show("[Error] clipboard error.", msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //ends copy y paste

        //ip local
        public static string GetLocalIPAddress()
        {
            string _ip = "Error ip";
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    _ip = ip.ToString();
                }                                               
            }
            return _ip;          
        }
        //ends ip local

        //folio para cotizaciones
        public static int setFolio()
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if (getFolio() > 0)
            {
                return getFolio() + 1;
            }
            else
            {
                return 1000000;
            }
        }

        public static int getFolio()
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            try
            {
                return int.Parse(sql.selectLastFromTable("cotizaciones", "folio"));
            }catch(Exception)
            {
                return 0;
            }
        }
        //ends folio

        public static void clearCotizacionesLocales()
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            try {
                cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE cristales_cotizados");
                cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE aluminio_cotizado");
                cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE herrajes_cotizados");
                cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE otros_cotizaciones");
                cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE modulos_cotizaciones");

                //autoincrement reseed -->
                cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (cristales_cotizados, RESEED, 1)");
                cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (aluminio_cotizado, RESEED, 1)");
                cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (herrajes_cotizados, RESEED, 1)");
                cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (otros_cotizaciones, RESEED, 1)");
                cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (modulos_cotizaciones, RESEED, 1)");
            }
            catch (Exception err)
            {
                errorLog(err.ToString());
            }
        }

        //borrar una sola tabla de cotizacion
        public static void clearCotizacionLocal(string table)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            try
            {
                cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE " + table);
               
                //autoincrement reseed -->
                cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT ("+table+", RESEED, 1)");                
            }
            catch (Exception err)
            {
                errorLog(err.ToString());
            }
        }
        //

        //get image from file
        public static Image getImageFromFile(string folder, string file_name, string extension)
        {
            Image img = null;
            try
            {
                img = Image.FromFile(Application.StartupPath + "\\pics\\" + folder + "\\" + file_name + "." + extension);
                img = new Bitmap(img, new Size(64, 32));
            }
            catch (Exception)
            {
                img = Properties.Resources.Actions_edit_delete_icon;
            }
            return img;
        }

        //Coloca imagen a modulo
        public static void setImage(string folder, string file_name, string extension, PictureBox image)
        {
            image.WaitOnLoad = false;
            try
            {              
                image.LoadAsync(Application.StartupPath + "\\pics\\" + folder + "\\" + file_name + "." + extension);
            }
            catch (Exception)
            {
                image.Image = Properties.Resources.No_Image_Available;
            }       
        }

        //check image if exist
        public static bool imageExist(string folder, string file_name, string extension)
        {
            return File.Exists(Application.StartupPath + "\\pics\\" + folder + "\\" + file_name + "." + extension);                    
        }

        //Coloca imagen a modulo
        public static void setBackgroundImg(string folder, string file_name, string extension, Control control)
        {
            try
            {
                Image img = new Bitmap(Application.StartupPath + "\\pics\\" + folder + "\\" + file_name + "." + extension);
                control.BackgroundImage = img;
                img = null;
            }
            catch (Exception)
            {
                control.BackgroundImage = Properties.Resources.no_color;
            }
        }
        //

        //Encuentra la id de un articulo para-sobreescribir
        public static Boolean getArticuloIdLocalDB(int tipo, int id)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            if (tipo == 1)
            {
                var c = (from x in cotizaciones.cristales_cotizados where x.id == id select x).SingleOrDefault();
                if (c == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (tipo == 2)
            {
                var c = (from x in cotizaciones.aluminio_cotizado where x.id == id select x).SingleOrDefault();
                if (c == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (tipo == 3)
            {
                var c = (from x in cotizaciones.herrajes_cotizados where x.id == id select x).SingleOrDefault();
                if (c == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (tipo == 4)
            {
                var c = (from x in cotizaciones.otros_cotizaciones where x.id == id select x).SingleOrDefault();
                if (c == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (tipo == 5)
            {
                var c = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();
                if (c == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        //

        //reload cotizaciones
        public static void reloadCotizaciones()
        {
            ((Form1)Application.OpenForms["Form1"]).reloadAll();      
        }
        //

        //deserializar clave + id COTIZACION
        public static int getIDFromClave(string clave)
        {
            int id = 0;
            bool g = false;
            string f = "";

            foreach(char x in clave)
            {
                if (g == true)
                {
                    f = f + x.ToString();                
                }
                if (x == '-')
                {
                    g = true;
                }               
            }
            try
            {
                id = int.Parse(f);
            }
            catch (Exception e)
            {
                errorLog(e.ToString());
            }

            return id;
        }

        //deserializar clave + id ORIGINAL
        public static int getOriginalIDFromClave(string clave)
        {
            int id = 0;
            bool g = false;
            string f = "";

            foreach (char x in clave)
            {
                if (g == true)
                {
                    if (x == '-')
                    {
                        break;
                    }
                    else
                    {
                        f = f + x.ToString();
                    }
                }
                if (x == '#')
                {
                    g = true;
                }
            }

            try
            {
                id = int.Parse(f);
            }
            catch (Exception e)
            {
                errorLog(e.ToString());
            }

            return id;
        }

        public static string getClave(string clave)
        {
            string f = "";
            foreach (char x in clave)
            {              
                if (x == '-')
                {
                    break;
                }
                f = f + x.ToString();
            }       
            return f;
        }
        //

        //load grid from cotizaciones locales
        public static void loadCotizacionesLocales(string tipo, DataGridView datagrid, bool editMode = false, bool buscar = false, string param = "")
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            if (tipo == "cristales")
            {
                var data = from x in cotizaciones.cristales_cotizados
                           select new
                           {
                               Id = x.id,
                               Folio = x.folio,
                               Clave = x.clave,
                               Artículo = x.articulo,
                               Venta = x.tipo_venta,
                               Lista = x.lista,
                               Proveedor = x.proveedor,
                               Cantidad = x.cantidad,
                               Largo = x.largo,
                               Alto = x.alto,
                               Filo_Muerto = x.filo_muerto,
                               Canteado = x.canteado,
                               Biselado = x.biselado,
                               Desconchado = x.desconchado,
                               Pecho_Paloma = x.pecho_paloma,
                               Perforado_media = x.perforado_media_pulgada,
                               Perforado_una = x.perforado_una_pulgada,
                               Perforado_dos = x.perforado_dos_pulgadas,
                               Grabado = x.grabado,
                               Esmerilado = x.esmerilado,
                               Utilidad = x.descuento,
                               Total = Math.Round((float)x.total, 2)
                           };
                if(buscar == true)
                {
                    data = null;
                    data = from x in cotizaciones.cristales_cotizados where x.clave.StartsWith(param) || x.articulo.StartsWith(param)
                           select new
                           {
                               Id = x.id,
                               Folio = x.folio,
                               Clave = x.clave,
                               Artículo = x.articulo,
                               Venta = x.tipo_venta,
                               Lista = x.lista,
                               Proveedor = x.proveedor,
                               Cantidad = x.cantidad,
                               Largo = x.largo,
                               Alto = x.alto,
                               Filo_Muerto = x.filo_muerto,
                               Canteado = x.canteado,
                               Biselado = x.biselado,
                               Desconchado = x.desconchado,
                               Pecho_Paloma = x.pecho_paloma,
                               Perforado_media = x.perforado_media_pulgada,
                               Perforado_una = x.perforado_una_pulgada,
                               Perforado_dos = x.perforado_dos_pulgadas,
                               Grabado = x.grabado,
                               Esmerilado = x.esmerilado,
                               Utilidad = x.descuento,
                               Total = Math.Round((float)x.total, 2)
                           };
                }
                if (data != null)
                {
                    if (datagrid.InvokeRequired == true)
                    {
                        datagrid.Invoke((MethodInvoker)delegate
                        {
                            datagrid.DataSource = data.ToList();
                            if(datagrid.RowCount <= 0)
                            {
                                datagrid.DataSource = null;
                            }
                            else
                            {
                                datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                            }
                        });
                    }
                    else
                    {
                        datagrid.DataSource = data.ToList();
                        if (datagrid.RowCount <= 0)
                        {
                            datagrid.DataSource = null;
                        }
                        else
                        {
                            datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                        }
                    }
                }
            }
            else if (tipo == "aluminio")
            {
                var data = from x in cotizaciones.aluminio_cotizado
                           select new
                           {
                               Id = x.id,
                               Folio = x.folio,
                               Clave = x.clave,
                               Artículo = x.articulo,
                               Lista = x.lista,
                               Proveedor = x.proveedor,
                               Linea = x.linea,
                               Cantidad = x.cantidad,
                               Largo = x.largo_total,
                               Acabado = x.acabado,
                               Utilidad = x.descuento,
                               Total = x.total
                           };
                if(buscar == true)
                {
                    data = null;
                    data = from x in cotizaciones.aluminio_cotizado where x.clave.StartsWith(param) || x.articulo.StartsWith(param)
                           select new
                           {
                               Id = x.id,
                               Folio = x.folio,
                               Clave = x.clave,
                               Artículo = x.articulo,
                               Lista = x.lista,
                               Proveedor = x.proveedor,
                               Linea = x.linea,
                               Cantidad = x.cantidad,
                               Largo = x.largo_total,
                               Acabado = x.acabado,
                               Utilidad = x.descuento,
                               Total = x.total
                           };
                }
                if (data != null)
                {
                    if (datagrid.InvokeRequired == true)
                    {
                        datagrid.Invoke((MethodInvoker)delegate
                        {
                            datagrid.DataSource = data.ToList();
                            if (datagrid.RowCount <= 0)
                            {
                                datagrid.DataSource = null;
                            }
                            else
                            {
                                datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                            }
                        });
                    }
                    else
                    {
                        datagrid.DataSource = data.ToList();
                        if (datagrid.RowCount <= 0)
                        {
                            datagrid.DataSource = null;
                        }
                        else
                        {
                            datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                        }
                    }
                }
            }
            else if (tipo == "herrajes")
            {
                var data = from x in cotizaciones.herrajes_cotizados
                           select new
                           {
                               Id = x.id,
                               Folio = x.folio,
                               Clave = x.clave,
                               Artículo = x.articulo,
                               Linea = x.linea,
                               Proveedor = x.proveedor,
                               Acabado = x.color,
                               Cantidad = x.cantidad,
                               Utilidad = x.descuento,
                               Total = x.total
                           };
                if(buscar == true)
                {
                    data = null;
                    data = from x in cotizaciones.herrajes_cotizados where x.clave.StartsWith(param) || x.articulo.StartsWith(param)
                           select new
                           {
                               Id = x.id,
                               Folio = x.folio,
                               Clave = x.clave,
                               Artículo = x.articulo,
                               Linea = x.linea,
                               Proveedor = x.proveedor,
                               Acabado = x.color,
                               Cantidad = x.cantidad,
                               Utilidad = x.descuento,
                               Total = x.total
                           };
                }
                if (data != null)
                {
                    if (datagrid.InvokeRequired == true)
                    {
                        datagrid.Invoke((MethodInvoker)delegate
                        {
                            datagrid.DataSource = data.ToList();
                            if (datagrid.RowCount <= 0)
                            {
                                datagrid.DataSource = null;
                            }
                            else
                            {
                                datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                            }
                        });
                    }
                    else
                    {
                        datagrid.DataSource = data.ToList();
                        if (datagrid.RowCount <= 0)
                        {
                            datagrid.DataSource = null;
                        }
                        else
                        {
                            datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                        }
                    }
                }
            }
            else if (tipo == "otros")
            {
                var data = from x in cotizaciones.otros_cotizaciones
                           select new
                           {
                               Id = x.id,
                               Folio = x.folio,
                               Clave = x.clave,
                               Artículo = x.articulo,
                               Linea = x.linea,
                               Proveedor = x.proveedor,
                               Color = x.color,
                               Largo = x.largo,
                               Alto = x.alto,
                               Cantidad = x.cantidad,
                               Utilidad = x.descuento,
                               Total = x.total
                           };
                if(buscar == true)
                {
                    data = null;
                    data = from x in cotizaciones.otros_cotizaciones where x.clave.StartsWith(param) || x.articulo.StartsWith(param)
                           select new
                           {
                               Id = x.id,
                               Folio = x.folio,
                               Clave = x.clave,
                               Artículo = x.articulo,
                               Linea = x.linea,
                               Proveedor = x.proveedor,
                               Color = x.color,
                               Largo = x.largo,
                               Alto = x.alto,
                               Cantidad = x.cantidad,
                               Utilidad = x.descuento,
                               Total = x.total
                           };
                }
                if (data != null)
                {
                    if (datagrid.InvokeRequired == true)
                    {
                        datagrid.Invoke((MethodInvoker)delegate
                        {
                            datagrid.DataSource = data.ToList();
                            if (datagrid.RowCount <= 0)
                            {
                                datagrid.DataSource = null;
                            }
                            else
                            {
                                datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                            }
                        });
                    }
                    else
                    {
                        datagrid.DataSource = data.ToList();
                        if (datagrid.RowCount <= 0)
                        {
                            datagrid.DataSource = null;
                        }
                        else
                        {
                            datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                        }
                    }
                }
            }
            else if (tipo == "modulos")
            {
                if (editMode == true)
                {
                    if (datagrid.InvokeRequired == true)
                    {
                        datagrid.Invoke((MethodInvoker)delegate
                        {
                            if (datagrid.RowCount > 0)
                            {
                                datagrid.Rows.Clear();
                            }
                            if (datagrid.ColumnCount > 0)
                            {
                                datagrid.Columns.Clear();
                            }
                            datagrid.Refresh();
                            DataGridViewImageColumn clm_1 = new DataGridViewImageColumn();
                            clm_1.Name = "CS";
                            clm_1.HeaderText = "CS";
                            datagrid.Columns.Add("Id", "Id");
                            datagrid.Columns.Add("Orden", "Orden");
                            datagrid.Columns.Add(clm_1);
                            datagrid.Columns.Add("Folio", "Folio");
                            datagrid.Columns.Add("Clave", "Clave");
                            datagrid.Columns.Add("Modulo_Id", "Modulo_Id");
                            datagrid.Columns.Add("Artículo", "Artículo");
                            datagrid.Columns.Add("Linea", "Linea");
                            datagrid.Columns.Add("Diseño", "Diseño");                  
                            datagrid.Columns.Add("Descripción", "Descripción");
                            datagrid.Columns.Add("Ubicación", "Ubicación");
                            datagrid.Columns.Add("Cristales", "Cristales");
                            datagrid.Columns.Add("Acabado", "Acabado");
                            datagrid.Columns.Add("Largo", "Largo");
                            datagrid.Columns.Add("Alto", "Alto");
                            datagrid.Columns.Add("Cantidad", "Cantidad");
                            datagrid.Columns.Add("Total", "Total");

                            var data = (from x in cotizaciones.modulos_cotizaciones where x.merge_id <= 0 && x.sub_folio == sub_folio orderby x.orden select x);
                            DataGridViewColumn descripcion = datagrid.Columns[9];
                            DataGridViewColumn orden = datagrid.Columns[1];
                            descripcion.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            descripcion.Width = 280;
                            orden.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            orden.DefaultCellStyle.Font = new Font("Arial", 15, FontStyle.Bold);
                            orden.DefaultCellStyle.ForeColor = Color.Red;

                            if(buscar == true)
                            {
                                data = null;
                                data = (from x in cotizaciones.modulos_cotizaciones where x.merge_id <= 0 && x.sub_folio == sub_folio && (x.clave.StartsWith(param) || x.articulo.StartsWith(param) || x.ubicacion.StartsWith(param) || x.id.ToString().StartsWith(param)) orderby x.orden select x);
                            }
                            foreach(var c in data)
                            {
                                if(c.pic == null)
                                {
                                    c.pic = constants.imageToByte(Properties.Resources.new_concepto);
                                }
                                datagrid.Rows.Add(c.id, c.orden, c.pic, c.folio, c.clave, c.modulo_id, c.articulo, c.linea, c.diseño, c.descripcion, c.ubicacion, getCristales(c.claves_cristales, c.news), c.acabado_perfil, c.largo, c.alto, c.cantidad, c.total);                               
                            }
                            if (datagrid.RowCount > 0)
                            {
                                datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                                setConceptEditZone(datagrid);
                            }
                        });
                    }
                    else
                    {
                        if (datagrid.RowCount > 0)
                        {
                            datagrid.Rows.Clear();
                        }
                        if (datagrid.ColumnCount > 0)
                        {
                            datagrid.Columns.Clear();
                        }
                        datagrid.Refresh();
                        DataGridViewImageColumn clm_1 = new DataGridViewImageColumn();
                        clm_1.Name = "CS";
                        clm_1.HeaderText = "CS";
                        datagrid.Columns.Add("Id", "Id");
                        datagrid.Columns.Add("Orden", "Orden");
                        datagrid.Columns.Add(clm_1);
                        datagrid.Columns.Add("Folio", "Folio");
                        datagrid.Columns.Add("Clave", "Clave");
                        datagrid.Columns.Add("Modulo_Id", "Modulo_Id");
                        datagrid.Columns.Add("Artículo", "Artículo");
                        datagrid.Columns.Add("Linea", "Linea");
                        datagrid.Columns.Add("Diseño", "Diseño");
                        datagrid.Columns.Add("Descripción", "Descripción");
                        datagrid.Columns.Add("Ubicación", "Ubicación");
                        datagrid.Columns.Add("Cristales", "Cristales");
                        datagrid.Columns.Add("Acabado", "Acabado");
                        datagrid.Columns.Add("Largo", "Largo");
                        datagrid.Columns.Add("Alto", "Alto");
                        datagrid.Columns.Add("Cantidad", "Cantidad");
                        datagrid.Columns.Add("Total", "Total");

                        var data = (from x in cotizaciones.modulos_cotizaciones where x.merge_id <= 0 && x.sub_folio == sub_folio orderby x.orden select x);
                        DataGridViewColumn descripcion = datagrid.Columns[9];
                        DataGridViewColumn orden = datagrid.Columns[1];
                        descripcion.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        descripcion.Width = 280;
                        orden.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        orden.DefaultCellStyle.Font = new Font("Arial", 15, FontStyle.Bold);
                        orden.DefaultCellStyle.ForeColor = Color.Red;

                        if (buscar == true)
                        {
                            data = null;
                            data = (from x in cotizaciones.modulos_cotizaciones where x.merge_id <= 0 && x.sub_folio == sub_folio && (x.clave.StartsWith(param) || x.articulo.StartsWith(param) || x.ubicacion.StartsWith(param) || x.id.ToString().StartsWith(param)) orderby x.orden select x);
                        }
                        foreach (var c in data)
                        {
                            if (c.pic == null)
                            {
                                c.pic = constants.imageToByte(Properties.Resources.new_concepto);
                            }
                            datagrid.Rows.Add(c.id, c.orden, c.pic, c.folio, c.clave, c.modulo_id, c.articulo, c.linea, c.diseño, c.descripcion, c.ubicacion, getCristales(c.claves_cristales, c.news), c.acabado_perfil, c.largo, c.alto, c.cantidad, c.total);                            
                        }                       
                        if (datagrid.RowCount > 0)
                        {
                            datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                            setConceptEditZone(datagrid);
                        }
                    }
                }
                else
                {
                    if (enable_cs == true)
                    {
                        var data = (from x in cotizaciones.modulos_cotizaciones
                                   where x.merge_id <= 0 && x.sub_folio == sub_folio orderby x.orden select x).AsEnumerable().Select(o => new
                                   {
                                       Id = o.id,
                                       CS = o.pic,
                                       Folio = o.folio,
                                       Clave = o.clave,
                                       Modulo_Id = o.modulo_id,
                                       Artículo = o.articulo,
                                       Linea = o.linea,
                                       Diseño = o.diseño,
                                       Descripción = o.descripcion,                                       
                                       Ubicación = o.ubicacion,
                                       Cristales = getCristales(o.claves_cristales, o.news),
                                       Acabado = o.acabado_perfil,
                                       Largo = o.largo,
                                       Alto = o.alto,
                                       Cantidad = o.cantidad,
                                       Total = o.total
                                   });
                        if (data != null)
                        {
                            if (datagrid.InvokeRequired == true)
                            {
                                datagrid.Invoke((MethodInvoker)delegate
                                {
                                    datagrid.DataSource = data.ToList();
                                    if (datagrid.RowCount <= 0)
                                    {
                                        datagrid.DataSource = null;
                                    }
                                    else
                                    {
                                        datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                                    }
                                });
                            }
                            else
                            {
                                datagrid.DataSource = data.ToList();
                                if (datagrid.RowCount <= 0)
                                {
                                    datagrid.DataSource = null;
                                }
                                else
                                {
                                    datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        byte[] n = null;
                        var data = (from x in cotizaciones.modulos_cotizaciones
                                   where x.merge_id <= 0 && x.sub_folio == sub_folio orderby x.orden select x).AsEnumerable().Select(o => new
                                   {
                                       Id = o.id,
                                       CS = n,
                                       Folio = o.folio,
                                       Clave = o.clave,
                                       Modulo_Id = o.modulo_id,
                                       Artículo = o.articulo,
                                       Linea = o.linea,
                                       Diseño = o.diseño,
                                       Descripción = o.descripcion,
                                       Ubicación = o.ubicacion,
                                       Cristales = getCristales(o.claves_cristales, o.news),
                                       Acabado = o.acabado_perfil,
                                       Largo = o.largo,
                                       Alto = o.alto,
                                       Cantidad = o.cantidad,
                                       Total = o.total
                                   });
                        if (data != null)
                        {
                            if (datagrid.InvokeRequired == true)
                            {
                                datagrid.Invoke((MethodInvoker)delegate
                                {
                                    datagrid.DataSource = data.ToList();
                                    if (datagrid.RowCount <= 0)
                                    {
                                        datagrid.DataSource = null;
                                    }
                                    else
                                    {
                                        datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                                    }
                                });
                            }
                            else
                            {
                                datagrid.DataSource = data.ToList();
                                if (datagrid.RowCount <= 0)
                                {
                                    datagrid.DataSource = null;
                                }
                                else
                                {
                                    datagrid.FirstDisplayedScrollingRowIndex = datagrid.RowCount - 1;
                                }
                            }
                        }
                    }              
                }
            }
        }

        public static void setConceptEditZone(DataGridView datagrid, bool editable=false)
        {
            if (datagrid.RowCount > 0)
            {
                cotizaciones_local cotizaciones = new cotizaciones_local();
                int p = 0;
                foreach (DataGridViewRow x in datagrid.Rows)
                {
                    if (x.Cells[5].Value.ToString() == "-1")
                    {
                        foreach (DataGridViewCell v in x.Cells)
                        {
                            if (v.ColumnIndex == 1 || v.ColumnIndex == 9 || v.ColumnIndex == 10 || v.ColumnIndex == 15)
                            {
                                v.ReadOnly = false;
                                v.Style.BackColor = Color.LightBlue;
                            }
                            else if (v.ColumnIndex == 13 || v.ColumnIndex == 14)
                            {                              
                                p = (int)x.Cells[0].Value;
                                var k = (from n in cotizaciones.modulos_cotizaciones where n.merge_id == p select n).Where(n => n.dir == 0).FirstOrDefault(); 
                                if(k != null)
                                {
                                    v.ReadOnly = false;
                                    v.Style.BackColor = Color.LightBlue;
                                }
                                else
                                {
                                    v.ReadOnly = true;
                                }
                            }
                            else
                            {
                                v.ReadOnly = true;
                            }
                            if (v.ColumnIndex == 11 || v.ColumnIndex == 12)
                            {
                                v.Style.BackColor = Color.LightGreen;
                            }
                            if (v.ColumnIndex == 0)
                            {
                                if (folio_abierto > 0 && x.Cells[2].Value.ToString() == "0")
                                {
                                    v.Style.BackColor = Color.Yellow;
                                }
                            }
                        }
                        x.Cells[5].Style.BackColor = Color.Orange;
                    }
                    else if (x.Cells[5].Value.ToString() == "-2")
                    {
                        foreach (DataGridViewCell v in x.Cells)
                        {
                            if (v.ColumnIndex == 1 || v.ColumnIndex == 6 || v.ColumnIndex == 9 || v.ColumnIndex == 10 || v.ColumnIndex > 12)
                            {
                                v.ReadOnly = false;
                                v.Style.BackColor = Color.LightBlue;
                            }
                            else
                            {
                                v.ReadOnly = true;
                            }
                            if (v.ColumnIndex == 0)
                            {
                                if (folio_abierto > 0 && x.Cells[2].Value.ToString() == "0")
                                {
                                    v.Style.BackColor = Color.Yellow;
                                }
                            }
                        }
                        x.Cells[5].Style.BackColor = Color.Orange;
                    }
                    else
                    {
                        foreach (DataGridViewCell v in x.Cells)
                        {
                            if (v.ColumnIndex == 1 || v.ColumnIndex == 9 || v.ColumnIndex == 10)
                            {
                                v.ReadOnly = false;
                                v.Style.BackColor = Color.LightBlue;
                            }
                            else
                            {
                                v.ReadOnly = true;
                            }
                            if (v.ColumnIndex == 11 || v.ColumnIndex == 12)
                            {
                                v.Style.BackColor = Color.LightGreen;
                            }
                            if (v.ColumnIndex == 0)
                            {
                                if (folio_abierto > 0 && x.Cells[2].Value.ToString() == "0")
                                {
                                    v.Style.BackColor = Color.Yellow;
                                }
                            }
                        }
                    }
                }
            }
        }

        //ver articulos para personalizados
        public static void getItemsToGetMerged(DataGridView datagrid, string filter="")
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            if (filter != "")
            {
                var data = (from x in cotizaciones.modulos_cotizaciones
                            where x.merge_id <= 0 && x.concept_id < 0 && x.sub_folio == sub_folio && (x.clave.StartsWith(filter) || x.ubicacion.StartsWith(filter) || x.id.ToString().StartsWith(filter))
                            orderby x.orden
                            select x).AsEnumerable().Select(o => new
                            {
                                Id = o.id,
                                CS = o.pic,
                                Folio = o.folio,
                                Clave = o.clave,
                                Modulo_Id = o.modulo_id,
                                Artículo = o.articulo,
                                Linea = o.linea,
                                Diseño = o.diseño,
                                Descripción = o.descripcion,
                                Ubicación = o.ubicacion,
                                Cristales = getCristales(o.claves_cristales, o.news),
                                Acabado = o.acabado_perfil,
                                Largo = o.largo,
                                Alto = o.alto,
                                Cantidad = o.cantidad,
                                Total = o.total
                            });
                if (data != null)
                {
                    if (datagrid.InvokeRequired == true)
                    {
                        datagrid.Invoke((MethodInvoker)delegate
                        {
                            datagrid.DataSource = data.ToList();
                            if (datagrid.RowCount <= 0)
                            {
                                datagrid.DataSource = null;
                            }
                            else
                            {
                                foreach (DataGridViewColumn x in datagrid.Columns)
                                {
                                    if (x.HeaderText == "Cristales" || x.HeaderText == "Acabado")
                                    {
                                        x.DefaultCellStyle.BackColor = Color.LightGreen;
                                    }
                                }
                            }
                        });
                    }
                    else
                    {
                        datagrid.DataSource = data.ToList();
                        if (datagrid.RowCount <= 0)
                        {
                            datagrid.DataSource = null;
                        }
                        else
                        {
                            foreach (DataGridViewColumn x in datagrid.Columns)
                            {
                                if (x.HeaderText == "Cristales" || x.HeaderText == "Acabado")
                                {
                                    x.DefaultCellStyle.BackColor = Color.LightGreen;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var data = (from x in cotizaciones.modulos_cotizaciones
                            where x.merge_id <= 0 && x.concept_id < 0 && x.sub_folio == sub_folio
                            orderby x.orden
                            select x).AsEnumerable().Select(o => new
                            {
                                Id = o.id,
                                CS = o.pic,
                                Folio = o.folio,
                                Clave = o.clave,
                                Modulo_Id = o.modulo_id,
                                Artículo = o.articulo,
                                Linea = o.linea,
                                Diseño = o.diseño,
                                Descripción = o.descripcion,
                                Ubicación = o.ubicacion,
                                Cristales = getCristales(o.claves_cristales, o.news),
                                Acabado = o.acabado_perfil,
                                Largo = o.largo,
                                Alto = o.alto,
                                Cantidad = o.cantidad,
                                Total = o.total
                            });
                if (data != null)
                {
                    if (datagrid.InvokeRequired == true)
                    {
                        datagrid.Invoke((MethodInvoker)delegate
                        {
                            datagrid.DataSource = data.ToList();
                            if (datagrid.RowCount <= 0)
                            {
                                datagrid.DataSource = null;
                            }
                            else
                            {
                                foreach (DataGridViewColumn x in datagrid.Columns)
                                {
                                    if (x.HeaderText == "Cristales" || x.HeaderText == "Acabado")
                                    {
                                        x.DefaultCellStyle.BackColor = Color.LightGreen;
                                    }
                                }
                            }
                        });
                    }
                    else
                    {
                        datagrid.DataSource = data.ToList();
                        if (datagrid.RowCount <= 0)
                        {
                            datagrid.DataSource = null;
                        }
                        else
                        {
                            foreach (DataGridViewColumn x in datagrid.Columns)
                            {
                                if (x.HeaderText == "Cristales" || x.HeaderText == "Acabado")
                                {
                                    x.DefaultCellStyle.BackColor = Color.LightGreen;
                                }
                            }
                        }
                    }
                }
            }            
        }
        //

        //ver articulos personalizados
        public static void getMargedItems(DataGridView datagrid, int perso_id, string filter="")
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            if (filter != "")
            {
                var data = (from x in cotizaciones.modulos_cotizaciones
                        where x.merge_id == perso_id && x.sub_folio == sub_folio && (x.clave.StartsWith(filter) || x.ubicacion.StartsWith(filter) || x.id.ToString().StartsWith(filter))
                        orderby x.orden
                        select x).AsEnumerable().Select(o => new
                        {
                            Id = o.id,
                            CS = o.pic,
                            Folio = o.folio,
                            Clave = o.clave,
                            Modulo_Id = o.modulo_id,
                            Artículo = o.articulo,
                            Linea = o.linea,
                            Configuración = o.dir == 0 ? "Indefinido" : o.dir >= 110 ? o.dir.ToString()[0] + "," + o.dir.ToString()[1] + "," + o.dir.ToString()[2] : o.dir.ToString(),
                            Diseño = o.diseño,
                            Descripción = o.descripcion,
                            Ubicación = o.ubicacion,
                            Cristales = getCristales(o.claves_cristales, o.news),
                            Acabado = o.acabado_perfil,
                            Largo = o.largo,
                            Alto = o.alto,
                            Cantidad = o.cantidad,
                            Total = o.total
                        });
                if (data != null)
                {
                    if (datagrid.InvokeRequired == true)
                    {
                        datagrid.Invoke((MethodInvoker)delegate
                        {
                            datagrid.DataSource = data.ToList();
                            if (datagrid.RowCount <= 0)
                            {
                                datagrid.DataSource = null;
                            }
                            else
                            {
                                foreach (DataGridViewColumn x in datagrid.Columns)
                                {
                                    if (x.HeaderText == "Configuración" || x.HeaderText == "Cristales" || x.HeaderText == "Acabado")
                                    {
                                        x.DefaultCellStyle.BackColor = Color.LightGreen;
                                    }
                                }
                            }
                        });
                    }
                    else
                    {
                        datagrid.DataSource = data.ToList();
                        if (datagrid.RowCount <= 0)
                        {
                            datagrid.DataSource = null;
                        }
                        else
                        {
                            foreach (DataGridViewColumn x in datagrid.Columns)
                            {
                                if (x.HeaderText == "Configuración" || x.HeaderText == "Cristales" || x.HeaderText == "Acabado")
                                {
                                    x.DefaultCellStyle.BackColor = Color.LightGreen;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                var data = (from x in cotizaciones.modulos_cotizaciones
                        where x.merge_id == perso_id && x.sub_folio == sub_folio
                        orderby x.orden
                        select x).AsEnumerable().Select(o => new
                        {
                            Id = o.id,
                            CS = o.pic,
                            Folio = o.folio,
                            Clave = o.clave,
                            Modulo_Id = o.modulo_id,
                            Artículo = o.articulo,
                            Linea = o.linea,
                            Configuración = o.dir == 0 ? "Indefinido" : o.dir >= 110 ? o.dir.ToString()[0] + "," + o.dir.ToString()[1] + "," + o.dir.ToString()[2] : o.dir.ToString(),
                            Diseño = o.diseño,
                            Descripción = o.descripcion,
                            Ubicación = o.ubicacion,
                            Cristales = getCristales(o.claves_cristales, o.news),
                            Acabado = o.acabado_perfil,
                            Largo = o.largo,
                            Alto = o.alto,
                            Cantidad = o.cantidad,
                            Total = o.total
                        });
                if (data != null)
                {
                    if (datagrid.InvokeRequired == true)
                    {
                        datagrid.Invoke((MethodInvoker)delegate
                        {
                            datagrid.DataSource = data.ToList();
                            if (datagrid.RowCount <= 0)
                            {
                                datagrid.DataSource = null;
                            }
                            else
                            {
                                foreach (DataGridViewColumn x in datagrid.Columns)
                                {
                                    if (x.HeaderText == "Configuración" || x.HeaderText == "Cristales" || x.HeaderText == "Acabado")
                                    {
                                        x.DefaultCellStyle.BackColor = Color.LightGreen;
                                    }
                                }
                            }
                        });
                    }
                    else
                    {
                        datagrid.DataSource = data.ToList();
                        if (datagrid.RowCount <= 0)
                        {
                            datagrid.DataSource = null;
                        }
                        else
                        {
                            foreach (DataGridViewColumn x in datagrid.Columns)
                            {
                                if (x.HeaderText == "Configuración" || x.HeaderText == "Cristales" || x.HeaderText == "Acabado")
                                {
                                    x.DefaultCellStyle.BackColor = Color.LightGreen;
                                }
                            }
                        }
                    }
                }
            }           
        }

        public static void updateModuloPersonalizado(int id)
        {           
            cotizaciones_local cotizaciones = new cotizaciones_local();
            
            var concepto = (from c in cotizaciones.modulos_cotizaciones where c.concept_id == id select c).SingleOrDefault();

            if (concepto != null)
            {
                concepto.modulo_id = -1;
                if (concepto.dir != 3)
                {
                    concepto.pic = null;
                }
                concepto.articulo = "";
                concepto.total = 0;
                concepto.linea = "";
                concepto.ubicacion = "";              
                concepto.acabado_perfil = "";
                concepto.diseño = "";
                concepto.claves_cristales = "";
                concepto.news = "";
                concepto.largo = 0;
                concepto.alto = 0;
                int dir_p = -1;

                var modulos = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == concepto.concept_id select x);
                if (modulos != null)
                {                  
                    Image img = null;
                    cotizaciones.SaveChanges();
                    foreach (var v in modulos)
                    {
                        if (concepto.dir != 3)
                        {
                            img = MergeTwoImages(byteToImage(concepto.pic), byteToImage(v.pic));                          
                        }
                        concepto.modulo_id = -1;
                        concepto.pic = img != null ? imageToByte(img) : concepto.dir == 3 ? concepto.pic : v.pic;
                        concepto.total = Math.Round((float)(concepto.total + v.total), 2);
                        concepto.articulo = concepto.articulo.Length > 0 ? concepto.articulo + " + " + v.articulo : v.articulo;
                        concepto.linea = concepto.linea.Length <= 0 ? v.linea : getLineasInConcept(concepto.linea, v.linea) == false ? concepto.linea + "/" + v.linea : concepto.linea;
                        concepto.ubicacion = v.ubicacion.Length <= 0 ? concepto.ubicacion : concepto.ubicacion.Length <= 0 ? v.ubicacion : getUbicacionInConcept(concepto.ubicacion, v.ubicacion) == true ? concepto.ubicacion : v.ubicacion.Length > 0 ? concepto.ubicacion + "/" + v.ubicacion : concepto.ubicacion;
                        concepto.acabado_perfil = concepto.acabado_perfil == "" ? v.acabado_perfil : concepto.acabado_perfil;
                        concepto.diseño = concepto.diseño.Length > 0 ? concepto.diseño + "/" + v.diseño : v.diseño;
                        concepto.claves_cristales = concepto.claves_cristales + v.claves_cristales;
                        concepto.news = concepto.news + v.news;
                    }
                    concepto.total = concepto.total * concepto.cantidad;
                }                          

                //New Medidas ------------------------------------------------------------------------------------------->
                var c = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == concepto.concept_id && x.dir > 0 select x).Count();

                if (c > 0)
                {
                    concepto.largo = 0;
                    concepto.alto = 0;
                    cotizaciones.SaveChanges();

                    modulos = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == concepto.concept_id && x.dir > 0 && x.dir < 100 orderby x.largo ascending select x).ThenBy(x => x.dir);

                    if (modulos != null)
                    {                     
                        foreach (var v in modulos)
                        {
                            concepto.largo = (int)getLargoUnificado((int)v.dir, (float)concepto.largo, (float)v.largo, (float)v.cantidad, dir_p);
                            dir_p = (int)v.dir;
                        }
                    }

                    dir_p = -1;

                    modulos = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == concepto.concept_id && x.dir > 0 && x.dir < 100 orderby x.alto ascending select x).ThenBy(x => x.dir);

                    if (modulos != null)
                    {
                        foreach (var v in modulos)
                        {
                            concepto.alto = (int)getAltoUnificado((int)v.dir, (float)concepto.alto, (float)v.alto, (float)v.cantidad, dir_p);
                            dir_p = (int)v.dir;
                        }
                    }
                    //----------------------------------------------------------------------------------------------------------->


                    //Nuevo Algoritmo ------------------------------------------------------------------------------------------->
                    modulos = null;
                    modulos = (from x in cotizaciones.modulos_cotizaciones where (x.merge_id == concepto.concept_id && x.dir >= 110) orderby x.dir ascending select x);

                    if(modulos != null)
                    {
                        try {
                            List<string> buffer;
                            List<string> g = new List<string>();
                            List<List<string>> rows = new List<List<string>>();
                            List<List<string>> columns = new List<List<string>>();
                            string[] k = null;

                            foreach (var x in modulos)
                            {
                                if (x.dir >= 110)
                                {
                                    g.Add(x.dir.ToString()[0] + "," + x.dir.ToString()[1] + "," + x.dir.ToString()[2]);
                                }
                            }

                            if (g.Count > 0)
                            {
                                for (int i = 1; i < 8; i++)
                                {
                                    buffer = new List<string>();
                                    foreach (string v in g)
                                    {
                                        k = v.Split(',');
                                        if (k.Length >= 2)
                                        {
                                            if (k[0] == i.ToString())
                                            {
                                                buffer.Add(v);
                                            }
                                        }
                                    }
                                    if (buffer.Count > 0)
                                    {
                                        rows.Add(buffer);
                                    }
                                }

                                k = null;

                                for (int i = 1; i < 8; i++)
                                {
                                    buffer = new List<string>();
                                    foreach (string v in g)
                                    {
                                        k = v.Split(',');
                                        if (k.Length >= 2)
                                        {
                                            if (k[1] == i.ToString())
                                            {
                                                buffer.Add(v);
                                            }
                                        }
                                    }
                                    if (buffer.Count > 0)
                                    {
                                        columns.Add(buffer);
                                    }
                                }

                                string[] u = null;
                                int w = -1;
                                int max = 0;
                                int _y = 0;
                                g = null;

                                //Get the largest row
                                for(int i = 0; i < rows.Count; i++)
                                {
                                    _y = 0;
                                    foreach (string f in rows[i])
                                    {
                                        u = f.Split(',');                                        
                                        if(u.Length >= 2)
                                        {
                                            w = stringToInt((u[0] + u[1] + u[2]));
                                            var z = (from v in cotizaciones.modulos_cotizaciones where v.merge_id == concepto.concept_id && v.dir == w select v).FirstOrDefault();
                                            if(z != null)
                                            {
                                                if (z.cantidad > 1)
                                                {
                                                    if (u[2] == "1")
                                                    {
                                                        _y = _y + (int)(z.largo * z.cantidad);
                                                    }
                                                    else
                                                    {
                                                        _y = _y + (int)z.largo;
                                                    }
                                                }
                                                else
                                                {
                                                    _y = _y + (int)z.largo;
                                                }
                                            }
                                        }
                                    }
                                    if(max < _y)
                                    {
                                        max = _y;
                                        g = rows[i];
                                    }
                                }                       

                                foreach (string p in g)
                                {
                                    u = p.Split(',');
                                    if (u.Length >= 2)
                                    {
                                        w = stringToInt((u[0] + u[1] + u[2]));
                                        var z = (from v in cotizaciones.modulos_cotizaciones where v.merge_id == concepto.concept_id && v.dir == w select v).FirstOrDefault();
                                        if (z != null)
                                        {
                                            if (z.cantidad > 1)
                                            {
                                                if (u[2] == "1")
                                                {
                                                    concepto.largo = concepto.largo + (z.largo * z.cantidad);                                                  
                                                }
                                                else
                                                {
                                                    concepto.largo = concepto.largo + z.largo;                                                   
                                                }
                                            }
                                            else
                                            {
                                                concepto.largo = concepto.largo + z.largo;                                               
                                            }
                                        }
                                    }
                                }

                                g = null;
                                max = 0;
                                //Get the hightest column
                                for (int i = 0; i < columns.Count; i++)
                                {
                                    _y = 0;
                                    foreach (string f in columns[i])
                                    {
                                        u = f.Split(',');
                                        if (u.Length >= 2)
                                        {
                                            w = stringToInt((u[0] + u[1] + u[2]));
                                            var z = (from v in cotizaciones.modulos_cotizaciones where v.merge_id == concepto.concept_id && v.dir == w select v).FirstOrDefault();
                                            if (z != null)
                                            {
                                                if (z.cantidad > 1)
                                                {
                                                    if (u[2] == "0")
                                                    {
                                                        _y = _y + (int)(z.alto * z.cantidad);
                                                    }
                                                    else
                                                    {
                                                        _y = _y + (int)z.alto;
                                                    }
                                                }
                                                else
                                                {
                                                    _y = _y + (int)z.alto;
                                                }
                                            }
                                        }
                                    }
                                    if (max < _y)
                                    {
                                        max = _y;
                                        g = columns[i];
                                    }
                                }

                                foreach (string p in g)
                                {
                                    u = p.Split(',');
                                    if (u.Length >= 2)
                                    {
                                        w = stringToInt((u[0] + u[1] + u[2]));
                                        var z = (from v in cotizaciones.modulos_cotizaciones where v.merge_id == concepto.concept_id && v.dir == w select v).FirstOrDefault();
                                        if (z != null)
                                        {
                                            if (z.cantidad > 1)
                                            {
                                                if (u[2] == "0")
                                                {
                                                    concepto.alto = concepto.alto + (z.alto * z.cantidad);                                                  
                                                }
                                                else
                                                {
                                                    concepto.alto = concepto.alto + z.alto;                                                  
                                                }
                                            }
                                            else
                                            {
                                                concepto.alto = concepto.alto + z.alto;                                               
                                            }
                                        }
                                    }
                                }
                             
                                //clear
                                buffer = null;
                                rows = null;
                                columns = null;
                                g = null;
                            }
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            errorLog(err.ToString());
                        }
                    }
                    //----------------------------------------------------------------------------------------------------------->
                }
            }
            cotizaciones.SaveChanges();
        }
        //

       public static float getLargoUnificado(int dir, float concept_largo, float modulo_largo, float cantidad, int dir_p)
        {
            float r = 0;
            if(dir == 2)
            {
                r = concept_largo + (modulo_largo * cantidad);                           
            }
            else if(dir == 1)
            {
                if(concept_largo > 0)
                {
                    if(modulo_largo > concept_largo)
                    {
                        if (dir != dir_p)
                        {
                            r = modulo_largo + concept_largo;
                        }
                        else
                        {
                            r = modulo_largo;
                        }
                    }
                    else
                    {
                        r = concept_largo;
                    }
                }
                else
                {
                    r = modulo_largo;
                }
            }
            return r;
        }

        public static float getAltoUnificado(int dir, float concept_alto, float modulo_alto, float cantidad, int dir_p)
        {
            float r = 0;
            if (dir == 1)
            {
                r = concept_alto + (modulo_alto * cantidad);                             
            }
            else if(dir == 2)
            {
                if (concept_alto > 0)
                {
                    if(modulo_alto > concept_alto)
                    {
                        if (dir != dir_p)
                        {
                            r = modulo_alto + concept_alto;
                        }
                        else
                        {
                            r = modulo_alto;
                        }
                    }
                    else
                    {
                        r = concept_alto;
                    }
                }
                else
                {
                    r = modulo_alto;
                }
            }
            return r;
        }

        //agregar a propiedades folio y nombre de cliente
        public static void setClienteToPropiedades(int folio=-1, string name="", string proyecto="", float desc=0, float utilidad=0, bool desglose_iva=true)
        {
            localDateBaseEntities3 propiedades = new localDateBaseEntities3();
            try {
                var k = (from x in propiedades.propiedades_local where x.id == 1 select x).SingleOrDefault();

                if (k != null)
                {
                    k.folio_abierto = folio;
                    k.nombre_cliente_abierto = name;
                    k.proyecto_abierto = proyecto;
                    k.desc_cotizacion = desc;
                    k.utilidad_cotizacion = utilidad;
                    k.desglose_iva = desglose_iva;
                }
                propiedades.SaveChanges();
            }
            catch (Exception e)
            {
                errorLog(e.ToString());
            }
        }

        public static void setFolioStart()
        {
            localDateBaseEntities3 propiedades = new localDateBaseEntities3();
            try {
                var k = (from x in propiedades.propiedades_local where x.id == 1 select x).SingleOrDefault();

                if (k != null)
                {
                    folio_abierto = (int)k.folio_abierto;
                    nombre_cotizacion = k.nombre_cliente_abierto.ToString();
                    nombre_proyecto = k.proyecto_abierto.ToString();
                    desc_cotizacion = (float)k.desc_cotizacion;
                    utilidad_cotizacion = (float)k.utilidad_cotizacion;
                    iva_desglosado = k.desglose_iva != null ? (bool)k.desglose_iva : iva_desglosado;
                }
            }catch(Exception e)
            {
                errorLog(e.ToString());
            }
        }
        //   

        //agregar a propiedades % de tipo de articulo
        public static void setPercentageToPropiedades(float cristales, float aluminio, float herrajes, float otros)
        {
            localDateBaseEntities3 propiedades = new localDateBaseEntities3();
            try {
                var k = (from x in propiedades.propiedades_local where x.id == 1 select x).SingleOrDefault();

                if (k != null)
                {
                    k.p_cristales = cristales;
                    k.p_aluminio = aluminio;
                    k.p_herrajes = herrajes;
                    k.p_otros = otros;
                };
                propiedades.SaveChanges();
                desc_cristales = cristales;
                desc_aluminio = aluminio;
                desc_herrajes = herrajes;
                desc_otros = otros;
            }
            catch (Exception e)
            {
                errorLog(e.ToString());
            }
        }

        public static void setPercentageOfArticulos(TextBox textbox, TextBox textbox2, TextBox textbox3, TextBox textbox4)
        {
            localDateBaseEntities3 propiedades = new localDateBaseEntities3();
            try {
                var k = (from x in propiedades.propiedades_local where x.id == 1 select x).SingleOrDefault();

                if (k != null)
                {
                    textbox.Text = k.p_cristales.ToString();
                    textbox2.Text = k.p_aluminio.ToString();
                    textbox3.Text = k.p_herrajes.ToString();
                    textbox4.Text = k.p_otros.ToString();
                };
                propiedades.SaveChanges();              
            }
            catch (Exception e)
            {
                errorLog(e.ToString());
            }
        }
        // 

        public static void loadPercentageOfArticulos()
        {
            localDateBaseEntities3 propiedades = new localDateBaseEntities3();
            try
            {
                var k = (from x in propiedades.propiedades_local where x.id == 1 select x).SingleOrDefault();

                if (k != null)
                {
                    desc_cristales = (float)k.p_cristales;
                    desc_aluminio = (float)k.p_aluminio;
                    desc_herrajes = (float)k.p_herrajes;
                    desc_otros = (float)k.p_otros;
                };
                propiedades.SaveChanges();
            }
            catch (Exception e)
            {
                errorLog(e.ToString());
            }
        }
        // 

        //filas borradas
        public static void setFilaBorradaOnLocalDB(int type, int folio, int id)
        {
            localDateBaseEntities3 delete = new localDateBaseEntities3();
            try {
                filas_borradas filas = new filas_borradas()
                {
                    type = type,
                    folio = folio,
                    articulo_id = id
                };
                delete.filas_borradas.Add(filas);
                delete.SaveChanges();
            }catch(Exception e)
            {
                errorLog(e.ToString());
            }
        }

        public static void deleteFilasBorradasFromLocalDB()
        {
            localDateBaseEntities3 delete = new localDateBaseEntities3();
            try
            {
                delete.Database.ExecuteSqlCommand("TRUNCATE TABLE filas_borradas");
               
                //autoincrement reseed -->
                delete.Database.ExecuteSqlCommand("DBCC CHECKIDENT (filas_borradas, RESEED, 1)");              
            }
            catch (Exception err)
            {
                errorLog(err.ToString());
            }
        }
        //

        //Reload precios de Cotizacion -------------------------------------------------------------------------------->

        //cristales --->
        public static float getNuevoCalculoCristalesCostos(string clave, float cantidad, float largo, float alto, float descuento, Boolean instalado, float filo_muerto)
        {
            float r = 0;
            listas_entities_pva lista = new listas_entities_pva();
            var h = (from x in lista.lista_costo_corte_e_instalado where x.clave == clave select x).SingleOrDefault();
            float costo = 0;
            float insta = 0;

            if (h != null)
            {
                costo = (float)h.costo_corte_m2;
                insta = (float)h.costo_instalado;
            }

            if(instalado == true)
            {
                r = ((insta * largo * alto * filo_muerto) * cantidad) + (((insta * largo * alto * filo_muerto) * cantidad) * descuento);
            }
            else
            {
                r = ((costo * largo * alto * filo_muerto) * cantidad) + (((costo * largo * alto * filo_muerto) * cantidad) * descuento);
            }
            return r;
        }

        public static float getNuevoCalculoCristalesPrecio(string clave, float cantidad, float largo, float alto, float descuento, Boolean instalado, float filo_muerto)
        {
            float r = 0;
            listas_entities_pva lista = new listas_entities_pva();
            var h = (from x in lista.lista_precio_corte_e_instalado where x.clave == clave select x).SingleOrDefault();
            float costo = 0;
            float insta = 0;

            if (h != null)
            {
                costo = (float)h.precio_venta_corte_m2;
                insta = (float)h.precio_venta_instalado;
            }

            if (instalado == true)
            {
                r = ((insta * largo * alto * filo_muerto) * cantidad) + (((insta * largo * alto * filo_muerto) * cantidad) * descuento);
            }
            else
            {
                r = ((costo * largo * alto * filo_muerto) * cantidad) + (((costo * largo * alto * filo_muerto) * cantidad) * descuento);
            }
            return r;
        }


        public static float getNuevoCalculoCristalesHojas(string clave, float cantidad, float descuento, float filo_muerto)
        {
            float r = 0;
            listas_entities_pva lista = new listas_entities_pva();
            var h = (from x in lista.lista_precios_hojas where x.clave == clave select x).SingleOrDefault();
            float precio = 0;          

            if (h != null)
            {
                precio = (float)h.precio_hoja;               
            }
           
            r = (precio * filo_muerto * cantidad) + ((precio * filo_muerto * cantidad) * descuento);
            return r;
        }

        public static void reloadCristales() 
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            float acabados = 0;
            var c_cristales = (from x in cotizaciones.cristales_cotizados select x);

            foreach(var cristales in c_cristales)
            {
                if(cristales != null)
                {
                    acabados = 0;
                    if (cristales.canteado != "")
                    {
                        acabados = acabados + calcularAcabado(cristales.canteado, (float)cristales.largo, (float)cristales.alto);
                    }

                    if (cristales.biselado != "")
                    {
                        acabados = acabados + calcularAcabado(cristales.biselado, (float)cristales.largo, (float)cristales.alto);
                    }

                    if (cristales.desconchado != "")
                    {
                        acabados = acabados + calcularAcabado(cristales.desconchado, (float)cristales.largo, (float)cristales.alto);
                    }

                    if (cristales.pecho_paloma != "")
                    {
                        acabados = acabados + calcularAcabado(cristales.pecho_paloma, (float)cristales.largo, (float)cristales.alto);
                    }

                    if (cristales.perforado_media_pulgada != "")
                    {
                        acabados = acabados + calcularAcabado(cristales.perforado_media_pulgada, (float)cristales.largo, (float)cristales.alto);
                    }

                    if (cristales.perforado_una_pulgada != "")
                    {
                        acabados = acabados + calcularAcabado(cristales.perforado_una_pulgada, (float)cristales.largo, (float)cristales.alto);
                    }

                    if (cristales.perforado_dos_pulgadas != "")
                    {
                        acabados = acabados + calcularAcabado(cristales.perforado_dos_pulgadas, (float)cristales.largo, (float)cristales.alto);
                    }

                    if (cristales.grabado != "")
                    {
                        acabados = acabados + calcularAcabado(cristales.grabado, (float)cristales.largo, (float)cristales.alto);
                    }

                    if (cristales.esmerilado != "")
                    {
                        acabados = acabados + calcularAcabado(cristales.esmerilado, (float)cristales.largo, (float)cristales.alto);
                    }

                    if (cristales.lista == "Cristal Costo Corte")
                    {                       
                        if (cristales.tipo_venta == "Metro Cuadrado")
                        {                          
                           cristales.total = Math.Round(acabados + getNuevoCalculoCristalesCostos(getClave(cristales.clave), (float)cristales.cantidad, (float)(cristales.largo / 1000), (float)(cristales.alto / 1000), (float)(cristales.descuento / 100), false, (float)(cristales.filo_muerto > 0 ? (cristales.filo_muerto / 100) + 1 : 1)), 2);
                        }
                        else
                        {
                            cristales.total = Math.Round(acabados + getNuevoCalculoCristalesCostos(getClave(cristales.clave), (float)cristales.cantidad, (float)(cristales.largo / 1000), (float)(cristales.alto / 1000), (float)(cristales.descuento / 100), true, (float)(cristales.filo_muerto > 0 ? (cristales.filo_muerto / 100) + 1 : 1)), 2);
                        }
                    }
                    else if(cristales.lista == "Cristal Precio Venta Corte")
                    {
                        if (cristales.tipo_venta == "Metro Cuadrado")
                        {
                            cristales.total = Math.Round(acabados + getNuevoCalculoCristalesPrecio(getClave(cristales.clave), (float)cristales.cantidad, (float)(cristales.largo / 1000), (float)(cristales.alto / 1000), (float)(cristales.descuento / 100), false, (float)(cristales.filo_muerto > 0 ? (cristales.filo_muerto / 100) + 1 : 1)), 2);
                        }
                        else
                        {
                            cristales.total = Math.Round(acabados + getNuevoCalculoCristalesPrecio(getClave(cristales.clave), (float)cristales.cantidad, (float)(cristales.largo / 1000), (float)(cristales.alto / 1000), (float)(cristales.descuento / 100), true, (float)(cristales.filo_muerto > 0 ? (cristales.filo_muerto / 100) + 1 : 1)), 2);
                        }
                    }
                    else if(cristales.lista == "Cristal Precio Hojas")
                    {
                        cristales.total = Math.Round(acabados + getNuevoCalculoCristalesHojas(getClave(cristales.clave), (float)cristales.cantidad, (float)(cristales.descuento / 100), (float)(cristales.filo_muerto > 0 ? (cristales.filo_muerto / 100) + 1 : 1)), 2);
                    }
                }
            }
            cotizaciones.SaveChanges();
            cotizaciones.Dispose();
        }

        public static float calcularAcabado(string a, float largo_t, float alto_t)
        {
            float r = 0;      
            string clave = string.Empty;
            string tipo = string.Empty;
            float largo = 0;
            float alto = 0;
            int cantidad = 0;
            string[] acabados = a.Split(',');

            listas_entities_pva listas = new listas_entities_pva();

            if (acabados.Length == 5)
            {
                clave = acabados[0];
                tipo = acabados[1];
                largo = stringToFloat(acabados[2]);
                alto = stringToFloat(acabados[3]);
                cantidad = stringToInt(acabados[4]);

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
                            r = (float)acabado.neto_recto * ((largo / 1000) * 2) * ((alto_t / 1000) * 2);
                        }
                        else if (largo == 0 && alto > 0)
                        {
                            r = (float)acabado.neto_recto * (((largo_t / 1000) * 2) * (alto / 1000) * 2);
                        }
                        else if (largo == 0 && alto == 0)
                        {
                            r = (float)acabado.neto_recto * ((largo_t / 1000) * 2) * ((alto_t / 1000) * 2);
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
                            r = (float)acabado.neto_curvo * ((largo / 1000) * 2) * ((alto_t / 1000) * 2);
                        }
                        else if (largo == 0 && alto > 0)
                        {
                            r = (float)acabado.neto_curvo * (((largo_t / 1000) * 2) * (alto / 1000) * 2);
                        }
                        else if (largo == 0 && alto == 0)
                        {
                            r = (float)acabado.neto_curvo * ((largo_t / 1000) * 2) * ((alto_t / 1000) * 2);
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
                                r = (float)acabado.neto_recto * (largo / 1000) * (alto_t / 1000);
                            }
                            else if (largo == 0 && alto > 0)
                            {
                                r = (float)acabado.neto_recto * (largo_t / 1000) * (alto / 1000);
                            }
                            else if (largo == 0 && alto == 0)
                            {
                                r = (float)acabado.neto_recto * (largo_t / 1000) * (alto_t / 1000);
                            }
                            else
                            {
                                r = 0;
                            }
                        }
                    }
                }
            }            
            return r;
        }
        //

        //aluminio ---->
        public static void reloadAluminio()
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            var t_aluminios = (from x in cotizaciones.aluminio_cotizado select x);

            foreach(var aluminio in t_aluminios)
            {
                if (aluminio != null)
                {
                    aluminio.total = Math.Round(getNuevoCalculoAluminio(constants.getOriginalIDFromClave(aluminio.clave), (float)(aluminio.largo_total / 1000), (float)aluminio.cantidad, (float)(aluminio.descuento / 100), aluminio.acabado), 2);                  
                }
            }
            cotizaciones.SaveChanges();
            cotizaciones.Dispose();
        }

        public static float getNuevoCalculoAluminio(int id, float largo, float cantidad, float descuento, string acabado)
        {
            listas_entities_pva listas = new listas_entities_pva();

            var aluminio = (from x in listas.perfiles where x.id == id select x).SingleOrDefault();
            float r = 0;
            float ext = 0;
            float largo_original = 1;
            if (aluminio != null)
            {
                try {
                    string cl = acabado;
                    var color = (from v in listas.colores_aluminio
                                 where v.clave == acabado
                                 select v).SingleOrDefault();
                    if (aluminio.largo > 1)
                    {
                        largo_original = (float)aluminio.largo;
                    }

                    if (color != null)
                    {
                        ext = (float)(largo * color.costo_extra_ml);
                        r = (float)(((((aluminio.crudo) / largo_original) * largo * cantidad) + (((largo * (aluminio.perimetro_dm2_ml/100) * (color.precio)) + ext) * cantidad)) + (float)(((((aluminio.crudo) / largo_original) * largo * cantidad) + (largo * (aluminio.perimetro_dm2_ml/100) * (color.precio) * cantidad)) * descuento));
                    }
                    else
                    {
                        switch (acabado)
                        {
                            case "crudo":
                                r = (((float)((aluminio.crudo) / largo_original) * largo) * cantidad) + ((((float)((aluminio.crudo) / largo_original) * largo) * cantidad) * descuento);
                                break;
                            case "blanco":
                                r = (((float)((aluminio.blanco) / largo_original) * largo) * cantidad) + ((((float)((aluminio.blanco) / largo_original) * largo) * cantidad) * descuento);
                                break;
                            case "hueso":
                                r = (((float)((aluminio.hueso) / largo_original) * largo) * cantidad) + ((((float)((aluminio.hueso) / largo_original) * largo) * cantidad) * descuento);
                                break;
                            case "champagne":
                                r = (((float)((aluminio.champagne) / largo_original) * largo) * cantidad) + ((((float)((aluminio.champagne) / largo_original) * largo) * cantidad) * descuento);
                                break;
                            case "gris":
                                r = (((float)((aluminio.gris) / largo_original) * largo) * cantidad) + ((((float)((aluminio.gris) / largo_original) * largo) * cantidad) * descuento);
                                break;
                            case "negro":
                                r = (((float)((aluminio.negro) / largo_original) * largo) * cantidad) + ((((float)((aluminio.negro) / largo_original) * largo) * cantidad) * descuento);
                                break;
                            case "brillante":
                                r = (((float)((aluminio.brillante) / largo_original) * largo) * cantidad) + ((((float)((aluminio.brillante) / largo_original) * largo) * cantidad) * descuento);
                                break;
                            case "natural":
                                r = (((float)((aluminio.natural_1) / largo_original) * largo) * cantidad) + ((((float)((aluminio.natural_1) / largo_original) * largo) * cantidad) * descuento);
                                break;
                            case "madera":
                                r = (((float)((aluminio.madera) / largo_original) * largo) * cantidad) + ((((float)((aluminio.madera) / largo_original) * largo) * cantidad) * descuento);
                                break;
                            case "chocolate":
                                r = (((float)((aluminio.chocolate) / largo_original) * largo) * cantidad) + ((((float)((aluminio.chocolate) / largo_original) * largo) * cantidad) * descuento);
                                break;
                            case "acero_inox":
                                r = (((float)((aluminio.acero_inox) / largo_original) * largo) * cantidad) + ((((float)((aluminio.acero_inox) / largo_original) * largo) * cantidad) * descuento);
                                break;
                            case "bronce":
                                r = (((float)((aluminio.bronce) / largo_original) * largo) * cantidad) + ((((float)((aluminio.bronce) / largo_original) * largo) * cantidad) * descuento);
                                break;
                            default: break;
                        }
                    }
                }
                catch (Exception)
                {
                    r = 0;
                }
            }
            return r;
        }

        //herrajes ------>
        public static void reloadHerrajes()
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            var t_herrajes = (from x in cotizaciones.herrajes_cotizados select x);

            foreach(var herrajes in t_herrajes)
            {
                if (herrajes != null)
                {
                    herrajes.total = Math.Round(getNuevoCalculoHerrajes(constants.getOriginalIDFromClave(herrajes.clave), (float)herrajes.cantidad, (float)(herrajes.descuento / 100), herrajes.color), 2);
                }
            }
            cotizaciones.SaveChanges();
            cotizaciones.Dispose();
        }

        public static float getNuevoCalculoHerrajes(int id, float cantidad, float descuento, string acabado)
        {
            listas_entities_pva listas = new listas_entities_pva();

            var herrajes = (from x in listas.herrajes where x.id == id select x).SingleOrDefault();
            float r = 0;

            if (herrajes != null)
            {               
                r = ((float)(herrajes.precio) * cantidad) + (((float)(herrajes.precio) * cantidad) * descuento);
            }
            return r;
        }

        //otros ----->
        public static void reloadOtros()
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            var t_otros = (from x in cotizaciones.otros_cotizaciones select x);

            foreach(var otros in t_otros)
            {
                if (otros != null)
                {
                    if (otros.largo != null)
                    {
                        otros.total = Math.Round(getNuevoCalculoOtros(getOriginalIDFromClave(otros.clave), (float)otros.cantidad, (float)(otros.descuento / 100), (float)(otros.largo / 1000), (float)(otros.alto / 1000)), 2);
                    }
                }
            }
            cotizaciones.SaveChanges();
            cotizaciones.Dispose();
        }

        //modulos ----->
        public static void reloadModulos(int s_folio=0, bool reload_c=true)
        {
            if(s_folio == 0)
            {
                s_folio = sub_folio;
            }

            cotizaciones_local cotizaciones = new cotizaciones_local();

            if (reload_c)
            {
                var t_modulos = (from x in cotizaciones.modulos_cotizaciones where x.sub_folio == s_folio select x);

                foreach (var modulo in t_modulos)
                {
                    if (modulo != null)
                    {
                        if (modulo.modulo_id > 0)
                        {
                            modulo.total = Math.Round(reloadCalcularCostoModulo((int)modulo.modulo_id, (float)modulo.mano_obra / 100, (int)modulo.cantidad, modulo.dimensiones, modulo.claves_cristales, (float)modulo.flete / 100, (float)modulo.desperdicio / 100, (float)modulo.utilidad / 100, modulo.claves_otros, modulo.claves_herrajes, modulo.claves_perfiles, modulo.news, modulo.acabado_perfil, modulo.id), 2);
                        }
                    }
                }

                cotizaciones.SaveChanges();
            }

            var n_conceptos = (from x in cotizaciones.modulos_cotizaciones where x.concept_id > 0 select x);

            foreach (var b in n_conceptos)
            {
                if (b != null)
                {
                    updateModuloPersonalizado((int)b.concept_id);
                }
            }

            cotizaciones.Dispose();
        }

        public static float getNuevoCalculoOtros(int id, float cantidad, float descuento, float largo, float alto)
        {
            listas_entities_pva listas = new listas_entities_pva();

            var otros = (from x in listas.otros where x.id == id select x).SingleOrDefault();
            float r = 0;

            if (otros != null)
            {
                if(otros.largo > 0 && otros.alto <= 0)
                {
                    r = (float)((otros.precio) * cantidad * largo) + (float)(((otros.precio) * cantidad * largo) * descuento);
                }
                else if(otros.largo <= 0 && otros.alto > 0)
                {
                    r = (float)((otros.precio) * cantidad * alto) + (float)(((otros.precio) * cantidad * alto) * descuento);
                }
                else if(otros.largo > 0 && otros.alto > 0)
                {
                    r = (float)((otros.precio) * largo * alto * cantidad) + (float)(((otros.precio) * largo * alto * cantidad) * descuento);                   
                }
                else
                {
                    r = ((float)(otros.precio) * cantidad) + (((float)(otros.precio) * cantidad) * descuento);
                }
            }
            return r;
        }

        public static void reloadPreciosCotizaciones(int s_folio=0, bool reload_p=true)
        {
            if (reload_p)
            {
                reloadCristales();
                reloadAluminio();
                reloadHerrajes();
                reloadOtros();
            }
            reloadModulos(s_folio, reload_p);
        }

        public static string getStringToPoint(string texto)
        {
            string[] g = texto.Split('-');
            string r = string.Empty; 
            
            if(g.Length > 0)
            {
                r = g[0];
            }          

            return r;
        }

        public static int getEspesorCristal(string text)
        {
            string k = string.Empty;
            foreach(char x in text)
            {
                if(stringToInt(x.ToString()) > 0)
                {
                    k = k + x.ToString();
                }
            }
            return stringToInt(k);
        }

        public static void downloadPropiedadesModel()
        {
            localDateBaseEntities3 p = new localDateBaseEntities3();
            sqlDateBaseManager sql = new sqlDateBaseManager();

            try
            {
                var propiedades = (from x in p.propiedades where x.id == 1 select x).SingleOrDefault();

                if (propiedades != null)
                {
                    float iva = stringToFloat(sql.getSingleSQLValue("propiedades", "iva", "id", "1", 0));
                    if (iva > 0)
                    {
                        propiedades.iva = iva;
                    }
                }
                p.SaveChanges();
            }
            catch (Exception err)
            {
                errorLog(err.ToString());
                MessageBox.Show("[Error]: error al cargar las propiedades de la aplicación.", msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void setPropiedadesModel(float iva)
        {
            localDateBaseEntities3 p = new localDateBaseEntities3();
            try
            {
                var propiedades = (from x in p.propiedades where x.id == 1 select x).SingleOrDefault();

                if (propiedades != null)
                {
                    propiedades.iva = iva;
                }
                p.SaveChanges();
            }
            catch (Exception err)
            {
                errorLog(err.ToString());
            }
        }

        public static float getPropiedadesModel()
        {
            float r = 1.16f;
            localDateBaseEntities3 p = new localDateBaseEntities3();
            try
            {
                var propiedades = (from x in p.propiedades where x.id == 1 select x).SingleOrDefault();

                if (propiedades != null)
                {
                    if ((float)propiedades.iva > 0)
                    {
                        r = (float)propiedades.iva;
                    }
                }
            }
            catch (Exception err)
            {
                errorLog(err.ToString());
            }
            return r;
        }

        public static void loadPropiedadesModel()
        {
            localDateBaseEntities3 p = new localDateBaseEntities3();
            try
            {
                var propiedades = (from x in p.propiedades where x.id == 1 select x).SingleOrDefault();

                if (propiedades != null)
                {
                    if ((float)propiedades.iva > 0)
                    {
                        iva = (float)propiedades.iva;
                    }
                }
            }
            catch (Exception err)
            {
                errorLog(err.ToString());
            }
        }

        public static float leerModuloAluminio(listas_entities_pva listas, string clave_modulo, string n_id, int[,] dim, string acabado)
        {            
            string clave = string.Empty;
            float count = 0;
            string buffer = string.Empty;
            string dir = string.Empty;
            int seccion = -1;
            float total = 0;
            float ext = 0;
            string[] z;

            foreach (char alm in clave_modulo)
            {
                if (alm == ':')
                {
                    clave = buffer;
                    buffer = string.Empty;
                    continue;
                }
                if (alm == '-')
                {
                    count = stringToFloat(buffer);
                    buffer = string.Empty;
                    continue;
                }
                if (alm == '$')
                {
                    dir = buffer;
                    buffer = string.Empty;
                    continue;
                }
                buffer = buffer + alm.ToString();
            }
            seccion = stringToInt(buffer);
            z = n_id.Split('-');
            clave = z[0];
            //Cantidad
            if (z.Length > 1)
            {
                count = stringToFloat(z[1]);
                //Acabado
                if (z.Length == 3)
                {
                    acabado = z[2];
                }
            }
            
            var perfil = (from c in listas.perfiles where c.clave == clave select c).SingleOrDefault();

            if (perfil != null)
            {
                float largo = dim[seccion, 0] / 1000f;
                float alto = dim[seccion, 1] / 1000f;
                var color = (from u in listas.colores_aluminio where u.clave == acabado select u).SingleOrDefault();

                if (color == null)
                {
                    if (acabado == "crudo")
                    {
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.crudo / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.crudo / perfil.largo) * alto) * count);
                        }
                    }
                    if (acabado == "blanco")
                    {
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.blanco / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.blanco / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "hueso")
                    {
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.hueso / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.hueso / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "champagne")
                    {
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.champagne / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.champagne / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "gris")
                    {
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.gris / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.gris / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "negro")
                    {
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.negro / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.negro / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "brillante")
                    {
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.brillante / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.brillante / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "natural")
                    {
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.natural_1 / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.natural_1 / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "madera")
                    {
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.madera / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.madera / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "chocolate")
                    {
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.chocolate / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.chocolate / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "acero_inox")
                    {
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.acero_inox / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.acero_inox / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "bronce")
                    {
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.bronce / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.bronce / perfil.largo) * alto) * count);
                        }
                    }
                }
                else
                {                 
                    if (dir == "largo")
                    {
                        ext = (float)(largo * color.costo_extra_ml);
                        total = (float)((((perfil.crudo / perfil.largo) * largo) * count) + ((((largo * (perfil.perimetro_dm2_ml/100)) * (color.precio)) + ext)) * count);
                    }
                    else if (dir == "alto")
                    {
                        ext = (float)(alto * color.costo_extra_ml);
                        total = (float)((((perfil.crudo / perfil.largo) * alto) * count) + ((((alto * (perfil.perimetro_dm2_ml/100)) * (color.precio)) + ext)) * count);
                    }                                       
                }
            }
            else
            {
                total = -1;
            }                                      
            return total;
        }

        public static float leerModuloCristales(listas_entities_pva listas, string clave_modulo, string n_id, int[,] dim, bool cs, int rows, int columns)
        {           
            string clave = "";
            float count = 0;
            int seccion = -1;
            string buffer = "";
            float total = 0;
            string[] z;

            foreach (char x in clave_modulo)
            {
                if (x == ':')
                {
                    clave = buffer;
                    buffer = "";
                    continue;
                }
                if (x == '$')
                {
                    count = stringToFloat(buffer);
                    buffer = "";
                    continue;
                }
                buffer = buffer + x.ToString();
            }
            seccion = stringToInt(buffer);
            z = n_id.Split('-');
            if (z.Length > 0)
            {
                clave = z[0];
                if (z.Length > 1)
                {
                    count = stringToFloat(z[1]);
                }
            }
            else
            {
                clave = n_id;
            }

            var cristal = (from v in listas.lista_costo_corte_e_instalado where v.clave == clave select v).SingleOrDefault();

            if (cristal != null)
            {
                if(cs == false)
                {
                    if (rows > 0 && columns > 0)
                    {
                        total = (float)((cristal.costo_corte_m2 * ((dim[seccion, 0] / 1000f) / columns) * ((dim[seccion, 1] / 1000f)) / rows) * count);
                    }
                }
                else
                {
                    total = (float)((cristal.costo_corte_m2 * (dim[seccion, 0] / 1000f) * (dim[seccion, 1] / 1000f)) * count);
                }
            }
            else
            {
                total = -1;
            }
            return total;
        }

        public static float leerModuloHerrajes(listas_entities_pva listas, string module, string n_id)
        {
            string clave = string.Empty;
            float count = 0;
            string buffer = "";
            float total = 0;
            int seccion = -1;
            string[] z;
            foreach (char x in module)
            {
                if (x == ':')
                {
                    clave = buffer;
                    buffer = "";
                    continue;
                }
                if (x == '$')
                {
                    count = stringToFloat(buffer);
                    buffer = "";
                    continue;
                }
                buffer = buffer + x.ToString();
            }
            seccion = stringToInt(buffer);
            z = n_id.Split('-');
            clave = z[0];
            if (z.Length > 1)
            {
                count = stringToFloat(z[1]);
            }
            var herraje = (from v in listas.herrajes where v.clave == clave select v).SingleOrDefault();

            if (herraje != null)
            {              
                total = total + (float)(herraje.precio * count);
            }
            else
            {
                total = -1;
            }
            return total;
        }

        public static float leerModuloOtros(listas_entities_pva listas, string module, string n_id, int[,] dim, bool cs, int rows, int columns)
        {
            string clave = string.Empty;
            float count = 0;
            string buffer = "";
            string dir = "";
            int seccion = -1;
            float total = 0;
            string[] z;

            foreach (char x in module)
            {
                if (x == ':')
                {
                    clave = buffer;
                    buffer = "";
                    continue;
                }
                if (x == '-')
                {
                    count = stringToFloat(buffer);
                    buffer = "";
                    continue;
                }
                if (x == '$')
                {
                    dir = buffer;
                    buffer = "";
                    continue;
                }
                buffer = buffer + x.ToString();
            }
            seccion = stringToInt(buffer);
            z = n_id.Split('-');
            clave = z[0];
            if (z.Length > 1)
            {
                count = stringToFloat(z[1]);
            }
            var otro = (from v in listas.otros where v.clave == clave select v).SingleOrDefault();

            if (otro != null)
            {
                float largo = dim[seccion, 0] / 1000f;
                float alto = dim[seccion, 1] / 1000f;
                if (otro.largo > 0 && otro.alto <= 0)
                {
                    if (dir == "largo")
                    {
                        total = total + (float)(otro.precio * largo * count);
                    }
                    else if (dir == "alto")
                    {
                        total = total + (float)(otro.precio * alto * count);
                    }
                }
                else if(otro.largo <= 0 && otro.alto > 0)
                {
                    if (dir == "largo")
                    {
                        total = total + (float)(otro.precio * largo * count);
                    }
                    else if (dir == "alto")
                    {
                        total = total + (float)(otro.precio * alto * count);
                    }
                }
                else if (otro.largo > 0 && otro.alto > 0)
                {
                    if(cs == false)
                    {
                        if (rows > 0 && columns > 0)
                        {
                            total = total + (float)(otro.precio * (largo / columns) * (alto / rows) * count);                           
                        }
                    }
                    else
                    {
                        total = total + (float)(otro.precio * largo * alto * count);                       
                    }
                }
                else
                {
                    total = total + (float)(otro.precio * count);
                }
            }
            else
            {
                total = -1;
            }
            return total;
        }

        public static float leerNewModuloAluminio(listas_entities_pva listas, string clave, int count, string dir, int seccion, string acabado, int[,] dim)
        {
            float costo = 0;
            float ext = 0;
            var perfil = (from c in listas.perfiles where c.clave == clave select c).SingleOrDefault();

            if (perfil != null)
            {
                float largo = dim[seccion, 0] / 1000f;
                float alto = dim[seccion, 1] / 1000f;
                var color = (from u in listas.colores_aluminio where u.clave == acabado select u).SingleOrDefault();

                if (color == null)
                {
                    if (acabado == "crudo")
                    {
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.crudo / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.crudo / perfil.largo) * alto) * count);
                        }
                    }
                    if (acabado == "blanco")
                    {
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.blanco / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.blanco / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "hueso")
                    {
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.hueso / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.hueso / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "champagne")
                    {
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.champagne / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.champagne / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "gris")
                    {
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.gris / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.gris / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "negro")
                    {
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.negro / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.negro / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "brillante")
                    {
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.brillante / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.brillante / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "natural")
                    {
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.natural_1 / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.natural_1 / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "madera")
                    {
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.madera / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.madera / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "chocolate")
                    {
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.chocolate / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.chocolate / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "acero_inox")
                    {
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.acero_inox / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.acero_inox / perfil.largo) * alto) * count);
                        }
                    }
                    else if (acabado == "bronce")
                    {
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.bronce / perfil.largo) * largo) * count);
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.bronce / perfil.largo) * alto) * count);
                        }
                    }
                }
                else
                {
                    if (dir == "largo")
                    {
                        ext = (float)(largo * color.costo_extra_ml);
                        costo = (float)((((perfil.crudo / perfil.largo) * largo) * count) + ((((largo * (perfil.perimetro_dm2_ml / 100)) * (color.precio)) + ext)) * count);
                    }
                    else if (dir == "alto")
                    {
                        ext = (float)(alto * color.costo_extra_ml);
                        costo = (float)((((perfil.crudo / perfil.largo) * alto) * count) + ((((alto * (perfil.perimetro_dm2_ml / 100)) * (color.precio)) + ext)) * count);
                    }
                }
            }
            else
            {
                costo = -1;
            }
            return costo;
        }

        public static float leerNewModuloCristales(listas_entities_pva listas, string clave, int count, int seccion, int[,] dim, bool cs, int rows, int columns)
        {           
            float total = 0;           

            var cristal = (from v in listas.lista_costo_corte_e_instalado where v.clave == clave select v).SingleOrDefault();

            if (cristal != null)
            {
                if (cs == false)
                {
                    if (rows > 0 && columns > 0)
                    {
                        total = (float)((cristal.costo_corte_m2 * ((dim[seccion, 0] / 1000f) / columns) * ((dim[seccion, 1] / 1000f)) / rows) * count);
                    }
                }
                else
                {
                    total = (float)((cristal.costo_corte_m2 * (dim[seccion, 0] / 1000f) * (dim[seccion, 1] / 1000f)) * count);
                }
            }
            else
            {
                total = -1;
            }
            return total;
        }

        public static float leerNewModuloHerrajes(listas_entities_pva listas, string clave, int count)
        {           
            float total = 0;
          
            var herraje = (from v in listas.herrajes where v.clave == clave select v).SingleOrDefault();

            if (herraje != null)
            {
                total = total + (float)(herraje.precio * count);
            }
            else
            {
                total = -1;
            }
            return total;
        }

        public static float leerNewModuloOtros(listas_entities_pva listas, string clave, int count, string dir, int seccion, int[,] dim, bool cs, int rows, int columns)
        {          
            float total = 0;
            
            var otro = (from v in listas.otros where v.clave == clave select v).SingleOrDefault();

            if (otro != null)
            {
                float largo = dim[seccion, 0] / 1000f;
                float alto = dim[seccion, 1] / 1000f;
                if (otro.largo > 0 && otro.alto <= 0)
                {
                    if (dir == "largo")
                    {
                        total = total + (float)(otro.precio * largo * count);
                    }
                    else if (dir == "alto")
                    {
                        total = total + (float)(otro.precio * alto * count);
                    }
                }
                else if (otro.largo <= 0 && otro.alto > 0)
                {
                    if (dir == "largo")
                    {
                        total = total + (float)(otro.precio * largo * count);
                    }
                    else if (dir == "alto")
                    {
                        total = total + (float)(otro.precio * alto * count);
                    }
                }
                else if (otro.largo > 0 && otro.alto > 0)
                {
                    if (cs == false)
                    {
                        if (rows > 0 && columns > 0)
                        {
                            total = total + (float)(otro.precio * (largo / columns) * (alto / rows) * count);
                        }
                    }
                    else
                    {
                        total = total + (float)(otro.precio * largo * alto * count);
                    }
                }
                else
                {
                    total = total + (float)(otro.precio * count);
                }
            }
            else
            {
                total = -1;
            }
            return total;
        }

        public static List<string> getLineasModulo()
        {
            localDateBaseEntities3 local = new localDateBaseEntities3();
            List<string> r = new List<string>();

            var lineas = (from x in local.lineas_modulos where x.id > 0 orderby x.linea_modulo ascending select x.linea_modulo);

            if(lineas != null)
            {
                foreach(string v in lineas)
                {
                    r.Add(v);
                }
            }
            return r;
        }

        public static List<string> getCategorias(string grupo)
        {
            localDateBaseEntities3 local = new localDateBaseEntities3();
            List<string> r = new List<string>();

            var categorias = (from x in local.categorias where x.grupo == grupo orderby x.categoria1 ascending select x.categoria1);

            if (categorias != null)
            {
                foreach (string v in categorias)
                {
                    r.Add(v);
                }
            }
            return r;
        }

        public static List<string> getProveedores(string grupo)
        {
            localDateBaseEntities3 local = new localDateBaseEntities3();
            List<string> r = new List<string>();

            var proveedores = (from x in local.proveedores where x.grupo == grupo orderby x.proveedor ascending select x.proveedor);

            if (proveedores != null)
            {
                foreach (string v in proveedores)
                {
                    r.Add(v);
                }
            }
            return r;
        }

        public static void abrirModulo(int modulo_id, DataGridView datagrid)
        {
            listas_entities_pva listas = new listas_entities_pva();

            int id = modulo_id;
            var modulos = (from x in listas.modulos where x.id == id select x).SingleOrDefault();
            string count = string.Empty;
            string buffer = string.Empty;
            string dir = string.Empty;
            string c_clave = string.Empty;
            string seccion = string.Empty;

            if (modulos != null)
            {
                //aluminio
                foreach (char alm in modulos.id_aluminio)
                {
                    if (alm != ',')
                    {
                        if (alm == ':')
                        {
                            c_clave = buffer;
                            buffer = string.Empty;
                            continue;
                        }
                        if (alm == '-')
                        {
                            count = buffer;
                            buffer = string.Empty;
                            continue;
                        }
                        if (alm == '$')
                        {
                            dir = buffer;
                            buffer = string.Empty;
                            continue;
                        }
                        buffer = buffer + alm.ToString();
                    }
                    else
                    {
                        seccion = buffer;
                        buffer = string.Empty;

                        var aluminio = (from c in listas.perfiles where c.clave == c_clave select c).SingleOrDefault();

                        if (aluminio != null)
                        {
                            datagrid.Rows.Add("Perfil", aluminio.id, c_clave, aluminio.articulo, count, dir, seccion);
                        }
                        else
                        {
                            datagrid.Rows.Add("Perfil", "", c_clave, "", count, dir, seccion);
                        }
                    }
                }
                //

                buffer = string.Empty;
                count = string.Empty;
                seccion = string.Empty;
                c_clave = string.Empty;

                //cristales
                foreach (char cri in modulos.clave_vidrio)
                {
                    if (cri != ',')
                    {
                        if (cri == ':')
                        {
                            c_clave = buffer;
                            buffer = string.Empty;
                            continue;
                        }
                        if (cri == '$')
                        {
                            count = buffer;
                            buffer = string.Empty;
                            continue;
                        }
                        buffer = buffer + cri.ToString();
                    }
                    else
                    {
                        seccion = buffer;
                        buffer = string.Empty;

                        var cristal = (from c in listas.lista_costo_corte_e_instalado where c.clave == c_clave select c).SingleOrDefault();

                        if (cristal != null)
                        {
                            datagrid.Rows.Add("Cristal", "", c_clave, cristal.articulo, count, "", seccion);
                        }
                        else
                        {
                            datagrid.Rows.Add("Cristal", "", c_clave, "", count, "", seccion);
                        }
                    }
                }
                //

                buffer = string.Empty;
                count = string.Empty;
                id = 0;
                seccion = string.Empty;
                c_clave = string.Empty;

                //herrajes
                foreach (char h in modulos.id_herraje)
                {
                    if (h != ',')
                    {
                        if (h == ':')
                        {
                            c_clave = buffer;
                            buffer = string.Empty;
                            continue;
                        }
                        if (h == '$')
                        {
                            count = buffer;
                            buffer = string.Empty;
                            continue;
                        }
                        buffer = buffer + h.ToString();
                    }
                    else
                    {
                        seccion = buffer;
                        buffer = string.Empty;

                        var herraje = (from c in listas.herrajes where c.clave == c_clave select c).SingleOrDefault();

                        if (herraje != null)
                        {
                            datagrid.Rows.Add("Herraje", herraje.id, c_clave, herraje.articulo, count, "", seccion);
                        }
                        else
                        {
                            datagrid.Rows.Add("Herraje", "", c_clave, "", count, "", seccion);
                        }
                    }
                }
                //

                buffer = string.Empty;
                count = string.Empty;
                id = 0;
                dir = string.Empty;
                seccion = string.Empty;
                c_clave = string.Empty;

                //otros
                foreach (char o in modulos.id_otros)
                {
                    if (o != ',')
                    {
                        if (o == ':')
                        {
                            c_clave = buffer;
                            buffer = string.Empty;
                            continue;
                        }
                        if (o == '-')
                        {
                            count = buffer;
                            buffer = string.Empty;
                            continue;
                        }
                        if (o == '$')
                        {
                            dir = buffer;
                            buffer = string.Empty;
                            continue;
                        }
                        buffer = buffer + o.ToString();
                    }
                    else
                    {
                        seccion = buffer;
                        buffer = string.Empty;

                        var otros = (from c in listas.otros where c.clave == c_clave select c).SingleOrDefault();

                        if (otros != null)
                        {
                            datagrid.Rows.Add("Otros", otros.id, c_clave, otros.articulo, count, dir, seccion);
                        }
                        else
                        {
                            datagrid.Rows.Add("Otros", "", c_clave, "", count, dir, seccion);
                        }
                    }
                }
            }
        }

        public static string[] leerEsquema(string esquema)
        {
            string[] r = new string[5];
            string buffer = string.Empty;
            string h = string.Empty, v = string.Empty;

            foreach (char c in esquema)
            {
                if (c == '$')
                {
                    if(r[2] != "VH")
                    {
                        if(r[2] == "V")
                        {
                            r[0] = v;
                        }
                        else if(r[2] == "H")
                        {
                            r[0] = h;
                        }
                    }
                    else
                    {
                        r[0] = "2";
                        r[3] = v;
                        r[4] = h;
                    }
                    buffer = "";
                    continue;
                }
                if (c == '@')
                {
                    r[1] = buffer;
                    break;
                }
                if(c == 'V' || c == 'H')
                {
                    r[2] = r[2] + c.ToString();
                    if(c == 'V')
                    {
                        v = buffer;
                        buffer = "";
                    }
                    else
                    {
                        h = buffer;
                        buffer = "";
                    }
                }
                else
                {
                    buffer = buffer + c.ToString();
                }
            }
            return r;
        }

        public static void loadDiseño(string dir_esquema, string nombre_esquema, TableLayoutPanel panel)
        {
            if (panel.InvokeRequired == true)
            {
                panel.Invoke((MethodInvoker)delegate
                {
                    PictureBox a1 = new PictureBox();
                    a1.Dock = DockStyle.Fill;
                    a1.BackColor = Color.LightBlue;
                    a1.InitialImage = Properties.Resources.loading_gif;
                    a1.SizeMode = PictureBoxSizeMode.StretchImage;
                    a1.BackgroundImageLayout = ImageLayout.Stretch;
                    a1.Margin = new Padding(0, 0, 0, 0);
                    if (panel.BackgroundImage != null)
                    {
                        a1.BackgroundImage = panel.BackgroundImage;
                    }
                    setImage(dir_esquema, nombre_esquema, "png", a1);
                    panel.Controls.Add(a1);
                    string[] esquema = leerEsquema(nombre_esquema);
                    int s = stringToInt(esquema[0]);
                    if (s > 1)
                    {
                        if (esquema[2] == "V")
                        {
                            panel.SetRowSpan(a1, s);
                        }
                        else if (esquema[2] == "H")
                        {
                            panel.SetColumnSpan(a1, s);
                        }
                        else if (esquema[2] == "VH")
                        {
                            panel.SetRowSpan(a1, stringToInt(esquema[3]));
                            panel.SetColumnSpan(a1, stringToInt(esquema[4]));
                        }
                    }
                });
            }
            else
            {
                PictureBox a1 = new PictureBox();
                a1.Dock = DockStyle.Fill;
                a1.BackColor = Color.LightBlue;
                a1.InitialImage = Properties.Resources.loading_gif;
                a1.SizeMode = PictureBoxSizeMode.StretchImage;
                a1.BackgroundImageLayout = ImageLayout.Stretch;
                a1.Margin = new Padding(0, 0, 0, 0);
                if (panel.BackgroundImage != null)
                {
                    a1.BackgroundImage = panel.BackgroundImage;
                }
                setImage(dir_esquema, nombre_esquema, "png", a1);
                panel.Controls.Add(a1);
                string[] esquema = leerEsquema(nombre_esquema);
                int s = stringToInt(esquema[0]);
                if (s > 1)
                {
                    if (esquema[2] == "V")
                    {
                        panel.SetRowSpan(a1, s);
                    }
                    else if (esquema[2] == "H")
                    {
                        panel.SetColumnSpan(a1, s);
                    }
                    else if (esquema[2] == "VH")
                    {
                        panel.SetRowSpan(a1, stringToInt(esquema[3]));
                        panel.SetColumnSpan(a1, stringToInt(esquema[4]));
                    }
                }
            }                                              
        }

        public static void loadDiseñoOnDimension(string dir_esquema, string nombre_esquema, TableLayoutPanel panel)
        {
            if (panel.InvokeRequired == true)
            {
                panel.Invoke((MethodInvoker)delegate
                {
                    PictureBox a1 = new PictureBox();
                    a1.SizeMode = PictureBoxSizeMode.AutoSize;
                    a1.BackColor = Color.LightBlue;
                    a1.InitialImage = Properties.Resources.loading_gif;
                    a1.SizeMode = PictureBoxSizeMode.StretchImage;
                    a1.Margin = new Padding(0, 0, 0, 0);
                    a1.BackgroundImageLayout = ImageLayout.Stretch;
                    setImage(dir_esquema, nombre_esquema, "png", a1);
                    panel.Controls.Add(a1);
                    string[] esquema = leerEsquema(nombre_esquema);
                    int s = stringToInt(esquema[0]);
                    if (s > 1)
                    {
                        if (esquema[2] == "V")
                        {
                            panel.SetRowSpan(a1, s);
                        }
                        else if (esquema[2] == "H")
                        {
                            panel.SetColumnSpan(a1, s);
                        }
                        else if (esquema[2] == "VH")
                        {
                            panel.SetRowSpan(a1, stringToInt(esquema[3]));
                            panel.SetColumnSpan(a1, stringToInt(esquema[4]));
                        }
                    }
                });
            }
            else
            {
                PictureBox a1 = new PictureBox();
                a1.SizeMode = PictureBoxSizeMode.AutoSize;
                a1.BackColor = Color.LightBlue;
                a1.InitialImage = Properties.Resources.loading_gif;
                a1.SizeMode = PictureBoxSizeMode.StretchImage;
                a1.Margin = new Padding(0, 0, 0, 0);
                a1.BackgroundImageLayout = ImageLayout.Stretch;
                setImage(dir_esquema, nombre_esquema, "png", a1);
                panel.Controls.Add(a1);
                string[] esquema = leerEsquema(nombre_esquema);
                int s = stringToInt(esquema[0]);
                if (s > 1)
                {
                    if (esquema[2] == "V")
                    {
                        panel.SetRowSpan(a1, s);
                    }
                    else if (esquema[2] == "H")
                    {
                        panel.SetColumnSpan(a1, s);
                    }
                    else if (esquema[2] == "VH")
                    {
                        panel.SetRowSpan(a1, stringToInt(esquema[3]));
                        panel.SetColumnSpan(a1, stringToInt(esquema[4]));
                    }
                }
            }      
        }

        public static Color getColor(string hex)
        {
            Color color;
            try
            {
                color = ColorTranslator.FromHtml(hex);
            }
            catch (Exception)
            {
                color = Color.Black;
            }
            return color;
        }


        public static float reloadCalcularCostoModulo(int modulo_id, float mano_obra, int cantidad, string dimensiones, string claves_cristales, float flete, float desperdicio, float utilidad, string claves_otros, string claves_herrajes, string claves_perfiles, string new_items, string acabado, int id)
        {
            listas_entities_pva listas = new listas_entities_pva();
            cotizaciones_local cotizacion;
            List<string> n_items = new List<string>();
            bool error = false;    
            float costo = 0;
            int rows = 0;
            int columns = 0;
            bool cs = false;
            string buffer = string.Empty;
            float get = 0;
            float _total = 0;

            if (new_items.Length > 0)
            {
                foreach (char x in new_items)
                {
                    if (x == ';')
                    {
                        n_items.Add(buffer);
                        buffer = string.Empty;
                        continue;
                    }
                    buffer = buffer + x.ToString();
                }
            }

            var modulo = (from x in listas.modulos where x.id == modulo_id select x).SingleOrDefault();

            if (modulo != null)
            {
                cs = (bool)modulo.cs;
                string[] s_t;
                string[] d_s = dimensiones.Split(',');
                int[] d_num = new int[d_s.Length-1];

                for(int i = 0; i < d_num.Length; i++)
                {
                    d_num[i] = int.Parse(d_s[i]);
                }

                int esquema_id = (int)modulo.id_diseño;

                var esquema = (from x in listas.esquemas where x.id == esquema_id select x).SingleOrDefault();

                int c = 0;

                if(esquema != null)
                {
                    if(esquema.marco == false)
                    {
                        c = 1;
                    }
                    rows = (int)esquema.filas;
                    columns = (int)esquema.columnas;
                }

                int[,] arr = new int[((int)modulo.secciones+1)+c,2];

                for (int i = 0; i < d_num.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        arr[c,0] = d_num[i];
                    }
                    else
                    {
                        arr[c,1] = d_num[i];
                        c++;
                    }
                }

                if (claves_perfiles.Length > 0)
                {
                    d_s = claves_perfiles.Split(',');
                    s_t = modulo.id_aluminio.Split(',');

                    if (s_t.Length != d_s.Length)
                    {
                        error = true;
                    }
                    else
                    {
                        for (int i = 0; i < s_t.Length - 1; i++)
                        {
                            get = leerModuloAluminio(listas, s_t[i], d_s[i], arr, acabado);
                            if (get >= 0)
                            {
                                costo = costo + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                    }
                }

                // new items ---------------------------
                if (n_items.Count > 0)
                {
                    foreach (string z in n_items)
                    {
                        string[] p = z.Split(',');
                        if (p[0] == "1")
                        {
                            get = leerNewModuloAluminio(listas, p[1], stringToInt(p[2]), p[3], stringToInt(p[4]), p[5], arr);
                            if (get >= 0)
                            {
                                costo = costo + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }                      
                    }
                }
                // --------------------------------------

                //Calcular Desperdicio
                costo = costo + (costo * desperdicio);
                //-------------------------------->

                if (claves_herrajes.Length > 0)
                {
                    d_s = claves_herrajes.Split(',');
                    s_t = modulo.id_herraje.Split(',');

                    if (s_t.Length != d_s.Length)
                    {
                        error = true;
                    }
                    else
                    {
                        for (int i = 0; i < s_t.Length - 1; i++)
                        {
                            get = leerModuloHerrajes(listas, s_t[i], d_s[i]);
                            if (get >= 0)
                            {
                                costo = costo + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                    }
                }

                if (claves_otros.Length > 0)
                {
                    d_s = claves_otros.Split(',');
                    s_t = modulo.id_otros.Split(',');

                    if (s_t.Length != d_s.Length)
                    {
                        error = true;
                    }
                    else
                    {
                        for (int i = 0; i < s_t.Length - 1; i++)
                        {
                            get = leerModuloOtros(listas, s_t[i], d_s[i], arr, cs, rows, columns);
                            if (get >= 0)
                            {
                                costo = costo + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                    }
                }

                if (claves_cristales.Length > 0)
                {
                    d_s = claves_cristales.Split(',');
                    s_t = modulo.clave_vidrio.Split(',');

                    if (s_t.Length != d_s.Length)
                    {
                        error = true;
                    }
                    else
                    {
                        for (int i = 0; i < s_t.Length - 1; i++)
                        {
                            get = leerModuloCristales(listas, s_t[i], d_s[i], arr, cs, rows, columns);
                            if (get >= 0)
                            {
                                costo = costo + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                    }
                }

                //new items ----------------
                if (n_items.Count > 0)
                {
                    foreach (string z in n_items)
                    {
                        string[] p = z.Split(',');
                        if (p[0] == "2")
                        {
                            get = leerNewModuloCristales(listas, p[1], stringToInt(p[2]), stringToInt(p[4]), arr, cs, rows, columns);
                            if (get >= 0)
                            {
                                costo = costo + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                        else if (p[0] == "3")
                        {
                            get = leerNewModuloHerrajes(listas, p[1], stringToInt(p[2]));
                            if (get >= 0)
                            {
                                costo = costo + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                        else if (p[0] == "4")
                        {
                            get = leerNewModuloOtros(listas, p[1], stringToInt(p[2]), p[3], stringToInt(p[4]), arr, cs, rows, columns);
                            if (get >= 0)
                            {
                                costo = costo + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                        //
                        else if (p[0] == "5")
                        {
                            if (p.Length == 4)
                            {
                                if (p[3] != "Total")
                                {
                                    costo = costo + stringToFloat(p[2]);
                                }
                                else
                                {
                                    _total = _total + stringToFloat(p[2]);
                                }
                            }
                        }
                    }
                }
                //------------------------->
                if(error == true)
                {
                    if (errors_Open.Contains(id) == false)
                    {
                        errors_Open.Add(id);
                        cotizacion = new cotizaciones_local();
                        var w = (from x in cotizacion.modulos_cotizaciones where x.id == id select x).SingleOrDefault();
                        if(w != null)
                        {
                            if(w.merge_id > 0)
                            {
                                if (errors_Open.Contains((int)w.merge_id) == false)
                                {
                                    errors_Open.Add((int)w.merge_id);
                                }
                            }
                        }
                    }
                }
            }
            //Resultados
            costo = costo + (costo * flete);
            costo = costo + (costo * mano_obra);
            costo = costo + (costo * utilidad);            
            //
            return ((costo) * cantidad) + _total;
        }

        public static void checkErrorsOnLoad()
        {
            if(errors_Open.Count > 0)
            {
                ((Form1)Application.OpenForms["form1"]).checkErrorsModulos();
                MessageBox.Show("[Advertencia]: Se han detectado algunos errores dentro de la configuración de los módulos siguientes. Configura de nuevo los módulos afectados.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static string getDiseñoModulo(int id_diseño)
        {
            listas_entities_pva listas = new listas_entities_pva();

            var diseño = (from x in listas.esquemas where x.id == id_diseño select x).SingleOrDefault();

            if (diseño != null)
            {
                return diseño.diseño;
            }
            else
            {
                return "";
            }
        }

        public static string getArticuloProveedorCristales(string clave)
        {
            string r = string.Empty;
            listas_entities_pva listas = new listas_entities_pva();
            var articulo = (from x in listas.lista_costo_corte_e_instalado where x.clave == clave select x).SingleOrDefault();
            if(articulo != null)
            {
                r = articulo.proveedor;
            }
            return r;       
        }

        public static string getArticuloProveedorPerfiles(int id)
        {
            string r = string.Empty;
            listas_entities_pva listas = new listas_entities_pva();
            var articulo = (from x in listas.perfiles where x.id == id select x).SingleOrDefault();
            if (articulo != null)
            {
                r = articulo.proveedor;
            }
            return r;
        }

        public static string getArticuloProveedorHerrajes(int id)
        {
            string r = string.Empty;
            listas_entities_pva listas = new listas_entities_pva();
            var articulo = (from x in listas.herrajes where x.id == id select x).SingleOrDefault();
            if (articulo != null)
            {
                r = articulo.proveedor;
            }
            return r;
        }

        public static string getArticuloProveedorOtros(int id)
        {
            string r = string.Empty;
            listas_entities_pva listas = new listas_entities_pva();
            var articulo = (from x in listas.otros where x.id == id select x).SingleOrDefault();
            if (articulo != null)
            {
                r = articulo.proveedor;
            }
            return r;
        }

        public static string getFormaPago()
        {
            string r = string.Empty;
            try
            {
                XDocument propiedades = XDocument.Load(propiedades_xml);

                var fp = (from x in propiedades.Descendants("Propiedades") select x.Element("FP")).SingleOrDefault();

                if (fp != null)
                {
                    r = fp.Value;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            return r;
        }

        //historial de login
        public static void crearHistorialLogin(string usuario, string nombre_pc, string ip, string fecha)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            int rows = sql.countSQLRows("historial_login");
            if (rows >= 50)
            {
                sql.deleteHistoryLogin();
                DataTable t = new DataTable();
                t = sql.createDataTableFromSQLTable("historial_login");
                sql.truncateSQLTable("historial_login");
                int c = 1;
                foreach (DataRow x in t.Rows)
                {
                    if (c >= 50)
                    {
                        break;
                    }
                    sql.insertHistoryLogin(c, x[1].ToString(), x[2].ToString(), x[3].ToString(), x[4].ToString());
                    c++;
                }
                t.Dispose();
            }
            sql.insertHistoryLogin(rows+1, usuario, nombre_pc, ip, fecha);
        }
        //ends historial de logins

        public static Image getImage(string dir)
        {
            try
            {
                Image img = Image.FromFile(dir);
                return img;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Byte[] imageToByte(Image image)
        {
            if (image != null)
            {
                Byte[] r;
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    r = ms.ToArray();
                    ms.Close();
                    ms.Dispose();
                };
                if (r.Length > 0)
                {
                    return r;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static Bitmap createBitmap(string dir, int width, int height)
        {
            try
            {
                Bitmap bm = new Bitmap(dir);
                return new Bitmap(bm, width, height);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Bitmap MergeTwoImages(Image firstImage, Image secondImage)
        {
            Bitmap outputImage = null;
            if (firstImage != null && secondImage != null)
            {
                int outputImageHeight = firstImage.Height > secondImage.Height ? firstImage.Height : secondImage.Height;

                int outputImageWidth = firstImage.Width + secondImage.Width;

                outputImage = new Bitmap(outputImageWidth + 6, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics graphics = Graphics.FromImage(outputImage))
                {
                    graphics.Clear(Color.White);
                    graphics.DrawImage(firstImage, new Rectangle(new Point(), firstImage.Size),
                        new Rectangle(new Point(), firstImage.Size), GraphicsUnit.Pixel);
                    graphics.DrawImage(secondImage, new Rectangle(new Point(firstImage.Width + 6, 0), secondImage.Size),
                        new Rectangle(new Point(), secondImage.Size), GraphicsUnit.Pixel);                   
                }
            }           
            return outputImage;
        }

        public static Image byteToImage(byte[] ba)
        {
            Image img = null;
            if (ba != null)
            {
                using (var ms = new MemoryStream(ba))
                {
                    img = Image.FromStream(ms);
                }
            }
            return img;
        }

        public static void duplicarConcepto(int tipo_cotizacion, int id_concepto, int sub_folio=-1)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            if(tipo_cotizacion == 1)
            {
                var cristal = (from x in cotizaciones.cristales_cotizados where x.id == id_concepto select x).SingleOrDefault();
                if(cristal != null)
                {
                    var cristales = new cristales_cotizados
                    {
                        folio = 00000,
                        clave = getClave(cristal.clave),
                        articulo = cristal.articulo,
                        lista = cristal.lista,
                        proveedor = cristal.proveedor,
                        largo = Math.Round((float)cristal.largo, 2),
                        alto = Math.Round((float)cristal.alto, 2),
                        canteado = cristal.canteado,
                        biselado = cristal.biselado,
                        desconchado = cristal.desconchado,
                        pecho_paloma = cristal.pecho_paloma,
                        perforado_media_pulgada = cristal.perforado_media_pulgada,
                        perforado_una_pulgada = cristal.perforado_una_pulgada,
                        perforado_dos_pulgadas = cristal.perforado_dos_pulgadas,
                        grabado = cristal.grabado,
                        esmerilado = cristal.esmerilado,
                        cantidad = Math.Round((float)cristal.cantidad, 2),
                        tipo_venta = cristal.tipo_venta,
                        descuento = Math.Round((float)cristal.descuento, 2),
                        total = Math.Round((float)cristal.total, 2),
                        filo_muerto = Math.Round((float)cristal.filo_muerto, 2),
                        descripcion = cristal.descripcion
                    };
                    cotizaciones.cristales_cotizados.Add(cristales);
                    cotizaciones.SaveChanges();
                }
            }
            else if(tipo_cotizacion == 2)
            {
                var perfil = (from x in cotizaciones.aluminio_cotizado where x.id == id_concepto select x).SingleOrDefault();
                if (perfil != null)
                {
                    var aluminio = new aluminio_cotizado
                    {
                        folio = 00000,
                        clave = getClave(perfil.clave),
                        articulo = perfil.articulo,
                        lista = perfil.lista,
                        proveedor = perfil.proveedor,
                        linea = perfil.linea,
                        largo_total = perfil.largo_total,
                        acabado = perfil.acabado,
                        cantidad = Math.Round((float)perfil.cantidad, 2),
                        descuento = Math.Round((float)perfil.descuento, 2),
                        total = Math.Round((float)perfil.total, 2),
                        descripcion = perfil.descripcion
                    };
                    cotizaciones.aluminio_cotizado.Add(aluminio);
                    cotizaciones.SaveChanges();
                }
            }
            else if (tipo_cotizacion == 3)
            {
                var herraje = (from x in cotizaciones.herrajes_cotizados where x.id == id_concepto select x).SingleOrDefault();
                if (herraje != null)
                {
                    var herrajes = new herrajes_cotizados
                    {
                        folio = 00000,
                        clave = getClave(herraje.clave),
                        articulo = herraje.articulo,
                        proveedor = herraje.proveedor,
                        linea = herraje.linea,
                        caracteristicas = herraje.caracteristicas,
                        color = herraje.color,
                        cantidad = Math.Round((float)herraje.cantidad, 2),
                        descuento = Math.Round((float)herraje.descuento, 2),
                        total = Math.Round((float)herraje.total, 2),
                        descripcion = herraje.descripcion
                    };
                    cotizaciones.herrajes_cotizados.Add(herrajes);
                    cotizaciones.SaveChanges();
                }
            }
            else if (tipo_cotizacion == 4)
            {
                var otros = (from x in cotizaciones.otros_cotizaciones where x.id == id_concepto select x).SingleOrDefault();
                if (otros != null)
                {
                    var o = new otros_cotizaciones
                    {
                        folio = 00000,
                        clave = getClave(otros.clave),
                        articulo = otros.articulo,
                        proveedor = otros.proveedor,
                        linea = otros.linea,
                        caracteristicas = otros.caracteristicas,
                        color = otros.color,
                        cantidad = Math.Round((float)otros.cantidad, 2),
                        descuento = Math.Round((float)otros.descuento, 2),
                        largo = Math.Round((float)otros.largo, 2),
                        alto = Math.Round((float)otros.alto, 2),
                        total = Math.Round((float)otros.total, 2),
                        descripcion = otros.descripcion
                    };
                    cotizaciones.otros_cotizaciones.Add(o);
                    cotizaciones.SaveChanges();
                }
            }
            else if (tipo_cotizacion == 5)
            {
                var modulos = (from x in cotizaciones.modulos_cotizaciones where x.id == id_concepto select x).SingleOrDefault();
                if (modulos != null)
                {
                    var paste_k = new modulos_cotizaciones()
                    {
                        folio = 00000,
                        modulo_id = modulos.modulo_id,
                        descripcion = modulos.descripcion,
                        mano_obra = modulos.mano_obra,
                        dimensiones = modulos.dimensiones,
                        acabado_perfil = modulos.acabado_perfil,
                        claves_cristales = modulos.claves_cristales,
                        cantidad = modulos.cantidad,
                        articulo = modulos.articulo,
                        linea = modulos.linea,
                        diseño = modulos.diseño,
                        clave = getClave(modulos.clave),
                        total = modulos.total,
                        largo = modulos.largo,
                        alto = modulos.alto,
                        flete = modulos.flete,
                        utilidad = modulos.utilidad,
                        desperdicio = modulos.desperdicio,
                        claves_otros = modulos.claves_otros,
                        claves_herrajes = modulos.claves_herrajes,
                        ubicacion = modulos.ubicacion,
                        pic = modulos.pic,
                        claves_perfiles = modulos.claves_perfiles,
                        merge_id = modulos.merge_id,
                        concept_id = modulos.concept_id,
                        sub_folio = sub_folio <= 0 ? modulos.sub_folio : sub_folio,
                        dir = modulos.dir,
                        news = modulos.news,
                        new_desing = modulos.new_desing,
                        orden = getCountPartidas()
                    };
                    cotizaciones.modulos_cotizaciones.Add(paste_k);
                    cotizaciones.SaveChanges();

                    if (modulos.modulo_id == -1)
                    {
                        int last = 0;
                        var g = (from x in cotizaciones.modulos_cotizaciones orderby x.id descending select x).FirstOrDefault();

                        if (g != null)
                        {
                            last = g.id;
                        }

                        var last_m = (from x in cotizaciones.modulos_cotizaciones where x.id == last select x).SingleOrDefault();

                        if (last_m != null)
                        {
                            last_m.concept_id = last;

                            var copy_m = from x in cotizaciones.modulos_cotizaciones where x.merge_id == id_concepto select x;

                            if (copy_m != null)
                            {
                                foreach (var v in copy_m)
                                {
                                    var paste_m = new modulos_cotizaciones()
                                    {
                                        folio = 00000,
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
                                        clave = getClave(v.clave),
                                        total = v.total,
                                        largo = v.largo,
                                        alto = v.alto,
                                        flete = v.flete,
                                        utilidad = v.utilidad,
                                        desperdicio = v.desperdicio,
                                        claves_otros = v.claves_otros,
                                        claves_herrajes = v.claves_herrajes,
                                        ubicacion = v.ubicacion,
                                        pic = v.pic,
                                        claves_perfiles = v.claves_perfiles,
                                        merge_id = last,
                                        concept_id = v.concept_id,
                                        sub_folio = sub_folio <= 0 ? v.sub_folio : sub_folio,
                                        dir = v.dir,
                                        news = v.news,
                                        new_desing = v.new_desing,
                                        orden = 0
                                    };
                                    cotizaciones.modulos_cotizaciones.Add(paste_m);
                                }
                            }
                        }
                        cotizaciones.SaveChanges();
                    }
                }
            }
        }

        public static void duplicarSubfolio(int origen_subfolio, int destino_subfolio)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            var modulos = (from x in cotizaciones.modulos_cotizaciones where x.sub_folio == origen_subfolio && x.merge_id == -1 select x);

            if(modulos != null)
            {
                foreach(var c in modulos)
                {
                    duplicarConcepto(5, c.id, destino_subfolio);
                }
            }
        }

        public static bool getLineasInConcept(string lineas, string linea_c)
        {
            bool r = false;
            string[] g = lineas.Split('/');
            foreach (string x in g)
            {
                if (x == linea_c)
                {
                    r = true;
                    break;
                }
            }
            return r;
        }

        public static bool getUbicacionInConcept(string ubicaciones, string ubicacion_c)
        {
            bool r = false;
            string[] g = ubicaciones.Split('/');
            foreach (string x in g)
            {
                if (x == ubicacion_c)
                {
                    r = true;
                    break;
                }
            }
            return r;
        }

        //send mail
        public static bool email_send(string from, string password, string to, string subject, string body, List<string> files)
        {
            bool r = true;
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(smtp);
            try {              
                mail.From = new MailAddress(from);
                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                
                if (files.Count > 0)
                {
                    Attachment attachment;
                    foreach (string x in files)
                    {
                        if (x != "")
                        {
                            attachment = new Attachment(x);
                            mail.Attachments.Add(attachment);
                        }
                    }
                }

                SmtpServer.Port = m_port;
                SmtpServer.Timeout = timeout;
                SmtpServer.Credentials = new NetworkCredential(from, password);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);              
            }
            catch (Exception err)
            {
                errorLog(err.ToString());
                r = false;
            }
            finally
            {
                SmtpServer.Dispose();
                mail.Dispose();               
            }
            return r;
        }
        //////--------------------------------------------------------------------------------------->

        public static string IASetAcabado(string acabado)
        {
            string r = string.Empty;
            string[] blanco = new string[] {"MEPCBL", "BL1001", "BL9010"};
            string[] hueso = new string[] {"BEG1013", "BEG9001"};
            string[] negro = new string[] {"EPENGRO", "ELEC400", "NGR8908", "NGR314", "NGRPN205"};
            string[] gris = new string[] {"EPEGREU", "GRE049", "SILM349", "GRSPG105", "GRPG331"};
            string[] natural = new string[] {"SMNATM", "SMNATB"};
            string[] brillante = new string[] {"SMNATBR"};
            string[] madera = new string[] {"EPEMCAL", "EPEMCAT", "EPEMCEL", "EPEMCET", "NT4668", "EPEMAFL", "EPEMAFT", "EPEMNAL", "EPEMNAT", "EPENP007", "EPEP3025", "NL4686", "MNTE", "CRZ4686", "CRZ4701", "PNL4252", "PNT44252"};
            string[] acero_inox = new string[] {"ELECACI"};
            string[] chocolate = new string[] {"EPECHOC", "CHCOMM214S"};
            string[] bronce = new string[] {"ELECBROC"};
            string[] champagne = new string[] {"ELEC100"};

            if (acabado == "blanco")
            {
                if (blanco.Length > 0)
                {
                    r = blanco[0];
                }
            }
            else if (acabado == "hueso")
            {
                if (hueso.Length > 0)
                {
                    r = hueso[0];
                }
            }
            else if (acabado == "negro")
            {
                if (negro.Length > 0)
                {
                    r = negro[0];
                }
            }
            else if (acabado == "gris")
            {
                if (gris.Length > 0)
                {
                    r = gris[0];
                }
            }
            else if (acabado == "natural")
            {
                if (natural.Length > 0)
                {
                    r = natural[0];
                }
            }
            else if (acabado == "brillante")
            {
                if (brillante.Length > 0)
                {
                    r = brillante[0];
                }
            }
            else if (acabado == "madera")
            {
                if (madera.Length > 0)
                {
                    r = madera[0];
                }
            }
            else if (acabado == "acero_inox")
            {
                if (acero_inox.Length > 0)
                {
                    r = acero_inox[0];
                }
            }
            else if (acabado == "chocolate")
            {
                if (chocolate.Length > 0)
                {
                    r = chocolate[0];
                }
            }
            else if (acabado == "bronce")
            {
                if (bronce.Length > 0)
                {
                    r = bronce[0];
                }
            }
            else if (acabado == "champagne")
            {
                if (champagne.Length > 0)
                {
                    r = champagne[0];
                }
            }

            foreach (string x in blanco)
            {
                if(x == acabado)
                {
                    r = "blanco";
                    break;
                }
            }

            foreach (string x in hueso)
            {
                if (x == acabado)
                {
                    r = "hueso";
                    break;
                }
            }

            foreach (string x in negro)
            {
                if (x == acabado)
                {
                    r = "negro";
                    break;
                }
            }

            foreach (string x in gris)
            {
                if (x == acabado)
                {
                    r = "gris";
                    break;
                }
            }

            foreach (string x in natural)
            {
                if (x == acabado)
                {
                    r = "natural";
                    break;
                }
            }

            foreach (string x in brillante)
            {
                if (x == acabado)
                {
                    r = "brillante";
                    break;
                }
            }

            foreach (string x in madera)
            {
                if (x == acabado)
                {
                    r = "madera";
                    break;
                }
            }

            foreach (string x in acero_inox)
            {
                if (x == acabado)
                {
                    r = "acero_inox";
                    break;
                }
            }

            foreach (string x in chocolate)
            {
                if (x == acabado)
                {
                    r = "chocolate";
                    break;
                }
            }

            foreach (string x in bronce)
            {
                if (x == acabado)
                {
                    r = "bronce";
                    break;
                }
            }

            foreach (string x in champagne)
            {
                if (x == acabado)
                {
                    r = "champagne";
                    break;
                }
            }
            return r;
        }

        public static string getCristales(string cristales_claves, string news_c)
        {         
            listas_entities_pva listas = new listas_entities_pva();

            string cristales = string.Empty;
            bool n_c = false;
            List<string> cris_list = new List<string>();
            string buffer = string.Empty;
            bool t = false;
            string c_clave = string.Empty;
            float c = 0;

            //obtener claves cristales new
            string[] news = news_c.Split(';');
            foreach (string k in news)
            {
                string[] o = k.Split(',');
                if (o[0] == "2")
                {
                    n_c = false;
                    foreach (string x in cris_list)
                    {
                        if (x == o[1])
                        {
                            n_c = true;
                            break;
                        }
                    }

                    if (n_c == false && stringToFloat(o[2]) > 0)
                    {
                        cris_list.Add(o[1]);
                    }
                }
            }

            //obtener claves cristales (no-repetir)
            foreach (char cri in cristales_claves)
            {
                if (cri != ',')
                {
                    if (t)
                    {
                        c = stringToFloat(cri.ToString());
                    }
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
                    foreach (string x in cris_list)
                    {
                        if (x == c_clave)
                        {
                            n_c = true;
                            break;
                        }
                    }

                    if (n_c == false && c > 0)
                    {
                        cris_list.Add(c_clave);
                    }
                }
            }
            //--------------------------------------------------------->

            //obtener nombre de cristales
            foreach (string x in cris_list)
            {
                var g = (from n in listas.lista_precio_corte_e_instalado where n.clave == x select n).SingleOrDefault();

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
            cris_list.Clear();       
            return cristales;
        }

        public static int getCountPartidas()
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            var count = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == -1 select x).Count();
            if (count > 0)
            {
                var modulos = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == -1 orderby x.orden descending select x).First();

                if (modulos != null)
                {
                    return (int)modulos.orden + 1;
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

        public static string setUserItemClave(string lista)
        {
            string s = "";
            sqlDateBaseManager sql = new sqlDateBaseManager();
            Random r = new Random();
            s = "USER" + r.Next(100000, 199999).ToString();

            while (sql.findSQLValue("clave", "clave", lista, s) == true)
            {
                s = "USER" + r.Next(100000, 199999).ToString();
            }
            return s;
        }

        public static void reloadUserItems(Form form)
        {
            try
            {           
                listas_entities_pva listas = new listas_entities_pva();
                var items = (from x in listas.paquetes select x);

                string[] n = null;
                string[] k = null;
                string a = string.Empty;
                float c = 0;
                string clave = string.Empty;

                if(items != null)
                {
                    foreach(var x in items)
                    {
                        a = x.comp_items;
                        c = 0;
                        if (a != "")
                        {
                            n = a.Split(',');
                            foreach(string z in n)
                            {
                                k = z.Split(':');
                                if (k.Length == 3)
                                {
                                    clave = k[1];
                                    if (k[0] == "1")
                                    {
                                        var vidrio = (from y in listas.lista_costo_corte_e_instalado where y.clave == clave select y).SingleOrDefault();

                                        if (vidrio != null)
                                        {
                                            c = c + (float)Math.Round((float)vidrio.costo_corte_m2 * stringToInt(k[2]), 2);
                                        }
                                    }
                                    else if (k[0] == "2")
                                    {
                                        var herraje = (from y in listas.herrajes where y.clave == clave select y).SingleOrDefault();

                                        if (herraje != null)
                                        {
                                            c = c + (float)Math.Round((float)herraje.precio * stringToInt(k[2]), 2);
                                        }
                                    }
                                    else if (k[0] == "3")
                                    {
                                        var otro = (from y in listas.otros where y.clave == clave select y).SingleOrDefault();

                                        if (otro != null)
                                        {
                                            c = c + (float)Math.Round((float)otro.precio * stringToInt(k[2]), 2);
                                        }
                                    }
                                }
                            }
                        }
                        //--------------------------------------------------------------------------------------------------------->
                        if (c > 0)
                        {
                            clave = x.comp_clave;
                            if (x.comp_type == "Cristal")
                            {
                                var cristales = (from v in listas.lista_costo_corte_e_instalado where v.clave == clave select v).SingleOrDefault();
                                if (cristales != null)
                                {                                   
                                    cristales.costo_corte_m2 = c;
                                }
                            }
                            else if (x.comp_type == "Herraje")
                            {
                                var harrajes = (from v in listas.herrajes where v.clave == clave select v).SingleOrDefault();
                                if (harrajes != null)
                                {
                                    harrajes.precio = c;
                                }
                            }
                            else if (x.comp_type == "Otros Materiales")
                            {
                                var otros = (from v in listas.otros where v.clave == clave select v).SingleOrDefault();
                                if (otros != null)
                                {
                                    otros.precio = c;
                                }
                            }
                        }
                    }
                    listas.SaveChanges();                
                }
            }
            catch (Exception err)
            {
                errorLog(err.ToString());
                MessageBox.Show(form, "[Error] no se pudierón actualizar los paquetes y servicios.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string getExternalImage(string type)
        {
            string path = string.Empty;

            if (type == "header")
            {
                if (File.Exists(Application.StartupPath + "\\pics\\reportes\\" + header_reporte + ".jpg"))
                {
                    Uri uri = new Uri(Application.StartupPath + "\\pics\\reportes\\" + header_reporte + ".jpg");
                    path = uri.AbsoluteUri;
                }
                else
                {
                    string web = "http://" + server + "/" + header_reporte + ".jpg";
                    path = web;                    
                }
            }
            else if(type == "marca")
            {
                if (File.Exists(Application.StartupPath + "\\pics\\reportes\\fondos\\" + header_reporte + ".bmp"))
                {
                    Uri uri = new Uri(Application.StartupPath + "\\pics\\reportes\\fondos\\" + header_reporte + ".bmp");
                    path = uri.AbsoluteUri;
                }
                else
                {
                    string web = "http://" + server + "/" + header_reporte + ".bmp";
                    path = web;
                }
            }

            return path;
        }

        public static bool getVigencia(DateTime date)
        {          
            return DateTime.Now <= date ? true : false;
        }

        public static bool getAlertVigencia(DateTime date)
        {          
            return DateTime.Now.AddDays(5) >= date ? true : false;
        }

        public static float getTCFromXML()
        {
            try
            {
                XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                var tc = (from x in opciones_xml.Descendants("Opciones") select x.Element("TC")).SingleOrDefault();

                if(tc != null)
                {
                    return stringToFloat(tc.Value.ToString());
                }
            }
            catch (Exception err)
            {
                errorLog(err.ToString());
                MessageBox.Show("[Error] no se puede obtener el tipo de cambio desde el archivo opciones.xml.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return 0;
        }

        public static void setPropiedadesXML(float tipo_cambio, float costo_aluminio)
        {
            try
            {
                //TC
                if (tipo_cambio > 0)
                {
                    if (tipo_cambio != tc)
                    {                       
                        XDocument opciones_xml = XDocument.Load(constants.opciones_xml);
                        var mv = from x in opciones_xml.Descendants("Opciones") select x;
                        foreach (XElement x in mv)
                        {
                            x.SetElementValue("TC", tipo_cambio.ToString());
                        }
                        opciones_xml.Save(constants.opciones_xml);
                    }
                }

                //Costo KG Aluminio
                if (costo_aluminio > 0)
                {
                    if (costo_aluminio != costo_aluminio_kg)
                    {
                        costo_aluminio_kg = costo_aluminio;
                        XDocument opciones_xml = XDocument.Load(constants.opciones_xml);
                        var mv = from x in opciones_xml.Descendants("Opciones") select x;
                        foreach (XElement x in mv)
                        {
                            x.SetElementValue("CAKG", costo_aluminio.ToString());
                        }
                        opciones_xml.Save(constants.opciones_xml);
                    }
                }
            }
            catch (Exception err)
            {
                errorLog(err.ToString());
                MessageBox.Show("[Error] no se puede guardar el tipo de cambio en el archivo opciones.xml.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void changeTC(float c_tc, float tc, string type)
        {
            listas_entities_pva listas = new listas_entities_pva();

            var c_costos = from x in listas.lista_costo_corte_e_instalado where x.moneda == type select x;
            foreach (var v in c_costos)
            {
                v.costo_corte_m2 = Math.Round(((float)v.costo_corte_m2 / c_tc) * tc, 2);
                v.costo_instalado = Math.Round(((float)v.costo_instalado / c_tc) * tc, 2);
            }

            var c_insta = from x in listas.lista_precio_corte_e_instalado where x.moneda == type select x;
            foreach (var v in c_insta)
            {
                v.precio_venta_corte_m2 = Math.Round(((float)v.precio_venta_corte_m2 / c_tc) * tc, 2);
                v.precio_venta_instalado = Math.Round(((float)v.precio_venta_instalado / c_tc) * tc, 2);
            }

            var c_hojas = from x in listas.lista_precios_hojas where x.moneda == type select x;
            foreach (var v in c_hojas)
            {
                v.precio_hoja = Math.Round(((float)v.precio_hoja / c_tc) * tc, 2);
            }

            var perfiles = from x in listas.perfiles where x.moneda == type select x;
            foreach (var v in perfiles)
            {
                v.crudo = Math.Round(((float)v.crudo / c_tc) * tc, 2);
                v.hueso = Math.Round(((float)v.hueso / c_tc) * tc, 2);
                v.natural_1 = Math.Round(((float)v.natural_1 / c_tc) * tc, 2);
                v.blanco = Math.Round(((float)v.blanco / c_tc) * tc, 2);
                v.champagne = Math.Round(((float)v.champagne / c_tc) * tc, 2);
                v.brillante = Math.Round(((float)v.brillante / c_tc) * tc, 2);
                v.bronce = Math.Round(((float)v.bronce / c_tc) * tc, 2);
                v.gris = Math.Round(((float)v.gris / c_tc) * tc, 2);
                v.madera = Math.Round(((float)v.madera / c_tc) * tc, 2);
                v.chocolate = Math.Round(((float)v.chocolate / c_tc) * tc, 2);
                v.acero_inox = Math.Round(((float)v.acero_inox / c_tc) * tc, 2);
            }

            var herrajes = from x in listas.herrajes where x.moneda == type select x;
            foreach (var v in herrajes)
            {
                v.precio = Math.Round(((float)v.precio / c_tc) * tc, 2);
            }

            var otros = from x in listas.otros where x.moneda == type select x;
            foreach (var v in otros)
            {
                v.precio = Math.Round(((float)v.precio / c_tc) * tc, 2);
            }

            listas.SaveChanges();
            // ---->
            constants.tc = tc;
            ((Form1)Application.OpenForms["Form1"]).setTCLabel(constants.tc);
            ((Form1)Application.OpenForms["Form1"]).loadListaFromLocal();
        }

        public static bool setConnectionToLoginServer(string _data, Form form)
        {
            if (!login_server.Connected || login_server.Available == 0)
            {
                login_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var result = login_server.BeginConnect(new IPEndPoint(getSocketIPAddress(server, form), 6400), null, null);
                bool success = result.AsyncWaitHandle.WaitOne(10000, true);
                if (success)
                {
                    try
                    {
                        byte[] data = System.Text.Encoding.Default.GetBytes(_data);
                        login_server.Send(data);
                        return true;
                    }
                    catch (Exception)
                    {
                        login_server.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                        login_server.Close();
                        return false;
                    }
                }
                else
                {
                    login_server.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                    login_server.Close();
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static IPAddress getSocketIPAddress(string p, Form form)
        {
            if(getIPformString(p) != null)
            {
                return getIPformString(p);
            }
            else if(getIPfromHost(p) != null)
            {
                return getIPfromHost(p);
            }
            else
            {
                MessageBox.Show(form, "[Error] no se ha podido definir la dirección IP de login server.", msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private static IPAddress getIPformString(string ip)
        {
            IPAddress r;
            if(IPAddress.TryParse(ip, out r))
            {
                return r;
            }
            else
            {
                return null;
            }          
        }

        private static IPAddress getIPfromHost(string p)
        {
            IPAddress[] ips = null;
            try
            {
                ips = Dns.GetHostAddresses(p);
            }
            catch (Exception err)
            {
                errorLog(err.ToString());
            }

            if (ips == null || ips.Length == 0)
            {
                ips[0] = null;
            }

            return ips[0];                    
        }

        public static void setTiendas(ComboBox box)
        {        
            sqlDateBaseManager sql = new sqlDateBaseManager();
            List<string> tiendas = sql.getTiendas();
            if (tiendas.Count > 0)
            {
                box.Items.Clear();
                foreach (string x in tiendas)
                {
                    box.Items.Add(x);
                }
                box.Text = org_name;
            }          
        }

        //Sub-Folio Titles
        public static string getSubfoliotitle(int subfolio)
        {
            string u = string.Empty;
            try
            {
                if (subfolio_titles.Count > 0)
                {
                    subfolio = subfolio - 1;
                    if (subfolio >= 0 && subfolio <= 4)
                    {
                        if (subfolio_titles[subfolio] != null)
                        {
                            u = subfolio_titles[subfolio];
                        }
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("[Error] error al obtener los títulos de sub-folio.", msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorLog(e.ToString());
            }
            return u;
        }

        public static void setSubfoliotitle(int subfolio, string title)
        {
            try
            {
                subfolio = subfolio - 1;
                subfolio_titles[subfolio] = title;
            }
            catch(Exception e)
            {
                MessageBox.Show("[Error] error al añadir el título de sub-folio.", msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorLog(e.ToString());
            }
        }

        public static string serializeSubfolio()
        {
            string r = string.Empty;
            try
            {
                foreach (string x in subfolio_titles)
                {
                    if (r.Length > 0)
                    {
                        r = r + "," + x;
                    }
                    else
                    {
                        r = x;
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("[Error] error al serializar títulos de sub-folio.", msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorLog(e.ToString());
            }
            return r;
        }

        public static void unserializeSubfolio(string titles)
        {
            try
            {
                subfolio_titles.Clear();
                initsubfoliotitles();
                string[] r = titles.Split(',');
                if (r.Length > 0)
                {
                    for (int i = 0; i < r.Length; i++)
                    {
                        subfolio_titles[i] = r[i];
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("[Error] error al deserializar títulos de sub-folio.", msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorLog(e.ToString());
            }
        }

        public static void loadSubfoliotitles()
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            try
            {
                unserializeSubfolio(sql.getSingleSQLValue("cotizaciones", "subfolio_titles", "folio", folio_abierto.ToString(), 0));
            }
            catch (Exception e)
            {
                MessageBox.Show("[Error] no se pudo obtener los títulos de los sub-folios\n¿Cuenta con conexión a internet?.", msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorLog(e.ToString());
            }
        }

        public static void initsubfoliotitles()
        {
            subfolio_titles.Add("");
            subfolio_titles.Add("");
            subfolio_titles.Add("");
            subfolio_titles.Add("");
            subfolio_titles.Add("");
        }
    }
}
