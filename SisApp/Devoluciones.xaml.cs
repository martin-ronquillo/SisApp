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
    /// Lógica de interacción para Devoluciones.xaml
    /// </summary>
    public partial class Devoluciones : Window
    {
        string Type;
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        DevolucionesController devoluciones = new DevolucionesController();
        List<DevolucionesController> listaDevoluciones = new List<DevolucionesController>();

        public Devoluciones(string Type)
        {
            InitializeComponent();

            devoluciones.listaDevoluciones.Clear();
            lv_registros.ItemsSource = null;

            if (Type == "Compra")
            {
                this.Type = Type;

                lbl_Devolucion.Content = "DEVOLVER COMPRA";
            }

            if (Type == "Venta")
            {
                this.Type = Type;

                lbl_Devolucion.Content = "DEVOLVER VENTA";
            }

            WindowState = WindowState.Maximized;
        }

        private void btn_consulta_Click(object sender, RoutedEventArgs e)
        {
            devoluciones.listaDevoluciones.Clear();
            lv_registros.ItemsSource = null;
            lv_registros.ItemsSource = devoluciones.listaRegistros(Type, dp_fechaConsulta.Text);
        }

        private void lv_registros_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DevolucionesController selectedItem = (DevolucionesController)lv_registros.SelectedItem;

            DevolucionDetalle devolucionDetalle = new DevolucionDetalle(Type, selectedItem.Id);
            devolucionDetalle.ShowDialog();

            devoluciones.listaDevoluciones.Clear();
            lv_registros.ItemsSource = null;
            lv_registros.ItemsSource = devoluciones.listaRegistros(Type, dp_fechaConsulta.Text);
        }

        private void lv_registros_KeyDown(object sender, KeyEventArgs e)
        {
            DevolucionesController selectedItem = (DevolucionesController)lv_registros.SelectedItem;

            DevolucionDetalle devolucionDetalle = new DevolucionDetalle(Type, selectedItem.Id);
            devolucionDetalle.ShowDialog();

            devoluciones.listaDevoluciones.Clear();
            lv_registros.ItemsSource = null;
            lv_registros.ItemsSource = devoluciones.listaRegistros(Type, dp_fechaConsulta.Text);
        }
    }
}
