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
    public partial class desglose_costo_m : Form
    {
        float total_t = 0, sub_total = 0, alum_t = 0, cris_t = 0, herra_t = 0, otros_t = 0, cost_add = 0, costo_ext = 0;
        int m_id = 0;

        public desglose_costo_m(int m_id)
        {
            InitializeComponent();
            textBox19.TextChanged += TextBox19_TextChanged;
            this.m_id = m_id;
            //------------->
            init(this.m_id);
            button1.Focus();
        }

        private void TextBox19_TextChanged(object sender, EventArgs e)
        {
            if(!constants.isInteger(textBox19.Text))
            {
                textBox19.Clear();
            }
        }

        private void resetCost()
        {
            datagridviewNE1.Rows.Clear();
            //-------------------------->
            total_t = 0;
            sub_total = 0;
            alum_t = 0;
            cris_t = 0;
            herra_t = 0;
            otros_t = 0;
            cost_add = 0;
            costo_ext = 0;
        }

        private float getCostoMateriales()
        {
            return (float)Math.Round((alum_t + herra_t + otros_t + cris_t + costo_ext), 2);
        }

        private float getSubTotal(float flete, float mano_obra, float utilidad)
        {
            return getCostoMateriales() * flete * mano_obra * utilidad;
        }

        private void init(int m_id, int p_cant=0)
        {
            using (cotizaciones_local cotizaciones = new cotizaciones_local())
            {
                var modulo = (from x in cotizaciones.modulos_cotizaciones where x.id == m_id select x).SingleOrDefault();
                if (modulo != null)
                {
                    textBox6.Text = modulo.articulo;
                    textBox7.Text = modulo.clave;
                    textBox8.Text = modulo.ubicacion;
                    textBox9.Text = modulo.largo.ToString();
                    textBox10.Text = modulo.alto.ToString();
                    textBox11.Text = modulo.acabado_perfil.ToString();
                    textBox12.Text = modulo.linea;
                    //Cantidad
                    int t_cant = p_cant > 0 ? p_cant : (int)modulo.cantidad;
                    textBox5.Text = t_cant.ToString();
                    textBox19.Text = textBox5.Text;
                    //
                    richTextBox1.Text = modulo.descripcion;                   
                    pictureBox1.Image = constants.byteToImage(modulo.pic);
                    if (modulo.modulo_id > 0)
                    {
                        textBox1.Text = modulo.desperdicio.ToString();
                        textBox2.Text = modulo.flete.ToString();
                        textBox3.Text = modulo.mano_obra.ToString();
                        textBox4.Text = modulo.utilidad.ToString();
                        loadModulo((int)modulo.modulo_id, (float)modulo.mano_obra / 100, t_cant, modulo.dimensiones, modulo.claves_cristales, (float)modulo.flete / 100, (float)modulo.desperdicio / 100, (float)modulo.utilidad / 100, modulo.claves_otros, modulo.claves_herrajes, modulo.claves_perfiles, modulo.news, modulo.acabado_perfil, modulo.id);
                    }
                    else
                    {
                        int c = 0;
                        float desperdicio = 0, flete = 0, mano_o = 0, utilidad = 0;
                        var merge_c = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == m_id select x);
                        if(merge_c != null)
                        {
                            foreach(var u in merge_c)
                            {
                                c++;
                                desperdicio = desperdicio + (float)u.desperdicio;
                                flete = flete + (float)u.flete;
                                mano_o = mano_o + (float)u.mano_obra;
                                utilidad = utilidad + (float)u.utilidad;
                                loadModulo((int)u.modulo_id, (float)u.mano_obra / 100, (int)u.cantidad * t_cant, u.dimensiones, u.claves_cristales, (float)u.flete / 100, (float)u.desperdicio / 100, (float)u.utilidad / 100, u.claves_otros, u.claves_herrajes, u.claves_perfiles, u.news, u.acabado_perfil, u.id);
                            }
                            textBox1.Text = Math.Round(desperdicio / c).ToString();
                            textBox2.Text = Math.Round(flete / c).ToString();
                            textBox3.Text = Math.Round(mano_o / c).ToString();
                            textBox4.Text = Math.Round(utilidad / c).ToString();
                        }
                    }
                    acomodarDesglose();
                    textBox14.Text = "$" + Math.Round(alum_t, 2).ToString();
                    textBox15.Text = "$" + Math.Round(herra_t, 2).ToString();
                    costo_ext = costo_ext * t_cant;
                    textBox16.Text = "$" + Math.Round(otros_t, 2).ToString() + " + $" + costo_ext;
                    textBox17.Text = "$" + Math.Round(cris_t, 2).ToString();
                    textBox18.Text = "$" + getCostoMateriales().ToString();

                    //SubTotal
                    sub_total = sub_total + cost_add;
                    textBox13.Text = "$" + sub_total.ToString() + (cost_add > 0 ? (" + $" + cost_add) : "");
                    //Total
                    total_t = (float)Math.Round((sub_total) * constants.iva, 2);
                    label9.Text = "$" + total_t.ToString("n");
                }
            }                  
        }

        private void acomodarDesglose()
        {
            datagridviewNE1.Sort(datagridviewNE1.Columns[0], ListSortDirection.Ascending);
            foreach (DataGridViewRow x in datagridviewNE1.Rows)
            {
                if (x.Cells[0].Value.ToString() == "Perfil")
                {
                    x.DefaultCellStyle.BackColor = Color.LightGray;
                }
                else if (x.Cells[0].Value.ToString() == "Cristal")
                {
                    x.DefaultCellStyle.BackColor = Color.LightBlue;
                }
                else if (x.Cells[0].Value.ToString() == "Herraje")
                {
                    x.DefaultCellStyle.BackColor = Color.LightYellow;
                }
                else if (x.Cells[0].Value.ToString() == "Otros")
                {
                    x.DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }
        }

        private void loadModulo(int modulo_id, float mano_obra, int cantidad, string dimensiones, string claves_cristales, float flete, float desperdicio, float utilidad, string claves_otros, string claves_herrajes, string claves_perfiles, string new_items, string acabado, int id)
        {
            listas_entities_pva listas = new listas_entities_pva();
            List<string> n_items = new List<string>();
            bool error = false;
            float costo = 0;
            int rows = 0;
            int columns = 0;
            bool cs = false;
            string buffer = string.Empty;
            float get = 0;

            if (new_items.Length > 0)
            {
                foreach (char x in new_items)
                {
                    if (x == ';')
                    {
                        n_items.Add(buffer);
                        buffer = string.Empty;
                        continue;
                    }
                    buffer = buffer + x.ToString();
                }
            }

            var modulo = (from x in listas.modulos where x.id == modulo_id select x).SingleOrDefault();

            if (modulo != null)
            {
                cs = (bool)modulo.cs;
                string[] s_t;
                string[] d_s = dimensiones.Split(',');
                int[] d_num = new int[d_s.Length - 1];

                for (int i = 0; i < d_num.Length; i++)
                {
                    d_num[i] = int.Parse(d_s[i]);
                }

                int esquema_id = (int)modulo.id_diseño;

                var esquema = (from x in listas.esquemas where x.id == esquema_id select x).SingleOrDefault();

                int c = 0;

                if (esquema != null)
                {
                    if (esquema.marco == false)
                    {
                        c = 1;
                    }
                    rows = (int)esquema.filas;
                    columns = (int)esquema.columnas;
                }

                int[,] arr = new int[((int)modulo.secciones + 1) + c, 2];

                for (int i = 0; i < d_num.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        arr[c, 0] = d_num[i];
                    }
                    else
                    {
                        arr[c, 1] = d_num[i];
                        c++;
                    }
                }

                if (claves_perfiles.Length > 0)
                {
                    d_s = claves_perfiles.Split(',');
                    s_t = modulo.id_aluminio.Split(',');

                    if (s_t.Length != d_s.Length)
                    {
                        error = true;
                    }
                    else
                    {
                        for (int i = 0; i < s_t.Length - 1; i++)
                        {
                            get = leerModuloAluminio(listas, s_t[i], d_s[i], arr, acabado, cantidad);
                            if (get >= 0)
                            {
                                costo = costo + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                    }
                }

                // new items ---------------------------
                if (n_items.Count > 0)
                {
                    foreach (string z in n_items)
                    {
                        string[] p = z.Split(',');
                        if (p[0] == "1")
                        {
                            get = leerNewModuloAluminio(listas, p[1], constants.stringToInt(p[2]), p[3], constants.stringToInt(p[4]), p[5], arr, cantidad);
                            if (get >= 0)
                            {
                                costo = costo + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                    }
                }
                // --------------------------------------

                //Calcular Desperdicio
                costo = costo + (costo * desperdicio);
                alum_t = alum_t + costo;
                //-------------------------------->

                if (claves_herrajes.Length > 0)
                {
                    d_s = claves_herrajes.Split(',');
                    s_t = modulo.id_herraje.Split(',');

                    if (s_t.Length != d_s.Length)
                    {
                        error = true;
                    }
                    else
                    {
                        for (int i = 0; i < s_t.Length - 1; i++)
                        {
                            get = leerModuloHerrajes(listas, s_t[i], d_s[i], cantidad);
                            if (get >= 0)
                            {
                                costo = costo + get;
                                herra_t = herra_t + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                    }
                }

                if (claves_otros.Length > 0)
                {
                    d_s = claves_otros.Split(',');
                    s_t = modulo.id_otros.Split(',');

                    if (s_t.Length != d_s.Length)
                    {
                        error = true;
                    }
                    else
                    {
                        for (int i = 0; i < s_t.Length - 1; i++)
                        {
                            get = leerModuloOtros(listas, s_t[i], d_s[i], arr, cs, rows, columns, cantidad);
                            if (get >= 0)
                            {
                                costo = costo + get;
                                otros_t = otros_t + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                    }
                }

                if (claves_cristales.Length > 0)
                {
                    d_s = claves_cristales.Split(',');
                    s_t = modulo.clave_vidrio.Split(',');

                    if (s_t.Length != d_s.Length)
                    {
                        error = true;
                    }
                    else
                    {
                        for (int i = 0; i < s_t.Length - 1; i++)
                        {
                            get = leerModuloCristales(listas, s_t[i], d_s[i], arr, cs, rows, columns, cantidad);
                            if (get >= 0)
                            {
                                costo = costo + get;
                                cris_t = cris_t + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                    }
                }

                //new items ----------------
                if (n_items.Count > 0)
                {
                    foreach (string z in n_items)
                    {
                        string[] p = z.Split(',');
                        if (p[0] == "2")
                        {
                            get = leerNewModuloCristales(listas, p[1], constants.stringToInt(p[2]), constants.stringToInt(p[4]), arr, cs, rows, columns, cantidad);
                            if (get >= 0)
                            {
                                costo = costo + get;
                                cris_t = cris_t + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                        else if (p[0] == "3")
                        {
                            get = leerNewModuloHerrajes(listas, p[1], constants.stringToInt(p[2]), cantidad);
                            if (get >= 0)
                            {
                                costo = costo + get;
                                herra_t = herra_t + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                        else if (p[0] == "4")
                        {
                            get = leerNewModuloOtros(listas, p[1], constants.stringToInt(p[2]), p[3], constants.stringToInt(p[4]), arr, cs, rows, columns, cantidad);
                            if (get >= 0)
                            {
                                costo = costo + get;
                                otros_t = otros_t + get;
                            }
                            else
                            {
                                error = true;
                            }
                        }
                        //
                        else if (p[0] == "5")
                        {
                            if (p.Length == 4)
                            {
                                if (p[3] != "Total")
                                {
                                    costo = costo + constants.stringToFloat(p[2]);
                                    costo_ext = costo_ext + constants.stringToFloat(p[2]);
                                }
                                else
                                {
                                    cost_add = cost_add + constants.stringToFloat(p[2]);                                   
                                }
                            }
                        }
                    }
                }
                //------------------------->
                sub_total = sub_total + costo * (flete + 1) * (mano_obra + 1) * (utilidad + 1);
            }         
        }

        private void setNewRow(string componente, int id_articulo, string clave, string articulo, float cantidad, float metros_lineales, float metros_cuadrados, float precio_u, float total)
        {
            bool find = false;
            metros_lineales = metros_lineales / 1000f;
            if (cantidad > 0)
            {
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    if (x.Cells[2].Value.ToString() == clave)
                    {
                        x.Cells[4].Value = constants.stringToFloat(x.Cells[4].Value.ToString()) + cantidad;
                        if (metros_lineales > 0)
                        {
                            x.Cells[5].Value = Math.Round(constants.stringToFloat(x.Cells[5].Value.ToString().Replace(" m", "")) + metros_lineales, 2) + " m";
                        }
                        if (metros_cuadrados > 0)
                        {
                            x.Cells[6].Value = Math.Round(constants.stringToFloat(x.Cells[6].Value.ToString().Replace(" m2", "")) + metros_cuadrados, 2) + " m2";
                        }
                        x.Cells[8].Value = constants.stringToFloat(x.Cells[8].Value.ToString()) + total;
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    datagridviewNE1.Rows.Add(componente, id_articulo.ToString() != "0" ? id_articulo.ToString() : "", clave, articulo, cantidad, metros_lineales.ToString() != "0" ? (Math.Round(metros_lineales, 2).ToString() + " m") : "", metros_cuadrados.ToString() != "0" ? (Math.Round(metros_cuadrados, 2).ToString() + " m2") : "", precio_u, total);
                }
            }
        }

        private float leerModuloAluminio(listas_entities_pva listas, string clave_modulo, string n_id, int[,] dim, string acabado, int t_count = 1)
        {
            string clave = string.Empty;
            float count = 0;
            string buffer = string.Empty;
            string dir = string.Empty;
            int seccion = -1;
            float total = 0;
            float ext = 0;
            string[] z;
            float pf = 0;

            foreach (char alm in clave_modulo)
            {
                if (alm == ':')
                {
                    clave = buffer;
                    buffer = string.Empty;
                    continue;
                }
                if (alm == '-')
                {
                    count = constants.stringToFloat(buffer);
                    buffer = string.Empty;
                    continue;
                }
                if (alm == '$')
                {
                    dir = buffer;
                    buffer = string.Empty;
                    continue;
                }
                buffer = buffer + alm.ToString();
            }
            seccion = constants.stringToInt(buffer);
            z = n_id.Split('-');
            clave = z[0];
            //Cantidad
            if (z.Length > 1)
            {
                count = constants.stringToFloat(z[1]);
                //Acabado
                if (z.Length == 3)
                {
                    acabado = z[2];
                }
            }

            count = count * t_count;

            var perfil = (from c in listas.perfiles where c.clave == clave select c).SingleOrDefault();

            if (perfil != null)
            {
                float largo = dim[seccion, 0] / 1000f;
                float alto = dim[seccion, 1] / 1000f;
                var color = (from u in listas.colores_aluminio where u.clave == acabado select u).SingleOrDefault();

                if (color == null)
                {
                    if (acabado == "crudo")
                    {
                        pf = (float)perfil.crudo;
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.crudo / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));

                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.crudo / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                    }
                    if (acabado == "blanco")
                    {
                        pf = (float)perfil.blanco;
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.blanco / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.blanco / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                    }
                    else if (acabado == "hueso")
                    {
                        pf = (float)perfil.hueso;
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.hueso / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.hueso / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                    }
                    else if (acabado == "champagne")
                    {
                        pf = (float)perfil.champagne;
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.champagne / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.champagne / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                    }
                    else if (acabado == "gris")
                    {
                        pf = (float)perfil.gris;
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.gris / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.gris / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                    }
                    else if (acabado == "negro")
                    {
                        pf = (float)perfil.negro;
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.negro / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.negro / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                    }
                    else if (acabado == "brillante")
                    {
                        pf = (float)perfil.brillante;
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.brillante / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.brillante / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                    }
                    else if (acabado == "natural")
                    {
                        pf = (float)perfil.natural_1;
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.natural_1 / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.natural_1 / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                    }
                    else if (acabado == "madera")
                    {
                        pf = (float)perfil.madera;
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.madera / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.madera / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                    }
                    else if (acabado == "chocolate")
                    {
                        pf = (float)perfil.chocolate;
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.chocolate / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.chocolate / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                    }
                    else if (acabado == "acero_inox")
                    {
                        pf = (float)perfil.acero_inox;
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.acero_inox / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.acero_inox / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                    }
                    else if (acabado == "bronce")
                    {
                        pf = (float)perfil.bronce;
                        if (dir == "largo")
                        {
                            total = (float)(((perfil.bronce / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                        else if (dir == "alto")
                        {
                            total = (float)(((perfil.bronce / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(total, 2));
                        }
                    }
                }
                else
                {
                    pf = (float)perfil.crudo;
                    if (dir == "largo")
                    {
                        ext = (float)(largo * color.costo_extra_ml);
                        total = (float)((((perfil.crudo / perfil.largo) * largo) * count) + ((((largo * (perfil.perimetro_dm2_ml / 100)) * (color.precio)) + ext)) * count);
                        setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)((((perfil.crudo / perfil.largo) * largo) * count) + ((((largo * (perfil.perimetro_dm2_ml / 100)) * (color.precio)) + ext))), 2), (float)Math.Round(total, 2));
                    }
                    else if (dir == "alto")
                    {
                        ext = (float)(alto * color.costo_extra_ml);
                        total = (float)((((perfil.crudo / perfil.largo) * alto) * count) + ((((alto * (perfil.perimetro_dm2_ml / 100)) * (color.precio)) + ext)) * count);
                        setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)((((perfil.crudo / perfil.largo) * alto) * count) + ((((alto * (perfil.perimetro_dm2_ml / 100)) * (color.precio)) + ext))), 2), (float)Math.Round(total, 2));
                    }
                }
            }
            else
            {
                total = -1;
            }
            return total;
        }

        private float leerModuloCristales(listas_entities_pva listas, string clave_modulo, string n_id, int[,] dim, bool cs, int rows, int columns, int t_count = 1)
        {
            string clave = "";
            float count = 0;
            int seccion = -1;
            string buffer = "";
            float total = 0;
            string[] z;
            float cr = 0;

            foreach (char x in clave_modulo)
            {
                if (x == ':')
                {
                    clave = buffer;
                    buffer = "";
                    continue;
                }
                if (x == '$')
                {
                    count = constants.stringToFloat(buffer);
                    buffer = "";
                    continue;
                }
                buffer = buffer + x.ToString();
            }
            seccion = constants.stringToInt(buffer);
            z = n_id.Split('-');
            if (z.Length > 0)
            {
                clave = z[0];
                if (z.Length > 1)
                {
                    count = constants.stringToFloat(z[1]);
                }
            }
            else
            {
                clave = n_id;
            }

            count = count * t_count;

            var cristal = (from v in listas.lista_costo_corte_e_instalado where v.clave == clave select v).SingleOrDefault();

            if (cristal != null)
            {
                cr = (float)Math.Round((float)cristal.costo_corte_m2, 2);
                float largo = dim[seccion, 0] / 1000f;
                float alto = dim[seccion, 1] / 1000f;
                if (cs == false)
                {
                    if (rows > 0 && columns > 0)
                    {
                        total = (float)((cristal.costo_corte_m2 * (largo / columns) * (alto / rows)) * count);
                        setNewRow("Cristal", 0, cristal.clave, cristal.articulo, count, 0, (float)Math.Round(((largo / columns) * (alto / rows)) * count, 2), cr, (float)Math.Round(total, 2));
                    }
                }
                else
                {
                    total = (float)((cristal.costo_corte_m2 * largo * alto) * count);
                    setNewRow("Cristal", 0, cristal.clave, cristal.articulo, count, 0, (float)Math.Round((largo * alto) * count, 2), cr, (float)Math.Round(total, 2));
                }
            }
            else
            {
                total = -1;
            }
            return total;
        }

        private float leerModuloHerrajes(listas_entities_pva listas, string module, string n_id, int t_count = 1)
        {
            string clave = string.Empty;
            float count = 0;
            string buffer = "";
            float total = 0;
            int seccion = -1;
            string[] z;
            float hrr = 0;

            foreach (char x in module)
            {
                if (x == ':')
                {
                    clave = buffer;
                    buffer = "";
                    continue;
                }
                if (x == '$')
                {
                    count = constants.stringToFloat(buffer);
                    buffer = "";
                    continue;
                }
                buffer = buffer + x.ToString();
            }
            seccion = constants.stringToInt(buffer);
            z = n_id.Split('-');
            clave = z[0];
            if (z.Length > 1)
            {
                count = constants.stringToFloat(z[1]);
            }

            count = count * t_count;

            var herraje = (from v in listas.herrajes where v.clave == clave select v).SingleOrDefault();

            if (herraje != null)
            {
                hrr = (float)Math.Round((float)herraje.precio, 2);
                total = total + (float)(herraje.precio * count);
                setNewRow("Herraje", herraje.id, herraje.clave, herraje.articulo, count, 0, 0, hrr, (float)Math.Round(total, 2));
            }
            else
            {
                total = -1;
            }
            return total;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            resetCost();
            init(m_id, constants.stringToInt(textBox19.Text));
        }

        private float leerModuloOtros(listas_entities_pva listas, string module, string n_id, int[,] dim, bool cs, int rows, int columns, int t_count = 1)
        {
            string clave = string.Empty;
            float count = 0;
            string buffer = "";
            string dir = "";
            int seccion = -1;
            float total = 0;
            string[] z;
            float otr = 0;

            foreach (char x in module)
            {
                if (x == ':')
                {
                    clave = buffer;
                    buffer = "";
                    continue;
                }
                if (x == '-')
                {
                    count = constants.stringToFloat(buffer);
                    buffer = "";
                    continue;
                }
                if (x == '$')
                {
                    dir = buffer;
                    buffer = "";
                    continue;
                }
                buffer = buffer + x.ToString();
            }
            seccion = constants.stringToInt(buffer);
            z = n_id.Split('-');
            clave = z[0];
            if (z.Length > 1)
            {
                count = constants.stringToFloat(z[1]);
            }

            count = count * t_count;

            var otro = (from v in listas.otros where v.clave == clave select v).SingleOrDefault();

            if (otro != null)
            {
                otr = (float)Math.Round((float)otro.precio, 2);
                float largo = dim[seccion, 0] / 1000f;
                float alto = dim[seccion, 1] / 1000f;
                if (otro.largo > 0 && otro.alto <= 0)
                {
                    if (dir == "largo")
                    {
                        total = total + (float)(otro.precio * largo * count);
                        setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, ((largo * 1000) * count), 0, otr, (float)Math.Round(total, 2));
                    }
                    else if (dir == "alto")
                    {
                        total = total + (float)(otro.precio * alto * count);
                        setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, ((alto * 1000) * count), 0, otr, (float)Math.Round(total, 2));
                    }
                }
                else if (otro.largo <= 0 && otro.alto > 0)
                {
                    if (dir == "largo")
                    {
                        total = total + (float)(otro.precio * largo * count);
                        setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, ((largo * 1000) * count), 0, otr, (float)Math.Round(total, 2));
                    }
                    else if (dir == "alto")
                    {
                        total = total + (float)(otro.precio * alto * count);
                        setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, ((alto * 1000) * count), 0, otr, (float)Math.Round(total, 2));
                    }
                }
                else if (otro.largo > 0 && otro.alto > 0)
                {
                    if (cs == false)
                    {
                        if (rows > 0 && columns > 0)
                        {
                            total = total + (float)(otro.precio * (largo / columns) * (alto / rows) * count);
                            setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, 0, (float)Math.Round(((largo / columns) * (alto / rows)) * count, 2), otr, (float)Math.Round(total, 2));
                        }
                    }
                    else
                    {
                        total = total + (float)(otro.precio * largo * alto * count);
                        setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, 0, (float)Math.Round((largo * alto) * count, 2), otr, (float)Math.Round(total, 2));
                    }
                }
                else
                {
                    total = total + (float)(otro.precio * count);
                    setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, 0, 0, otr, (float)Math.Round(total, 2));
                }
            }
            else
            {
                total = -1;
            }
            return total;
        }

        private float leerNewModuloAluminio(listas_entities_pva listas, string clave, int count, string dir, int seccion, string acabado, int[,] dim, int t_count = 1)
        {
            float costo = 0;
            float ext = 0;
            var perfil = (from c in listas.perfiles where c.clave == clave select c).SingleOrDefault();

            if (perfil != null)
            {
                float largo = dim[seccion, 0] / 1000f;
                float alto = dim[seccion, 1] / 1000f;
                var color = (from u in listas.colores_aluminio where u.clave == acabado select u).SingleOrDefault();
                float pf = 0;
                count = count * t_count;

                if (color == null)
                {
                    if (acabado == "crudo")
                    {
                        pf = (float)perfil.crudo;
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.crudo / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.crudo / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                    }
                    if (acabado == "blanco")
                    {
                        pf = (float)perfil.blanco;
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.blanco / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.blanco / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                    }
                    else if (acabado == "hueso")
                    {
                        pf = (float)perfil.hueso;
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.hueso / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.hueso / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                    }
                    else if (acabado == "champagne")
                    {
                        pf = (float)perfil.champagne;
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.champagne / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.champagne / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                    }
                    else if (acabado == "gris")
                    {
                        pf = (float)perfil.gris;
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.gris / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.gris / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                    }
                    else if (acabado == "negro")
                    {
                        pf = (float)perfil.negro;
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.negro / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.negro / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                    }
                    else if (acabado == "brillante")
                    {
                        pf = (float)perfil.brillante;
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.brillante / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.brillante / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                    }
                    else if (acabado == "natural")
                    {
                        pf = (float)perfil.natural_1;
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.natural_1 / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.natural_1 / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                    }
                    else if (acabado == "madera")
                    {
                        pf = (float)perfil.madera;
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.madera / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.madera / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                    }
                    else if (acabado == "chocolate")
                    {
                        pf = (float)perfil.chocolate;
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.chocolate / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.chocolate / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                    }
                    else if (acabado == "acero_inox")
                    {
                        pf = (float)perfil.acero_inox;
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.acero_inox / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.acero_inox / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                    }
                    else if (acabado == "bronce")
                    {
                        pf = (float)perfil.bronce;
                        if (dir == "largo")
                        {
                            costo = (float)(((perfil.bronce / perfil.largo) * largo) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                        else if (dir == "alto")
                        {
                            costo = (float)(((perfil.bronce / perfil.largo) * alto) * count);
                            setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)(pf / perfil.largo), 2), (float)Math.Round(costo, 2));
                        }
                    }
                }
                else
                {
                    pf = (float)perfil.crudo;
                    if (dir == "largo")
                    {
                        ext = (float)(largo * color.costo_extra_ml);
                        costo = (float)((((perfil.crudo / perfil.largo) * largo) * count) + ((((largo * (perfil.perimetro_dm2_ml / 100)) * (color.precio)) + ext)) * count);
                        setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((largo * 1000) * count), 0, (float)Math.Round((float)((((perfil.crudo / perfil.largo) * largo) * count) + ((((largo * (perfil.perimetro_dm2_ml / 100)) * (color.precio)) + ext))), 2), (float)Math.Round(costo, 2));
                    }
                    else if (dir == "alto")
                    {
                        ext = (float)(alto * color.costo_extra_ml);
                        costo = (float)((((perfil.crudo / perfil.largo) * alto) * count) + ((((alto * (perfil.perimetro_dm2_ml / 100)) * (color.precio)) + ext)) * count);
                        setNewRow("Perfil", perfil.id, perfil.clave, perfil.articulo, count, ((alto * 1000) * count), 0, (float)Math.Round((float)((((perfil.crudo / perfil.largo) * alto) * count) + ((((alto * (perfil.perimetro_dm2_ml / 100)) * (color.precio)) + ext))), 2), (float)Math.Round(costo, 2));
                    }
                }
            }
            else
            {
                costo = -1;
            }
            return costo;
        }

        private float leerNewModuloCristales(listas_entities_pva listas, string clave, int count, int seccion, int[,] dim, bool cs, int rows, int columns, int t_count = 1)
        {
            float total = 0;
            float cr = 0;
            var cristal = (from v in listas.lista_costo_corte_e_instalado where v.clave == clave select v).SingleOrDefault();

            if (cristal != null)
            {
                float largo = dim[seccion, 0] / 1000f;
                float alto = dim[seccion, 1] / 1000f;
                cr = (float)Math.Round((float)cristal.costo_corte_m2, 2);
                count = count * t_count;

                if (cs == false)
                {
                    if (rows > 0 && columns > 0)
                    {                        
                        total = (float)((cristal.costo_corte_m2 * (largo / columns) * (alto / rows)) * count);
                        setNewRow("Cristal", 0, cristal.clave, cristal.articulo, count, 0, (float)Math.Round(((largo / columns) * (alto / rows)) * count, 2), cr, (float)Math.Round(total, 2));
                    }
                }
                else
                {
                    total = (float)((cristal.costo_corte_m2 * largo * alto) * count);
                    setNewRow("Cristal", 0, cristal.clave, cristal.articulo, count, 0, (float)Math.Round((largo * alto) * count, 2), cr, (float)Math.Round(total, 2));
                }
            }
            else
            {
                total = -1;
            }
            return total;
        }

        private float leerNewModuloHerrajes(listas_entities_pva listas, string clave, int count, int t_count = 1)
        {
            float total = 0;
            float hrr = 0;
            var herraje = (from v in listas.herrajes where v.clave == clave select v).SingleOrDefault();

            if (herraje != null)
            {
                count = count * t_count;
                hrr = (float)Math.Round((float)herraje.precio, 2);
                total = total + (float)(herraje.precio * count);
                setNewRow("Herraje", herraje.id, herraje.clave, herraje.articulo, count, 0, 0, hrr, (float)Math.Round(total, 2));
            }
            else
            {
                total = -1;
            }
            return total;
        }

        private float leerNewModuloOtros(listas_entities_pva listas, string clave, int count, string dir, int seccion, int[,] dim, bool cs, int rows, int columns, int t_count = 1)
        {
            float total = 0;
            float otr = 0;
            var otro = (from v in listas.otros where v.clave == clave select v).SingleOrDefault();

            if (otro != null)
            {
                otr = (float)Math.Round((float)otro.precio, 2);
                float largo = dim[seccion, 0] / 1000f;
                float alto = dim[seccion, 1] / 1000f;
                count = count * t_count;

                if (otro.largo > 0 && otro.alto <= 0)
                {
                    if (dir == "largo")
                    {
                        total = total + (float)(otro.precio * largo * count);
                        setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, ((largo * 1000) * count), 0, otr, (float)Math.Round(total, 2));
                    }
                    else if (dir == "alto")
                    {
                        total = total + (float)(otro.precio * alto * count);
                        setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, ((alto * 1000) * count), 0, otr, (float)Math.Round(total, 2));
                    }
                }
                else if (otro.largo <= 0 && otro.alto > 0)
                {
                    if (dir == "largo")
                    {
                        total = total + (float)(otro.precio * largo * count);
                        setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, ((largo * 1000) * count), 0, otr, (float)Math.Round(total, 2));
                    }
                    else if (dir == "alto")
                    {
                        total = total + (float)(otro.precio * alto * count);
                        setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, ((alto * 1000) * count), 0, otr, (float)Math.Round(total, 2));
                    }
                }
                else if (otro.largo > 0 && otro.alto > 0)
                {
                    if (cs == false)
                    {
                        if (rows > 0 && columns > 0)
                        {
                            total = total + (float)(otro.precio * (largo / columns) * (alto / rows) * count);
                            setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, 0, (float)Math.Round(((largo / columns) * (alto / rows)) * count, 2), otr, (float)Math.Round(total, 2));
                        }
                    }
                    else
                    {
                        total = total + (float)(otro.precio * largo * alto * count);
                        setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, 0, (float)Math.Round((largo * alto) * count, 2), otr, (float)Math.Round(total, 2));
                    }
                }
                else
                {
                    total = total + (float)(otro.precio * count);
                    setNewRow("Otros", otro.id, otro.clave, otro.articulo, count, 0, 0, otr, (float)Math.Round(total, 2));
                }
            }
            else
            {
                total = -1;
            }
            return total;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (modulo_data md = new modulo_data())
            {
                foreach (DataGridViewRow x in datagridviewNE1.Rows)
                {
                    DataRow row = md.Tables[1].NewRow();
                    row[0] = x.Cells[0].Value;
                    row[1] = x.Cells[1].Value;
                    row[2] = x.Cells[2].Value;
                    row[3] = x.Cells[3].Value;
                    row[4] = x.Cells[4].Value;
                    row[5] = x.Cells[5].Value;
                    row[6] = x.Cells[6].Value;
                    row[7] = x.Cells[7].Value;
                    row[8] = x.Cells[8].Value;
                    md.Tables[1].Rows.Add(row);
                }
                Bitmap bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.DrawToBitmap(bm, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
                Bitmap gm_2 = new Bitmap(bm, 120, 105);
                md.Tables["img_modulo"].Rows.Clear();
                DataRow row_2 = md.Tables["img_modulo"].NewRow();
                row_2[0] = constants.imageToByte(gm_2);
                md.Tables["img_modulo"].Rows.Add(row_2);
                new modulo_precios(md, textBox7.Text, textBox6.Text, textBox12.Text, textBox11.Text, textBox5.Text, textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, total_t.ToString(), "Largo: " + textBox9.Text + " - " + "Alto: " + textBox10.Text, textBox14.Text, textBox15.Text, textBox16.Text, textBox17.Text, sub_total.ToString(), cost_add > 0 ? ("+ (" + cost_add.ToString() + ")") : "", "M/L", "M/C", textBox8.Text).ShowDialog(this);
                bm = null;
                gm_2 = null;
            }
        }
    }
}
