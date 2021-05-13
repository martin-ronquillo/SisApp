using DataModels;
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
using LinqToDB;
using System.Text.RegularExpressions;

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para ArticulosCRUD.xaml
    /// </summary>
    public partial class ArticulosCRUD : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        bool Validacion = true;
        bool Edit = false;
        Product Producto;

        public ArticulosCRUD()
        {
            InitializeComponent();

            lbl_Articulos.Content = "NUEVO ARTICULO";

            try
            {
                foreach (Category category in db.Categories)
                {
                    cbBox_categoria.Items.Add(category.CategoryName);
                }

                cbBox_categoria.SelectedItem = "N/A";

                foreach (TradeMark tradeMark in db.TradeMarks)
                {
                    cbBox_marca.Items.Add(tradeMark.MarkName);
                }

                cbBox_marca.SelectedItem = "N/A";
            }
            catch(Exception e)
            {
                MessageBox.Show("Excepcion controlada: \n"+e);

                this.Close();
            }

            cbBox_almacen.IsEnabled = false;
            txt_porcentajeGanancia.IsEnabled = false;
            txt_precioVenta.IsEnabled = false;
            txt_precioCompra.IsEnabled = false;

            txt_precioCompra.Visibility = Visibility.Collapsed;
            txt_precioVenta.Visibility = Visibility.Collapsed;
            txt_porcentajeGanancia.Visibility = Visibility.Collapsed;
        }

        public ArticulosCRUD(int Id)
        {
            InitializeComponent();

            txt_porcentajeGanancia.IsEnabled = false;
            txt_precioVenta.IsEnabled = false;

            Edit = true;

            lbl_Articulos.Content = "EDITANDO ARTICULO";

            try
            {
                Producto = db.Products.LoadWith(t => t.TradeMark).LoadWith(t => t.Category).LoadWith(t => t.ProductsStores).FirstOrDefault(pro => pro.Id.Equals(Id));

                foreach (Store store in db.Stores)
                {
                    cbBox_almacen.Items.Add(store.StoreName);
                }

                foreach (Category category in db.Categories)
                {
                    cbBox_categoria.Items.Add(category.CategoryName);
                }

                foreach (TradeMark tradeMark in db.TradeMarks)
                {
                    cbBox_marca.Items.Add(tradeMark.MarkName);
                }

                cbBox_marca.SelectedItem = Producto.TradeMark.MarkName;
                cbBox_categoria.SelectedItem = Producto.Category.CategoryName;
                txt_codigoBarras.Text = Producto.BarCode;
                txt_descripcionArticulo.Text = Producto.ProductName;
                txt_porcentajeGanancia.Text = "0";
                txt_precioCompra.Text = "0";
                txt_precioVenta.Text = "0";
            }
            catch (Exception e)
            {
                MessageBox.Show("Excepcion controlada: \n" + e);

                this.Close();
            }
        }

        private void btn_limpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarForm();
        }

        private void btn_guardar_Click(object sender, RoutedEventArgs e)
        {
            ValidaDatos();

            if (Validacion)
            {
                GuardaInfo();

                this.Close();
            }
            else
            {
                MessageBox.Show("No se guardaron los datos");
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /*
         * 
         * 
         * Metodos
         * 
         * 
         */
        public void LimpiarForm()
        {
            txt_codigoBarras.Text = "";
            txt_descripcionArticulo.Text = "";
            cbBox_categoria.SelectedItem = "N/A";
            cbBox_marca.SelectedItem = "N/A";
        }

        public void GuardaInfo()
        {
            try
            {
                if (cbBox_almacen.SelectedItem != null)
                {
                    Store store = db.Stores.FirstOrDefault(sto => sto.StoreName.Equals(cbBox_almacen.SelectedItem.ToString()));
                    ProductsStore productsStore = db.ProductsStores.FirstOrDefault(pro => pro.StoreId.Equals(store.Id) & pro.ProductId.Equals(Producto.Id));

                    productsStore.PriceByUnit = Math.Round(float.Parse(txt_precioVenta.Text), 2);
                    productsStore.SalePricePercent = Math.Round(float.Parse(txt_porcentajeGanancia.Text), 2);

                    db.Update(productsStore);
                }

                if (Edit)
                {
                    Producto.BarCode = txt_codigoBarras.Text.ToUpper();
                    Producto.CategoryId = db.Categories.FirstOrDefault(cat => cat.CategoryName.Equals(cbBox_categoria.SelectedItem)).Id;
                    Producto.ProductName = txt_descripcionArticulo.Text.ToUpper();
                    Producto.TradeMarkId = db.TradeMarks.FirstOrDefault(tm => tm.MarkName.Equals(cbBox_marca.SelectedItem)).Id;
                    Producto.SalePricePercent = Math.Round(float.Parse(txt_porcentajeGanancia.Text), 2);
                    Producto.SalePrice = Math.Round(float.Parse(txt_precioVenta.Text), 2);


                    db.Update(Producto);
                }
                else
                {
                    Product product = new Product
                    {
                        ProductName = txt_descripcionArticulo.Text.ToUpper(),
                        BarCode = txt_codigoBarras.Text.ToUpper(),
                        CategoryId = db.Categories.FirstOrDefault(cat => cat.CategoryName.Equals(cbBox_categoria.SelectedItem)).Id,
                        TradeMarkId = db.TradeMarks.FirstOrDefault(tm => tm.MarkName.Equals(cbBox_marca.SelectedItem)).Id,
                        SalePricePercent = 0,
                        Stock = 0,
                        SalePrice = 0,
                        PucharsePrice = 0,
                    };

                    db.Insert(product);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("No se pudo guardar la informacion. \n Excepcion controlada: "+e);
            }
        }

        public void ValidaDatos()
        {
            Validacion = true;

            if (txt_descripcionArticulo.Text == "")
            {
                Validacion = false;
            }
        }

        private void cbBox_almacen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Store store = db.Stores.FirstOrDefault(sto => sto.StoreName.Equals(cbBox_almacen.SelectedItem.ToString()));
                ProductsStore productsStore = db.ProductsStores.FirstOrDefault(pro => pro.StoreId.Equals(store.Id) & pro.ProductId.Equals(Producto.Id));

                txt_porcentajeGanancia.Text = productsStore.SalePricePercent.ToString();
                txt_precioVenta.Text = productsStore.PriceByUnit.ToString();
                txt_precioCompra.Text = productsStore.PurchasePrice.ToString();

                txt_porcentajeGanancia.IsEnabled = true;
                txt_precioVenta.IsEnabled = true;
            }
            catch
            {
                MessageBox.Show("El producto no posee inventario en el almacen seleccionado");
                txt_precioCompra.Text = "0";
                txt_precioVenta.Text = "";
                txt_porcentajeGanancia.Text = "";

                txt_porcentajeGanancia.IsEnabled = false;
                txt_precioVenta.IsEnabled = false;
            }
        }

        private void txt_porcentajeGanancia_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter | e.Key == Key.Tab)
            {
                string cadena = txt_porcentajeGanancia.Text;
                cadena = cadena.Replace(".", ",");
                float value;

                if (float.TryParse(cadena, out value))
                {
                    float valCompra = float.Parse(txt_precioCompra.Text);
                    float valVenta = (valCompra * value) + valCompra;

                    txt_porcentajeGanancia.Text = cadena;
                    txt_precioVenta.Text = valVenta.ToString();
                }
                else
                {
                    txt_porcentajeGanancia.Text = "0";
                }
            }
        }

        private void txt_precioVenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter | e.Key == Key.Tab)
            {
                string cadena = txt_precioVenta.Text;
                cadena = cadena.Replace(".", ",");
                float value;

                if (float.TryParse(cadena, out value))
                {
                    float valCompra = float.Parse(txt_precioCompra.Text);
                    float valVentaPercent = ((value - valCompra) / valCompra);

                    txt_precioVenta.Text = cadena;
                    txt_porcentajeGanancia.Text = valVentaPercent.ToString();
                }
            }
            else
            {
                txt_precioVenta.Text = "0";
            }
        }
    }
}
