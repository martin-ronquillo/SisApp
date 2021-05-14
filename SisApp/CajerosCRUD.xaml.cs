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
    /// Lógica de interacción para CajerosCRUD.xaml
    /// </summary>
    public partial class CajerosCRUD : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        private int Id = 0;

        public CajerosCRUD()
        {
            InitializeComponent();

            LlenaCombo();

            txt_cashier.IsEnabled = false;
        }

        public CajerosCRUD(int Id)
        {
            this.Id = Id;

            InitializeComponent();

            LlenaCombo();

            LlenaInfo();

            txt_cashier.IsEnabled = false;
        }

        private void btn_Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (Valida())
            {
                try
                {
                    if (Id == 0)
                    {
                        Cashier cashier = new Cashier
                        {
                            CheckerName = txt_cashier.Text,
                            Direction = txt_address.Text,
                            StoreId = db.Stores.First(sto => sto.StoreName.Equals(cbBox_store.SelectedItem.ToString())).Id
                        };

                        db.Insert(cashier);
                    }
                    else
                    {
                        var cashier = db.Cashiers.First(cas => cas.Id.Equals(Id));

                        cashier.Direction = txt_address.Text.ToUpper();
                        cashier.StoreId = db.Stores.First(sto => sto.StoreName.Equals(cbBox_store.SelectedItem.ToString())).Id;

                        db.Update(cashier);
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show("No se puede guardar la informacion. \nExcepcion controlada: " + exc);
                }

                this.Close();
            }
        }

        private void btn_Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void LlenaCombo()
        {
            foreach (var item in db.Stores)
            {
                cbBox_store.Items.Add(item.StoreName);
            }
        }

        public void LlenaInfo()
        {
            try
            {
                var cashier = db.Cashiers.LoadWith(foo => foo.Store).First(cas => cas.Id.Equals(Id));
                //var store = db.Stores.First(sto => sto.StoreName.Equals(cbBox_store.SelectedItem.ToString()));

                txt_cashier.Text = cashier.CheckerName;
                txt_address.Text = cashier.Direction;
                cbBox_store.SelectedItem = cashier.Store.StoreName;
            }
            catch
            {
                //Nothing here
            }
        }

        public bool Valida()
        {
            bool valida = true;

            if (cbBox_store.SelectedItem.ToString() == "" | cbBox_store.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un almacen");
                valida = false;
            }

            return valida;
        }
    }
}
