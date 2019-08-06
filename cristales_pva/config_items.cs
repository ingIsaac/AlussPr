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
    public partial class config_items : Form
    {
        string clave = string.Empty;
        int id = -1;
        int componente = -1;
        int index = -1;
        bool se;
        bool wizard;

        public config_items(int index, int componente, string clave, int id, bool se=false, bool wizard=false)
        {
            InitializeComponent();
            this.clave = clave;
            this.id = id;
            this.componente = componente;
            this.index = index;
            this.se = se;
            this.wizard = wizard;
            textBox1.KeyDown += TextBox1_KeyDown;
            dataGridView1.CellClick += DataGridView1_CellClick;
            dataGridView1.CellLeave += DataGridView1_CellLeave;
        }

        private void DataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(dataGridView1.RowCount > 0)
            {
                dataGridView1.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
            }
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                loadList();
            }
        }

        private void loadList()
        {
            listas_entities_pva lista = new listas_entities_pva();
            string filter = textBox1.Text;
            dataGridView1.DataSource = null;
            if (componente == 0)
            {
                var linea = (from x in lista.herrajes where x.id == id select x.linea).SingleOrDefault();

                if (linea != null)
                {
                    if (se == false)
                    {
                        if (filter != "")
                        {
                            var herraje = from u in lista.herrajes
                                          where u.linea == linea && (u.clave.Contains(filter) || u.articulo.Contains(filter))
                                          select new
                                          {
                                              Id = u.id,
                                              Clave = u.clave,
                                              Artículo = u.articulo,
                                              Linea = u.linea,
                                              Proveedor = u.proveedor,
                                              Color = u.color,
                                              Precio = "$" + u.precio
                                          };
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = herraje.ToList();
                        }
                        else
                        {
                            var herraje = from u in lista.herrajes
                                          where u.linea == linea
                                          select new
                                          {
                                              Id = u.id,
                                              Clave = u.clave,
                                              Artículo = u.articulo,
                                              Linea = u.linea,
                                              Proveedor = u.proveedor,
                                              Color = u.color,
                                              Precio = "$" + u.precio
                                          };
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = herraje.ToList();
                        }
                    }
                    else
                    {
                        if (filter != "")
                        {
                            var herraje = from u in lista.herrajes
                                          where u.clave.Contains(filter) || u.articulo.Contains(filter)
                                          select new
                                          {
                                              Id = u.id,
                                              Clave = u.clave,
                                              Artículo = u.articulo,
                                              Linea = u.linea,
                                              Proveedor = u.proveedor,
                                              Color = u.color,
                                              Precio = "$" + u.precio
                                          };
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = herraje.ToList();
                        }
                        else
                        {
                            var herraje = from u in lista.herrajes
                                          select new
                                          {
                                              Id = u.id,
                                              Clave = u.clave,
                                              Artículo = u.articulo,
                                              Linea = u.linea,
                                              Proveedor = u.proveedor,
                                              Color = u.color,
                                              Precio = "$" + u.precio
                                          };
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = herraje.ToList();
                        }
                    }
                }                
            }
            else if (componente == 1)
            {
                var linea = (from x in lista.otros where x.id == id select x.linea).SingleOrDefault();

                if (linea != null)
                {
                    if (se == false)
                    {
                        if (filter != "")
                        {
                            var otros = from u in lista.otros
                                        where u.linea == linea && (u.clave.Contains(filter) || u.articulo.Contains(filter))
                                        select new
                                        {
                                            Id = u.id,
                                            Clave = u.clave,
                                            Artículo = u.articulo,
                                            Linea = u.linea,
                                            Proveedor = u.proveedor,
                                            Color = u.color,
                                            Precio = "$" + u.precio
                                        };
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = otros.ToList();
                        }
                        else
                        {
                            var otros = from u in lista.otros
                                        where u.linea == linea
                                        select new
                                        {
                                            Id = u.id,
                                            Clave = u.clave,
                                            Artículo = u.articulo,
                                            Linea = u.linea,
                                            Proveedor = u.proveedor,
                                            Color = u.color,
                                            Precio = "$" + u.precio
                                        };
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = otros.ToList();
                        }
                    }
                    else
                    {
                        if (filter != "")
                        {
                            var otros = from u in lista.otros
                                        where u.clave.Contains(filter) || u.articulo.Contains(filter)
                                        select new
                                        {
                                            Id = u.id,
                                            Clave = u.clave,
                                            Artículo = u.articulo,
                                            Linea = u.linea,
                                            Proveedor = u.proveedor,
                                            Color = u.color,
                                            Precio = "$" + u.precio
                                        };
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = otros.ToList();
                        }
                        else
                        {
                            var otros = from u in lista.otros                                        
                                        select new
                                        {
                                            Id = u.id,
                                            Clave = u.clave,
                                            Artículo = u.articulo,
                                            Linea = u.linea,
                                            Proveedor = u.proveedor,
                                            Color = u.color,
                                            Precio = "$" + u.precio
                                        };
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = otros.ToList();
                        }
                    }
                }
            }
            else if (componente == 2)
            {
                if (filter != "")
                {
                    var cristales = from x in lista.lista_costo_corte_e_instalado
                                    where x.clave.Contains(filter) || x.articulo.Contains(filter)
                                    select new
                                    {
                                        Clave = x.clave,
                                        Artículo = x.articulo,
                                        Proveedor = x.proveedor,
                                        Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                        Costo_Instalado = "$" + x.costo_instalado
                                    };
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = cristales.ToList();
                }
                else
                {
                    var cristales = from x in lista.lista_costo_corte_e_instalado
                                    select new
                                    {
                                        Clave = x.clave,
                                        Artículo = x.articulo,
                                        Proveedor = x.proveedor,
                                        Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                        Costo_Instalado = "$" + x.costo_instalado
                                    };
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = cristales.ToList();
                }
            }
            else if (componente == 3)
            {
                if (filter != "")
                {
                    var perfiles = from x in lista.perfiles
                                    where x.clave.Contains(filter) || x.articulo.Contains(filter)
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
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = perfiles.ToList();
                }
                else
                {
                    var perfiles = from x in lista.perfiles
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
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = perfiles.ToList();
                }
            }
            lista.Dispose();
        }

        private void config_items_Load(object sender, EventArgs e)
        {
            loadList();
            textBox1.Select();
        }

        private void cambiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount > 0)
            {
                if (wizard == false)
                {
                    if (componente == 0)
                    {
                        ((config_modulo)Application.OpenForms["config_modulo"]).setNewHerraje(index, constants.stringToInt(dataGridView1.CurrentRow.Cells[0].Value.ToString()), dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[2].Value.ToString(), dataGridView1.CurrentRow.Cells[5].Value.ToString());
                    }
                    else if (componente == 1)
                    {
                        ((config_modulo)Application.OpenForms["config_modulo"]).setNewOtros(index, constants.stringToInt(dataGridView1.CurrentRow.Cells[0].Value.ToString()), dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[2].Value.ToString(), dataGridView1.CurrentRow.Cells[5].Value.ToString());
                    }
                    else if (componente == 2)
                    {
                        ((config_modulo)Application.OpenForms["config_modulo"]).setNewCristal(index, dataGridView1.CurrentRow.Cells[0].Value.ToString(), dataGridView1.CurrentRow.Cells[1].Value.ToString());
                    }
                    else if (componente == 3)
                    {
                        ((config_modulo)Application.OpenForms["config_modulo"]).setNewPerfiles(index, constants.stringToInt(dataGridView1.CurrentRow.Cells[0].Value.ToString()), dataGridView1.CurrentRow.Cells[1].Value.ToString(), dataGridView1.CurrentRow.Cells[2].Value.ToString());
                    }
                }
                else
                {
                    ((change_colors)Application.OpenForms["change_colors"]).setCristal(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                }
                this.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            loadList();
        }
    }
}
