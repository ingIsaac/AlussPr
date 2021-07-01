using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Reporting.WinForms;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Drawing;

namespace cristales_pva
{
    public partial class reportes : Form
    {
        listas_entities_pva listas = new listas_entities_pva();
        List<string> cris_list = new List<string>();
        string c_clave = string.Empty;
        string buffer = string.Empty; 
        bool n_c = false;
        string cristales = string.Empty;
        string cliente = string.Empty;
        string proyecto = string.Empty;
        string folio = string.Empty;
        float subtotal = 0;
        float iva = 0;
        float total = 0;
        float descuento = 0;
        float desc_cant = 0;
        float utilidad = 0;
        bool sort_a_z = true;
        bool load = false;
        float _c = 0;

        public reportes(string cliente, string proyecto, string folio, float subtotal, float iva, float total, float descuento, float desc_cant, float utilidad)
        {
            InitializeComponent();
            textBox4.Leave += TextBox4_Leave;
            this.cliente = cliente;
            this.folio = folio;
            this.proyecto = proyecto;
            this.subtotal = subtotal;
            this.iva = iva;
            this.total = total;
            this.descuento = descuento;
            this.desc_cant = desc_cant;
            this.utilidad = utilidad;
            //--------------->
            textBox4.Enter += TextBox4_Enter;
            textBox5.Enter += TextBox5_Enter;
            textBox5.Leave += TextBox5_Leave;
            //--------------->
            checkBox2.Checked = constants.op1;
            checkBox1.Checked = constants.op2;
            checkBox3.Checked = constants.op3;
            checkBox4.Checked = constants.op4;
            //OP5 ----------->
            checkBox6.Checked = constants.op6;
            checkBox7.Checked = constants.op7;
            checkBox8.Checked = constants.op8;
            checkBox9.Checked = constants.op9;
            checkBox10.Checked = constants.op10;
            checkBox11.Checked = constants.op11;
            load = true;
            label6.Text = "Sub-Folio: " + constants.sub_folio;
            
            //load reporte
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if (cliente != "")
            {
                setData(cliente, proyecto + (constants.getSubfoliotitle(constants.sub_folio) != string.Empty ? (proyecto != string.Empty ? " - " + constants.getSubfoliotitle(constants.sub_folio) : constants.getSubfoliotitle(constants.sub_folio)) : string.Empty), sql.getSingleSQLValue("clientes", "domicilio", "nombre", cliente, 0), sql.getSingleSQLValue("clientes", "telefono", "nombre", cliente, 0), sql.getSingleSQLValue("clientes", "correo_electronico", "nombre", cliente, 0));
            }
            loadReporte(cliente, proyecto, folio, subtotal, iva, total);
            reportViewer1.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;
            //            
        }

        private void TextBox4_Leave(object sender, EventArgs e)
        {
            if (constants.isLong(textBox4.Text) == true)
            {
                if (textBox4.Text.Length == 10)
                {
                    textBox4.Text = string.Format("{0:(###)###-##-##}", Convert.ToInt64(textBox4.Text));
                }
                else if (textBox4.Text.Length == 7)
                {
                    textBox4.Text = string.Format("{0:###-##-##}", Convert.ToInt64(textBox4.Text));
                }
                else if (textBox4.Text.Length == 6)
                {
                    textBox4.Text = string.Format("{0:##-##-##}", Convert.ToInt64(textBox4.Text));
                }
            }


            if (textBox4.Text.Length == 0)
            {
                textBox4.Text = "(669)974-3456";
                textBox4.ForeColor = Color.LightGray;
            }
        }

        private void TextBox5_Leave(object sender, EventArgs e)
        {
            if (textBox5.Text.Length == 0)
            {
                textBox5.Text = "user@mail.com";
                textBox5.ForeColor = Color.LightGray;
            }
        }

        private void TextBox5_Enter(object sender, EventArgs e)
        {
            if (textBox5.ForeColor == Color.LightGray)
            {
                textBox5.Text = string.Empty;
                textBox5.ForeColor = SystemColors.WindowText;
            }
        }

        private void TextBox4_Enter(object sender, EventArgs e)
        {
            if (textBox4.ForeColor == Color.LightGray)
            {
                textBox4.Text = string.Empty;
                textBox4.ForeColor = SystemColors.WindowText;
            }
        }

