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
    /// Lógica de interacción para Almacenes.xaml
    /// </summary>
    public partial class Almacenes : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        List<AlmacenesController> listaAlmacenes = new List<AlmacenesController>();

        public Almacenes()
        {
            InitializeComponent();

            listaAlmacenes.Clear();
            lv_almacen.ItemsSource = null;

            LlenaLista();
        }

        private void btn_agregar_Click(object sender, RoutedEventArgs e)
        {
            AlmacenesCRUD almacenesCRUD = new AlmacenesCRUD();
            almacenesCRUD.ShowDialog();
            LlenaLista();
        }

        private void btn_editar_Click(object sender, RoutedEventArgs e)
        {
            AlmacenesController selectedItem = (AlmacenesController)lv_almacen.SelectedItem;

            if (selectedItem != null)
            {
                AlmacenesCRUD almacenesCRUD = new AlmacenesCRUD(selectedItem.Id);
                almacenesCRUD.ShowDialog();
            }

            LlenaLista();
        }

        private void btn_eliminar_Click(object sender, RoutedEventArgs e)
        {
            AlmacenesController selectedItem = (AlmacenesController)lv_almacen.SelectedItem;

            if (selectedItem != null)
            {
                Confirmar confirmar = new Confirmar(selectedItem.StoreName);

                confirmar.ShowDialog();

                if (Singleton.Instancia.confirma)
                {
                    var store = db.Stores.First(sto => sto.Id.Equals(selectedItem.Id));

                    db.Delete(store);

                    Singleton.Instancia.confirma = false;
                }
            }

            LlenaLista();
        }

        public void LlenaLista()
        {
            listaAlmacenes.Clear();
            lv_almacen.ItemsSource = null;

            foreach (var item in db.Stores)
            {
                listaAlmacenes.Add(
                    new AlmacenesController
                    {
                        Id = (int)item.Id,
                        Ruc = item.Ruc,
                        Address = item.Direction,
                        StoreName = item.StoreName,
                        Telephone = item.Telephone,
                        Email = item.Email
                    }
                );
            }

            lv_almacen.ItemsSource = listaAlmacenes;
        }
    }

    public class AlmacenesController
    {
        public int Id { get; set; }
        public string Ruc { get; set; }
        public string Address { get; set; }
        public string StoreName { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
    }
}
