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
    public partial class merge_items : Form
    {
        bool get_merged;
        bool personalizacion;
        int perso_id;
        bool reload = false;

        public merge_items(bool get_merged = false, bool personalizacion = false, int perso_id = -1)
        {
            InitializeComponent();
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
            datagridviewNE1.CellClick += DatagridviewNE1_CellClick;
            datagridviewNE1.CellLeave += DatagridviewNE1_CellLeave;
            datagridviewNE1.DataBindingComplete += DatagridviewNE1_DataBindingComplete;
            textBox1.KeyDown += TextBox1_KeyDown;
            contextMenuStrip1.Items[2].Visible = false;
            contextMenuStrip1.Items[3].Visible = false;
            this.FormClosed += Merge_items_FormClosed;
            this.get_merged = get_merged;
            this.personalizacion = personalizacion;
            this.perso_id = perso_id;
            initLoad();
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                initLoad(textBox1.Text);
            }
        }

        public void initLoad(string filter="")
        {
            if (personalizacion == true)
            {
                constants.getItemsToGetMerged(datagridviewNE1, filter);
                this.Text = "Añadir artículos...";
            }
            else if (get_merged == true)
            {
                constants.getMargedItems(datagridviewNE1, perso_id, filter);
                this.Text = "Artículos de esté concepto...";
            }
        }

        public void reloadMergedItems()
        {
            if (get_merged == true)
            {
                constants.getMargedItems(datagridviewNE1, perso_id);
                this.Text = "Artículos de esté concepto...";
            }
        }

        private void Merge_items_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (reload == true)
            {
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL();
                }
            }
        }

        private void DatagridviewNE1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            checkErrorsModulos();
        }

        public void checkErrorsModulos()
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    foreach (int v in constants.errors_Open)
                    {
                        if (v == constants.stringToInt(x.Cells[0].Value.ToString()))
                        {
                            x.Cells[0].Style.BackColor = Color.Red;
                        }
                    }
                }
            }
        }

        private void DatagridviewNE1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                foreach(DataGridViewCell x in datagridviewNE1.CurrentRow.Cells)
                {
                    if(x.OwningColumn.HasDefaultCellStyle == false)
                    {
                        if (x.ColumnIndex != 0)
                        {
                            x.Style.BackColor = Color.White;
                        }                    
                    }
                }
            }
        }

        private void DatagridviewNE1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                foreach (DataGridViewCell x in datagridviewNE1.CurrentRow.Cells)
                {
                    if (x.OwningColumn.HasDefaultCellStyle == false)
                    {
                        if (x.ColumnIndex != 0)
                        {
                            x.Style.BackColor = Color.LightGray;
                        }                  
                    }
                }
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (personalizacion == true)
            {
                contextMenuStrip1.Items[0].Visible = false;
                contextMenuStrip1.Items[1].Visible = false;
                contextMenuStrip1.Items[2].Visible = true;
                contextMenuStrip1.Items[3].Visible = false;
                contextMenuStrip1.Items[4].Visible = false;
            }
            else if (get_merged == true)
            {
                contextMenuStrip1.Items[0].Visible = true;
                contextMenuStrip1.Items[1].Visible = true;
                contextMenuStrip1.Items[2].Visible = false;
                contextMenuStrip1.Items[3].Visible = true;
                contextMenuStrip1.Items[4].Visible = true;
            }
        }

        //Añadir a concepto
        private void añadirAConceptoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                if (datagridviewNE1.CurrentRow.Cells[4].Value.ToString() != "-2")
                {
                    cotizaciones_local cotizaciones = new cotizaciones_local();

                    var modulos = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == perso_id select x).Count();

                    if (modulos < 5)
                    {
                        if (modulos <= 0)
                        {
                            borrarImagenPredeterminada(perso_id);
                        }
                        int id = (int)datagridviewNE1.CurrentRow.Cells[0].Value;
                        setDir dir = new setDir(id, perso_id, constants.byteToImage((byte[])datagridviewNE1.CurrentRow.Cells[1].Value), getMedidas(id, "largo"), getMedidas(id, "alto"));
                        dir.ShowDialog();
                        if (dir.close == true)
                        {
                            constants.getItemsToGetMerged(datagridviewNE1);
                            if (Application.OpenForms["articulos_cotizacion"] != null)
                            {
                                ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).reloadModulos();
                            }
                            if (datagridviewNE1.RowCount <= 0)
                            {
                                this.Close();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "[Error] este concepto ya tiene demaciados artículos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(this, "[Error] no posible añadir esté artículo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private float getMedidas(int id, string dimension)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            var modulos = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();

            if (modulos != null)
            {
                if (dimension == "largo")
                {
                    return (float)modulos.largo;
                }
                else if (dimension == "alto")
                {
                    return (float)modulos.alto;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private void borrarImagenPredeterminada(int perso_id)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            var concepto = (from x in cotizaciones.modulos_cotizaciones where x.id == perso_id select x).SingleOrDefault();
            if (concepto != null)
            {
                concepto.pic = null;
            }
            cotizaciones.SaveChanges();
        }

        //Remover de concepto
        private void removerDeConceptoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                ((Form1)Application.OpenForms["form1"]).setArticuloPersonalizacion((int)datagridviewNE1.CurrentRow.Cells[0].Value, perso_id, 0, true);
                constants.getMargedItems(datagridviewNE1, perso_id);
                //-------------------------------------------------------------------------------------------->
                if (datagridviewNE1.RowCount <= 0)
                {
                    constants.resetModuloPersonalizado(perso_id);
                }
                //Reload
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).reloadModulos();
                }
                if (Application.OpenForms["edit_expresss"] != null)
                {                  
                    ((edit_expresss)Application.OpenForms["edit_expresss"]).reloadALL();
                }
                //Close
                if (datagridviewNE1.RowCount <= 0)
                {
                    this.Close();
                }
            }
        }

        //Editar
        private void editarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                ((Form1)Application.OpenForms["Form1"]).setArticuloCotizadoToEdit((int)datagridviewNE1.CurrentRow.Cells[0].Value, this);
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {
                    Application.OpenForms["articulos_cotizacion"].Close();
                    Application.OpenForms["form1"].Select();
                    if (Application.OpenForms["form1"].WindowState == FormWindowState.Minimized)
                    {
                        Application.OpenForms["form1"].WindowState = FormWindowState.Maximized;
                    }
                }
                this.Close();
            }
        }

        //Eliminar
        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                ((Form1)Application.OpenForms["Form1"]).eliminarArticuloCotizado((int)datagridviewNE1.CurrentRow.Cells[0].Value);
                if (personalizacion == true)
                {
                    constants.getItemsToGetMerged(datagridviewNE1);
                }
                else if (get_merged == true)
                {
                    constants.getMargedItems(datagridviewNE1, perso_id);
                }
                //----------------------------------------------------------------------------------------->
                if (datagridviewNE1.RowCount <= 0)
                {
                    constants.resetModuloPersonalizado(perso_id);
                }
                //Reload
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {                  
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).reloadModulos();
                }
                //Close
                if (datagridviewNE1.RowCount <= 0)
                {
                    this.Close();
                }
            }
        }

        private void setNewDir(int id, int dir)
        {
            cotizaciones_local cotizacion = new cotizaciones_local();

            var modulo = (from x in cotizacion.modulos_cotizaciones where x.id == id select x).SingleOrDefault();

            if(modulo != null)
            {
                modulo.dir = dir;
            }
            cotizacion.SaveChanges();
            constants.updateModuloPersonalizado(perso_id);
            constants.getMargedItems(datagridviewNE1, perso_id);
        }     

        //indefinido
        private void indefinidoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                try
                {
                    int id = (int)datagridviewNE1.CurrentRow.Cells[0].Value;
                    new setDir(id, perso_id, constants.byteToImage((byte[])datagridviewNE1.CurrentRow.Cells[1].Value), getMedidas(id, "largo"), getMedidas(id, "alto"), false, true).ShowDialog();
                }
                catch (Exception err)
                {
                    MessageBox.Show(this, "[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    constants.errorLog(err.ToString());
                }
            }
        }

        //manual
        private void asignaciónManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                new asignar_dir().ShowDialog();
            }
        }

        public void asignacionManual(int dir)
        {
            reload = true;
            setNewDir((int)datagridviewNE1.CurrentRow.Cells[0].Value, dir);
        }

        private void eliminarConfiguraciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "¿Deseas eliminar toda la configuración de integración?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cotizaciones_local cotizacion = new cotizaciones_local();

                var modulos = from x in cotizacion.modulos_cotizaciones where x.merge_id == perso_id select x;

                if(modulos != null)
                {
                    foreach (var x in modulos)
                    {
                        x.dir = 0;
                    }
                    cotizacion.SaveChanges();
                    reloadMergedItems();
                }
            }
        }

        //Buscar
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            initLoad(textBox1.Text);
        }

        //Reload
        private void button1_Click(object sender, EventArgs e)
        {
            initLoad();
        }
    }
}
