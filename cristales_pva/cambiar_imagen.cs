using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace cristales_pva
{
    public partial class cambiar_imagen : Form
    {
        List<string> esquemas = new List<string>();
        int id;
        Bitmap pic = null;
        string acabado = string.Empty;
        int desing = 0;
        bool enable = false;
        sqlDateBaseManager search = new sqlDateBaseManager();

        public cambiar_imagen(int id)
        {
            InitializeComponent();
            datagridviewNE1.CellEnter += DatagridviewNE1_CellEnter;
            datagridviewNE1.CellClick += DatagridviewNE1_CellClick;
            this.id = id;
        }

        private void DatagridviewNE1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (enable == true)
            {
                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image = null;
                }
                desing = constants.stringToInt(datagridviewNE1.CurrentRow.Cells[0].Value.ToString());
                LeerDiseño(desing);
            }          
        }

        private void DatagridviewNE1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (enable == false)
            {
                enable = true;
            }
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image = null;
            }
            desing = constants.stringToInt(datagridviewNE1.CurrentRow.Cells[0].Value.ToString());
            LeerDiseño(desing);
        }

        //Leer diseño
        private void LeerDiseño(int id_diseño)
        {
            listas_entities_pva listas = new listas_entities_pva();
            var diseño = (from x in listas.esquemas where x.id == id_diseño select x).SingleOrDefault();
            if (diseño != null)
            {
                tableLayoutPanel1.Controls.Clear();
                label1.Text = diseño.id + "-" + diseño.nombre;
                getEsquemasFromDiseño(diseño.esquemas);
                tableLayoutPanel1.RowCount = (int)diseño.filas;
                tableLayoutPanel1.ColumnCount = (int)diseño.columnas;
                label3.Text = "Diseño: (" + diseño.diseño + ")";
                foreach (string e in esquemas)
                {
                    if (File.Exists(constants.folder_resources_dir + "esquemas\\corredizas\\" + e + ".png"))
                    {
                        constants.loadDiseño("esquemas\\corredizas\\", e, tableLayoutPanel1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\puertas\\" + e + ".png"))
                    {
                        constants.loadDiseño("esquemas\\puertas\\", e, tableLayoutPanel1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_abatibles\\" + e + ".png"))
                    {
                        constants.loadDiseño("esquemas\\ventanas_abatibles\\", e, tableLayoutPanel1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\ventanas_fijas\\" + e + ".png"))
                    {
                        constants.loadDiseño("esquemas\\ventanas_fijas\\", e, tableLayoutPanel1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\templados\\" + e + ".png"))
                    {
                        constants.loadDiseño("esquemas\\templados\\", e, tableLayoutPanel1);
                    }
                    else if (File.Exists(constants.folder_resources_dir + "esquemas\\otros\\" + e + ".png"))
                    {
                        constants.loadDiseño("esquemas\\otros\\", e, tableLayoutPanel1);
                    }
                }
                if (diseño.marco == true)
                {
                    if (diseño.grupo == "puerta")
                    {
                        tableLayoutPanel1.Padding = new Padding(10, 10, 10, 0);
                    }
                    else
                    {
                        tableLayoutPanel1.Padding = new Padding(10, 10, 10, 10);
                    }
                }
                else
                {
                    tableLayoutPanel1.Padding = new Padding(0, 0, 0, 0);
                }
                tableLayoutPanel1.RowStyles.Clear();
                for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
                {
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100 / tableLayoutPanel1.RowCount));
                }
                tableLayoutPanel1.ColumnStyles.Clear();
                for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
                {
                    ColumnStyle sty = new ColumnStyle(SizeType.Percent, 100 / tableLayoutPanel1.ColumnCount);
                    tableLayoutPanel1.ColumnStyles.Add(sty);
                }
                checkAcabado(acabado);
            }
        }

        //get esquemas
        private void getEsquemasFromDiseño(string diseño_esquemas)
        {
            esquemas.Clear();
            string buffer = string.Empty;
            foreach (char x in diseño_esquemas)
            {
                if (x == ',')
                {
                    esquemas.Add(buffer);
                    buffer = string.Empty;
                    continue;
                }
                buffer = buffer + x.ToString();
            }
        }

        //cambiar imagen
        private void button2_Click(object sender, EventArgs e)
        {
            if (tableLayoutPanel1.Controls.Count > 0 || pictureBox1.Image != null) {
                cotizaciones_local cotizaciones = new cotizaciones_local();

                var data = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();

                if (data != null)
                {
                    if (tableLayoutPanel1.Controls.Count > 0)
                    {
                        Bitmap bm = new Bitmap(tableLayoutPanel1.Width, tableLayoutPanel1.Height);
                        tableLayoutPanel1.DrawToBitmap(bm, new Rectangle(0, 0, tableLayoutPanel1.Width, tableLayoutPanel1.Height));
                        Bitmap gm_2 = new Bitmap(bm, 120, 105);
                        data.pic = constants.imageToByte(gm_2);
                        data.dir = 3;
                        data.new_desing = desing > 0 ? desing.ToString() : "";
                        cotizaciones.SaveChanges();
                        bm = null;
                        gm_2 = null;
                    }
                    else if (pictureBox1.Image != null)
                    {
                        data.pic = constants.imageToByte(pictureBox1.Image);
                        data.dir = 3;
                        cotizaciones.SaveChanges();
                    }
                    if (Application.OpenForms["articulos_cotizacion"] != null)
                    {
                        ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).reloadModulos();
                    }
                    if (Application.OpenForms["edit_expresss"] != null)
                    {
                        ((edit_expresss)Application.OpenForms["edit_expresss"]).reloadALL();
                    }
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("[Error] no se ha seleccionado un esquema o imagen.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Buscar Esquema
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            new sqlDateBaseManager().dropTableOnGridViewWithFilter(datagridviewNE1, "esquemas", "grupo", comboBox1.Text);
        }

        //Buscar imagen
        private void button1_Click(object sender, EventArgs e)
        {
            if (tableLayoutPanel1.Controls.Count > 0)
            {
                tableLayoutPanel1.Controls.Clear();
                tableLayoutPanel1.ColumnCount = 1;
                tableLayoutPanel1.RowCount = 1;
                tableLayoutPanel1.BackgroundImage = null;
            }
            openFileDialog1.Title = "Selecciona una imagen.";
            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg) | *.jpg; *.jpeg";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap bm = new Bitmap(openFileDialog1.FileName);
                pic = new Bitmap(bm, 120, 105);
                bm = null;
                pictureBox1.Image = pic;
                pic = null;
            }
        }

        //Diseño personalizado
        private void button3_Click(object sender, EventArgs e)
        {
            new diseño_p(id).ShowDialog();
            this.Close();
        }

        //acabado
        private void loadColoresAluminio()
        {
            listas_entities_pva listas = new listas_entities_pva();

            var colores = from x in listas.colores_aluminio select x;

            if (colores != null)
            {
                comboBox2.Items.Clear();
                foreach (var c in colores)
                {
                    comboBox2.Items.Add(c.clave);
                }
            }
        }

        private void checkAcabado(string acabado)
        {
            if (acabado != "")
            {
                if (constants.imageExist("acabados_especiales", acabado, "jpg") == true)
                {
                    for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
                    {
                        constants.setBackgroundImg("acabados_especiales", acabado, "jpg", tableLayoutPanel1.Controls[i]);
                    }
                    constants.setBackgroundImg("acabados_especiales", acabado, "jpg", tableLayoutPanel1);
                }
                else
                {
                    for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
                    {
                        constants.setBackgroundImg("acabados_perfil", acabado, "jpg", tableLayoutPanel1.Controls[i]);
                    }
                    constants.setBackgroundImg("acabados_perfil", acabado, "jpg", tableLayoutPanel1);
                }
            }   
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tableLayoutPanel1.Controls.Count > 0 && comboBox2.SelectedIndex >= 0)
            {
                acabado = string.Empty;
                for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
                {
                    constants.setBackgroundImg("acabados_especiales", comboBox2.Text, "jpg", tableLayoutPanel1.Controls[i]);
                }
                constants.setBackgroundImg("acabados_especiales", comboBox2.Text, "jpg", tableLayoutPanel1);
            }
            else
            {
                comboBox2.SelectedIndex = -1;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            var data = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();

            if (data != null)
            {
                data.dir = 0;
                cotizaciones.SaveChanges();
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).reloadModulos();
                }
                if (Application.OpenForms["edit_expresss"] != null)
                {
                    ((edit_expresss)Application.OpenForms["edit_expresss"]).reloadALL();
                }
                this.Close();
            }
            //
        }

        private void cambiar_imagen_Load(object sender, EventArgs e)
        {
            new sqlDateBaseManager().dropTableOnGridView(datagridviewNE1, "esquemas");
            datagridviewNE1.ClearSelection();
            loadColoresAluminio();
            
            cotizaciones_local cotizaciones = new cotizaciones_local();
            listas_entities_pva listas = new listas_entities_pva();

            var data = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();

            if (data != null)
            {
                if (data.modulo_id != -2)
                {
                    comboBox2.Enabled = false;
                    acabado = data.acabado_perfil;
                    if(data.modulo_id >= 0)
                    {
                        int modulo_id = (int)data.modulo_id;
                        var modulo = (from x in listas.modulos where x.id == modulo_id select x).SingleOrDefault();
                        if(modulo != null)
                        {
                            int diseño_id = (int)modulo.id_diseño;
                            LeerDiseño(diseño_id);
                        }
                    }
                    if (data.new_desing != "" && data.new_desing != null)
                    {
                        if (constants.stringToInt(data.new_desing) > 0)
                        {
                            desing = constants.stringToInt(data.new_desing);
                            LeerDiseño(desing);
                        }
                    }
                }
            }
        }

        //Sin imagen
        private void button5_Click(object sender, EventArgs e)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            var data = (from x in cotizaciones.modulos_cotizaciones where x.id == id select x).SingleOrDefault();

            if (data != null)
            {
                data.pic = constants.imageToByte(Properties.Resources.blank);
                data.dir = 3;
                cotizaciones.SaveChanges();
                
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).reloadModulos();
                }
                if (Application.OpenForms["edit_expresss"] != null)
                {
                    ((edit_expresss)Application.OpenForms["edit_expresss"]).reloadALL();
                }
                this.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           search.dropTableOnGridViewWithFilter(datagridviewNE1, "esquemas", "nombre", textBox1.Text);
        }
    }
}
