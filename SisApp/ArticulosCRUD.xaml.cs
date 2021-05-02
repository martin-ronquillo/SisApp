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
                int nPro = db.Products.Count() + 1;

                txt_nArticulo.Text = nPro.ToString("D7");

                foreach (Category category in db.Categories)
                {
                    cbBox_categoria.Items.Add(category.CategoryName);
                }

                cbBox_categoria.SelectedItem = "N/A";
            }
            catch
            {
                int nPro = 1;
                txt_nArticulo.Text = nPro.ToString("D7");
            }
        }

        public ArticulosCRUD(int Id)
        {
            InitializeComponent();

            Edit = true;

            lbl_Articulos.Content = "EDITANDO ARTICULO";

            try
            {
                Producto = db.Products.FirstOrDefault(pro => pro.Id.Equals(Id));

                txt_nArticulo.Text = Producto.Id.ToString("D7");

                foreach (Category category in db.Categories)
                {
                    cbBox_categoria.Items.Add(category.CategoryName);
                }

                cbBox_categoria.SelectedItem = db.Categories.FirstOrDefault(cat => cat.Id.Equals(Producto.CategoryId)).CategoryName;
                txt_codigoBarras.Text = Producto.BarCode;
                txt_descripcionArticulo.Text = Producto.ProductName;
            }
            catch(Exception e)
            {
                MessageBox.Show("Error, no se encontro el Producto \n Excepcion controlada: \n"+e);

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
        }

        public void GuardaInfo()
        {
            if (Edit)
            {
                Producto.BarCode = txt_codigoBarras.Text.ToUpper();
                Producto.CategoryId = db.Categories.FirstOrDefault(cat => cat.CategoryName.Equals(cbBox_categoria.SelectedItem)).Id;
                Producto.ProductName = txt_descripcionArticulo.Text.ToUpper();

                db.Update(Producto);
            }
            else
            {
                Product product = new Product
                {
                    ProductName = txt_descripcionArticulo.Text.ToUpper(),
                    BarCode = txt_codigoBarras.Text.ToUpper(),
                    CategoryId = db.Categories.FirstOrDefault(cat => cat.CategoryName.Equals(cbBox_categoria.SelectedItem)).Id,
                    Stock = 0,
                    SalePrice = 0,
                    PucharsePrice = 0,
                    TradeMarkId = 1
                };

                db.Insert(product);
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
    }
}
