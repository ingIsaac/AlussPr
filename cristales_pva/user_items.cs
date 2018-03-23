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
    public partial class user_items : Form
    {
        int id = 0;

        public user_items()
        {
            InitializeComponent();
            textBox10.KeyDown += TextBox10_KeyDown;
            textBox1.KeyDown += TextBox1_KeyDown;
            datagridviewNE2.CellEndEdit += DatagridviewNE2_CellEndEdit;
            this.FormClosed += User_items_FormClosed;
        }

        private void User_items_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (constants.updater_form_close == true)
            {
                new loading_form().ShowDialog();
            }
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                listas_entities_pva listas = new listas_entities_pva();
                var otros = from x in listas.otros
                            where x.clave.StartsWith("USER") && (x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text))
                            orderby x.articulo ascending
                            select new
                            {
                                Id = x.id,
                                Clave = x.clave,
                                Artículo = x.articulo,
                                Linea = x.linea,
                                Color = x.color,
                                Precio = "$" + x.precio
                            };
                datagridviewNE1.DataSource = null;
                datagridviewNE1.DataSource = otros.ToList();
            }
        }

        private void DatagridviewNE2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if(datagridviewNE2.RowCount > 0)
            {
                calcular();
            }
        }

        private void loadAll()
        {
            if(backgroundWorker1.IsBusy == false)
            {
                backgroundWorker1.RunWorkerAsync();
            }           
        }

        private void TextBox10_KeyDown(object sender, KeyEventArgs e)
        {
           if(e.KeyData == Keys.Enter)
            {
                listas_entities_pva listas = new listas_entities_pva();
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        var costo_corte = from x in listas.lista_costo_corte_e_instalado
                                          where x.articulo.StartsWith(textBox10.Text) || x.clave.StartsWith(textBox10.Text)
                                          orderby x.articulo ascending
                                          select new
                                          {
                                              Clave = x.clave,
                                              Artículo = x.articulo,
                                              Proveedor = x.proveedor,
                                              Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                              Costo_Instalado = "$" + x.costo_instalado
                                          };
                        datagridviewNE3.DataSource = null;
                        datagridviewNE3.DataSource = costo_corte.ToList();
                        break;
                    case 1:
                        var herrajes = from x in listas.herrajes
                                       where x.articulo.StartsWith(textBox10.Text) || x.clave.StartsWith(textBox10.Text)
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
                        datagridviewNE3.DataSource = null;
                        datagridviewNE3.DataSource = herrajes.ToList();
                        break;
                    case 2:
                        var otros = from x in listas.otros
                                    where x.articulo.StartsWith(textBox10.Text) || x.clave.StartsWith(textBox10.Text)
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
                        datagridviewNE3.DataSource = null;
                        datagridviewNE3.DataSource = otros.ToList();
                        break;
                    default: break;
                }
                listas.Dispose();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadListas();
        }

        private void loadListas()
        {
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
                    if (datagridviewNE3.InvokeRequired == true)
                    {
                        datagridviewNE3.Invoke((MethodInvoker)delegate
                        {
                            datagridviewNE3.DataSource = null;
                            datagridviewNE3.DataSource = cristal.ToList();
                        });
                    }
                    else {
                        datagridviewNE3.DataSource = null;
                        datagridviewNE3.DataSource = cristal.ToList();
                    }
                    break;              
                case 1:
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
                    if (datagridviewNE3.InvokeRequired == true)
                    {
                        datagridviewNE3.Invoke((MethodInvoker)delegate
                        {
                            datagridviewNE3.DataSource = null;
                            datagridviewNE3.DataSource = herraje.ToList();
                        });
                    }
                    else {
                        datagridviewNE3.DataSource = null;
                        datagridviewNE3.DataSource = herraje.ToList();
                    }
                    break;
                case 2:
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
                    if (datagridviewNE3.InvokeRequired == true)
                    {
                        datagridviewNE3.Invoke((MethodInvoker)delegate
                        {
                            datagridviewNE3.DataSource = null;
                            datagridviewNE3.DataSource = data.ToList();
                        });
                    }
                    else {
                        datagridviewNE3.DataSource = null;
                        datagridviewNE3.DataSource = data.ToList();
                    }
                    break;
                default:
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            loadListas();
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            listas_entities_pva listas = new listas_entities_pva();
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    var costo_corte = from x in listas.lista_costo_corte_e_instalado
                                      where x.articulo.StartsWith(textBox10.Text) || x.clave.StartsWith(textBox10.Text)
                                      orderby x.articulo ascending
                                      select new
                                      {
                                          Clave = x.clave,
                                          Artículo = x.articulo,
                                          Proveedor = x.proveedor,
                                          Costo_Corte_m2 = "$" + x.costo_corte_m2,
                                          Costo_Instalado = "$" + x.costo_instalado
                                      };
                    datagridviewNE3.DataSource = null;
                    datagridviewNE3.DataSource = costo_corte.ToList();
                    break;              
                case 1:
                    var herrajes = from x in listas.herrajes
                                   where x.articulo.StartsWith(textBox10.Text) || x.clave.StartsWith(textBox10.Text)
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
                    datagridviewNE3.DataSource = null;
                    datagridviewNE3.DataSource = herrajes.ToList();
                    break;
                case 2:
                    var otros = from x in listas.otros
                                where x.articulo.StartsWith(textBox10.Text) || x.clave.StartsWith(textBox10.Text)
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
                    datagridviewNE3.DataSource = null;
                    datagridviewNE3.DataSource = otros.ToList();
                    break;
                default: break;
            }
            listas.Dispose();
        }

        private void user_items_Load(object sender, EventArgs e)
        {
            loadAll();
            comboBox2.Text = "paquetes";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            loadAll();
        }

        //Guardar
        private void button1_Click(object sender, EventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if(textBox2.Text != "")
            {
                if(comboBox2.Text != "")
                {
                    if(sql.findSQLValue("clave", "clave", "otros", textBox3.Text) == false)
                    {
                        sql.insertListaOtros(constants.setUserItemClave(), textBox2.Text, "", comboBox2.Text, comboBox2.Text == "servicios" ? richTextBox1.Text : getArticulos(), textBox6.Text, (float)Math.Round(constants.stringToFloat(textBox9.Text), 2), DateTime.Today.ToString("dd/MM/yyyy"), constants.stringToFloat(textBox7.Text), constants.stringToFloat(textBox8.Text));
                        MessageBox.Show("Se ha creado el " + comboBox2.Text.Remove(comboBox2.Text.Length-1) + ".", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        loadAll();
                    }
                    else
                    {
                        if (MessageBox.Show("¿Deseas actualizar estos datos?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            sql.updateListaOtros(this.id, textBox2.Text, "", comboBox2.Text, comboBox2.Text == "servicios" ? richTextBox1.Text : getArticulos(), textBox6.Text, (float)Math.Round(constants.stringToFloat(textBox9.Text), 2), DateTime.Today.ToString("dd/MM/yyyy"), constants.stringToFloat(textBox7.Text), constants.stringToFloat(textBox8.Text));
                            MessageBox.Show("Se ha actualizado el " + comboBox2.Text.Remove(comboBox2.Text.Length - 1) + ".", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            loadAll();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("[Error] se necesita seleccionar una linea.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error] se necesita escribir el nombre del " + comboBox2.Text.Remove(comboBox2.Text.Length - 1) + ".", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string getArticulos()
        {
            string r = string.Empty;
            foreach(DataGridViewRow x in datagridviewNE2.Rows)
            {
                if(x.Cells[0].Value.ToString() == "Cristal")
                {
                    if(r == "")
                    {
                        r = r + "1:" + x.Cells[1].Value.ToString() + ":" + x.Cells[3].Value.ToString();
                    }
                    else
                    {
                        r = r + ",1:" + x.Cells[1].Value.ToString() + ":" + x.Cells[3].Value.ToString();
                    }
                }
                else if(x.Cells[0].Value.ToString() == "Herraje")
                {
                    if (r == "")
                    {
                        r = r + "2:" + x.Cells[1].Value.ToString() + ":" + x.Cells[3].Value.ToString();
                    }
                    else
                    {
                        r = r + ",2:" + x.Cells[1].Value.ToString() + ":" + x.Cells[3].Value.ToString();
                    }
                }
                else if (x.Cells[0].Value.ToString() == "Otros")
                {
                    if (r == "")
                    {
                        r = r + "3:" + x.Cells[1].Value.ToString() + ":" + x.Cells[3].Value.ToString();
                    }
                    else
                    {
                        r = r + ",3:" + x.Cells[1].Value.ToString() + ":" + x.Cells[3].Value.ToString();
                    }
                }
            }
            return r;
        }

        private void load(int id)
        {
            listas_entities_pva listas = new listas_entities_pva();
            datagridviewNE2.Rows.Clear();
            this.id = id;
            var otros = (from x in listas.otros where x.id == id select x).SingleOrDefault();
            if(otros != null)
            {
                textBox3.Text = otros.clave;
                textBox2.Text = otros.articulo;
                comboBox2.Text = otros.linea;
                textBox6.Text = otros.color;
                textBox7.Text = otros.largo.ToString();
                textBox8.Text = otros.alto.ToString();
                textBox9.Text = otros.precio.ToString();
                richTextBox1.Text = otros.linea == "servicios" ? otros.caracteristicas : "";
                if(otros.linea == "paquetes")
                {
                    string[] v = otros.caracteristicas.Split(',');
                    string[] k = null;
                    string clave = string.Empty;
                    foreach(string x in v)
                    {
                        k = x.Split(':');
                        if (k.Length == 3)
                        {
                            clave = k[1];
                            dynamic p = null;
                            if (k[0] == "1")
                            {
                                p = (from y in listas.lista_costo_corte_e_instalado where y.clave == clave select y.articulo).SingleOrDefault();
                            }
                            else if (k[0] == "2")
                            {
                                p = (from y in listas.herrajes where y.clave == clave select y.articulo).SingleOrDefault();
                            }
                            else if (k[0] == "3")
                            {
                                p = (from y in listas.otros where y.clave == clave select y.articulo).SingleOrDefault();
                            }
                            if (p != null)
                            {
                                datagridviewNE2.Rows.Add(getComponente(k[0]), clave, p, k[2]);
                            }
                        }
                    }
                }
            }
        }

        private string getComponente(string i)
        {
            if(i == "1")
            {
                return "Cristal";
            }
            else if(i == "2")
            {
                return "Herraje";
            }
            else if (i == "3")
            {
                return "Otros";
            }
            else
            {
                return "";
            }
        }

        private string getComponenteFromList()
        {
            if (comboBox1.SelectedIndex == 0)
            {
                return "Cristal";
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                return "Herraje";
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                return "Otros";
            }
            else
            {
                return "";
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox2.Text == "paquetes")
            {
                richTextBox1.Clear();
                richTextBox1.Enabled = false;
                label3.Text = "Artículo:";
                textBox9.Enabled = false;
                textBox6.Enabled = true;
                textBox7.Enabled = true;
                textBox8.Enabled = true;
            }
            else
            {
                label3.Text = "Servicio:";
                datagridviewNE2.Rows.Clear();
                richTextBox1.Enabled = true;
                textBox9.Enabled = true;
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                textBox8.Enabled = false;
                textBox6.Clear();
                textBox7.Clear();
                textBox8.Clear();
                textBox9.Clear();
            }
        }

        private void calcular()
        {
            if (comboBox2.Text == "paquetes")
            {
                string clave = string.Empty;
                float c = 0;
                listas_entities_pva listas = new listas_entities_pva();
                foreach (DataGridViewRow x in datagridviewNE2.Rows)
                {
                    clave = x.Cells[1].Value.ToString();
                    if (x.Cells[0].Value.ToString() == "Cristal")
                    {
                        var cristales = (from y in listas.lista_costo_corte_e_instalado where y.clave == clave select y).SingleOrDefault();
                        if (cristales != null)
                        {
                            c = c + ((float)cristales.costo_corte_m2 * constants.stringToFloat(x.Cells[3].Value.ToString()));
                        }
                    }
                    else if (x.Cells[0].Value.ToString() == "Herraje")
                    {
                        var herrajes = (from y in listas.herrajes where y.clave == clave select y).SingleOrDefault();
                        if (herrajes != null)
                        {
                            c = c + ((float)herrajes.precio * constants.stringToFloat(x.Cells[3].Value.ToString()));
                        }
                    }
                    else if (x.Cells[0].Value.ToString() == "Otros")
                    {
                        var otros = (from y in listas.otros where y.clave == clave select y).SingleOrDefault();
                        if (otros != null)
                        {
                            c = c + ((float)otros.precio * constants.stringToFloat(x.Cells[3].Value.ToString()));
                        }
                    }
                }
                textBox9.Text = Math.Round(c, 2).ToString();
            }
        }

        //Editar
        private void editarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                load((int)datagridviewNE1.CurrentRow.Cells[0].Value);
                button4.Visible = true;
                calcular();
            }
        }

        //Agregar
        private void agregarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE3.Rows.Count > 0)
            {
                if (comboBox2.Text == "paquetes")
                {
                    datagridviewNE2.Rows.Add(getComponenteFromList(), comboBox1.SelectedIndex == 0 ? datagridviewNE3.CurrentRow.Cells[0].Value.ToString() : datagridviewNE3.CurrentRow.Cells[1].Value.ToString(), comboBox1.SelectedIndex == 0 ? datagridviewNE3.CurrentRow.Cells[1].Value.ToString() : datagridviewNE3.CurrentRow.Cells[2].Value.ToString(), "1");
                    calcular();
                }
                else
                {
                    MessageBox.Show("[Error] para empaquetar artículos seleccioné la opción 'paquetes'.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Remover
        private void removerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(datagridviewNE2.RowCount > 0)
            {
                datagridviewNE2.Rows.Remove(datagridviewNE2.CurrentRow);
                calcular();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listas_entities_pva listas = new listas_entities_pva();
            var otros = from x in listas.otros
                        where x.clave.StartsWith("USER") && (x.articulo.StartsWith(textBox1.Text) || x.clave.StartsWith(textBox1.Text))
                        orderby x.articulo ascending
                        select new
                        {
                            Id = x.id,
                            Clave = x.clave,
                            Artículo = x.articulo,
                            Linea = x.linea,
                            Color = x.color,
                            Precio = "$" + x.precio
                        };
            datagridviewNE1.DataSource = null;
            datagridviewNE1.DataSource = otros.ToList();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox3.Text = "00000000000";
            textBox2.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            this.id = 0;
            datagridviewNE2.Rows.Clear();
            textBox9.Clear();
            richTextBox1.Clear();
            button4.Visible = false;
        }

        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(constants.user_access >= 5)
            {
                if(datagridviewNE1.Rows.Count > 0)
                {
                    sqlDateBaseManager sql = new sqlDateBaseManager();
                    string id = datagridviewNE1.CurrentRow.Cells[0].Value.ToString();
                    sql.deleteSQLValue("otros", "id", id);
                    MessageBox.Show("El artículo ha sido eliminado con exito.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loadAll();
                }
            }
            else
            {
                MessageBox.Show("[Error] solo un usuario con privilegios de grado (5) puede acceder a esta característica.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (datagridviewNE1.InvokeRequired == true)
            {
                datagridviewNE1.Invoke((MethodInvoker)delegate
                {
                    datagridviewNE1.DataSource = null;
                    datagridviewNE1.DataSource = new sqlDateBaseManager().createDataTableFromSQLTableWithFilter("otros", "clave", "USER");
                });
            }
            else
            {
                datagridviewNE1.DataSource = null;
                datagridviewNE1.DataSource = new sqlDateBaseManager().createDataTableFromSQLTableWithFilter("otros", "clave", "USER");
            }
        }
    }
}
