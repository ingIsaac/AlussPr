﻿using System;
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
    public partial class desglose : Form
    {
        string factor = string.Empty;
        string titulo = string.Empty;

        public desglose(string factor, string titulo)
        {
            InitializeComponent();
            reportViewer1.ZoomMode = ZoomMode.PageWidth;
            reportViewer1.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;
            this.factor = factor;
            this.titulo = titulo;
        }

        private void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            reportViewer1.LocalReport.ReleaseSandboxAppDomain();
            reportViewer1.LocalReport.Dispose();
        }

        private void desglose_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'reportes_dataSet.materiales_modulos' table. You can move, or remove it, as needed.
            this.materiales_modulosTableAdapter.Fill(this.reportes_dataSet.materiales_modulos);
            string display_n = string.Empty;
            if(constants.nombre_cotizacion != string.Empty)
            {
                display_n = " - " + constants.nombre_cotizacion;
            }          
            //---------->
            if (constants.nombre_proyecto != string.Empty)
            {
                display_n = display_n + " - " + constants.nombre_proyecto;
            }
            //---------->
            if (titulo != string.Empty)
            {
                display_n = display_n + " - " + titulo;
            }
            //---------->
            reportViewer1.LocalReport.DisplayName = "Desglose" + display_n;
            reportViewer1.LocalReport.SetParameters(new ReportParameter("header", constants.getExternalImage("header")));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("cliente", constants.nombre_cotizacion == "" ? "n/a" : constants.nombre_cotizacion));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("proyecto", constants.nombre_proyecto == "" ? "n/a" : constants.nombre_proyecto));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("folio", constants.folio_abierto.ToString()));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("factor", "x" + factor));
            reportViewer1.LocalReport.SetParameters(new ReportParameter("titulo", titulo));
            ReportPageSettings ps = reportViewer1.LocalReport.GetDefaultPageSettings();
            this.reportViewer1.ParentForm.Width = ps.PaperSize.Width;
            this.reportViewer1.RefreshReport();
        }
    }
}
