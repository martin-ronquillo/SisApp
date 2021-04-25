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

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para AgregaCliente.xaml
    /// </summary>
    public partial class AgregaCliente : Window
    {
        bool validacion = true;
        bool editClient = false;
        int clienteId = 0;

        public static string miConexion = System.Configuration.ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext = new DataClasses1DataContext(miConexion);

        public AgregaCliente()
        {
            InitializeComponent();

            LimpiaForm();

            try
            {
                Cliente cliente = dataContext.Cliente.OrderByDescending(cli => cli.Id).FirstOrDefault();

                int nCliente = cliente.Id + 1;

                txt_nCliente.Text = nCliente.ToString("D7");
            }
            catch
            {
                int nCliente = 1;
                txt_nCliente.Text = nCliente.ToString("D7");

            }
        }

        public AgregaCliente(int id)
        {
            InitializeComponent();

            LimpiaForm();

            editClient = true;

            Cliente cliente = dataContext.Cliente.First(cli => cli.Id.Equals(id));

            txt_nCliente.Text = "Editando";

            clienteId = cliente.Id;
            txt_cedula_cliente.Text = cliente.Cedula;
            txt_nombre_cliente.Text = cliente.Nombre;
            txt_apellido_cliente.Text = cliente.Apellido;
            txt_direccion_cliente.Text = cliente.Direccion;
            txt_email_cliente.Text = cliente.Email;
            txt_telefono_cliente.Text = cliente.Telefono;
        }

        private void btn_guardar_cliente_Click(object sender, RoutedEventArgs e)
        {
            ValidaForm();

            if (validacion)
            {
                if (editClient)
                {
                    Cliente cliente = dataContext.Cliente.First(cli => cli.Id.Equals(clienteId));

                    cliente.Cedula = txt_cedula_cliente.Text;
                    cliente.Nombre = txt_nombre_cliente.Text.ToUpper();
                    cliente.Apellido = txt_apellido_cliente.Text.ToUpper();
                    cliente.Direccion = txt_direccion_cliente.Text.ToUpper();
                    cliente.Email = txt_email_cliente.Text.ToUpper();
                    cliente.Telefono = txt_telefono_cliente.Text;

                    dataContext.SubmitChanges();

                    this.Close();
                }
                else
                {
                    Cliente cliente = new Cliente
                    {
                        Cedula = txt_cedula_cliente.Text,
                        Nombre = txt_nombre_cliente.Text.ToUpper(),
                        Apellido = txt_apellido_cliente.Text.ToUpper(),
                        Direccion = txt_direccion_cliente.Text.ToUpper(),
                        Email = txt_email_cliente.Text.ToUpper(),
                        Telefono = txt_telefono_cliente.Text
                    };

                    dataContext.Cliente.InsertOnSubmit(cliente);

                    dataContext.SubmitChanges();

                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("No se guardaron los datos");
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_limpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiaForm();
        }

        public void LimpiaForm()
        {
            txt_cedula_cliente.Text = null;
            txt_nombre_cliente.Text = null;
            txt_apellido_cliente.Text = null;
            txt_direccion_cliente.Text = null;
            txt_email_cliente.Text = null;
            txt_telefono_cliente.Text = null;
        }

        public void ValidaForm()
        {
            validacion = true;

            //Valida la cedula
            if (Regex.IsMatch(txt_cedula_cliente.Text, "[^0-9]") | string.IsNullOrEmpty(txt_cedula_cliente.Text) | txt_cedula_cliente.Text.Length > 10)
            {
                MessageBox.Show("El campo 'Cedula' debe contener 10 numeros");

                validacion = false;
            }

            //Valida el nombre
            if (!Regex.IsMatch(txt_nombre_cliente.Text, "[^0-9]") | string.IsNullOrEmpty(txt_nombre_cliente.Text))
            {
                MessageBox.Show("El campo 'Nombre' solo puede contener letras");

                validacion = false;
            }

            //Valida el apellido
            if (!Regex.IsMatch(txt_apellido_cliente.Text, "[^0-9]") | string.IsNullOrEmpty(txt_apellido_cliente.Text))
            {
                MessageBox.Show("El campo 'Apellido' solo puede contener letras");

                validacion = false;
            }

            //Valida direccion
            if (!Regex.IsMatch(txt_direccion_cliente.Text, "[^0-9]"))
            {
                if (string.IsNullOrEmpty(txt_direccion_cliente.Text))
                {

                }
                else
                {
                    MessageBox.Show("El campo 'Direccion' solo puede contener letras");

                    validacion = false;
                }
            }

            //Valida Email
            if (!Regex.IsMatch(txt_email_cliente.Text, "[^0-9]"))
            {
                if (string.IsNullOrEmpty(txt_email_cliente.Text))
                {

                }
                else
                {
                    MessageBox.Show("El campo 'Email' solo puede contener letras");

                    validacion = false;
                }
            }

            //Valida Telefono
            if (Regex.IsMatch(txt_telefono_cliente.Text, "[^0-9]") | txt_telefono_cliente.Text.Length < 7 & txt_telefono_cliente.Text.Length > 10)
            {
                if (string.IsNullOrEmpty(txt_telefono_cliente.Text))
                {

                }
                else
                {
                    MessageBox.Show("El campo 'Telefono' debe contener entre 7 y 10 numeros");

                    validacion = false;
                }
            }
        }
    }
}
