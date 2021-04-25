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
using System.Globalization;

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para Clientes.xaml
    /// </summary>
    public partial class Clientes : Window
    {
        public static string miConexion = ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext = new DataClasses1DataContext(miConexion);

        public Clientes()
        {
            InitializeComponent();

            LlenaListView();

            //Oculta el boton para agregar Clientes en caso de no ser administrador
            if (LoggedUser.Rol != "ADMIN")
            {
                btn_Agrega_cliente.Visibility = Visibility.Collapsed;
                btn_Edita_cliente.Visibility = Visibility.Collapsed;
                btn_Elimina_cliente.Visibility = Visibility.Collapsed;
            }
        }

        //Boton Agrega Cliente
        private void Agrega_cliente_Click(object sender, RoutedEventArgs e)
        {
            AgregaCliente agregaCliente = new AgregaCliente();

            agregaCliente.ShowDialog();

            LlenaListView();
        }

        //Boton Edita Cliente
        private void btn_Edita_cliente_Click(object sender, RoutedEventArgs e)
        {
            EditaCliente();

            LlenaListView();
        }

        //Boton Elimina Cliente
        private void btn_Elimina_cliente_Click(object sender, RoutedEventArgs e)
        {
            EliminaCliente();

            LlenaListView();
        }

        //Boton Seleccionar
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarCliente();
        }

        //Seleccionar item del listView en KeyDown Event
        private void lv_clientes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SeleccionarCliente();
            }
        }

        //Boton Actualizar
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            BuscaEmpleado();
        }

        //Boton Cancelar
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Boton Buscar
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            BuscaEmpleado();
            lv_clientes.Focus();
        }

        //Buscar en KeyDown Event
        private void txt_buscaCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BuscaEmpleado();
                lv_clientes.Focus();
            }
        }

        public void LlenaListView()
        {
            //Crea una lista y la rellena con los datos de la tabla Cliente, combina el nombre y el apellido en una sola casilla
            List<Cliente> listaClientes = new List<Cliente>();

            foreach (Cliente cliente in dataContext.Cliente)
            {
                listaClientes.Add(
                    new Cliente()
                    {
                        Id = cliente.Id,
                        Cedula = cliente.Cedula,
                        Nombre = cliente.Nombre + ' ' + cliente.Apellido,
                        Direccion = cliente.Direccion,
                        Email = cliente.Email,
                        Telefono = cliente.Telefono
                    }
                );
            }

            lv_clientes.ItemsSource = listaClientes;

            //lv_clientes.ItemsSource = dataContext.Cliente;
        }

        public void BuscaEmpleado()
        {
            //Rellena una lista con los clientes de la BD
            List<Cliente> listaClientes = new List<Cliente>();

            foreach (Cliente cliente in dataContext.Cliente)
            {
                listaClientes.Add(
                    new Cliente()
                    {
                        Id = cliente.Id,
                        Cedula = cliente.Cedula,
                        Nombre = cliente.Nombre + ' ' + cliente.Apellido,
                        Direccion = cliente.Direccion,
                        Email = cliente.Email,
                        Telefono = cliente.Telefono
                    }
                );
            }

            //Filtra en el campo "nombre" de la lista segun el parametro de busqueda
            listaClientes = listaClientes.Where(cli => cli.Nombre.Contains(txt_buscaCliente.Text.ToUpper())).ToList();
            txt_buscaCliente.Text = "";

            if (listaClientes.Count() != 0)
            {
                List<Cliente> listaClientes2 = new List<Cliente>();

                foreach (Cliente cliente in listaClientes)
                {
                    listaClientes2.Add(
                        new Cliente()
                        {
                            Id = cliente.Id,
                            Cedula = cliente.Cedula,
                            Nombre = cliente.Nombre + ' ' + cliente.Apellido,
                            Direccion = cliente.Direccion,
                            Email = cliente.Email,
                            Telefono = cliente.Telefono
                        }
                    );
                }

                lv_clientes.ItemsSource = listaClientes2;
            }
            else
            {
                MessageBox.Show("No se ha encontrado ninguna coincidencia");
            }
        }

        public void SeleccionarCliente()
        {
            Cliente selectedCliente = (Cliente)lv_clientes.SelectedItem;

            if (selectedCliente != null)
            {
                Singleton.Instancia.selectedCliente = selectedCliente.Id;
            }
            else
            {
                MessageBox.Show("No se ha seleccionado ningun cliente, Consumidor Final establecido por defecto");

                Singleton.Instancia.selectedCliente = 2;
            }

            this.Close();
        }

        public void EliminaCliente()
        {
            Cliente selectedCliente = (Cliente)lv_clientes.SelectedItem;

            string nombre = selectedCliente.Nombre + " " + selectedCliente.Apellido;

            Confirmar confirmar = new Confirmar(nombre);

            confirmar.ShowDialog();


        }

        public void EditaCliente()
        {
            Cliente selectedCliente = (Cliente)lv_clientes.SelectedItem;

            if (selectedCliente != null)
            {
                AgregaCliente agregaCliente = new AgregaCliente(selectedCliente.Id);

                agregaCliente.ShowDialog();
            }
        }
    }
}
