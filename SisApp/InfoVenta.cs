using DataModels;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SisApp
{
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
        public string CodigoBarras { get; set; }

        public List<ArticulosVenta> InsertaArticulo(List<ArticulosVenta> listaProductos, string BarCode)
        {
            try
            {
                var cashier = db.Cashiers.FirstOrDefault(foo => foo.CheckerName.Equals(Environment.MachineName));

                var product = db.Products.FirstOrDefault(pro => pro.BarCode.Equals(BarCode.ToUpper()));

                var producto = db.ProductsStores.LoadWith(foo => foo.Product).FirstOrDefault(foo => foo.StoreId.Equals(cashier.StoreId) & foo.ProductId.Equals(product.Id));

                if (producto.Stock >= 1)
                {
                    Cantidad = 1;
                    listaProductos.Add(
                        new ArticulosVenta()
                        {
                            Id = (int)producto.Product.Id,
                            Cantidad = 1,
                            Producto = producto.Product.ProductName,
                            ValorUnidad = (float)producto.PriceByUnit,
                            ValorTotal = Cantidad * (float)producto.PriceByUnit,
                            CodigoBarras = producto.Product.BarCode
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
                MessageBox.Show("Producto no disponible en este almacen");
            }

            return listaProductos;
        }

        public List<ArticulosVenta> AumentaArticulo(List<ArticulosVenta> listaProductos, string BarCode)
        {
            try
            {
                var cashier = db.Cashiers.FirstOrDefault(foo => foo.CheckerName.Equals(Environment.MachineName));

                var product = db.Products.FirstOrDefault(pro => pro.BarCode.Equals(BarCode.ToUpper()));

                var producto = db.ProductsStores.LoadWith(foo => foo.Product).FirstOrDefault(foo => foo.StoreId.Equals(cashier.StoreId) & foo.ProductId.Equals(product.Id));

                var found = listaProductos.FirstOrDefault(foo => foo.CodigoBarras == BarCode);

                if (producto.Stock >= found.Cantidad + 1)
                {
                    found.Cantidad = found.Cantidad + 1;
                    found.ValorTotal = found.Cantidad * (float)found.ValorUnidad;
                }
                else
                {
                    MessageBox.Show("Producto sin stock");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return listaProductos;
        }

        public List<ArticulosVenta> ReduceArticulo(int selectedProducto, List<ArticulosVenta> listaProductos)
        {
            try
            {
                var found = listaProductos.FirstOrDefault(foo => foo.Id == selectedProducto);

                found.Cantidad = found.Cantidad - 1;

                if (found.Cantidad <= 0)
                {
                    listaProductos.Remove(found);
                }
                else
                {
                    found.ValorTotal = found.Cantidad * (float)found.ValorUnidad;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
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
                SubTotal += Math.Round(articulo.ValorTotal - (articulo.ValorTotal * 0.12), 2);
                Iva += Math.Round(articulo.ValorTotal * 0.12, 2);
                Total += Math.Round(articulo.ValorTotal, 2);
            }

            TotalAnterior = Math.Round(Total, 2);
        }

        public void DescuentoFactura(int descuento)
        {
            Total = TotalAnterior;

            double aPorcentaje = 100;
            Descuento = descuento / aPorcentaje;

            Total -= Math.Round(TotalAnterior * (descuento / aPorcentaje), 2);
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
        private string CodeFactura { get; set; }
        private List<ArticulosVenta> Articulos { get; set; }

        SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        public Facturar(int User, string Cliente, string Fecha, List<ArticulosVenta> Articulos, double[] Valores, string CodeFactura)
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
            this.Caja = (int)db.Cashiers.FirstOrDefault(foo => foo.CheckerName.Equals(Environment.MachineName)).Id;

            if (CodeFactura == "")
            {
                Random r = new Random();
                this.CodeFactura = r.Next(1000, 9999999).ToString("D7");
            }
            else
            {
                this.CodeFactura = CodeFactura;
            }
        }

        public void RegistraVenta()
        {
            try
            {
                //Encuentra al cliente segun la ID
                var cliente = db.Customers.First(cli => cli.Id.Equals(Singleton.Instancia.selectedCliente));
                var store = db.Cashiers.LoadWith(foo => foo.Store).FirstOrDefault(foo => foo.CheckerName.Equals(Environment.MachineName));

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
                    StoreId = store != null ? store.Store.Id : 1,
                    SaleCode = CodeFactura
                };

                db.Insert(Venta);
            }
            catch
            {
                MessageBox.Show("No se pudo generar la venta");
            }

            var venta = db.Sales.OrderByDescending(vent => vent.Id).FirstOrDefault();

            this.Id = (int)venta.Id;
        }

        public void RegistraFactura()
        {
            try
            {
                foreach (ArticulosVenta articulo in Articulos)
                {
                    var cashier = db.Cashiers.FirstOrDefault(foo => foo.CheckerName.Equals(Environment.MachineName));

                    var product = db.Products.First(pro => pro.Id.Equals(articulo.Id));

                    var producto = db.ProductsStores.LoadWith(foo => foo.Product).FirstOrDefault(foo => foo.StoreId.Equals(cashier.StoreId) & foo.ProductId.Equals(product.Id));

                    if (product.Stock >= articulo.Cantidad)
                    {
                        ProductsSale ventaProducto = new ProductsSale
                        {
                            SaleId = this.Id,
                            ProductId = articulo.Id,
                            Amount = articulo.Cantidad,
                            SalePrice = Math.Round(articulo.ValorUnidad, 2),
                            TotalPrice = Math.Round(articulo.ValorTotal, 2),
                        };

                        db.Insert(ventaProducto);
                    }
                    else
                    {
                        MessageBox.Show(String.Format("El articulo: {0}, solo dispone de {1} cantidades en el inventario y usted esta intentando vender {2}", articulo.Producto, producto.Stock, articulo.Cantidad));

                        var venta = db.Sales.OrderByDescending(vent => vent.Id).FirstOrDefault();

                        db.Delete(venta);
                    }

                    //Reduce el stock total del producto en la tabla "Products"
                    product.Stock = product.Stock - articulo.Cantidad;
                    db.Update(product);

                    producto.Stock = producto.Stock - articulo.Cantidad;
                    db.Update(producto);
                }
            }
            catch
            {
                MessageBox.Show("No se pudo generar la factura");
            }
        }
    }
}