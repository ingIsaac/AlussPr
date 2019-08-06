using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Reporting.WinForms;

namespace cristales_pva
{
    public partial class historial_registros : Form
    {
        sqlDateBaseManager sql = new sqlDateBaseManager();
        System.Timers.Timer timer = new System.Timers.Timer(constants.monitor_interval * 60 * 1000);
        int recorrido = 0;

        public historial_registros()
        {
            InitializeComponent();
            this.FormClosing += Historial_registros_FormClosing;
            textBox1.KeyPress += TextBox1_KeyPress;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
            backgroundWorker2.RunWorkerCompleted += BackgroundWorker2_RunWorkerCompleted;
            datagridviewNE1.Click += DatagridviewNE1_Click;
            reportViewer1.LocalReport.EnableExternalImages = true;
            reportViewer1.ZoomMode = ZoomMode.PageWidth;
            reportViewer1.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;            
            Load += Historial_registros_Load;
            datagridviewNE1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.Shown += Historial_registros_Shown;
            setYears();   
        }

        private void Historial_registros_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Application.OpenForms["monitor"] != null)
            {
                Application.OpenForms["monitor"].Close();
            }
        }

        private void DatagridviewNE1_Click(object sender, EventArgs e)
        {
            if(datagridviewNE1.Rows.Count > 0)
            {
                string[] param = new string[] { datagridviewNE1.CurrentRow.Cells[0].Value.ToString(), datagridviewNE1.CurrentRow.Cells[1].Value.ToString(), datagridviewNE1.CurrentRow.Cells[2].Value.ToString(), datagridviewNE1.CurrentRow.Cells[3].Value.ToString(), datagridviewNE1.CurrentRow.Cells[4].Value.ToString(), datagridviewNE1.CurrentRow.Cells[5].Value.ToString(), datagridviewNE1.CurrentRow.Cells[6].Value.ToString() };
                new informe(param).ShowDialog(this);
            }
        }

        private void Historial_registros_Shown(object sender, EventArgs e)
        {
            constants.setTiendas(comboBox4);
            comboBox4.Text = constants.org_name;
            setTimer();
            loadPresupuestos(string.Empty, false);
            comboBox2.Text = getMesName(DateTime.Now.Month.ToString());
            comboBox3.Text = DateTime.Now.Year.ToString();
        }

        private void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            reportViewer1.LocalReport.ReleaseSandboxAppDomain();
            reportViewer1.LocalReport.Dispose();
        }

        private void setYears()
        {
            for (int i = 2017; i <= DateTime.Today.Year; i++)
            {
                comboBox3.Items.Add(i);
            }
        }

        private void setTimer()
        {
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
        }

        private void stopTimer()
        {
            timer.Stop();
        }

        private void startTimer()
        {
            timer.Start();
        }

        private void Historial_registros_Load(object sender, EventArgs e)
        {
            resetReport(new cotizaciones_local());
            this.datos_reporteTableAdapter.Fill(this.reportes_dataSet.datos_reporte);
            reportViewer1.LocalReport.SetParameters(new ReportParameter("Image", constants.getExternalImage("header")));
            ReportPageSettings ps = reportViewer1.LocalReport.GetDefaultPageSettings();
            this.reportViewer1.ParentForm.Width = ps.PaperSize.Width;
            this.reportViewer1.RefreshReport();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (checkBox3.Checked)
            {
                if((recorrido + 1) < comboBox4.Items.Count)
                {
                    recorrido++;
                }
                else
                {
                    recorrido = 0;
                }
                loadPresupuestos(textBox1.Text, checkBox2.Checked, comboBox4.Items[recorrido].ToString());
            }
            else
            {
                loadPresupuestos(textBox1.Text, checkBox2.Checked);
            }
        }

        private void BackgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;
            tabControl1.SelectedTab = tabPage2;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureBox1.Visible = false;           
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                loadPresupuestos(textBox1.Text);
            }
        }

        private void loadPresupuestos(string filter="", bool fecha=true, string tienda="")
        {
            if (backgroundWorker1.IsBusy == false && backgroundWorker2.IsBusy == false)
            {
                pictureBox1.Visible = true;
                string _fecha = string.Empty;
                if (fecha)
                {
                    if (checkBox1.Checked)
                    {
                        if(getMesName(DateTime.Now.Month.ToString()) != comboBox2.Text)
                        {
                            comboBox2.Text = getMesName(DateTime.Now.Month.ToString());
                        }
                        if(DateTime.Now.Year.ToString() != comboBox3.Text)
                        {
                            comboBox3.Text = DateTime.Now.Year.ToString();
                        }
                    }
                    _fecha = getMesInt(comboBox2.Text) + "/" + comboBox3.Text;
                }
                if(tienda == "")
                {
                    tienda = comboBox4.Text;
                }
                label6.Text = tienda;
                string[] s = new string[] {_fecha, filter, tienda};
                backgroundWorker1.RunWorkerAsync(s);              
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loadPresupuestos();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            loadPresupuestos(textBox1.Text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                loadPresupuestos(comboBox1.Text);
            }
            else
            {
                loadPresupuestos();
            }
        }

        private void setcolors()
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
                else if(etapa == "Proyecto Terminado")
                {
                    x.Cells[4].Style.BackColor = Color.LightGreen;
                    x.DefaultCellStyle.BackColor = Color.Gray;
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] s = e.Argument as string[];
            sql.dropPresupuestosOnGridView(datagridviewNE1, constants.stringToInt(textBox2.Text) < 10 ? 10 : constants.stringToInt(textBox2.Text), s[2], s[0], s[1]);
            setcolors();
            if(s[0] != "")
            {
                label7.Text = "Total del Periodo: (" + datagridviewNE1.Rows.Count + ") Presupuestos.";
            }
            else
            {
                label7.Text = string.Empty;
            }
            ///-------------------------------------------------------> MONITOR 
            Form monitor = Application.OpenForms["monitor"];
            if (monitor.InvokeRequired)
            {
                monitor.Invoke((MethodInvoker)delegate
                {
                    if (monitor != null)
                    {
                        ((monitor)monitor).setData(datagridviewNE1.DataSource, label6.Text, getPeriodo(), checkBox3.Checked);
                        ((monitor)monitor).setcolors();
                    }
                });              
            }
            else
            {
                if (monitor != null)
                {
                    ((monitor)monitor).setData(datagridviewNE1.DataSource, label6.Text, getPeriodo(), checkBox3.Checked);
                    ((monitor)monitor).setcolors();
                }
            }
        }

        private string getPeriodo()
        {
            if(checkBox2.Text != ""  && checkBox2.Checked)
            {
                return comboBox2.Text + " - " + comboBox3.Text;
            }
            else
            {
                return string.Empty;
            }
        }

        private void resetReport(cotizaciones_local cotizaciones)
        {
            cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE datos_reporte");
            cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (datos_reporte, RESEED, 1)");
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (reportViewer1.InvokeRequired == true)
            {
                reportViewer1.Invoke((MethodInvoker)delegate
                {
                    try
                    {
                        cotizaciones_local cotizaciones = new cotizaciones_local();
                        resetReport(cotizaciones);
                        string[] p = e.Argument as string[];
                        if (p.Length == 7)
                        {
                            string informe = sql.selectRegistroPresupuestos(constants.stringToInt(p[0]), "informe");
                            reportViewer1.LocalReport.SetParameters(new ReportParameter("informe", informe));
                            reportViewer1.LocalReport.SetParameters(new ReportParameter("responsable", p[3]));
                            reportViewer1.LocalReport.SetParameters(new ReportParameter("etapa", p[4]));
                            reportViewer1.LocalReport.SetParameters(new ReportParameter("fecha_inicio", p[5]));
                            reportViewer1.LocalReport.SetParameters(new ReportParameter("fecha_entrega", p[6]));

                            var reporte = new datos_reporte()
                            {
                                Cliente = p[1],
                                Nombre_Proyecto = p[2],
                                Fecha = DateTime.Today.ToString("dd/MM/yyyy"),
                                Folio = p[0],
                                Subtotal = 0,
                                IVA = 0,
                                Total = 0
                            };
                            cotizaciones.datos_reporte.Add(reporte);
                            cotizaciones.SaveChanges();
                        }

                        var datos = (from x in cotizaciones.datos_reporte select x);
                        datos_reporteBindingSource.DataSource = datos.ToList();
                        this.datos_reporteTableAdapter.Fill(this.reportes_dataSet.datos_reporte);
                        this.reportViewer1.LocalReport.Refresh();
                        this.reportViewer1.RefreshReport();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.ToString());
                    }
                });
            }         
        }

        private string getMesInt(string mes)
        {
            switch (mes)
            {
                case "Enero":
                    return "01";
                case "Febrero":
                    return "02";
                case "Marzo":
                    return "03";
                case "Abril":
                    return "04";
                case "Mayo":
                    return "05";
                case "Junio":
                    return "06";
                case "Julio":
                    return "07";
                case "Agosto":
                    return "08";
                case "Septiembre":
                    return "09";
                case "Octubre":
                    return "10";
                case "Noviembre":
                    return "11";
                case "Diciembre":
                    return "12";
                default:
                    return "";
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

        public void getInformeToPrint(string[] param)
        {
            if (backgroundWorker2.IsBusy == false && backgroundWorker1.IsBusy == false)
            {
                pictureBox1.Visible = true;
                backgroundWorker2.RunWorkerAsync(param);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            loadPresupuestos(string.Empty, false);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                startTimer();
            }
            else
            {
                stopTimer();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!constants.isInteger(textBox2.Text))
            {
                textBox2.Text = "";
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadPresupuestos(string.Empty, false);
            label6.Text = comboBox4.Text;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                recorrido = comboBox4.SelectedIndex;
                pictureBox2.Visible = true;
            }
            else
            {
                pictureBox2.Visible = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(Application.OpenForms["monitor"] == null)
            {
                monitor m = new monitor();
                m.setData(datagridviewNE1.DataSource, label6.Text, getPeriodo(), checkBox3.Checked);
                m.setcolors();
                m.Show();
            }
            else
            {
                Application.OpenForms["monitor"].Select();                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
        }
    }
}
