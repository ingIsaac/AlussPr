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
    public partial class colores : Form
    {
        bool no_modulo = false;
        bool wizard = false;
        bool pa = true;
        int index = -1;
        string type = string.Empty;

        public colores(bool pa=true, int index=-1, bool no_modulo=false, bool wizard=false)
        {
            InitializeComponent();
            datagridviewNE1.CellClick += DatagridviewNE1_CellClick;
            datagridviewNE1.CellLeave += DatagridviewNE1_CellLeave;
            sqlDateBaseManager sql = new sqlDateBaseManager();
            if (pa == true)
            {
                DataTable table = sql.createDataTableFromSQLTable("colores_aluminio");

                foreach(DataColumn x in table.Columns)
                {
                    datagridviewNE1.Columns.Add(x.ColumnName, x.ColumnName);
                }

                DataGridViewImageColumn clm_1 = new DataGridViewImageColumn();
                clm_1.Name = "muestra";
                clm_1.HeaderText = "muestra";
                datagridviewNE1.Columns.Add(clm_1);
                datagridviewNE1.Columns["muestra"].DisplayIndex = 2;

                foreach(DataRow x in table.Rows)
                {
                    datagridviewNE1.Rows.Add(x.ItemArray[0],x.ItemArray[1],x.ItemArray[2],x.ItemArray[3],x.ItemArray[4],x.ItemArray[5],x.ItemArray[6], constants.getImageFromFile("acabados_especiales", x.ItemArray[1].ToString(), "jpg"));
                }

                type = "pa";
            }
            else
            {
                datagridviewNE1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                datagridviewNE1.Columns.Add("id", "id");
                datagridviewNE1.Columns.Add("acabado", "acabado");
                DataGridViewImageColumn clm_1 = new DataGridViewImageColumn();
                clm_1.Name = "muestra";
                clm_1.HeaderText = "muestra";
                clm_1.ImageLayout = DataGridViewImageCellLayout.Stretch;
                datagridviewNE1.Columns.Add(clm_1);

                datagridviewNE1.Rows.Add("1", "crudo", constants.getImageFromFile("acabados_perfil", "crudo", "jpg"));
                datagridviewNE1.Rows.Add("2", "blanco", constants.getImageFromFile("acabados_perfil", "blanco", "jpg"));
                datagridviewNE1.Rows.Add("3", "hueso", constants.getImageFromFile("acabados_perfil", "blanco", "jpg"));
                datagridviewNE1.Rows.Add("4", "champagne", constants.getImageFromFile("acabados_perfil", "champagne", "jpg"));
                datagridviewNE1.Rows.Add("5", "gris", constants.getImageFromFile("acabados_perfil", "gris", "jpg"));
                datagridviewNE1.Rows.Add("6", "negro", constants.getImageFromFile("acabados_perfil", "negro", "jpg"));
                datagridviewNE1.Rows.Add("7", "brillante", constants.getImageFromFile("acabados_perfil", "brillante", "jpg"));
                datagridviewNE1.Rows.Add("8", "natural", constants.getImageFromFile("acabados_perfil", "natural", "jpg"));
                datagridviewNE1.Rows.Add("9", "madera", constants.getImageFromFile("acabados_perfil", "madera", "jpg"));
                datagridviewNE1.Rows.Add("10", "chocolate", constants.getImageFromFile("acabados_perfil", "chocolate", "jpg"));
                datagridviewNE1.Rows.Add("11", "acero_inox", constants.getImageFromFile("acabados_perfil", "acero_inox", "jpg"));
                datagridviewNE1.Rows.Add("12", "bronce", constants.getImageFromFile("acabados_perfil", "bronce", "jpg"));
                type = "lista";
            }
            this.no_modulo = no_modulo;
            this.pa = pa;
            this.index = index;
            this.wizard = wizard;
        }

        private void DatagridviewNE1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                datagridviewNE1.CurrentRow.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            }
        }

        private void DatagridviewNE1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                datagridviewNE1.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
            }
        }

        private void seleccionarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(datagridviewNE1.RowCount > 0)
            {
                if (no_modulo == false)
                {
                    if (index == -1)
                    {
                        ((config_modulo)Application.OpenForms["config_modulo"]).getColorAluminio(datagridviewNE1.CurrentRow.Cells[1].Value.ToString());
                    }
                    else
                    {
                        ((config_modulo)Application.OpenForms["config_modulo"]).getColorAluminoManual(datagridviewNE1.CurrentRow.Cells[1].Value.ToString(), index, type);
                    }
                }
                else
                {
                    if(wizard == false)
                    {
                        ((Form1)Application.OpenForms["form1"]).setAcabadoColor(datagridviewNE1.CurrentRow.Cells[1].Value.ToString());
                    }
                    else
                    {
                        ((change_colors)Application.OpenForms["change_colors"]).setColorAnodizado(datagridviewNE1.CurrentRow.Cells[1].Value.ToString());
                    }
                }
                this.Close();
            }
        }
    }
}
