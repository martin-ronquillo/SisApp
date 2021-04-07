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
using System.Configuration;
using System.Threading;

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para Loggin.xaml
    /// </summary>
    public partial class Loggin : Window
    {
        DataClasses1DataContext dataContext;

        public Loggin()
        {
            InitializeComponent();

            string miConexion = ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;

            dataContext = new DataClasses1DataContext(miConexion);
        }

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User user = dataContext.User.First(us => us.usuario.Equals(txtUsuario.Text));

                if (user.usuario == txtUsuario.Text & user.password == txtPassword.Password)
                {
                    Singleton.Instancia.estado = false;
                    Singleton.Instancia.idUser = user.Id;

                    MainWindow mainWindow = new MainWindow();
                    Application.Current.MainWindow = mainWindow;

                    this.Close();
                }
            }
            catch
            {
                MessageBox.Show("Usuario o Contraseña incorrectos");
            }
        }

    }
}
