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

        public merge_items(bool get_merged = false, bool personalizacion = false, int perso_id = -1)
        {
            InitializeComponent();
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
            datagridviewNE1.CellClick += DatagridviewNE1_CellClick;
            datagridviewNE1.CellLeave += DatagridviewNE1_CellLeave;
            contextMenuStrip1.Items[2].Visible = false;
            contextMenuStrip1.Items[3].Visible = false;
            this.get_merged = get_merged;
            this.personalizacion = personalizacion;
            this.perso_id = perso_id;
            if (personalizacion == true)
            {
                constants.getItemsToGetMerged(datagridviewNE1);
                this.Text = "Añadir artículos...";
            }
            else if (get_merged == true)
            {
                constants.getMargedItems(datagridviewNE1, perso_id);
                this.Text = "Artículos de esté concepto...";
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
                        x.Style.BackColor = Color.White;
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
                        x.Style.BackColor = Color.LightGray;
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
                        MessageBox.Show("[Error] este concepto ya tiene demaciados artículos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("[Error] no posible añadir esté artículo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void setImagenPredeterminada(int perso_id)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            var concepto = (from x in cotizaciones.modulos_cotizaciones where x.id == perso_id select x).SingleOrDefault();
            if (concepto != null)
            {
                concepto.pic = constants.imageToByte(Properties.Resources.new_concepto);
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
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).reloadModulos();
                }
                if (Application.OpenForms["edit_expresss"] != null)
                {
                    ((edit_expresss)Application.OpenForms["edit_expresss"]).reloadALL();
                }
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
                ((Form1)Application.OpenForms["Form1"]).setArticuloCotizadoToEdit((int)datagridviewNE1.CurrentRow.Cells[0].Value);
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

        //al lado
        private void alLadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                setNewDir((int)datagridviewNE1.CurrentRow.Cells[0].Value, 2);
            }
        }

        //sobre o por debajo
        private void sobrePorDebajoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                setNewDir((int)datagridviewNE1.CurrentRow.Cells[0].Value, 1);
            }
        }

        //indefinido
        private void indefinidoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                setNewDir((int)datagridviewNE1.CurrentRow.Cells[0].Value, 0);
            }
        }
    }
}
