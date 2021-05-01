using DataModels;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para Loggin.xaml
    /// </summary>
    public partial class Loggin : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        public Loggin()
        {
            InitializeComponent();
        }

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            IniciaSesion();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IniciaSesion();
            }
        }

        public void IniciaSesion()
        {
            try
            {
                var user = db.Users.First(us => us.LogUser.Equals(txtUsuario.Text));

                if (user.LogUser == txtUsuario.Text & user.Password == txtPassword.Password)
                {
                    Singleton.Instancia.estado = false;
                    Singleton.Instancia.idUser = (int)user.Id;

                    MainWindow mainWindow = new MainWindow();
                    Application.Current.MainWindow = mainWindow;

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Usuario o Contraseña incorrectos");
                    txtUsuario.Focus();
                }
            }
            catch
            {
                MessageBox.Show("Usuario o Contraseña incorrectos");
                txtUsuario.Focus();
            }
        }
    }
}
