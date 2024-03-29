﻿using System;
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
    public partial class cm_opciones : Form
    {
        public cm_opciones()
        {
            InitializeComponent();
            //-->
            textBox1.TextChanged += TextBox1_TextChanged;
            textBox2.TextChanged += TextBox2_TextChanged;
            textBox3.TextChanged += TextBox3_TextChanged;
            textBox4.TextChanged += TextBox4_TextChanged;
        }

        private void TextBox4_TextChanged(object sender, EventArgs e)
        {
            constants.CheckInputIntegerValue(textBox4);
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            constants.CheckInputIntegerValue(textBox3);
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            constants.CheckInputIntegerValue(textBox2);
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            constants.CheckInputIntegerValue(textBox1);
        }

        private void cm_opciones_Load(object sender, EventArgs e)
        {
            string[] parametros = new string[] {};
            try
            {
                XDocument opciones = XDocument.Load(constants.opciones_xml);

                var op = (from x in opciones.Descendants("Opciones") select x.Element("CM")).SingleOrDefault();

                if (op != null)
                {
                    parametros = op.Value.Split(',');
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }

            if (parametros.Length == 4)
            {
                textBox1.Text = constants.stringToFloat(parametros[0]).ToString();
                textBox2.Text = constants.stringToFloat(parametros[1]).ToString();
                textBox3.Text = constants.stringToFloat(parametros[2]).ToString();
                textBox4.Text = constants.stringToFloat(parametros[3]).ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                var opciones = from x in opciones_xml.Descendants("Opciones") select x;

                foreach (XElement x in opciones)
                {
                   x.SetElementValue("CM", constants.stringToInt(textBox1.Text) + "," + constants.stringToInt(textBox2.Text) + "," + constants.stringToInt(textBox3.Text) + "," + constants.stringToInt(textBox4.Text));
                }
                opciones_xml.Save(constants.opciones_xml);
                if (Application.OpenForms["config_modulo"] != null)
                {
                    ((config_modulo)Application.OpenForms["config_modulo"]).setNewParameters(constants.stringToInt(textBox1.Text).ToString(), constants.stringToInt(textBox2.Text).ToString(), constants.stringToInt(textBox3.Text).ToString(), constants.stringToInt(textBox4.Text).ToString());
                }
                this.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
        }
    }
}