        private void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            reportViewer1.LocalReport.ReleaseSandboxAppDomain();
            reportViewer1.LocalReport.Dispose();
        }

        private void setData(string cliente, string proyecto, string domicilio, string telefono, string email)
        {
            textBox1.Text = cliente != "" ? cliente : textBox1.Text;
            textBox2.Text = proyecto != "" ? proyecto : textBox2.Text;
            textBox3.Text = domicilio != "" ? domicilio : textBox3.Text;

            if (telefono != "")
            {
                textBox4.Text = telefono;
                textBox4.ForeColor = SystemColors.WindowText;
            }

            if (email != "")
            {
                textBox5.Text = email;
                textBox5.ForeColor = SystemColors.WindowText;
            }
        }

        public string deserializarPrecioSpecial(string precio_especial, int subfolio)
        {
            string r = string.Empty;
            if(precio_especial != string.Empty)
            {
                if (precio_especial.Contains("#") || precio_especial.Contains("&"))
                {
                    string[] u = precio_especial.Split('#');
                    foreach (string v in u)
                    {
                        string[] k = v.Split('&');
                        if (k.Length == 2)
                        {
                            int s_f = constants.stringToInt(k[0]);
                            if(s_f == subfolio)
                            {
                                r = k[1];
                            }
                        }
                    }
                }
                else
                {
                    r = precio_especial;
                }
            }
            return r;
        }

