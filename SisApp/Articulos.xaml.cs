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
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        public Articulos()
        {
            InitializeComponent();

            LlenaComboBox();

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
            List<Product> listaProducto = new List<Product>();

            if (cb_almacen.SelectedItem.ToString() == "TODOS")
            {
                foreach (Product producto in db.Products)
                {
                    //Si el checkBox "dispone stock" esta activo
                    if (chckB_stock.IsChecked == true)
                    {
                        if (producto.Stock != 0)
                        {
                            listaProducto.Add(
                                new Product()
                                {
                                    Id = (int)producto.Id,
                                    BarCode = producto.BarCode,
                                    ProductName = producto.ProductName,
                                    Stock = producto.Stock,
                                    SalePrice = (float)producto.SalePrice,
                                    PucharsePrice = producto.PucharsePrice,
                                }
                            );
                        }
                    }
                    else
                    {
                        listaProducto.Add(
                            new Product()
                            {
                                Id = (int)producto.Id,
                                BarCode = producto.BarCode,
                                ProductName = producto.ProductName,
                                Stock = producto.Stock,
                                SalePrice = (float)producto.SalePrice,
                                PucharsePrice = producto.PucharsePrice
                            }
                        );
                    }
                }
            }
            else
            {
                try
                {
                    List<ProductsStore> listaPs = new List<ProductsStore>();

                    listaPs = db.ProductsStores.Where(ps => ps.Store.StoreName.Equals(cb_almacen.SelectedItem.ToString())).ToList();

                    foreach (ProductsStore ps in listaPs)
                    {
                        Product producto = db.Products.First(pro => pro.Id.Equals(ps.ProductId));

                        //Si el checkBox "dispone stock" esta activo
                        if (chckB_stock.IsChecked == true)
                        {
                            if (producto.Stock != 0)
                            {
                                listaProducto.Add(
                                    new Product()
                                    {
                                        Id = (int)producto.Id,
                                        BarCode = producto.BarCode,
                                        ProductName = producto.ProductName,
                                        Stock = producto.Stock,
                                        SalePrice = (float)producto.SalePrice,
                                        PucharsePrice = producto.PucharsePrice
                                    }
                                );
                            }
                        }
                        else
                        {
                            listaProducto.Add(
                                new Product()
                                {
                                    Id = (int)producto.Id,
                                    BarCode = producto.BarCode,
                                    ProductName = producto.ProductName,
                                    Stock = producto.Stock,
                                    SalePrice = (float)producto.SalePrice,
                                    PucharsePrice = producto.PucharsePrice,
                                }
                            );
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error al llenar la vista \n Exception: \n" + e);
                }
            }

            lv_productos.ItemsSource = listaProducto;
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
            Product selectedProducto = (Product)lv_productos.SelectedItem;

            if (selectedProducto != null)
            {
                try
                {
                    Product producto = db.Products.First(pro => pro.Id.Equals(selectedProducto.Id));

                    Category categoria = db.Categories.First(cat => cat.Id.Equals(producto.CategoryId));

                    txt_categoria.Text = categoria.CategoryName;

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
                }
                catch
                {
                    txt_categoria.Text = "N/A";
                }

                txt_codigoBarra.Text = selectedProducto.BarCode;
                txt_existencias.Text = selectedProducto.Stock.ToString();
                txt_producto.Text = selectedProducto.ProductName;
                txt_precioCompra.Text = selectedProducto.PucharsePrice.ToString();
                txt_precioVenta.Text = selectedProducto.SalePrice.ToString();
            }
        }

        public void BuscaProductos()
        {
            List<Product> listaProducto = new List<Product>();

            try
            {
                var productos = db.Products.Where(pro => pro.ProductName.Contains(txt_busca_producto.Text.ToUpper()));

                if (productos.Count() != 0)
                {
                    if (cb_almacen.SelectedItem.ToString() == "TODOS")
                    {
                        foreach (Product producto in productos)
                        {
                            //Si el checkBox "dispone stock" esta activo
                            if (chckB_stock.IsChecked == true)
                            {
                                if (producto.Stock != 0)
                                {
                                    listaProducto.Add(
                                        new Product()
                                        {
                                            Id = (int)producto.Id,
                                            BarCode = producto.BarCode,
                                            ProductName = producto.ProductName,
                                            Stock = producto.Stock,
                                            SalePrice = (float)producto.SalePrice,
                                            PucharsePrice = producto.PucharsePrice,
                                        }
                                    );
                                }

                                lbl_busquedaInfo.Content = "Buscando '" + txt_busca_producto.Text + "' en Todos los almacenes con stock disponible";
                            }
                            else
                            {
                                listaProducto.Add(
                                    new Product()
                                    {
                                        Id = (int)producto.Id,
                                        BarCode = producto.BarCode,
                                        ProductName = producto.ProductName,
                                        Stock = producto.Stock,
                                        SalePrice = (float)producto.SalePrice,
                                        PucharsePrice = producto.PucharsePrice
                                    }
                                );

                                lbl_busquedaInfo.Content = "Buscando '" + txt_busca_producto.Text + "' en Todos los almacenes";
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            List<ProductsStore> listaPs = new List<ProductsStore>();
                            Store almacen = db.Stores.First(alm => alm.StoreName.Equals(cb_almacen.SelectedItem.ToString()));

                            //Busca en los productos y los almacenes relacionados en la tabla ProductsStores
                            /*var query =
                            from almacenProducto in db.ProductsStores
                            where almacenProducto.StoreId == almacen.Id
                            select new { AlmacenProducto = almacenProducto };*/

                            listaPs = db.ProductsStores.Where(ps => ps.StoreId.Equals(almacen.Id)).ToList();

                            foreach (Product product in productos)
                            {
                                foreach (ProductsStore productsStore in listaPs)
                                {
                                    if (product.Id == productsStore.ProductId)
                                    {
                                        //Si el checkBox "dispone stock" esta activo
                                        if (chckB_stock.IsChecked == true)
                                        {
                                            if (product.Stock != 0)
                                            {
                                                listaProducto.Add(
                                                    new Product()
                                                    {
                                                        Id = (int)product.Id,
                                                        BarCode = product.BarCode,
                                                        ProductName = product.ProductName,
                                                        Stock = product.Stock,
                                                        SalePrice = (float)product.SalePrice,
                                                        PucharsePrice = product.PucharsePrice
                                                    }
                                                );
                                            }

                                            lbl_busquedaInfo.Content = "Buscando '" + txt_busca_producto.Text + "' en el almacen " + cb_almacen.SelectedItem.ToString() + " con stock disponible";
                                        }
                                        else
                                        {
                                            listaProducto.Add(
                                                new Product()
                                                {
                                                    Id = (int)product.Id,
                                                    BarCode = product.BarCode,
                                                    ProductName = product.ProductName,
                                                    Stock = product.Stock,
                                                    SalePrice = (float)product.SalePrice,
                                                    PucharsePrice = product.PucharsePrice,
                                                }
                                            );

                                            lbl_busquedaInfo.Content = "Buscando '" + txt_busca_producto.Text + "' en el almacen " + cb_almacen.SelectedItem.ToString();
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Error al llenar la vista \n Exception: \n" + e);
                        }
                    }
                    txt_busca_producto.Text = "";

                    lv_productos.ItemsSource = listaProducto;
                }
                else
                {
                    lv_productos.ItemsSource = null;
                }
            }
            catch
            {
                MessageBox.Show("No se ha encontrado ninguna coincidencia");
            }
        }
    }
}
