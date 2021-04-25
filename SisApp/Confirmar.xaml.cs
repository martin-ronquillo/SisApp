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

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para Confirmar.xaml
    /// </summary>
    public partial class Confirmar : Window
    {

        public Confirmar(string nombre)
        {
            InitializeComponent();

            txt_eliminar.Text = "¿Esta seguro de eliminar el elemento: " + nombre + "?";

        }

        private void btn_confirmar_Click(object sender, RoutedEventArgs e)
        {
            Singleton.Instancia.confirma = true;

            this.Close();
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
