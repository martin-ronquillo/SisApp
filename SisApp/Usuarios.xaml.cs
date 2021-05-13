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
    /// Lógica de interacción para Usuarios.xaml
    /// </summary>
    public partial class Usuarios : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        List<UsuariosController> listaUsuarios = new List<UsuariosController>();

        public Usuarios()
        {
            InitializeComponent();

            listaUsuarios.Clear();
            lv_usuarios.ItemsSource = null;

            LlenaLista();
        }

        private void btn_agregar_Click(object sender, RoutedEventArgs e)
        {
            UsuariosCRUD usuariosCRUD = new UsuariosCRUD();
            usuariosCRUD.ShowDialog();
            LlenaLista();
        }

        private void btn_editar_Click(object sender, RoutedEventArgs e)
        {
            UsuariosController selectedItem = (UsuariosController)lv_usuarios.SelectedItem;

            if (selectedItem != null)
            {
                UsuariosCRUD usuariosCRUD = new UsuariosCRUD(selectedItem.Id);
                usuariosCRUD.ShowDialog();
                LlenaLista();
            }
        }

        private void btn_eliminar_Click(object sender, RoutedEventArgs e)
        {
            UsuariosController selectedItem = (UsuariosController)lv_usuarios.SelectedItem;

            if (selectedItem != null)
            {
                Confirmar confirmar = new Confirmar(selectedItem.NameLastname);

                confirmar.ShowDialog();

                if (Singleton.Instancia.confirma)
                {
                    var user = db.Users.First(us => us.Id.Equals(selectedItem.Id));

                    db.Delete(user);

                    Singleton.Instancia.confirma = false;
                }
            }

            LlenaLista();
        }

        public void LlenaLista()
        {
            listaUsuarios.Clear();
            lv_usuarios.ItemsSource = null;

            var usuarios = db.Users.LoadWith(t => t.Role);

            foreach (var item in usuarios)
            {
                listaUsuarios.Add(
                    new UsuariosController
                    {
                        Id = (int)item.Id,
                        Ci = item.Ci,
                        NameLastname = item.Name + " " + item.LastName,
                        Role = item.Role.RolColumn,
                        LoggUser = item.LogUser,
                        Password = "*****"
                    }
                );
            }

            lv_usuarios.ItemsSource = listaUsuarios;
        }
    }

    public class UsuariosController
    {
        public int Id { get; set; }
        public string Ci { get; set; }
        public string NameLastname { get; set; }
        public string Role { get; set; }
        public string LoggUser { get; set; }
        public string Password { get; set; }
    }
}
