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
    /// Lógica de interacción para ProveedoresCRUD.xaml
    /// </summary>
    public partial class ProveedoresCRUD : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        private int Id = 0;

        public ProveedoresCRUD()
        {
            InitializeComponent();

            int nPro = db.Providers.Count() + 1;
            txt_nProveedor.Text = nPro.ToString("D7");

            cbBox_tipoCuenta.Items.Add("AHORROS");
            cbBox_tipoCuenta.Items.Add("CORRIENTE");

            cbBox_tipoCuenta.SelectedItem = "CORRIENTE";
        }

        public ProveedoresCRUD(int Id)
        {
            InitializeComponent();

            txt_nProveedor.Text = Id.ToString("D7");

            this.Id = Id;

            var provider = db.Providers.First(pro => pro.Id.Equals(Id));

            txt_ruc.Text = provider.Ruc;
            txt_city.Text = provider.City;
            txt_providerName.Text = provider.ProviderName;
            txt_nombreVendedor.Text = provider.SalesManName;
            txt_telephoneOne.Text = provider.TelephoneOne;
            txt_telephoneTwo.Text = provider.TelephoneTwo;
            txt_telefonoVendedor.Text = provider.SalesManTelephone;
            txt_webSite.Text = provider.WebSite;
            txt_address.Text = provider.Address;
            txt_emailVendedor.Text = provider.SalesManEmail;
            txt_banco.Text = provider.Bank;
            cbBox_tipoCuenta.SelectedItem = provider.AccountType;
            txt_nombreCuenta.Text = provider.AccountName;
            txt_numeroCuenta.Text = provider.AccountNumber;

            cbBox_tipoCuenta.Items.Add("AHORROS");
            cbBox_tipoCuenta.Items.Add("CORRIENTE");
        }

        private void CreaProveedor()
        {
            try
            {
                Provider provider = new Provider
                {
                    Ruc = txt_ruc.Text.ToUpper(),
                    City = txt_city.Text.ToUpper(),
                    ProviderName = txt_providerName.Text.ToUpper(),
                    SalesManName = txt_nombreVendedor.Text.ToUpper(),
                    TelephoneOne = txt_telephoneOne.Text.ToUpper(),
                    TelephoneTwo = txt_telephoneTwo.Text.ToUpper(),
                    SalesManTelephone = txt_telefonoVendedor.Text.ToUpper(),
                    WebSite = txt_webSite.Text.ToUpper(),
                    Address = txt_address.Text.ToUpper(),
                    SalesManEmail = txt_emailVendedor.Text.ToUpper(),
                    Bank = txt_banco.Text.ToUpper(),
                    AccountType = cbBox_tipoCuenta.SelectedItem.ToString(),
                    AccountName = txt_nombreCuenta.Text.ToUpper(),
                    AccountNumber = txt_numeroCuenta.Text.ToUpper(),
                };

                db.Insert(provider);
            }
            catch(Exception e)
            {
                MessageBox.Show("Error al generar el ingreso: "+e);
            }
        }

        private void ActualizaProveedor()
        {
            try
            {
                var provider = db.Providers.First(pro => pro.Id.Equals(Id));

                provider.Ruc = txt_ruc.Text.ToUpper();
                provider.City = txt_city.Text.ToUpper();
                provider.ProviderName = txt_providerName.Text.ToUpper();
                provider.SalesManName = txt_nombreVendedor.Text.ToUpper();
                provider.TelephoneOne = txt_telephoneOne.Text.ToUpper();
                provider.TelephoneTwo = txt_telephoneTwo.Text.ToUpper();
                provider.SalesManTelephone = txt_telefonoVendedor.Text.ToUpper();
                provider.WebSite = txt_webSite.Text.ToUpper();
                provider.Address = txt_address.Text.ToUpper();
                provider.SalesManEmail = txt_emailVendedor.Text.ToUpper();
                provider.Bank = txt_banco.Text.ToUpper();
                provider.AccountType = cbBox_tipoCuenta.SelectedItem.ToString();
                provider.AccountName = txt_nombreCuenta.Text.ToUpper();
                provider.AccountNumber = txt_numeroCuenta.Text.ToUpper();

                db.Update(provider);
            }
            catch(Exception e)
            {
                MessageBox.Show("Error al actualizar: " + e);
            }
        }

        private void btn_guardar_Click(object sender, RoutedEventArgs e)
        {
            if (Id == 0)
            {
                CreaProveedor();
            }
            else
            {
                ActualizaProveedor();
            }

            this.Close();
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
