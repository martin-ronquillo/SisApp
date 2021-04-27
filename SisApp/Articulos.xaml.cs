using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para Articulos.xaml
    /// </summary>
    public partial class Articulos : Window
    {
        public static string miConexion = ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext = new DataClasses1DataContext(miConexion);

        public Articulos()
        {
            InitializeComponent();

            LlenaInfo();

            LlenaListView();
        }

        private void chckB_stock_Checked(object sender, RoutedEventArgs e)
        {
            LlenaListView();
        }

        private void cb_almacen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LlenaListView();
        }

        private void lv_productos_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DatosProducto();
        }

        private void lv_productos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DatosProducto();
            }
        }

        /*
         * 
         * 
         * METODOS DE LA VENTANA
         * 
         * 
         */

        public void LlenaListView()
        {
            //Crea una lista y la rellena con los datos de la tabla Articulos
            List<Producto> listaProducto = new List<Producto>();

            if (cb_almacen.SelectedItem.ToString() == "Todos")
            {
                foreach (Producto producto in dataContext.Producto)
                {
                    //Si el checkBox "dispone stock" esta activo
                    if(chckB_stock.IsChecked == true)
                    {
                        if (producto.Stock != 0)
                        {
                            listaProducto.Add(
                                new Producto()
                                {
                                    Id = producto.Id,
                                    Categoria = producto.Categoria,
                                    CodigoBarra = producto.CodigoBarra,
                                    Producto1 = producto.Producto1,
                                    Stock = producto.Stock,
                                    PrecioVenta = producto.PrecioVenta,
                                    PrecioCompra = producto.PrecioCompra
                                }
                            );
                        }
                    }
                    else
                    {
                        listaProducto.Add(
                            new Producto()
                            {
                                Id = producto.Id,
                                Categoria = producto.Categoria,
                                CodigoBarra = producto.CodigoBarra,
                                Producto1 = producto.Producto1,
                                Stock = producto.Stock,
                                PrecioVenta = producto.PrecioVenta,
                                PrecioCompra = producto.PrecioCompra
                            }
                        );
                    }
                }
            }
            else
            {
                //Almacen almacen = dataContext.Almacen.First(alm => alm.Almacen1.Equals(cb_almacen.SelectedItem.ToString()));
                List<AlmacenProducto> listaAP = new List<AlmacenProducto>();

                listaAP = dataContext.AlmacenProducto.Where(ap => ap.Almacen.Almacen1.Equals(cb_almacen.SelectedItem.ToString())).ToList();

                foreach (AlmacenProducto ap in listaAP)
                {
                    Producto producto = dataContext.Producto.First(pro => pro.Id.Equals(ap.ProductoId));

                    //Si el checkBox "dispone stock" esta activo
                    if (chckB_stock.IsChecked == true)
                    {
                        if (producto.Stock != 0)
                        {
                            listaProducto.Add(
                                new Producto()
                                {
                                    Id = producto.Id,
                                    Categoria = producto.Categoria,
                                    CodigoBarra = producto.CodigoBarra,
                                    Producto1 = producto.Producto1,
                                    Stock = producto.Stock,
                                    PrecioVenta = producto.PrecioVenta,
                                    PrecioCompra = producto.PrecioCompra
                                }
                            );
                        }
                    }
                    else
                    {
                        listaProducto.Add(
                            new Producto()
                            {
                                Id = producto.Id,
                                Categoria = producto.Categoria,
                                CodigoBarra = producto.CodigoBarra,
                                Producto1 = producto.Producto1,
                                Stock = producto.Stock,
                                PrecioVenta = producto.PrecioVenta,
                                PrecioCompra = producto.PrecioCompra
                            }
                        );
                    }
                }
            }

            lv_productos.ItemsSource = listaProducto;
        }

        public void LlenaInfo()
        {
            cb_almacen.Items.Add("Todos");
            cb_almacen.SelectedItem = "Todos";

            foreach (Almacen almacen in dataContext.Almacen)
            {
                cb_almacen.Items.Add(almacen.Almacen1);
            }
        }

        public void DatosProducto()
        {
            Producto selectedProducto = (Producto)lv_productos.SelectedItem;

            if (selectedProducto != null)
            {

                try
                {
                    Producto producto = dataContext.Producto.First(pro => pro.Id.Equals(selectedProducto.Id));

                    Categoria categoria = dataContext.Categoria.First(cat => cat.Id.Equals(producto.CategoriaId));

                    //Llena la categoria del producto
                    txt_categoria.Text = categoria.Nombre;

                    //Hace una consulta a la tabla relacional entre Almacen y Producto para poder acceder a sus datos (Tabla AlmacenProducto)
                    var query =
                        from almacenProducto in dataContext.AlmacenProducto
                        where almacenProducto.AlmacenId == 8
                        where almacenProducto.ProductoId == producto.Id
                        select new { AlmacenProducto = almacenProducto };

                    txt_enBodega.Text = query.First().AlmacenProducto.Stock.ToString();
                }
                catch
                {
                    txt_categoria.Text = "N/A";

                    txt_enBodega.Text = "N/A";
                }

                txt_codigoBarra.Text = selectedProducto.CodigoBarra;
                txt_existencias.Text = selectedProducto.Stock.ToString();
                txt_producto.Text = selectedProducto.Producto1;
                txt_precioCompra.Text = selectedProducto.PrecioCompra.ToString();
                txt_precioVenta.Text = selectedProducto.PrecioVenta.ToString();
            }
        }
    }
}
