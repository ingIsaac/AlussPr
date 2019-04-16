using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cristales_pva
{
    public partial class registro_presupuesto : Form
    {
        public registro_presupuesto()
        {
            InitializeComponent();
            dateTimePicker1.ValueChanged += DateTimePicker1_ValueChanged;
            dateTimePicker1.MinDate = DateTime.Today;
            this.Text = this.Text + " - " + constants.org_name;
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            label15.Text = dateTimePicker1.Value.ToString("dd/MM/yyyy");
        }

        private void registro_presupuesto_Load(object sender, EventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();

            label5.Text = constants.folio_abierto.ToString();
            label6.Text = constants.nombre_cotizacion;
            label7.Text = constants.nombre_proyecto;
            label8.Text = constants.autor_cotizacion != "" ? constants.autor_cotizacion : sql.getSingleSQLValue("cotizaciones", "usuario", "folio", constants.folio_abierto.ToString(), 0);
            label14.Text = DateTime.Now.ToString("dd/MM/yyyy");                      
            string etapa = sql.selectRegistroPresupuestos(constants.folio_abierto, "etapa");
            string informe = sql.selectRegistroPresupuestos(constants.folio_abierto, "informe");
            string fecha_entrega = sql.selectRegistroPresupuestos(constants.folio_abierto, "fecha_entrega");
            string fecha_inicio = sql.selectRegistroPresupuestos(constants.folio_abierto, "fecha_inicio");
            if(fecha_inicio != string.Empty)
            {
                label14.Text = fecha_inicio;
            }
            comboBox1.Text = etapa;
            if (comboBox1.Text == "")
            {
                textBox1.Text = etapa;
            }
            richTextBox1.Text = informe;
            label15.Text = fecha_entrega;          
        }

        //actualizar
        private void button1_Click(object sender, EventArgs e)
        {
            if (label15.Text != "")
            {
                sqlDateBaseManager sql = new sqlDateBaseManager();
                string line = "\n-----------------------------------------> " + DateTime.Now + "\n\n";
                if (sql.findSQLValue("folio", "folio", "registro_presupuestos", label5.Text) == false)
                {
                    sql.insertRegistroPresupuestos(comboBox1.Text == "" ? textBox1.Text : comboBox1.Text, richTextBox2.Text != "" ? richTextBox2.Text + line : richTextBox2.Text, label14.Text, label15.Text, constants.folio_abierto, constants.org_name);
                    richTextBox1.Text = richTextBox2.Text != "" ? richTextBox2.Text + line : richTextBox2.Text;
                    sql.updateCotizacionRegistro(constants.folio_abierto, "Registrada");
                    ((Form1)Application.OpenForms["form1"]).setVerificarRegistro();
                    MessageBox.Show("Se ha registrado esta cotización.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    sql.updateRegistroPresupuestos(comboBox1.Text == "" ? textBox1.Text : comboBox1.Text, richTextBox2.Text != "" ? richTextBox1.Text + richTextBox2.Text + line : richTextBox1.Text + richTextBox2.Text, label15.Text, constants.folio_abierto);
                    richTextBox1.Text = richTextBox2.Text != "" ? richTextBox1.Text + richTextBox2.Text + line : richTextBox1.Text + richTextBox2.Text;
                    MessageBox.Show("Se ha actualizado esta cotización.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("[Error] se necesita asignar una fecha de entrega estimada.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(comboBox1.Text != "" && textBox1.Text != "")
            {
                comboBox1.SelectedIndex = -1;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex >= 0)
            {
                textBox1.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("¿Desea eliminar esté informe?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {
                richTextBox1.Text = "";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show("¿Está seguro de eliminar esté registro?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (r == DialogResult.Yes)
            {
                sqlDateBaseManager sql = new sqlDateBaseManager();
                sql.updateCotizacionRegistro(constants.folio_abierto, "Sin Registro");
                sql.deleteRegistroPresupuestos(constants.folio_abierto, true);
                ((Form1)Application.OpenForms["form1"]).setVerificarRegistro();
                if (Application.OpenForms["historial_registros"] != null)
                {
                    Application.OpenForms["historial_registros"].Close();
                }
                this.Close();
            }
        }
    }
}
