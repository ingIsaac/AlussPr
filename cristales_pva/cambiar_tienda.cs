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
    public partial class cambiar_tienda : Form
    {
        public cambiar_tienda()
        {
            InitializeComponent();
        }

        private void cambiar_tienda_Load(object sender, EventArgs e)
        {
            constants.setTiendas(comboBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != string.Empty)
            {
                try
                {
                    XDocument propiedades_xml = XDocument.Load(constants.propiedades_xml);
                    string alias = string.Empty;

                    alias = new sqlDateBaseManager().getAliasTienda(comboBox1.Text);

                    if (alias != string.Empty)
                    {
                        var propiedades = from x in propiedades_xml.Descendants("Propiedades") select x;

                        foreach (XElement x in propiedades)
                        {
                            x.SetElementValue("ORG", alias);
                            x.SetElementValue("ORG_N", comboBox1.Text);
                        }
                        propiedades_xml.Save(constants.propiedades_xml);

                        DialogResult r = MessageBox.Show(this, "El programa debe ser reiniciado. ¿Deseas reiniciar ahora mismo?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (r == DialogResult.Yes)
                        {
                            Environment.Exit(0);
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "[Error] no se pudo encontrar el 'alias' de esta tienda.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                    MessageBox.Show(this, "[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "[Error] necesitas seleccionar una tienda.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
