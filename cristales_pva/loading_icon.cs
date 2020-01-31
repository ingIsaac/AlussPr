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
    public partial class loading_icon : Form
    {
        BackgroundWorker bw;
        
        public loading_icon()
        {
            InitializeComponent();
            bw = new BackgroundWorker();
            bw.DoWork += Bw_DoWork;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
            System.Diagnostics.FileVersionInfo info = constants.getFileInfoVersion();           
            label4.Text = info != null ? info.LegalCopyright : string.Empty;
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (constants.error == true)
            {
                Environment.Exit(0);
            }
            this.Close();
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {    
            //Load Version & XML Data      
            try
            {
                //Get version
                constants.getSoftwareVersion();
                label2.Text = "v." + constants.version;
                //Get XML Data
                loadServerData();
                constants.setServerCredentials();
            }
            catch (Exception err)
            {
                MessageBox.Show(this, "[Error]: <:SQLLocalDB?> Se necesitan instalar los complementos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
                constants.error = true;
            }
            //wait
            System.Threading.Thread.Sleep(5000);
        }

        private void loading_icon_Load(object sender, EventArgs e)
        {
            if (!bw.IsBusy)
            {
                bw.RunWorkerAsync();
            }
        }

        private void loadOpcionesData()
        {
            try
            {
                XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                var mv = (from x in opciones_xml.Descendants("Opciones") select x.Element("MV")).SingleOrDefault();
                var opi = (from x in opciones_xml.Descendants("Opciones") select x.Element("OPI")).SingleOrDefault();
                var ma = (from x in opciones_xml.Descendants("Opciones") select x.Element("MA")).SingleOrDefault();
                var acc = (from x in opciones_xml.Descendants("Opciones") select x.Element("ACC")).SingleOrDefault();
                var ecs = (from x in opciones_xml.Descendants("Opciones") select x.Element("ECS")).SingleOrDefault();
                var erl = (from x in opciones_xml.Descendants("Opciones") select x.Element("ERL")).SingleOrDefault();
                var ivd = (from x in opciones_xml.Descendants("Opciones") select x.Element("IVD")).SingleOrDefault();
                var pai = (from x in opciones_xml.Descendants("Opciones") select x.Element("PAI")).SingleOrDefault();
                var spac = (from x in opciones_xml.Descendants("Opciones") select x.Element("SPAC")).SingleOrDefault();
                var ajma = (from x in opciones_xml.Descendants("Opciones") select x.Element("AJMA")).SingleOrDefault();
                var m_liva = (from x in opciones_xml.Descendants("Opciones") select x.Element("MLIVA")).SingleOrDefault();
                var pac = (from x in opciones_xml.Descendants("Opciones") select x.Element("PAC")).SingleOrDefault();
                var lim_sm = (from x in opciones_xml.Descendants("Opciones") select x.Element("LIM_SM")).SingleOrDefault();
                var fsconfig = (from x in opciones_xml.Descendants("Opciones") select x.Element("FSCONFIG")).SingleOrDefault();
                var tc = (from x in opciones_xml.Descendants("Opciones") select x.Element("TC")).SingleOrDefault();
                var EATCC = (from x in opciones_xml.Descendants("Opciones") select x.Element("EATCC")).SingleOrDefault();
                var EAKG = (from x in opciones_xml.Descendants("Opciones") select x.Element("EAKG")).SingleOrDefault();
                var FACTORY_ALU = (from x in opciones_xml.Descendants("Opciones") select x.Element("FACTORY_ALU")).SingleOrDefault();
                var FACTORY_CRI = (from x in opciones_xml.Descendants("Opciones") select x.Element("FACTORY_CRI")).SingleOrDefault();
                var ANUNCIOS = (from x in opciones_xml.Descendants("Opciones") select x.Element("ANC")).SingleOrDefault();
                var AC_SORT = (from x in opciones_xml.Descendants("Opciones") select x.Element("AC_SORT")).SingleOrDefault();
                var CB_OP = (from x in opciones_xml.Descendants("Opciones") select x.Element("CB_OP")).SingleOrDefault();

                var op1 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP1")).SingleOrDefault();
                var op2 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP2")).SingleOrDefault();
                var op3 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP3")).SingleOrDefault();
                var op4 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP4")).SingleOrDefault();
                var op5 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP5")).SingleOrDefault();
                var op6 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP6")).SingleOrDefault();
                var op7 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP7")).SingleOrDefault();
                var op8 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP8")).SingleOrDefault();
                var op9 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP9")).SingleOrDefault();
                var op10 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP10")).SingleOrDefault();

                if (mv != null)
                {
                    if (mv.Value == "true")
                    {
                        constants.maximizar_ventanas = true;
                    }
                    else
                    {
                        constants.maximizar_ventanas = false;
                    }
                }

                if (opi != null)
                {
                    if (opi.Value == "true")
                    {
                        constants.optimizar_inicio = true;
                    }
                    else
                    {
                        constants.optimizar_inicio = false;
                    }
                }

                if (ma != null)
                {
                    if (ma.Value == "true")
                    {
                        constants.mostrar_acabado = true;
                    }
                    else
                    {
                        constants.mostrar_acabado = false;
                    }
                }

                if (acc != null)
                {
                    if (acc.Value == "true")
                    {
                        constants.ac_cotizacion = true;
                    }
                    else
                    {
                        constants.ac_cotizacion = false;
                    }
                }

                if (ecs != null)
                {
                    if (ecs.Value == "true")
                    {
                        constants.enable_cs = true;
                    }
                    else
                    {
                        constants.enable_cs = false;
                    }
                }

                if (erl != null)
                {
                    if (erl.Value == "true")
                    {
                        constants.enable_rules = true;
                    }
                    else
                    {
                        constants.enable_rules = false;
                    }
                }

                if (ivd != null)
                {
                    if (ivd.Value == "true")
                    {
                        constants.iva_desglosado = true;
                    }
                    else
                    {
                        constants.iva_desglosado = false;
                    }
                }

                if (pai != null)
                {
                    if (pai.Value == "true")
                    {
                        constants.permitir_ajuste_iva = true;
                    }
                    else
                    {
                        constants.permitir_ajuste_iva = false;
                    }
                }

                if (spac != null)
                {
                    if (spac.Value == "true")
                    {
                        constants.siempre_permitir_ac = true;
                    }
                    else
                    {
                        constants.siempre_permitir_ac = false;
                    }
                }

                if (ajma != null)
                {
                    if (ajma.Value == "true")
                    {
                        constants.ajustar_medidas_aut = true;
                    }
                    else
                    {
                        constants.ajustar_medidas_aut = false;
                    }
                }

                if (m_liva != null)
                {
                    if (m_liva.Value == "true")
                    {
                        constants.m_liva = true;
                    }
                    else
                    {
                        constants.m_liva = false;
                    }
                }

                if (pac != null)
                {
                    if (pac.Value == "true")
                    {
                        constants.p_ac = true;
                    }
                    else
                    {
                        constants.p_ac = false;
                    }
                }

                if (lim_sm != null)
                {
                    constants.lim_sm = constants.stringToFloat(lim_sm.Value.ToString());
                }

                if (fsconfig != null)
                {
                    constants.fsconfig = constants.stringToFloat(fsconfig.Value.ToString());
                }

                if (tc != null)
                {
                    constants.tc = constants.stringToFloat(tc.Value.ToString());
                }

                if (EATCC != null)
                {
                    if (EATCC.Value == "true")
                    {
                        constants.enable_c_tc = true;
                    }
                    else
                    {
                        constants.enable_c_tc = false;
                    }
                }

                if (EAKG != null)
                {
                    if (EAKG.Value == "true")
                    {
                        constants.enable_costo_alum_kg = true;
                    }
                    else
                    {
                        constants.enable_costo_alum_kg = false;
                    }
                }

                if (FACTORY_ALU != null)
                {
                    if (FACTORY_ALU.Value.ToString() != string.Empty)
                    {
                        constants.factory_acabado_perfil = FACTORY_ALU.Value.ToString();
                    }
                }

                if (FACTORY_CRI != null)
                {
                    if (FACTORY_CRI.Value.ToString() != string.Empty)
                    {
                        constants.factory_cristal = FACTORY_CRI.Value.ToString();
                    }
                }

                if (ANUNCIOS != null)
                {
                    if (ANUNCIOS.Value == "true")
                    {
                        constants.anuncios = true;
                    }
                    else
                    {
                        constants.anuncios = false;
                    }
                }

                if (AC_SORT != null)
                {
                    if (AC_SORT.Value.ToString() != string.Empty)
                    {
                        constants.ac_sort = AC_SORT.Value.ToString();
                    }                   
                }

                if (CB_OP != null)
                {
                    if (CB_OP.Value == "true")
                    {
                        constants.copybox_option = true;
                    }
                    else
                    {
                        constants.copybox_option = false;
                    }
                }           

                //--------------------------------------------------------------------------------------------------> Reporte

                if (op1 != null)
                {
                    if (op1.Value == "true")
                    {
                        constants.op1 = true;
                    }
                    else
                    {
                        constants.op1 = false;
                    }
                }

                if (op2 != null)
                {
                    if (op2.Value == "true")
                    {
                        constants.op2 = true;
                    }
                    else
                    {
                        constants.op2 = false;
                    }
                }

                if (op3 != null)
                {
                    if (op3.Value == "true")
                    {
                        constants.op3 = true;
                    }
                    else
                    {
                        constants.op3 = false;
                    }
                }

                if (op4 != null)
                {
                    if (op4.Value == "true")
                    {
                        constants.op4 = true;
                    }
                    else
                    {
                        constants.op4 = false;
                    }
                }

                if (op5 != null)
                {
                    if (op5.Value == "true")
                    {
                        constants.op5 = true;
                    }
                    else
                    {
                        constants.op5 = false;
                    }
                }

                if (op6 != null)
                {
                    if (op6.Value == "true")
                    {
                        constants.op6 = true;
                    }
                    else
                    {
                        constants.op6 = false;
                    }
                }

                if (op7 != null)
                {
                    if (op7.Value == "true")
                    {
                        constants.op7 = true;
                    }
                    else
                    {
                        constants.op7 = false;
                    }
                }

                if (op8 != null)
                {
                    if (op8.Value == "true")
                    {
                        constants.op8 = true;
                    }
                    else
                    {
                        constants.op8 = false;
                    }
                }

                if (op9 != null)
                {
                    if (op9.Value == "true")
                    {
                        constants.op9 = true;
                    }
                    else
                    {
                        constants.op9 = false;
                    }
                }

                if (op10 != null)
                {
                    if (op10.Value == "true")
                    {
                        constants.op10 = true;
                    }
                    else
                    {
                        constants.op10 = false;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(this, "[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
                Environment.Exit(0);
            }
        }

        private void loadServerData()
        {
            try
            {
                loadOpcionesData();
                XDocument propiedades_xml = XDocument.Load(constants.propiedades_xml);

                var server = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("SERVER")).SingleOrDefault();
                var db = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("DB")).SingleOrDefault();
                var org = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("ORG")).SingleOrDefault();
                var org_name = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("ORG_N")).SingleOrDefault();
                var mbd = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("MBD")).SingleOrDefault();
                var updater_enable = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("UDE")).SingleOrDefault();
                var updater_form_close = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("UC")).SingleOrDefault();
                var ingreso_ac = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("INGRESO_AC")).SingleOrDefault();
                var user_ac = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("USER_AC")).SingleOrDefault();
                var password_ac = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("PASSWORD_AC")).SingleOrDefault();
                var smtp = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("SMTP")).SingleOrDefault();
                var m_port = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("M_PORT")).SingleOrDefault();
                var timeout = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("TIMEOUT")).SingleOrDefault();

                if (server != null)
                {
                    constants.server = server.Value;
                }

                if (db != null)
                {
                    constants.data_base = db.Value;
                }

                if (org != null)
                {
                    constants.header_reporte = org.Value;
                }

                if (org_name != null)
                {
                    constants.org_name = org_name.Value;
                }

                if (mbd != null)
                {
                    constants.msg_box_caption = mbd.Value;
                }

                if (updater_enable != null)
                {
                    if (updater_enable.Value == "true")
                    {
                        constants.updater_enable = true;
                    }
                    else
                    {
                        constants.updater_enable = false;
                    }
                }

                if (updater_form_close != null)
                {
                    if (updater_form_close.Value == "true")
                    {
                        constants.updater_form_close = true;
                    }
                    else
                    {
                        constants.updater_form_close = false;
                    }
                }

                if (ingreso_ac != null)
                {
                    constants.ingreso_ac = ingreso_ac.Value == "true" ? true : false;
                }

                if (user_ac != null)
                {
                    constants.user_ac = user_ac.Value.ToUpper();
                }

                if (password_ac != null)
                {
                    constants.password_ac = password_ac.Value;
                }

                if (smtp != null)
                {
                    constants.smtp = smtp.Value;
                }

                if (m_port != null)
                {
                    constants.m_port = constants.stringToInt(m_port.Value.ToString());
                }

                if (timeout != null)
                {
                    constants.timeout = constants.stringToInt(timeout.Value.ToString());
                }

                //load porcentajes de articulos
                constants.loadPercentageOfArticulos();
            }
            catch (Exception err)
            {
                MessageBox.Show(this, "[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
                Environment.Exit(0);
            }
        }
    }
}
