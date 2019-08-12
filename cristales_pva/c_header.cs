using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;

namespace cristales_pva
{
    public partial class c_header : Form
    {
        string path = Application.StartupPath + "\\pics\\reportes";

        public c_header()
        {
            InitializeComponent();
            loadHeaders();
        }

        private void loadHeaders()
        {
            try
            {
                comboBox1.Items.Clear();
                string[] files = Directory.GetFiles(path, "*.jpg").Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
                foreach (string x in files)
                {
                    comboBox1.Items.Add(x);
                }
                comboBox1.Text = constants.header_reporte;
            }
            catch(Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != string.Empty)
            {
                DialogResult q = MessageBox.Show("¿Desea guardar los cambios de manera permanente?", constants.msg_box_caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (q == DialogResult.Yes)
                {
                    if (saveNewHeader(comboBox1.Text))
                    {
                        MessageBox.Show("Se ha guardado con exito el nuevo encabezado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                }
                else if(q == DialogResult.No)
                {
                    constants.header_reporte = comboBox1.Text;
                }
                else
                {
                    //Do nothing
                }
            }
            else
            {
                MessageBox.Show("[Error] Debés seleccionar un encabezado si deseas guardar.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.ImageLocation = path + "\\" + comboBox1.Text + ".jpg";
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show("[Error] No se ha podido procesar la imagen, intente de nuevo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //Marca de Agua
            pictureBox2.ImageLocation = path + "\\fondos\\" + comboBox1.Text + ".bmp";
        }

        private bool saveNewHeader(string header)
        {
            bool r = false;
            try
            {
                XDocument propiedades_xml = XDocument.Load(constants.propiedades_xml);

                var propiedades = from x in propiedades_xml.Descendants("Propiedades") select x;

                foreach (XElement x in propiedades)
                {
                    x.SetElementValue("ORG", header);
                }
                propiedades_xml.Save(constants.propiedades_xml);
                constants.header_reporte = header;
                r = true;
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show(this, "[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return r;
        }
    }
}
