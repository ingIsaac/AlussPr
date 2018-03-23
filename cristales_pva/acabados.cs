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
    public partial class acabados : Form
    {
        int index = -1;
        public acabados(int index)
        {
            InitializeComponent();
            this.index = index;
        }

        private void acabados_Load(object sender, EventArgs e)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            sql.dropTableOnGridView(datagridviewNE1, "acabados");
        }

        private void seleccionarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                string clave = datagridviewNE1.CurrentRow.Cells[0].Value.ToString();
                ((Form1)Application.OpenForms["Form1"]).setAcabado(clave, index);
                this.Close();
            }
        }
    }
}
