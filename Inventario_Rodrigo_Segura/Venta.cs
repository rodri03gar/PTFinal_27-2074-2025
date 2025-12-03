using System;
using System.Collections.Generic;

namespace InventarioApp
{
    // Encabezado de la venta 
    public class Venta
    {
        public int Id { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
        public decimal TotalVenta { get; set; }
        public DateTime Fecha { get; set; }
    }

    // Detalle de cada producto vendido en una transacción
    public class DetalleVenta
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}