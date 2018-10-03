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
    public partial class config_mail : Form
    {
        public config_mail()
        {
            InitializeComponent();
            textBox1.Text = constants.smtp;
            textBox2.Text = constants.m_port.ToString();
            textBox3.Text = constants.timeout.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty && textBox2.Text != string.Empty && textBox3.Text != string.Empty)
            {
                try
                {
                    XDocument propiedades_xml = XDocument.Load(constants.propiedades_xml);

                    var propiedades = from x in propiedades_xml.Descendants("Propiedades") select x;

                    foreach (XElement x in propiedades)
                    {
                        x.SetElementValue("SMTP", textBox1.Text);
                        x.SetElementValue("M_PORT", constants.stringToInt(textBox2.Text));
                        x.SetElementValue("TIMEOUT", constants.stringToInt(textBox3.Text));
                    }
                    propiedades_xml.Save(constants.propiedades_xml);
                    constants.smtp = textBox1.Text;
                    constants.m_port = constants.stringToInt(textBox2.Text);
                    constants.timeout = constants.stringToInt(textBox3.Text);
                    MessageBox.Show(this, "Se ha guardado la nueva configuración.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception err)
                {
                    constants.errorLog(err.ToString());
                    MessageBox.Show(this, "[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "[Error] necesitas introducir todos los datos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
