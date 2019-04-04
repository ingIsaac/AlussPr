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
    public partial class edit_expresss : Form
    {
        cotizaciones_local cotizaciones;
        int merged_id = -1;
        int id;
        Image img = null;

        public edit_expresss()
        {
            InitializeComponent();        
            datagridviewNE1.CellClick += DatagridviewNE1_CellClick;
            datagridviewNE1.MouseMove += DatagridviewNE1_MouseMove;
            datagridviewNE1.MouseDown += DatagridviewNE1_MouseDown;
            datagridviewNE2.CellClick += DatagridviewNE2_CellClick;
            datagridviewNE2.DragDrop += DatagridviewNE2_DragDrop;
            datagridviewNE2.DragOver += DatagridviewNE2_DragOver;
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
            contextMenuStrip2.Opening += ContextMenuStrip2_Opening;
            cargarModulosCotizados();
            label4.Text = "Sub-Folio: " + constants.sub_folio;
            if (Application.OpenForms["articulos_cotizacion"] != null)
            {
                Application.OpenForms["articulos_cotizacion"].Close();
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

        private void añadirConcepto(int id)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
                     
            var modulos = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == merged_id select x).Count();

            if (modulos < 5)
            {
                if (datagridviewNE2.Rows.Count <= 0)
                {
                    borrarImagenPredeterminada(merged_id);
                }
                setDir dir = new setDir(id, merged_id, img, getMedidas(id, "largo"), getMedidas(id, "alto"));
                dir.ShowDialog();
                if (dir.close == true)
                {
                    cargarMergedItems(merged_id);
                    cargarModulosCotizados();
                }
            }
            else
            {
                MessageBox.Show("[Error] este concepto ya tiene demaciados artículos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }              
        }

        private float getMedidas(int id, string dimension)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            var modulos = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();

            if(modulos != null)
            {
                if(dimension == "largo")
                {
                    return (float)modulos.largo;
                }
                else if(dimension == "alto")
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

        private void DatagridviewNE1_MouseDown(object sender, MouseEventArgs e)
        {
            if(datagridviewNE1.RowCount > 0)
            { 
                id = 0;
                if ((int)datagridviewNE1.CurrentRow.Cells[3].Value > 0)
                {
                    id = (int)datagridviewNE1.CurrentRow.Cells[0].Value;
                    img = constants.byteToImage((Byte[])datagridviewNE1.CurrentRow.Cells[1].Value);
                }
            }
        }       

        private void DatagridviewNE1_MouseMove(object sender, MouseEventArgs e)
        {           
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (id > 0)
                {
                    DragDropEffects dropEffect = datagridviewNE1.DoDragDrop(id, DragDropEffects.Move);
                }              
            }                       
        }

        private void DatagridviewNE2_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;       
        }

        private void DatagridviewNE2_DragDrop(object sender, DragEventArgs e)
        {          
            if (e.Effect == DragDropEffects.Move)
            {
                if (datagridviewNE2.ColumnCount > 0)
                {
                    añadirConcepto(id);
                }
            }          
        }

        private void ContextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if(datagridviewNE2.RowCount <= 0)
            {
                e.Cancel = true;
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
           if(datagridviewNE1.RowCount > 0)
            {
                contextMenuStrip1.Items[0].Visible = true;
                if(datagridviewNE1.CurrentRow.Cells[3].Value.ToString() == "-1")
                {
                    contextMenuStrip1.Items[2].Visible = true;
                }
                else
                {
                    contextMenuStrip1.Items[2].Visible = false;
                }
            }
            else
            {
                contextMenuStrip1.Items[0].Visible = false;
                contextMenuStrip1.Items[2].Visible = false;
            }
        }

        private void DatagridviewNE2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(datagridviewNE2.RowCount > 0)
            {
                if (Application.OpenForms["config_modulo"] != null)
                {
                    ((config_modulo)Application.OpenForms["config_modulo"]).resetSession((int)datagridviewNE2.CurrentRow.Cells[2].Value, (int)datagridviewNE2.CurrentRow.Cells[0].Value);                   
                }
            }
        }

        private void DatagridviewNE1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                if(datagridviewNE1.CurrentRow.Cells[3].Value.ToString() == "-1")
                {
                    pictureBox2.Image = Properties.Resources.Forward_icon;
                    cargarMergedItems((int)datagridviewNE1.CurrentRow.Cells[0].Value);
                }
                else
                {
                    pictureBox2.Image = null;
                    label3.Text = "";
                    label6.Text = "";
                    label7.Text = "";
                    if (Application.OpenForms["config_modulo"] != null)
                    {
                        ((config_modulo)Application.OpenForms["config_modulo"]).resetSession((int)datagridviewNE1.CurrentRow.Cells[3].Value, (int)datagridviewNE1.CurrentRow.Cells[0].Value);
                    }
                }
            }
        }

        public void reloadALL(int id = -1)
        {
            cargarModulosCotizados();
            datagridviewNE2.Rows.Clear();
            if(id > 0)
            {
                foreach(DataGridViewRow x in datagridviewNE1.Rows)
                {
                    if ((int)x.Cells[0].Value == id)
                    {
                        datagridviewNE1.FirstDisplayedScrollingRowIndex = x.Index;
                        x.Selected = true;
                    }
                }
            }
        }

        //cargar articulos cotizados
        private void cargarModulosCotizados()
        {
            if (datagridviewNE1.RowCount > 0)
            {
                datagridviewNE1.Rows.Clear();
            }
            if (datagridviewNE1.ColumnCount > 0)
            {
                datagridviewNE1.Columns.Clear();
            }
            datagridviewNE1.Refresh();
            DataGridViewImageColumn clm = new DataGridViewImageColumn();
            clm.Name = "CS";
            clm.HeaderText = "CS";
            datagridviewNE1.Columns.Add("Id", "Id");
            datagridviewNE1.Columns.Add(clm);
            datagridviewNE1.Columns.Add("Ubicación", "Ubicación");
            datagridviewNE1.Columns.Add("Módulo_Id", "Módulo_Id");
            datagridviewNE1.Columns[2].DefaultCellStyle.BackColor = Color.LightGreen;
            datagridviewNE1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            datagridviewNE1.Columns[2].DefaultCellStyle.Font = new Font("Arial", 12f, FontStyle.Bold);
            datagridviewNE1.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            cotizaciones = new cotizaciones_local();
            var data = (from x in cotizaciones.modulos_cotizaciones where x.merge_id <= 0 && x.modulo_id != -2 && x.sub_folio == constants.sub_folio select x);
            float sum = 0;
            if (textBox1.Text != "")
            {
                string param = textBox1.Text;
                data = null;
                data = (from x in cotizaciones.modulos_cotizaciones where x.merge_id <= 0 && x.sub_folio == constants.sub_folio && (x.clave.StartsWith(param) || x.articulo.StartsWith(param) || x.ubicacion.StartsWith(param)) select x);
            }
            foreach (var c in data)
            {
                if (c != null)
                {
                    if(c.pic == null)
                    {
                        c.pic = constants.imageToByte(Properties.Resources.new_concepto);
                    }
                    datagridviewNE1.Rows.Add(c.id, c.pic, c.ubicacion, c.modulo_id);
                    sum = (sum + (float)c.total);
                }
            }
            if (datagridviewNE1.RowCount > 0)
            {
                datagridviewNE1.FirstDisplayedScrollingRowIndex = datagridviewNE1.RowCount - 1;
            }
            sum = sum * constants.iva;
            label5.Text = "$ " + sum.ToString("n"); ;
            label2.Text = "Se encontrarón: " + datagridviewNE1.RowCount + " partidas.";
            getUnificados();        
        }

        //unificacion
        private void getUnificados()
        {
            foreach(DataGridViewRow x in datagridviewNE1.Rows)
            {
                if((int)x.Cells[3].Value == -1)
                {
                    x.DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }
        }

        //Cargar merged items
        private void cargarMergedItems(int id)
        {
            merged_id = id;
            if (datagridviewNE2.RowCount > 0)
            {
                datagridviewNE2.Rows.Clear();
            }
            if (datagridviewNE2.ColumnCount > 0)
            {
                datagridviewNE2.Columns.Clear();
            }
            datagridviewNE2.Refresh();
            DataGridViewImageColumn clm = new DataGridViewImageColumn();
            clm.Name = "CS";
            clm.HeaderText = "CS";
            datagridviewNE2.Columns.Add("Id", "Id");
            datagridviewNE2.Columns.Add(clm);
            datagridviewNE2.Columns.Add("Módulo_Id", "Módulo_Id");
            cotizaciones = new cotizaciones_local();
            var data = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == id && x.sub_folio == constants.sub_folio select x);
            float sum = 0;  
            foreach (var c in data)
            {
                if (c != null)
                {
                    datagridviewNE2.Rows.Add(c.id, c.pic, c.modulo_id);
                    sum = (sum + (float)c.total);
                }
            }
            sum = sum * constants.iva;
            label3.Text = "$ " + sum.ToString("n");

            var modulo = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();
            if (modulo != null)
            {
                label6.Text = "Largo: " + modulo.largo.ToString();
                label7.Text = "Alto: " + modulo.alto.ToString();
            }

        }

        //Buscar
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            cargarModulosCotizados();
        }       

        //Eliminar
        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                ((Form1)Application.OpenForms["Form1"]).eliminarArticuloCotizado((int)datagridviewNE1.CurrentRow.Cells[0].Value);
                datagridviewNE2.Rows.Clear();
                cargarModulosCotizados();
            }
        }

        //Unificar Conceptos
        private void unificarAConceptosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            try
            {
                var modulo_p = new modulos_cotizaciones
                {
                    folio = 00000,
                    modulo_id = -1,
                    descripcion = "",
                    mano_obra = 0,
                    dimensiones = "",
                    largo = 0,
                    alto = 0,
                    acabado_perfil = "",
                    claves_cristales = "",
                    cantidad = 1,
                    flete = 0,
                    desperdicio = 0,
                    utilidad = 0,
                    articulo = "",
                    linea = "",
                    diseño = "",
                    clave = "md",
                    total = 0,
                    claves_otros = "",
                    claves_herrajes = "",
                    ubicacion = "",
                    claves_perfiles = "",
                    pic = constants.imageToByte(Properties.Resources.new_concepto),
                    merge_id = -1,
                    concept_id = 0,
                    sub_folio = constants.sub_folio,
                    dir = 0,
                    news = "",
                    new_desing = "",
                    orden = 0
                };
                cotizaciones.modulos_cotizaciones.Add(modulo_p);
                cotizaciones.SaveChanges();
                ((Form1)Application.OpenForms["Form1"]).countCotizacionesArticulo();
                ((Form1)Application.OpenForms["Form1"]).loadCountArticulos();
                cargarModulosCotizados();              
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }       

        //Eliminar desde marged items
        private void eliminarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (datagridviewNE2.RowCount > 0)
            {
                ((Form1)Application.OpenForms["Form1"]).eliminarArticuloCotizado((int)datagridviewNE2.CurrentRow.Cells[0].Value);
                cargarMergedItems(merged_id);
                cargarModulosCotizados();
            }
        }

        //Remover de concepto
        private void removerDeConceptoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE2.RowCount > 0)
            {
                ((Form1)Application.OpenForms["form1"]).setArticuloPersonalizacion((int)datagridviewNE2.CurrentRow.Cells[0].Value, merged_id, 0, true);
                cargarMergedItems(merged_id);
                cargarModulosCotizados();
            }
        }

        //Cambiar esquema
        private void cambiarEsquemaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                if (Application.OpenForms["cambiar_imagen"] == null)
                {
                    new cambiar_imagen((int)datagridviewNE1.CurrentRow.Cells[0].Value).ShowDialog();
                    ((Form1)Application.OpenForms["form1"]).refreshNewArticulo(5);
                }
            }
        }
        //---------------------------------------------------->
    }
}
