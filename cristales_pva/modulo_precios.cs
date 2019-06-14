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
    public partial class modulo_precios : Form
    {
        public modulo_precios(modulo_data md, string clave, string name, string linea, string acabado, string cant, string desp, string flete, string m_o, string util, string tot, string dimensiones, string tot_alum, string tot_herraje, string tot_otros, string tot_cristales, string subtot, string costo_add, string d_1, string d_2, string ubicacion)
        {
            InitializeComponent();
            this.Text = name + " - " + linea;
            ReportDataSource rd = new ReportDataSource("modulo_precios", md.Tables[1]);
            ReportDataSource rd_2 = new ReportDataSource("img_modulo", md.Tables[2]);
            reportViewer1.ZoomMode = ZoomMode.PageWidth;
            reportViewer1.LocalReport.DataSources.Add(rd);
            reportViewer1.LocalReport.DataSources.Add(rd_2);
            reportViewer1.LocalReport.SetParameters(new ReportParameter("modulo_clave", "Clave: " + clave));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("modulo_name", "Nombre: " + name));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("modulo_linea", "Linea: " + linea));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("modulo_acabado", "Acabado: " + acabado));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("cant", cant));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("desp", desp));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("flete", flete));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("m_o", m_o));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("util", util));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("tot", tot));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("dimensiones", dimensiones));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("tot_alum", tot_alum));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("tot_herrajes", tot_herraje));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("tot_otros", tot_otros));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("tot_cristales", tot_cristales));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("subtot", subtot));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("costo_add", costo_add));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("d_1", d_1));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("d_2", d_2));
            if (tot_otros.Contains("+"))
            {
                string[] y = tot_otros.Split(' ');
                if(y.Length > 0)
                {
                    tot_otros = y[0];
                }
            }
            reportViewer1.LocalReport.SetParameters(new ReportParameter("modulo_ubicacion", "Ubicación: " + ubicacion));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("cost_mats", Math.Round(constants.stringToFloat(tot_alum.Replace("$", "")) + constants.stringToFloat(tot_herraje.Replace("$", "")) + constants.stringToFloat(tot_otros.Replace("$", "")) + constants.stringToFloat(tot_cristales.Replace("$", "")), 2).ToString()));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("header", constants.getExternalImage("header")));
            reportViewer1.LocalReport.Refresh();
            reportViewer1.RefreshReport();
        }

        private void modulo_precios_Load(object sender, EventArgs e)
        { 
                    
        }
    }
}
