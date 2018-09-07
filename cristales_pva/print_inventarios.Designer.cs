namespace cristales_pva
{
    partial class print_inventarios
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
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource3 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.inventarios_tBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.reportes_dataSet = new cristales_pva.reportes_dataSet();
            this.entradas_tBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.salidas_tBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.inventarios_tBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportes_dataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.entradas_tBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.salidas_tBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "inventarios_t";
            reportDataSource1.Value = this.inventarios_tBindingSource;
            reportDataSource2.Name = "entradas_t";
            reportDataSource2.Value = this.entradas_tBindingSource;
            reportDataSource3.Name = "salidas_t";
            reportDataSource3.Value = this.salidas_tBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource3);
            this.reportViewer1.LocalReport.EnableExternalImages = true;
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "cristales_pva.inventarios.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(924, 505);
            this.reportViewer1.TabIndex = 0;
            // 
            // inventarios_tBindingSource
            // 
            this.inventarios_tBindingSource.DataMember = "inventarios_t";
            this.inventarios_tBindingSource.DataSource = this.reportes_dataSet;
            // 
            // reportes_dataSet
            // 
            this.reportes_dataSet.DataSetName = "reportes_dataSet";
            this.reportes_dataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // entradas_tBindingSource
            // 
            this.entradas_tBindingSource.DataMember = "entradas_t";
            this.entradas_tBindingSource.DataSource = this.reportes_dataSet;
            // 
            // salidas_tBindingSource
            // 
            this.salidas_tBindingSource.DataMember = "salidas_t";
            this.salidas_tBindingSource.DataSource = this.reportes_dataSet;
            // 
            // print_inventarios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 505);
            this.Controls.Add(this.reportViewer1);
            this.Name = "print_inventarios";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.print_inventarios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.inventarios_tBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportes_dataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.entradas_tBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.salidas_tBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource inventarios_tBindingSource;
        private reportes_dataSet reportes_dataSet;
        private System.Windows.Forms.BindingSource entradas_tBindingSource;
        private System.Windows.Forms.BindingSource salidas_tBindingSource;
    }
}