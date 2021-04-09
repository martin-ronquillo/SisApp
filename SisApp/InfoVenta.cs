using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SisApp
{
    class InfoVenta
    {
        public static string miConexion = ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext = new DataClasses1DataContext(miConexion);
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

        public ClienteFactura()
        {
            try
            {
                Cliente cliente = dataContext.Cliente.First(cli => cli.Id.Equals(2));

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

        public ClienteFactura(int id)
        {
            try
            {
                Cliente cliente = dataContext.Cliente.First(cli => cli.Id.Equals(id));

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
                Cliente cliente = dataContext.Cliente.First(cli => cli.Id.Equals(2));

                Id = cliente.Id;
                Cedula = cliente.Cedula;
                Nombre = cliente.Nombre;
                Apellido = cliente.Apellido;
                Direccion = cliente.Direccion;
                Email = cliente.Email;
                Telefono = cliente.Telefono;
            }
        }
    }

    public class ArticulosVenta
    {
        public static string miConexion = ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext = new DataClasses1DataContext(miConexion);

        public int Id { get; set; }
        public int Cantidad { get; set; }
        public string Producto { get; set; }
        public int ValorUnidad { get; set; }
        public int ValorTotal { get; set; }

        public List<ArticulosVenta> InsertaArticulo(int productoId)
        {
            List<VentaProducto> listaProductos = new List<VentaProducto>();

            List<ArticulosVenta> listaPrueba = new List<ArticulosVenta>();

            listaPrueba.Add(new ArticulosVenta() { Id = 1, Cantidad = 5, Producto = "kk con chocolate xd", ValorUnidad = 12, ValorTotal = 10 });

            return listaPrueba;
            /*Producto producto = dataContext.Producto.First(pro => pro.Id.Equals(productoId));

            listaProductos.Add(
                    new VentaProducto()
                    {
                        ProductoId = producto.Id,
                        Producto = producto.Producto1,
                        ValorUnidad = producto.PrecioVenta,
                        ValorTotal = producto.PrecioVenta
                    }
                );*/
        }
    }
}