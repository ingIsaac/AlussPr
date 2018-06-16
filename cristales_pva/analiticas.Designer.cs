namespace cristales_pva
{
    partial class analiticas
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(analiticas));
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.reportes_dataSet = new cristales_pva.reportes_dataSet();
            this.datos_reporteBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.datos_reporteTableAdapter = new cristales_pva.reportes_dataSetTableAdapters.datos_reporteTableAdapter();
            this.articulos_reporteBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.articulos_reporteTableAdapter = new cristales_pva.reportes_dataSetTableAdapters.articulos_reporteTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.reportes_dataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.datos_reporteBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.articulos_reporteBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "formato_reporte";
            reportDataSource1.Value = this.datos_reporteBindingSource;
            reportDataSource2.Name = "articulos_reporte";
            reportDataSource2.Value = this.articulos_reporteBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "cristales_pva.analiticas.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(871, 502);
            this.reportViewer1.TabIndex = 0;
            // 
            // reportes_dataSet
            // 
            this.reportes_dataSet.DataSetName = "reportes_dataSet";
            this.reportes_dataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // datos_reporteBindingSource
            // 
            this.datos_reporteBindingSource.DataMember = "datos_reporte";
            this.datos_reporteBindingSource.DataSource = this.reportes_dataSet;
            // 
            // datos_reporteTableAdapter
            // 
            this.datos_reporteTableAdapter.ClearBeforeFill = true;
            // 
            // articulos_reporteBindingSource
            // 
            this.articulos_reporteBindingSource.DataMember = "articulos_reporte";
            this.articulos_reporteBindingSource.DataSource = this.reportes_dataSet;
            // 
            // articulos_reporteTableAdapter
            // 
            this.articulos_reporteTableAdapter.ClearBeforeFill = true;
            // 
            // analiticas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(871, 502);
            this.Controls.Add(this.reportViewer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "analiticas";
            this.Text = "Analíticas";
            this.Load += new System.EventHandler(this.analiticas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.reportes_dataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.datos_reporteBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.articulos_reporteBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource datos_reporteBindingSource;
        private reportes_dataSet reportes_dataSet;
        private System.Windows.Forms.BindingSource articulos_reporteBindingSource;
        private reportes_dataSetTableAdapters.datos_reporteTableAdapter datos_reporteTableAdapter;
        private reportes_dataSetTableAdapters.articulos_reporteTableAdapter articulos_reporteTableAdapter;
    }
}