        public void loadReporte(string cliente, string proyecto, string folio, float subtotal, float iva, float total, bool pic=true)
        {
            float _utilidad = 0;
            //Incluir utilidad 
            if (checkBox9.Checked)
            {
                _utilidad = this.utilidad > 0 ? (this.utilidad / 100) + 1 : 1;
            }
            else
            {
                _utilidad = 1;
            }

            //ocultar forma_pago
            string _forma_pago_ext = string.Empty;
            if (!checkBox3.Checked)
            {
                _forma_pago_ext = "_sp";
            }

            //ocultar desglose          
            if (!constants.iva_desglosado)
            {
                checkBox5.Checked = true;
                //---------------------------------------->
                if (checkBox7.Checked)
                {
                    reportViewer1.LocalReport.ReportEmbeddedResource = "cristales_pva.reporte2_c" + _forma_pago_ext + ".rdlc";
                }
                else
                {
                    reportViewer1.LocalReport.ReportEmbeddedResource = "cristales_pva.reporte2" + _forma_pago_ext + ".rdlc";
                }
            }
            else
            {
                checkBox5.Checked = false;
                //---------------------------------------->
                if (checkBox7.Checked)
                {
                    reportViewer1.LocalReport.ReportEmbeddedResource = "cristales_pva.reporte_c" + _forma_pago_ext + ".rdlc";
                }
                else
                {
                    reportViewer1.LocalReport.ReportEmbeddedResource = "cristales_pva.reporte" + _forma_pago_ext + ".rdlc";
                }
            }           
            //------------------------------------------------------------------------------------>

            string[] modelo = null;
            string[] acabado_c = null;
            string display = string.Empty;
            cotizaciones_local cotizaciones = new cotizaciones_local();
            if (proyecto != "" || cliente != "")
            {
                this.Text = cliente + (proyecto != string.Empty ? " - " + proyecto : string.Empty);
            }
            else
            {
                this.Text = "Reporte (n/g).";
            }          
            if(textBox1.Text != string.Empty)
            {
                display = textBox1.Text;
            }
            else
            {
                display = this.Text;
            }
            reportViewer1.LocalReport.DisplayName = display + " - " + (constants.getSubfoliotitle(constants.sub_folio) != string.Empty ? constants.getSubfoliotitle(constants.sub_folio) : constants.sub_folio.ToString());
            reportViewer1.ZoomMode = ZoomMode.PageWidth;
            reportViewer1.LocalReport.EnableExternalImages = true;
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Image", constants.getExternalImage("header")));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("precio_especial", deserializarPrecioSpecial(constants.precio_especial_desc, constants.sub_folio)));
            if (checkBox6.Checked)
            {
                reportViewer1.LocalReport.SetParameters(new ReportParameter("marca", constants.getExternalImage("marca")));
            }
            else
            {
                reportViewer1.LocalReport.SetParameters(new ReportParameter("marca", " "));
            }
            if (descuento > 0 && checkBox4.Checked)
            {
                if (checkBox10.Checked)
                {
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("descuento", "Precio Especial: \n(-" + descuento + "%)"));
                }
                else
                {
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("descuento", "Precio Especial: "));
                }
                reportViewer1.LocalReport.SetParameters(new ReportParameter("desc_cant", "$ " + total.ToString("n")));
                subtotal = this.subtotal + (desc_cant * _utilidad);
                iva = subtotal * (constants.iva - 1);
                total = subtotal + iva;
            }
            else
            {
                reportViewer1.LocalReport.SetParameters(new ReportParameter("descuento", " "));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("desc_cant", " "));
            }
            if (checkBox3.Checked == true)
            {
                reportViewer1.LocalReport.SetParameters(new ReportParameter("formaPago", constants.getFormaPago()));
            }
            else
            {
                reportViewer1.LocalReport.SetParameters(new ReportParameter("formaPago", "n/a"));
            }          

            try
            {   
                cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE datos_reporte");
                cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE articulos_reporte");
                cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (datos_reporte, RESEED, 1)");
                cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (articulos_reporte, RESEED, 1)");

                var reporte = new datos_reporte()
                {
                    Cliente = textBox1.Text,
                    Nombre_Proyecto = textBox2.Text,
                    Fecha = DateTime.Today.ToString("dd/MM/yyyy"),
                    Domicilio = textBox3.Text,
                    Telefono = textBox4.ForeColor == Color.LightGray ? string.Empty : textBox4.Text,
                    email = textBox5.ForeColor == Color.LightGray ? string.Empty : textBox5.Text,
                    Folio = folio,
                    Subtotal = subtotal,
                    IVA = iva,
                    Total = total
                };
                cotizaciones.datos_reporte.Add(reporte);
                cotizaciones.SaveChanges();

                var aluminio = from x in cotizaciones.aluminio_cotizado select x;
                var vidrio = from x in cotizaciones.cristales_cotizados select x;
                var herraje = from x in cotizaciones.herrajes_cotizados select x;
                var otros = from x in cotizaciones.otros_cotizaciones select x;
                var modulos = from x in cotizaciones.modulos_cotizaciones where x.merge_id <= 0 && x.sub_folio == constants.sub_folio orderby x.orden select x;
                
                if (checkBox2.Checked == true)
                {
                    modulos = null;
                    if (sort_a_z == true)
                    {
                        modulos = from x in cotizaciones.modulos_cotizaciones where x.merge_id <= 0 && x.sub_folio == constants.sub_folio orderby x.ubicacion ascending select x;
                    }
                    else
                    {
                        modulos = from x in cotizaciones.modulos_cotizaciones where x.merge_id <= 0 && x.sub_folio == constants.sub_folio orderby x.ubicacion descending select x;
                    }
                }

                string descripcion = string.Empty;
                bool t = false;

                //Reportes Modulos ----------------------------------------------------------------------->
                foreach (var c in modulos)
                {
                    //obtener claves cristales new
                    string[] news = c.news.Split(';');
                    foreach(string k in news)
                    {
                        string[] o = k.Split(',');
                        if(o[0] == "2")
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

                            if (n_c == false && constants.stringToFloat(o[2]) > 0)
                            {
                                cris_list.Add(o[1]);
                            }
                        }
                    }         
                    //obtener claves cristales (no-repetir)
                    foreach (char cri in c.claves_cristales)
                    {
                        if (cri != ',')
                        {
                            if (t)
                            {
                                _c = constants.stringToFloat(cri.ToString());
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

                            if (n_c == false && _c > 0)
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
                    //-------------------------------------------------------------->

                    //------> load image
                    int n_diseño = -1;
                    int m_id = (int)c.modulo_id;
                    var o_modulo = (from g in listas.modulos where g.id == m_id select g).SingleOrDefault();
                    
                    if(o_modulo != null)
                    {
                        int d_id = (int)o_modulo.id_diseño;
                        var o_diseño = (from h in listas.esquemas where h.id == d_id select h).SingleOrDefault();
                        if(o_diseño != null)
                        {
                            n_diseño = o_diseño.id;
                        }
                    }
                    //

                    //Color del anodizado
                    string acabado = c.acabado_perfil;
                    string clr = "";
                    var colores = (from x in listas.colores_aluminio where x.clave == acabado select x).SingleOrDefault();

                    if (colores != null)
                    {
                        clr = " - " + colores.color;
                    }
                    //

                    byte[] p = null;                   

                    if (checkBox1.Checked)
                    {
                        p = c.pic;
                        System.Drawing.Bitmap bm = new System.Drawing.Bitmap(constants.byteToImage(p), 100, 85);
                        p = constants.imageToByte(bm);
                        bm = null;                     
                    }

                    string articulo = string.Empty;
                    string mosquitero = string.Empty;
                    string elevacion = string.Empty;

                    if (checkBox11.Checked)
                    {
                        if (c.articulo != string.Empty)
                        {
                            articulo = "\n-Artículo: " + c.articulo.Replace(" C/M", "").Replace(" S/M", "").Replace("C/M", "").Replace("S/M", "").Replace("*(CS)-", "");
                        }
                    }

                    if (c.articulo.Contains("C/M") == true)
                    {
                        mosquitero = "\nIncluye Mosquitero.";
                    }
                    else if(c.articulo.Contains("S/M") == true)
                    {
                        mosquitero = "\nSin Mosquitero.";
                    }

                    if (acabado != "")
                    {
                        acabado = "\n-Acabado: " + c.acabado_perfil + clr;
                    }
                    else
                    {
                        acabado = "";
                    }

                    if(c.diseño != "CM")
                    {
                        if (cristales.Length > 0)
                        {
                            descripcion = "-Linea: " + c.linea + articulo + acabado + "\n-Cristal:\n" + cristales + "\n-Descripción:\n" + c.descripcion + mosquitero;
                        }
                        else
                        {
                            descripcion = "-Linea: " + c.linea + articulo + acabado + "\n-Descripción:\n" + c.descripcion + mosquitero;
                        }
                    }
                    else
                    {
                        string[] k = c.claves_otros.Split(',');
                        foreach(string r in k)
                        {
                            string[] y = r.Split('-');
                            if(y.Length == 2)
                            {
                                string cl = y[0];
                                float cant = constants.stringToFloat(y[1]);

                                listas_entities_pva listas = new listas_entities_pva();
                                var ot = (from x in listas.otros where x.clave == cl select x).SingleOrDefault();

                                if(ot != null)
                                {
                                    if(ot.linea == "motores" && cant > 0)
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
                        modelo = c.articulo.Split('-');
                        if (modelo.Length > 1)
                        {
                            descripcion = "-Linea: " + c.linea + "\n-Modelo: " + modelo[1] + acabado + "\n-Descripción:\n" + c.descripcion + mosquitero + elevacion;
                        }
                        else
                        {
                            descripcion = "-Linea: " + c.linea + "\n-Modelo: " + c.articulo + acabado + "\n-Descripción:\n" + c.descripcion + mosquitero + elevacion;
                        }
                    }

                    if (c.modulo_id == -2)
                    {
                        descripcion = c.descripcion;
                    }

                    var q_5 = new articulos_reporte()
                    {
                        concepto = c.ubicacion,
                        descripcion = descripcion,
                        largo = Math.Round((float)c.largo / 1000, 2),
                        alto = Math.Round((float)c.alto / 1000, 2),
                        cantidad = c.cantidad,
                        total = c.total * _utilidad,
                        pic = p,
                        desc_p = checkBox8.Checked == true ? "(-" + descuento + "%)\n$" + (Math.Round(((float)c.total * _utilidad) - (((float)c.total * _utilidad) * (descuento / 100)), 2)).ToString("n") : ""
                    };
                    cotizaciones.articulos_reporte.Add(q_5);
                    cris_list.Clear();
                    cristales = string.Empty;
                    descripcion = string.Empty;
                }

                //Reportes aluminio ------------------------------------------------------------------------>
                foreach (var c in aluminio)
                {
                    //color del anodizado
                    string acabado = c.acabado;
                    string clr = "";
                    var colores = (from x in listas.colores_aluminio where x.clave == acabado select x).SingleOrDefault();

                    if(colores != null)
                    {
                        clr = " - " + colores.color;
                    }
                    //

                    string desc = string.Empty;

                    desc = "-Acabado: " + acabado + clr;
                    
                    desc = desc.ToUpper();

                    if (c.descripcion != "")
                    {
                        if (desc.Length > 0)
                        {
                            desc = desc + "\n\n-Descripción:\n" + c.descripcion;
                        }
                        else
                        {
                            desc = "-Descripción:\n" + c.descripcion;
                        }
                    }

                    var q_1 = new articulos_reporte()
                    {
                        concepto = c.articulo,
                        descripcion = desc,
                        largo = Math.Round((float)c.largo_total / 1000, 2),
                        cantidad = c.cantidad,
                        total = c.total * _utilidad,
                        pic = null
                    };
                    cotizaciones.articulos_reporte.Add(q_1);
                }

                //Reportes cristales ----------------------------------------------------------------------->
                foreach (var c in vidrio)
                {
                    descripcion = string.Empty;
                          
                    if(c.filo_muerto > 0)
                    {
                        descripcion = "-filo muerto";
                    }

                    string v = string.Empty;

                    if (c.biselado != "")
                    {
                        acabado_c = c.biselado.Split(',');
                        if (acabado_c.Length > 0)
                        {
                            v = acabado_c[0];
                            var acabados = (from x in listas.acabados where x.clave == v select x).SingleOrDefault();
                            if (acabados != null)
                            {
                                if (descripcion.Length > 0)
                                {
                                    descripcion = descripcion + "\n-" + acabados.acabado1;
                                }
                                else
                                {
                                    descripcion = "-" + acabados.acabado1;
                                }
                            }
                        }
                    }

                    if (c.canteado != "")
                    {
                        acabado_c = c.canteado.Split(',');
                        if (acabado_c.Length > 0)
                        {
                            v = acabado_c[0];
                            var acabados = (from x in listas.acabados where x.clave == v select x).SingleOrDefault();
                            if (acabados != null)
                            {
                                if (descripcion.Length > 0)
                                {
                                    descripcion = descripcion + "\n-" + acabados.acabado1;
                                }
                                else
                                {
                                    descripcion = "-" + acabados.acabado1;
                                }
                            }
                        }
                    }

                    if (c.desconchado != "")
                    {
                        acabado_c = c.desconchado.Split(',');
                        if (acabado_c.Length > 0)
                        {
                            v = acabado_c[0];
                            var acabados = (from x in listas.acabados where x.clave == v select x).SingleOrDefault();
                            if (acabados != null)
                            {
                                if (descripcion.Length > 0)
                                {
                                    descripcion = descripcion + "\n-" + acabados.acabado1;
                                }
                                else
                                {
                                    descripcion = "-" + acabados.acabado1;
                                }
                            }
                        }
                    }

                    if (c.pecho_paloma != "")
                    {
                        acabado_c = c.pecho_paloma.Split(',');
                        if (acabado_c.Length > 0)
                        {
                            v = acabado_c[0];
                            var acabados = (from x in listas.acabados where x.clave == v select x).SingleOrDefault();
                            if (acabados != null)
                            {
                                if (descripcion.Length > 0)
                                {
                                    descripcion = descripcion + "\n-" + acabados.acabado1;
                                }
                                else
                                {
                                    descripcion = "-" + acabados.acabado1;
                                }
                            }
                        }
                    }

                    if (c.esmerilado != "")
                    {
                        acabado_c = c.esmerilado.Split(',');
                        if (acabado_c.Length > 0)
                        {
                            v = acabado_c[0];
                            var acabados = (from x in listas.acabados where x.clave == v select x).SingleOrDefault();
                            if (acabados != null)
                            {
                                if (descripcion.Length > 0)
                                {
                                    descripcion = descripcion + "\n-" + acabados.acabado1;
                                }
                                else
                                {
                                    descripcion = "-" + acabados.acabado1;
                                }
                            }
                        }
                    }

                    if (c.grabado != "")
                    {
                        acabado_c = c.grabado.Split(',');
                        if (acabado_c.Length > 0)
                        {
                            v = acabado_c[0];
                            var acabados = (from x in listas.acabados where x.clave == v select x).SingleOrDefault();
                            if (acabados != null)
                            {
                                if (descripcion.Length > 0)
                                {
                                    descripcion = descripcion + "\n-" + acabados.acabado1;
                                }
                                else
                                {
                                    descripcion = "-" + acabados.acabado1;
                                }
                            }
                        }
                    }

                    if (c.perforado_media_pulgada != "")
                    {
                        acabado_c = c.perforado_media_pulgada.Split(',');
                        if (acabado_c.Length > 0)
                        {
                            v = acabado_c[0];
                            var acabados = (from x in listas.acabados where x.clave == v select x).SingleOrDefault();
                            if (acabados != null)
                            {
                                if (descripcion.Length > 0)
                                {
                                    descripcion = descripcion + "\n-" + acabados.acabado1;
                                }
                                else
                                {
                                    descripcion = "-" + acabados.acabado1;
                                }
                            }
                        }
                    }

                    if (c.perforado_una_pulgada != "")
                    {
                        acabado_c = c.perforado_una_pulgada.Split(',');
                        if (acabado_c.Length > 0)
                        {
                            v = acabado_c[0];
                            var acabados = (from x in listas.acabados where x.clave == v select x).SingleOrDefault();
                            if (acabados != null)
                            {
                                if (descripcion.Length > 0)
                                {
                                    descripcion = descripcion + "\n-" + acabados.acabado1;
                                }
                                else
                                {
                                    descripcion = "-" + acabados.acabado1;
                                }
                            }
                        }
                    }

                    if (c.perforado_dos_pulgadas != "")
                    {
                        acabado_c = c.perforado_dos_pulgadas.Split(',');
                        if (acabado_c.Length > 0)
                        {
                            v = acabado_c[0];
                            var acabados = (from x in listas.acabados where x.clave == v select x).SingleOrDefault();
                            if (acabados != null)
                            {
                                if (descripcion.Length > 0)
                                {
                                    descripcion = descripcion + "\n-" + acabados.acabado1;
                                }
                                else
                                {
                                    descripcion = "-" + acabados.acabado1;
                                }
                            }
                        }
                    }

                    if (c.tipo_venta == "Metro Cuadrado")
                    {
                        if (descripcion.Length > 0)
                        {
                            descripcion = descripcion + "\n-Vidrio/Corte.";
                        }
                        else
                        {
                            descripcion = "-Vidrio/Corte.";
                        }
                    }
                    else if (c.tipo_venta == "Instalado")
                    {
                        if (descripcion.Length > 0)
                        {
                            descripcion = descripcion + "\n-Incluye Instalación.";
                        }
                        else
                        {
                            descripcion = "-Incluye Instalación.";
                        }
                    }
                    else if (c.tipo_venta == "Hoja")
                    {
                        if (descripcion.Length > 0)
                        {
                            descripcion = descripcion + "\n-Hoja Completa.";
                        }
                        else
                        {
                            descripcion = "-Hoja Completa.";
                        }
                    }

                    descripcion = descripcion.ToUpper();

                    if (c.descripcion != "")
                    {
                        if (descripcion.Length > 0)
                        {
                            descripcion = descripcion + "\n\n-Descripción:\n" + c.descripcion;
                        }
                        else
                        {
                            descripcion = "-Descripción:\n" + c.descripcion;
                        }
                    }

                    var q_2 = new articulos_reporte()
                    {
                        concepto = c.articulo,
                        descripcion = descripcion,
                        largo = Math.Round((float)c.largo / 1000, 2),
                        alto = Math.Round((float)c.alto / 1000, 2),
                        cantidad = c.cantidad,
                        total = c.total * _utilidad,
                        pic = null                               
                    };
                    cotizaciones.articulos_reporte.Add(q_2);
                }

                //Reportes Herrajes ---------------------------------------------------------------------->
                foreach (var c in herraje)
                {
                    string desc = string.Empty;

                    if (c.color != "")
                    {
                        desc = "-Color: " + c.color;
                    }

                    desc = desc.ToUpper();

                    if (c.descripcion != "")
                    {
                        if (desc.Length > 0)
                        {
                            desc = desc + "\n\n-Descripción:\n" + c.descripcion;
                        }
                        else
                        {
                            desc = "-Descripción:\n" + c.descripcion;
                        }
                    }

                    var q_3 = new articulos_reporte()
                    {
                        concepto = c.articulo,
                        descripcion = desc,
                        cantidad = c.cantidad,
                        total = c.total * _utilidad,
                        pic = null
                    };
                    cotizaciones.articulos_reporte.Add(q_3);
                }

                //Reportes otros ------------------------------------------------------------------------>
                foreach (var c in otros)
                {
                    string desc = string.Empty;

                    if (c.linea == "concepto-extra")
                    {
                        desc = c.caracteristicas;
                    }
                    else
                    {
                        if(c.color != "")
                        {
                            desc = "-Color: " + c.color;
                        }

                        desc = desc.ToUpper();

                        if (c.descripcion != "")
                        {
                            if (desc.Length > 0)
                            {
                                desc = desc + "\n\n-Descripción:\n" + c.descripcion;
                            }
                            else
                            {
                                desc = "-Descripción:\n" + c.descripcion;
                            }
                        }                       
                    }

                    string clave = c.clave;
                    var lista_otros = (from g in listas.otros where g.clave == clave select g).SingleOrDefault();
                    bool l = false;
                    bool a = false;
                    if(lista_otros != null)
                    {
                       if(lista_otros.largo > 0)
                       {
                            l = true;
                       }
                       if(lista_otros.alto > 0)
                        {
                            a = true;
                        }
                    }

                    var q_4 = new articulos_reporte()
                    {
                        concepto = c.articulo,
                        descripcion = desc,
                        largo = l == true ? Math.Round((float)c.largo / 1000, 2) : 0,
                        alto = a == true ? Math.Round((float)c.alto / 1000, 2) : 0,
                        cantidad = c.cantidad,
                        total = c.total * _utilidad,
                        pic = null
                    };
                    cotizaciones.articulos_reporte.Add(q_4);
                }
               
                cotizaciones.SaveChanges();
                var datos = (from x in cotizaciones.datos_reporte select x);
                var datos_2 = (from x in cotizaciones.articulos_reporte select x);            
                datos_reporteBindingSource.DataSource = datos.ToList();
                articulos_reporteBindingSource.DataSource = datos_2.ToList();                
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            } 
                     
            reportViewer1.LocalReport.Refresh();
            reportViewer1.RefreshReport();

            //Check if utilidad
            if(utilidad > 0 && !checkBox9.Checked)
            {
                DialogResult r = MessageBox.Show("Se esta añadiendo una utilidad al total y no esta habilitada la opción: 'incluir utilidad en partida'.\n\n¿Deseas incluir la utilidad en las partidas dentro del reporte?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(r == DialogResult.Yes)
                {
                    checkBox9.Checked = true;
                }
            }
        }

        private void reportes_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'reportes_dataSet.datos_reporte' table. You can move, or remove it, as needed.
            this.datos_reporteTableAdapter.Fill(this.reportes_dataSet.datos_reporte);
            // TODO: This line of code loads data into the 'reportes_dataSet.articulos_reporte' table. You can move, or remove it, as needed.
            this.articulos_reporteTableAdapter.Fill(this.reportes_dataSet.articulos_reporte);
            ReportPageSettings ps = reportViewer1.LocalReport.GetDefaultPageSettings();
            this.reportViewer1.ParentForm.Width = ps.PaperSize.Width;
            this.reportViewer1.RefreshReport();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (load == true)
            {
                loadReporte(cliente, proyecto, folio, subtotal, iva, total);
                if (checkBox1.Checked == true)
                {
                    setNewOption("OP2", "true");
                    constants.op2 = true;
                }
                else
                {
                    setNewOption("OP2", "false");
                    constants.op2 = false;
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (load == true)
            {
                loadReporte(cliente, proyecto, folio, subtotal, iva, total);
                if (checkBox2.Checked == true)
                {
                    setNewOption("OP1", "true");
                    constants.op1 = true;
                }
                else
                {
                    setNewOption("OP1", "false");
                    constants.op1 = false;
                }
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (load == true)
            {
                loadReporte(cliente, proyecto, folio, subtotal, iva, total);
                if (checkBox3.Checked == true)
                {
                    setNewOption("OP3", "true");
                    constants.op3 = true;
                }
                else
                {
                    setNewOption("OP3", "false");
                    constants.op3 = false;
                }
            }
        }

        //actualizar datos
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult re = MessageBox.Show(this, "¿Deseas actualizar los datos del reporte?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (re == DialogResult.Yes)
            {
                string[] r = ((Form1)Application.OpenForms["form1"]).getDesglose();
                this.cliente = textBox1.Text;
                this.folio = constants.folio_abierto.ToString();
                this.proyecto = textBox2.Text;
                this.subtotal = constants.stringToFloat(r[0]);
                this.iva = constants.stringToFloat(r[1]);
                this.total = constants.stringToFloat(r[2]);
                this.descuento = constants.desc_cotizacion;
                this.desc_cant = constants.desc_cant;
                this.utilidad = constants.utilidad_cotizacion;
                sqlDateBaseManager sql = new sqlDateBaseManager();
                if (cliente != "")
                {
                    setData(cliente, proyecto, textBox3.Text, textBox4.ForeColor == Color.LightGray ? string.Empty : textBox4.Text, textBox5.ForeColor == Color.LightGray ? string.Empty : textBox5.Text);
                }
                loadReporte(cliente, proyecto, folio, subtotal, iva, total);
            }
        }
        //

        private void button2_Click(object sender, EventArgs e)
        {
            sort_a_z = true;
            loadReporte(cliente, proyecto, folio, subtotal, iva, total);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sort_a_z = false;
            loadReporte(cliente, proyecto, folio, subtotal, iva, total);
        }

        private void checkBox4_CheckedChanged_1(object sender, EventArgs e)
        {
            if (load == true)
            {
                loadReporte(cliente, proyecto, folio, subtotal, iva, total);
                if (checkBox4.Checked == true)
                {
                    setNewOption("OP4", "true");
                    constants.op4 = true;
                }
                else
                {
                    setNewOption("OP4", "false");
                    constants.op4 = false;
                }
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (load == true)
            {
                loadReporte(cliente, proyecto, folio, subtotal, iva, total);
                if (checkBox6.Checked == true)
                {
                    setNewOption("OP6", "true");
                    constants.op6 = true;
                }
                else
                {
                    setNewOption("OP6", "false");
                    constants.op6 = false;
                }
            }
        }

        //correo
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show(this, "¿Deseas adjuntar el reporte de cotización a este correo?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            string file = string.Empty;
            if (r == DialogResult.Yes)
            {
                openFileDialog1.ShowDialog();
                file = openFileDialog1.FileName;
                if (Application.OpenForms["mail"] == null)
                {
                    mail mail = new mail(textBox5.Text, file);
                    mail.Show(this);
                    mail.Select();
                }
                else
                {
                    Application.OpenForms["mail"].Select();
                }
            }
            else if(r == DialogResult.No)
            {
                if (Application.OpenForms["mail"] == null)
                {
                    mail mail = new mail(textBox5.Text, file);
                    mail.Show(this);
                    mail.Select();
                }
                else
                {
                    Application.OpenForms["mail"].Select();
                }
            }           
        }
         
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (load == true)
            {
                loadReporte(cliente, proyecto, folio, subtotal, iva, total);
                if(checkBox7.Checked == true)
                {
                    setNewOption("OP7", "true");
                    constants.op7 = true;
                }
                else
                {
                    setNewOption("OP7", "false");
                    constants.op7 = false;
                }
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (load == true)
            {
                loadReporte(cliente, proyecto, folio, subtotal, iva, total);
                if (checkBox8.Checked == true)
                {
                    setNewOption("OP8", "true");
                    constants.op8 = true;
                }
                else
                {
                    setNewOption("OP8", "false");
                    constants.op8 = false;
                }
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (load == true)
            {
                loadReporte(cliente, proyecto, folio, subtotal, iva, total);
                if (checkBox9.Checked == true)
                {
                    setNewOption("OP9", "true");
                    constants.op9 = true;
                }
                else
                {
                    setNewOption("OP9", "false");
                    constants.op9 = false;
                }
            }
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (load == true)
            {
                loadReporte(cliente, proyecto, folio, subtotal, iva, total);
                if (checkBox10.Checked == true)
                {
                    setNewOption("OP10", "true");
                    constants.op10 = true;
                }
                else
                {
                    setNewOption("OP10", "false");
                    constants.op10 = false;
                }
            }
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (load == true)
            {
                loadReporte(cliente, proyecto, folio, subtotal, iva, total);
                if (checkBox11.Checked == true)
                {
                    setNewOption("OP11", "true");
                    constants.op11 = true;
                }
                else
                {
                    setNewOption("OP11", "false");
                    constants.op11 = false;
                }
            }
        }

        public void reload()
        {
            loadReporte(cliente, proyecto, folio, subtotal, iva, total);
        }

        private void setNewOption(string opcion, string v)
        {
            try
            {
                XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                var mv = from x in opciones_xml.Descendants("Opciones") select x;

                foreach (XElement x in mv)
                {
                    x.SetElementValue(opcion, v);
                }
                opciones_xml.Save(constants.opciones_xml);
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Forma de pago
        private void button5_Click(object sender, EventArgs e)
        {
            new forma_pago().ShowDialog();
        }

        //Precio Especial
        private void button6_Click(object sender, EventArgs e)
        {
            new precio_especial().ShowDialog();
        }
    }
}
