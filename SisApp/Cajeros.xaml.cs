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
        List<Cajeros> listaCajeros = new List<Cajeros>();
        public string CheckerName { get; set; }
        public string Direction { get; set; }
        public string Store { get; set; }

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
            listaCajeros.Clear();
            lv_cajeros.ItemsSource = null;

            var cajeros = db.Cashiers.LoadWith(t => t.Store).Where(foo => foo.Store.StoreName.Equals(cbBox_almacen.SelectedItem.ToString()));

            foreach (var item in cajeros)
            {
                listaCajeros.Add(
                    new Cajeros
                    {
                        CheckerName = item.CheckerName,
                        Direction = item.Direction,
                        Store = item.Store.StoreName
                    }
                );
            }

            lv_cajeros.ItemsSource = listaCajeros;
        }
    }
}
