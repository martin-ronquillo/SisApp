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

                if (producto.Stock >= 1)
                {
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
                else
                {
                    MessageBox.Show("Producto sin stock");
                }
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

                        if (producto.Stock >= cantidadProvisional)
                        {
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
                        else
                        {
                            MessageBox.Show("Producto sin stock");
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
        public double Efectivo { get; set; }

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
            Efectivo = efectivo;

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

    public class Facturar
    {
        private int Id { get; set; }    //ID de la ultima venta
        private int User { get; set; }  //Recibe un ID de tipo int
        private string Cliente { get; set; }    //Recibe la Cedula de tipo string
        private int Caja { get; set; }   //Recibe el nombre de la caja en la que se genera la venta
        private string Fecha { get; set; }
        private double SubTotal { get; set; }
        private double Iva { get; set; }
        private double Descuento { get; set; }
        private double Total { get; set; }
        private double Efectivo { get; set; }
        private double Cambio { get; set; }
        private List<ArticulosVenta> Articulos { get; set; }

        public static string miConexion = ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;
        DataClasses1DataContext dataContext = new DataClasses1DataContext(miConexion);

        public Facturar(int User, string Cliente, string Fecha, List<ArticulosVenta> Articulos, double[] Valores)
        {
            this.User = User;
            this.Cliente = Cliente;
            this.Fecha = Fecha;
            this.Articulos = Articulos;
            this.SubTotal = Valores[0];
            this.Iva = Valores[1];
            this.Descuento = Valores[2];
            this.Total = Valores[3];
            this.Efectivo = Valores[4];
            this.Cambio = Valores[5];
        }

        public void RegistraVenta()
        {
            try
            {
                //Encuentra al cliente segun la cedula y obtiene su ID
                Cliente cliente = dataContext.Cliente.First(cli => cli.Cedula.Equals(Cliente));
                //Encuentra la caja segun el nombre y obtiene su ID
                try
                {
                    Caja caja = dataContext.Caja.First(caj => caj.NombreCaja.Equals(Environment.MachineName));

                    this.Caja = caja.Id;
                }
                catch
                {
                    Caja caja = new Caja
                    {
                        NombreCaja = Environment.MachineName,
                        Direccion = "Undefined",
                        Departamento = "Undefined",
                        AlmacenId = 1,
                    };

                    dataContext.Caja.InsertOnSubmit(caja);

                    dataContext.SubmitChanges();
                    
                    Caja Caja = dataContext.Caja.First(caj => caj.NombreCaja.Equals(Environment.MachineName));

                    this.Caja = Caja.Id;
                }

                Venta Venta = new Venta
                {
                    UserId = User,
                    ClienteId = cliente.Id,
                    FechaId = Convert.ToDateTime(Fecha),
                    TotalFactura = (decimal)Total,
                    SubTotal = (decimal)SubTotal,
                    Iva = (decimal)Iva,
                    Descuento = (decimal)Descuento,
                    Efectivo = (decimal)Efectivo,
                    Cambio = (decimal)Cambio,
                    CajaId = Caja,
                };

                dataContext.Venta.InsertOnSubmit(Venta);

                dataContext.SubmitChanges();

                Venta venta = dataContext.Venta.OrderByDescending(vent => vent.Id).FirstOrDefault();

                this.Id = venta.Id;
            }
            catch
            {
                MessageBox.Show("No se pudo generar la venta");
            }
        }

        public void RegistraFactura()
        {
            try
            {
                foreach (ArticulosVenta articulo in Articulos)
                {
                    VentaProducto ventaProducto = new VentaProducto
                    {
                        VentaId = this.Id,
                        ProductoId = articulo.Id,
                        Cantidad = articulo.Cantidad,
                        Producto = articulo.Producto,
                        ValorUnidad = (decimal)articulo.ValorUnidad,
                        ValorTotal = (decimal)articulo.ValorTotal,
                    };

                    dataContext.VentaProducto.InsertOnSubmit(ventaProducto);

                    dataContext.SubmitChanges();
                }
            }
            catch
            {
                MessageBox.Show("No se pudo generar la factura");
            }
        }
    }
}