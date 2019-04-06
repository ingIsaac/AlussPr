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
    public partial class setDir : Form
    {
        int id;
        int perso_id;
        bool remove;
        bool reload;
        public bool close = false;
        string dir = string.Empty;

        public setDir(int id, int perso_id, Image img, float largo, float alto, bool remove = false, bool reload = false)
        {
            InitializeComponent();
            this.id = id;
            this.perso_id = perso_id;
            this.remove = remove;
            this.reload = reload;
            pictureBox1.Image = img;
            textBox1.Text = largo.ToString();
            textBox2.Text = alto.ToString();
            label3.Text = "*Nota: Para asignar de forma manual la medida total, seleccione la configuración de manera indefinida.";
            loadTable();
            comboBox1.Text = "1";
            comboBox2.Text = "1";
        }
       
        private void loadTable()
        {
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

            for (int i = 0; i < (tableLayoutPanel1.RowCount * tableLayoutPanel1.ColumnCount); i++)
            {
                PictureBox a1 = new PictureBox();
                a1.Dock = DockStyle.Fill;
                a1.InitialImage = Properties.Resources.loading_gif;
                a1.SizeMode = PictureBoxSizeMode.StretchImage;
                a1.BackgroundImageLayout = ImageLayout.Stretch;
                a1.Margin = new Padding(0, 0, 0, 0);
                a1.BackColor = Color.White;
                tableLayoutPanel1.Controls.Add(a1);
            }
        }

        //Columna
        private void button1_Click(object sender, EventArgs e)
        {
            ((Form1)Application.OpenForms["form1"]).setArticuloPersonalizacion(id, perso_id, constants.stringToInt(dir), remove);
            close = true;
            if (reload)
            {
                if (Application.OpenForms["merge_items"] != null)
                {
                    ((merge_items)Application.OpenForms["merge_items"]).reloadMergedItems();
                }
                if(Application.OpenForms["articulos_cotizacion"] != null)
                {
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL();
                }
            }
            Close();
        }

        //Fila
        private void button2_Click(object sender, EventArgs e)
        {
            ((Form1)Application.OpenForms["form1"]).setArticuloPersonalizacion(id, perso_id, constants.stringToInt(dir), remove);
            close = true;
            if (reload)
            {
                if (Application.OpenForms["merge_items"] != null)
                {
                    ((merge_items)Application.OpenForms["merge_items"]).reloadMergedItems();
                }
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL();
                }
            }
            Close();
        }

        private void getNext(bool row)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            string dir;
            var z = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == perso_id select x);
            int x_0 = 0;
            int y_0 = 0;
            bool r = false;

            if (z != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (row)
                    {
                        dir = (comboBox1.Text + (i+1).ToString() + (label7.Text == "Fila" ? "0" : "1"));
                    }
                    else
                    {  
                        dir = ((i+1).ToString() + comboBox2.Text + (label7.Text == "Fila" ? "0" : "1"));
                    }

                    r = false;

                    foreach (var x in z)
                    {
                        if (x.dir >= 110)
                        {
                            tableLayoutPanel1.GetControlFromPosition(constants.stringToInt(x.dir.ToString()[1].ToString()) - 1, constants.stringToInt(x.dir.ToString()[0].ToString()) - 1).BackColor = Color.Red;
                            string _dir = x.dir.ToString();
                            if ("" + _dir[0].ToString() + _dir[1].ToString() == "" + dir[0] + dir[1])
                            {
                                r = true;
                                break;
                            }
                        }
                    }

                    if(r == false)
                    {
                        this.dir = dir;
                        label6.Text = dir[0] + "," + dir[1];
                        if (row)
                        {
                            x_0 = constants.stringToInt(comboBox1.Text);
                            if (x_0 > 0)
                            {
                                tableLayoutPanel1.GetControlFromPosition(i, x_0 - 1).BackColor = Color.Green;
                            }
                        }
                        else
                        {
                            y_0 = constants.stringToInt(comboBox2.Text);
                            if (y_0 > 0)
                            {
                                tableLayoutPanel1.GetControlFromPosition(y_0 - 1, i).BackColor = Color.Green;
                            }
                        }
                        break;
                    }
                }
            }
        }

        //Indefinido
        private void button3_Click(object sender, EventArgs e)
        {
            ((Form1)Application.OpenForms["form1"]).setArticuloPersonalizacion(id, perso_id, 0, remove);
            close = true;
            if (reload)
            {
                if (Application.OpenForms["merge_items"] != null)
                {
                    ((merge_items)Application.OpenForms["merge_items"]).reloadMergedItems();
                }
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL();
                }
            }
            Close();
        }

        private void setCoords(bool row)
        {
            foreach(Control p in tableLayoutPanel1.Controls)
            {
                p.BackColor = Color.White;
            }

            int x = constants.stringToInt(comboBox1.Text);
            int y = constants.stringToInt(comboBox2.Text);

            if (x > 0 && y > 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (!row)
                    {
                        tableLayoutPanel1.GetControlFromPosition(y - 1, i).BackColor = Color.LightBlue;
                    }
                    else
                    {
                        tableLayoutPanel1.GetControlFromPosition(i, x - 1).BackColor = Color.LightBlue;
                    }                   
                }
            }                                    
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label7.Text = "Vertical";
            setCoords(false);
            getNext(false);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label7.Text = "Horizontal";
            setCoords(true);
            getNext(true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label7.Text = "Horizontal";
            setCoords(true);
            getNext(true);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label7.Text = "Vertical";
            setCoords(false);
            getNext(false);
        }
    }
}
