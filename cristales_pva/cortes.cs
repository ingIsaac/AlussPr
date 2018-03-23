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
    public partial class cortes : Form
    {
        cotizaciones_local cotizaciones = new cotizaciones_local();

        public cortes(string clave, string acabado)
        {
            InitializeComponent();
            loadCortes(clave, acabado);
        }

        private void loadCortes(string clave, string acabado)
        {
            var data = from x in cotizaciones.cortes 
                       where x.clave == clave && x.acabado == acabado
                       group x by x.partida
                       into g
                       select new
                       {
                           Id = g.FirstOrDefault().id,
                           Clave = g.FirstOrDefault().clave,
                           Artículo = g.FirstOrDefault().articulo,
                           Módulo_Id = g.FirstOrDefault().modulo_id,
                           Concepto = g.Key,
                           Acabado = g.FirstOrDefault().acabado,
                           Cantidad = g.Sum(s => s.cantidad),
                           Metros_Lineales = Math.Round(g.Sum(s => (float)(s.longitud_corte/1000)),2),
                           Tramo_Perfil = Math.Round((float)g.FirstOrDefault().tramo_perfil, 2)                         
                       };
            if (data != null)
            {
                if (datagridviewNE1.InvokeRequired == true)
                {
                    datagridviewNE1.Invoke((MethodInvoker)delegate
                    {
                        datagridviewNE1.DataSource = data.ToList();
                        if (datagridviewNE1.RowCount <= 0)
                        {
                            datagridviewNE1.DataSource = null;
                        }
                    });
                }
                else
                {
                    datagridviewNE1.DataSource = data.ToList();
                    if (datagridviewNE1.RowCount <= 0)
                    {
                        datagridviewNE1.DataSource = null;
                    }
                }
            }
        }
    }
}
