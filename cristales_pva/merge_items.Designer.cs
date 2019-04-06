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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eliminarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.añadirAConceptoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removerDeConceptoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configuraciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indefinidoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asignaciónManualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eliminarConfiguraciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.datagridviewNE1 = new cristales_pva.datagridviewNE();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.datagridviewNE1)).BeginInit();
            this.SuspendLayout();
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
            this.contextMenuStrip1.Size = new System.Drawing.Size(230, 114);
            // 
            // editarToolStripMenuItem
            // 
            this.editarToolStripMenuItem.Name = "editarToolStripMenuItem";
            this.editarToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.editarToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.editarToolStripMenuItem.Text = "Editar";
            this.editarToolStripMenuItem.Click += new System.EventHandler(this.editarToolStripMenuItem_Click);
            // 
            // eliminarToolStripMenuItem
            // 
            this.eliminarToolStripMenuItem.Name = "eliminarToolStripMenuItem";
            this.eliminarToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.eliminarToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.eliminarToolStripMenuItem.Text = "Eliminar";
            this.eliminarToolStripMenuItem.Click += new System.EventHandler(this.eliminarToolStripMenuItem_Click);
            // 
            // añadirAConceptoToolStripMenuItem
            // 
            this.añadirAConceptoToolStripMenuItem.Name = "añadirAConceptoToolStripMenuItem";
            this.añadirAConceptoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.añadirAConceptoToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.añadirAConceptoToolStripMenuItem.Text = "Añadir a Concepto";
            this.añadirAConceptoToolStripMenuItem.Click += new System.EventHandler(this.añadirAConceptoToolStripMenuItem_Click);
            // 
            // removerDeConceptoToolStripMenuItem
            // 
            this.removerDeConceptoToolStripMenuItem.Name = "removerDeConceptoToolStripMenuItem";
            this.removerDeConceptoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.removerDeConceptoToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.removerDeConceptoToolStripMenuItem.Text = "Remover de Concepto";
            this.removerDeConceptoToolStripMenuItem.Click += new System.EventHandler(this.removerDeConceptoToolStripMenuItem_Click);
            // 
            // configuraciónToolStripMenuItem
            // 
            this.configuraciónToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.indefinidoToolStripMenuItem,
            this.asignaciónManualToolStripMenuItem,
            this.eliminarConfiguraciónToolStripMenuItem});
            this.configuraciónToolStripMenuItem.Name = "configuraciónToolStripMenuItem";
            this.configuraciónToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.configuraciónToolStripMenuItem.Text = "Configuración";
            // 
            // indefinidoToolStripMenuItem
            // 
            this.indefinidoToolStripMenuItem.Name = "indefinidoToolStripMenuItem";
            this.indefinidoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.indefinidoToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.indefinidoToolStripMenuItem.Text = "Cambiar Configuración";
            this.indefinidoToolStripMenuItem.Click += new System.EventHandler(this.indefinidoToolStripMenuItem_Click);
            // 
            // asignaciónManualToolStripMenuItem
            // 
            this.asignaciónManualToolStripMenuItem.Name = "asignaciónManualToolStripMenuItem";
            this.asignaciónManualToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.M)));
            this.asignaciónManualToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.asignaciónManualToolStripMenuItem.Text = "Asignación Manual";
            this.asignaciónManualToolStripMenuItem.Click += new System.EventHandler(this.asignaciónManualToolStripMenuItem_Click);
            // 
            // eliminarConfiguraciónToolStripMenuItem
            // 
            this.eliminarConfiguraciónToolStripMenuItem.Name = "eliminarConfiguraciónToolStripMenuItem";
            this.eliminarConfiguraciónToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.Z)));
            this.eliminarConfiguraciónToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.eliminarConfiguraciónToolStripMenuItem.Text = "Eliminar Configuración";
            this.eliminarConfiguraciónToolStripMenuItem.Click += new System.EventHandler(this.eliminarConfiguraciónToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(598, 535);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(349, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "*Nota: el número de artículos para efectuar una unión esta limitado a (5).";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.Location = new System.Drawing.Point(57, 532);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(359, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // datagridviewNE1
            // 
            this.datagridviewNE1.AllowUserToAddRows = false;
            this.datagridviewNE1.AllowUserToDeleteRows = false;
            this.datagridviewNE1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datagridviewNE1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.datagridviewNE1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.datagridviewNE1.BackgroundColor = System.Drawing.SystemColors.Highlight;
            this.datagridviewNE1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datagridviewNE1.ContextMenuStrip = this.contextMenuStrip1;
            this.datagridviewNE1.Location = new System.Drawing.Point(0, 0);
            this.datagridviewNE1.Name = "datagridviewNE1";
            this.datagridviewNE1.ReadOnly = true;
            this.datagridviewNE1.Size = new System.Drawing.Size(1003, 522);
            this.datagridviewNE1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 535);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Buscar:";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Image = global::cristales_pva.Properties.Resources.Actions_edit_undo_icon;
            this.button1.Location = new System.Drawing.Point(951, 526);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(46, 31);
            this.button1.TabIndex = 4;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // merge_items
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 560);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.datagridviewNE1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "merge_items";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.datagridviewNE1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private datagridviewNE datagridviewNE1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eliminarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem añadirAConceptoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removerDeConceptoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configuraciónToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asignaciónManualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indefinidoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eliminarConfiguraciónToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}