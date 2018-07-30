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
            loadVigencia();
        }

        private void loadVigencia()
        {
            DateTime date = new sqlDateBaseManager().getvigenciaTienda(constants.org_name);
            textBox1.Text = date.ToShortDateString();
            if(constants.getVigencia(date))
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
    }
}
