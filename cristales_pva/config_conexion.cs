using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace cristales_pva
{
    public partial class config_conexion : Form
    {
        XDocument propiedades_xml = XDocument.Load(constants.propiedades_xml);

        public config_conexion()
        {
            InitializeComponent();
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
        }

        private void config_conexion_Load(object sender, EventArgs e)
        {
            textBox1.Text = constants.server_user;
            textBox5.Text = constants.server_password;
            textBox3.Select();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(backgroundWorker1.IsBusy == false)
            {
                pictureBox1.Visible = true;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        //guardar nuevas credenciales
        private void button1_Click(object sender, EventArgs e)
        {
            localDateBaseEntities3 local = new localDateBaseEntities3();
            var credenciales = (from x in local.logins where x.id == 1 select x).SingleOrDefault();

            if(credenciales != null)
            {
                credenciales.usuario = textBox1.Text;
                credenciales.contraseña = textBox5.Text;
            }
            local.SaveChanges();
            constants.setServerCredentials();
            Close();
        }

        ~config_conexion()
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            constants.user = textBox3.Text;
            constants.password = textBox4.Text;
            sqlDateBaseManager sql = new sqlDateBaseManager();

            if (sql.setServerConnection() == true)
            {
                if (sql.isUserAllowed(constants.user, constants.password) == true)
                {
                    if (constants.user_access == 6)
                    {
                        if (sql.findSQLValue("clave_activacion", "clave_activacion", "claves_activacion", textBox2.Text) == true)
                        {
                            if (sql.findActivation("serial", "serial", "pc_activadas", textBox2.Text) == false)
                            {
                                if (sql.findActivation("mac_pc_activada", "mac_pc_activada", "pc_activadas", constants.getMACAddress()) == false)
                                {
                                    if (constants._false_activation == false)
                                    {
                                        try
                                        {
                                            var mac_id = from x in propiedades_xml.Descendants("Propiedades") select x;
                                            string mac = constants.getMACAddress();
                                            foreach (XElement elm in mac_id)
                                            {
                                                elm.SetElementValue("ID", mac);
                                            }
                                            propiedades_xml.Save(constants.propiedades_xml);
                                            sql.insertNewPCActivation(mac, textBox2.Text, Environment.MachineName);
                                            constants.mac_address = mac;
                                            MessageBox.Show("El equipo ha sido activado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        catch (Exception err)
                                        {
                                            MessageBox.Show("[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            constants.errorLog(err.ToString());
                                        }
                                    }
                                    else
                                    {
                                        constants._false_activation = false;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("[Error] este equipo ya se encuentra activado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("[Error] clave de activación se encuentra en uso.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] clave de activación no valida.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] solo puede tener acceso el administrador.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("[Error] Acceso no autorizado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
        }
    }
}
