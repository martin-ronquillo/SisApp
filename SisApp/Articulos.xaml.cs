using DataModels;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para Articulos.xaml
    /// </summary>
    public partial class Articulos : Window
    {
        public bool checkBox = false;

        SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        InventarioArticulos inventarioArticulos = new InventarioArticulos();

        List<InventarioArticulos> listaTodosProducto = new List<InventarioArticulos>();

        public Articulos()
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;

            if (LoggedUser.Rol != "ADMIN")
            {
                btn_EliminaArticulo.Visibility = Visibility.Collapsed;
                menuMantenimiento.Visibility = Visibility.Collapsed;
                menuCompras.Visibility = Visibility.Collapsed;
            }

            LlenaComboBox();
        }

        private void chckB_stock_Checked(object sender, RoutedEventArgs e)
        {
            checkBox = !checkBox;
            LlenaListView();
        }

        private void cb_almacen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LlenaListView();
            lbl_busquedaInfo.Content = "";
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

        private void txt_busca_producto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter & txt_busca_producto.Text != "" | e.Key == Key.Tab & txt_busca_producto.Text != "")
            {
                BuscaProductos();
            }
            if (e.Key == Key.Enter & txt_busca_producto.Text == null | e.Key == Key.Tab & txt_busca_producto.Text == null)
            {
                LlenaListView();
                lbl_busquedaInfo.Content = "";
            }
        }

        private void btn_Recargar_Click(object sender, RoutedEventArgs e)
        {
            LlenaListView();
            lbl_busquedaInfo.Content = "";
        }

        //Boton Agregar Producto
        private void btn_NuevoArticulo_Click(object sender, RoutedEventArgs e)
        {
            ArticulosCRUD articulosCRUD = new ArticulosCRUD();

            articulosCRUD.ShowDialog();

            LlenaListView();
        }

        //Boton Editar Producto
        private void btn_EditaArticulo_Click(object sender, RoutedEventArgs e)
        {
            EditaProducto();

            LlenaListView();
        }

        private void btn_EliminaArticulo_Click(object sender, RoutedEventArgs e)
        {
            EliminaProducto();

            LlenaListView();
        }

        /*
         * 
         * 
         * METODOS DE LA VENTANA
         * 
         * 
         */
        public void EditaProducto()
        {
            InventarioArticulos selectedProducto = (InventarioArticulos)lv_productos.SelectedItem;

            if (selectedProducto != null)
            {
                ArticulosCRUD articulosCRUD = new ArticulosCRUD(selectedProducto.Id);

                articulosCRUD.ShowDialog();
            }
        }

        public void EliminaProducto()
        {
            InventarioArticulos selectedProducto = (InventarioArticulos)lv_productos.SelectedItem;

            if (selectedProducto != null)
            {
                Confirmar confirmar = new Confirmar(selectedProducto.ProductName);

                confirmar.ShowDialog();

                if (Singleton.Instancia.confirma)
                {
                    var producto = db.Products.First(pro => pro.Id.Equals(selectedProducto.Id));

                    db.Delete(producto);

                    Singleton.Instancia.confirma = false;
                }
            }
        }

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

            if (chckB_stock.IsChecked == true & cb_almacen.SelectedItem.ToString() == "TODOS")
            {
                lbl_busquedaInfo.Content = "Buscando '" + txt_busca_producto.Text + "' en Todos los almacenes con Stock";
            }

            if (chckB_stock.IsChecked == false & cb_almacen.SelectedItem.ToString() == "TODOS")
            {
                lbl_busquedaInfo.Content = "Buscando '" + txt_busca_producto.Text + "' en Todos los almacenes";
            }

            if (chckB_stock.IsChecked == true & cb_almacen.SelectedItem.ToString() != "TODOS")
            {
                lbl_busquedaInfo.Content = "Buscando '" + txt_busca_producto.Text + "' en el almacen " + cb_almacen.SelectedItem.ToString() + " con stock disponible";
            }

            if (chckB_stock.IsChecked == false & cb_almacen.SelectedItem.ToString() != "TODOS")
            {
                lbl_busquedaInfo.Content = "Buscando '" + txt_busca_producto.Text + "' en el almacen " + cb_almacen.SelectedItem.ToString();
            }

            txt_busca_producto.Text = "";
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

        public void DatosProducto()
        {
            InventarioArticulos selectedProducto = (InventarioArticulos)lv_productos.SelectedItem;

            if (selectedProducto != null)
            {
                try
                {
                    Product producto = db.Products.First(pro => pro.Id.Equals(selectedProducto.Id));

                    try
                    {
                        Category categoria = db.Categories.First(cat => cat.Id.Equals(producto.CategoryId));

                        txt_categoria.Text = categoria.CategoryName;
                    }
                    catch
                    {
                        txt_categoria.Text = "N/A";
                    }

                    try
                    {
                        //Hace una consulta a la tabla relacional entre Stores y Products para poder acceder a sus datos (Tabla ProductsStores)
                        var query =
                            from almacenProducto in db.ProductsStores
                            where almacenProducto.StoreId == 1
                            where almacenProducto.ProductId == producto.Id
                            select new { AlmacenProducto = almacenProducto };

                        txt_enBodega.Text = query.First().AlmacenProducto.Stock.ToString();
                    }
                    catch
                    {
                        txt_enBodega.Text = "N/A";
                    }

                    try
                    {
                        TradeMark tradeMark = db.TradeMarks.First(tm => tm.Id.Equals(producto.TradeMarkId));

                        txt_marca.Text = tradeMark.MarkName;
                    }
                    catch
                    {
                        txt_marca.Text = "N/A";
                    }
                }
                catch
                {
                    txt_categoria.Text = "N/A";
                    txt_enBodega.Text = "N/A";
                    txt_marca.Text = "N/A";
                }

                txt_codigoBarra.Text = selectedProducto.BarCode;
                txt_existencias.Text = selectedProducto.Stock.ToString();
                txt_producto.Text = selectedProducto.ProductName;
                txt_precioCompra.Text = selectedProducto.PurchasePrice.ToString();
                txt_precioVenta.Text = selectedProducto.SalePrice.ToString();
            }
        }

        /*
         * 
         * 
         * Menu Actions
         * 
         * 
         */

        private void creaCategoria_Click(object sender, RoutedEventArgs e)
        {
            Categorias categorias = new Categorias();
            categorias.Show();
        }

        private void creaMarca_Click(object sender, RoutedEventArgs e)
        {
            Marcas marcas = new Marcas();
            marcas.Show();
        }

        private void ingresaArticulos_Click(object sender, RoutedEventArgs e)
        {
            IngresoArticulos ingresoArticulos = new IngresoArticulos();
            ingresoArticulos.Show();
        }

        private void consultaIngresos_Click(object sender, RoutedEventArgs e)
        {
            ConsultaIE consultaIE = new ConsultaIE(1);
            consultaIE.Show();
        }

        private void egresaArticulos_Click(object sender, RoutedEventArgs e)
        {
            EgresoArticulos egresoArticulos = new EgresoArticulos();
            egresoArticulos.Show();
        }

        private void consultaEgresos_Click(object sender, RoutedEventArgs e)
        {
            ConsultaIE consultaIE = new ConsultaIE(2);
            consultaIE.Show();
        }

        private void traspasoArticulos_Click(object sender, RoutedEventArgs e)
        {
            TraspasoArticulos traspasoArticulos = new TraspasoArticulos();
            traspasoArticulos.Show();
        }

        private void consultaTraspasos_Click(object sender, RoutedEventArgs e)
        {
            ConsultaIE consultaIE = new ConsultaIE(3);
            consultaIE.Show();
        }

        private void consultaProveedores_Click(object sender, RoutedEventArgs e)
        {
            Proveedores proveedores = new Proveedores();
            proveedores.Show();
        }

        private void ingresaCompras_Click(object sender, RoutedEventArgs e)
        {
            Compras compras = new Compras();
            compras.Show();
        }

        private void consultaCompras_Click(object sender, RoutedEventArgs e)
        {
            ConsultaIE consultaIE = new ConsultaIE(4);
            consultaIE.Show();
        }

        private void realizaDevolucion_Click(object sender, RoutedEventArgs e)
        {
            Devoluciones devoluciones = new Devoluciones("Compra");
            devoluciones.Show();
        }

        private void ingresaVenta_Click(object sender, RoutedEventArgs e)
        {
            Ventas ventas = new Ventas();
            ventas.Show();
        }

        private void consultaVenta_Click(object sender, RoutedEventArgs e)
        {
            ConsultaIE consultaIE = new ConsultaIE(5);
            consultaIE.Show();
        }

        private void realizaDevolucionVenta_Click(object sender, RoutedEventArgs e)
        {
            Devoluciones devoluciones = new Devoluciones("Venta");
            devoluciones.Show();
        }

        private void creaUsuario_Click(object sender, RoutedEventArgs e)
        {
            Usuarios usuarios = new Usuarios();
            usuarios.Show();
        }

        private void agregaCajero_Click(object sender, RoutedEventArgs e)
        {
            Cajeros cajeros = new Cajeros();
            cajeros.Show();
        }

        private void kardex_Click(object sender, RoutedEventArgs e)
        {

        }

        private void almacen_Click(object sender, RoutedEventArgs e)
        {
            Almacenes almacenes = new Almacenes();
            almacenes.Show();
        }
    }
}