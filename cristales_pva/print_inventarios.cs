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
    public partial class print_inventarios : Form
    {
        string lista;
        string peps;
        DataTable Table;

        public print_inventarios(DataTable table, string lista, string peps="")
        {
            InitializeComponent();
            this.Table = table;
            this.lista = lista;
            this.peps = peps;
            this.Text = constants.org_name.ToUpper() + " - Existencias";
            reportViewer1.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;           
        }

        private void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            reportViewer1.LocalReport.ReleaseSandboxAppDomain();
            reportViewer1.LocalReport.Dispose();
        }

        private void print_inventarios_Load(object sender, EventArgs e)
        {            
            if (peps == "salidas")
            {
                reportViewer1.LocalReport.ReportEmbeddedResource = "cristales_pva.inventarios_salidas.rdlc";
                this.salidas_tBindingSource.DataSource = Table;
            }
            else if (peps == "entradas")
            {
                reportViewer1.LocalReport.ReportEmbeddedResource = "cristales_pva.inventarios_entradas.rdlc";
                this.entradas_tBindingSource.DataSource = Table;
            }
            else
            {
                this.inventarios_tBindingSource.DataSource = Table;
            }
            //----------------------------------------------------------------------------------------------------------->
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Image", constants.getExternalImage("header")));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("lista", lista));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("tienda", constants.org_name));
            reportViewer1.ZoomMode = ZoomMode.PageWidth;
            ReportPageSettings ps = reportViewer1.LocalReport.GetDefaultPageSettings();
            this.reportViewer1.ParentForm.Width = ps.PaperSize.Width;
            this.reportViewer1.RefreshReport();
        }
    }
}
