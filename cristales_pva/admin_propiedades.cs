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
            textBox1.TextChanged += TextBox1_TextChanged;
            textBox4.TextChanged += TextBox4_TextChanged;
            if(constants.user_access < 6 || constants.local == true)
            {
                textBox4.Enabled = false;
            }
            else
            {
                textBox4.Enabled = true;
            }
        }

        private void TextBox4_TextChanged(object sender, EventArgs e)
        {
            constants.CheckInputIntegerValue(textBox4);
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            constants.CheckInputIntegerValue(textBox1);
        }

        private void admin_propiedades_Load(object sender, EventArgs e)
        {
            textBox1.Text = constants.updater_interval.ToString();
            textBox4.Text = Math.Round((constants.getPropiedadesModel() - 1) * 100, 2).ToString();
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
            //modalidad liva
            if (constants.m_liva == true)
            {
                checkBox8.Checked = true;
            }
            else
            {
                checkBox8.Checked = false;
            }
            //preguntar si se desea actualizar
            if (constants.p_ac == true)
            {
                checkBox9.Checked = true;
            }
            else
            {
                checkBox9.Checked = false;
            }
            //preguntar si se desea actualizar
            if (constants.enable_c_tc == true)
            {
                checkBox10.Checked = true;
            }
            else
            {
                checkBox10.Checked = false;
            }
            //anuncios
            if (constants.anuncios == true)
            {
                checkBox11.Checked = true;
            }
            else
            {
                checkBox11.Checked = false;
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
                            MessageBox.Show(this, "[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                x.SetElementValue("MLIVA", getMLIVA());
                                x.SetElementValue("PAC", getPAC());
                                x.SetElementValue("EATCC", getATCC());
                                x.SetElementValue("ANC", getAnuncios());
                            }
                            opciones_xml.Save(constants.opciones_xml);
                        }
                        catch (Exception err)
                        {
                            constants.errorLog(err.ToString());
                            MessageBox.Show(this, "[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        if (constants.user_access == 6 && constants.local == false)
                        {
                            try
                            {
                                sqlDateBaseManager sql = new sqlDateBaseManager();
                                localDateBaseEntities3 p = new localDateBaseEntities3();
                                int input = constants.stringToInt(textBox4.Text);
                                if (input > 0)
                                {
                                    float iva = ((float)constants.stringToInt(textBox4.Text) / 100) + 1;
                                    if (constants.iva_desglosado == true)
                                    {
                                        constants.iva = (float)Math.Round(iva, 2);
                                    }
                                    if(MessageBox.Show(this, "¿Deseas actualizar el válor del IVA en la base de datos?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        sql.updatePropiedades(iva);
                                    }
                                    else
                                    {
                                        MessageBox.Show(this, "El válor del IVA se a modificado de manera local y volverá a tomar su válor desde la base de datos una vez reiniciado o actualizado el programa.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    constants.setPropiedadesModel(iva);
                                    //-------------------------------->
                                    ((Form1)Application.OpenForms["form1"]).reloadIVA();
                                    ((Form1)Application.OpenForms["form1"]).calcularTotalesCotizacion();
                                }
                                else
                                {
                                    MessageBox.Show(this, "[Error]: dato de IVA no válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            catch (Exception err)
                            {
                                constants.errorLog(err.ToString());
                                MessageBox.Show(this, "[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        // ---------------------------------------------------------------------------------------------------
                        MessageBox.Show(this, "Se necesita reiniciar el programa para ver algunos cambios.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(this, "[Error]: dato de IVA no válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(this, "[Error]: intervalo no válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "[Error]: intervalo no válido.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private string getMLIVA()
        {
            if (checkBox8.Checked == true)
            {
                constants.m_liva = true;
                ((Form1)Application.OpenForms["form1"]).setModoLIVA(true);
                return "true";
            }
            else
            {
                constants.m_liva = false;
                ((Form1)Application.OpenForms["form1"]).disableModoLIVA();
                return "false";
            }
        }

        private string getPAC()
        {
            if (checkBox9.Checked == true)
            {
                constants.p_ac = true;
                return "true";
            }
            else
            {
                constants.p_ac = false;
                constants.reload_precios = true;
                return "false";
            }
        }

        private string getATCC()
        {
            if (checkBox10.Checked == true)
            {
                constants.enable_c_tc = true;
                return "true";
            }
            else
            {
                constants.enable_c_tc = false;
                return "false";
            }
        }

        private string getAnuncios()
        {
            if (checkBox11.Checked == true)
            {
                constants.anuncios = true;
                return "true";
            }
            else
            {
                constants.anuncios = false;
                return "false";
            }
        }

        ~admin_propiedades()
        {

        }
    }
}
