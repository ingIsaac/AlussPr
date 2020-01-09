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
    public partial class d_produccion : Form
    {
        public d_produccion(List<string[]> table, int id, string s_d_produccion="", bool reporte=false)
        {
            InitializeComponent();
            loadPerfiles(table, id, s_d_produccion);
        }

        private void loadPerfiles(List<string[]> table, int id, string perfiles="")
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            List<string[]> k = new List<string[]>();

            BackgroundWorker bg = new BackgroundWorker();

            bg.DoWork += (sender, e) =>
            {
                if (perfiles == string.Empty)
                {
                    perfiles = sql.getSingleSQLValueById("modulos", "p_claves", "id", id, 0);
                }

                if (perfiles.Length > 0)
                {
                    string[] t = perfiles.Split(',');
                    foreach (string v in t)
                    {
                        string[] u = v.Split(';');
                        if (u.Length == 4)
                        {
                            k.Add(u);
                        }
                    }
                }
            };
            bg.RunWorkerCompleted += (sernder, e) =>
            {
                int c = 0;

                foreach (string[] x in table)
                {
                    if (k.Count > 0 && c <= k.Count - 1)
                    {
                        string[] y = k[c];
                        if (y[0] != x[0])
                        {
                            datagridviewNE1.Rows.Add(x[0], x[1], x[2], x[3], 0, 0, 0);
                        }
                        else
                        {
                            datagridviewNE1.Rows.Add(x[0], x[1], x[2], x[3], constants.stringToFloat(y[1]), constants.stringToFloat(y[2]), constants.stringToFloat(y[3]));
                        }
                    }
                    else
                    {
                        datagridviewNE1.Rows.Add(x[0], x[1], x[2], x[3], 0, 0, 0);
                    }
                    c++;
                }
                //--------------->
                label1.Visible = false;
            };
            if(!bg.IsBusy)
            {
                label1.Visible = true;
                bg.RunWorkerAsync();
            }               
        }

        private string getDiseñoProduccion()
        {
            string r = string.Empty;
            if (datagridviewNE1.RowCount > 0)
            {
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    if (r.Length == 0)
                    {
                        r = x.Cells[0].Value.ToString() + ";" + x.Cells[4].Value.ToString() + ";" + x.Cells[5].Value.ToString() + ";" + x.Cells[6].Value.ToString();
                    }
                    else
                    {
                        r = r + "," + x.Cells[0].Value.ToString() + ";" + x.Cells[4].Value.ToString() + ";" + x.Cells[5].Value.ToString() + ";" + x.Cells[6].Value.ToString();
                    }
                }
            }
            return r;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                ((crear_modulo)Application.OpenForms["crear_modulo"]).d_produccion = datagridviewNE1;
                ((crear_modulo)Application.OpenForms["crear_modulo"]).s_d_produccion = getDiseñoProduccion();
                MessageBox.Show(this, "Se ha guardado el diseño de producción.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }
    }
}
