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
    /// Lógica de interacción para Proveedores.xaml
    /// </summary>
    public partial class Proveedores : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        List<ProveedoresController> listaProveedores = new List<ProveedoresController>();

        public Proveedores()
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;

            LlenaListView();
        }

        private void lv_proveedores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MuestraInfoProveedor();
        }

        private void lv_proveedores_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MuestraInfoProveedor();
            }
        }

        private void btn_nuevo_Click(object sender, RoutedEventArgs e)
        {
            ProveedoresCRUD proveedoresCRUD = new ProveedoresCRUD();
            proveedoresCRUD.ShowDialog();
            LlenaListView();
        }

        private void btn_editar_Click(object sender, RoutedEventArgs e)
        {
            ProveedoresController selectedItem = (ProveedoresController)lv_proveedores.SelectedItem;

            if (selectedItem != null)
            {
                ProveedoresCRUD proveedoresCRUD = new ProveedoresCRUD(selectedItem.Id);
                proveedoresCRUD.ShowDialog();
                LlenaListView();
            }
        }

        private void btn_eliminar_Click(object sender, RoutedEventArgs e)
        {
            ProveedoresController selectedItem = (ProveedoresController)lv_proveedores.SelectedItem;

            if (selectedItem != null)
            {
                string nombre = selectedItem.ProviderName;

                Confirmar confirmar = new Confirmar(nombre);

                confirmar.ShowDialog();

                if (Singleton.Instancia.confirma)
                {
                    var provider = db.Providers.First(pro => pro.Id.Equals(selectedItem.Id));

                    db.Delete(provider);

                    Singleton.Instancia.confirma = false;

                    LlenaListView();
                }
            }
        }

        public void MuestraInfoProveedor()
        {
            ProveedoresController selectedItem = (ProveedoresController)lv_proveedores.SelectedItem;

            var provider = db.Providers.First(pro => pro.Id.Equals(selectedItem.Id));

            txt_ruc.Text = provider.Ruc;
            txt_ciudad.Text = provider.City;
            txt_razonSocial.Text = provider.ProviderName;
            txt_vendedor.Text = provider.SalesManName;
            txt_telefonoOne.Text = provider.TelephoneOne;
            txt_telefonoTwo.Text = provider.TelephoneTwo;
            txt_telefonoVen.Text = provider.SalesManTelephone;
            txt_website.Text = provider.WebSite;
            txt_direccion.Text = provider.Address;
            txt_emailVendedor.Text = provider.SalesManEmail;
            txt_banco.Text = provider.Bank;
            txt_tipoCuenta.Text = provider.AccountType;
            txt_NombreCuenta.Text = provider.AccountName;
            txt_NumeroCuenta.Text = provider.AccountNumber;
        }

        public void LlenaListView()
        {
            listaProveedores.Clear();
            lv_proveedores.ItemsSource = null;

            foreach (var item in db.Providers)
            {
                listaProveedores.Add(
                    new ProveedoresController
                    {
                        Id = (int)item.Id,
                        ProviderName = item.ProviderName,
                        Ruc = item.Ruc,
                        City = item.City,
                        Address = item.Address,
                        TelephoneOne = item.TelephoneOne,
                        TelephoneTwo = item.TelephoneTwo,
                        WebSite = item.WebSite,
                        SalesManName = item.SalesManName,
                        SalesManTelephone = item.SalesManTelephone,
                        SalesManEmail = item.SalesManEmail,
                        Bank = item.Bank,
                        AccountType = item.AccountType,
                        AccountName = item.AccountName,
                        AccountNumber = item.AccountNumber
                    }
                );
            }

            lv_proveedores.ItemsSource = listaProveedores;
        }
    }
}
