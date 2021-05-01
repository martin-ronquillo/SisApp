using System.Windows;

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
