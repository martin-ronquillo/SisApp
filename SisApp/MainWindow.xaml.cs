using System;
using System.Windows;
using System.Windows.Input;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;

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

        ArticulosVenta venta = new ArticulosVenta();

        List<ArticulosVenta> listaProductos = new List<ArticulosVenta>();

        private void txt_cod_producto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int idProducto = int.Parse(txt_cod_producto.Text);

                if (listaProductos.FirstOrDefault(pro => pro.Id.Equals(idProducto)) == null)
                {
                    lv_facturar.ItemsSource = null;

                    listaProductos = venta.InsertaArticulo(listaProductos, idProducto);

                    lv_facturar.ItemsSource = listaProductos;
                }
                else
                {
                    lv_facturar.ItemsSource = null;

                    listaProductos = venta.ActualizaArticulo(listaProductos, idProducto);

                    lv_facturar.ItemsSource = listaProductos;
                }
            }

            txt_cod_producto.Focus();
        }
    }
}
