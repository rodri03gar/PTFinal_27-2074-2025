using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace InventarioApp
{
    public class GestorVentas
    {
        private readonly string connectionString = "server=localhost;database=inventario_db;uid=root;pwd=;";

        public List<Venta> ListaVentas { get; private set; }

        public GestorVentas()
        {
            // Cargar ventas desde el archivo JSON al iniciar
            ListaVentas = PersistenceHelper.LoadSalesJson(PersistenceHelper.SalesJsonPath);
        }

        public bool RegistrarVenta(Venta nuevaVenta)
        {
            if (nuevaVenta.Detalles.Count == 0) return false;

            MySqlConnection con = null;
            MySqlTransaction transaction = null;

            try
            {
                con = new MySqlConnection(connectionString);
                con.Open();
                transaction = con.BeginTransaction();

                foreach (var detalle in nuevaVenta.Detalles)
                {
                    if (detalle.Cantidad <= 0)
                        throw new Exception($"La cantidad vendida para el producto ID {detalle.ProductoId} debe ser positiva.");

                    string updateQuery = "UPDATE productos SET stock = stock - @cantidad WHERE id = @id AND stock >= @cantidad";
                    MySqlCommand cmd = new MySqlCommand(updateQuery, con, transaction);

                    cmd.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
                    cmd.Parameters.AddWithValue("@id", detalle.ProductoId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Stock insuficiente o producto ID {detalle.ProductoId} no encontrado en inventario.");
                    }
                }

                transaction.Commit();

                // Asignar ID y añadir a la lista en memoria
                nuevaVenta.Id = this.ListaVentas.Count > 0 ? this.ListaVentas.Max(v => v.Id) + 1 : 1;
                this.ListaVentas.Add(nuevaVenta);

                // Guardar la lista actualizada en el archivo JSON
                PersistenceHelper.SaveSalesJson(PersistenceHelper.SalesJsonPath, this.ListaVentas);

                return true;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show("Error al registrar la venta. Motivo: " + ex.Message, "Error de Venta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                con?.Close();
            }
        }

        public List<Producto> ObtenerTodosLosProductos()
        {
            var productos = new List<Producto>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT id, nombre, precio, stock FROM productos";
                    MySqlCommand cmd = new MySqlCommand(query, con);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productos.Add(new Producto
                            {
                                Id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre"),
                                Precio = reader.GetDecimal("precio"),
                                Stock = reader.GetInt32("stock")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener productos para reportes: " + ex.Message);
            }
            return productos;
        }

        public Producto ObtenerProductoMasCaro(List<Producto> productos)
        {
            return productos.OrderByDescending(p => p.Precio).FirstOrDefault();
        }

        public Producto ObtenerProductoConMenorStock(List<Producto> productos)
        {
            return productos.Where(p => p.Stock > 0).OrderBy(p => p.Stock).FirstOrDefault();
        }

        public int ObtenerTotalExistencias(List<Producto> productos)
        {
            return productos.Sum(p => p.Stock);
        }

        public decimal ObtenerVentasTotales()
        {
            return ListaVentas.Sum(v => v.TotalVenta);
        }
    }
}