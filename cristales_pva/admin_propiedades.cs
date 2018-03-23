using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Xml.Linq;
using System.Linq;
using System.Windows.Forms;

namespace cristales_pva
{
    public partial class admin_propiedades : Form
    {
        public admin_propiedades()
        {
            InitializeComponent();
            if(constants.user_access < 6 || constants.local == true)
            {
                textBox4.Enabled = false;
            }
            else
            {
                textBox4.Enabled = true;
            }
        }      

        private void admin_propiedades_Load(object sender, EventArgs e)
        {
            textBox1.Text = constants.updater_interval.ToString();
            textBox4.Text = ((constants.getPropiedadesModel() - 1) * 100).ToString();
            //updater
            if (constants.updater_enable == false)
            {
                checkBox1.Checked = false;
            }
            else
            {
                checkBox1.Checked = true;
            }
            //update when close a form
            if (constants.updater_form_close == false)
            {
                checkBox2.Checked = false;
            }
            else
            {
                checkBox2.Checked = true;
            }
            //Maximizar ventanas
            if (constants.maximizar_ventanas == true)
            {
                checkBox3.Checked = true;
            }
            else
            {
                checkBox3.Checked = false;
            }
            //optimizar base de datos al inicio
            if (constants.optimizar_inicio == true)
            {
                checkBox4.Checked = true;
            }
            else
            {
                checkBox4.Checked = false;
            }
            //mostrar acabado en reporte
            if (constants.mostrar_acabado == true)
            {
                checkBox5.Checked = true;
            }
            else
            {
                checkBox5.Checked = false;
            }
            //actualizar cotizacion al abrir
            if (constants.ac_cotizacion == true)
            {
                checkBox6.Checked = true;
            }
            else
            {
                checkBox6.Checked = false;
            }
            //permitir ajuste iva
            if (constants.permitir_ajuste_iva == true)
            {
                checkBox7.Checked = true;
            }
            else
            {
                checkBox7.Checked = false;
            }           
        }

        private void button1_Click(object sender, EventArgs e)
        {          
            if (constants.isInteger(textBox1.Text) == true)
            {
                if (int.Parse(textBox1.Text) > 0)
                {
                    if (constants.isFloat(textBox4.Text))
                    {
                        //-------------------------------------------------------------------------------------------------
                        try
                        {
                            XDocument propiedades_xml = XDocument.Load(constants.propiedades_xml);

                            var propiedades = from x in propiedades_xml.Descendants("Propiedades") select x;

                            foreach (XElement x in propiedades)
                            {
                                x.SetElementValue("UC", getEnableCloseForm());
                                x.SetElementValue("UDE", getEnable());
                                x.SetElementValue("UDT", textBox1.Text);
                            }
                            propiedades_xml.Save(constants.propiedades_xml);
                            constants.updater_interval = constants.stringToInt(textBox1.Text);
                        }
                        catch (Exception err)
                        {
                            constants.errorLog(err.ToString());
                            MessageBox.Show("[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        try
                        {
                            XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                            var mv = from x in opciones_xml.Descendants("Opciones") select x;

                            foreach (XElement x in mv)
                            {
                                x.SetElementValue("MV", getMV());
                                x.SetElementValue("OPI", getOptimizacionMode());
                                x.SetElementValue("MA", getMostrarAcabado());
                                x.SetElementValue("ACC", getAC_Cotizacion());
                                x.SetElementValue("PAI", getPAI());
                            }
                            opciones_xml.Save(constants.opciones_xml);
                        }
                        catch (Exception err)
                        {
                            constants.errorLog(err.ToString());
                            MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        if (constants.user_access == 6 && constants.local == false)
                        {
                            try
                            {
                                sqlDateBaseManager sql = new sqlDateBaseManager();
                                localDateBaseEntities3 p = new localDateBaseEntities3();
                                float iva = (float.Parse(textBox4.Text) / 100) + 1;
                                if (constants.iva_desglosado == true)
                                {
                                    constants.iva = iva;
                                }
                                sql.updatePropiedades(iva);
                                constants.setPropiedadesModel(iva);
                                ((Form1)Application.OpenForms["form1"]).reloadIVA();
                            }
                            catch (Exception err)
                            {
                                constants.errorLog(err.ToString());
                                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        // ---------------------------------------------------------------------------------------------------
                        MessageBox.Show("Se necesita reiniciar el programa para ver algunos cambios.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("[Error]: dato de IVA no válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("[Error]: intervalo no válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("[Error]: intervalo no válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }                   
        }

        private float stringToFloat(string num)
        {
            float r = 0;
            if (float.TryParse(num, out r) == true)
            {
                return r;
            }
            else
            {
                return 0;
            }
        }       

        private string getEnable()
        {
            if(checkBox1.Checked == true)
            {
                constants.updater_enable = true;
                return "true";
            }
            else
            {
                constants.updater_enable = false;
                return "false";
            }
        }

        private string getEnableCloseForm()
        {
            if (checkBox2.Checked == true)
            {
                constants.updater_form_close = true;
                return "true";
            }
            else
            {
                constants.updater_form_close = false;
                return "false";
            }
        }

        private string getMV()
        {
            if (checkBox3.Checked == true)
            {
                constants.maximizar_ventanas = true;
                return "true";
            }
            else
            {
                constants.maximizar_ventanas = false;
                return "false";
            }
        }

        private string getOptimizacionMode()
        {
            if (checkBox4.Checked == true)
            {
                constants.optimizar_inicio = true;
                return "true";
            }
            else
            {
                constants.optimizar_inicio = false;
                return "false";
            }
        }

        private string getMostrarAcabado()
        {
            if (checkBox5.Checked == true)
            {
                constants.mostrar_acabado = true;
                return "true";
            }
            else
            {
                constants.mostrar_acabado = false;
                return "false";
            }
        }

        private string getAC_Cotizacion()
        {
            if (checkBox6.Checked == true)
            {
                constants.ac_cotizacion = true;
                return "true";
            }
            else
            {
                constants.ac_cotizacion = false;
                return "false";
            }
        }

        private string getPAI()
        {
            if (checkBox7.Checked == true)
            {
                constants.permitir_ajuste_iva = true;
                ((Form1)Application.OpenForms["form1"]).permitirAjusteIVA(true);
                return "true";
            }
            else
            {
                constants.permitir_ajuste_iva = false;
                ((Form1)Application.OpenForms["form1"]).permitirAjusteIVA(false);
                return "false";
            }
        }

        ~admin_propiedades()
        {

        }
    }
}
