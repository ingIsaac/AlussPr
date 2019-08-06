using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace cristales_pva
{
    public partial class fabrica : Form
    {
        public fabrica()
        {
            InitializeComponent();
        }

        private void fabrica_Load(object sender, EventArgs e)
        {
            getCategeriasCristal();
            loadCristales();
            LoadFabrica();
        }

        private void getCategeriasCristal()
        {
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(constants.getCategorias("vidrio").ToArray());
        }

        private void LoadFabrica()
        {
            if(constants.factory_acabado_perfil != string.Empty)
            {
                string[] c = constants.factory_acabado_perfil.Split(',');
                if(c.Length == 2)
                {
                    label3.Text = c[1];
                    button1.Visible = true;
                    if (c[0] == "0")
                    {
                        constants.setImage("acabados_perfil", c[1], "jpg", pictureBox1);
                    }
                    else if (c[0] == "1")
                    {
                        constants.setImage("acabados_especiales", c[1], "jpg", pictureBox1);
                    }
                }
            }
            if(constants.factory_cristal != string.Empty)
            {
                string[] c = constants.factory_cristal.Split(',');
                if(c.Length == 2)
                {
                    label4.Text = c[1];
                    button2.Visible = true;
                }               
            }
        }

        private void loadAcabado(int index)
        {
            if(index == 0)
            {
                //Reset
                datagridviewNE1.Rows.Clear();
                datagridviewNE1.Columns.Clear();
                //

                datagridviewNE1.Columns.Add("id", "id");
                datagridviewNE1.Columns.Add("acabado", "acabado");
                DataGridViewImageColumn clm_1 = new DataGridViewImageColumn();
                clm_1.Name = "muestra";
                clm_1.HeaderText = "muestra";
                clm_1.ImageLayout = DataGridViewImageCellLayout.Stretch;
                datagridviewNE1.Columns.Add(clm_1);

                datagridviewNE1.Rows.Add("1", "crudo", constants.getImageFromFile("acabados_perfil", "crudo", "jpg"));
                datagridviewNE1.Rows.Add("2", "blanco", constants.getImageFromFile("acabados_perfil", "blanco", "jpg"));
                datagridviewNE1.Rows.Add("3", "hueso", constants.getImageFromFile("acabados_perfil", "blanco", "jpg"));
                datagridviewNE1.Rows.Add("4", "champagne", constants.getImageFromFile("acabados_perfil", "champagne", "jpg"));
                datagridviewNE1.Rows.Add("5", "gris", constants.getImageFromFile("acabados_perfil", "gris", "jpg"));
                datagridviewNE1.Rows.Add("6", "negro", constants.getImageFromFile("acabados_perfil", "negro", "jpg"));
                datagridviewNE1.Rows.Add("7", "brillante", constants.getImageFromFile("acabados_perfil", "brillante", "jpg"));
                datagridviewNE1.Rows.Add("8", "natural", constants.getImageFromFile("acabados_perfil", "natural", "jpg"));
                datagridviewNE1.Rows.Add("9", "madera", constants.getImageFromFile("acabados_perfil", "madera", "jpg"));
                datagridviewNE1.Rows.Add("10", "chocolate", constants.getImageFromFile("acabados_perfil", "chocolate", "jpg"));
                datagridviewNE1.Rows.Add("11", "acero_inox", constants.getImageFromFile("acabados_perfil", "acero_inox", "jpg"));
                datagridviewNE1.Rows.Add("12", "bronce", constants.getImageFromFile("acabados_perfil", "bronce", "jpg"));
            }
            else if(index == 1)
            {
                //Reset
                datagridviewNE1.Rows.Clear();
                datagridviewNE1.Columns.Clear();
                //

                sqlDateBaseManager sql = new sqlDateBaseManager();
                DataTable table = sql.createDataTableFromSQLTable("colores_aluminio");

                foreach (DataColumn x in table.Columns)
                {
                    datagridviewNE1.Columns.Add(x.ColumnName, x.ColumnName);
                }

                //Remove no necesary columns
                datagridviewNE1.Columns.RemoveAt(6);
                datagridviewNE1.Columns.RemoveAt(5);
                //

                DataGridViewImageColumn clm_1 = new DataGridViewImageColumn();
                clm_1.Name = "muestra";
                clm_1.HeaderText = "muestra";
                datagridviewNE1.Columns.Add(clm_1);
                datagridviewNE1.Columns["muestra"].DisplayIndex = 2;

                foreach (DataRow x in table.Rows)
                {
                    datagridviewNE1.Rows.Add(x.ItemArray[0], x.ItemArray[1], x.ItemArray[2], x.ItemArray[3], x.ItemArray[4], constants.getImageFromFile("acabados_especiales", x.ItemArray[1].ToString(), "jpg"));
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadAcabado(comboBox1.SelectedIndex);
        }

        private void loadCristales()
        {
            listas_entities_pva listas = new listas_entities_pva();
            var filter = from x in listas.lista_costo_corte_e_instalado
                         where x.articulo.Contains(comboBox2.Text)
                         orderby x.articulo ascending
                         select new
                         {
                             Clave = x.clave,
                             Artículo = x.articulo,
                             Proveedor = x.proveedor,
                             Costo_Corte_m2 = "$" + x.costo_corte_m2
                         };
            datagridviewNE2.DataSource = null;
            datagridviewNE2.DataSource = filter.ToList();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadCristales();
        }

        private void seleccionarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(datagridviewNE1.RowCount > 0)
            {
                string acabado = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                constants.factory_acabado_perfil =  comboBox1.SelectedIndex + "," + acabado;
                label3.Text = acabado;
                button1.Visible = true;
                if (comboBox1.SelectedIndex == 0)
                {
                    constants.setImage("acabados_perfil", acabado, "jpg", pictureBox1);
                }
                else if(comboBox1.SelectedIndex == 1)
                {
                    constants.setImage("acabados_especiales", acabado, "jpg", pictureBox1);
                }
                saveToXML();
            }
        }

        private void seleccionarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(datagridviewNE2.RowCount > 0)
            {
                string clave = datagridviewNE2.CurrentRow.Cells[0].Value.ToString();
                string articulo = datagridviewNE2.CurrentRow.Cells[1].Value.ToString();
                constants.factory_cristal = clave + "," + articulo; ;
                label4.Text = articulo;
                button2.Visible = true;
                saveToXML();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            constants.factory_acabado_perfil = string.Empty;
            pictureBox1.Image = null;
            label3.Text = "";
            button1.Visible = false;
            saveToXML();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            constants.factory_cristal = string.Empty;
            label4.Text = "";
            button2.Visible = false;
            saveToXML();
        }

        private void saveToXML()
        {
            try
            {
                XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                var mv = from x in opciones_xml.Descendants("Opciones") select x;

                foreach (XElement x in mv)
                {
                    x.SetElementValue("FACTORY_ALU", constants.factory_acabado_perfil);
                    x.SetElementValue("FACTORY_CRI", constants.factory_cristal);                  
                }
                opciones_xml.Save(constants.opciones_xml);
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
