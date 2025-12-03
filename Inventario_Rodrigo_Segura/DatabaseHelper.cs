using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace InventarioApp
{
    public class DatabaseHelper
    {
        private string conexion = "server=localhost;database=inventario_db;uid=root;pwd=;";

        public MySqlConnection Conectar()
        {
            return new MySqlConnection(conexion);
        }

        public DataTable ObtenerProductos()
        {
            DataTable tabla = new DataTable();
            try
            {
                using (var cn = Conectar())
                {
                    cn.Open();
                    string query = "SELECT * FROM productos";
                    using (var da = new MySqlDataAdapter(query, cn))
                    {
                        da.Fill(tabla);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
            return tabla;
        }

        public void InsertarProducto(string nombre, int stock, decimal precio)
        {
            try
            {
                using (var cn = Conectar())
                {
                    cn.Open();
                    string query = "INSERT INTO productos (nombre, stock, precio) VALUES (@n, @s, @p)";
                    using (var cmd = new MySqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@n", nombre);
                        cmd.Parameters.AddWithValue("@s", stock);
                        cmd.Parameters.AddWithValue("@p", precio);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error al agregar producto: " + ex.Message);
            }
        }

        public void ActualizarProducto(int id, string nombre, int stock, decimal precio)
        {
            try
            {
                using (var cn = Conectar())
                {
                    cn.Open();
                    string query = "UPDATE productos SET nombre=@n, stock=@s, precio=@p WHERE id=@id";
                    using (var cmd = new MySqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@n", nombre);
                        cmd.Parameters.AddWithValue("@s", stock);
                        cmd.Parameters.AddWithValue("@p", precio);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error al actualizar producto: " + ex.Message);
            }
        }

        public void EliminarProducto(int id)
        {
            try
            {
                using (var cn = Conectar())
                {
                    cn.Open();
                    string query = "DELETE FROM productos WHERE id=@id";
                    using (var cmd = new MySqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error al eliminar producto: " + ex.Message);
            }
        }
    }
}
