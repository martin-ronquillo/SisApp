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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataClasses1DataContext dataContext;

        public MainWindow()
        {
            InitializeComponent();

            string miConexion = ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;

            dataContext = new DataClasses1DataContext(miConexion);

            //True: Muestra el loggin
            if (Singleton.Instancia.estado)
            {
                //Instancia el formulario loggin y se le da el foco principal, para que la ejecucion termine en caso de cerrar el form
                Loggin loggin = new Loggin();
                Application.Current.MainWindow = loggin;

                loggin.ShowDialog();
            }

            //Devuelve el Mainfocus al formulario
            Application.Current.MainWindow = this;

            LoggedUser userData = new LoggedUser(Singleton.Instancia.idUser);

            txt_vendedor.Text = userData.Nombre + " " + userData.Apellido;

            date_fecha.DisplayDateEnd = DateTime.Today;
            date_fecha.SelectedDate = DateTime.Today;

            ClienteFactura clienteFactura = new ClienteFactura();

            txt_cliente.Text = clienteFactura.Nombre + ' ' + clienteFactura.Apellido;
            txt_cedula.Text = clienteFactura.Cedula;
            txt_email.Text = clienteFactura.Email;
            txt_direccion.Text = clienteFactura.Direccion;
        }

        //Boton busca el cliente al que se va a facturar
        private void btn_busca_cliente_Click(object sender, RoutedEventArgs e)
        {
            Clientes clientes = new Clientes();
            
            clientes.ShowDialog();

            ClienteFactura clienteFactura = new ClienteFactura(Singleton.Instancia.selectedCliente);

            txt_cliente.Text = clienteFactura.Nombre + ' ' + clienteFactura.Apellido;
            txt_cedula.Text = clienteFactura.Cedula;
            txt_email.Text = clienteFactura.Email;
            txt_direccion.Text = clienteFactura.Direccion;
        }

        private void txt_cod_producto_KeyDown(object sender, KeyEventArgs e)
        {
            ArticulosVenta venta = new ArticulosVenta();

            if (e.Key == Key.Enter)
                lv_facturar.ItemsSource = venta.InsertaArticulo(2);
        }
    }
}
