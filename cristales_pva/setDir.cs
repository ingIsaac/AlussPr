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
    public partial class setDir : Form
    {
        int id;
        int perso_id;
        bool remove;
        public bool close = false;

        public setDir(int id, int perso_id, Image img, float largo, float alto, bool remove = false)
        {
            InitializeComponent();
            this.id = id;
            this.perso_id = perso_id;
            this.remove = remove;
            pictureBox1.Image = img;
            textBox1.Text = largo.ToString();
            textBox2.Text = alto.ToString();
            label3.Text = "*Nota: Usar siempre de referencia la apartura de mayor dimension para relacionar niveles.\n\n*Nota: Para asignar de forma manual la medida total, seleccione la configuración de manera indefinida.";
        }

        //Columna
        private void button1_Click(object sender, EventArgs e)
        {
            ((Form1)Application.OpenForms["form1"]).setArticuloPersonalizacion(id, perso_id, 1, remove);
            close = true;
            Close();
        }

        //Fila
        private void button2_Click(object sender, EventArgs e)
        {
            ((Form1)Application.OpenForms["form1"]).setArticuloPersonalizacion(id, perso_id, 2, remove);
            close = true;
            Close();
        }

        //Indefinido
        private void button3_Click(object sender, EventArgs e)
        {
            ((Form1)Application.OpenForms["form1"]).setArticuloPersonalizacion(id, perso_id, 0, remove);
            close = true;
            Close();
        }
    }
}
