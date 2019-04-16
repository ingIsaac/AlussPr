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
    public partial class anuncios_manager : Form
    {
        int anuncio_id = 0;

        public anuncios_manager()
        {
            InitializeComponent();
        }

        private void anuncios_manager_Load(object sender, EventArgs e)
        {
            loadAnuncios();
        }

        private void loadAnuncios()
        {
            new sqlDateBaseManager().getAnunciosTable(datagridviewNE1);
        }

        private void modificarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(datagridviewNE1.RowCount > 0)
            {
                anuncio_id = (int)datagridviewNE1.CurrentRow.Cells[0].Value;
                richTextBox1.Text = datagridviewNE1.CurrentRow.Cells[1].Value.ToString();
                button2.Visible = true;            
            }
        }

        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(datagridviewNE1.RowCount > 0)
            {
                if (MessageBox.Show(this, "¿Desea eliminar el anuncio seleccionado?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    new sqlDateBaseManager().eliminarAnuncio((int)datagridviewNE1.CurrentRow.Cells[0].Value);
                    MessageBox.Show(this, "Se ha eliminado el anuncio exitosamente!", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loadAnuncios();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            anuncio_id = 0;
            richTextBox1.Clear();
            button2.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(anuncio_id > 0)
            {
                if (MessageBox.Show(this, "¿Desea modificar el anuncio seleccionado?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    new sqlDateBaseManager().modificarAnuncio(anuncio_id, richTextBox1.Text);
                    MessageBox.Show(this, "Se ha modificado el anuncio exitosamente!", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loadAnuncios();
                }
            }
            else
            {
                new sqlDateBaseManager().newAnuncio(richTextBox1.Text, DateTime.Today.ToShortDateString());
                MessageBox.Show(this, "Se ha creado el anuncio exitosamente!", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                loadAnuncios();
            }
        }
    }
}
