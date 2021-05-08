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
using DataModels;
using LinqToDB;

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para SeleccionaIngreso.xaml
    /// </summary>
    public partial class SeleccionaIngreso : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        InventarioArticulos inventarioArticulos = new InventarioArticulos();
        IngresaProductos IngresaProductos = new IngresaProductos();

        List<InventarioArticulos> listaTodosProducto = new List<InventarioArticulos>();
        List<ProductosSeleccionados> listaSeleccionados = new List<ProductosSeleccionados>();

        public bool checkBox = false;

        public SeleccionaIngreso()
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;

            LlenaComboBox();
        }

        public SeleccionaIngreso(DataGrid dataGrid)
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;

            LlenaComboBox();

            listaSeleccionados.Clear();
            //Si ya habia elementos en el datagrid, esos elementos se mantienen a menos que sea retirados aqui
            foreach (var item in dataGrid.Items.OfType<IngresaProductos>())
            {
                listaSeleccionados.Add(
                    new ProductosSeleccionados
                    {
                        Id = item.Id,
                        BarCode = item.BarCode,
                        ProductName = item.ProductName,
                        SalePricePercent = item.SalePricePercent,
                        SalePrice = item.SalePrice,
                        Amount = item.Amount,
                        PurchasePrice = item.PurchasePrice,
                        TotalPrice = item.TotalPrice,
                        Discount = item.Discount,
                        SubTotal = item.SubTotal,
                        Tax = item.Tax
                    }
                );
            }

            lv_productosSeleccionados.ItemsSource = listaSeleccionados;
        }

        /*
         * 
         * Funciones
         * 
         */

        public void LlenaListView()
        {
            lv_productos.ItemsSource = null;

            listaTodosProducto.Clear();

            listaTodosProducto = inventarioArticulos.LlenaArticulos(cb_almacen.SelectedItem.ToString(), checkBox);

            lv_productos.ItemsSource = listaTodosProducto;
        }

        public void BuscaProductos()
        {
            lv_productos.ItemsSource = null;

            listaTodosProducto.Clear();

            listaTodosProducto = inventarioArticulos.BuscaArticulos(cb_almacen.SelectedItem.ToString(), checkBox, txt_busca_producto.Text.ToUpper());

            lv_productos.ItemsSource = listaTodosProducto;
        }

        public void LlenaComboBox()
        {
            cb_almacen.Items.Add("TODOS");
            cb_almacen.SelectedItem = "TODOS";

            foreach (Store almacen in db.Stores)
            {
                cb_almacen.Items.Add(almacen.StoreName);
            }
        }

        /*
         * 
         * Acciones de los Botones, controladores, etc
         * 
         */

        //Boton para finalizar la seleccion
        private void btn_Finalizar_Click(object sender, RoutedEventArgs e)
        {
            Singleton.Instancia.listaIngresos = IngresaProductos.ListaIngresaProductos(listaSeleccionados);
            this.Close();
        }
        //Boton recargar lista todos los productos
        private void btn_Recargar_Click(object sender, RoutedEventArgs e)
        {
            LlenaListView();
        }
        //Agrega un producto a la lista de productos seleccionados
        private void lv_productos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LlenaSeleccionados();
        }
        //Agrega un producto a la lista de productos seleccionados
        private void lv_productos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LlenaSeleccionados();
            }
        }
        //Quita un producto seleccionado de la lista
        private void lv_productosSeleccionados_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ProductosSeleccionados selectedProducto = (ProductosSeleccionados)lv_productosSeleccionados.SelectedItem;

                if (selectedProducto != null)
                {
                    if (listaSeleccionados.Exists(ls => ls.Id.Equals(selectedProducto.Id)))
                    {
                        listaSeleccionados.Remove(selectedProducto);
                    }

                    lv_productosSeleccionados.ItemsSource = null;

                    lv_productosSeleccionados.ItemsSource = listaSeleccionados;
                }
            }
        }
        //ComboBox filtrar por almacen
        private void cb_almacen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LlenaListView();
        }
        //CheckBox "dispone stock"
        private void chckB_stock_Checked(object sender, RoutedEventArgs e)
        {
            checkBox = !checkBox;
            LlenaListView();
        }
        //TextBox busqueda
        private void txt_busca_producto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter & txt_busca_producto.Text != "" | e.Key == Key.Tab & txt_busca_producto.Text != "")
            {
                BuscaProductos();
            }
            if (e.Key == Key.Enter & txt_busca_producto.Text == null | e.Key == Key.Tab & txt_busca_producto.Text == null)
            {
                LlenaListView();
            }
        }
        //Llena lista seleccionados
        public void LlenaSeleccionados()
        {
            InventarioArticulos selectedProducto = (InventarioArticulos)lv_productos.SelectedItem;

            if (selectedProducto != null)
            {
                if (listaSeleccionados.Exists(ls => ls.Id.Equals(selectedProducto.Id)))
                {

                }
                else
                {
                    listaSeleccionados.Add(
                        new ProductosSeleccionados()
                        {
                            Id = selectedProducto.Id,
                            BarCode = selectedProducto.BarCode,
                            ProductName = selectedProducto.ProductName,
                            SalePricePercent = selectedProducto.SalePricePercent,
                            SalePrice = selectedProducto.SalePrice,
                            PurchasePrice = selectedProducto.PurchasePrice,
                            Amount = 1,
                            TotalPrice = selectedProducto.PurchasePrice,
                            Discount = 0,
                            SubTotal = 0,
                            Tax = 0
                        }
                    );
                }

                lv_productosSeleccionados.ItemsSource = null;

                lv_productosSeleccionados.ItemsSource = listaSeleccionados;
            }
        }
    }
}
