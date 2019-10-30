using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml.Linq;

namespace cristales_pva
{
    public partial class update : Form
    {
        public update(string new_version)
        {
            InitializeComponent();
            if (new_version != "")
            {
                label2.Text = "versión: " + new_version;
            }
            else
            {
                Close();
            }
            this.FormClosed += Update_FormClosed;
        }

        private void Update_FormClosed(object sender, FormClosedEventArgs e)
        {
            constants.update_later = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (z, y) =>
            {
                try
                {
                    label3.Text = "Recopilando datos...";
                    string rs = "false";
                    string esquemas = "false";
                    if (new sqlDateBaseManager().getNewConfigurationUpdate() == true)
                    {
                        rs = "true";
                    }
                    if (new sqlDateBaseManager().getNewConfigurationUpdateEsquemas() == true)
                    {
                        esquemas = "true";
                    }
                    XDocument updater_xml = XDocument.Load(constants.updater_xml);

                    var updater = from x in updater_xml.Descendants("UPDATER") select x;

                    foreach (XElement x in updater)
                    {
                        x.SetElementValue("ALL-DATA", rs);
                        x.SetElementValue("SCHEMAS", esquemas);
                    }
                    updater_xml.Save(constants.updater_xml);
                    y.Result = true;
                }
                catch (Exception err)
                {
                    label3.Text = "Error.";
                    constants.errorLog(err.ToString());
                    MessageBox.Show("[Error] el archivo data_updater.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            bg.RunWorkerCompleted += (z, y) =>
            {
                if(y.Result != null)
                {
                    if((bool)y.Result)
                    {
                        try
                        {
                            label3.Text = "Abriendo software de actualización...";
                            Process.Start(Application.StartupPath + "\\updater.exe");
                            //Exit
                            System.Environment.Exit(1);
                        }
                        catch (Exception err)
                        {
                            label3.Text = "Error.";
                            constants.errorLog(err.ToString());
                            MessageBox.Show("[Error] no se encontro el archivo updater.exe en la carpeta el programa.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }                        
                    }
                }
            };
            if(!bg.IsBusy)
            {
                bg.RunWorkerAsync();
            }                 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
