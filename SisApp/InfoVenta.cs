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

        public int VentaNum()
        {
            try
            {
                Venta venta = dataContext.Venta.OrderByDescending(o => o.Id).FirstOrDefault();

                int nVenta = venta.Id + 1;

                return nVenta;
            }
            catch
            {
                int venta = 1;

                return venta;
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

                Cantidad = 1;
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

        public List<ArticulosVenta> AumentaArticulo(List<ArticulosVenta> listaProductos, int Id)
        {
            foreach (ArticulosVenta articulo in listaProductos)
            {
                if (articulo.Id == Id)
                {
                    try
                    {
                        Producto producto = dataContext.Producto.First(pro => pro.Id.Equals(Id));

                        int cantidadProvisional = articulo.Cantidad + 1;

                        Cantidad = cantidadProvisional;

                        listaProductos.Remove(articulo);

                        listaProductos.Add(
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

            return listaProductos;
        }

        public List<ArticulosVenta> ReduceArticulo(ArticulosVenta selectedProducto, List<ArticulosVenta> listaProductos)
        {
            foreach (ArticulosVenta articulo in listaProductos)
            {
                if (articulo.Id == selectedProducto.Id)
                {
                    try
                    {
                        Producto producto = dataContext.Producto.First(pro => pro.Id.Equals(articulo.Id));

                        int cantidadProvisional = articulo.Cantidad - 1;

                        Cantidad = cantidadProvisional;

                        if (cantidadProvisional <= 0)
                        {
                            listaProductos.Remove(articulo);
                        }
                        else
                        {
                            listaProductos.Remove(articulo);

                            listaProductos.Add(
                                new ArticulosVenta()
                                {
                                    Id = selectedProducto.Id,
                                    Cantidad = cantidadProvisional,
                                    Producto = producto.Producto1,
                                    ValorUnidad = (float)producto.PrecioVenta,
                                    ValorTotal = Cantidad * (float)producto.PrecioVenta
                                }
                            );
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Producto no encontrado");
                    }

                    break;
                }
            }

            return listaProductos;
        }
    }

    public class InfoFactura
    {
        public double SubTotal { get; set; }
        public double Iva { get; set; }
        public double Descuento { get; set; }
        public double Total { get; set; }
        public double TotalAnterior { get; set; }
        public double Cambio { get; set; }

        public static string miConexion = ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext = new DataClasses1DataContext(miConexion);

        public void ValoresFactura(List<ArticulosVenta> articulos)
        {
            SubTotal = 0;
            Iva = 0;
            Total = 0;
            Descuento = 0;

            foreach (ArticulosVenta articulo in articulos)
            {
                SubTotal += (articulo.ValorTotal - (articulo.ValorTotal * 0.12));
                Iva += articulo.ValorTotal * 0.12;
                Total += articulo.ValorTotal;
            }

            TotalAnterior = Total;
        }

        public void DescuentoFactura(int descuento)
        {
            Total = TotalAnterior;

            double aPorcentaje = 100;
            Descuento = descuento / aPorcentaje;

            Total -= (TotalAnterior * (descuento / aPorcentaje));
        }

        public void PagaEfectivo(double efectivo)
        {
            Cambio = 0;
            if (efectivo < Total)
            {
                MessageBox.Show("Efectivo insuficiente");
                Cambio = 0;
            }
            else
            {
                Cambio = efectivo - Total;
                Cambio = Math.Round(Cambio, 2);
            }
        }
    }
}