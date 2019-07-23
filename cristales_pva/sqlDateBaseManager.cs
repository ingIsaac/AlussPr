using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using Microsoft.Reporting.WinForms;

namespace cristales_pva
{
    class sqlDateBaseManager
    {
        SqlConnection connection;

        public string getConnectionString()
        {
            return @"Data Source="+constants.server+";" +
                "Initial Catalog="+constants.data_base+";" +
                "User ID="+constants.server_user+";" +
                "Password="+constants.server_password+";" +
                "MultipleActiveResultSets=true;" +
                "Trusted_Connection=false;" +
                "Connection Timeout = 30;";
        }

        public ConnectionState getConnectionState()
        {
            return connection.State;
        }

        public Boolean setServerConnection()
        {
            if (constants.online == true)
            {
                connection = new SqlConnection();
                connection.ConnectionString = getConnectionString();
                try
                {
                    connection.Open();
                    connection.Close();
                    constants.online = true;
                    ((Form1)Application.OpenForms["form1"]).setToolStripStatus("Status: Conectado", true);
                    return true;
                }
                catch (SqlException err)
                {
                    constants.errorLog(err.ToString());
                    ((Form1)Application.OpenForms["form1"]).setToolStripStatus("Status: Desconectado", false);
                    if (err.Number == 18456)
                    {
                        MessageBox.Show("[Error] No se puede conectar al servidor [Login Error]. ", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("[Error] No se puede conectar al servidor. ", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return false;
                }
                finally
                {
                    connection.Dispose();
                }
            }
            else
            {
                MessageBox.Show("[Error] No se puede conectar al servidor. ", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }    
        }

        public Boolean checkConnection()
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            try
            {
                connection.Open();
                connection.Close();
                ((Form1)Application.OpenForms["form1"]).setToolStripStatus("Status: Conectado", true);
                constants.online = true;
                return true;
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
                ((Form1)Application.OpenForms["form1"]).setToolStripStatus("Status: Desconectado", false);
                constants.online = false;
                return false;
            }
            finally
            {
                connection.Dispose();
            }
        }

        public Boolean isUserAllowed(string user, string password)
        {
            Boolean b = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT id, acceso, restringir FROM usuarios WHERE usuario='" + user + "' AND password='"+ password +"'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    b = true;
                    //Get user access
                    if (r.GetValue(1) != null && r.GetValue(2) != null)
                    {
                        constants.user_access = (int)r.GetValue(1);
                        //Check user forbid
                        constants.user_forbid = (bool)r.GetValue(2);
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return b;
        }

        public Boolean isFolioExist(int folio)
        {
            Boolean b = true;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT TOP(1) * FROM cotizaciones WHERE folio='" + folio + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (!r.Read())
                {
                    b = false;
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return b;
        }

        public Boolean getLostDiseño(int id)
        {
            Boolean b = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM esquemas WHERE id='" + id + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    b = true;
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return b;
        }

        public int getLastRow(string table, string column)
        {
            int result = 0;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT TOP 1 "+ column + " FROM "+ table + " ORDER BY "+ column + " DESC";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result = (int)r.GetValue(0);
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public int getUserId(string user_name)
        {
            int result = 0;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT id FROM usuarios where usuario='" + user_name + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result = (int)r.GetValue(0);
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public List<string> getUsersList()
        {
            List<string> result = new List<string>();
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT usuario FROM usuarios";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while(r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result.Add(r.GetValue(0).ToString());
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public int getDiseñoID(string nombre)
        {
            int result = 0;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT id FROM esquemas where nombre='" + nombre + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result = (int)r.GetValue(0);
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public System.Drawing.Bitmap getEsquemaPic(int id_diseño)
        {
            System.Drawing.Bitmap bm = null;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT pic FROM esquemas_pics where id_diseño='" + id_diseño + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        Byte[] bytes = (Byte[])r.GetValue(0);
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes))
                        {
                            bm = new System.Drawing.Bitmap(ms);
                        };                       
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return bm;
        }

        public System.Drawing.Bitmap getColorImg(string clave)
        {
            System.Drawing.Bitmap bm = null;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT img FROM colores_imagenes WHERE clave='" + clave + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        Byte[] bytes = (Byte[])r.GetValue(0);
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes))
                        {
                            bm = new System.Drawing.Bitmap(ms);
                        };
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return bm;
        }

        public Boolean getCategoriaRep(string categoria, string grupo)
        {
            Boolean result = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM categorias WHERE categoria='" + categoria + "' AND grupo='" + grupo + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public Boolean getProveedorRep(string proveedor, string grupo)
        {
            Boolean result = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM proveedores WHERE proveedor='" + proveedor + "' AND grupo='" + grupo + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public Boolean findSQLValue(string column, string column_target, string table, string value_target)
        {
            Boolean result = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT "+column+" FROM "+table+" WHERE "+column_target+"='"+value_target+"'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if(r.Read())
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public Boolean getActivation()
        {
            Boolean activated = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT mac_pc_activada FROM pc_activadas WHERE mac_pc_activada='" + constants.mac_address + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    activated = true;
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return activated;
        }

        public Boolean deleteSQLValue(string table, string column_target, string value_target)
        {
            Boolean result = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM " + table + " WHERE " + column_target + "='" + value_target + "'";
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public List<string> findSQLValueList(string column, string column_target, string table, string value_target)
        {
            List<string> list = new List<string>();
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT " + column + " FROM " + table + " WHERE " + column_target + " LIKE '" + value_target + "%'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(r.GetValue(0).ToString());                   
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return list;
        }

        public List<string> selectColumnList(string column, string table)
        {
            List<string> list = new List<string>();
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT " + column + " FROM " + table;
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(r.GetValue(0).ToString());
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return list;
        }

        public Boolean findActivation(string column, string column_target, string table, string value_target)
        {
            Boolean result = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT " + column + " FROM " + table + " WHERE " + column_target + "='" + value_target + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("[Error] esta credencial de usuario no tiene permisos de activador.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(e.ToString());
                constants._false_activation = true;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public string getSingleSQLValue(string table, string column, string column_target, string value_target, int index_column)
        {
            string result = "";
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT " + column + " FROM " + table + " WHERE " + column_target + "='"+value_target+"'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while(r.Read())
                {
                    if(!r.IsDBNull(index_column))
                    {
                        result = r.GetValue(index_column).ToString();
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }            
            return result;
        }

        public string getSingleSQLValueById(string table, string column, string column_target, int id, int index_column)
        {
            string result = "";
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT " + column + " FROM " + table + " WHERE " + column_target + "='" + id + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(index_column))
                    {
                        result = r.GetValue(index_column).ToString();
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }      

        public void insertNewPCActivation(string mac, string serial, string pc_name)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO pc_activadas (mac_pc_activada, serial, pc_name) VALUES (@MAC, @SERIAL, @PC_NAME)";
            cmd.Parameters.AddWithValue("@MAC", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@MAC"].Value = mac;
            cmd.Parameters.AddWithValue("@SERIAL", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@SERIAL"].Value = serial;
            cmd.Parameters.AddWithValue("@PC_NAME", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PC_NAME"].Value = pc_name;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void insertNewSQLValue(string column, string table, string value)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO " + table + " ("+column+") VALUES (@VALUE)";
            cmd.Parameters.AddWithValue("@VALUE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@VALUE"].Value = value;

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void dropTableOnGridView(DataGridView dataview, string table, bool order=false, string column="", string value="", bool precise = false)
        {
            DataTable data = new DataTable();
            try
            {
                string query = string.Empty;

                if (order == false)
                {
                    query = "SELECT * FROM " + table;
                }
                else
                {
                    if (precise)
                    {
                        query = "SELECT * FROM " + table + " ORDER BY CASE WHEN " + column + "='" + value + "' THEN 1 END DESC";
                    }
                    else
                    {
                        query = "SELECT * FROM " + table + " ORDER BY CASE WHEN " + column + " LIKE '" + value + "%' THEN 1 END DESC";
                    }
                }

                SqlDataAdapter da = new SqlDataAdapter(query, getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(data);
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }          
        }

        public void dropTableOnGridViewWithFilter(DataGridView dataview, string table, string column, string value, bool accurate=false)
        {
            DataTable data = new DataTable();
            try
            {
                SqlDataAdapter da = null;
                if (accurate == false)
                {
                    da = new SqlDataAdapter("SELECT * FROM " + table + " WHERE " + column + " LIKE '" + value + "%'", getConnectionString());
                }
                else
                {
                    da = new SqlDataAdapter("SELECT * FROM " + table + " WHERE " + column + "='" + value + "'", getConnectionString());
                }
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(data);
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        public void getModuloFilterUser(DataGridView dataview, string table, string column, string value)
        {
            DataTable data = new DataTable();
            try
            {
                SqlDataAdapter da = null;
                da = new SqlDataAdapter("SELECT * FROM " + table + " WHERE " + column + "='" + value + "' AND user='" + constants.user + "'", getConnectionString());
                           
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(data);
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        public void updateRow(string table, string column, string new_value, string target_column, string target_value, Boolean date=false)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE "+table+" SET "+column+"=@NEWVALUE WHERE "+target_column+"=@TARGETVALUE";
            cmd.Parameters.AddWithValue("@TARGETVALUE", target_value);

            if (date == false)
            {
                cmd.Parameters.AddWithValue("@NEWVALUE", System.Data.SqlDbType.Float);
                try
                {
                    cmd.Parameters["@NEWVALUE"].Value = Math.Round(float.Parse(new_value), 2);
                }
                catch (Exception)
                {
                    cmd.Parameters["@NEWVALUE"].Value = 0;
                }            
            }
            else
            {
                cmd.Parameters.AddWithValue("@NEWVALUE", System.Data.SqlDbType.VarChar);
                cmd.Parameters["@NEWVALUE"].Value = new_value;
            }

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void insertListaCosto(string clave, string articulo, float precio_proveedor, float p_desperdicio, float p_mano_obra, float costo_corte, float p_utilidad, float precio_corte, string fecha, string proveedor, string moneda)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO costo_corte_precio (clave, articulo, precio_proveedor, p_desperdicio, p_mano_obra, costo_corte, p_utilidad, precio_corte, fecha, proveedor, moneda) VALUES (@CLAVE,@ARTICULO,@PP,@DESPER,@MO,@CC,@PU,@PC,@FECHA,@PROV,@MN)";

            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@PP", System.Data.SqlDbType.Float);
            cmd.Parameters["@PP"].Value = Math.Round(precio_proveedor, 2);
            cmd.Parameters.AddWithValue("@DESPER", System.Data.SqlDbType.Float);
            cmd.Parameters["@DESPER"].Value = Math.Round(p_desperdicio, 2);
            cmd.Parameters.AddWithValue("@MO", System.Data.SqlDbType.Float);
            cmd.Parameters["@MO"].Value = Math.Round(p_mano_obra, 2);
            cmd.Parameters.AddWithValue("@CC", System.Data.SqlDbType.Float);
            cmd.Parameters["@CC"].Value = Math.Round(costo_corte, 2);
            cmd.Parameters.AddWithValue("@PU", System.Data.SqlDbType.Float);
            cmd.Parameters["@PU"].Value = Math.Round(p_utilidad, 2);
            cmd.Parameters.AddWithValue("@PC", System.Data.SqlDbType.Float);
            cmd.Parameters["@PC"].Value = Math.Round(precio_corte, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@MN", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@MN"].Value = moneda;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateListaCosto(string clave, string articulo, float precio_proveedor, float p_desperdicio, float p_mano_obra, float costo_corte, float p_utilidad, float precio_corte, string fecha, string proveedor, string moneda)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE costo_corte_precio SET articulo=@ARTICULO, precio_proveedor=@PP, p_desperdicio=@DESPER, p_mano_obra=@MO, costo_corte=@CC, p_utilidad=@PU, precio_corte=@PC, fecha=@FECHA, proveedor=@PROV, moneda=@MN WHERE clave='" + clave + "'";
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@PP", System.Data.SqlDbType.Float);
            cmd.Parameters["@PP"].Value = Math.Round(precio_proveedor, 2);
            cmd.Parameters.AddWithValue("@DESPER", System.Data.SqlDbType.Float);
            cmd.Parameters["@DESPER"].Value = Math.Round(p_desperdicio, 2);
            cmd.Parameters.AddWithValue("@MO", System.Data.SqlDbType.Float);
            cmd.Parameters["@MO"].Value = Math.Round(p_mano_obra, 2);
            cmd.Parameters.AddWithValue("@CC", System.Data.SqlDbType.Float);
            cmd.Parameters["@CC"].Value = Math.Round(costo_corte, 2);
            cmd.Parameters.AddWithValue("@PU", System.Data.SqlDbType.Float);
            cmd.Parameters["@PU"].Value = Math.Round(p_utilidad, 2);
            cmd.Parameters.AddWithValue("@PC", System.Data.SqlDbType.Float);
            cmd.Parameters["@PC"].Value = Math.Round(precio_corte, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@MN", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@MN"].Value = moneda;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void insertListaInstalado(string clave, string articulo, float mano_obra, float materiales, float flete, float costo, float utilidad, float precio, string fecha, string proveedor, string moneda)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO instalado (clave, articulo, p_mano_obra_insta, p_costo_materiales, costo_flete, costo_instalado, p_utilidad_insta, precio_insta, fecha, proveedor, moneda) VALUES " 
                + "(@CLAVE,@ARTICULO,@MANO,@MATE,@FLETE,@COSTO,@UTI,@PRECIO,@FECHA,@PROV,@MN)";
            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@MANO", System.Data.SqlDbType.Float);
            cmd.Parameters["@MANO"].Value = Math.Round(mano_obra, 2);
            cmd.Parameters.AddWithValue("@MATE", System.Data.SqlDbType.Float);
            cmd.Parameters["@MATE"].Value = Math.Round(materiales, 2);
            cmd.Parameters.AddWithValue("@FLETE", System.Data.SqlDbType.Float);
            cmd.Parameters["@FLETE"].Value = Math.Round(flete, 2);
            cmd.Parameters.AddWithValue("@COSTO", System.Data.SqlDbType.Float);
            cmd.Parameters["@COSTO"].Value = Math.Round(costo, 2);
            cmd.Parameters.AddWithValue("@UTI", System.Data.SqlDbType.Float);
            cmd.Parameters["@UTI"].Value = Math.Round(utilidad, 2);
            cmd.Parameters.AddWithValue("@PRECIO", System.Data.SqlDbType.Float);
            cmd.Parameters["@PRECIO"].Value = Math.Round(precio, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@MN", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@MN"].Value = moneda;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateListaInstalado(string clave, string articulo, float mano_obra, float materiales, float flete, float costo, float utilidad, float precio, string fecha, string proveedor, string moneda)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE instalado SET articulo=@ARTICULO, p_mano_obra_insta=@MANO, p_costo_materiales=@MATE, costo_flete=@FLETE, costo_instalado=@COSTO, p_utilidad_insta=@UTI, precio_insta=@PRECIO, fecha=@FECHA, proveedor=@PROV, moneda=@MN WHERE clave='" + clave + "'";
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@MANO", System.Data.SqlDbType.Float);
            cmd.Parameters["@MANO"].Value = Math.Round(mano_obra, 2);
            cmd.Parameters.AddWithValue("@MATE", System.Data.SqlDbType.Float);
            cmd.Parameters["@MATE"].Value = Math.Round(materiales, 2);
            cmd.Parameters.AddWithValue("@FLETE", System.Data.SqlDbType.Float);
            cmd.Parameters["@FLETE"].Value = Math.Round(flete, 2);
            cmd.Parameters.AddWithValue("@COSTO", System.Data.SqlDbType.Float);
            cmd.Parameters["@COSTO"].Value = Math.Round(costo, 2);
            cmd.Parameters.AddWithValue("@UTI", System.Data.SqlDbType.Float);
            cmd.Parameters["@UTI"].Value = Math.Round(utilidad, 2);
            cmd.Parameters.AddWithValue("@PRECIO", System.Data.SqlDbType.Float);
            cmd.Parameters["@PRECIO"].Value = Math.Round(precio, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@MN", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@MN"].Value = moneda;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void insertListaHojas(string clave, string articulo, float largo, float ancho, float costo_hoja, float utilidad, float precio, string fecha, string proveedor, string moneda)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO hojas (clave, articulo, largo, ancho, costo_hoja, p_utilidad_hoja, precio_hoja, fecha, proveedor, moneda) VALUES " 
                + "(@CLAVE,@ARTICULO,@LARGO,@ANCHO,@COSTO,@UTILIDAD,@PRECIO,@FECHA,@PROV,@MN)";
            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LARGO", System.Data.SqlDbType.Float);
            cmd.Parameters["@LARGO"].Value = Math.Round(largo, 2);
            cmd.Parameters.AddWithValue("@ANCHO", System.Data.SqlDbType.Float);
            cmd.Parameters["@ANCHO"].Value = Math.Round(ancho, 2);
            cmd.Parameters.AddWithValue("@COSTO", System.Data.SqlDbType.Float);
            cmd.Parameters["@COSTO"].Value = Math.Round(costo_hoja, 2);
            cmd.Parameters.AddWithValue("@UTILIDAD", System.Data.SqlDbType.Float);
            cmd.Parameters["@UTILIDAD"].Value = Math.Round(utilidad, 2);
            cmd.Parameters.AddWithValue("@PRECIO", System.Data.SqlDbType.Float);
            cmd.Parameters["@PRECIO"].Value = Math.Round(precio, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@MN", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@MN"].Value = moneda;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateListaHojas(string clave, string articulo, float largo, float ancho, float costo_hoja, float utilidad, float precio, string fecha, string proveedor, string moneda)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE hojas SET articulo=@ARTICULO, largo=@LARGO, ancho=@ANCHO, costo_hoja=@COSTO, p_utilidad_hoja=@UTILIDAD, precio_hoja=@PRECIO, fecha=@FECHA, proveedor=@PROV, moneda=@MN WHERE clave='" + clave + "'";
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LARGO", System.Data.SqlDbType.Float);
            cmd.Parameters["@LARGO"].Value = Math.Round(largo, 2);
            cmd.Parameters.AddWithValue("@ANCHO", System.Data.SqlDbType.Float);
            cmd.Parameters["@ANCHO"].Value = Math.Round(ancho, 2);
            cmd.Parameters.AddWithValue("@COSTO", System.Data.SqlDbType.Float);
            cmd.Parameters["@COSTO"].Value = Math.Round(costo_hoja, 2);
            cmd.Parameters.AddWithValue("@UTILIDAD", System.Data.SqlDbType.Float);
            cmd.Parameters["@UTILIDAD"].Value = Math.Round(utilidad, 2);
            cmd.Parameters.AddWithValue("@PRECIO", System.Data.SqlDbType.Float);
            cmd.Parameters["@PRECIO"].Value = Math.Round(precio, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@MN", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@MN"].Value = moneda;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void insertListaAcabados(string clave, string acabado, float neto_recto, float neto_curvo, string fecha)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO acabados (clave, acabado, neto_recto, neto_curvo, fecha) VALUES (@CLAVE,@ACABADO,@RECTO,@CURVO,@FECHA)";
            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ACABADO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ACABADO"].Value = acabado;
            cmd.Parameters.AddWithValue("@RECTO", System.Data.SqlDbType.Float);
            cmd.Parameters["@RECTO"].Value = Math.Round(neto_recto, 2);
            cmd.Parameters.AddWithValue("@CURVO", System.Data.SqlDbType.Float);
            cmd.Parameters["@CURVO"].Value = Math.Round(neto_curvo, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateListaAcabados(string clave, string acabado, float neto_recto, float neto_curvo, string fecha)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE acabados SET acabado=@ACABADO, neto_recto=@RECTO, neto_curvo=@CURVO, fecha=@FECHA WHERE clave='" + clave + "'";
            cmd.Parameters.AddWithValue("@ACABADO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ACABADO"].Value = acabado;
            cmd.Parameters.AddWithValue("@RECTO", System.Data.SqlDbType.Float);
            cmd.Parameters["@RECTO"].Value = Math.Round(neto_recto, 2);
            cmd.Parameters.AddWithValue("@CURVO", System.Data.SqlDbType.Float);
            cmd.Parameters["@CURVO"].Value = Math.Round(neto_curvo, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void insertListaAluminio(string clave, string articulo, string linea, string proveedor, float largo, float ancho, float per_a, float crudo, float blanco, float hueso, float champagne, float gris, float negro, float brillante, float natural, float madera, float peso, string fecha, float chocolate, float acero_inox, float bronce, float peso_teorico, string moneda)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO perfiles (clave, articulo, linea, proveedor, largo, ancho_perfil, perimetro_dm2_ml, crudo, blanco, hueso, champagne, gris, negro, brillante, natural_1, madera, kg_peso_lineal, fecha, chocolate, acero_inox, bronce, moneda, peso_teorico) VALUES "
                + "(@CLAVE,@ARTICULO,@LINEA,@PROV,@LARGO,@ANCHO,@PERA,@CRUDO,@BLANCO,@HUESO,@CHAMP,@GRIS,@NEGRO,@BRI,@NAT,@MAD,@PESO,@FECHA,@CHOCO,@ANOX,@BRONCE,@MN,@PT)";            
            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@LARGO", System.Data.SqlDbType.Float);
            cmd.Parameters["@LARGO"].Value = Math.Round(largo, 2);
            cmd.Parameters.AddWithValue("@ANCHO", System.Data.SqlDbType.Float);
            cmd.Parameters["@ANCHO"].Value = Math.Round(ancho, 2);
            cmd.Parameters.AddWithValue("@PERA", System.Data.SqlDbType.Float);
            cmd.Parameters["@PERA"].Value = Math.Round(per_a, 2);            
            cmd.Parameters.AddWithValue("@CRUDO", System.Data.SqlDbType.Float);
            cmd.Parameters["@CRUDO"].Value = Math.Round(crudo, 2);
            cmd.Parameters.AddWithValue("@BLANCO", System.Data.SqlDbType.Float);
            cmd.Parameters["@BLANCO"].Value = Math.Round(blanco, 2);
            cmd.Parameters.AddWithValue("@HUESO", System.Data.SqlDbType.Float);
            cmd.Parameters["@HUESO"].Value = Math.Round(hueso, 2);
            cmd.Parameters.AddWithValue("@CHAMP", System.Data.SqlDbType.Float);
            cmd.Parameters["@CHAMP"].Value = Math.Round(champagne, 2);
            cmd.Parameters.AddWithValue("@GRIS", System.Data.SqlDbType.Float);
            cmd.Parameters["@GRIS"].Value = Math.Round(gris, 2);
            cmd.Parameters.AddWithValue("@NEGRO", System.Data.SqlDbType.Float);
            cmd.Parameters["@NEGRO"].Value = Math.Round(negro, 2);
            cmd.Parameters.AddWithValue("@BRI", System.Data.SqlDbType.Float);
            cmd.Parameters["@BRI"].Value = Math.Round(brillante, 2);
            cmd.Parameters.AddWithValue("@NAT", System.Data.SqlDbType.Float);
            cmd.Parameters["@NAT"].Value = Math.Round(natural, 2);
            cmd.Parameters.AddWithValue("@MAD", System.Data.SqlDbType.Float);
            cmd.Parameters["@MAD"].Value = Math.Round(madera, 2);
            cmd.Parameters.AddWithValue("@PESO", System.Data.SqlDbType.Float);
            cmd.Parameters["@PESO"].Value = Math.Round(peso, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@CHOCO", System.Data.SqlDbType.Float);
            cmd.Parameters["@CHOCO"].Value = Math.Round(chocolate, 2);
            cmd.Parameters.AddWithValue("@ANOX", System.Data.SqlDbType.Float);
            cmd.Parameters["@ANOX"].Value = Math.Round(acero_inox, 2);
            cmd.Parameters.AddWithValue("@BRONCE", System.Data.SqlDbType.Float);
            cmd.Parameters["@BRONCE"].Value = Math.Round(bronce, 2);
            cmd.Parameters.AddWithValue("@MN", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@MN"].Value = moneda;
            cmd.Parameters.AddWithValue("@PT", System.Data.SqlDbType.Float);
            cmd.Parameters["@PT"].Value = Math.Round(peso_teorico, 3);
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateListaAluminio(int id, string articulo, string linea, string proveedor, float largo, float ancho, float per_a, float crudo, float blanco, float hueso, float champagne, float gris, float negro, float brillante, float natural, float madera, float peso, string fecha, float chocolate, float acero_inox, float bronce, float peso_teorico, string moneda)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE perfiles SET articulo=@ARTICULO, linea=@LINEA, proveedor=@PROV, largo=@LARGO, ancho_perfil=@ANCHO, perimetro_dm2_ml=@PERA, crudo=@CRUDO, blanco=@BLANCO, hueso=@HUESO, champagne=@CHAMP, gris=@GRIS, negro=@NEGRO, brillante=@BRI, natural_1=@NAT, madera=@MAD, kg_peso_lineal=@PESO, fecha=@FECHA, chocolate=@CHOCO, acero_inox=@ANOX, bronce=@BRONCE, moneda=@MN, peso_teorico=@PT WHERE id='" + id + "'";
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@LARGO", System.Data.SqlDbType.Float);
            cmd.Parameters["@LARGO"].Value = Math.Round(largo, 2);
            cmd.Parameters.AddWithValue("@ANCHO", System.Data.SqlDbType.Float);
            cmd.Parameters["@ANCHO"].Value = Math.Round(ancho, 2);
            cmd.Parameters.AddWithValue("@PERA", System.Data.SqlDbType.Float);
            cmd.Parameters["@PERA"].Value = Math.Round(per_a, 2);           
            cmd.Parameters.AddWithValue("@CRUDO", System.Data.SqlDbType.Float);
            cmd.Parameters["@CRUDO"].Value = Math.Round(crudo, 2);
            cmd.Parameters.AddWithValue("@BLANCO", System.Data.SqlDbType.Float);
            cmd.Parameters["@BLANCO"].Value = Math.Round(blanco, 2);
            cmd.Parameters.AddWithValue("@HUESO", System.Data.SqlDbType.Float);
            cmd.Parameters["@HUESO"].Value = Math.Round(hueso, 2);
            cmd.Parameters.AddWithValue("@CHAMP", System.Data.SqlDbType.Float);
            cmd.Parameters["@CHAMP"].Value = Math.Round(champagne, 2);
            cmd.Parameters.AddWithValue("@GRIS", System.Data.SqlDbType.Float);
            cmd.Parameters["@GRIS"].Value = Math.Round(gris, 2);
            cmd.Parameters.AddWithValue("@NEGRO", System.Data.SqlDbType.Float);
            cmd.Parameters["@NEGRO"].Value = Math.Round(negro, 2);
            cmd.Parameters.AddWithValue("@BRI", System.Data.SqlDbType.Float);
            cmd.Parameters["@BRI"].Value = Math.Round(brillante, 2);
            cmd.Parameters.AddWithValue("@NAT", System.Data.SqlDbType.Float);
            cmd.Parameters["@NAT"].Value = Math.Round(natural, 2);
            cmd.Parameters.AddWithValue("@MAD", System.Data.SqlDbType.Float);
            cmd.Parameters["@MAD"].Value = Math.Round(madera, 2);
            cmd.Parameters.AddWithValue("@PESO", System.Data.SqlDbType.Float);
            cmd.Parameters["@PESO"].Value = Math.Round(peso, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@CHOCO", System.Data.SqlDbType.Float);
            cmd.Parameters["@CHOCO"].Value = Math.Round(chocolate, 2);
            cmd.Parameters.AddWithValue("@ANOX", System.Data.SqlDbType.Float);
            cmd.Parameters["@ANOX"].Value = Math.Round(acero_inox, 2);
            cmd.Parameters.AddWithValue("@BRONCE", System.Data.SqlDbType.Float);
            cmd.Parameters["@BRONCE"].Value = Math.Round(bronce, 2);
            cmd.Parameters.AddWithValue("@MN", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@MN"].Value = moneda;
            cmd.Parameters.AddWithValue("@PT", System.Data.SqlDbType.Float);
            cmd.Parameters["@PT"].Value = Math.Round(peso_teorico, 3);
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void insertListaHerrajes(string clave, string articulo, string proveedor, string linea, string caracteristicas, string color, float precio, string fecha, string moneda)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO herrajes (clave, articulo, proveedor, linea, caracteristicas, color, precio, fecha, moneda) VALUES "
                + "(@CLAVE,@ARTICULO,@PROV,@LINEA,@CARAC,@COLOR,@PRECIO,@FECHA,@MN)";
            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@CARAC", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CARAC"].Value = caracteristicas;
            cmd.Parameters.AddWithValue("@COLOR", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@COLOR"].Value = color;
            cmd.Parameters.AddWithValue("@PRECIO", System.Data.SqlDbType.Float);
            cmd.Parameters["@PRECIO"].Value = Math.Round(precio, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@MN", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@MN"].Value = moneda;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateListaHerrajes(int id, string articulo, string proveedor, string linea, string caracteristicas, string color, float precio, string fecha, string moneda)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE herrajes SET articulo=@ARTICULO, proveedor=@PROV, linea=@LINEA, caracteristicas=@CARAC, color=@COLOR, precio=@PRECIO, fecha=@FECHA, moneda=@MN WHERE id='" + id + "'";
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@CARAC", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CARAC"].Value = caracteristicas;
            cmd.Parameters.AddWithValue("@COLOR", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@COLOR"].Value = color;
            cmd.Parameters.AddWithValue("@PRECIO", System.Data.SqlDbType.Float);
            cmd.Parameters["@PRECIO"].Value = Math.Round(precio, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@MN", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@MN"].Value = moneda;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void insertListaOtros(string clave, string articulo, string proveedor, string linea, string caracteristicas, string color, float precio, string fecha, float largo, float alto, string moneda)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO otros (clave, articulo, proveedor, linea, caracteristicas, color, largo, alto, precio, fecha, moneda) VALUES " +
                "(@CLAVE,@ARTICULO,@PROV,@LINEA,@CARAC,@COLOR,@LARGO,@ALTO,@PRECIO,@FECHA,@MN)";
            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@CARAC", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CARAC"].Value = caracteristicas;
            cmd.Parameters.AddWithValue("@COLOR", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@COLOR"].Value = color;
            cmd.Parameters.AddWithValue("@LARGO", System.Data.SqlDbType.Float);
            cmd.Parameters["@LARGO"].Value = Math.Round(largo, 2);
            cmd.Parameters.AddWithValue("@ALTO", System.Data.SqlDbType.Float);
            cmd.Parameters["@ALTO"].Value = Math.Round(alto, 2);
            cmd.Parameters.AddWithValue("@PRECIO", System.Data.SqlDbType.Float);
            cmd.Parameters["@PRECIO"].Value = Math.Round(precio, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@MN", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@MN"].Value = moneda;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateListaOtros(int id, string articulo, string proveedor, string linea, string caracteristicas, string color, float precio, string fecha, float largo, float alto, string moneda)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE otros SET articulo=@ARTICULO, proveedor=@PROV, linea=@LINEA, caracteristicas=@CARAC, color=@COLOR, largo=@LARGO, alto=@ALTO, precio=@PRECIO, fecha=@FECHA, moneda=@MN WHERE id=" + id;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@CARAC", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CARAC"].Value = caracteristicas;
            cmd.Parameters.AddWithValue("@COLOR", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@COLOR"].Value = color;
            cmd.Parameters.AddWithValue("@LARGO", System.Data.SqlDbType.Float);
            cmd.Parameters["@LARGO"].Value = Math.Round(largo, 2);
            cmd.Parameters.AddWithValue("@ALTO", System.Data.SqlDbType.Float);
            cmd.Parameters["@ALTO"].Value = Math.Round(alto, 2);
            cmd.Parameters.AddWithValue("@PRECIO", System.Data.SqlDbType.Float);
            cmd.Parameters["@PRECIO"].Value = Math.Round(precio, 2);
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@MN", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@MN"].Value = moneda;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void insertListaColores(string clave, string color, string proveedor, float precio, float costo_extra, string fecha)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO colores_aluminio (clave, color, proveedor, precio_m2, costo_extra_ml, fecha) VALUES (@CLAVE,@COLOR,@PROV,@PRECIO,@CE,@FECHA)";
            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@COLOR", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@COLOR"].Value = color;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@PRECIO", System.Data.SqlDbType.Float);
            cmd.Parameters["@PRECIO"].Value = Math.Round(precio, 2);
            cmd.Parameters.AddWithValue("@CE", System.Data.SqlDbType.Float);
            cmd.Parameters["@CE"].Value = Math.Round(costo_extra, 2);          
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void insertColoresImagenes(string clave, System.Drawing.Bitmap img)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO colores_imagenes (clave, img) values (@CLAVE,@IMG)";
            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@IMG", System.Data.SqlDbType.VarBinary);
            cmd.Parameters["@IMG"].Value = constants.imageToByte(img);           
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateColoresImagenes(string clave, System.Drawing.Bitmap img)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "UPDATE colores_imagenes SET img=@IMG WHERE clave='" + clave + "'";            
            cmd.Parameters.AddWithValue("@IMG", System.Data.SqlDbType.VarBinary);
            cmd.Parameters["@IMG"].Value = constants.imageToByte(img);
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateListaColores(int id, string color, string proveedor, float precio, float costo_extra, string fecha)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();                    

            cmd.Connection = connection;
            cmd.CommandText = "UPDATE colores_aluminio SET color=@COLOR, proveedor=@PROV, precio_m2=@PRECIO, costo_extra_ml=@CE, fecha=@FECHA WHERE id=" + id;
            cmd.Parameters.AddWithValue("@COLOR", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@COLOR"].Value = color;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@PRECIO", System.Data.SqlDbType.Float);
            cmd.Parameters["@PRECIO"].Value = Math.Round(precio, 2);
            cmd.Parameters.AddWithValue("@CE", System.Data.SqlDbType.Float);
            cmd.Parameters["@CE"].Value = Math.Round(costo_extra, 2);          
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public DataTable fillDataTable(string table)
        {
            string query = "SELECT * FROM dstut.dbo.[" + table + "]";

            using (SqlConnection sqlConn = new SqlConnection(getConnectionString()))
            using (SqlCommand cmd = new SqlCommand(query, sqlConn))
            {
                sqlConn.Open();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
        }

        //actualizacion de historial
        public void insertHistory(int id, string clave, string articulo, string linea, string lista, string fecha)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO historial_actualizacion (id, clave, articulo, linea, lista, fecha) VALUES (@ID, @CLAVE, @ARTICULO, @LINEA, @LISTA, @FECHA)";
            cmd.Parameters.AddWithValue("@ID", SqlDbType.Int);
            cmd.Parameters["@ID"].Value = id;
            cmd.Parameters.AddWithValue("@CLAVE", SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LINEA", SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@LISTA", SqlDbType.VarChar);
            cmd.Parameters["@LISTA"].Value = lista;
            cmd.Parameters.AddWithValue("@FECHA", SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }

        }

        public void deleteHistory()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "TRUNCATE TABLE historial_actualizacion";
            
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }

        }
        //ends historial de actualizacion

        //actualizacion de login
        public void insertHistoryLogin(int id, string id_usuario, string nombre_pc, string ip, string fecha)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO historial_login (id, id_usuario, nombre_pc, ip, fecha) VALUES (@ID, @USUARIO, @PCNAME, @IP, @FECHA)";
            cmd.Parameters.AddWithValue("@ID", SqlDbType.Int);
            cmd.Parameters["@ID"].Value = id;
            cmd.Parameters.AddWithValue("@USUARIO", SqlDbType.VarChar);
            cmd.Parameters["@USUARIO"].Value = id_usuario;
            cmd.Parameters.AddWithValue("@PCNAME", SqlDbType.VarChar);
            cmd.Parameters["@PCNAME"].Value = nombre_pc;
            cmd.Parameters.AddWithValue("@IP", SqlDbType.VarChar);
            cmd.Parameters["@IP"].Value = ip;
            cmd.Parameters.AddWithValue("@FECHA", SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
        }

        public void deleteHistoryLogin()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "DELETE TOP (1) FROM historial_login";

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }

        }
        //ends historial de login

        //SQL Data Table tools
        public int countSQLRows(string table)
        {
            int c = 0;
            SqlConnection connection = new SqlConnection(getConnectionString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT COUNT(*) FROM " + table;

            try
            {
                connection.Open();
                c = (int)cmd.ExecuteScalar();
            }catch(Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
            return c;
        }

        public void truncateSQLTable(string table)
        {
            SqlConnection connection = new SqlConnection(getConnectionString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM " + table;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
        }

        public DataTable createDataTableFromSQLTable(string table)
        {
            DataTable dt = new DataTable();
            try {                               
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM " + table, getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);               
                da.Fill(dt);               
            }catch(Exception e)
            {
                constants.errorLog(e.ToString());
            }
            return dt;
        }

        public DataTable createDataTableFromSQLTableWithFilter(string table, string column, string value)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM " + table + " WHERE " + column + " LIKE '" + value + "%'", getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(dt);
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            return dt;
        }

        public string selectLastFromTable(string table, string column)
        {
            string last = "";
            SqlConnection connection = new SqlConnection(getConnectionString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT "+column+" FROM "+table+" ORDER BY id ASC";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        last = r.GetValue(0).ToString();
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
            return last;
        }
        //ends SQL Data Table tools

        //COTIZACIONES       
        public void insertCotizacionCristal(int folio, string clave, string articulo, string lista, float largo, float alto,
            string canteado, string biselado, string desconchado, string pecho_p, string per_media, string per_una, string per_dos,
            string grabado, string esmerilado, string t_venta, float cant, float desc, float total, string proveedor, float filo_muerto, string descripcion)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO cristales_cotizados (folio, clave, articulo, lista, largo, alto, canteado, biselado, desconchado, pecho_paloma," +
               "perforado_media_pulgada, perforado_una_pulgada, perforado_dos_pulgadas, grabado, esmerilado, tipo_venta, cantidad, descuento, total, proveedor, filo_muerto, descripcion) "+
               " VALUES (@FOLIO, @CLAVE, @ARTICULO, @LISTA, @LARGO, @ALTO, @CANDO, @BISE, @DESCON, @PP, @PERMEDIA, @PERUNA, @PERDOS, @GRB, @ESME, @VENTA, @CANT, @DESC, @TOT, @PROV, @FM, @DESCR)";
            cmd.Parameters.AddWithValue("@FOLIO", SqlDbType.Int);
            cmd.Parameters["@FOLIO"].Value = folio;
            cmd.Parameters.AddWithValue("@CLAVE", SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LISTA", SqlDbType.VarChar);
            cmd.Parameters["@LISTA"].Value = lista;
            cmd.Parameters.AddWithValue("@LARGO", SqlDbType.Float);
            cmd.Parameters["@LARGO"].Value = largo;
            cmd.Parameters.AddWithValue("@ALTO", SqlDbType.Float);
            cmd.Parameters["@ALTO"].Value = alto;
            cmd.Parameters.AddWithValue("@CANDO", SqlDbType.VarChar);
            cmd.Parameters["@CANDO"].Value = canteado;
            cmd.Parameters.AddWithValue("@BISE", SqlDbType.VarChar);
            cmd.Parameters["@BISE"].Value = biselado;
            cmd.Parameters.AddWithValue("@DESCON", SqlDbType.VarChar);
            cmd.Parameters["@DESCON"].Value = desconchado;
            cmd.Parameters.AddWithValue("@PP", SqlDbType.VarChar);
            cmd.Parameters["@PP"].Value = pecho_p;
            cmd.Parameters.AddWithValue("@PERMEDIA", SqlDbType.VarChar);
            cmd.Parameters["@PERMEDIA"].Value = per_media;
            cmd.Parameters.AddWithValue("@PERUNA", SqlDbType.VarChar);
            cmd.Parameters["@PERUNA"].Value = per_una;
            cmd.Parameters.AddWithValue("@PERDOS", SqlDbType.VarChar);
            cmd.Parameters["@PERDOS"].Value = per_dos;
            cmd.Parameters.AddWithValue("@GRB", SqlDbType.VarChar);
            cmd.Parameters["@GRB"].Value = grabado;
            cmd.Parameters.AddWithValue("@ESME", SqlDbType.VarChar);
            cmd.Parameters["@ESME"].Value = esmerilado;
            cmd.Parameters.AddWithValue("@VENTA", SqlDbType.VarChar);
            cmd.Parameters["@VENTA"].Value = t_venta;
            cmd.Parameters.AddWithValue("@CANT", SqlDbType.Float);
            cmd.Parameters["@CANT"].Value = cant;
            cmd.Parameters.AddWithValue("@DESC", SqlDbType.Float);
            cmd.Parameters["@DESC"].Value = desc;
            cmd.Parameters.AddWithValue("@TOT", SqlDbType.Float);
            cmd.Parameters["@TOT"].Value = total;
            cmd.Parameters.AddWithValue("@PROV", SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@FM", SqlDbType.Float);
            cmd.Parameters["@FM"].Value = filo_muerto;
            cmd.Parameters.AddWithValue("@DESCR", SqlDbType.VarChar);
            cmd.Parameters["@DESCR"].Value = descripcion;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
        }

        public void updateCotizacionCristal(int id, int folio, string clave, string articulo, string lista, float largo, float alto,
            string canteado, string biselado, string desconchado, string pecho_p, string per_media, string per_una, string per_dos,
            string grabado, string esmerilado, string t_venta, float cant, float desc, float total, string proveedor, float filo_muerto, string descripcion)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE cristales_cotizados SET folio=@FOLIO, clave=@CLAVE, articulo=@ARTICULO, lista=@LISTA, largo= @LARGO, alto=@ALTO, canteado=@CANDO, biselado=@BISE, desconchado=@DESCON, pecho_paloma=@PP," +
               "perforado_media_pulgada=@PERMEDIA, perforado_una_pulgada=@PERUNA, perforado_dos_pulgadas=@PERDOS, grabado=@GRB, esmerilado=@ESME, tipo_venta=@VENTA, cantidad=@CANT, descuento=@DESC, total=@TOT, proveedor=@PROV, filo_muerto=@FM, descripcion=@DESCR" +
               " WHERE id='" + id + "'";
            cmd.Parameters.AddWithValue("@FOLIO", SqlDbType.Int);
            cmd.Parameters["@FOLIO"].Value = folio;
            cmd.Parameters.AddWithValue("@CLAVE", SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LISTA", SqlDbType.VarChar);
            cmd.Parameters["@LISTA"].Value = lista;
            cmd.Parameters.AddWithValue("@LARGO", SqlDbType.Float);
            cmd.Parameters["@LARGO"].Value = largo;
            cmd.Parameters.AddWithValue("@ALTO", SqlDbType.Float);
            cmd.Parameters["@ALTO"].Value = alto;
            cmd.Parameters.AddWithValue("@CANDO", SqlDbType.VarChar);
            cmd.Parameters["@CANDO"].Value = canteado;
            cmd.Parameters.AddWithValue("@BISE", SqlDbType.VarChar);
            cmd.Parameters["@BISE"].Value = biselado;
            cmd.Parameters.AddWithValue("@DESCON", SqlDbType.VarChar);
            cmd.Parameters["@DESCON"].Value = desconchado;
            cmd.Parameters.AddWithValue("@PP", SqlDbType.VarChar);
            cmd.Parameters["@PP"].Value = pecho_p;
            cmd.Parameters.AddWithValue("@PERMEDIA", SqlDbType.VarChar);
            cmd.Parameters["@PERMEDIA"].Value = per_media;
            cmd.Parameters.AddWithValue("@PERUNA", SqlDbType.VarChar);
            cmd.Parameters["@PERUNA"].Value = per_una;
            cmd.Parameters.AddWithValue("@PERDOS", SqlDbType.VarChar);
            cmd.Parameters["@PERDOS"].Value = per_dos;
            cmd.Parameters.AddWithValue("@GRB", SqlDbType.VarChar);
            cmd.Parameters["@GRB"].Value = grabado;
            cmd.Parameters.AddWithValue("@ESME", SqlDbType.VarChar);
            cmd.Parameters["@ESME"].Value = esmerilado;
            cmd.Parameters.AddWithValue("@VENTA", SqlDbType.VarChar);
            cmd.Parameters["@VENTA"].Value = t_venta;
            cmd.Parameters.AddWithValue("@CANT", SqlDbType.Float);
            cmd.Parameters["@CANT"].Value = cant;
            cmd.Parameters.AddWithValue("@DESC", SqlDbType.Float);
            cmd.Parameters["@DESC"].Value = desc;
            cmd.Parameters.AddWithValue("@TOT", SqlDbType.Float);
            cmd.Parameters["@TOT"].Value = total;
            cmd.Parameters.AddWithValue("@PROV", SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@FM", SqlDbType.Float);
            cmd.Parameters["@FM"].Value = filo_muerto;
            cmd.Parameters.AddWithValue("@DESCR", SqlDbType.VarChar);
            cmd.Parameters["@DESCR"].Value = descripcion;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
        }

        public void insertCotizacionAluminio(int folio, string clave, string articulo, string lista, string linea, string largo_total, string acabado, float cantidad, float desc, float total, string proveedor, string descripcion)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO aluminio_cotizado (folio, clave, articulo, lista, linea, largo_total, acabado, cantidad, descuento, total, proveedor, descripcion)"+
                "VALUES (@FOLIO, @CLAVE, @ARTICULO, @LISTA, @LINEA, @LARGO, @ACABADO, @CANT, @DESC, @TOT, @PROV, @DESCR)";
            cmd.Parameters.AddWithValue("@FOLIO", SqlDbType.Int);
            cmd.Parameters["@FOLIO"].Value = folio;
            cmd.Parameters.AddWithValue("@CLAVE", SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LISTA", SqlDbType.VarChar);
            cmd.Parameters["@LISTA"].Value = lista;
            cmd.Parameters.AddWithValue("@LINEA", SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@LARGO", SqlDbType.VarChar);
            cmd.Parameters["@LARGO"].Value = largo_total;
            cmd.Parameters.AddWithValue("@ACABADO", SqlDbType.VarChar);
            cmd.Parameters["@ACABADO"].Value = acabado;
            cmd.Parameters.AddWithValue("@CANT", SqlDbType.Float);
            cmd.Parameters["@CANT"].Value = cantidad;
            cmd.Parameters.AddWithValue("@DESC", SqlDbType.Float);
            cmd.Parameters["@DESC"].Value = desc;
            cmd.Parameters.AddWithValue("@TOT", SqlDbType.Float);
            cmd.Parameters["@TOT"].Value = total;
            cmd.Parameters.AddWithValue("@PROV", SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@DESCR", SqlDbType.VarChar);
            cmd.Parameters["@DESCR"].Value = descripcion;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
        }


        public void updateCotizacionAluminio(int id, int folio, string clave, string articulo, string lista, string linea, string largo_total, string acabado, float cantidad, float desc, float total, string proveedor, string descripcion)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE aluminio_cotizado SET folio=@FOLIO, clave=@CLAVE, articulo=@ARTICULO, lista=@LISTA, linea=@LINEA, largo_total=@LARGO, acabado=@ACABADO, cantidad=@CANT, descuento=@DESC, total=@TOT, proveedor=@PROV, descripcion=@DESCR" +
                " WHERE id='" + id + "'";
            cmd.Parameters.AddWithValue("@FOLIO", SqlDbType.Int);
            cmd.Parameters["@FOLIO"].Value = folio;
            cmd.Parameters.AddWithValue("@CLAVE", SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LISTA", SqlDbType.VarChar);
            cmd.Parameters["@LISTA"].Value = lista;
            cmd.Parameters.AddWithValue("@LINEA", SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@LARGO", SqlDbType.VarChar);
            cmd.Parameters["@LARGO"].Value = largo_total;
            cmd.Parameters.AddWithValue("@ACABADO", SqlDbType.VarChar);
            cmd.Parameters["@ACABADO"].Value = acabado;
            cmd.Parameters.AddWithValue("@CANT", SqlDbType.Float);
            cmd.Parameters["@CANT"].Value = cantidad;
            cmd.Parameters.AddWithValue("@DESC", SqlDbType.Float);
            cmd.Parameters["@DESC"].Value = desc;
            cmd.Parameters.AddWithValue("@TOT", SqlDbType.Float);
            cmd.Parameters["@TOT"].Value = total;
            cmd.Parameters.AddWithValue("@PROV", SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@DESCR", SqlDbType.VarChar);
            cmd.Parameters["@DESCR"].Value = descripcion;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
        }


        public void insertCotizacionHerrajes(int folio, string clave, string articulo, string proveedor, string linea, string caracteristicas, string color, float cantidad, float desc, float total, string descripcion)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO herrajes_cotizados (folio, clave, articulo, proveedor, linea, caracteristicas, color, cantidad, descuento, total, descripcion)" +
                "VALUES (@FOLIO, @CLAVE, @ARTICULO, @PROVEEDOR, @LINEA, @CARACTERISTICAS, @COLOR, @CANT, @DESC, @TOT, @DESCR)";
            cmd.Parameters.AddWithValue("@FOLIO", SqlDbType.Int);
            cmd.Parameters["@FOLIO"].Value = folio;
            cmd.Parameters.AddWithValue("@CLAVE", SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@PROVEEDOR", SqlDbType.VarChar);
            cmd.Parameters["@PROVEEDOR"].Value = proveedor;
            cmd.Parameters.AddWithValue("@LINEA", SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@CARACTERISTICAS", SqlDbType.VarChar);
            cmd.Parameters["@CARACTERISTICAS"].Value = caracteristicas;
            cmd.Parameters.AddWithValue("@COLOR", SqlDbType.VarChar);
            cmd.Parameters["@COLOR"].Value = color;
            cmd.Parameters.AddWithValue("@CANT", SqlDbType.Float);
            cmd.Parameters["@CANT"].Value = cantidad;
            cmd.Parameters.AddWithValue("@DESC", SqlDbType.Float);
            cmd.Parameters["@DESC"].Value = desc;
            cmd.Parameters.AddWithValue("@TOT", SqlDbType.Float);
            cmd.Parameters["@TOT"].Value = total;
            cmd.Parameters.AddWithValue("@DESCR", SqlDbType.VarChar);
            cmd.Parameters["@DESCR"].Value = descripcion;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
        }

        public void updateCotizacionHerrajes(int id, int folio, string clave, string articulo, string proveedor, string linea, string caracteristicas, string color, float cantidad, float desc, float total, string descripcion)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE herrajes_cotizados SET folio=@FOLIO, clave=@CLAVE, articulo=@ARTICULO, proveedor=@PROVEEDOR, linea=@LINEA, caracteristicas=@CARACTERISTICAS, color=@COLOR, cantidad=@CANT, descuento=@DESC, total=@TOT, descripcion=@DESCR" +
                " WHERE id='" + id + "'";
            cmd.Parameters.AddWithValue("@FOLIO", SqlDbType.Int);
            cmd.Parameters["@FOLIO"].Value = folio;
            cmd.Parameters.AddWithValue("@CLAVE", SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@PROVEEDOR", SqlDbType.VarChar);
            cmd.Parameters["@PROVEEDOR"].Value = proveedor;
            cmd.Parameters.AddWithValue("@LINEA", SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@CARACTERISTICAS", SqlDbType.VarChar);
            cmd.Parameters["@CARACTERISTICAS"].Value = caracteristicas;
            cmd.Parameters.AddWithValue("@COLOR", SqlDbType.VarChar);
            cmd.Parameters["@COLOR"].Value = color;
            cmd.Parameters.AddWithValue("@CANT", SqlDbType.Float);
            cmd.Parameters["@CANT"].Value = cantidad;
            cmd.Parameters.AddWithValue("@DESC", SqlDbType.Float);
            cmd.Parameters["@DESC"].Value = desc;
            cmd.Parameters.AddWithValue("@TOT", SqlDbType.Float);
            cmd.Parameters["@TOT"].Value = total;
            cmd.Parameters.AddWithValue("@DESCR", SqlDbType.VarChar);
            cmd.Parameters["@DESCR"].Value = descripcion;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
        }


        public void insertCotizacionOtros(int folio, string clave, string articulo, string proveedor, string linea, string caracteristicas, string color, float cantidad, float desc, float total, float largo, float alto, string descripcion)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO otros_cotizaciones (folio, clave, articulo, proveedor, linea, caracteristicas, color, cantidad, largo, alto, descuento, total, descripcion)" +
                "VALUES (@FOLIO, @CLAVE, @ARTICULO, @PROVEEDOR, @LINEA, @CARACTERISTICAS, @COLOR, @CANT, @LARGO, @ALTO, @DESC, @TOT, @DESCR)";
            cmd.Parameters.AddWithValue("@FOLIO", SqlDbType.Int);
            cmd.Parameters["@FOLIO"].Value = folio;
            cmd.Parameters.AddWithValue("@CLAVE", SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@PROVEEDOR", SqlDbType.VarChar);
            cmd.Parameters["@PROVEEDOR"].Value = proveedor;
            cmd.Parameters.AddWithValue("@LINEA", SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@CARACTERISTICAS", SqlDbType.VarChar);
            cmd.Parameters["@CARACTERISTICAS"].Value = caracteristicas;
            cmd.Parameters.AddWithValue("@COLOR", SqlDbType.VarChar);
            cmd.Parameters["@COLOR"].Value = color;
            cmd.Parameters.AddWithValue("@CANT", SqlDbType.Float);
            cmd.Parameters["@CANT"].Value = cantidad;
            cmd.Parameters.AddWithValue("@DESC", SqlDbType.Float);
            cmd.Parameters["@DESC"].Value = desc;
            cmd.Parameters.AddWithValue("@TOT", SqlDbType.Float);
            cmd.Parameters["@TOT"].Value = total;
            cmd.Parameters.AddWithValue("@LARGO", SqlDbType.Float);
            cmd.Parameters["@LARGO"].Value = largo;
            cmd.Parameters.AddWithValue("@ALTO", SqlDbType.Float);
            cmd.Parameters["@ALTO"].Value = alto;
            cmd.Parameters.AddWithValue("@DESCR", SqlDbType.VarChar);
            cmd.Parameters["@DESCR"].Value = descripcion;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
        }

        public void updateCotizacionOtros(int id, int folio, string clave, string articulo, string proveedor, string linea, string caracteristicas, string color, float cantidad, float desc, float total, float largo, float alto, string descripcion)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE otros_cotizaciones SET folio=@FOLIO, clave=@CLAVE, articulo=@ARTICULO, proveedor=@PROVEEDOR, linea=@LINEA, caracteristicas=@CARACTERISTICAS, color=@COLOR, cantidad=@CANT, largo=@LARGO, alto=@ALTO, descuento=@DESC, total=@TOT, descripcion=@DESCR" +
                " WHERE id='"+id+"'";
            cmd.Parameters.AddWithValue("@FOLIO", SqlDbType.Int);
            cmd.Parameters["@FOLIO"].Value = folio;
            cmd.Parameters.AddWithValue("@CLAVE", SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@PROVEEDOR", SqlDbType.VarChar);
            cmd.Parameters["@PROVEEDOR"].Value = proveedor;
            cmd.Parameters.AddWithValue("@LINEA", SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@CARACTERISTICAS", SqlDbType.VarChar);
            cmd.Parameters["@CARACTERISTICAS"].Value = caracteristicas;
            cmd.Parameters.AddWithValue("@COLOR", SqlDbType.VarChar);
            cmd.Parameters["@COLOR"].Value = color;
            cmd.Parameters.AddWithValue("@CANT", SqlDbType.Float);
            cmd.Parameters["@CANT"].Value = cantidad;
            cmd.Parameters.AddWithValue("@DESC", SqlDbType.Float);
            cmd.Parameters["@DESC"].Value = desc;
            cmd.Parameters.AddWithValue("@TOT", SqlDbType.Float);
            cmd.Parameters["@TOT"].Value = total;
            cmd.Parameters.AddWithValue("@LARGO", SqlDbType.Float);
            cmd.Parameters["@LARGO"].Value = largo;
            cmd.Parameters.AddWithValue("@ALTO", SqlDbType.Float);
            cmd.Parameters["@ALTO"].Value = alto;
            cmd.Parameters.AddWithValue("@DESCR", SqlDbType.VarChar);
            cmd.Parameters["@DESCR"].Value = descripcion;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
        }


        public void insertCotizacionModulo(int folio, int modulo_id, string descripcion, float mano_obra, string dimensiones, string acabado_perfil, string claves_cristales, int cantidad, int largo, int alto, float flete, float desperdicio, float utilidad, string claves_otros, string claves_herrajes, string ubicacion, string claves_perfiles, byte[] cs, int marge_id, int concept_id, string articulo, float total, int sub_folio, int dir, string news, string new_desing, int orden)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO modulos_cotizaciones (folio, modulo_id, descripcion, mano_obra, dimensiones, acabado_perfil, claves_cristales, cantidad, largo, alto, flete, desperdicio, utilidad, claves_otros, claves_herrajes, ubicacion, claves_perfiles, cs, marge_id, concept_id, articulo, total, sub_folio, dir, news, new_desing, orden)" +
                "VALUES (@FOLIO, @ID, @DESC, @MO, @DIMEN, @AP, @CC, @CANT, @LARGO, @ALTO, @FLETE, @DESP, @UTI, @CLVOTR, @CLVH, @UB, @CLVP, @CS, @MID, @CID, @NA, @TOT, @SFOLIO, @DIR, @NEWS, @NEWD, @ORDEN)";
            cmd.Parameters.AddWithValue("@FOLIO", SqlDbType.Int);
            cmd.Parameters["@FOLIO"].Value = folio;
            cmd.Parameters.AddWithValue("@ID", SqlDbType.Int);
            cmd.Parameters["@ID"].Value = modulo_id;
            cmd.Parameters.AddWithValue("@DESC", SqlDbType.VarChar);
            cmd.Parameters["@DESC"].Value = descripcion;
            cmd.Parameters.AddWithValue("@MO", SqlDbType.Float);
            cmd.Parameters["@MO"].Value = mano_obra;
            cmd.Parameters.AddWithValue("@DIMEN", SqlDbType.VarChar);
            cmd.Parameters["@DIMEN"].Value = dimensiones;
            cmd.Parameters.AddWithValue("@AP", SqlDbType.VarChar);
            cmd.Parameters["@AP"].Value = acabado_perfil;
            cmd.Parameters.AddWithValue("@CC", SqlDbType.VarChar);
            cmd.Parameters["@CC"].Value = claves_cristales;           
            cmd.Parameters.AddWithValue("@CANT", SqlDbType.Int);
            cmd.Parameters["@CANT"].Value = cantidad;
            cmd.Parameters.AddWithValue("@LARGO", SqlDbType.Int);
            cmd.Parameters["@LARGO"].Value = largo;
            cmd.Parameters.AddWithValue("@ALTO", SqlDbType.Int);
            cmd.Parameters["@ALTO"].Value = alto;
            cmd.Parameters.AddWithValue("@FLETE", SqlDbType.Float);
            cmd.Parameters["@FLETE"].Value = flete;
            cmd.Parameters.AddWithValue("@DESP", SqlDbType.Float);
            cmd.Parameters["@DESP"].Value = desperdicio;
            cmd.Parameters.AddWithValue("@UTI", SqlDbType.Float);
            cmd.Parameters["@UTI"].Value = utilidad;
            cmd.Parameters.AddWithValue("@CLVOTR", SqlDbType.VarChar);
            cmd.Parameters["@CLVOTR"].Value = claves_otros;
            cmd.Parameters.AddWithValue("@CLVH", SqlDbType.VarChar);
            cmd.Parameters["@CLVH"].Value = claves_herrajes;
            cmd.Parameters.AddWithValue("@UB", SqlDbType.VarChar);
            cmd.Parameters["@UB"].Value = ubicacion;
            cmd.Parameters.AddWithValue("@CLVP", SqlDbType.VarChar);
            cmd.Parameters["@CLVP"].Value = claves_perfiles;
            cmd.Parameters.AddWithValue("@CS", SqlDbType.VarBinary);
            cmd.Parameters["@CS"].Value = cs;
            cmd.Parameters.AddWithValue("@MID", SqlDbType.Int);
            cmd.Parameters["@MID"].Value = marge_id;
            cmd.Parameters.AddWithValue("@CID", SqlDbType.Int);
            cmd.Parameters["@CID"].Value = concept_id;
            cmd.Parameters.AddWithValue("@NA", SqlDbType.VarChar);
            cmd.Parameters["@NA"].Value = articulo;
            cmd.Parameters.AddWithValue("@TOT", SqlDbType.Float);
            cmd.Parameters["@TOT"].Value = total;
            cmd.Parameters.AddWithValue("@SFOLIO", SqlDbType.Int);
            cmd.Parameters["@SFOLIO"].Value = sub_folio;
            cmd.Parameters.AddWithValue("@DIR", SqlDbType.Int);
            cmd.Parameters["@DIR"].Value = dir;
            cmd.Parameters.AddWithValue("@NEWS", SqlDbType.VarChar);
            cmd.Parameters["@NEWS"].Value = news;
            cmd.Parameters.AddWithValue("@NEWD", SqlDbType.VarChar);
            cmd.Parameters["@NEWD"].Value = new_desing;
            cmd.Parameters.AddWithValue("@ORDEN", SqlDbType.Int);
            cmd.Parameters["@ORDEN"].Value = orden;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
        }

        public void updateCotizacionModulo(int id, int folio, int modulo_id, string descripcion, float mano_obra, string dimensiones, string acabado_perfil, string claves_cristales, int cantidad, int largo, int alto, float flete, float desperdicio, float utilidad, string claves_otros, string claves_herrajes, string ubicacion, string claves_perfiles, byte[] cs, int marge_id, int concept_id, string articulo, float total, int sub_folio, int dir, string news, string new_desing, int orden)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE modulos_cotizaciones SET folio=@FOLIO, modulo_id=@ID_M, descripcion=@DESC, mano_obra=@MO, dimensiones=@DIMEN, acabado_perfil=@AP, claves_cristales=@CC, cantidad=@CANT, largo=@LARGO, alto=@ALTO, flete=@FLETE, desperdicio=@DESP, utilidad=@UTI, claves_otros=@CLVOTR, claves_herrajes=@CLVH, ubicacion=@UB, claves_perfiles=@CLVP, cs=@CS, marge_id=@MID, concept_id=@CID, articulo=@NA, total=@TOT, sub_folio=@SFOLIO, dir=@DIR, news=@NEWS, new_desing=@NEWD, orden=@ORDEN" +
                " WHERE id='" + id + "'";
            cmd.Parameters.AddWithValue("@FOLIO", SqlDbType.Int);
            cmd.Parameters["@FOLIO"].Value = folio;
            cmd.Parameters.AddWithValue("@ID_M", SqlDbType.Int);
            cmd.Parameters["@ID_M"].Value = modulo_id;
            cmd.Parameters.AddWithValue("@DESC", SqlDbType.VarChar);
            cmd.Parameters["@DESC"].Value = descripcion;
            cmd.Parameters.AddWithValue("@MO", SqlDbType.Float);
            cmd.Parameters["@MO"].Value = mano_obra;
            cmd.Parameters.AddWithValue("@DIMEN", SqlDbType.VarChar);
            cmd.Parameters["@DIMEN"].Value = dimensiones;
            cmd.Parameters.AddWithValue("@AP", SqlDbType.VarChar);
            cmd.Parameters["@AP"].Value = acabado_perfil;
            cmd.Parameters.AddWithValue("@CC", SqlDbType.VarChar);
            cmd.Parameters["@CC"].Value = claves_cristales;           
            cmd.Parameters.AddWithValue("@CANT", SqlDbType.Int);
            cmd.Parameters["@CANT"].Value = cantidad;
            cmd.Parameters.AddWithValue("@LARGO", SqlDbType.Int);
            cmd.Parameters["@LARGO"].Value = largo;
            cmd.Parameters.AddWithValue("@ALTO", SqlDbType.Int);
            cmd.Parameters["@ALTO"].Value = alto;
            cmd.Parameters.AddWithValue("@FLETE", SqlDbType.Float);
            cmd.Parameters["@FLETE"].Value = flete;
            cmd.Parameters.AddWithValue("@DESP", SqlDbType.Float);
            cmd.Parameters["@DESP"].Value = desperdicio;
            cmd.Parameters.AddWithValue("@UTI", SqlDbType.Float);
            cmd.Parameters["@UTI"].Value = utilidad;
            cmd.Parameters.AddWithValue("@CLVOTR", SqlDbType.VarChar);
            cmd.Parameters["@CLVOTR"].Value = claves_otros;
            cmd.Parameters.AddWithValue("@CLVH", SqlDbType.VarChar);
            cmd.Parameters["@CLVH"].Value = claves_herrajes;
            cmd.Parameters.AddWithValue("@UB", SqlDbType.VarChar);
            cmd.Parameters["@UB"].Value = ubicacion;
            cmd.Parameters.AddWithValue("@CLVP", SqlDbType.VarChar);
            cmd.Parameters["@CLVP"].Value = claves_perfiles;
            cmd.Parameters.AddWithValue("@CS", SqlDbType.VarBinary);
            cmd.Parameters["@CS"].Value = cs;
            cmd.Parameters.AddWithValue("@MID", SqlDbType.Int);
            cmd.Parameters["@MID"].Value = marge_id;
            cmd.Parameters.AddWithValue("@CID", SqlDbType.Int);
            cmd.Parameters["@CID"].Value = concept_id;
            cmd.Parameters.AddWithValue("@NA", SqlDbType.VarChar);
            cmd.Parameters["@NA"].Value = articulo;
            cmd.Parameters.AddWithValue("@TOT", SqlDbType.Float);
            cmd.Parameters["@TOT"].Value = total;
            cmd.Parameters.AddWithValue("@SFOLIO", SqlDbType.Int);
            cmd.Parameters["@SFOLIO"].Value = sub_folio;
            cmd.Parameters.AddWithValue("@DIR", SqlDbType.Int);
            cmd.Parameters["@DIR"].Value = dir;
            cmd.Parameters.AddWithValue("@NEWS", SqlDbType.VarChar);
            cmd.Parameters["@NEWS"].Value = news;
            cmd.Parameters.AddWithValue("@NEWD", SqlDbType.VarChar);
            cmd.Parameters["@NEWD"].Value = new_desing;
            cmd.Parameters.AddWithValue("@ORDEN", SqlDbType.Int);
            cmd.Parameters["@ORDEN"].Value = orden;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
        }

        public void insertCotizacion(int folio, string cliente, string usuario, string fecha, string nombre_proyecto, float descuento, float utilidad, string tienda, bool iva_desglosado, string registro, float tc, string subfolio_titles, string precio_especial)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO cotizaciones (folio, cliente, fecha, usuario, nombre_proyecto, descuento, utilidad, tienda, iva_desglosado, registro, tc, subfolio_titles, precio_especial) VALUES (@FOLIO, @CLIENTE, @FECHA, @USUARIO, @PROYECTO, @DESC, @UTIL, @TIENDA, @IVA, @REG, @TC, @SFOL, @PESP)";
            cmd.Parameters.AddWithValue("@FOLIO", SqlDbType.Int);
            cmd.Parameters["@FOLIO"].Value = folio;
            cmd.Parameters.AddWithValue("@CLIENTE", SqlDbType.VarChar);
            cmd.Parameters["@CLIENTE"].Value = cliente;
            cmd.Parameters.AddWithValue("@FECHA", SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@USUARIO", SqlDbType.VarChar);
            cmd.Parameters["@USUARIO"].Value = usuario;
            cmd.Parameters.AddWithValue("@PROYECTO", SqlDbType.VarChar);
            cmd.Parameters["@PROYECTO"].Value = nombre_proyecto;
            cmd.Parameters.AddWithValue("@DESC", SqlDbType.Float);
            cmd.Parameters["@DESC"].Value = descuento;
            cmd.Parameters.AddWithValue("@UTIL", SqlDbType.Float);
            cmd.Parameters["@UTIL"].Value = utilidad;
            cmd.Parameters.AddWithValue("@TIENDA", SqlDbType.VarChar);
            cmd.Parameters["@TIENDA"].Value = tienda;
            cmd.Parameters.AddWithValue("@IVA", SqlDbType.Bit);
            cmd.Parameters["@IVA"].Value = iva_desglosado;
            cmd.Parameters.AddWithValue("@REG", SqlDbType.VarChar);
            cmd.Parameters["@REG"].Value = registro;
            cmd.Parameters.AddWithValue("@TC", SqlDbType.Float);
            cmd.Parameters["@TC"].Value = tc;
            cmd.Parameters.AddWithValue("@SFOL", SqlDbType.VarChar);
            cmd.Parameters["@SFOL"].Value = subfolio_titles;
            cmd.Parameters.AddWithValue("@PESP", SqlDbType.VarChar);
            cmd.Parameters["@PESP"].Value = precio_especial;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }

        }

        public void updateCotizacion(int folio, string fecha, string cliente, string proyecto, float descuento, float utilidad, bool iva_desglosado, float tc, string subfolio_titles, string precio_especial)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE cotizaciones SET fecha=@FECHA, cliente=@CLIENTE, nombre_proyecto=@PROYECTO, descuento=@DESC, utilidad=@UTIL, iva_desglosado=@IVA, tc=@TC, subfolio_titles=@SFOL, precio_especial=@PESP WHERE folio='" + folio + "'";
            cmd.Parameters.AddWithValue("@FECHA", SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@CLIENTE", SqlDbType.VarChar);
            cmd.Parameters["@CLIENTE"].Value = cliente;
            cmd.Parameters.AddWithValue("@PROYECTO", SqlDbType.VarChar);
            cmd.Parameters["@PROYECTO"].Value = proyecto;
            cmd.Parameters.AddWithValue("@DESC", SqlDbType.Float);
            cmd.Parameters["@DESC"].Value = descuento;
            cmd.Parameters.AddWithValue("@UTIL", SqlDbType.Float);
            cmd.Parameters["@UTIL"].Value = utilidad;
            cmd.Parameters.AddWithValue("@IVA", SqlDbType.Bit);
            cmd.Parameters["@IVA"].Value = iva_desglosado;
            cmd.Parameters.AddWithValue("@TC", SqlDbType.Float);
            cmd.Parameters["@TC"].Value = tc;
            cmd.Parameters.AddWithValue("@SFOL", SqlDbType.VarChar);
            cmd.Parameters["@SFOL"].Value = subfolio_titles;
            cmd.Parameters.AddWithValue("@PESP", SqlDbType.VarChar);
            cmd.Parameters["@PESP"].Value = precio_especial;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }

        }

        public void updateCotizacionRegistro(int folio, string registro)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE cotizaciones SET registro=@REG WHERE folio='" + folio + "'";
            cmd.Parameters.AddWithValue("@REG", SqlDbType.VarChar);
            cmd.Parameters["@REG"].Value = registro;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }

        }

        public void updateCotizacionUsuario(int folio, string usuario, string tienda)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE cotizaciones SET usuario=@USR, tienda=@TIENDA WHERE folio='" + folio + "'";
            cmd.Parameters.AddWithValue("@USR", SqlDbType.VarChar);
            cmd.Parameters["@USR"].Value = usuario;
            cmd.Parameters.AddWithValue("@TIENDA", SqlDbType.VarChar);
            cmd.Parameters["@TIENDA"].Value = tienda;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }

        }

        public int countCotizaciones(string org, Boolean getALL = false, string filtro = "", Boolean activar_filtro = false)
        {
            int c = 0;
            SqlConnection connection = new SqlConnection(getConnectionString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            if (getALL == true)
            {
                if (activar_filtro == true)
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM cotizaciones WHERE tienda ='" + org + "' AND (cliente LIKE '" + filtro + "%' OR folio LIKE '%" + constants.stringToInt(filtro) + "' OR nombre_proyecto LIKE '" + filtro + "%')";
                }
                else
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM cotizaciones WHERE tienda ='" + org + "'";
                }
            }
            else
            {
                if (activar_filtro == true)
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM cotizaciones WHERE usuario ='" + constants.user + "' AND tienda ='" + org + "' AND (cliente LIKE '" + filtro + "%' OR folio LIKE '%" + constants.stringToInt(filtro) + "' OR nombre_proyecto LIKE '" + filtro + "%')";
                }
                else
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM cotizaciones WHERE usuario ='" + constants.user + "' AND tienda ='" + org + "'";
                }
            }
            try
            {
                connection.Open();
                c = (int)cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
            return c;
        }

        public void dropTableCotizacionesOnGridView(DataGridView dataview, string org, Boolean getALL = false, string filtro="", Boolean activar_filtro=false, int offset = 0, int div = 0)
        {
            DataTable data = new DataTable();
            try
            {
                if (getALL == true)
                {
                    if (activar_filtro == true)
                    {
                        SqlDataAdapter da = new SqlDataAdapter("SELECT id, folio, cliente, fecha, usuario, nombre_proyecto, registro FROM cotizaciones WHERE tienda ='" + org + "' AND (cliente LIKE '" + filtro + "%' OR folio LIKE '%" + constants.stringToInt(filtro) + "' OR nombre_proyecto LIKE '" + filtro + "%') ORDER BY id OFFSET " + offset + " ROWS FETCH NEXT " + div + " ROWS ONLY", getConnectionString());
                        SqlCommandBuilder cb = new SqlCommandBuilder(da);
                        da.Fill(data);
                    }
                    else
                    {
                        SqlDataAdapter da = new SqlDataAdapter("SELECT id, folio, cliente, fecha, usuario, nombre_proyecto, registro FROM cotizaciones WHERE tienda ='" + org + "' ORDER BY id OFFSET " + offset + " ROWS FETCH NEXT " + div + " ROWS ONLY", getConnectionString());
                        SqlCommandBuilder cb = new SqlCommandBuilder(da);
                        da.Fill(data);
                    }
                }
                else
                {
                    if (activar_filtro == true)
                    {
                        SqlDataAdapter da = new SqlDataAdapter("SELECT id, folio, cliente, fecha, usuario, nombre_proyecto, registro FROM cotizaciones WHERE usuario ='" + constants.user + "' AND tienda ='" + org + "' AND (cliente LIKE '" + filtro + "%' OR folio LIKE '%" + constants.stringToInt(filtro) + "' OR nombre_proyecto LIKE '" + filtro + "%') ORDER BY id OFFSET " + offset + " ROWS FETCH NEXT " + div + " ROWS ONLY", getConnectionString());
                        SqlCommandBuilder cb = new SqlCommandBuilder(da);
                        da.Fill(data);
                    }
                    else
                    {
                        SqlDataAdapter da = new SqlDataAdapter("SELECT id, folio, cliente, fecha, usuario, nombre_proyecto, registro FROM cotizaciones WHERE usuario ='" + constants.user + "' AND tienda ='" + org + "' ORDER BY id OFFSET " + offset + " ROWS FETCH NEXT " + div + " ROWS ONLY", getConnectionString());
                        SqlCommandBuilder cb = new SqlCommandBuilder(da);
                        da.Fill(data);
                    }
                }
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        public int countCotizacionesFecha(string date, string org, Boolean getALL = false)
        {
            int c = 0;
            SqlConnection connection = new SqlConnection(getConnectionString());
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            if (getALL == true)
            {
                cmd.CommandText = "SELECT COUNT(*) FROM cotizaciones WHERE tienda = '" + org + "' AND fecha LIKE '%" + date + "%'";
            }
            else
            {
                cmd.CommandText = "SELECT COUNT(*) FROM cotizaciones WHERE usuario ='" + constants.user + "' AND tienda ='" + org + "' AND fecha LIKE '%" + date + "%'";
            }
            try
            {
                connection.Open();
                c = (int)cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
            return c;
        }       

        public void dropTableCotizacionesOnGridViewFromDate(DataGridView dataview, string date, string org, Boolean getALL = false, int offset = 0, int div = 0)
        {
            DataTable data = new DataTable();
            try
            {
                if (getALL == true)
                {                 
                    SqlDataAdapter da = new SqlDataAdapter("SELECT id, folio, cliente, fecha, usuario, nombre_proyecto, registro FROM cotizaciones WHERE tienda = '" + org + "' AND fecha LIKE '%" + date + "%' ORDER BY id OFFSET " + offset + " ROWS FETCH NEXT " + div + " ROWS ONLY", getConnectionString());
                    SqlCommandBuilder cb = new SqlCommandBuilder(da);
                    da.Fill(data);                  
                }
                else
                {               
                    SqlDataAdapter da = new SqlDataAdapter("SELECT id, folio, cliente, fecha, usuario, nombre_proyecto, registro FROM cotizaciones WHERE usuario ='" + constants.user + "' AND tienda ='" + org + "' AND fecha LIKE '%" + date + "%' ORDER BY id OFFSET " + offset + " ROWS FETCH NEXT " + div + " ROWS ONLY", getConnectionString());
                    SqlCommandBuilder cb = new SqlCommandBuilder(da);
                    da.Fill(data);                                    
                }
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        private string getClave(string clave)
        {
            string[] r = clave.Split('-');
            return r[0];     
        }

        public void dropTableArticulosCotizacionesToLocal(string table, int folio)
        {
            DataTable data = new DataTable();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM " + table + " WHERE  folio = '" + folio + "'", getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(data);

                cotizaciones_local cotizaciones = new cotizaciones_local();

                if (table == "cristales_cotizados")
                {                 
                    for(int i = 0; i < data.Rows.Count; i++)
                    {
                        cristales_cotizados cristales = new cristales_cotizados()
                        {
                            folio = (int)data.Rows[i][1],
                            clave = getClave(data.Rows[i][2].ToString()) + "-" + data.Rows[i][0].ToString(),
                            articulo = data.Rows[i][3].ToString(),
                            lista = data.Rows[i][4].ToString(),
                            largo = Math.Round((double)data.Rows[i][5], 2),
                            alto = Math.Round((double)data.Rows[i][6], 2),
                            canteado = data.Rows[i][7].ToString(),
                            biselado = data.Rows[i][8].ToString(),
                            desconchado = data.Rows[i][9].ToString(),
                            pecho_paloma = data.Rows[i][10].ToString(),
                            perforado_media_pulgada = data.Rows[i][11].ToString(),
                            perforado_una_pulgada = data.Rows[i][12].ToString(),
                            perforado_dos_pulgadas = data.Rows[i][13].ToString(),
                            grabado = data.Rows[i][14].ToString(),
                            esmerilado = data.Rows[i][15].ToString(),
                            tipo_venta = data.Rows[i][16].ToString(),
                            cantidad = Math.Round((double)data.Rows[i][17], 2),
                            descuento = Math.Round((double)data.Rows[i][18], 2),
                            total = Math.Round((double)data.Rows[i][19], 2),
                            proveedor = data.Rows[i][20].ToString(),
                            filo_muerto = Math.Round((double)data.Rows[i][21], 2),
                            descripcion = data.Rows[i][22].ToString()
                        };
                        cotizaciones.cristales_cotizados.Add(cristales);
                    }
                    cotizaciones.SaveChanges();
                    data.Dispose();
                }
                else if(table == "aluminio_cotizado")
                {
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        aluminio_cotizado aluminio = new aluminio_cotizado()
                        {
                            folio = (int)data.Rows[i][1],
                            clave = getClave(data.Rows[i][2].ToString()) + "-" + data.Rows[i][0].ToString(),
                            articulo = data.Rows[i][3].ToString(),
                            lista = data.Rows[i][4].ToString(),
                            linea = data.Rows[i][5].ToString(),
                            largo_total = Math.Round(double.Parse(data.Rows[i][6].ToString()), 2),
                            acabado = data.Rows[i][7].ToString(),
                            cantidad = Math.Round((double)data.Rows[i][8], 2),
                            descuento = Math.Round((double)data.Rows[i][9], 2),
                            total = Math.Round((double)data.Rows[i][10], 2),
                            proveedor = data.Rows[i][11].ToString(),
                            descripcion = data.Rows[i][12].ToString()
                        };
                        cotizaciones.aluminio_cotizado.Add(aluminio);
                    }
                    cotizaciones.SaveChanges();
                    data.Dispose();
                }
                else if (table == "herrajes_cotizados")
                {
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        herrajes_cotizados herrajes = new herrajes_cotizados()
                        {
                            folio = (int)data.Rows[i][1],
                            clave = getClave(data.Rows[i][2].ToString()) + "-" + data.Rows[i][0].ToString(),
                            articulo = data.Rows[i][3].ToString(),
                            proveedor = data.Rows[i][4].ToString(),
                            linea = data.Rows[i][5].ToString(),
                            caracteristicas = data.Rows[i][6].ToString(),
                            color = data.Rows[i][7].ToString(),
                            cantidad = Math.Round((double)data.Rows[i][8], 2),
                            descuento = Math.Round((double)data.Rows[i][9], 2),
                            total = Math.Round((double)data.Rows[i][10], 2),
                            descripcion = data.Rows[i][11].ToString()
                        };
                        cotizaciones.herrajes_cotizados.Add(herrajes);
                    }
                    cotizaciones.SaveChanges();
                }
                else if (table == "otros_cotizaciones")
                {
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        otros_cotizaciones otros = new otros_cotizaciones()
                        {
                            folio = (int)data.Rows[i][1],
                            clave = getClave(data.Rows[i][2].ToString()) + "-" + data.Rows[i][0].ToString(),
                            articulo = data.Rows[i][3].ToString(),
                            proveedor = data.Rows[i][4].ToString(),
                            linea = data.Rows[i][5].ToString(),
                            caracteristicas = data.Rows[i][6].ToString(),
                            color = data.Rows[i][7].ToString(),
                            cantidad = Math.Round((double)data.Rows[i][8], 2),
                            largo = Math.Round((double)data.Rows[i][9], 2),
                            alto = Math.Round((double)data.Rows[i][10], 2),
                            descuento = Math.Round((double)data.Rows[i][11], 2),
                            total = Math.Round((double)data.Rows[i][12], 2),
                            descripcion = data.Rows[i][13].ToString()
                        };
                        cotizaciones.otros_cotizaciones.Add(otros);
                    }
                    cotizaciones.SaveChanges();
                    data.Dispose();
                }
                else if (table == "modulos_cotizaciones")
                {
                    listas_entities_pva listas = new listas_entities_pva();

                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        int modulo_id = (int)data.Rows[i][2];
                        int concept = (int)data.Rows[i][20];

                        var modulo_l = (from x in listas.modulos where x.id == modulo_id select x).SingleOrDefault();
                        if (modulo_id > 0)
                        {
                            if (modulo_l != null)
                            {
                                int esquema_id = (int)modulo_l.id_diseño;
                                var esquema = (from v in listas.esquemas where v.id == esquema_id select v).SingleOrDefault();

                                if (esquema != null)
                                {
                                    modulos_cotizaciones modulo = new modulos_cotizaciones()
                                    {
                                        clave = getClave(modulo_l.clave) + "-" + data.Rows[i][0].ToString(),
                                        folio = (int)data.Rows[i][1],
                                        modulo_id = modulo_id,
                                        descripcion = data.Rows[i][3].ToString(),
                                        mano_obra = Math.Round((double)data.Rows[i][4], 2),
                                        dimensiones = data.Rows[i][5].ToString(),
                                        acabado_perfil = data.Rows[i][6].ToString(),
                                        claves_cristales = data.Rows[i][7].ToString(),
                                        cantidad = (int)data.Rows[i][8],
                                        largo = (int)data.Rows[i][9],
                                        alto = (int)data.Rows[i][10],
                                        flete = Math.Round((double)data.Rows[i][11], 2),
                                        desperdicio = Math.Round((double)data.Rows[i][12], 2),
                                        utilidad = Math.Round((double)data.Rows[i][13], 2),
                                        claves_otros = data.Rows[i][14].ToString(),
                                        claves_herrajes = data.Rows[i][15].ToString(),
                                        ubicacion = data.Rows[i][16].ToString(),
                                        claves_perfiles = data.Rows[i][17].ToString(),
                                        articulo = modulo_l.articulo,
                                        linea = modulo_l.linea,
                                        diseño = esquema.diseño,
                                        pic = (byte[])data.Rows[i][18],
                                        merge_id = (int)data.Rows[i][19],
                                        concept_id = (int)data.Rows[i][20],
                                        total = Math.Round((double)data.Rows[i][22], 2),
                                        sub_folio = (int)data.Rows[i][23],
                                        dir = (int)data.Rows[i][24],
                                        news = data.Rows[i][25].ToString(),
                                        new_desing = data.Rows[i][26].ToString(),
                                        orden = (int)data.Rows[i][27]
                                    };
                                    cotizaciones.modulos_cotizaciones.Add(modulo);
                                }
                                cotizaciones.SaveChanges();
                                data.Dispose();
                            }
                        }
                        else
                        {
                            modulos_cotizaciones modulo = new modulos_cotizaciones()
                            {
                                clave = "md-" + data.Rows[i][0].ToString(),
                                folio = (int)data.Rows[i][1],
                                modulo_id = (int)data.Rows[i][2],
                                descripcion = data.Rows[i][3].ToString(),
                                mano_obra = Math.Round((double)data.Rows[i][4], 2),
                                dimensiones = data.Rows[i][5].ToString(),
                                acabado_perfil = data.Rows[i][6].ToString(),
                                claves_cristales = data.Rows[i][7].ToString(),
                                cantidad = (int)data.Rows[i][8],
                                largo = (int)data.Rows[i][9],
                                alto = (int)data.Rows[i][10],
                                flete = Math.Round((double)data.Rows[i][11], 2),
                                desperdicio = Math.Round((double)data.Rows[i][12], 2),
                                utilidad = Math.Round((double)data.Rows[i][13], 2),
                                claves_otros = data.Rows[i][14].ToString(),
                                claves_herrajes = data.Rows[i][15].ToString(),
                                ubicacion = data.Rows[i][16].ToString(),
                                claves_perfiles = data.Rows[i][17].ToString(),
                                pic = (byte[])data.Rows[i][18],
                                merge_id = (int)data.Rows[i][19],
                                concept_id = (int)data.Rows[i][20],
                                articulo = data.Rows[i][21].ToString(),
                                total = Math.Round((double)data.Rows[i][22], 2),
                                sub_folio = (int)data.Rows[i][23],
                                dir = (int)data.Rows[i][24],
                                news = data.Rows[i][25].ToString(),
                                new_desing = data.Rows[i][26].ToString(),
                                orden = (int)data.Rows[i][27]
                            };
                            cotizaciones.modulos_cotizaciones.Add(modulo);
                            cotizaciones.SaveChanges();
                            data.Dispose();
                        }
                    }
                    setNewIndex();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.folio_abierto = -1;
                constants.nombre_cotizacion = string.Empty;
                constants.nombre_proyecto = string.Empty;
                constants.precio_especial_desc = string.Empty;
                constants.subfolio_titles.Clear();
                constants.initsubfoliotitles();
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        //Reordena el index 
        private void setNewIndex()
        {
            cotizaciones_local cotizaciones = new cotizaciones_local();

            var conceptos = (from x in cotizaciones.modulos_cotizaciones where x.concept_id > 0 select x);
                       
            foreach (var c in conceptos)
            {
                if(c != null)
                {
                    int cp = (int)c.concept_id;
                    int h = c.id;
                    var modulos = (from x in cotizaciones.modulos_cotizaciones where x.merge_id == cp select x);
                    foreach (var g in modulos)
                    {
                        if(g != null)
                        {
                            g.merge_id = h;
                        }
                    }
                    c.concept_id = h;
                }
            }
            cotizaciones.SaveChanges();
        }

        public void deleteCotizacion(string table, int folio)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM "+table+" WHERE folio='"+folio+"'";                       
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }

        }
        //ENDS COTIZACIONES

        //Clientes
        public void insertNewClient(string name, string telefono, string correo, string domicilio)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO clientes (nombre, correo_electronico, telefono, domicilio) VALUES (@NOMBRE, @CORREO, @TEL, @DOM)";
            cmd.Parameters.AddWithValue("@NOMBRE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@NOMBRE"].Value = name;
            cmd.Parameters.AddWithValue("@CORREO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CORREO"].Value = correo;
            cmd.Parameters.AddWithValue("@TEL", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@TEL"].Value = telefono;
            cmd.Parameters.AddWithValue("@DOM", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@DOM"].Value = domicilio;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateClient(string name, string telefono, string correo, string domicilio)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "UPDATE clientes SET correo_electronico=@CORREO, telefono=@TEL, domicilio=@DOM WHERE nombre='"+name+"'";          
            cmd.Parameters.AddWithValue("@CORREO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CORREO"].Value = correo;
            cmd.Parameters.AddWithValue("@TEL", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@TEL"].Value = telefono;
            cmd.Parameters.AddWithValue("@DOM", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@DOM"].Value = domicilio;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }


        public void deleteClient(string name)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM clientes WHERE nombre=@NOMBRE";
            cmd.Parameters.AddWithValue("@NOMBRE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@NOMBRE"].Value = name;            

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
        //

        //DELETE ARTICULOS EN UPDATE DE COTIZACION
        public void deleteFilasArticulos(string table, int id)
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM " + table + " WHERE id='" + id + "'";
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                cmd.Dispose();
            }
        }
        //

        //upload propiedades
        public void updatePropiedades(float iva)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = "UPDATE propiedades SET iva=@IVA WHERE id='1'";
            cmd.Parameters.AddWithValue("@IVA", System.Data.SqlDbType.Float);
            cmd.Parameters["@IVA"].Value = iva;            
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                constants.errorLog(err.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
        //

        //modulos
        public void createModule(string clave, string articulo, string linea, string c_vidrio, string id_aluminio, string id_herraje, string id_otros, int secciones, string descripcion, string usuario, int id_diseño, bool cs, string parametros, string reglas, bool privado)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO modulos (clave, articulo, linea, clave_vidrio, id_aluminio, id_herraje, id_otros, secciones, descripcion, usuario, id_diseño, cs, parametros, reglas, privado) VALUES (@CLAVE, @ARTICULO, @LINEA, @C_VIDRIO, @ID_A, @ID_H, @ID_O, @SECC, @DESC, @USER, @DIS, @CS, @PARAM, @REG, @PRIV)";
            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@C_VIDRIO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@C_VIDRIO"].Value = c_vidrio;
            cmd.Parameters.AddWithValue("@ID_A", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ID_A"].Value = id_aluminio;
            cmd.Parameters.AddWithValue("@ID_H", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ID_H"].Value = id_herraje;
            cmd.Parameters.AddWithValue("@ID_O", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ID_O"].Value = id_otros;
            cmd.Parameters.AddWithValue("@SECC", System.Data.SqlDbType.Int);
            cmd.Parameters["@SECC"].Value = secciones;            
            cmd.Parameters.AddWithValue("@DESC", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@DESC"].Value = descripcion;
            cmd.Parameters.AddWithValue("@USER", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@USER"].Value = usuario;            
            cmd.Parameters.AddWithValue("@DIS", System.Data.SqlDbType.Int);
            cmd.Parameters["@DIS"].Value = id_diseño;
            cmd.Parameters.AddWithValue("@CS", System.Data.SqlDbType.Bit);
            cmd.Parameters["@CS"].Value = cs;
            cmd.Parameters.AddWithValue("@PARAM", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PARAM"].Value = parametros;
            cmd.Parameters.AddWithValue("@REG", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@REG"].Value = reglas;
            cmd.Parameters.AddWithValue("@PRIV", System.Data.SqlDbType.Bit);
            cmd.Parameters["@PRIV"].Value = privado;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateModule(int id, string articulo, string linea, string clave_vidrio, string id_aluminio, string id_herraje, string id_otros, int secciones, string descripcion, int id_diseño, bool cs, string parametros, string reglas, bool privado)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "UPDATE modulos SET articulo=@ARTICULO, linea=@LINEA, clave_vidrio=@C_VIDRIO, id_aluminio=@ID_A, id_herraje=@ID_H, id_otros=@ID_O, secciones=@SECC, descripcion=@DESC, id_diseño=@DIS, cs=@CS, parametros=@PARAM, reglas=@REG, privado=@PRIV WHERE id='" + id + "'";

            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@C_VIDRIO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@C_VIDRIO"].Value = clave_vidrio;
            cmd.Parameters.AddWithValue("@ID_A", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ID_A"].Value = id_aluminio;
            cmd.Parameters.AddWithValue("@ID_H", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ID_H"].Value = id_herraje;
            cmd.Parameters.AddWithValue("ID_O", System.Data.SqlDbType.VarChar);
            cmd.Parameters["ID_O"].Value = id_otros;            
            cmd.Parameters.AddWithValue("@SECC", System.Data.SqlDbType.Int);
            cmd.Parameters["@SECC"].Value = secciones;          
            cmd.Parameters.AddWithValue("@DESC", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@DESC"].Value = descripcion;
            cmd.Parameters.AddWithValue("@DIS", System.Data.SqlDbType.Int);
            cmd.Parameters["@DIS"].Value = id_diseño;
            cmd.Parameters.AddWithValue("@CS", System.Data.SqlDbType.Bit);
            cmd.Parameters["@CS"].Value = cs;
            cmd.Parameters.AddWithValue("@PARAM", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PARAM"].Value = parametros;
            cmd.Parameters.AddWithValue("@REG", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@REG"].Value = reglas;
            cmd.Parameters.AddWithValue("@PRIV", System.Data.SqlDbType.Bit);
            cmd.Parameters["@PRIV"].Value = privado;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }


        public void deleteModule(int id)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM modulos WHERE id='" + id + "'";        
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
        //

        //crear esquema
        public void crearEsquema(string nombre, int filas, int columnas, string diseño, string esquemas, bool marco, string grupo)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO esquemas (nombre, filas, columnas, diseño, esquemas, marco, grupo) VALUES (@NOM, @FILAS, @COL, @DIS, @ESQ, @MARCO, @GRP)";
            cmd.Parameters.AddWithValue("@NOM", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@NOM"].Value = nombre;
            cmd.Parameters.AddWithValue("@FILAS", System.Data.SqlDbType.Int);
            cmd.Parameters["@FILAS"].Value = filas;
            cmd.Parameters.AddWithValue("@COL", System.Data.SqlDbType.Int);
            cmd.Parameters["@COL"].Value = columnas;
            cmd.Parameters.AddWithValue("@DIS", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@DIS"].Value = diseño;
            cmd.Parameters.AddWithValue("@ESQ", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ESQ"].Value = esquemas;
            cmd.Parameters.AddWithValue("@MARCO", System.Data.SqlDbType.Bit);
            cmd.Parameters["@MARCO"].Value = marco;
            cmd.Parameters.AddWithValue("@GRP", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@GRP"].Value = grupo;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
        //

        //borrar esquema
        public void borrarEsquema(int id)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM esquemas WHERE id='" + id + "'";
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
        //    

        //crear linea
        public void CreateNewLine(string line_name)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO lineas_modulos (linea_modulo) VALUES (@LINEA)";
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = line_name;
           
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
        //

        //BORRAR LINEA
        public void deleteLine(int id)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM lineas_modulos WHERE id='" + id + "'";
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
        //

        //categorias y proveedores
        public void createCategoria(string categoria, string grupo)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO categorias (categoria, grupo) VALUES (@CAT, @GRP)";
            cmd.Parameters.AddWithValue("@CAT", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CAT"].Value = categoria;
            cmd.Parameters.AddWithValue("@GRP", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@GRP"].Value = grupo;
            
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateCategoria(int id, string categoria, string grupo)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "UPDATE categorias SET categoria=@CAT, grupo=@GRP WHERE id='" + id + "'";

            cmd.Parameters.AddWithValue("@CAT", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CAT"].Value = categoria;
            cmd.Parameters.AddWithValue("@GRP", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@GRP"].Value = grupo;

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void deleteCategoria(int id)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM categorias WHERE id='" + id + "'";
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void createProveedor(string proveedor, string grupo)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "INSERT INTO proveedores (proveedor, grupo) VALUES (@PRO, @GRP)";
            cmd.Parameters.AddWithValue("@PRO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PRO"].Value = proveedor;
            cmd.Parameters.AddWithValue("@GRP", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@GRP"].Value = grupo;

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateProveedor(int id, string proveedor, string grupo)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "UPDATE proveedores SET proveedor=@PRO, grupo=@GRP WHERE id='" + id + "'";

            cmd.Parameters.AddWithValue("@PRO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PRO"].Value = proveedor;
            cmd.Parameters.AddWithValue("@GRP", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@GRP"].Value = grupo;

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void deleteProveedor(int id)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM proveedores WHERE id='" + id + "'";
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
        //

        //Updater
        public string getNewVersionUpdate()
        {
            string result = string.Empty;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT version FROM updater ORDER BY id ASC";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result = r.GetValue(0).ToString();
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public bool getNewConfigurationUpdate()
        {
            bool result = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT actualizacion_completa FROM updater ORDER BY id ASC";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result = (bool)r.GetValue(0);
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }
        //

        public bool getNewConfigurationUpdateEsquemas()
        {
            bool result = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT esquemas FROM updater ORDER BY id ASC";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result = (bool)r.GetValue(0);
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }
        //

        public Boolean isModuleAlive(int id)
        {
            Boolean result = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM modulos WHERE id='" + id + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        //Registro de presupuestos
        public void insertRegistroPresupuestos(string etapa, string informe, string fecha_inicio, string fecha_entrega, int folio, string org)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "INSERT INTO registro_presupuestos (etapa, informe, fecha_inicio, fecha_entrega, folio, org) VALUES (@ETAPA, @INFORME, @FI, @FE, @FOLIO, @ORG)";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@ETAPA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ETAPA"].Value = etapa;
            cmd.Parameters.AddWithValue("@INFORME", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@INFORME"].Value = informe;
            cmd.Parameters.AddWithValue("@FI", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FI"].Value = fecha_inicio;
            cmd.Parameters.AddWithValue("@FE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FE"].Value = fecha_entrega;
            cmd.Parameters.AddWithValue("@FOLIO", System.Data.SqlDbType.Int);
            cmd.Parameters["@FOLIO"].Value = folio;
            cmd.Parameters.AddWithValue("@ORG", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ORG"].Value = org;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateRegistroPresupuestos(string etapa, string informe, string fecha_entrega, int folio)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "UPDATE registro_presupuestos SET etapa=@ETAPA, informe=@INFORME, fecha_entrega=@FE WHERE folio='" + folio + "'";
            cmd.Parameters.AddWithValue("@ETAPA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ETAPA"].Value = etapa;
            cmd.Parameters.AddWithValue("@INFORME", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@INFORME"].Value = informe;
            cmd.Parameters.AddWithValue("@FE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FE"].Value = fecha_entrega;            
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void deleteRegistroPresupuestos(int folio, bool msg=false)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM registro_presupuestos WHERE folio='" + folio + "'";
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                if(msg == true)
                {
                    MessageBox.Show("Se ha eliminado el registro de está cotización.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public string selectRegistroPresupuestos(int folio, string column)
        {
            string result = string.Empty;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT " + column + " FROM registro_presupuestos WHERE folio='" + folio + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result = r.GetString(0);
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public void dropPresupuestosOnGridView(DataGridView dataview, int limit, string tienda, string fecha="", string search="")
        {
            DataTable data = new DataTable();
            try
            {
                string query = string.Empty;

                if (search == "")
                {
                    if (fecha == "")
                    {
                        query = "SELECT TOP " + limit + " registro_presupuestos.folio, cotizaciones.cliente, cotizaciones.nombre_proyecto, cotizaciones.usuario, registro_presupuestos.etapa, registro_presupuestos.fecha_inicio, registro_presupuestos.fecha_entrega FROM registro_presupuestos INNER JOIN cotizaciones ON registro_presupuestos.folio = cotizaciones.folio WHERE registro_presupuestos.org = '" + tienda + "' ORDER BY folio DESC";
                    }
                    else
                    {
                        query = "SELECT TOP " + limit + " registro_presupuestos.folio, cotizaciones.cliente, cotizaciones.nombre_proyecto, cotizaciones.usuario, registro_presupuestos.etapa, registro_presupuestos.fecha_inicio, registro_presupuestos.fecha_entrega FROM registro_presupuestos INNER JOIN cotizaciones ON registro_presupuestos.folio = cotizaciones.folio WHERE registro_presupuestos.org = '" + tienda + "' AND (registro_presupuestos.fecha_inicio LIKE '%" + fecha + "%' OR registro_presupuestos.fecha_entrega LIKE '%" + fecha + "%') ORDER BY folio DESC";
                    }
                }
                else
                {
                    query = "SELECT TOP " + limit + " registro_presupuestos.folio, cotizaciones.cliente, cotizaciones.nombre_proyecto, cotizaciones.usuario, registro_presupuestos.etapa, registro_presupuestos.fecha_inicio, registro_presupuestos.fecha_entrega FROM registro_presupuestos INNER JOIN cotizaciones ON registro_presupuestos.folio = cotizaciones.folio WHERE registro_presupuestos.org = '" + tienda + "' AND (registro_presupuestos.fecha_inicio LIKE '%" + fecha + "%' OR registro_presupuestos.fecha_entrega LIKE '%" + fecha + "%') AND (registro_presupuestos.folio LIKE '%" + constants.stringToInt(search) + "' OR cotizaciones.cliente LIKE '" + search + "%' OR cotizaciones.nombre_proyecto LIKE '" + search + "%' OR registro_presupuestos.etapa = '" + search + "') ORDER BY folio DESC";
                }

                SqlDataAdapter da = new SqlDataAdapter(query, getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(data);
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        public Boolean verificarRegistro(int folio)
        {
            Boolean result = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM registro_presupuestos WHERE folio='" + folio + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public List<string> getTiendas()
        {
            List<string> list = new List<string>();
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT nombre_tienda FROM tiendas";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(r.GetValue(0).ToString());
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return list;
        }

        public Boolean getTienda(string tienda)
        {
            Boolean result = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT * FROM tiendas WHERE nombre_tienda='" + tienda + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public int getTiendaID(string tienda)
        {
            int result = -1;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT id FROM tiendas WHERE nombre_tienda='" + tienda + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result = constants.stringToInt(r.GetValue(0).ToString());
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public string getAliasTienda(string tienda)
        {
            string result = string.Empty;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT alias FROM tiendas WHERE nombre_tienda='" + tienda + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result = r.GetValue(0).ToString();
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public DateTime getvigenciaTienda(string tienda, Form form=null)
        {
            DateTime result = new DateTime();
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();           

            cmd.Connection = connection;
            cmd.CommandText = "SELECT vigencia FROM tiendas WHERE nombre_tienda='" + tienda + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result = Convert.ToDateTime(r.GetValue(0));
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
                MessageBox.Show("[Error] no se ha podido verificar la licencia de esté programa, intente de nuevo.\n\nDe continuar el problema póngase en contacto con el proveedor del sistema.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                if(form != null)
                {
                    form.Close();
                }
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }


        public string getvigenciaType(string tienda)
        {
            string result = string.Empty;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT tipo_licencia FROM tiendas WHERE nombre_tienda='" + tienda + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result = r.GetValue(0).ToString();
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public bool getIvaDesglosado(int folio)
        {
            bool result = true;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT iva_desglosado FROM cotizaciones WHERE folio='" + folio + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result = (bool)r.GetValue(0);
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public DataTable selectProduccionTable(int folio)
        {
            DataTable data = new DataTable();
            try
            {
                SqlDataAdapter adapter = null;
                adapter = new SqlDataAdapter("SELECT * FROM produccion WHERE folio = '" + folio + "'", getConnectionString());                           
                SqlCommandBuilder cb = new SqlCommandBuilder(adapter);
                adapter.Fill(data);               
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
            return data;
        }

        public void updateProduccionTable(int id, string parameters, string observaciones, int m_id)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "UPDATE produccion SET parameters=@PARAM, observaciones=@OBS, m_id=@MID WHERE id='" + id + "'";
            cmd.Parameters.AddWithValue("@PARAM", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PARAM"].Value = parameters;
            cmd.Parameters.AddWithValue("@OBS", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@OBS"].Value = observaciones;
            cmd.Parameters.AddWithValue("@MID", System.Data.SqlDbType.Int);
            cmd.Parameters["@MID"].Value = m_id;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void insertProduccionTable(int m_id, string parameters, string observaciones, int folio)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "INSERT INTO produccion (parameters, observaciones, m_id, folio) VALUES (@PARAM, @OBS, @MID, @FOLIO)";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@PARAM", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PARAM"].Value = parameters;
            cmd.Parameters.AddWithValue("@OBS", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@OBS"].Value = observaciones;
            cmd.Parameters.AddWithValue("@MID", System.Data.SqlDbType.Int);
            cmd.Parameters["@MID"].Value = m_id;
            cmd.Parameters.AddWithValue("@FOLIO", System.Data.SqlDbType.Int);
            cmd.Parameters["@FOLIO"].Value = folio;         
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void deleteProduccionTable(int folio, bool msg = false, Form form=null)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "DELETE FROM produccion WHERE folio='" + folio + "'";
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                if (msg == true)
                {
                    if (form != null)
                    {
                        MessageBox.Show(form, "Se ha eliminado la orden ligada al folio: " + folio + ".", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Se ha eliminado la orden ligada al folio: " + folio + ".", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public Boolean getProduccionID(int id)
        {
            Boolean b = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT id FROM produccion WHERE id='" + id + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    b = true;
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return b;
        }

        public Boolean getFolioProduccion(int folio)
        {
            Boolean b = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT TOP 1 folio FROM produccion WHERE folio='" + folio + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    b = true;
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return b;
        }

        public float getTC()
        {
            float b = 0;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT tc FROM propiedades WHERE id='1'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    b = constants.stringToFloat(r.GetValue(0).ToString());
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
                MessageBox.Show("[Error] se no puede obtener el tipo de cambio del servidor.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return b;
        }

        public float getCotizacionTC(int folio)
        {
            float b = 0;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT tc FROM cotizaciones WHERE folio='" + folio + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    b = constants.stringToFloat(r.GetValue(0).ToString());
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
                MessageBox.Show("[Error] No se puedo obtener el tipo de cambio de la cotización.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return b;
        }

        public float getCostoAluminioKG()
        {
            float b = 0;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT ref_kg_aluminio FROM propiedades WHERE id='1'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    b = constants.stringToFloat(r.GetValue(0).ToString());
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return b;
        }

        public void setCostoAluminioKG(float costo)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "UPDATE propiedades SET ref_kg_aluminio='" + costo + "' WHERE id='1'";
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Se ha guardado con exito la nueva configuración.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void insertNewPaquete(string comp_clave, string comp_items, string comp_type, string comp_articulo)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "INSERT INTO paquetes (comp_clave, comp_items, comp_type, comp_articulo) VALUES (@CLAVE, @ITEMS, @TYPE, @ARTICULO)";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = comp_clave;
            cmd.Parameters.AddWithValue("@ITEMS", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ITEMS"].Value = comp_items;
            cmd.Parameters.AddWithValue("@TYPE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@TYPE"].Value = comp_type;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = comp_articulo;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updatePaquete(int id, string comp_clave, string comp_items, string comp_articulo)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "UPDATE paquetes SET comp_items=@ITEMS, comp_articulo=@ARTICULO WHERE id='" + id + "'";
            cmd.Connection = connection;          
            cmd.Parameters.AddWithValue("@ITEMS", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ITEMS"].Value = comp_items;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = comp_articulo;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void deletePaquete(int id)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "DELETE FROM paquetes WHERE id='" + id + "'";
            cmd.Connection = connection;
          
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public int getIDFromClave(string clave, string table)
        {
            int id = 0;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT id FROM " + table + " WHERE clave='" + clave + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    id = constants.stringToInt(r.GetValue(0).ToString());
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return id;
        }

        //Inventarios ---------------------------------------------------------------------------------------------------------------->
        public void newInventario(string clave, string articulo, string linea, string proveedor, string lista, string costeo, float existencia, int tienda_id)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "INSERT INTO inventario (clave, articulo, linea, proveedor, lista, costeo, existencia, tienda_id) VALUES (@CLAVE, @ARTICULO, @LINEA, @PROV, @LISTA, @COSTEO, @EXIST, @TIENDA)";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@LISTA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LISTA"].Value = lista;
            cmd.Parameters.AddWithValue("@COSTEO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@COSTEO"].Value = costeo;
            cmd.Parameters.AddWithValue("@EXIST", System.Data.SqlDbType.Float);
            cmd.Parameters["@EXIST"].Value = existencia;
            cmd.Parameters.AddWithValue("@TIENDA", System.Data.SqlDbType.Int);
            cmd.Parameters["@TIENDA"].Value = tienda_id;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateInventario(int id, string articulo, string linea, string proveedor, string costeo)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "UPDATE inventario SET articulo=@ARTICULO, linea=@LINEA, proveedor=@PROV, costeo=@COSTEO WHERE id='" + id + "'";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;          
            cmd.Parameters.AddWithValue("@COSTEO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@COSTEO"].Value = costeo;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public float getExistencia(string clave)
        {
            float result = 0;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT existencia FROM inventario WHERE clave='" + clave + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result = constants.stringToFloat(r.GetValue(0).ToString());
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public void updateExistencias(string clave, float cant)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "UPDATE inventario SET existencia=existencia+@CANT WHERE clave='" + clave + "'";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@CANT", System.Data.SqlDbType.Float);
            cmd.Parameters["@CANT"].Value = cant;           
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void deleteInventario()
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "TRUNCATE TABLE inventario";
            cmd.Connection = connection;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void deleteRegistroInventario(int id)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "DELETE FROM inventario WHERE id='" + id + "'";
            cmd.Connection = connection;

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public DataTable getInventario(string lista, int tienda, string filtro="", string filtro_value="")
        {
            DataTable dt = new DataTable();
            try
            {
                string query = string.Empty;
                if(filtro == "" && filtro_value == "")
                {
                    query = "SELECT * FROM inventario WHERE tienda_id='" + tienda + "' AND lista='" + lista + "'";
                }
                else
                {
                    query = "SELECT * FROM inventario WHERE tienda_id='" + tienda + "' AND lista='" + lista + "' ORDER BY CASE WHEN " + filtro + "='" + filtro_value + "' THEN 1 END DESC, articulo ASC";
                }
                SqlDataAdapter da = new SqlDataAdapter(query, getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(dt);
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            return dt;
        }

        public DataTable getArticuloInventario(string lista, int tienda, string value)
        {
            DataTable dt = new DataTable();
            try
            {
                string query = string.Empty;              
                query = "SELECT id, clave, articulo, linea, proveedor, costeo, existencia FROM inventario WHERE tienda_id='" + tienda + "' AND lista='" + lista + "' AND (clave LIKE '" + value + "%' OR articulo LIKE '" + value + "%' OR linea LIKE '" + value + "%' OR proveedor LIKE '" + value + "%') ORDER BY articulo";            
                SqlDataAdapter da = new SqlDataAdapter(query, getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(dt);
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            return dt;
        }

        public void getSalidas(int limit, DataGridView dataview, string lista, int tienda, string filtro = "", string filtro_value = "")
        {
            DataTable data = new DataTable();
            try
            {
                string query = string.Empty;
                if (filtro == "" && filtro_value == "")
                {
                    query = "SELECT TOP " + limit + " id, clave, articulo, linea, proveedor, salidas, comentarios, fecha FROM salidas_inventario WHERE tienda='" + tienda + "' AND lista='" + lista + "'";
                }
                else
                {
                    query = "SELECT TOP " + limit + " id, clave, articulo, linea, proveedor, salidas, comentarios, fecha FROM salidas_inventario WHERE tienda='" + tienda + "' AND lista='" + lista + "' AND " + filtro + "='" + filtro_value + "'";
                }

                SqlDataAdapter da = new SqlDataAdapter(query, getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(data);
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        public void getSalidasPeriodo(int limit, DataGridView dataview, string lista, int tienda, string fecha, bool periodo=false, string fecha_2="")
        {
            DataTable data = new DataTable();
            try
            {
                string query = string.Empty;

                if (!periodo)
                {
                    if (fecha_2 == "")
                    {
                        query = "SELECT id, clave, articulo, linea, proveedor, salidas, comentarios, fecha FROM salidas_inventario WHERE tienda='" + tienda + "' AND lista='" + lista + "' AND fecha='" + fecha + "'";
                    }
                    else
                    {
                        query = "SELECT id, clave, articulo, linea, proveedor, salidas, comentarios, fecha FROM salidas_inventario WHERE tienda='" + tienda + "' AND lista='" + lista + "' AND fecha between '" + fecha + "' AND '" + fecha_2 + "'";
                    }
                }
                else
                {
                    query = "SELECT id, clave, articulo, linea, proveedor, salidas, comentarios, fecha FROM salidas_inventario WHERE tienda='" + tienda + "' AND lista='" + lista + "' AND fecha LIKE '" + fecha + "%'";
                }

                SqlDataAdapter da = new SqlDataAdapter(query, getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(data);
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        public void getEntradas(int limit, DataGridView dataview, string lista, int tienda, string filtro = "", string filtro_value = "")
        {
            DataTable data = new DataTable();
            try
            {
                string query = string.Empty;
                if (filtro == "" && filtro_value == "")
                {
                    query = "SELECT TOP " + limit + " id, clave, articulo, linea, proveedor, entradas, comentarios, fecha FROM entradas_inventario WHERE tienda='" + tienda + "' AND lista='" + lista + "'";
                }
                else
                {
                    query = "SELECT TOP " + limit + " id, clave, articulo, linea, proveedor, entradas, comentarios, fecha FROM entradas_inventario WHERE tienda='" + tienda + "' AND lista='" + lista + "' AND " + filtro + "='" + filtro_value + "'";
                }

                SqlDataAdapter da = new SqlDataAdapter(query, getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(data);
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        public void getEntradasPeriodo(int limit, DataGridView dataview, string lista, int tienda, string fecha, bool periodo=false, string fecha_2="")
        {
            DataTable data = new DataTable();
            try
            {
                string query = string.Empty;

                if (!periodo)
                {
                    if (fecha_2 == "")
                    {
                        query = "SELECT id, clave, articulo, linea, proveedor, entradas, comentarios, fecha FROM entradas_inventario WHERE tienda='" + tienda + "' AND lista='" + lista + "' AND fecha='" + fecha + "'";
                    }
                    else
                    {
                        query = "SELECT id, clave, articulo, linea, proveedor, entradas, comentarios, fecha FROM entradas_inventario WHERE tienda='" + tienda + "' AND lista='" + lista + "' AND fecha between '" + fecha + "' AND '" + fecha_2 + "'";
                    }
                }
                else
                {
                    query = "SELECT id, clave, articulo, linea, proveedor, entradas, comentarios, fecha FROM entradas_inventario WHERE tienda='" + tienda + "' AND lista='" + lista + "' AND fecha LIKE '" + fecha + "%'";
                }

                SqlDataAdapter da = new SqlDataAdapter(query, getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(data);
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        public void newSalida(string clave, string articulo, string linea, string proveedor, string lista, float salidas, string fecha, int tienda, string comentarios)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "INSERT INTO salidas_inventario (clave, articulo, linea, proveedor, lista, salidas, fecha, tienda, comentarios) VALUES (@CLAVE, @ARTICULO, @LINEA, @PROV, @LISTA, @SALIDAS, @FECHA, @TIENDA, @COMENT)";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@LISTA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LISTA"].Value = lista;
            cmd.Parameters.AddWithValue("@SALIDAS", System.Data.SqlDbType.Float);
            cmd.Parameters["@SALIDAS"].Value = salidas;
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@TIENDA", System.Data.SqlDbType.Int);
            cmd.Parameters["@TIENDA"].Value = tienda;
            cmd.Parameters.AddWithValue("@COMENT", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@COMENT"].Value = comentarios;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void newEntrada(string clave, string articulo, string linea, string proveedor, string lista, float entradas, string fecha, int tienda, string comentarios)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "INSERT INTO entradas_inventario (clave, articulo, linea, proveedor, lista, entradas, fecha, tienda, comentarios) VALUES (@CLAVE, @ARTICULO, @LINEA, @PROV, @LISTA, @ENTRADAS, @FECHA, @TIENDA, @COMENT)";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@CLAVE", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CLAVE"].Value = clave;
            cmd.Parameters.AddWithValue("@ARTICULO", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@ARTICULO"].Value = articulo;
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@PROV", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@PROV"].Value = proveedor;
            cmd.Parameters.AddWithValue("@LISTA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LISTA"].Value = lista;
            cmd.Parameters.AddWithValue("@ENTRADAS", System.Data.SqlDbType.Float);
            cmd.Parameters["@ENTRADAS"].Value = entradas;
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;
            cmd.Parameters.AddWithValue("@TIENDA", System.Data.SqlDbType.Int);
            cmd.Parameters["@TIENDA"].Value = tienda;
            cmd.Parameters.AddWithValue("@COMENT", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@COMENT"].Value = comentarios;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public List<string> getInventarioClaves(int tienda_id, string lista)
        {
            List<string> dt = new List<string>();
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT clave FROM inventario WHERE tienda_id='" + tienda_id + "' AND lista='" + lista + "'";

            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    dt.Add(r.GetValue(0).ToString());
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            return dt;
        }

        public List<string> getChangelog()
        {
            List<string> dt = new List<string>();
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT TOP 10 title, text FROM changelog ORDER BY id DESC";

            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0) && !r.IsDBNull(1))
                    {
                        dt.Add(r.GetValue(0).ToString() + "\n\n" + r.GetValue(1).ToString() + "\n\n");
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            return dt;
        }

        public List<string> getAnuncios()
        {
            List<string> dt = new List<string>();
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT TOP 10 texto, fecha FROM anuncios ORDER BY id DESC";

            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0) && !r.IsDBNull(1))
                    {
                        dt.Add("[" + r.GetValue(1).ToString() + "]\n\n" + r.GetValue(0).ToString() + "\n---------------------------------------\n");
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            return dt;
        }

        public void getHistorialSalidas(int limit, DataGridView dataview, string lista, int tienda, string clave)
        {
            DataTable data = new DataTable();
            try
            {
                string query = string.Empty;

                query = "SELECT id, clave, articulo, linea, proveedor, salidas, comentarios, fecha FROM salidas_inventario WHERE tienda='" + tienda + "' AND lista='" + lista + "' AND clave='" + clave + "'";

                SqlDataAdapter da = new SqlDataAdapter(query, getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(data);
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        public void getHistorialEntradas(int limit, DataGridView dataview, string lista, int tienda, string clave)
        {
            DataTable data = new DataTable();
            try
            {
                string query = string.Empty;

                query = "SELECT id, clave, articulo, linea, proveedor, entradas, comentarios, fecha FROM entradas_inventario WHERE tienda='" + tienda + "' AND lista='" + lista + "' AND clave='" + clave + "'";

                SqlDataAdapter da = new SqlDataAdapter(query, getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(data);
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        public void getVariaciones(DataGridView dataview, string linea)
        {
            DataTable data = new DataTable();
            try
            {
                string query = string.Empty;

                query = "SELECT id, nombre, linea, descripcion FROM variaciones WHERE linea='" + linea + "'";

                SqlDataAdapter da = new SqlDataAdapter(query, getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(data);
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        public List<string> getVariacion(int id)
        {
            List<string> result = new List<string>();
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT id, nombre, linea, cambios, nuevos, descripcion FROM variaciones WHERE id='" + id + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        result.Add(r.GetValue(0).ToString());
                    }
                    if (!r.IsDBNull(1))
                    {
                        result.Add(r.GetValue(1).ToString());
                    }
                    if (!r.IsDBNull(2))
                    {
                        result.Add(r.GetValue(2).ToString());
                    }
                    if (!r.IsDBNull(3))
                    {
                        result.Add(r.GetValue(3).ToString());
                    }
                    if (!r.IsDBNull(4))
                    {
                        result.Add(r.GetValue(4).ToString());
                    }
                    if (!r.IsDBNull(5))
                    {
                        result.Add(r.GetValue(5).ToString());
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public void deleteVariacion(int id)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "DELETE FROM variaciones WHERE id='" + id + "'";
            cmd.Connection = connection;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void newVariacion(string nombre, string linea, string cambios, string nuevos, string descripcion)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "INSERT INTO variaciones (nombre, linea, cambios, nuevos, descripcion) VALUES (@NOM, @LINEA, @CAMBIOS, @NUEVOS, @DESC)";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@NOM", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@NOM"].Value = nombre;
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@CAMBIOS", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CAMBIOS"].Value = cambios;
            cmd.Parameters.AddWithValue("@NUEVOS", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@NUEVOS"].Value = nuevos;
            cmd.Parameters.AddWithValue("@DESC", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@DESC"].Value = descripcion;           
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void updateVariacion(int id, string linea, string cambios, string nuevos, string descripcion)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "UPDATE variaciones SET linea=@LINEA, cambios=@CAMBIOS, nuevos=@NUEVOS, descripcion=@DESC WHERE id='" + id + "'";
            cmd.Connection = connection;          
            cmd.Parameters.AddWithValue("@LINEA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@LINEA"].Value = linea;
            cmd.Parameters.AddWithValue("@CAMBIOS", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@CAMBIOS"].Value = cambios;
            cmd.Parameters.AddWithValue("@NUEVOS", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@NUEVOS"].Value = nuevos;
            cmd.Parameters.AddWithValue("@DESC", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@DESC"].Value = descripcion;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public Boolean existVariacion(string nombre)
        {
            Boolean result = false;
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "SELECT nombre FROM variaciones WHERE nombre='" + nombre + "'";
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public List<cotizacion_info> getCountPresupuestos(string empresa)
        {
            List<cotizacion_info> result = new List<cotizacion_info>();
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "select folio, fecha from cotizaciones where tienda=@EMPRESA";
            cmd.Parameters.AddWithValue("@EMPRESA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@EMPRESA"].Value = empresa;
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while(r.Read())
                {
                    if (!r.IsDBNull(0) && !r.IsDBNull(1))
                    {
                        result.Add(new cotizacion_info((int)r.GetValue(0), r.GetValue(1).ToString()));
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public List<p_registros> getCountRegistros(string empresa)
        {
            List<p_registros> result = new List<p_registros>();
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandText = "select folio, fecha_inicio from registro_presupuestos where org=@EMPRESA";
            cmd.Parameters.AddWithValue("@EMPRESA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@EMPRESA"].Value = empresa;
            try
            {
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0) && !r.IsDBNull(1))
                    {
                        result.Add(new p_registros((int)r.GetValue(0), r.GetValue(1).ToString()));
                    }
                }
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public void getAnunciosTable(DataGridView dataview)
        {
            DataTable data = new DataTable();
            try
            {
                string query = string.Empty;

                query = "SELECT * FROM anuncios";

                SqlDataAdapter da = new SqlDataAdapter(query, getConnectionString());
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Fill(data);
                //crear un puntero si la peticion se genera desde otro thread...
                if (dataview.InvokeRequired == true)
                {
                    dataview.Invoke((MethodInvoker)delegate
                    {
                        dataview.DataSource = data;
                    });
                }
                else
                {
                    dataview.DataSource = data;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("[Error] <?>.", constants.msg_box_caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                constants.errorLog(err.ToString());
            }
            finally
            {
                data.Dispose();
            }
        }

        public void newAnuncio(string texto, string fecha)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "INSERT INTO anuncios (texto, fecha) VALUES (@TXT, @FECHA)";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@TXT", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@TXT"].Value = texto;
            cmd.Parameters.AddWithValue("@FECHA", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@FECHA"].Value = fecha;          
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void modificarAnuncio(int id, string texto)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "UPDATE anuncios SET texto=@TXT WHERE id=@ID";
            cmd.Connection = connection;
            cmd.Parameters.AddWithValue("@TXT", System.Data.SqlDbType.VarChar);
            cmd.Parameters["@TXT"].Value = texto;
            cmd.Parameters.AddWithValue("@ID", System.Data.SqlDbType.Int);
            cmd.Parameters["@ID"].Value = id;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public void eliminarAnuncio(int id)
        {
            connection = new SqlConnection();
            connection.ConnectionString = getConnectionString();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "DELETE FROM anuncios WHERE id=@ID";
            cmd.Connection = connection;           
            cmd.Parameters.AddWithValue("@ID", System.Data.SqlDbType.Int);
            cmd.Parameters["@ID"].Value = id;
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                constants.errorLog(e.ToString());
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        ~sqlDateBaseManager()
        {

        }
    }
}
