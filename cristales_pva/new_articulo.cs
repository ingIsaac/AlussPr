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
    public partial class new_articulo : Form
    {
        List<string> lista;

        public new_articulo(List<string> lista)
        {
            InitializeComponent();
            datagridviewNE1.CellClick += DatagridviewNE1_CellClick;
            datagridviewNE1.CellLeave += DatagridviewNE1_CellLeave;
            this.lista = lista;
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if(datagridviewNE1.RowCount <= 0)
            {
                e.Cancel = true;
            }
        }

        private void DatagridviewNE1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                datagridviewNE1.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
        }

        private void DatagridviewNE1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                datagridviewNE1.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
            }
        }

        //cargar filtros
        public void setFiltros()
        {
            if (comboBox1.SelectedIndex == 0)
            {
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                comboBox2.Items.AddRange(constants.getProveedores("vidrio").ToArray());
                comboBox3.Items.AddRange(constants.getCategorias("vidrio").ToArray());
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                comboBox2.Items.AddRange(constants.getProveedores("aluminio").ToArray());
                comboBox3.Items.AddRange(constants.getCategorias("aluminio").ToArray());
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                comboBox2.Items.AddRange(constants.getProveedores("herraje").ToArray());
                comboBox3.Items.AddRange(constants.getCategorias("herraje").ToArray());
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                comboBox2.Items.AddRange(constants.getProveedores("otros").ToArray());
                comboBox3.Items.AddRange(constants.getCategorias("otros").ToArray());
            }
        }

        //Carga los productos
        private void loadListas()
        {
            setFiltros();
            listas_entities_pva listas = new listas_entities_pva();
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    var cristal = from x in listas.lista_costo_corte_e_instalado
                                  orderby x.articulo ascending
                                  select new
                                  {
                                      Clave = x.clave,
                                      Artículo = x.articulo,
                                      Proveedor = x.proveedor,
                                      Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                      Costo_Instalado = "$" + x.costo_instalado
                                  };
                    if (datagridviewNE1.InvokeRequired == true)
                    {
                        datagridviewNE1.Invoke((MethodInvoker)delegate
                        {
                            datagridviewNE1.DataSource = null;
                            datagridviewNE1.DataSource = cristal.ToList();
                        });
                    }
                    else {
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = cristal.ToList();
                    }
                    break;
                case 1:
                    var aluminio = from x in listas.perfiles
                                   orderby x.articulo ascending
                                   select new
                                   {
                                       Id = x.id,
                                       Clave = x.clave,
                                       Artículo = x.articulo,
                                       Linea = x.linea,
                                       Proveedor = x.proveedor,
                                       Largo = x.largo + " m",
                                       Crudo = "$" + x.crudo,
                                       Blanco = "$" + x.blanco,
                                       Hueso = "$" + x.hueso,
                                       Champagne = "$" + x.champagne,
                                       Gris = "$" + x.gris,
                                       Negro = "$" + x.negro,
                                       Brillante = "$" + x.brillante,
                                       Natural = "$" + x.natural_1,
                                       Madera = "$" + x.madera,
                                       Chocolate = "$" + x.chocolate,
                                       Acero_Inox = "$" + x.acero_inox,
                                       Bronce = "$" + x.bronce
                                   };
                    if (datagridviewNE1.InvokeRequired == true)
                    {
                        datagridviewNE1.Invoke((MethodInvoker)delegate
                        {
                            datagridviewNE1.DataSource = null;
                            datagridviewNE1.DataSource = aluminio.ToList();
                        });
                    }
                    else {
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = aluminio.ToList();
                    }
                    break;
                case 2:
                    var herraje = from x in listas.herrajes
                                  orderby x.articulo ascending
                                  select new
                                  {
                                      Id = x.id,
                                      Clave = x.clave,
                                      Artículo = x.articulo,
                                      Linea = x.linea,
                                      Proveedor = x.proveedor,
                                      Caracteristicas = x.caracteristicas,
                                      Color = x.color,
                                      Precio = "$" + x.precio
                                  };
                    if (datagridviewNE1.InvokeRequired == true)
                    {
                        datagridviewNE1.Invoke((MethodInvoker)delegate
                        {
                            datagridviewNE1.DataSource = null;
                            datagridviewNE1.DataSource = herraje.ToList();
                        });
                    }
                    else {
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = herraje.ToList();
                    }
                    break;
                case 3:
                    var data = from x in listas.otros
                               orderby x.articulo ascending
                               select new
                               {
                                   Id = x.id,
                                   Clave = x.clave,
                                   Artículo = x.articulo,
                                   Linea = x.linea,
                                   Proveedor = x.proveedor,
                                   Caracteristicas = x.caracteristicas,
                                   Color = x.color,
                                   Precio = "$" + x.precio
                               };
                    if (datagridviewNE1.InvokeRequired == true)
                    {
                        datagridviewNE1.Invoke((MethodInvoker)delegate
                        {
                            datagridviewNE1.DataSource = null;
                            datagridviewNE1.DataSource = data.ToList();
                        });
                    }
                    else {
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = data.ToList();
                    }
                    break;
                default:
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadListas();
            textBox1.Focus();
        }

        //Cargar proveedores
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listas_entities_pva listas = new listas_entities_pva();
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    if (comboBox3.Text != "")
                    {
                        var filter = from x in listas.lista_costo_corte_e_instalado
                                     where (x.articulo.StartsWith(comboBox3.Text)) && (x.proveedor == comboBox2.Text)
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                         Costo_Instalado = "$" + x.costo_instalado
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.lista_costo_corte_e_instalado
                                     where x.proveedor == comboBox2.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                         Costo_Instalado = "$" + x.costo_instalado
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    break;
                case 1:
                    if (comboBox3.Text != "")
                    {
                        var filter = from x in listas.perfiles
                                     where x.linea == comboBox3.Text && x.proveedor == comboBox2.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Largo = x.largo + " m",
                                         Crudo = "$" + x.crudo,
                                         Blanco = "$" + x.blanco,
                                         Hueso = "$" + x.hueso,
                                         Champagne = "$" + x.champagne,
                                         Gris = "$" + x.gris,
                                         Negro = "$" + x.negro,
                                         Brillante = "$" + x.brillante,
                                         Natural = "$" + x.natural_1,
                                         Madera = "$" + x.madera,
                                         Chocolate = "$" + x.chocolate,
                                         Acero_Inox = "$" + x.acero_inox,
                                         Bronce = "$" + x.bronce
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.perfiles
                                     where x.proveedor == comboBox2.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Largo = x.largo + " m",
                                         Crudo = "$" + x.crudo,
                                         Blanco = "$" + x.blanco,
                                         Hueso = "$" + x.hueso,
                                         Champagne = "$" + x.champagne,
                                         Gris = "$" + x.gris,
                                         Negro = "$" + x.negro,
                                         Brillante = "$" + x.brillante,
                                         Natural = "$" + x.natural_1,
                                         Madera = "$" + x.madera,
                                         Chocolate = "$" + x.chocolate,
                                         Acero_Inox = "$" + x.acero_inox,
                                         Bronce = "$" + x.bronce
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    break;
                case 2:
                    if (comboBox3.Text != "")
                    {
                        var filter = from x in listas.herrajes
                                     where x.linea == comboBox3.Text && x.proveedor == comboBox2.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.herrajes
                                     where x.proveedor == comboBox2.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    break;
                case 3:
                    if (comboBox3.Text != "")
                    {
                        var filter = from x in listas.otros
                                     where x.linea == comboBox3.Text && x.proveedor == comboBox2.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.otros
                                     where x.proveedor == comboBox2.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    break;
                default:
                    break;
            }
            listas.Dispose();
            textBox1.Focus();
        }

        //carga con filtros
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            listas_entities_pva listas = new listas_entities_pva();
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    if (comboBox2.Text != "")
                    {
                        var filter = from x in listas.lista_costo_corte_e_instalado
                                     where (x.articulo.StartsWith(comboBox3.Text)) && (x.proveedor == comboBox2.Text)
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                         Costo_Instalado = "$" + x.costo_instalado
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.lista_costo_corte_e_instalado
                                     where x.articulo.StartsWith(comboBox3.Text)
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Proveedor = x.proveedor,
                                         Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                         Costo_Instalado = "$" + x.costo_instalado
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    break;
                case 1:
                    if (comboBox2.Text != "")
                    {
                        var filter = from x in listas.perfiles
                                     where x.linea == comboBox3.Text && x.proveedor == comboBox2.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Largo = x.largo + " m",
                                         Crudo = "$" + x.crudo,
                                         Blanco = "$" + x.blanco,
                                         Hueso = "$" + x.hueso,
                                         Champagne = "$" + x.champagne,
                                         Gris = "$" + x.gris,
                                         Negro = "$" + x.negro,
                                         Brillante = "$" + x.brillante,
                                         Natural = "$" + x.natural_1,
                                         Madera = "$" + x.madera,
                                         Chocolate = "$" + x.chocolate,
                                         Acero_Inox = "$" + x.acero_inox,
                                         Bronce = "$" + x.bronce
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.perfiles
                                     where x.linea == comboBox3.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Largo = x.largo + " m",
                                         Crudo = "$" + x.crudo,
                                         Blanco = "$" + x.blanco,
                                         Hueso = "$" + x.hueso,
                                         Champagne = "$" + x.champagne,
                                         Gris = "$" + x.gris,
                                         Negro = "$" + x.negro,
                                         Brillante = "$" + x.brillante,
                                         Natural = "$" + x.natural_1,
                                         Madera = "$" + x.madera,
                                         Chocolate = "$" + x.chocolate,
                                         Acero_Inox = "$" + x.acero_inox,
                                         Bronce = "$" + x.bronce
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    break;
                case 2:
                    if (comboBox2.Text != "")
                    {
                        var filter = from x in listas.herrajes
                                     where x.linea == comboBox3.Text && x.proveedor == comboBox2.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.herrajes
                                     where x.linea == comboBox3.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    break;
                case 3:
                    if (comboBox2.Text != "")
                    {
                        var filter = from x in listas.otros
                                     where x.linea == comboBox3.Text && x.proveedor == comboBox2.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    else
                    {
                        var filter = from x in listas.otros
                                     where x.linea == comboBox3.Text
                                     orderby x.articulo ascending
                                     select new
                                     {
                                         Id = x.id,
                                         Clave = x.clave,
                                         Artículo = x.articulo,
                                         Linea = x.linea,
                                         Proveedor = x.proveedor,
                                         Caracteristicas = x.caracteristicas,
                                         Color = x.color,
                                         Precio = "$" + x.precio
                                     };
                        datagridviewNE1.DataSource = null;
                        datagridviewNE1.DataSource = filter.ToList();
                    }
                    break;
                default:
                    break;
            }
            listas.Dispose();
            textBox1.Focus();
        }

        //reset button
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
                loadListas();
            }
        }

        //buscar
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listas_entities_pva listas = new listas_entities_pva();
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    var costo_corte = from x in listas.lista_costo_corte_e_instalado
                                      where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                      orderby x.articulo ascending
                                      select new
                                      {
                                          Clave = x.clave,
                                          Artículo = x.articulo,
                                          Proveedor = x.proveedor,
                                          Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                          Costo_Instalado = "$" + x.costo_instalado
                                      };
                    datagridviewNE1.DataSource = null;
                    datagridviewNE1.DataSource = costo_corte.ToList();
                    break;
                case 1:
                    var perfiles = from x in listas.perfiles
                                   where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                   orderby x.articulo ascending
                                   select new
                                   {
                                       Id = x.id,
                                       Clave = x.clave,
                                       Artículo = x.articulo,
                                       Linea = x.linea,
                                       Proveedor = x.proveedor,
                                       Largo = x.largo + " m",
                                       Crudo = "$" + x.crudo,
                                       Blanco = "$" + x.blanco,
                                       Hueso = "$" + x.hueso,
                                       Champagne = "$" + x.champagne,
                                       Gris = "$" + x.gris,
                                       Negro = "$" + x.negro,
                                       Brillante = "$" + x.brillante,
                                       Natural = "$" + x.natural_1,
                                       Madera = "$" + x.madera,
                                       Chocolate = "$" + x.chocolate,
                                       Acero_Inox = "$" + x.acero_inox,
                                       Bronce = "$" + x.bronce
                                   };
                    datagridviewNE1.DataSource = null;
                    datagridviewNE1.DataSource = perfiles.ToList();
                    break;
                case 2:
                    var herrajes = from x in listas.herrajes
                                   where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                   orderby x.articulo ascending
                                   select new
                                   {
                                       Id = x.id,
                                       Clave = x.clave,
                                       Artículo = x.articulo,
                                       Linea = x.linea,
                                       Proveedor = x.proveedor,
                                       Caracteristicas = x.caracteristicas,
                                       Color = x.color,
                                       Precio = "$" + x.precio
                                   };
                    datagridviewNE1.DataSource = null;
                    datagridviewNE1.DataSource = herrajes.ToList();
                    break;
                case 3:
                    var otros = from x in listas.otros
                                where x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text)
                                orderby x.articulo ascending
                                select new
                                {
                                    Id = x.id,
                                    Clave = x.clave,
                                    Artículo = x.articulo,
                                    Linea = x.linea,
                                    Proveedor = x.proveedor,
                                    Caracteristicas = x.caracteristicas,
                                    Color = x.color,
                                    Precio = "$" + x.precio
                                };
                    datagridviewNE1.DataSource = null;
                    datagridviewNE1.DataSource = otros.ToList();
                    break;
                default: break;
            }
            listas.Dispose();
        }

        private void añadirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                if (Application.OpenForms["config_modulo"] != null)
                {
                    if (comboBox1.SelectedIndex == 0)
                    {
                        ((config_modulo)Application.OpenForms["config_modulo"]).setNewItem(2, datagridviewNE1.CurrentRow.Cells[0].Value.ToString());
                        ((config_modulo)Application.OpenForms["config_modulo"]).selectTab(2);
                        ((config_modulo)Application.OpenForms["config_modulo"]).setComponentEnable(2);
                    }
                    else if(comboBox1.SelectedIndex == 1)
                    {
                        ((config_modulo)Application.OpenForms["config_modulo"]).setNewItem(1, datagridviewNE1.CurrentRow.Cells[1].Value.ToString());
                        ((config_modulo)Application.OpenForms["config_modulo"]).selectTab(1);
                        ((config_modulo)Application.OpenForms["config_modulo"]).setComponentEnable(1);
                    }
                    else if (comboBox1.SelectedIndex == 2)
                    {
                        ((config_modulo)Application.OpenForms["config_modulo"]).setNewItem(3, datagridviewNE1.CurrentRow.Cells[1].Value.ToString());
                        ((config_modulo)Application.OpenForms["config_modulo"]).selectTab(3);
                        ((config_modulo)Application.OpenForms["config_modulo"]).setComponentEnable(3);
                    }
                    else if (comboBox1.SelectedIndex == 3)
                    {
                        ((config_modulo)Application.OpenForms["config_modulo"]).setNewItem(4, datagridviewNE1.CurrentRow.Cells[1].Value.ToString());
                        ((config_modulo)Application.OpenForms["config_modulo"]).selectTab(4);
                        ((config_modulo)Application.OpenForms["config_modulo"]).setComponentEnable(4);
                    }
                }
            }
        }

        //boton nuevos costos
        private void button2_Click(object sender, EventArgs e)
        {
            if (constants.user_access <= 1 && constants.permitir_cp == false)
            {
                new delete_password(false, true).ShowDialog();
            }
            else
            {
                new new_costos(lista).ShowDialog();
            }
        }
    }
}
