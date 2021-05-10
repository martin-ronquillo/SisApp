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
    /// Lógica de interacción para DevolucionDetalle.xaml
    /// </summary>
    public partial class DevolucionDetalle : Window
    {
        readonly SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        DetalleDevolucion detalleDevolucion = new DetalleDevolucion();
        List<DetalleDevolucion> listaProductos = new List<DetalleDevolucion>();

        private string Type;
        private int Id;

        public DevolucionDetalle(string Type, int Id)
        {
            InitializeComponent();

            this.Type = Type;
            this.Id = Id;

            if (Type == "Compra")
            {
                lbl_efectivo.Visibility = Visibility.Collapsed;
                txt_efectivo.Visibility = Visibility.Collapsed;
                lbl_cambio.Visibility = Visibility.Collapsed;
                txt_cambio.Visibility = Visibility.Collapsed;
            }

            if (Type == "Venta")
            {
                lbl_proveedor.Visibility = Visibility.Collapsed;
                txt_proveedor.Visibility = Visibility.Collapsed;
            }

            LlenaInfo();

            detalleDevolucion.listaDetalleDevolucion.Clear();
            lv_registros.ItemsSource = null;
            lv_registros.ItemsSource = detalleDevolucion.detalleDevolucion(Type, Id);
        }

        private void btn_devolver_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            string RefoundCode = r.Next(1000, 9999999).ToString("D7");

            if (Type == "Compra")
            {
                try
                {
                    PurchasesRefund purchasesRefund = new PurchasesRefund
                    {
                        RefoundCode = RefoundCode,
                        RefoundDate = DateTime.Today.ToString(),
                        RefoundType = Type.ToUpper(),
                        PurchaseId = Id
                    };

                    db.Insert(purchasesRefund);

                    this.Close();
                }
                catch(Exception exc)
                {
                    MessageBox.Show("No se pudo realizar la devolucion, excepcion controlada: "+exc);
                }
            }

            if (Type == "Venta")
            {
                try
                {
                    SalesRefund salesRefund = new SalesRefund
                    {
                        RefoundCode = RefoundCode,
                        RefoundDate = DateTime.Today.ToString(),
                        RefoundType = Type.ToUpper(),
                        SalesId = Id
                    };

                    db.Insert(salesRefund);

                    this.Close();
                }
                catch (Exception exc)
                {
                    MessageBox.Show("No se pudo realizar la devolucion, excepcion controlada: " + exc);
                }
            }
        }

        public void LlenaInfo()
        {
            if (Type == "Compra")
            {
                var registro = db.Purchases.LoadWith(t => t.Provider).First(foo => foo.Id.Equals(Id));

                txt_codigo.Text = registro.PurchaseCode;
                txt_fecha.Text = registro.PurchaseDate;
                txt_proveedor.Text = registro.Provider.ProviderName;
                txt_subTotal.Text = registro.SubPrice.ToString();
                txt_descuento.Text = registro.Discount.ToString();
                txt_iva.Text = registro.Tax.ToString();
                txt_valTotal.Text = registro.TotalPrice.ToString();
            }

            if (Type == "Venta")
            {
                var registro = db.Sales.First(foo => foo.Id.Equals(Id));

                txt_codigo.Text = registro.Id.ToString("D7");
                txt_fecha.Text = registro.SaleDate;
                txt_efectivo.Text = registro.Cash.ToString();
                txt_cambio.Text = registro.RemainingCash.ToString();
                txt_subTotal.Text = registro.SubPrice.ToString();
                txt_descuento.Text = registro.Discount.ToString();
                txt_iva.Text = registro.Tax.ToString();
                txt_valTotal.Text = registro.TotalPrice.ToString();
            }
        }
    }
}
