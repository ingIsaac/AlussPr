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
    public partial class new_costos : Form
    {
        List<string> lista;

        public new_costos(List<string> lista)
        {
            InitializeComponent();
            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;
            datagridviewNE1.EditingControlShowing += DatagridviewNE1_EditingControlShowing;
            this.lista = lista;
            loadLista();
        }

        private void DatagridviewNE1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (datagridviewNE1.CurrentCell.OwningColumn.Index == 0)
            {
                if (e.Control is TextBox)
                {
                    ((TextBox)(e.Control)).CharacterCasing = CharacterCasing.Upper;
                }
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if(datagridviewNE1.RowCount <= 0 || datagridviewNE1.CurrentRow.Index == (datagridviewNE1.Rows.Count - 1))
            {
                e.Cancel = true;
            }
        }

        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(datagridviewNE1.RowCount > 0)
            {
                datagridviewNE1.Rows.Remove(datagridviewNE1.CurrentRow);
            }
        }

        private void setNewCostos()
        {
            lista.Clear();
            datagridviewNE1.EndEdit();
            foreach(DataGridViewRow x in datagridviewNE1.Rows)
            {
                if (x.Cells[0].Value != null && x.Cells[1].Value != null && x.Cells[2].Value != null)
                {
                    if (x.Cells[0].Value.ToString() != string.Empty && x.Cells[1].Value.ToString() != string.Empty && x.Cells[2].Value.ToString() != string.Empty)
                    {
                        lista.Add("5," + x.Cells[0].Value.ToString() + "," + x.Cells[1].Value.ToString() + "," + x.Cells[2].Value.ToString());
                    }
                }              
            }
            if (Application.OpenForms["config_modulo"] != null)
            {
                ((config_modulo)Application.OpenForms["config_modulo"]).setNewCosto(lista);
            }
        }

        private void loadLista()
        {
            if (lista.Count > 0)
            {
                string[] p = null;
                foreach (string x in lista)
                {
                    p = x.Split(',');
                    if (p.Length == 4)
                    {
                        datagridviewNE1.Rows.Add(p[1].ToUpper(), p[2], p[3]);
                    }
                }
            }
        }

        //Borrar
        private void button1_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.Rows.Count > 0)
            {
                datagridviewNE1.Rows.Clear();
            }
        }

        //Guardar
        private void button2_Click(object sender, EventArgs e)
        {
            setNewCostos();
            Close();
        }
    }
}
