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
    public partial class copy : Form
    {
        public copy()
        {
            InitializeComponent();
        }

        private void copy_Load(object sender, EventArgs e)
        {
            loadCopy();
        }

        public void loadCopy(string filter="")
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();
            StringConverter converter = new StringConverter();

            if (filter != "")
            {
                var data = (from x in cotizaciones.copyboxes where x.merge_id <= 0 && (x.ubicacion.StartsWith(filter) || x.clave.StartsWith(filter) || x.id.ToString().StartsWith(filter)) select x).AsEnumerable().Select(o => new
                {
                    Id = o.id,
                    CS = o.pic,
                    Folio = o.folio,
                    Clave = o.clave,
                    Modulo_Id = o.modulo_id,
                    Artículo = o.articulo,
                    Linea = o.linea,
                    Diseño = o.diseño,
                    Descripción = o.descripcion,
                    Ubicación = o.ubicacion,
                    Cristales = constants.getCristales(o.claves_cristales, o.news),
                    Acabado = o.acabado_perfil,
                    Largo = o.largo,
                    Alto = o.alto,
                    Cantidad = o.cantidad,
                    Total = o.total
                });
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
            else
            {
                var data = (from x in cotizaciones.copyboxes where x.merge_id <= 0 select x).AsEnumerable().Select(o => new
                {
                    Id = o.id,
                    CS = o.pic,
                    Folio = o.folio,
                    Clave = o.clave,
                    Modulo_Id = o.modulo_id,
                    Artículo = o.articulo,
                    Linea = o.linea,
                    Diseño = o.diseño,
                    Descripción = o.descripcion,
                    Ubicación = o.ubicacion,
                    Cristales = constants.getCristales(o.claves_cristales, o.news),
                    Acabado = o.acabado_perfil,
                    Largo = o.largo,
                    Alto = o.alto,
                    Cantidad = o.cantidad,
                    Total = o.total
                });
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

        private void button1_Click(object sender, EventArgs e)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE copybox");
            cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (copybox, RESEED, 1)");
            loadCopy();
            datagridviewNE1.Refresh();
        }

        //Añadir
        private void añadirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(datagridviewNE1.RowCount > 0)
            {
                insertCopy((int)datagridviewNE1.CurrentRow.Cells[0].Value);
            }
        }

        //Borrar
        private void borrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagridviewNE1.RowCount > 0)
            {
                deleteCopy((int)datagridviewNE1.CurrentRow.Cells[0].Value);
            }         
        }

        private void deleteCopy(int id)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            var copy = (from x in cotizaciones.copyboxes where x.id == id select x).SingleOrDefault();

            if (copy != null)
            {
                cotizaciones.copyboxes.Remove(copy);
                cotizaciones.SaveChanges();
                loadCopy();
                datagridviewNE1.Refresh();
            }
        }

        private void insertCopy(int id)
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            var copy = (from x in cotizaciones.copyboxes where x.id == id select x).SingleOrDefault();

            if (copy != null)
            {
                var paste = new modulos_cotizaciones()
                {
                    folio = 00000,
                    modulo_id = copy.modulo_id,
                    descripcion = copy.descripcion,
                    mano_obra = copy.mano_obra,
                    dimensiones = copy.dimensiones,
                    acabado_perfil = copy.acabado_perfil,
                    claves_cristales = copy.claves_cristales,
                    cantidad = copy.cantidad,
                    articulo = copy.articulo,
                    linea = copy.linea,
                    diseño = copy.diseño,
                    clave = copy.clave,
                    total = copy.total,
                    largo = copy.largo,
                    alto = copy.alto,
                    flete = copy.flete,
                    utilidad = copy.utilidad,
                    desperdicio = copy.desperdicio,
                    claves_otros = copy.claves_otros,
                    claves_herrajes = copy.claves_herrajes,
                    ubicacion = copy.ubicacion,
                    pic = copy.pic,
                    claves_perfiles = copy.claves_perfiles,
                    merge_id = copy.merge_id,
                    concept_id = copy.concept_id,
                    sub_folio = constants.sub_folio,
                    dir = copy.dir,
                    news = copy.news,
                    new_desing = copy.new_desing,
                    orden = constants.getCountPartidas()
                };
                cotizaciones.modulos_cotizaciones.Add(paste);
                cotizaciones.copyboxes.Remove(copy);
                cotizaciones.SaveChanges();

                if (copy.modulo_id == -1)
                {
                    int last = 0;
                    var g = (from x in cotizaciones.modulos_cotizaciones orderby x.id descending select x).FirstOrDefault();

                    if (g != null)
                    {
                        last = g.id;
                    }

                    var last_m = (from x in cotizaciones.modulos_cotizaciones where x.id == last select x).SingleOrDefault();

                    if (last_m != null)
                    {
                        last_m.concept_id = last;
                        var copy_m = from x in cotizaciones.copyboxes where x.merge_id == id select x;

                        if (copy_m != null)
                        {
                            foreach (var v in copy_m)
                            {
                                var paste_m = new modulos_cotizaciones()
                                {
                                    folio = 00000,
                                    modulo_id = v.modulo_id,
                                    descripcion = v.descripcion,
                                    mano_obra = v.mano_obra,
                                    dimensiones = v.dimensiones,
                                    acabado_perfil = v.acabado_perfil,
                                    claves_cristales = v.claves_cristales,
                                    cantidad = v.cantidad,
                                    articulo = v.articulo,
                                    linea = v.linea,
                                    diseño = v.diseño,
                                    clave = v.clave,
                                    total = v.total,
                                    largo = v.largo,
                                    alto = v.alto,
                                    flete = v.flete,
                                    utilidad = v.utilidad,
                                    desperdicio = v.desperdicio,
                                    claves_otros = v.claves_otros,
                                    claves_herrajes = v.claves_herrajes,
                                    ubicacion = v.ubicacion,
                                    pic = v.pic,
                                    claves_perfiles = v.claves_perfiles,
                                    merge_id = last,
                                    concept_id = v.concept_id,
                                    sub_folio = constants.sub_folio,
                                    dir = v.dir,
                                    news = v.news,
                                    new_desing = v.new_desing,
                                    orden = 0
                                };
                                cotizaciones.modulos_cotizaciones.Add(paste_m);
                                cotizaciones.copyboxes.Remove(v);
                            }
                        }
                    }
                }
                cotizaciones.SaveChanges();
                loadCopy();
                datagridviewNE1.Refresh();
                ((Form1)Application.OpenForms["form1"]).reloadAll();
                ((Form1)Application.OpenForms["form1"]).refreshNewArticulo(5);
                if (Application.OpenForms["articulos_cotizacion"] != null)
                {
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).resetRowSelect();
                    ((articulos_cotizacion)Application.OpenForms["articulos_cotizacion"]).loadALL();
                }
                if(datagridviewNE1.RowCount == 0)
                {
                    cotizaciones.Database.ExecuteSqlCommand("TRUNCATE TABLE copybox");
                    cotizaciones.Database.ExecuteSqlCommand("DBCC CHECKIDENT (copybox, RESEED, 1)");
                    this.Close();
                }
            }
        }

        //Buscar
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            loadCopy(textBox1.Text);
        }
    }
}
