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
    public partial class vigencia : Form
    {
        public vigencia()
        {
            InitializeComponent();
        }

        private void loadVigencia()
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            DateTime t = sql.getvigenciaTienda(constants.org_name, this);          
            DateTime date = Convert.ToDateTime(t);
            textBox1.Text = date.ToShortDateString();
            textBox2.Text = sql.getvigenciaType(constants.org_name).ToUpper();
            if (constants.getVigencia(date))
            {
                label2.Text = "Activa";
                label2.BackColor = Color.Green;
            }
            else
            {
                label2.Text = "Vencida";
                label2.ForeColor = Color.Red;
            }          
        }

        private void vigencia_Load(object sender, EventArgs e)
        {
            loadVigencia();
        }
    }
}
