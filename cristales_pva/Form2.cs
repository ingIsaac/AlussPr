using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
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
            users = new localDateBaseEntities3();
            try
            {
                loadServerData();
                constants.getSoftwareVersion();
                constants.setServerCredentials();
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
                MessageBox.Show("[Error]: <:SQLLocalDB?> Se necesitan instalar los complementos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (checkBox4.Checked)
            {
                if (MessageBox.Show("¿Deseas ingresar de manera automática?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (constants.user_ac != "" && constants.password_ac != "")
                    {
                        comboBox1.Text = constants.user_ac;
                        textBox2.Text = constants.password_ac;
                        ingresar();
                    }
                    else
                    {
                        MessageBox.Show("[Error] no se tiene configurado un usuario para ingreso automático.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("[Error] Acceso no autorizado. El usuario no está registrado en este equipo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else {
                sqlDateBaseManager sql = new sqlDateBaseManager();

                if (sql.setServerConnection() == true)
                {
                    if (sql.isUserAllowed(constants.user, constants.password) == true)
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
                                if (constants.getVigencia(sql.getvigenciaTienda(constants.org_name)))
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
                                            MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                    MessageBox.Show("El periodo de la licencia a expirado, póngase en contacto con el proveedor del sistema.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    constants.connected = false;
                                }
                            }
                            else
                            {
                                MessageBox.Show("[Error] no existe registro de esta tienda.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                constants.connected = false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("[Error] este equipo no se encuentra activado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            constants.connected = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("[Error] Acceso no autorizado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("[Error]: <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void loadOpcionesData()
        {
            try
            {
                XDocument opciones_xml = XDocument.Load(constants.opciones_xml);

                var mv = (from x in opciones_xml.Descendants("Opciones") select x.Element("MV")).SingleOrDefault();
                var opi = (from x in opciones_xml.Descendants("Opciones") select x.Element("OPI")).SingleOrDefault();
                var ma = (from x in opciones_xml.Descendants("Opciones") select x.Element("MA")).SingleOrDefault();
                var acc = (from x in opciones_xml.Descendants("Opciones") select x.Element("ACC")).SingleOrDefault();
                var ecs = (from x in opciones_xml.Descendants("Opciones") select x.Element("ECS")).SingleOrDefault();
                var erl = (from x in opciones_xml.Descendants("Opciones") select x.Element("ERL")).SingleOrDefault();
                var ivd = (from x in opciones_xml.Descendants("Opciones") select x.Element("IVD")).SingleOrDefault();
                var pai = (from x in opciones_xml.Descendants("Opciones") select x.Element("PAI")).SingleOrDefault();
                var spac = (from x in opciones_xml.Descendants("Opciones") select x.Element("SPAC")).SingleOrDefault();
                var m_liva = (from x in opciones_xml.Descendants("Opciones") select x.Element("MLIVA")).SingleOrDefault();
                var pac = (from x in opciones_xml.Descendants("Opciones") select x.Element("PAC")).SingleOrDefault();
                var lim_sm = (from x in opciones_xml.Descendants("Opciones") select x.Element("LIM_SM")).SingleOrDefault();

                var op1 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP1")).SingleOrDefault();
                var op2 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP2")).SingleOrDefault();
                var op3 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP3")).SingleOrDefault();
                var op4 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP4")).SingleOrDefault();
                var op5 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP5")).SingleOrDefault();
                var op6 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP6")).SingleOrDefault();
                var op7 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP7")).SingleOrDefault();
                var op8 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP8")).SingleOrDefault();
                var op9 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP9")).SingleOrDefault();
                var op10 = (from x in opciones_xml.Descendants("Opciones") select x.Element("OP10")).SingleOrDefault();

                if (mv != null)
                {
                    if(mv.Value == "true")
                    {
                        constants.maximizar_ventanas = true;
                    }
                    else
                    {
                        constants.maximizar_ventanas = false;
                    }
                }

                if (opi != null)
                {
                    if (opi.Value == "true")
                    {
                        constants.optimizar_inicio = true;
                    }
                    else
                    {
                        constants.optimizar_inicio = false;
                    }
                }

                if (ma != null)
                {
                    if (ma.Value == "true")
                    {
                        constants.mostrar_acabado = true;
                    }
                    else
                    {
                        constants.mostrar_acabado = false;
                    }
                }

                if (acc != null)
                {
                    if (acc.Value == "true")
                    {
                        constants.ac_cotizacion = true;
                    }
                    else
                    {
                        constants.ac_cotizacion = false;
                    }
                }

                if (ecs != null)
                {
                    if (ecs.Value == "true")
                    {
                        constants.enable_cs = true;
                    }
                    else
                    {
                        constants.enable_cs = false;
                    }
                }

                if (erl != null)
                {
                    if (erl.Value == "true")
                    {
                        constants.enable_rules = true;
                    }
                    else
                    {
                        constants.enable_rules = false;
                    }
                }

                if (ivd != null)
                {
                    if (ivd.Value == "true")
                    {
                        constants.iva_desglosado = true;
                    }
                    else
                    {
                        constants.iva_desglosado = false;
                    }
                }

                if (pai != null)
                {
                    if (pai.Value == "true")
                    {
                        constants.permitir_ajuste_iva = true;
                    }
                    else
                    {
                        constants.permitir_ajuste_iva = false;
                    }
                }              

                if (spac != null)
                {
                    if (spac.Value == "true")
                    {
                        constants.siempre_permitir_ac = true;
                    }
                    else
                    {
                        constants.siempre_permitir_ac = false;
                    }
                }

                if (m_liva != null)
                {
                    if (m_liva.Value == "true")
                    {
                        constants.m_liva = true;
                    }
                    else
                    {
                        constants.m_liva = false;
                    }
                }

                if (pac != null)
                {
                    if (pac.Value == "true")
                    {
                        constants.p_ac = true;
                    }
                    else
                    {
                        constants.p_ac = false;
                    }
                }

                if (lim_sm != null)
                {
                    constants.lim_sm = constants.stringToFloat(lim_sm.Value.ToString());
                }

                if (op1 != null)
                {
                    if (op1.Value == "true")
                    {
                        constants.op1 = true;
                    }
                    else
                    {
                        constants.op1 = false;
                    }
                }

                if (op2 != null)
                {
                    if (op2.Value == "true")
                    {
                        constants.op2 = true;
                    }
                    else
                    {
                        constants.op2 = false;
                    }
                }

                if (op3 != null)
                {
                    if (op3.Value == "true")
                    {
                        constants.op3 = true;
                    }
                    else
                    {
                        constants.op3 = false;
                    }
                }

                if (op4 != null)
                {
                    if (op4.Value == "true")
                    {
                        constants.op4 = true;
                    }
                    else
                    {
                        constants.op4 = false;
                    }
                }

                if (op5 != null)
                {
                    if (op5.Value == "true")
                    {
                        constants.op5 = true;
                    }
                    else
                    {
                        constants.op5 = false;
                    }
                }

                if (op6 != null)
                {
                    if (op6.Value == "true")
                    {
                        constants.op6 = true;
                    }
                    else
                    {
                        constants.op6 = false;
                    }
                }

                if (op7 != null)
                {
                    if (op7.Value == "true")
                    {
                        constants.op7 = true;
                    }
                    else
                    {
                        constants.op7 = false;
                    }
                }

                if (op8 != null)
                {
                    if (op8.Value == "true")
                    {
                        constants.op8 = true;
                    }
                    else
                    {
                        constants.op8 = false;
                    }
                }

                if (op9 != null)
                {
                    if (op9.Value == "true")
                    {
                        constants.op9 = true;
                    }
                    else
                    {
                        constants.op9 = false;
                    }
                }

                if (op10 != null)
                {
                    if (op10.Value == "true")
                    {
                        constants.op10 = true;
                    }
                    else
                    {
                        constants.op10 = false;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] el archivo opciones.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
        }

        private void loadServerData()
        {
            try
            {
                loadOpcionesData();
                XDocument propiedades_xml = XDocument.Load(constants.propiedades_xml);

                var server = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("SERVER")).SingleOrDefault();
                var db = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("DB")).SingleOrDefault();
                var org = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("ORG")).SingleOrDefault();
                var org_name = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("ORG_N")).SingleOrDefault();
                var mbd = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("MBD")).SingleOrDefault();
                var updater_enable = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("UDE")).SingleOrDefault();
                var updater_form_close = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("UC")).SingleOrDefault();
                var ingreso_ac = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("INGRESO_AC")).SingleOrDefault();
                var user_ac = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("USER_AC")).SingleOrDefault();
                var password_ac = (from x in propiedades_xml.Descendants("Propiedades") select x.Element("PASSWORD_AC")).SingleOrDefault();

                if (server != null)
                {
                    constants.server = server.Value;
                }

                if (db != null)
                {
                    constants.data_base = db.Value;
                }

                if (org != null)
                {
                    constants.header_reporte = org.Value;
                }

                if (org_name != null)
                {
                    constants.org_name = org_name.Value;
                }

                if (mbd != null)
                {
                    constants.msg_box_caption = mbd.Value;
                }

                if (updater_enable != null)
                {
                    if(updater_enable.Value == "true")
                    {
                        constants.updater_enable = true;
                    }
                    else
                    {
                        constants.updater_enable = false;
                    }
                }

                if (updater_form_close != null)
                {
                    if (updater_form_close.Value == "true")
                    {
                        constants.updater_form_close = true;
                    }
                    else
                    {
                        constants.updater_form_close = false;
                    }
                }            

                if(ingreso_ac != null)
                {
                    constants.ingreso_ac = ingreso_ac.Value == "true" ? true : false;
                }

                if (user_ac != null)
                {
                    constants.user_ac = user_ac.Value.ToUpper();
                }

                if (password_ac != null)
                {
                    constants.password_ac = password_ac.Value;
                }

                //load porcentajes de articulos
                constants.loadPercentageOfArticulos();
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
                Environment.Exit(0);
            }
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
                MessageBox.Show("[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("[Error] el archivo error_log.txt no se encuentra en la carpeta de instalación o se está dañado.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("[Error] el archivo propiedades.xml no se encuentra en la carpeta de instalación o se está dañado." + Application.StartupPath, constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void códigoDeVigenciaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new vigencia().ShowDialog(this);
        }
    }
}
