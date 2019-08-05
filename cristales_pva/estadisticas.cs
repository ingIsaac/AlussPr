using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace cristales_pva
{
    public partial class estadisticas : Form
    {
        public estadisticas()
        {
            InitializeComponent();
            constants.setTiendas(comboBox3);
            setYears();
            comboBox2.Text = DateTime.Today.Year.ToString();
            comboBox3.Text = constants.org_name;
            comboBox1.SelectedIndex = 0;
            Title title = chart1.Titles.Add("Presupuestos");
            title.Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);
            title.ForeColor = Color.Red;
            loadInfo();                    
        }

        private void loadInfo()
        {
            List<cotizacion_info> info = new List<cotizacion_info>();
            List<p_registros> info_2 = new List<p_registros>();
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (sender, e) =>
            {
                //Load Data
                sqlDateBaseManager sql = new sqlDateBaseManager();
                info = sql.getCountPresupuestos(comboBox3.Text);
                info_2 = sql.getCountRegistros(comboBox3.Text);
            };
            bg.RunWorkerCompleted += (sender, e) =>
            {
                try
                {
                    string[] meses = new string[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
                    int año = constants.stringToInt(comboBox2.Text);
                    int mes = getMesInt(comboBox1.Text);
                    int _c = 0, total = 0;
                    chart1.Series.Clear();
                    //Series
                    //Serie 1
                    if (checkBox1.Checked)
                    {
                        Series serie = chart1.Series.Add("Conteo de presupuestos");
                        serie.ChartType = SeriesChartType.Line;
                        if (info.Count > 0)
                        {
                            if (mes > 0)
                            {
                                for (int i = 1; i < DateTime.DaysInMonth(año, mes); i++)
                                {
                                    _c = info.Where(v => v.dia == i && v.mes == mes && v.año == año).Count();
                                    total = total + _c;
                                    serie.Points.AddXY(i, _c);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < meses.Length; i++)
                                {
                                    _c = info.Where(v => v.mes == (i + 1) && v.año == año).Count();
                                    total = total + _c;
                                    serie.Points.AddXY(meses[i], _c);
                                }
                            }
                        }
                        serie.LegendText = serie.Name + "\n\n-Total: " + total + "\n-Año: " + año + "\n-Mes: " + getMesName(mes.ToString()) + "\n\n";
                    }
                    //Serie 2
                    if (checkBox2.Checked)
                    {
                        _c = 0;
                        total = 0;
                        Series serie2 = chart1.Series.Add("Conteo de registros");
                        serie2.ChartType = SeriesChartType.Line;
                        serie2.Color = Color.Red;
                        if (info_2.Count > 0)
                        {
                            if (mes > 0)
                            {
                                for (int i = 1; i < DateTime.DaysInMonth(año, mes); i++)
                                {
                                    _c = info_2.Where(v => v.dia == i && v.mes == mes && v.año == año).Count();
                                    total = total + _c;
                                    serie2.Points.AddXY(i, _c);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < meses.Length; i++)
                                {
                                    _c = info_2.Where(v => v.mes == (i + 1) && v.año == año).Count();
                                    total = total + _c;
                                    serie2.Points.AddXY(meses[i], _c);
                                }
                            }
                        }
                        serie2.LegendText = serie2.Name + "\n\n-Total: " + total + "\n-Año: " + año + "\n-Mes: " + getMesName(mes.ToString());
                    }
                    if (chart1.Titles.Count > 0)
                    {
                        chart1.Titles[0].Text = "Presupuestos\n\n" + comboBox3.Text;
                    }
                    info.Clear();
                    info_2.Clear();
                }
                catch (Exception)
                {
                    //Do nothing
                }
            };
            if (!bg.IsBusy)
            {
                bg.RunWorkerAsync();
            }
        }

        private void setYears()
        {
            for (int i = 2017; i <= DateTime.Today.Year; i++)
            {
                comboBox2.Items.Add(i);
            }
        }

        private int getMesInt(string mes)
        {
            switch (mes)
            {
                case "Enero":
                    return 1;
                case "Febrero":
                    return 2;
                case "Marzo":
                    return 3;
                case "Abril":
                    return 4;
                case "Mayo":
                    return 5;
                case "Junio":
                    return 6;
                case "Julio":
                    return 7;
                case "Agosto":
                    return 8;
                case "Septiembre":
                    return 9;
                case "Octubre":
                    return 10;
                case "Noviembre":
                    return 11;
                case "Diciembre":
                    return 12;
                default:
                    return 0;
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

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadInfo();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadInfo();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (PrintDocument doc = new PrintDocument())
                {
                    doc.PrintPage += (s, p) =>
                    {
                        Bitmap MyChartPanel = new Bitmap(chart1.Width, chart1.Height);
                        chart1.DrawToBitmap(MyChartPanel, new Rectangle(0, 0, chart1.Width, chart1.Height));
                        p.Graphics.DrawImage(new Bitmap(MyChartPanel, p.MarginBounds.Width, p.MarginBounds.Height / 2), new Rectangle(0, 0, p.MarginBounds.Width, p.MarginBounds.Height / 2));
                    };

                    PrintDialog dialog = new PrintDialog();
                    dialog.Document = doc;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        doc.Print();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this, "[Error] no se pudo imprimir el documento, intenta de nuevo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            loadInfo();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            loadInfo();
        }
    }
}
