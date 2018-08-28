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
    public partial class monitor : Form
    {
        public monitor()
        {
            InitializeComponent();
        }

        public void setData(object data, string store, string periodo, bool carrussel=false)
        {
            try
            {
                label1.Text = store;
                label2.Text = periodo;
                if (carrussel)
                {
                    pictureBox1.Visible = true;
                }
                else
                {
                    pictureBox1.Visible = false;
                }
                if (data != null)
                {
                    datagridviewNE1.DataSource = data;
                }
                datagridviewNE1.Refresh();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                MessageBox.Show(this, "[Error] no se ha podido cargar los datos.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void setcolors()
        {
            DateTime time;
            string etapa;
            foreach (DataGridViewRow x in datagridviewNE1.Rows)
            {
                time = DateTime.Parse(x.Cells[6].Value.ToString());
                etapa = x.Cells[4].Value.ToString();
                //Fechas
                if ((time.AddDays(-10) > DateTime.Today) && etapa != "Proyecto Terminado")
                {
                    x.Cells[6].Style.BackColor = Color.LightGreen;
                }
                else if ((time > DateTime.Today) && etapa != "Proyecto Terminado")
                {
                    x.Cells[6].Style.BackColor = Color.Yellow;
                }
                else if ((time <= DateTime.Today) && etapa != "Proyecto Terminado")
                {
                    x.Cells[6].Style.BackColor = Color.Red;
                }

                //Etapas
                if (etapa == "Cotización Aceptada")
                {
                    x.Cells[4].Style.BackColor = Color.LightBlue;
                }
                else if (etapa == "Requisición de Material")
                {
                    x.Cells[4].Style.BackColor = Color.LightYellow;
                }
                else if (etapa == "Fabricación")
                {
                    x.Cells[4].Style.BackColor = Color.Orange;
                }
                else if (etapa == "Instalación")
                {
                    x.Cells[4].Style.BackColor = Color.LightPink;
                }
                else if (etapa == "Proyecto Terminado")
                {
                    x.Cells[4].Style.BackColor = Color.LightGreen;
                    x.DefaultCellStyle.BackColor = Color.Gray;
                }
            }
        }
    }

}
