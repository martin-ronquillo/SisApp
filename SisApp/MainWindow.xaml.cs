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

                //Devuelve el Mainfocus al formulario
                Application.Current.MainWindow = this;
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }

            LimpiaForm();
        }

        //Controla las teclas que se presionan estando en el formulario
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += HandleKeyPress;
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                MainWindow helperWindow = new MainWindow();

                helperWindow.Show();
            }

            if (e.Key == Key.F2)
            {
                BuscaCliente();
            }

            if (e.Key == Key.F6)
            {
                Articulos articulos = new Articulos();

                articulos.Show();
            }
        }

        //Boton busca el cliente al que se va a facturar
        private void btn_busca_cliente_Click(object sender, RoutedEventArgs e)
        {
            BuscaCliente();
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
                LlenaInfoFactura();
                //Modifica el total al Agregar un item de la lista si existe un descuento mayor que 0
                if (txt_descuento.Text != "0" & !string.IsNullOrEmpty(txt_descuento.Text))
                {
                    InfoFactura.DescuentoFactura(int.Parse(txt_descuento.Text));
                    txt_val_total.Text = InfoFactura.Total.ToString();
                }
                //Modificar el Cambio en caso de que la casilla "Efectivo" este llena
                if (txt_efectivo.Text != "0" & !string.IsNullOrEmpty(txt_efectivo.Text))
                {
                    if (InfoFactura.Total < double.Parse(txt_efectivo.Text))
                    {
                        AgregaEfectivo();
                    }
                    else
                    {
                        txt_efectivo.Text = "0";
                    }
                }
            }
        }

        //Quitar producto al listView
        private void lv_facturar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                QuitaElemento();
                LlenaInfoFactura();
                //Modifica el total al Quitar un item de la lista si existe un descuento mayor que 0
                if (txt_descuento.Text != "0" & !string.IsNullOrEmpty(txt_descuento.Text))
                {
                    InfoFactura.DescuentoFactura(int.Parse(txt_descuento.Text));
                    txt_val_total.Text = InfoFactura.Total.ToString();
                }
                //Modificar el Cambio en caso de que la casilla "Efectivo" este llena
                if (txt_efectivo.Text != "0" & !string.IsNullOrEmpty(txt_efectivo.Text))
                {
                    if (InfoFactura.Total < double.Parse(txt_efectivo.Text))
                    {
                        AgregaEfectivo();
                    }
                    else
                    {
                        txt_efectivo.Text = "0";
                    }
                }
            }
        }

        //Agregar un descuento
        private void txt_descuento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter | e.Key == Key.Tab)
            {
                if (Regex.IsMatch(txt_descuento.Text, "[^0-9]"))
                {
                    MessageBox.Show("Solo numeros enteros");
                    txt_descuento.Text = "0";
                    txt_val_total.Text = InfoFactura.TotalAnterior.ToString();
                }
                if (txt_descuento.Text == "0" | string.IsNullOrEmpty(txt_descuento.Text))
                {
                    txt_descuento.Text = "0";

                    InfoFactura.Total = InfoFactura.TotalAnterior;
                    txt_val_total.Text = InfoFactura.TotalAnterior.ToString();
                    
                    //Modificar el Cambio en caso de que la casilla "Efectivo" este llena
                    if (txt_efectivo.Text != "0" & !string.IsNullOrEmpty(txt_efectivo.Text))
                    {
                        if (InfoFactura.Total < double.Parse(txt_efectivo.Text))
                        {
                            AgregaEfectivo();
                        }
                        else
                        {
                            txt_efectivo.Text = "0";
                        }
                    }
                }
                else
                {
                    InfoFactura.DescuentoFactura(int.Parse(txt_descuento.Text));
                    txt_val_total.Text = InfoFactura.Total.ToString();

                    //Modificar el Cambio en caso de que la casilla "Efectivo" este llena
                    if (txt_efectivo.Text != "0" & !string.IsNullOrEmpty(txt_efectivo.Text))
                    {
                        if (InfoFactura.Total < double.Parse(txt_efectivo.Text))
                        {
                            AgregaEfectivo();
                        }
                        else
                        {
                            txt_efectivo.Text = "0";
                        }
                    }
                }
            }
        }

        //Agrega efectivo
        private void txt_efectivo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter | e.Key == Key.Tab)
            {
                AgregaEfectivo();
            }
        }

        //Boton que genera la factura
        private void btn_facturar_Click(object sender, RoutedEventArgs e)
        {
            if (txt_efectivo.Text != "0" & !string.IsNullOrEmpty(txt_efectivo.Text))
            {
                if (InfoFactura.Total <= double.Parse(txt_efectivo.Text))
                {
                    double[] valoresFactura = new double[] { InfoFactura.SubTotal, InfoFactura.Iva, InfoFactura.Descuento, InfoFactura.Total, InfoFactura.Efectivo, InfoFactura.Cambio };

                    Facturar facturar = new Facturar(Singleton.Instancia.idUser, txt_cedula.Text, date_fecha.Text, listaProductos, valoresFactura);

                    facturar.RegistraVenta();

                    facturar.RegistraFactura();

                    LimpiaForm();
                }
                else
                {
                    MessageBox.Show("Error, El valor del Efectivo no puede ser menor que el Total a Pagar");
                }
            }
            else
            {
                MessageBox.Show("Error, El valor del Efectivo no puede ser menor que el Total a Pagar");
            }
        }

        /*
         * 
         * 
         * 
         *  METODOS UTILIZADOS PARA EJECUCIONES DE FUNCIONES DESDE LOS BOTONES, TEXTBOX, ETC.
         * 
         * 
         * 
         */

        public void BuscaCliente()
        {
            Clientes clientes = new Clientes();

            clientes.ShowDialog();

            ClienteFactura clienteFactura = new ClienteFactura(Singleton.Instancia.selectedCliente);

            txt_cliente.Text = clienteFactura.Nombre + ' ' + clienteFactura.Apellido;
            txt_cedula.Text = clienteFactura.Cedula;
            txt_telefono.Text = clienteFactura.Telefono;
            txt_email.Text = clienteFactura.Email;
            txt_direccion.Text = clienteFactura.Direccion;
        }

        public void AgregaElemento()
        {
            if (Regex.IsMatch(txt_cod_producto.Text, "[^0-9]") | string.IsNullOrEmpty(txt_cod_producto.Text))
            {
                MessageBox.Show("Solo numeros enteros");
            }
            else
            {
                int idProducto = int.Parse(txt_cod_producto.Text);

                if (listaProductos.FirstOrDefault(pro => pro.Id.Equals(idProducto)) == null)
                {
                    lv_facturar.ItemsSource = null;

                    listaProductos = ArticulosVenta.InsertaArticulo(listaProductos, idProducto);

                    lv_facturar.ItemsSource = listaProductos;

                    //Si no existen elementos en el listView, desactiva el boton Facturar
                    if (listaProductos.Count <= 0)
                    {
                        btn_facturar.IsEnabled = false;
                    }
                    else
                    {
                        btn_facturar.IsEnabled = true;
                    }
                }
                else
                {
                    lv_facturar.ItemsSource = null;

                    listaProductos = ArticulosVenta.AumentaArticulo(listaProductos, idProducto);

                    lv_facturar.ItemsSource = listaProductos;
                    
                    //Si no existen elementos en el listView, desactiva el boton Facturar
                    if (listaProductos.Count <= 0)
                    {
                        btn_facturar.IsEnabled = false;
                    }
                    else
                    {
                        btn_facturar.IsEnabled = true;
                    }
                }
            }

            txt_cod_producto.Text = "";

            txt_cod_producto.Focus();
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

                if (listaProductos.Count <= 0)
                {
                    btn_facturar.IsEnabled = false;
                }
                else
                {
                    btn_facturar.IsEnabled = true;
                }
            }
        }

        public void LlenaInfoFactura()
        {
            InfoFactura.ValoresFactura(listaProductos);

            txt_subTotal.Text = InfoFactura.SubTotal.ToString();
            txt_iva.Text = InfoFactura.Iva.ToString();
            txt_val_total.Text = InfoFactura.Total.ToString();
        }

        public void AgregaEfectivo()
        {
            string cadena = txt_efectivo.Text;
            cadena = cadena.Replace(".", ",");
            double value;
            if (!double.TryParse(cadena, out value))
            {
                MessageBox.Show("Solo numeros!");
                txt_efectivo.Focus();
                txt_efectivo.Text = "0";
            }
            else
            {
                InfoFactura.PagaEfectivo(double.Parse(cadena));
                txt_cambio.Text = InfoFactura.Cambio.ToString();
            }
        }

        public void LimpiaForm()
        {
            LoggedUser userData = new LoggedUser(Singleton.Instancia.idUser);

            txt_vendedor.Text = userData.Nombre + " " + userData.Apellido;

            date_fecha.DisplayDateEnd = DateTime.Today;
            date_fecha.SelectedDate = DateTime.Today;
            //Obtener el nombre del equipo en el que se esta ejecutando la venta
            txt_caja.Text = Environment.MachineName;

            //Numero de Venta
            InfoVenta infoVenta = new InfoVenta();

            txt_nVenta.Text = infoVenta.VentaNum().ToString("D7");

            //Establece por defecto al cliente "Consumidor Final" paraa facturar
            ClienteFactura clienteFactura = new ClienteFactura();

            txt_cliente.Text = clienteFactura.Nombre + ' ' + clienteFactura.Apellido;
            txt_cedula.Text = clienteFactura.Cedula;
            txt_telefono.Text = clienteFactura.Telefono;
            txt_email.Text = clienteFactura.Email;
            txt_direccion.Text = clienteFactura.Direccion;

            lv_facturar.ItemsSource = null;

            txt_subTotal.Text = null;
            txt_iva.Text = null;
            txt_descuento.Text = null;
            txt_val_total.Text = null;
            txt_efectivo.Text = null;
            txt_cambio.Text = null;
            btn_facturar.IsEnabled = false;
        }
    }
}
