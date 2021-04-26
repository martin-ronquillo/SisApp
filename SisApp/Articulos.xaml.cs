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

        public void LlenaListView()
        {
            /*var dt = new DataTable();

            dt.Columns.Add("CodigoBarra", typeof(string));
            dt.Columns.Add("Producto1", typeof(string));
            dt.Columns.Add("Stock", typeof(int));
            dt.Columns.Add("PrecioVenta", typeof(decimal));
            dt.Columns.Add("PrecioCompra", typeof(decimal));*/

            //Crea una lista y la rellena con los datos de la tabla Articulos
            List<Producto> listaProducto = new List<Producto>();

            foreach (Producto producto in dataContext.Producto)
            {
                //dt.Rows.Add(new object[] { producto.CodigoBarra, producto.Producto1, producto.Stock, producto.PrecioVenta, producto.PrecioCompra });
                
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

        private void lv_productos_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Producto selectedProducto = (Producto)lv_productos.SelectedItem;

            try
            {
                Producto producto = dataContext.Producto.First(pro => pro.Id.Equals(selectedProducto.Id));

                Categoria categoria = dataContext.Categoria.First(cat => cat.Id.Equals(producto.CategoriaId));

                txt_categoria.Text = categoria.Nombre;

                AlmacenProducto almacenProducto = dataContext.AlmacenProducto.Join();
            }
            catch
            {

            }


            if (selectedProducto != null)
            {
                txt_codigoBarra.Text = selectedProducto.CodigoBarra;
                txt_existencias.Text = selectedProducto.Stock.ToString();
                txt_producto.Text = selectedProducto.Producto1;
            }
        }

        private void lv_productos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Producto selectedProducto = (Producto)lv_productos.SelectedItem;

                if (selectedProducto != null)
                {
                    txt_producto.Text = selectedProducto.Producto1;
                }
            }
        }
    }
}
