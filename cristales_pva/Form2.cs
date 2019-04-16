using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Xml.Linq;

namespace cristales_pva
{
    public partial class Form2 : Form
    {
        localDateBaseEntities3 users;
        BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        bool new_sesion = false;

        public Form2(bool new_sesion)
        {
            InitializeComponent();           
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            comboBox1.KeyPress += ComboBox1_KeyPress;
            if(new_sesion == true)
            {
                checkBox2.Enabled = false;
                this.new_sesion = new_sesion;
            }
        }

        private void ComboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetter(e.KeyChar))
            {
                e.KeyChar = Char.ToUpper(e.KeyChar);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            pictureBox3.Visible = false;
            label3.Visible = false;
            comboBox1.Select();
            //Load User Data
            users = new localDateBaseEntities3();
            try
            {
                var u = (from x in users.userLocals select x.user);

                if (u != null)
                {
                    foreach (var d in u)
                    {
                        comboBox1.Items.Add(d.ToString());
                    }
                }                                        
            }
            catch (Exception err)
            {
                MessageBox.Show(this, "[Error]: <:SQLLocalDB?> Se necesitan instalar los complementos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
                constants.error = true;
            }
            finally
            {
                users.Dispose();
            }       
            ////// -------------------------------------------------------
            if (constants.error == true)
            {
                Environment.Exit(0);
            }
            else
            {
                checkIDStarUp();
            }
            checkBox4.Checked = constants.ingreso_ac;
            if(Application.OpenForms["loading_icon"] != null)
            {
                Application.OpenForms["loading_icon"].Close();
            }
            if (checkBox4.Checked)
            {
                if (MessageBox.Show(this, "¿Deseas ingresar de manera automática?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (constants.user_ac != "" && constants.password_ac != "")
                    {
                        comboBox1.Text = constants.user_ac;
                        textBox2.Text = constants.password_ac;
                        ingresar();
                    }
                    else
                    {
                        MessageBox.Show(this, "[Error] no se tiene configurado un usuario para ingreso automático.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (constants.connected == false)
            {
                Environment.Exit(0);    
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ingresar();
        }       

        private void ingresar()
        {
            if (backgroundWorker1.IsBusy != true)
            {
                constants.user = comboBox1.Text;
                constants.password = textBox2.Text;
                pictureBox3.Visible = true;
                label3.Visible = true;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            if (checkBox2.Checked == true)
            {
                if (localConnection() == true)
                {
                    constants.connected = true;
                    constants.local = true;
                    pictureBox2.Image = Properties.Resources.Computer_icon2;
                }
                else
                {
                    MessageBox.Show(this, "[Error] acceso no autorizado.\n\nEl usuario no está registrado en este equipo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                sqlDateBaseManager sql = new sqlDateBaseManager();

                if (sql.setServerConnection() == true)
                {                   
                    if (sql.isUserAllowed(constants.user, constants.password) == true)
                    {
                        if (!constants.user_forbid)
                        {
                            //Connect to Login Server ------------------------------------------------------------------>
                            if (constants.login_server != null)
                            {
                                if (constants.login_server.Connected)
                                {
                                    constants.login_server.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                                    constants.login_server.Close();
                                }
                            }
                            //------------------------------------------------------------------------------------------>
                            if (constants.setConnectionToLoginServer(constants.user + " - " + constants.org_name, this)) //Try to Connect
                            {
                                string mac = constants.getMACAddress();
                                if (sql.findActivation("mac_pc_activada", "mac_pc_activada", "pc_activadas", mac) == true)
                                {
                                    XDocument propiedades_xml = XDocument.Load(constants.propiedades_xml);
                                    var mac_id = from x in propiedades_xml.Descendants("Propiedades") select x;
                                    foreach (XElement elm in mac_id)
                                    {
                                        elm.SetElementValue("ID", mac);
                                    }
                                    propiedades_xml.Save(constants.propiedades_xml);
                                    constants.mac_address = mac;
                                }
                                if (sql.getActivation() == true)
                                {
                                    if (sql.getTienda(constants.org_name) == true)
                                    {
                                        //Licencia                                      
                                        if (constants.getVigencia(sql.getvigenciaTienda(constants.org_name)))
                                        {
                                            constants.licencia = sql.getvigenciaType(constants.org_name).ToUpper();
                                            if (constants.licencia != string.Empty)
                                            {
                                                constants.connected = true;
                                                pictureBox2.Image = Properties.Resources.database_icon_check;
                                                constants.user_id = sql.getUserId(constants.user);
                                                label3.Text = "Actualizando Historial...";
                                                constants.crearHistorialLogin(constants.user, Environment.MachineName, constants.getPublicIP(), DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                                                users = new localDateBaseEntities3();
                                                var d = (from x in users.userLocals where x.user == comboBox1.Text select x).SingleOrDefault();

                                                if (d == null)
                                                {
                                                    try
                                                    {
                                                        userLocal lu = new userLocal()
                                                        {
                                                            user = comboBox1.Text,
                                                            password = textBox2.Text,
                                                            remember = isRemembered()
                                                        };
                                                        users.userLocals.Add(lu);
                                                        users.SaveChanges();
                                                    }
                                                    catch (Exception err)
                                                    {
                                                        MessageBox.Show(this, "[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                        constants.errorLog(err.ToString());
                                                    }
                                                    finally
                                                    {
                                                        users.Dispose();
                                                    }
                                                }
                                                else
                                                {
                                                    if (d.remember == false && isRemembered() == true)
                                                    {
                                                        d.remember = true;
                                                        users.SaveChanges();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show(this, "[Error] no se a podido identificar el tipo de licencia, ingrese de nuevo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                constants.connected = false;
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show(this, "El periodo de la licencia a expirado, póngase en contacto con el proveedor del sistema.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            constants.connected = false;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show(this, "[Error] no existe registro de esta tienda.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        constants.connected = false;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(this, "[Error] este equipo no se encuentra activado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    constants.connected = false;
                                }
                            }
                            else
                            {
                                MessageBox.Show(this, "[Error] no se ha podido ingresar al login server.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                constants.connected = false;
                            }                                                    
                        }
                        else
                        {
                            MessageBox.Show(this, "[Error] acceso no autorizado.\n\nSe ha restringido el ingreso a este usuario.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            constants.connected = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "[Error] acceso no autorizado.\n\nUsuario o contraseña incorrectos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        constants.connected = false;
                    }
                }               
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {            
            if (constants.connected == true)
            {
                Thread.Sleep(2000);
                pictureBox3.Visible = false;
                label3.Visible = false;
                if (new_sesion == true)
                {
                    ((Form1)Application.OpenForms["Form1"]).ReloadData();
                }                            
                Close();
            }
            else
            {
                pictureBox3.Visible = false;
                label3.Visible = false;
            }                   
        }

        private Boolean isRemembered()
        {
            if(checkBox1.Checked == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Text = string.Empty;
            getRememberedPassword();
        }

        private void getRememberedPassword()
        {
            users = new localDateBaseEntities3();
            try
            {
                var k = (from x in users.userLocals where x.user == comboBox1.Text select x).SingleOrDefault();

                if (k != null)
                {
                    if (k.remember == true)
                    {
                        textBox2.Text = k.password;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(this, "[Error]: <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                users.Dispose();
            }
        }
            
        private void borrarHistorialDeUsuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult r;
            users = new localDateBaseEntities3();
            r = MessageBox.Show(new Form() { TopMost = true }, "¿Estas seguro de eliminar el historial de usuarios?.", constants.msg_box_caption
                , MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {
                try
                {                   
                    users.Database.ExecuteSqlCommand("TRUNCATE TABLE userLocal");
                    comboBox1.Items.Clear();
                }
                catch (Exception err)
                {
                    MessageBox.Show("[Error]: <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    constants.errorLog(err.ToString());
                }
                finally
                {
                    users.Dispose();
                }
            }
        }

        private Boolean localConnection()
        {
            Boolean r = false;
            users = new localDateBaseEntities3();
            try
            {
                var u = (from x in users.userLocals where x.user == comboBox1.Text select x).SingleOrDefault();

                if(u != null)
                {
                    if(u.password == textBox2.Text)
                    {
                        r = true;
                    }                
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error]: <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                users.Dispose();
            }
            return r;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox2.Checked == true)
            {
                pictureBox2.Image = Properties.Resources.Computer_icon;
            }
            else
            {
                pictureBox2.Image = Properties.Resources.database_icon_cross;
            }
        }

        private void configuraciónDeConexiónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            config_conexion conf = new config_conexion();
            conf.ShowDialog();
        }

        private void checkIDStarUp()
        {
            string mac = "";
            try
            {
                XDocument propiedades = XDocument.Load(constants.propiedades_xml);

                var mac_id = (from x in propiedades.Descendants("Propiedades") select x.Element("ID")).SingleOrDefault();
                
                if(mac_id != null)
                {
                    mac = mac_id.Value;
                    constants.mac_address = mac;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(this, "[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }

            if (mac != constants.getMACAddress())
            {
                constants.mac_address = string.Empty;
                users = new localDateBaseEntities3();
                try
                {
                    users.Database.ExecuteSqlCommand("TRUNCATE TABLE userLocal");
                    comboBox1.Items.Clear();
                }
                catch(Exception err)
                {
                    MessageBox.Show("[Error]: <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    constants.errorLog(err.ToString());
                }
                finally
                {
                    users.Dispose();
                }
            }
        }       

        //check box mostrar caracteres
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox3.Checked == true)
            {
                textBox2.PasswordChar = '\0';
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.PasswordChar = '*';
                textBox2.UseSystemPasswordChar = true;
            }
        }
        //

        //Error log
        private void errorLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Application.StartupPath + "\\error_log.txt") == true)
            {
                System.Diagnostics.Process.Start(Application.StartupPath + "\\error_log.txt");
            }
            else
            {
                MessageBox.Show(this, "[Error] el archivo error_log.txt no se encuentra en la carpeta de instalación o se está dañado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        ~Form2()
        {

        }

        //Informacion
        private void button2_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["info"] == null)
            {
                new info().ShowDialog();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.CharacterCasing = CharacterCasing.Upper;
        }

        //Ingreso ac
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            setIngresoAC(checkBox4.Checked);
        }

        private void setIngresoAC(bool ingreso)
        {
            try
            {
                XDocument propiedades_xml = XDocument.Load(constants.propiedades_xml);

                var propiedades = from x in propiedades_xml.Descendants("Propiedades") select x;

                foreach (XElement x in propiedades)
                {
                    x.SetElementValue("INGRESO_AC", ingreso == true ? "true" : "false");
                }
                propiedades_xml.Save(constants.propiedades_xml);
                constants.ingreso_ac = ingreso;
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show(this, "[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void códigoDeVigenciaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new vigencia().ShowDialog(this);
        }
    }
}
