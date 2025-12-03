using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace InventarioApp
{
    public static class PersistenceHelper
    {
        public static string ProductsJsonPath => Path.Combine(System.Windows.Forms.Application.StartupPath, "productos.json");
        public static string SalesJsonPath => Path.Combine(System.Windows.Forms.Application.StartupPath, "ventas.json");

        // Manejo de Productos
        public static void SaveProductsJson(string path, IEnumerable<Producto> products)
        {
            var json = JsonConvert.SerializeObject(products, Formatting.Indented);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        public static List<Producto> LoadProductsJson(string path)
        {
            if (!File.Exists(path)) return new List<Producto>();
            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                return JsonConvert.DeserializeObject<List<Producto>>(json) ?? new List<Producto>();
            }
            catch
            {
                return new List<Producto>();
            }
        }

        // Manejo de Ventas 
        public static void SaveSalesJson(string path, IEnumerable<Venta> sales)
        {
            var json = JsonConvert.SerializeObject(sales, Formatting.Indented);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        public static List<Venta> LoadSalesJson(string path)
        {
            if (!File.Exists(path)) return new List<Venta>();
            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                return JsonConvert.DeserializeObject<List<Venta>>(json) ?? new List<Venta>();
            }
            catch
            {
                return new List<Venta>();
            }
        }
    }
}