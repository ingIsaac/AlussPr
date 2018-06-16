using Microsoft.Reporting.WinForms;
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
    public partial class analiticas : Form
    {
        string cliente = string.Empty;
        string proyecto = string.Empty;
        string folio = string.Empty;
        float subtotal = 0;
        float iva = 0;
        float total = 0;
        float descuento = 0;
        float desc_cant = 0;
        float utilidad = 0;
        float gasto_total = 0;
        float utilidad_total = 0;
        float costo_materiales = 0;

        public analiticas(string cliente, string proyecto, string folio, float subtotal, float iva, float total, float descuento, float desc_cant, float utilidad)
        {
            InitializeComponent();
            reportViewer1.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;
            reportViewer1.LocalReport.EnableExternalImages = true;
            reportViewer1.ZoomMode = ZoomMode.PageWidth;
            this.cliente = cliente;
            this.folio = folio;
            this.proyecto = proyecto;
            this.subtotal = subtotal;
            this.iva = iva;
            this.total = total;
            this.descuento = descuento;
            this.desc_cant = desc_cant;
            this.utilidad = utilidad;
            this.Text = this.Text + " - Sub-Folio: " + constants.sub_folio;
            loadAnaliticas();
        }

        private void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            reportViewer1.LocalReport.ReleaseSandboxAppDomain();
            reportViewer1.LocalReport.Dispose();
        }

        private void analiticas_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'reportes_dataSet.datos_reporte' table. You can move, or remove it, as needed.
            this.datos_reporteTableAdapter.Fill(this.reportes_dataSet.datos_reporte);
            // TODO: This line of code loads data into the 'reportes_dataSet.articulos_reporte' table. You can move, or remove it, as needed.
            this.articulos_reporteTableAdapter.Fill(this.reportes_dataSet.articulos_reporte);
            ReportPageSettings ps = reportViewer1.LocalReport.GetDefaultPageSettings();
            this.reportViewer1.ParentForm.Width = ps.PaperSize.Width;
            this.reportViewer1.RefreshReport();
        }

        private void loadAnaliticas()
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Image", constants.getExternalImage("header")));

            if (utilidad > 0)
            {
                reportViewer1.LocalReport.SetParameters(new ReportParameter("u_label", "+ Utilidad: " + utilidad + "%"));
            }
            else
            {
                reportViewer1.LocalReport.SetParameters(new ReportParameter("u_label", " "));
            }

            try
            {
                float _utilidad = 0;               
                float _total = 0;
                float _total_desc = 0;
                float _sub_total = this.subtotal;
                
                //Incluir utilidad 
                if (constants.op9)
                {
                    _utilidad = this.utilidad > 0 ? (this.utilidad / 100) + 1 : 1;
                }
                else
                {
                    _utilidad = 1;
                }

                cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE datos_reporte");
                cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE articulos_reporte");
                cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (datos_reporte, RESEED, 1)");
                cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (articulos_reporte, RESEED, 1)");

                if (descuento > 0)
                {
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("desc_n", descuento.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("desc_cant", desc_cant.ToString()));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("descuento", "Descuento: (-" + descuento + "%)"));
                    iva = subtotal * (constants.iva - 1);
                    total = subtotal + iva;
                }
                else
                {
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("desc_cant", "0"));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("desc_n", "0"));
                    reportViewer1.LocalReport.SetParameters(new ReportParameter("descuento", " "));
                }

                var reporte = new datos_reporte()
                {
                    Cliente = cliente,
                    Nombre_Proyecto = proyecto,
                    Fecha = DateTime.Today.ToString("dd/MM/yyyy"),
                    Folio = folio,
                    Subtotal = subtotal,
                    IVA = iva,
                    Total = total
                };
                cotizaciones.datos_reporte.Add(reporte);
                cotizaciones.SaveChanges();           

                var modulos = from x in cotizaciones.modulos_cotizaciones where x.merge_id <= 0 && x.sub_folio == constants.sub_folio orderby x.orden select x;
                byte[] p = null;               
                int s = 0;
                float desperdicio = 0;
                float flete = 0;
                float mano_o = 0;
                float utilidad = 0;
                float _t = 0;

                foreach (var c in modulos)
                {
                    if (c.pic != null)
                    {
                        p = c.pic;
                        Bitmap bm = new Bitmap(constants.byteToImage(p), 100, 85);
                        p = constants.imageToByte(bm);
                        bm = null;
                    }                  

                    s = 0;
                    desperdicio = 0;
                    flete = 0;
                    mano_o = 0;
                    utilidad = 0;

                    if (c.modulo_id == -1)
                    {               
                        int n = c.id;
                        var k = from x in cotizaciones.modulos_cotizaciones where x.merge_id == n select x;
                        foreach(var j in k)
                        {
                            s++;
                            desperdicio = (float)j.desperdicio + desperdicio;
                            flete = (float)j.flete + flete;
                            mano_o = (float)j.mano_obra + mano_o;
                            utilidad = (float)j.utilidad + utilidad;
                        }
                    }

                    if (s > 0)
                    {
                        desperdicio = desperdicio / s;
                        flete = flete / s;
                        mano_o = mano_o / s;
                        utilidad = utilidad / s;
                    }

                    _total = (float)c.total;
                    _total_desc = ((_total) - ((_total) * (descuento / 100)));

                    if (c.modulo_id != -2)
                    {
                        if (utilidad != 0)
                        {
                            utilidad_total = utilidad_total + (_total - (_total / ((utilidad / 100) + 1)));
                            gasto_total = gasto_total + (_total / ((utilidad / 100) + 1));
                        }
                        else
                        {
                            utilidad_total = utilidad_total + (float)(_total - (_total / ((c.utilidad / 100) + 1)));
                            gasto_total = gasto_total + (float)(_total / ((c.utilidad / 100) + 1));
                        }
                        if (c.modulo_id != -1)
                        {
                            _t = (float)(_total / ((c.utilidad / 100) + 1));
                            _t = (float)(_t / ((c.mano_obra / 100) + 1));
                            _t = (float)(_t / ((c.flete / 100) + 1));
                            _t = (float)(_t / ((c.desperdicio / 100) + 1));
                            costo_materiales = costo_materiales + _t;
                        }
                        else
                        {
                            int q = (int)c.id;
                            var u = from x in cotizaciones.modulos_cotizaciones where x.merge_id == q select x;

                            if (u != null)
                            {
                                foreach (var x in u)
                                {
                                    _t = (float)(x.total / ((x.utilidad / 100) + 1));
                                    _t = (float)(_t / ((x.mano_obra / 100) + 1));
                                    _t = (float)(_t / ((x.flete / 100) + 1));
                                    _t = (float)(_t / ((x.desperdicio / 100) + 1));
                                    costo_materiales = costo_materiales + _t;
                                }
                            }
                        }                     
                    }

                    var v = new articulos_reporte()
                    {
                        concepto = c.ubicacion + " - " + c.linea + " - " + c.acabado_perfil,
                        largo = Math.Round((float)c.largo / 1000, 2),
                        alto = Math.Round((float)c.alto / 1000, 2),
                        cantidad = c.cantidad,
                        total = _total,
                        pic = p,
                        desp = desperdicio != 0 ? desperdicio.ToString() : c.desperdicio.ToString(),
                        flete = flete != 0 ? flete.ToString() : c.flete.ToString(),
                        mano_obra = mano_o != 0 ? mano_o.ToString() : c.mano_obra.ToString(),
                        utilidad = utilidad != 0 ? utilidad.ToString() : c.utilidad.ToString(),
                        desc_p = descuento > 0 ? "(-" + descuento + "%)\n$" + _total_desc.ToString("n") : ""
                    };
                    cotizaciones.articulos_reporte.Add(v);
                }
                cotizaciones.SaveChanges();

                utilidad_total = utilidad_total + (_sub_total * (this.utilidad / 100));                                               

                reportViewer1.LocalReport.SetParameters(new ReportParameter("gasto_total", Math.Round(gasto_total, 2).ToString()));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("utilidad_total", Math.Round(utilidad_total, 2).ToString()));
                reportViewer1.LocalReport.SetParameters(new ReportParameter("costo_mats", Math.Round(costo_materiales, 2).ToString()));

                var datos = (from x in cotizaciones.datos_reporte select x);
                var datos_2 = (from x in cotizaciones.articulos_reporte select x);
                datos_reporteBindingSource.DataSource = datos.ToList();
                articulos_reporteBindingSource.DataSource = datos_2.ToList();
            }
            catch (Exception err) { MessageBox.Show(err.ToString()); }
        }
    }
}
