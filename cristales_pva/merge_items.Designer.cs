namespace cristales_pva
{
    partial class merge_items
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(merge_items));
            this.datagridviewNE1 = new cristales_pva.datagridviewNE();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eliminarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.añadirAConceptoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removerDeConceptoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configuraciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alLadoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sobrePorDebajoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indefinidoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.datagridviewNE1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // datagridviewNE1
            // 
            this.datagridviewNE1.AllowUserToAddRows = false;
            this.datagridviewNE1.AllowUserToDeleteRows = false;
            this.datagridviewNE1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.datagridviewNE1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.datagridviewNE1.BackgroundColor = System.Drawing.SystemColors.Highlight;
            this.datagridviewNE1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridviewNE1.ContextMenuStrip = this.contextMenuStrip1;
            this.datagridviewNE1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.datagridviewNE1.Location = new System.Drawing.Point(0, 0);
            this.datagridviewNE1.Name = "datagridviewNE1";
            this.datagridviewNE1.ReadOnly = true;
            this.datagridviewNE1.Size = new System.Drawing.Size(1003, 620);
            this.datagridviewNE1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editarToolStripMenuItem,
            this.eliminarToolStripMenuItem,
            this.añadirAConceptoToolStripMenuItem,
            this.removerDeConceptoToolStripMenuItem,
            this.configuraciónToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(193, 136);
            // 
            // editarToolStripMenuItem
            // 
            this.editarToolStripMenuItem.Name = "editarToolStripMenuItem";
            this.editarToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.editarToolStripMenuItem.Text = "Editar";
            this.editarToolStripMenuItem.Click += new System.EventHandler(this.editarToolStripMenuItem_Click);
            // 
            // eliminarToolStripMenuItem
            // 
            this.eliminarToolStripMenuItem.Name = "eliminarToolStripMenuItem";
            this.eliminarToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.eliminarToolStripMenuItem.Text = "Eliminar";
            this.eliminarToolStripMenuItem.Click += new System.EventHandler(this.eliminarToolStripMenuItem_Click);
            // 
            // añadirAConceptoToolStripMenuItem
            // 
            this.añadirAConceptoToolStripMenuItem.Name = "añadirAConceptoToolStripMenuItem";
            this.añadirAConceptoToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.añadirAConceptoToolStripMenuItem.Text = "Añadir a Concepto";
            this.añadirAConceptoToolStripMenuItem.Click += new System.EventHandler(this.añadirAConceptoToolStripMenuItem_Click);
            // 
            // removerDeConceptoToolStripMenuItem
            // 
            this.removerDeConceptoToolStripMenuItem.Name = "removerDeConceptoToolStripMenuItem";
            this.removerDeConceptoToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.removerDeConceptoToolStripMenuItem.Text = "Remover de Concepto";
            this.removerDeConceptoToolStripMenuItem.Click += new System.EventHandler(this.removerDeConceptoToolStripMenuItem_Click);
            // 
            // configuraciónToolStripMenuItem
            // 
            this.configuraciónToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alLadoToolStripMenuItem,
            this.sobrePorDebajoToolStripMenuItem,
            this.indefinidoToolStripMenuItem});
            this.configuraciónToolStripMenuItem.Name = "configuraciónToolStripMenuItem";
            this.configuraciónToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.configuraciónToolStripMenuItem.Text = "Configuración";
            // 
            // alLadoToolStripMenuItem
            // 
            this.alLadoToolStripMenuItem.Name = "alLadoToolStripMenuItem";
            this.alLadoToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.alLadoToolStripMenuItem.Text = "Al Lado";
            this.alLadoToolStripMenuItem.Click += new System.EventHandler(this.alLadoToolStripMenuItem_Click);
            // 
            // sobrePorDebajoToolStripMenuItem
            // 
            this.sobrePorDebajoToolStripMenuItem.Name = "sobrePorDebajoToolStripMenuItem";
            this.sobrePorDebajoToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.sobrePorDebajoToolStripMenuItem.Text = "Sobre / Por Debajo";
            this.sobrePorDebajoToolStripMenuItem.Click += new System.EventHandler(this.sobrePorDebajoToolStripMenuItem_Click);
            // 
            // indefinidoToolStripMenuItem
            // 
            this.indefinidoToolStripMenuItem.Name = "indefinidoToolStripMenuItem";
            this.indefinidoToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.indefinidoToolStripMenuItem.Text = "Indefinido";
            this.indefinidoToolStripMenuItem.Click += new System.EventHandler(this.indefinidoToolStripMenuItem_Click);
            // 
            // merge_items
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 620);
            this.Controls.Add(this.datagridviewNE1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "merge_items";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.datagridviewNE1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private datagridviewNE datagridviewNE1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eliminarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem añadirAConceptoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removerDeConceptoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configuraciónToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alLadoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sobrePorDebajoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indefinidoToolStripMenuItem;
    }
}