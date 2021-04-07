using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para Clientes.xaml
    /// </summary>
    public partial class Clientes : Window
    {
        public static string miConexion = ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext = new DataClasses1DataContext(miConexion);

        public Clientes(string nombre)
        {
            InitializeComponent();

            BuscaEmpleado(nombre);
        }

        public Object BuscaEmpleado(string nombreCliente)
        {
            var clientes = dataContext.Cliente.Where(cli => cli.Nombre.Equals(nombreCliente)).ToList();

            if (clientes.Count().ToString() != "0")
            {
                foreach (Cliente cliente in clientes)
                {
                    MessageBox.Show(clientes.Count().ToString());
                }
                return 0;
            }
            else
            {
                MessageBox.Show("No se ha encontrado ninguna coincidencia");
            }
           
            return 0;
        }
    }
}
