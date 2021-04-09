using System;
using System.Windows;
using System.Windows.Input;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;

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

            //Numero de Venta
            InfoVenta infoVenta = new InfoVenta();

            txt_nVenta.Text = infoVenta.VentaNum().ToString();

            //Seleccionar cliente a facturar
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

        //Instancia en el archivo InfoVenta
        ArticulosVenta ArticulosVenta = new ArticulosVenta();

        InfoFactura InfoFactura = new InfoFactura();

        //Crea la lista que contendra los productos a facturar
        List<ArticulosVenta> listaProductos = new List<ArticulosVenta>();

        //Agregar Producto al listView
        private void txt_cod_producto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AgregaElemento();
                InfoFactura.SubtotalFactura(listaProductos);
            }

            txt_cod_producto.Text = "";

            txt_cod_producto.Focus();
        }

        //Quitar producto al listView
        private void lv_facturar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                QuitaElemento();
            }
        }

        public void AgregaElemento()
        {
            if (Regex.IsMatch(txt_cod_producto.Text, "[^0-9]") | string.IsNullOrEmpty(txt_cod_producto.Text))
            {
                MessageBox.Show("Solo numeros");
            }
            else
            {
                int idProducto = int.Parse(txt_cod_producto.Text);

                if (listaProductos.FirstOrDefault(pro => pro.Id.Equals(idProducto)) == null)
                {
                    lv_facturar.ItemsSource = null;

                    listaProductos = ArticulosVenta.InsertaArticulo(listaProductos, idProducto);

                    lv_facturar.ItemsSource = listaProductos;
                }
                else
                {
                    lv_facturar.ItemsSource = null;

                    listaProductos = ArticulosVenta.AumentaArticulo(listaProductos, idProducto);

                    lv_facturar.ItemsSource = listaProductos;
                }
            }
        }

        public void QuitaElemento()
        {
            ArticulosVenta selectedProducto = (ArticulosVenta)lv_facturar.SelectedItem;

            if (selectedProducto != null)
            {
                lv_facturar.ItemsSource = null;

                listaProductos = ArticulosVenta.ReduceArticulo(selectedProducto, listaProductos);

                lv_facturar.ItemsSource = listaProductos;

                lv_facturar.ScrollIntoView(selectedProducto);
            }
        }
    }
}
