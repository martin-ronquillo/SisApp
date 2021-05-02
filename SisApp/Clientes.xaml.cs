using DataModels;
using LinqToDB;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para Clientes.xaml
    /// </summary>
    public partial class Clientes : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");

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
            LlenaListView();
        }

        //Boton Cancelar
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Boton Buscar
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            BuscaCliente();
            lv_clientes.Focus();
        }

        //Buscar en KeyDown Event
        private void txt_buscaCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BuscaCliente();
                lv_clientes.Focus();
            }
        }

        public void LlenaListView()
        {
            //Crea una lista y la rellena con los datos de la tabla Cliente, combina el nombre y el apellido en una sola casilla
            List<Customer> listaClientes = new List<Customer>();

            foreach (Customer cliente in db.Customers)
            {
                listaClientes.Add(
                    new Customer()
                    {
                        Id = cliente.Id,
                        Ci = cliente.Ci,
                        Name = cliente.Name + ' ' + cliente.LastName,
                        HomeAddress = cliente.HomeAddress,
                        Email = cliente.Email,
                        Telephone = cliente.Telephone
                    }
                );
            }

            lv_clientes.ItemsSource = listaClientes;

            //lv_clientes.ItemsSource = dataContext.Cliente;
        }

        public void BuscaCliente()
        {
            //Rellena una lista con los clientes de la BD
            List<Customer> listaClientes = new List<Customer>();

            foreach (Customer cliente in db.Customers)
            {
                listaClientes.Add(
                    new Customer()
                    {
                        Id = cliente.Id,
                        Ci = cliente.Ci,
                        Name = cliente.Name + ' ' + cliente.LastName,
                        HomeAddress = cliente.HomeAddress,
                        Email = cliente.Email,
                        Telephone = cliente.Telephone
                    }
                );
            }

            //Filtra en el campo "nombre" de la lista segun el parametro de busqueda
            listaClientes = listaClientes.Where(cli => cli.Name.Contains(txt_buscaCliente.Text.ToUpper())).ToList();
            txt_buscaCliente.Text = "";

            if (listaClientes.Count() != 0)
            {
                List<Customer> listaClientes2 = new List<Customer>();

                foreach (Customer cliente in listaClientes)
                {
                    listaClientes2.Add(
                        new Customer()
                        {
                            Id = cliente.Id,
                            Ci = cliente.Ci,
                            Name = cliente.Name + ' ' + cliente.LastName,
                            HomeAddress = cliente.HomeAddress,
                            Email = cliente.Email,
                            Telephone = cliente.Telephone
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
            Customer selectedCliente = (Customer)lv_clientes.SelectedItem;

            if (selectedCliente != null)
            {
                Singleton.Instancia.selectedCliente = (int)selectedCliente.Id;
            }
            else
            {
                MessageBox.Show("No se ha seleccionado ningun cliente, Consumidor Final establecido por defecto");

                Singleton.Instancia.selectedCliente = 1;
            }

            this.Close();
        }

        public void EliminaCliente()
        {
            Customer selectedCliente = (Customer)lv_clientes.SelectedItem;

            if (selectedCliente != null)
            {
                string nombre = selectedCliente.Name + " " + selectedCliente.LastName;

                Confirmar confirmar = new Confirmar(nombre);

                confirmar.ShowDialog();

                if (Singleton.Instancia.confirma)
                {
                    var cliente = db.Customers.First(cli => cli.Id.Equals(selectedCliente.Id));

                    db.Delete(cliente);
                }
            }
        }

        public void EditaCliente()
        {
            Customer selectedCliente = (Customer)lv_clientes.SelectedItem;

            if (selectedCliente != null)
            {
                AgregaCliente agregaCliente = new AgregaCliente((int)selectedCliente.Id);

                agregaCliente.ShowDialog();
            }
        }
    }
}
