using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows;
using DataModels;
using LinqToDB;

namespace SisApp
{
    class InfoVenta
    {
        readonly SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        public int VentaNum()
        {
            try
            {
                var venta = db.Sales.OrderByDescending(sa => sa.Id).FirstOrDefault();

                int nVenta = (int)venta.Id + 1;

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
        readonly SisAppCompactDB db = new SisAppCompactDB("ConnStr");

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
                var cliente = db.Customers.First(cli => cli.Id.Equals(1));

                Id = (int)cliente.Id;
                Cedula = cliente.Ci;
                Nombre = cliente.Name;
                Apellido = cliente.LastName;
                Direccion = cliente.HomeAddress;
                Email = cliente.Email;
                Telefono = cliente.Telephone;
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
                var cliente = db.Customers.First(cli => cli.Id.Equals(id));

                Id = (int)cliente.Id;
                Cedula = cliente.Ci;
                Nombre = cliente.Name;
                Apellido = cliente.LastName;
                Direccion = cliente.HomeAddress;
                Email = cliente.Email;
                Telefono = cliente.Telephone;
            }
            catch
            {
                var cliente = db.Customers.First(cli => cli.Id.Equals(1));

                Id = (int)cliente.Id;
                Cedula = cliente.Ci;
                Nombre = cliente.Name;
                Apellido = cliente.LastName;
                Direccion = cliente.HomeAddress;
                Email = cliente.Email;
                Telefono = cliente.Telephone;
            }
        }
    }

    public class ArticulosVenta
    {
        readonly SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        public int Id { get; set; }
        public int Cantidad { get; set; } = 1;
        public string Producto { get; set; }
        public float ValorUnidad { get; set; }
        public float ValorTotal { get; set; }

        public List<ArticulosVenta> InsertaArticulo(List<ArticulosVenta> listaProductos, int Id)
        {
            try
            {
                var producto = db.Products.First(pro => pro.Id.Equals(Id));

                if (producto.Stock >= 1)
                {
                    Cantidad = 1;
                    listaProductos.Add(
                        new ArticulosVenta()
                        {
                            Id = Id,
                            Cantidad = 1,
                            Producto = producto.ProductName,
                            ValorUnidad = (float)producto.SalePrice,
                            ValorTotal = Cantidad * (float)producto.SalePrice
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
                        var producto = db.Products.First(pro => pro.Id.Equals(Id));

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
                                    Producto = producto.ProductName,
                                    ValorUnidad = (float)producto.SalePrice,
                                    ValorTotal = Cantidad * (float)producto.SalePrice
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

        public List<ArticulosVenta> ReduceArticulo(int selectedProducto, List<ArticulosVenta> listaProductos)
        {
            foreach (ArticulosVenta articulo in listaProductos)
            {
                if (articulo.Id == selectedProducto)
                {
                    try
                    {
                        var producto = db.Products.First(pro => pro.Id.Equals(selectedProducto));

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
                                    Id = selectedProducto,
                                    Cantidad = cantidadProvisional,
                                    Producto = producto.ProductName,
                                    ValorUnidad = (float)producto.SalePrice,
                                    ValorTotal = Cantidad * (float)producto.SalePrice
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

        SisAppCompactDB db = new SisAppCompactDB("ConnStr");

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
                //Encuentra al cliente segun la ID
                var cliente = db.Customers.First(cli => cli.Id.Equals(Singleton.Instancia.selectedCliente));

                //Encuentra la caja segun el nombre y obtiene su ID. Si no existe, se crea
                try
                {
                    var caja = db.Cashiers.First(caj => caj.CheckerName.Equals(Environment.MachineName));

                    this.Caja = (int)caja.Id;
                }
                catch
                {
                    Cashier caja = new Cashier
                    {
                        CheckerName = Environment.MachineName,
                        Direction = "Undefined",
                        StoreId = 0,
                    };

                    db.Insert(caja);

                    var Caja = db.Cashiers.First(caj => caj.CheckerName.Equals(Environment.MachineName));

                    this.Caja = (int)Caja.Id;
                }

                Sale Venta = new Sale
                {
                    UserId = User,
                    CustomerId = cliente.Id,
                    SaleDate = Fecha,
                    TotalPrice = Total,
                    SubPrice = SubTotal,
                    Tax = Iva,
                    Discount = Descuento,
                    Cash = Efectivo,
                    RemainingCash = Cambio,
                    CashierId = Caja,
                };

                db.Insert(Venta);

                var venta = db.Sales.OrderByDescending(vent => vent.Id).FirstOrDefault();

                this.Id = (int)venta.Id;
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
                    var producto = db.Products.First(pro => pro.Id.Equals(articulo.Id));

                    if (producto.Stock >= articulo.Cantidad)
                    {
                        ProductsSale ventaProducto = new ProductsSale
                        {
                            SaleId = this.Id,
                            ProductId = articulo.Id,
                            Amount = articulo.Cantidad,
                            SalePrice = articulo.ValorUnidad,
                            TotalPrice = articulo.ValorTotal,
                        };

                        db.Insert(ventaProducto);

                        //Reduce el stock total del producto en la tabla "Products"
                        producto.Stock = producto.Stock - articulo.Cantidad;

                        db.Update(producto);
                    }
                    else
                    {
                        MessageBox.Show(String.Format("El articulo: {0}, solo dispone de {1} cantidades en el inventario y usted esta intentando vender {2}", articulo.Producto, producto.Stock, articulo.Cantidad));

                        var venta = db.Sales.OrderByDescending(vent => vent.Id).FirstOrDefault();

                        db.Delete(venta);
                    }
                }
            }
            catch
            {
                MessageBox.Show("No se pudo generar la factura");
            }
        }
    }
}