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
    public partial class a_presupuestos : Form
    {
        public a_presupuestos()
        {
            InitializeComponent();
            loadPresupuestos();
            button2.Focus();
            label2.Text = label2.Text + " - " + getMesName(DateTime.Today.Month.ToString());
            this.KeyDown += A_presupuestos_KeyDown;
        }

        private void A_presupuestos_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void loadPresupuestos()
        {
            string empresa = constants.org_name;
            if (empresa == string.Empty)
            {
                this.Close();
            }
            label3.Text = empresa;
            List<cotizacion_info> info = new List<cotizacion_info>();
            List<p_registros> info_2 = new List<p_registros>();
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (sender, e) =>
            {
                //Load Data
                sqlDateBaseManager sql = new sqlDateBaseManager();
                info = sql.getCountPresupuestos(empresa);
                info_2 = sql.getCountRegistros(empresa);
            };
            bg.RunWorkerCompleted += (sender, e) =>
            {
                try
                {
                    int presupuestos = 0;
                    int registros = 0;
                    int mes = DateTime.Today.Month;
                    if (info.Count > 0)
                    {
                        presupuestos = info.Where(v => v.mes == mes && v.año == DateTime.Today.Year).Count();
                    }
                    if (info_2.Count > 0)
                    {
                        registros = info_2.Where(v => v.mes == mes && v.año == DateTime.Today.Year).Count();
                    }
                    label1.Text = "En lo que va del mes de " + getMesName(mes.ToString()) + " se han guardado (" + presupuestos + ") presupuestos de los cuales se han registrado (" + registros + ").";
                }
                catch (Exception)
                {
                    //Do nothing
                }
            };
            if (!bg.IsBusy)
            {
                label1.Text = "Cargando...";
                bg.RunWorkerAsync();
            }        
        }

        private string getMesName(string mes)
        {
            switch (mes)
            {
                case "1":
                    return "Enero";
                case "2":
                    return "Febrero";
                case "3":
                    return "Marzo";
                case "4":
                    return "Abril";
                case "5":
                    return "Mayo";
                case "6":
                    return "Junio";
                case "7":
                    return "Julio";
                case "8":
                    return "Agosto";
                case "9":
                    return "Septiembre";
                case "10":
                    return "Octubre";
                case "11":
                    return "Noviembre";
                case "12":
                    return "Diciembre";
                default:
                    return "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            new estadisticas().ShowDialog();
        }
    }
}
