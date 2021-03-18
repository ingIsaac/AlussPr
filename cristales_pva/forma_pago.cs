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
    public partial class forma_pago : Form
    {
        public forma_pago()
        {
            InitializeComponent();
            try
            {
                XDocument propiedades_xml = XDocument.Load(constants.propiedades_xml);

                var fp = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("FP")).SingleOrDefault();

                if(fp != null)
                {
                    richTextBox1.Text = fp.Value;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(this, "[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {        
            try
            {
                XDocument propiedades_xml = XDocument.Load(constants.propiedades_xml);

                var mv = from x in propiedades_xml.Descendants("Propiedades") select x;

                foreach (XElement x in mv)
                {
                    x.SetElementValue("FP", richTextBox1.Text);
                }

                propiedades_xml.Save(constants.propiedades_xml);
                MessageBox.Show(this, "Se han guardado los cambios.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                ((reportes)Application.OpenForms["reportes"]).reload();
                Close();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show(this, "[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }           
        }
    }
}
