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
    public partial class modulo_data_form : Form
    {
        public modulo_data_form(modulo_data md, string clave, string name, string linea, string dimensiones, string autor)
        {
            InitializeComponent();
            this.Text = name + " - " + linea;
            ReportDataSource rd = new ReportDataSource("modulo_data", md.Tables[0]);
            ReportDataSource rd_2 = new ReportDataSource("img_modulo", md.Tables[2]);
            string display = string.Empty;
            if(clave != string.Empty)
            {
                display = clave;
            }
            //------------------------>
            if(name != string.Empty)
            {
                display = display + " - " + name;
            }
            //------------------------>
            if (linea != string.Empty)
            {
                display = display + " - " + linea;
            }
            //------------------------>
            if (display == string.Empty)
            {
                display = "n/a";
            }
            reportViewer1.LocalReport.DisplayName = display;
            reportViewer1.ZoomMode = ZoomMode.PageWidth;
            reportViewer1.LocalReport.DataSources.Add(rd);
            reportViewer1.LocalReport.DataSources.Add(rd_2);
            reportViewer1.LocalReport.SetParameters(new ReportParameter("module_clave", "Clave: " + clave));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("module_name", "Nombre: " + name));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("module_linea", "Linea: " + linea));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("dimensiones", dimensiones));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("autor", "Autor: " + autor));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("header", constants.getExternalImage("header")));
            reportViewer1.LocalReport.Refresh();
            reportViewer1.RefreshReport();
        }

        private void modulo_data_form_Load(object sender, EventArgs e)
        {
            
        }
    }
}
