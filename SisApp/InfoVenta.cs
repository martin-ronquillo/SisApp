using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows;

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
        public int Cantidad { get; set; } = 1;
        public string Producto { get; set; }
        public float ValorUnidad { get; set; }
        public float ValorTotal { get; set; }

        public List<ArticulosVenta> InsertaArticulo(List<ArticulosVenta> listaProductos, int Id)
        {
            try
            {
                Producto producto = dataContext.Producto.First(pro => pro.Id.Equals(Id));

                listaProductos.Add(
                    new ArticulosVenta()
                    {
                        Id = Id,
                        Cantidad = 1,
                        Producto = producto.Producto1,
                        ValorUnidad = (float)producto.PrecioVenta,
                        ValorTotal = Cantidad * (float)producto.PrecioVenta
                    }
                );
            }
            catch
            {
                MessageBox.Show("Producto no encontrado");
            }

            return listaProductos;
        }

        public List<ArticulosVenta> ActualizaArticulo(List<ArticulosVenta> listaArticulo, int Id)
        {
            foreach (ArticulosVenta articulo in listaArticulo)
            {
                if (articulo.Id == Id)
                {
                    try
                    {
                        Producto producto = dataContext.Producto.First(pro => pro.Id.Equals(Id));

                        int cantidadProvisional = articulo.Cantidad + 1;

                        Cantidad = cantidadProvisional;

                        listaArticulo.Remove(articulo);

                        listaArticulo.Add(
                            new ArticulosVenta()
                            {
                                Id = Id,
                                Cantidad = cantidadProvisional,
                                Producto = producto.Producto1,
                                ValorUnidad = (float)producto.PrecioVenta,
                                ValorTotal = Cantidad * (float)producto.PrecioVenta
                            }
                        );
                    }
                    catch
                    {
                        MessageBox.Show("Producto no encontrado");
                    }

                    break;
                }
            }

            return listaArticulo;
        }
    }
}