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
    /// Lógica de interacción para Cajeros.xaml
    /// </summary>
    public partial class Cajeros : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        List<CajerosController> listaCajeros = new List<CajerosController>();

        public Cajeros()
        {
            InitializeComponent();

            listaCajeros.Clear();
            lv_cajeros.ItemsSource = null;

            foreach (var item in db.Stores)
            {
                cbBox_almacen.Items.Add(item.StoreName);
            }
        }

        private void cbBox_almacen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LlenaLista();
        }

        private void btn_agregar_Click(object sender, RoutedEventArgs e)
        {
            CajerosCRUD cajerosCRUD = new CajerosCRUD();
            cajerosCRUD.ShowDialog();
            LlenaLista();
        }

        private void btn_editar_Click(object sender, RoutedEventArgs e)
        {
            CajerosController selectedItem = (CajerosController)lv_cajeros.SelectedItem;

            if (selectedItem != null)
            {
                CajerosCRUD cajerosCRUD = new CajerosCRUD(selectedItem.Id);
                cajerosCRUD.ShowDialog();
            }
            LlenaLista();
        }

        private void btn_eliminar_Click(object sender, RoutedEventArgs e)
        {
            CajerosController selectedItem = (CajerosController)lv_cajeros.SelectedItem;

            if (selectedItem != null)
            {
                Confirmar confirmar = new Confirmar(selectedItem.CheckerName);

                confirmar.ShowDialog();

                if (Singleton.Instancia.confirma)
                {
                    var cashier = db.Cashiers.First(cas => cas.Id.Equals(selectedItem.Id));

                    if (cashier.CheckerName != Environment.MachineName)
                    {
                        db.Delete(cashier);
                    }
                    else
                    {
                        MessageBox.Show("Actualmente se esta usando el cajero " + cashier.CheckerName + " por lo que no es posible eliminarlo.");
                    }

                    Singleton.Instancia.confirma = false;
                }
            }
            LlenaLista();
        }

        public void LlenaLista()
        {
            listaCajeros.Clear();
            lv_cajeros.ItemsSource = null;

            var cajeros = db.Cashiers.LoadWith(t => t.Store).Where(foo => foo.Store.StoreName.Equals(cbBox_almacen.SelectedItem.ToString()));

            foreach (var item in cajeros)
            {
                listaCajeros.Add(
                    new CajerosController
                    {
                        Id = (int)item.Id,
                        CheckerName = item.CheckerName,
                        Direction = item.Direction,
                        Store = item.Store.StoreName
                    }
                );
            }

            lv_cajeros.ItemsSource = listaCajeros;
        }
    }

    public class CajerosController
    {
        public int Id { get; set; }
        public string CheckerName { get; set; }
        public string Direction { get; set; }
        public string Store { get; set; }
    }
}
