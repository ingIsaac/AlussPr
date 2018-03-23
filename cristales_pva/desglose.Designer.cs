namespace cristales_pva
{
    partial class desglose
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(desglose));
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.reportes_dataSet = new cristales_pva.reportes_dataSet();
            this.materiales_modulosBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.materiales_modulosTableAdapter = new cristales_pva.reportes_dataSetTableAdapters.materiales_modulosTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.reportes_dataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.materiales_modulosBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "materiales_modulo";
            reportDataSource1.Value = this.materiales_modulosBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.EnableExternalImages = true;
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "cristales_pva.desglose.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(893, 538);
            this.reportViewer1.TabIndex = 0;
            // 
            // reportes_dataSet
            // 
            this.reportes_dataSet.DataSetName = "reportes_dataSet";
            this.reportes_dataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // materiales_modulosBindingSource
            // 
            this.materiales_modulosBindingSource.DataMember = "materiales_modulos";
            this.materiales_modulosBindingSource.DataSource = this.reportes_dataSet;
            // 
            // materiales_modulosTableAdapter
            // 
            this.materiales_modulosTableAdapter.ClearBeforeFill = true;
            // 
            // desglose
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 538);
            this.Controls.Add(this.reportViewer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "desglose";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Desglose";
            this.Load += new System.EventHandler(this.desglose_Load);
            ((System.ComponentModel.ISupportInitialize)(this.reportes_dataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.materiales_modulosBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource materiales_modulosBindingSource;
        private reportes_dataSet reportes_dataSet;
        private reportes_dataSetTableAdapters.materiales_modulosTableAdapter materiales_modulosTableAdapter;
    }
}