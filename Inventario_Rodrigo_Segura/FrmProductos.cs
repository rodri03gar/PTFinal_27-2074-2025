using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Linq;

namespace InventarioApp
{
    public partial class FrmProductos : Form
    {
        string connectionString = "server=localhost;database=inventario_db;uid=root;pwd=;";
        private GestorVentas gestorVentas = new GestorVentas();

        public FrmProductos()
        {
            InitializeComponent();
        }

        private void FrmProductos_Load(object sender, EventArgs e)
        {
            CargarProductos();
        }

        public void CargarProductos()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    // Lógica para revisar si la tabla está vacía y resetear el AUTO_INCREMENT.
                    string countQuery = "SELECT COUNT(*) FROM productos";
                    MySqlCommand countCmd = new MySqlCommand(countQuery, con);
                    long rowCount = (long)countCmd.ExecuteScalar();

                    if (rowCount == 0)
                    {
                        // Si no hay filas, garantizamos que el próximo ID sea 1.
                        string resetQuery = "ALTER TABLE productos AUTO_INCREMENT = 1";
                        MySqlCommand resetCmd = new MySqlCommand(resetQuery, con);
                        resetCmd.ExecuteNonQuery();
                    }

                    MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM productos", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvProducts.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text == "" || txtPrecio.Text == "" || txtStock.Text == "")
            {
                MessageBox.Show("Todos los campos son obligatorios.");
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    string query = "INSERT INTO productos (nombre, precio, stock) VALUES (@n, @p, @s)";
                    MySqlCommand cmd = new MySqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@n", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@p", Convert.ToDecimal(txtPrecio.Text));
                    cmd.Parameters.AddWithValue("@s", Convert.ToInt32(txtStock.Text));

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Producto agregado correctamente.");
                    CargarProductos();
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (txtID.Text == "")
            {
                MessageBox.Show("Selecciona un producto primero.");
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    string query = "UPDATE productos SET nombre=@n, precio=@p, stock=@s WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtID.Text));
                    cmd.Parameters.AddWithValue("@n", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@p", Convert.ToDecimal(txtPrecio.Text));
                    cmd.Parameters.AddWithValue("@s", Convert.ToInt32(txtStock.Text));

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Producto actualizado.");
                    CargarProductos();
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (txtID.Text == "")
            {
                MessageBox.Show("Selecciona un producto.");
                return;
            }

            if (MessageBox.Show("¿Eliminar este producto?", "Confirmación",
                MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            try
            {
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    string query = "DELETE FROM productos WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtID.Text));

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Producto eliminado.");
                    CargarProductos();
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtID.Text = dgvProducts.Rows[e.RowIndex].Cells["id"].Value.ToString();
                txtNombre.Text = dgvProducts.Rows[e.RowIndex].Cells["nombre"].Value.ToString();
                txtPrecio.Text = dgvProducts.Rows[e.RowIndex].Cells["precio"].Value.ToString();
                txtStock.Text = dgvProducts.Rows[e.RowIndex].Cells["stock"].Value.ToString();
            }
        }

        private void LimpiarCampos()
        {
            txtID.Text = "";
            txtNombre.Text = "";
            txtPrecio.Text = "";
            txtStock.Text = "";
        }

        private void btnAbrirVentas_Click(object sender, EventArgs e)
        {
            // Creamos el formulario de venta y le pasamos nuestra instancia única de gestor
            FrmVenta formularioVentas = new FrmVenta(this.gestorVentas);
            // También le pasamos el callback para que recargue el inventario en el grid
            formularioVentas.RecargarInventarioCallback = this.CargarProductos;
            formularioVentas.ShowDialog();
        }

        private void btnGenerarReportes_Click(object sender, EventArgs e)
        {
            var todosLosProductos = gestorVentas.ObtenerTodosLosProductos();
            decimal ventasTotales = gestorVentas.ObtenerVentasTotales();

            var masCaro = gestorVentas.ObtenerProductoMasCaro(todosLosProductos);
            var menorStock = gestorVentas.ObtenerProductoConMenorStock(todosLosProductos);
            int totalExistencias = gestorVentas.ObtenerTotalExistencias(todosLosProductos);

            string reporteCaro = masCaro != null ? $"{masCaro.Nombre} ({masCaro.Precio:C})" : "N/A";
            string reporteStock = menorStock != null ? $"{menorStock.Nombre} ({menorStock.Stock} uds)" : "N/A";

            string mensajeReporte =
                $"Reporte de Inventario:\n\n" +
                $"Producto más caro: {reporteCaro}\n" +
                $"Producto con menor stock: {reporteStock}\n" +
                $"Total de existencias: {totalExistencias} unidades\n\n" +
                $"Ventas Totales (Persistidas): {ventasTotales:C}";

            MessageBox.Show(mensajeReporte, "Reportes del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}