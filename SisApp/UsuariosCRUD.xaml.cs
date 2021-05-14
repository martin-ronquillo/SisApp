using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para UsuariosCRUD.xaml
    /// </summary>
    public partial class UsuariosCRUD : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        private int Id = 0;

        public UsuariosCRUD()
        {
            InitializeComponent();

            LlenaCombo();
        }

        public UsuariosCRUD(int Id)
        {
            InitializeComponent();

            this.Id = Id;

            LlenaCombo();

            LlenaInfo();
        }

        private void btn_Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (Valida())
            {
                if (Id == 0)
                {
                    User user = new User
                    {
                        Ci = txt_cedula.Text,
                        Name = txt_name.Text.ToUpper(),
                        LastName = txt_lastname.Text.ToUpper(),
                        LogUser = txt_perfil.Text.ToUpper(),
                        Password = txt_password.Password,
                        RoleId = db.Rols.First(foo => foo.RolColumn.Equals(cbBox_role.SelectedItem.ToString())).Id
                    };

                    db.Insert(user);

                    this.Close();
                }
                else
                {
                    try
                    {
                        var usuario = db.Users.LoadWith(t => t.Role).FirstOrDefault(foo => foo.Id.Equals(Id));

                        usuario.Ci = txt_cedula.Text;
                        usuario.Name = txt_name.Text.ToUpper();
                        usuario.LastName = txt_lastname.Text.ToUpper();
                        usuario.LogUser = txt_perfil.Text.ToUpper();
                        usuario.Password = txt_password.Password;
                        usuario.RoleId = db.Rols.First(foo => foo.RolColumn.Equals(cbBox_role.SelectedItem.ToString())).Id;

                        db.Update(usuario);

                        this.Close();
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("No se pudo actualizar los datos. \nExcepcion controlada: \n" + exc);
                    }
                }
            }
        }

        private void btn_Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void LlenaCombo()
        {
            foreach (var item in db.Rols)
            {
                cbBox_role.Items.Add(item.RolColumn);
            }
        }

        public void LlenaInfo()
        {
            try
            {
                var usuario = db.Users.LoadWith(t => t.Role).FirstOrDefault(foo => foo.Id.Equals(Id));

                cbBox_role.SelectedItem = usuario.Role.RolColumn;

                txt_cedula.Text = usuario.Ci;
                txt_name.Text = usuario.Name;
                txt_lastname.Text = usuario.LastName;
                txt_perfil.Text = usuario.LogUser;
                txt_password.Password = usuario.Password;
            }
            catch (Exception e)
            {
                MessageBox.Show("No se puede editar el usuario. \n Excepcion controlada: \n" + e);
            }
        }

        public bool Valida()
        {
            bool valida = true;

            if (Regex.IsMatch(txt_cedula.Text, "[^0-9]") | string.IsNullOrEmpty(txt_cedula.Text) | txt_cedula.Text.Length != 10 & txt_cedula.Text.Length != 13)
            {
                MessageBox.Show("El campo 'Cedula' es invalido");

                valida = false;
            }
            //Valida el nombre
            if (!Regex.IsMatch(txt_name.Text, "[^0-9]") | string.IsNullOrEmpty(txt_name.Text))
            {
                MessageBox.Show("El campo 'Nombre' solo puede contener letras");

                valida = false;
            }
            //Valida el apellido
            if (!Regex.IsMatch(txt_lastname.Text, "[^0-9]") | string.IsNullOrEmpty(txt_lastname.Text))
            {
                MessageBox.Show("El campo 'Apellido' solo puede contener letras");

                valida = false;
            }
            //Valida el nombre de usuario
            if (string.IsNullOrEmpty(txt_perfil.Text))
            {
                MessageBox.Show("El campo 'Perfil' esta vacio");

                valida = false;
            }
            //Valida la contraseña
            if (string.IsNullOrEmpty(txt_password.Password))
            {
                MessageBox.Show("El campo 'Contraseña' esta vacio");

                valida = false;
            }
            //Valida el comboBox
            if (cbBox_role.SelectedItem.ToString() == "" & cbBox_role.SelectedItem != null)
            {
                MessageBox.Show("Seleccione un privilegio");

                valida = false;
            }

            return valida;
        }
    }
}
