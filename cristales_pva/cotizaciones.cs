using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace cristales_pva
{
    class cotizaciones
    {
        private void deleteAllDeletedArticulos()
        {
            localDateBaseEntities3 delete = new localDateBaseEntities3();
            sqlDateBaseManager sql = new sqlDateBaseManager();

            var t_fila = (from x in delete.filas_borradas select x);

            foreach (var fila in t_fila)
            {
                if (fila != null)
                {
                    switch (fila.type)
                    {
                        case 1:
                            sql.deleteFilasArticulos("cristales_cotizados", (int)fila.articulo_id);
                            break;
                        case 2:
                            sql.deleteFilasArticulos("aluminio_cotizado", (int)fila.articulo_id);
                            break;
                        case 3:
                            sql.deleteFilasArticulos("herrajes_cotizados", (int)fila.articulo_id);
                            break;
                        case 4:
                            sql.deleteFilasArticulos("otros_cotizaciones", (int)fila.articulo_id);
                            break;
                        case 5:
                            sql.deleteFilasArticulos("modulos_cotizaciones", (int)fila.articulo_id);
                            break;
                        default: break;
                    }
                }
            }
        }
        //------------------------------------------

        public void guardarCotizacion(TextBox textbox, TextBox textbox2, Button boton, Label label, BackgroundWorker worker, bool new_cotizacion=false, Form form=null)
        {
            sqlDateBaseManager sql = new sqlDateBaseManager();
            cotizaciones_local cotizaciones = new cotizaciones_local();
            int folio = constants.setFolio();                                
        
            try
            {
                var t_cristales = (from x in cotizaciones.cristales_cotizados select x);
                var t_aluminios = (from x in cotizaciones.aluminio_cotizado select x);
                var t_herrajes = (from x in cotizaciones.herrajes_cotizados select x);
                var t_otros = (from x in cotizaciones.otros_cotizaciones select x);
                var t_modulos = (from x in cotizaciones.modulos_cotizaciones select x);

                if (sql.findSQLValue("folio", "folio", "cotizaciones", constants.folio_abierto.ToString()) == false)
                {
                    if (sql.isFolioExist(folio) == false)
                    {
                        if (textbox.Text != "")
                        {
                            deleteAllDeletedArticulos();
                            sql.insertCotizacion(folio, textbox.Text, constants.user, DateTime.Today.ToString("dd/MM/yyyy") + " -", textbox2.Text, constants.desc_cotizacion, constants.utilidad_cotizacion, constants.org_name, constants.iva_desglosado, "Sin Registro", constants.tc);
                            constants.cotizacion_error = false;
                            constants.folio_abierto = folio;
                            constants.nombre_cotizacion = textbox.Text;
                            constants.nombre_proyecto = textbox2.Text;
                            constants.autor_cotizacion = constants.user;
                            constants.fecha_cotizacion = DateTime.Today.ToString("dd/MM/yyyy");

                            foreach (var cristales in t_cristales)
                            {
                                if (cristales != null)
                                {
                                    cristales.folio = constants.folio_abierto;
                                    sql.insertCotizacionCristal(constants.folio_abierto, cristales.clave, cristales.articulo, cristales.lista, (float)cristales.largo, (float)cristales.alto, cristales.canteado,
                                        cristales.biselado, cristales.desconchado, cristales.pecho_paloma, cristales.perforado_media_pulgada, cristales.perforado_una_pulgada,
                                        cristales.perforado_dos_pulgadas, cristales.grabado, cristales.esmerilado, cristales.tipo_venta, (float)cristales.cantidad, (float)cristales.descuento,
                                        (float)cristales.total, cristales.proveedor, (float)cristales.filo_muerto, cristales.descripcion);
                                }
                            }

                            foreach (var aluminio in t_aluminios)
                            {
                                if (aluminio != null)
                                {
                                    aluminio.folio = constants.folio_abierto;
                                    sql.insertCotizacionAluminio(constants.folio_abierto, aluminio.clave, aluminio.articulo, aluminio.lista, aluminio.linea, aluminio.largo_total.ToString(), aluminio.acabado,
                                        (float)aluminio.cantidad, (float)aluminio.descuento, (float)aluminio.total, aluminio.proveedor, aluminio.descripcion);
                                }
                            }

                            foreach (var herrajes in t_herrajes)
                            {
                                if (herrajes != null)
                                {
                                    herrajes.folio = constants.folio_abierto;
                                    sql.insertCotizacionHerrajes(constants.folio_abierto, herrajes.clave, herrajes.articulo, herrajes.proveedor, herrajes.linea, herrajes.caracteristicas, herrajes.color,
                                        (float)herrajes.cantidad, (float)herrajes.descuento, (float)herrajes.total, herrajes.descripcion);
                                }
                            }

                            foreach (var otros in t_otros)
                            {
                                if (otros != null)
                                {
                                    otros.folio = constants.folio_abierto;
                                    sql.insertCotizacionOtros(constants.folio_abierto, otros.clave, otros.articulo, otros.proveedor, otros.linea, otros.caracteristicas, otros.color,
                                        (float)otros.cantidad, (float)otros.descuento, (float)otros.total, (float)otros.largo, (float)otros.alto, otros.descripcion);
                                }
                            }

                            foreach (var modulos in t_modulos)
                            {
                                if (modulos != null)
                                {
                                    modulos.folio = constants.folio_abierto;
                                    sql.insertCotizacionModulo(constants.folio_abierto, (int)modulos.modulo_id, modulos.descripcion, (float)modulos.mano_obra, modulos.dimensiones, modulos.acabado_perfil, modulos.claves_cristales,
                                    (int)modulos.cantidad, (int)modulos.largo, (int)modulos.alto, (float)modulos.flete, (float)modulos.desperdicio, (float)modulos.utilidad, modulos.claves_otros, modulos.claves_herrajes, modulos.ubicacion, modulos.claves_perfiles, modulos.pic, (int)modulos.merge_id, (int)modulos.concept_id, modulos.articulo, (float)Math.Round((float)modulos.total, 2), (int)modulos.sub_folio, (int)modulos.dir, modulos.news, modulos.new_desing, (int)modulos.orden);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(form, "[Error]: La cotización necesita ser ligada a un cliente para ser guardada.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            worker.CancelAsync();
                            constants.cotizacion_error = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show(form, "[Error]: La cotización no puede ser guardada en este momento, intenta de nuevo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        worker.CancelAsync();                       
                        constants.cotizacion_error = true;
                    }
                }
                else
                {
                    if (new_cotizacion == true)
                    {
                        if (sql.isFolioExist(folio) == false)
                        {
                            if (textbox.Text != "")
                            {
                                deleteAllDeletedArticulos();
                                sql.insertCotizacion(folio, textbox.Text, constants.user, DateTime.Today.ToString("dd/MM/yyyy") + " -", textbox2.Text, constants.desc_cotizacion, constants.utilidad_cotizacion, constants.org_name, constants.iva_desglosado, "Sin Registro", constants.tc);
                                constants.cotizacion_error = false;
                                constants.folio_abierto = folio;
                                constants.nombre_cotizacion = textbox.Text;
                                constants.nombre_proyecto = textbox2.Text;
                                constants.autor_cotizacion = constants.user;
                                constants.fecha_cotizacion = DateTime.Today.ToString("dd/MM/yyyy");

                                foreach (var cristales in t_cristales)
                                {
                                    if (cristales != null)
                                    {
                                        cristales.folio = constants.folio_abierto;
                                        sql.insertCotizacionCristal(constants.folio_abierto, cristales.clave, cristales.articulo, cristales.lista, (float)cristales.largo, (float)cristales.alto, cristales.canteado,
                                            cristales.biselado, cristales.desconchado, cristales.pecho_paloma, cristales.perforado_media_pulgada, cristales.perforado_una_pulgada,
                                            cristales.perforado_dos_pulgadas, cristales.grabado, cristales.esmerilado, cristales.tipo_venta, (float)cristales.cantidad, (float)cristales.descuento,
                                            (float)cristales.total, cristales.proveedor, (float)cristales.filo_muerto, cristales.descripcion);
                                    }
                                }

                                foreach (var aluminio in t_aluminios)
                                {
                                    if (aluminio != null)
                                    {
                                        aluminio.folio = constants.folio_abierto;
                                        sql.insertCotizacionAluminio(constants.folio_abierto, aluminio.clave, aluminio.articulo, aluminio.lista, aluminio.linea, aluminio.largo_total.ToString(), aluminio.acabado,
                                            (float)aluminio.cantidad, (float)aluminio.descuento, (float)aluminio.total, aluminio.proveedor, aluminio.descripcion);
                                    }
                                }

                                foreach (var herrajes in t_herrajes)
                                {
                                    if (herrajes != null)
                                    {
                                        herrajes.folio = constants.folio_abierto;
                                        sql.insertCotizacionHerrajes(constants.folio_abierto, herrajes.clave, herrajes.articulo, herrajes.proveedor, herrajes.linea, herrajes.caracteristicas, herrajes.color,
                                            (float)herrajes.cantidad, (float)herrajes.descuento, (float)herrajes.total, herrajes.descripcion);
                                    }
                                }

                                foreach (var otros in t_otros)
                                {
                                    if (otros != null)
                                    {
                                        otros.folio = constants.folio_abierto;
                                        sql.insertCotizacionOtros(constants.folio_abierto, otros.clave, otros.articulo, otros.proveedor, otros.linea, otros.caracteristicas, otros.color,
                                            (float)otros.cantidad, (float)otros.descuento, (float)otros.total, (float)otros.largo, (float)otros.alto, otros.descripcion);
                                    }
                                }

                                foreach (var modulos in t_modulos)
                                {
                                    if (modulos != null)
                                    {
                                        modulos.folio = constants.folio_abierto;
                                        sql.insertCotizacionModulo(constants.folio_abierto, (int)modulos.modulo_id, modulos.descripcion, (float)modulos.mano_obra, modulos.dimensiones, modulos.acabado_perfil, modulos.claves_cristales,
                                        (int)modulos.cantidad, (int)modulos.largo, (int)modulos.alto, (float)modulos.flete, (float)modulos.desperdicio, (float)modulos.utilidad, modulos.claves_otros, modulos.claves_herrajes, modulos.ubicacion, modulos.claves_perfiles, modulos.pic, (int)modulos.merge_id, (int)modulos.concept_id, modulos.articulo, (float)Math.Round((float)modulos.total, 2), (int)modulos.sub_folio, (int)modulos.dir, modulos.news, modulos.new_desing, (int)modulos.orden);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show(form, "[Error]: La cotización necesita ser ligada a un cliente para ser guardada.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                worker.CancelAsync();
                                constants.cotizacion_error = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show(form, "[Error]: La cotización no puede ser guardada en este momento, intenta de nuevo.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            worker.CancelAsync();
                            constants.cotizacion_error = true;
                        }
                    }
                    else
                    {
                        //update si se tiene el mismo folio
                        DialogResult r = MessageBox.Show(form, "Ya existe una operación con el mismo número de folio.\n\n ¿Desea sobreescribirla?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (r == DialogResult.Yes)
                        {
                            deleteAllDeletedArticulos();
                            sql.updateCotizacion(constants.folio_abierto, constants.getStringToPoint(sql.getSingleSQLValueById("cotizaciones", "fecha", "folio", constants.folio_abierto, 0)) + "- UM: " + DateTime.Today.ToString("dd/MM/yyyy"), textbox.Text, textbox2.Text, constants.desc_cotizacion, constants.utilidad_cotizacion, constants.iva_desglosado, constants.tc);
                            constants.nombre_cotizacion = textbox.Text;
                            constants.nombre_proyecto = textbox2.Text;

                            foreach (var cristales in t_cristales)
                            {
                                if (cristales != null)
                                {
                                    if (cristales.folio > 0)
                                    {
                                        sql.updateCotizacionCristal(constants.getIDFromClave(cristales.clave), constants.folio_abierto, constants.getClave(cristales.clave), cristales.articulo, cristales.lista, (float)cristales.largo, (float)cristales.alto, cristales.canteado,
                                            cristales.biselado, cristales.desconchado, cristales.pecho_paloma, cristales.perforado_media_pulgada, cristales.perforado_una_pulgada,
                                            cristales.perforado_dos_pulgadas, cristales.grabado, cristales.esmerilado, cristales.tipo_venta, (float)cristales.cantidad, (float)cristales.descuento,
                                            (float)cristales.total, cristales.proveedor, (float)cristales.filo_muerto, cristales.descripcion);
                                    }
                                    else
                                    {
                                        sql.insertCotizacionCristal(constants.folio_abierto, cristales.clave, cristales.articulo, cristales.lista, (float)cristales.largo, (float)cristales.alto, cristales.canteado,
                                        cristales.biselado, cristales.desconchado, cristales.pecho_paloma, cristales.perforado_media_pulgada, cristales.perforado_una_pulgada,
                                        cristales.perforado_dos_pulgadas, cristales.grabado, cristales.esmerilado, cristales.tipo_venta, (float)cristales.cantidad, (float)cristales.descuento,
                                        (float)cristales.total, cristales.proveedor, (float)cristales.filo_muerto, cristales.descripcion);
                                    }
                                }
                            }

                            foreach (var aluminio in t_aluminios)
                            {
                                if (aluminio != null)
                                {
                                    if (aluminio.folio > 0)
                                    {
                                        sql.updateCotizacionAluminio(constants.getIDFromClave(aluminio.clave), constants.folio_abierto, constants.getClave(aluminio.clave), aluminio.articulo, aluminio.lista, aluminio.linea, aluminio.largo_total.ToString(), aluminio.acabado,
                                            (float)aluminio.cantidad, (float)aluminio.descuento, (float)aluminio.total, aluminio.proveedor, aluminio.descripcion);
                                    }
                                    else
                                    {
                                        sql.insertCotizacionAluminio(constants.folio_abierto, aluminio.clave, aluminio.articulo, aluminio.lista, aluminio.linea, aluminio.largo_total.ToString(), aluminio.acabado,
                                        (float)aluminio.cantidad, (float)aluminio.descuento, (float)aluminio.total, aluminio.proveedor, aluminio.descripcion);
                                    }
                                }
                            }

                            foreach (var herrajes in t_herrajes)
                            {
                                if (herrajes != null)
                                {
                                    if (herrajes.folio > 0)
                                    {
                                        sql.updateCotizacionHerrajes(constants.getIDFromClave(herrajes.clave), constants.folio_abierto, constants.getClave(herrajes.clave), herrajes.articulo, herrajes.proveedor, herrajes.linea, herrajes.caracteristicas, herrajes.color,
                                            (float)herrajes.cantidad, (float)herrajes.descuento, (float)herrajes.total, herrajes.descripcion);
                                    }
                                    else
                                    {
                                        sql.insertCotizacionHerrajes(constants.folio_abierto, herrajes.clave, herrajes.articulo, herrajes.proveedor, herrajes.linea, herrajes.caracteristicas, herrajes.color,
                                        (float)herrajes.cantidad, (float)herrajes.descuento, (float)herrajes.total, herrajes.descripcion);
                                    }
                                }
                            }

                            foreach (var otros in t_otros)
                            {
                                if (otros != null)
                                {
                                    if (otros.folio > 0)
                                    {
                                        sql.updateCotizacionOtros(constants.getIDFromClave(otros.clave), constants.folio_abierto, constants.getClave(otros.clave), otros.articulo, otros.proveedor, otros.linea, otros.caracteristicas, otros.color,
                                            (float)otros.cantidad, (float)otros.descuento, (float)otros.total, (float)otros.largo, (float)otros.alto, otros.descripcion);
                                    }
                                    else
                                    {
                                        sql.insertCotizacionOtros(constants.folio_abierto, otros.clave, otros.articulo, otros.proveedor, otros.linea, otros.caracteristicas, otros.color,
                                        (float)otros.cantidad, (float)otros.descuento, (float)otros.total, (float)otros.largo, (float)otros.alto, otros.descripcion);
                                    }
                                }
                            }

                            foreach (var modulos in t_modulos)
                            {
                                if (modulos != null)
                                {
                                    if (modulos.folio > 0)
                                    {
                                        sql.updateCotizacionModulo(constants.getIDFromClave(modulos.clave), constants.folio_abierto, (int)modulos.modulo_id, modulos.descripcion, (float)modulos.mano_obra, modulos.dimensiones, modulos.acabado_perfil, modulos.claves_cristales,
                                        (int)modulos.cantidad, (int)modulos.largo, (int)modulos.alto, (float)modulos.flete, (float)modulos.desperdicio, (float)modulos.utilidad, modulos.claves_otros, modulos.claves_herrajes, modulos.ubicacion, modulos.claves_perfiles, modulos.pic, (int)modulos.merge_id, (int)modulos.concept_id, modulos.articulo, (float)Math.Round((float)modulos.total, 2), (int)modulos.sub_folio, (int)modulos.dir, modulos.news, modulos.new_desing, (int)modulos.orden);
                                    }
                                    else
                                    {
                                        sql.insertCotizacionModulo(constants.folio_abierto, (int)modulos.modulo_id, modulos.descripcion, (float)modulos.mano_obra, modulos.dimensiones, modulos.acabado_perfil, modulos.claves_cristales,
                                        (int)modulos.cantidad, (int)modulos.largo, (int)modulos.alto, (float)modulos.flete, (float)modulos.desperdicio, (float)modulos.utilidad, modulos.claves_otros, modulos.claves_herrajes, modulos.ubicacion, modulos.claves_perfiles, modulos.pic, (int)modulos.merge_id, (int)modulos.concept_id, modulos.articulo, (float)Math.Round((float)modulos.total, 2), (int)modulos.sub_folio, (int)modulos.dir, modulos.news, modulos.new_desing, (int)modulos.orden);
                                    }
                                }
                            }
                        }
                        else
                        {
                            DialogResult n = MessageBox.Show(form, "¿Desea guardar una nueva cotización a partir de estos datos?", constants.msg_box_caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (n == DialogResult.Yes)
                            {
                                ((guardar_cotizacion)Application.OpenForms["guardar_cotizacion"]).new_cotizacion = true;
                                textbox.Enabled = true;
                                textbox2.Enabled = true;
                                boton.Enabled = true;
                                label.Text = constants.setFolio().ToString();
                                textbox.Text = "";
                                textbox2.Text = "";
                                worker.CancelAsync();
                            }
                            else
                            {
                                worker.CancelAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                cotizaciones.Dispose();
            }           
        }

        ~cotizaciones()
        {

        }
    }
}
