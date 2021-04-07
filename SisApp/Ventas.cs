using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SisApp
{
    class Ventas
    {
        public static string miConexion = ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext = new DataClasses1DataContext(miConexion);
    }

    class LoggedUser
    {
        public static string miConexion = ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext = new DataClasses1DataContext(miConexion);

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Cargo { get; set; } 

        public LoggedUser(int id)
        {
            try
            {
                User user = dataContext.User.First(us => us.Id.Equals(id));

                Id = user.Id;
                Nombre = user.nombre;
                Apellido = user.apellido;
                Cargo = user.cargo;
            }
            catch
            {
                Id = 0;
                Nombre = null;
                Apellido = null;
                Cargo = 0;
            }
        }
    }

    class ClienteFactura
    {
        public static string miConexion = ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext = new DataClasses1DataContext(miConexion);

        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }

        List<Cliente> listaClientes = new List<Cliente>();


        public ClienteFactura()
        {
            try
            {
                Cliente cliente = dataContext.Cliente.First(cli => cli.Id.Equals(1));

                Id = cliente.Id;
                Cedula = cliente.Cedula;
                Nombre = cliente.Nombre;
                Apellido = cliente.Apellido;
                Direccion = cliente.Direccion;
                Email = cliente.Email;
                Telefono = cliente.Telefono;
            }
            catch
            {
                Id = 0;
                Nombre = null;
                Apellido = null;
                Direccion = null;
                Email = null;
                Telefono = null;

            }
        }

        public ClienteFactura(string nombre)
        {
            try
            {
                /*Cliente clienteeee = (Cliente)dataContext.Cliente.Where(cli => cli.Nombre.Equals(nombre));

                foreach (Object clientes in clienteeee)
                {

                }

                /*Id = cliente.Id;
                Cedula = cliente.Cedula;
                Nombre = cliente.Nombre;
                Apellido = cliente.Apellido;
                Direccion = cliente.Direccion;
                Email = cliente.Email;
                Telefono = cliente.Telefono;*/
            }
            catch
            {
                Id = 0;
                Nombre = null;
                Apellido = null;
                Direccion = null;
                Email = null;
                Telefono = null;

            }
        }
    }
}