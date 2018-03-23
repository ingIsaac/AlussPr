using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;

namespace cristales_pva
{
    public partial class mail : Form
    {
        List<string> files = new List<string>();
        List<string> contactos = new List<string>();
        AutoCompleteStringCollection source = new AutoCompleteStringCollection();

        int item_index = -1;
        bool enviado = false;

        public mail(string to)
        {
            InitializeComponent();
            comboBox1.Text = to;
            textBox1.Text = constants.email;
            textBox2.Text = constants.email_pw;
            listView1.ItemSelectionChanged += ListView1_ItemSelectionChanged;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            backgroundWorker2.RunWorkerCompleted += BackgroundWorker2_RunWorkerCompleted;
            comboBox1.AutoCompleteMode = AutoCompleteMode.Suggest;
            comboBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
            loadContactos();           
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                contextMenuStrip2.Show(MousePosition);
            }
        }

        private void loadContactos()
        {
            if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false)
            {
                comboBox1.Enabled = false;
                pictureBox1.Visible = true;
                label7.Text = "Contactos...";
                backgroundWorker2.RunWorkerAsync();
            }
        }

        private void ListView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected == true)
            {
                item_index = e.ItemIndex;
                contextMenuStrip1.Show(MousePosition);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "") 
            {
                if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false)
                {
                    if (textBox4.Text != "")
                    {
                        pictureBox1.Visible = true;
                        label7.Text = "Enviando...";
                        backgroundWorker1.RunWorkerAsync();
                    }
                    else
                    {
                        DialogResult r = MessageBox.Show("El correo no tiene asunto. ¿Deseas continuar?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if(r == DialogResult.Yes)
                        {
                            pictureBox1.Visible = true;
                            label7.Text = "Enviando...";
                            backgroundWorker1.RunWorkerAsync();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("[Error] Necesitas colocar la dirección del correo del destinatario.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //agregar adjuntos
        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Selecciona un archivo";
            openFileDialog1.FileName = "";
            string[] s = null;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                files.Add(openFileDialog1.FileName);
                listView1.Items.Clear();
                foreach(string x in files)
                {
                    s = x.Split('\\');
                    listView1.Items.Add(s[s.Length-1]);
                }
            }
        }

        //borrar adjuntos
        private void button3_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            files.Clear();
            item_index = -1;
        }

        //Quitar adjunto
        private void quitarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (item_index >= 0)
                {
                    listView1.Items.RemoveAt(item_index);
                    files.RemoveAt(item_index);
                    item_index = -1;
                    loadContactos();
                }
            }
            catch (Exception) { }       
        }

        //proceso para enviar...
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {       
            if (constants.email_send(textBox1.Text, textBox2.Text, comboBox1.Text, textBox4.Text, richTextBox1.Text, files) == true)
            {
                enviado = true;
            }
            else
            {
                enviado = false;
            }       
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
            if(enviado == true)
            {
                label7.Text = "";
                MessageBox.Show("El correo ha sido enviado!", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                label7.Text = "";
                MessageBox.Show("[Error] El correo no ha podido ser enviado, intenta de nuevo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void getContactos()
        {
            try
            {
                this.contactos.Clear();
                XDocument directorio_xml = XDocument.Load(constants.directorio_xml);

                var contactos = directorio_xml.Descendants("Directorio").Elements("Contacto");

                foreach (var x in contactos)
                {
                    this.contactos.Add(x.Value);
                }
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void setNewContact(string contacto)
        {
            try
            {
                if (isContactoExist(contacto) == false)
                {
                    XDocument directorio_xml = XDocument.Load(constants.directorio_xml);
                    directorio_xml.Descendants("Directorio").Last().Add(new XElement("Contacto", contacto));
                    directorio_xml.Save(constants.directorio_xml);
                }
                else
                {
                    MessageBox.Show("[Error] El contacto ya existe en el directorio.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private bool isContactoExist(string contacto)
        {
            bool r = false;
            try
            {
                XDocument directorio_xml = XDocument.Load(constants.directorio_xml);

                var contactos = directorio_xml.Descendants("Directorio").Elements("Contacto").Where(x => x.Value == contacto).FirstOrDefault();

                if (contactos != null)
                {
                    r = true;
                }
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            return r;
        }


        private void borrarContacto(string contacto)
        {
            try
            {
                XDocument directorio_xml = XDocument.Load(constants.directorio_xml);

                directorio_xml.Descendants("Directorio").Elements("Contacto").Where(x => x.Value == contacto).Remove();

                directorio_xml.Save(constants.directorio_xml);
                loadContactos();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        private void borrarTodos()
        {
            try
            {
                XDocument directorio_xml = XDocument.Load(constants.directorio_xml);

                directorio_xml.Descendants("Directorio").Elements("Contacto").Remove();
                
                directorio_xml.Save(constants.directorio_xml);
                loadContactos();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
        }

        //cargar contactos
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            getContactos();
        }

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            comboBox1.Enabled = true;
            pictureBox1.Visible = false;
            label7.Text = "";
            source.Clear();
            source.AddRange(contactos.ToArray());
            comboBox1.AutoCompleteCustomSource = source;
            listBox1.Items.Clear();
            foreach(string x in contactos)
            {
                listBox1.Items.Add(x);
            }
                   
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
            {
                setNewContact(textBox3.Text);
                textBox3.Text = "";
                loadContactos();
            }
            else
            {
                textBox3.Focus();
                MessageBox.Show("[Error] Necesitas añadir una dirección de correo electrónico al contacto.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void quitarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                borrarContacto(listBox1.SelectedItem.ToString());
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("Se borrarán todos los contactos en el directorio. ¿Deseas continuar?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {
                borrarTodos();
                loadContactos();
            }
        }
    }
}
