using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Diagnostics;

namespace InventarioApp
{
    public partial class FrmVenta : Form
    {
        private List<Producto> productosEnMemoria = new List<Producto>();
        private GestorVentas gestorVentas;
        private readonly string connectionString = "server=localhost;database=inventario_db;uid=root;pwd=;";

        public Action RecargarInventarioCallback { get; set; }

        public FrmVenta()
        {
            InitializeComponent();
        }

        public FrmVenta(GestorVentas gestorExistente)
        {
            InitializeComponent();
            this.gestorVentas = gestorExistente;
        }
        private void FrmVenta_Load(object sender, EventArgs e)
        {
            CargarProductosParaVenta();
        }

        private void CargarProductosParaVenta()
        {
            if (gestorVentas == null) return;

            productosEnMemoria.Clear();
            cmbProductos.DataSource = null;

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT id, nombre, precio, stock FROM productos WHERE stock > 0";
                    MySqlCommand cmd = new MySqlCommand(query, con);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productosEnMemoria.Add(new Producto
                            {
                                Id = reader.GetInt32("id"),
                                Nombre = reader.GetString("nombre"),
                                Precio = reader.GetDecimal("precio"),
                                Stock = reader.GetInt32("stock")
                            });
                        }
                    }
                }

                cmbProductos.DataSource = productosEnMemoria;
                cmbProductos.DisplayMember = "Nombre";
                cmbProductos.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                // Solo mostramos error si no estamos en modo diseño.
                if (Process.GetCurrentProcess().ProcessName != "devenv")
                {
                    MessageBox.Show("Error al cargar productos para la venta: " + ex.Message, "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            if (cmbProductos.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un producto.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida y positiva.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var productoSeleccionado = cmbProductos.SelectedItem as Producto;

            if (cantidad > productoSeleccionado.Stock)
            {
                MessageBox.Show($"Stock insuficiente. Solo hay {productoSeleccionado.Stock} unidades.", "Stock Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            var detalle = new DetalleVenta
            {
                ProductoId = productoSeleccionado.Id,
                NombreProducto = productoSeleccionado.Nombre,
                Cantidad = cantidad,
                PrecioUnitario = productoSeleccionado.Precio
            };

            var nuevaVenta = new Venta
            {
                Fecha = DateTime.Now,
                Detalles = new List<DetalleVenta> { detalle },
                TotalVenta = detalle.Subtotal
            };

            if (gestorVentas.RegistrarVenta(nuevaVenta))
            {
                MessageBox.Show($"Venta registrada por {nuevaVenta.TotalVenta:C}.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                RecargarInventarioCallback?.Invoke();

                CargarProductosParaVenta();
                txtCantidad.Text = "";
            }
        }
    }
}