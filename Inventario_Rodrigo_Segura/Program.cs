using Inventario_Rodrigo_Segura;
using System;
using System.Windows.Forms;

namespace InventarioApp
{
    static class Program
    {
        // Punto de entrada de la aplicación
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); 
        }
    }
}
