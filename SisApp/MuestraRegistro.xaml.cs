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
    /// Lógica de interacción para MuestraRegistro.xaml
    /// </summary>
    public partial class MuestraRegistro : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        private int Id;
        private int TipoConsulta;

        List<ConsultaRegistroInfo> listaRegistros = new List<ConsultaRegistroInfo>();

        //Tipo consulta = 1, consulta de Ingresos. Else, consulta de Egresos
        public MuestraRegistro(int Id, int TipoConsulta)
        {
            InitializeComponent();

            this.Id = Id;
            this.TipoConsulta = TipoConsulta;

            LlenaInfo();

            LlenaListView();
        }

        public void LlenaListView()
        {
            if (TipoConsulta == 1)
            {
                var ingresos = db.ProductsReceipts.LoadWith(foo => foo.Product).Where(re => re.ReceiptId.Equals(Id));

                foreach (var item in ingresos)
                {
                    listaRegistros.Add(
                        new ConsultaRegistroInfo
                        {
                            Id = (int)item.Product.Id,
                            Code = item.Product.BarCode,
                            Product = item.Product.ProductName,
                            Amount = (float)item.Amount,
                            Purchase = (float)item.PurchasePrice,
                            Total = (float)item.Amount * (float)item.PurchasePrice
                        }
                    );
                }

                lv_registros.ItemsSource = listaRegistros;
            }
            else
            {
                var egresos = db.EgressProducts.LoadWith(foo => foo.Product).Where(re => re.EgressId.Equals(Id));

                foreach (var item in egresos)
                {
                    listaRegistros.Add(
                        new ConsultaRegistroInfo
                        {
                            Id = (int)item.Product.Id,
                            Code = item.Product.BarCode,
                            Product = item.Product.ProductName,
                            Amount = (float)item.Amount,
                            Purchase = (float)item.PurchasePrice,
                            Total = (float)item.Amount * (float)item.PurchasePrice
                        }
                    );
                }

                lv_registros.ItemsSource = listaRegistros;
            }
        }

        public void LlenaInfo()
        {
            if (TipoConsulta == 1)
            {
                var ingreso = db.Receipts.LoadWith(foo => foo.Store).First(foo => foo.Id.Equals(Id));

                lbl_perfilRegistro.Content = "INGRESO EN: " + ingreso.Store.StoreName;
                txt_nRegistro.Text = ingreso.ReceiptCode;
                txt_fechaIngreso.Text = ingreso.ReceiptDate;
                txt_tipoIngreso.Text = ingreso.Type;
                txt_totalIngreso.Text = ingreso.TotalPriceReceipt.ToString();
            }
            else
            {
                var egreso = db.Egresses.LoadWith(foo => foo.Store).First(foo => foo.Id.Equals(Id));

                lbl_perfilRegistro.Content = "EGRESO EN: " + egreso.Store.StoreName;
                lbl_fecha.Content = "Fecha de Egreso";
                lbl_tipo.Content = "Tipo de Egreso";
                lbl_total.Content = "V. Total Egresado";

                txt_nRegistro.Text = egreso.EgressCode;
                txt_fechaIngreso.Text = egreso.EgressDate;
                txt_tipoIngreso.Text = egreso.Type;
                txt_totalIngreso.Text = egreso.TotalPriceEgress.ToString();
            }
        }
    }
}
