using DataModels;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SisApp
{
    class InventarioArticulos
    {
        public int Id { get; set; }
        public string BarCode { get; set; }
        public string ProductName { get; set; }
        public float Stock { get; set; }
        public string Category { get; set; }
        public float StockBodega { get; set; }
        public float SalePricePercent { get; set; }
        public float SalePrice { get; set; }
        public float PurchasePrice { get; set; }
        public string TradeMark { get; set; }
        List<InventarioArticulos> listaArticulos = new List<InventarioArticulos>();

        readonly SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        public List<InventarioArticulos> LlenaArticulos(string comboBox, bool checkBox)
        {
            if (comboBox == "TODOS")
            {
                foreach (Product producto in db.Products.LoadWith(t => t.TradeMark))
                {
                    //Si el checkBox "dispone stock" esta activo
                    if (checkBox)
                    {
                        if (producto.Stock != 0)
                        {
                            try
                            {
                                listaArticulos.Add(
                                    new InventarioArticulos()
                                    {
                                        Id = (int)producto.Id,
                                        BarCode = producto.BarCode != null ? producto.BarCode : "N/A",
                                        ProductName = producto.ProductName != null ? producto.ProductName : "N/A",
                                        Stock = (float)producto.Stock,
                                        SalePricePercent = (float)producto.SalePricePercent,
                                        SalePrice = (float)producto.SalePrice,
                                        PurchasePrice = (float)producto.PucharsePrice,
                                        TradeMark = producto.TradeMark.MarkName != null ? producto.TradeMark.MarkName : "N/A"
                                    }
                                );
                            }
                            catch
                            {
                                //Nothing here
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            listaArticulos.Add(
                                new InventarioArticulos()
                                {
                                    Id = (int)producto.Id,
                                    BarCode = producto.BarCode != null ? producto.BarCode : "N/A",
                                    ProductName = producto.ProductName != null ? producto.ProductName : "N/A",
                                    Stock = (float)producto.Stock,
                                    SalePricePercent = (float)producto.SalePricePercent,
                                    SalePrice = (float)producto.SalePrice,
                                    PurchasePrice = (float)producto.PucharsePrice,
                                    TradeMark = producto.TradeMark.MarkName != null ? producto.TradeMark.MarkName : "N/A"
                                }
                            );
                        }
                        catch
                        {

                        }
                    }
                }
            }
            else
            {
                try
                {
                    List<ProductsStore> listaPs = new List<ProductsStore>();

                    listaPs = db.ProductsStores.Where(ps => ps.Store.StoreName.Equals(comboBox)).ToList();

                    foreach (ProductsStore ps in listaPs)
                    {
                        Product producto = db.Products.LoadWith(t => t.TradeMark).LoadWith(proSto => proSto.ProductsStores).First(pro => pro.Id.Equals(ps.ProductId));

                        //Si el checkBox "dispone stock" esta activo
                        if (checkBox)
                        {
                            if (producto.ProductsStores.FirstOrDefault().Stock != 0)
                            {
                                try
                                {
                                    listaArticulos.Add(
                                        new InventarioArticulos()
                                        {
                                            Id = (int)producto.Id,
                                            BarCode = producto.BarCode != null ? producto.BarCode : "N/A",
                                            ProductName = producto.ProductName != null ? producto.ProductName : "N/A",
                                            Stock = (float)producto.ProductsStores.FirstOrDefault().Stock,
                                            SalePricePercent = (float)producto.ProductsStores.FirstOrDefault().SalePricePercent,
                                            SalePrice = (float)producto.ProductsStores.FirstOrDefault().PriceByUnit,
                                            PurchasePrice = (float)producto.ProductsStores.FirstOrDefault().PurchasePrice,
                                            TradeMark = producto.TradeMark.MarkName != null ? producto.TradeMark.MarkName : "N/A"
                                        }
                                    );
                                }
                                catch
                                {
                                    //Nothing here
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                listaArticulos.Add(
                                    new InventarioArticulos()
                                    {
                                        Id = (int)producto.Id,
                                        BarCode = producto.BarCode != null ? producto.BarCode : "N/A",
                                        ProductName = producto.ProductName != null ? producto.ProductName : "N/A",
                                        Stock = (float)producto.ProductsStores.FirstOrDefault().Stock,
                                        SalePricePercent = (float)producto.ProductsStores.FirstOrDefault().SalePricePercent,
                                        SalePrice = (float)producto.ProductsStores.FirstOrDefault().PriceByUnit,
                                        PurchasePrice = (float)producto.ProductsStores.FirstOrDefault().PurchasePrice,
                                        TradeMark = producto.TradeMark.MarkName != null ? producto.TradeMark.MarkName : "N/A"
                                    }
                                );
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error al llenar la vista \n Exception: \n" + e);
                }
            }

            return listaArticulos;
        }

        public List<InventarioArticulos> BuscaArticulos(string comboBox, bool checkBox, string busqueda)
        {
            try
            {
                if (comboBox == "TODOS")
                {
                    foreach (Product producto in db.Products.LoadWith(t => t.TradeMark).Where(pro => pro.ProductName.Contains(busqueda)))
                    {
                        //Si el checkBox "dispone stock" esta activo
                        if (checkBox)
                        {
                            if (producto.Stock != 0)
                            {
                                try
                                {
                                    listaArticulos.Add(
                                        new InventarioArticulos()
                                        {
                                            Id = (int)producto.Id,
                                            BarCode = producto.BarCode != null ? producto.BarCode : "N/A",
                                            ProductName = producto.ProductName != null ? producto.ProductName : "N/A",
                                            Stock = (float)producto.Stock,
                                            SalePricePercent = (float)producto.SalePricePercent,
                                            SalePrice = (float)producto.SalePrice,
                                            PurchasePrice = (float)producto.PucharsePrice,
                                            TradeMark = producto.TradeMark.MarkName != null ? producto.TradeMark.MarkName : "N/A"
                                        }
                                    );
                                }
                                catch
                                {
                                    //Nothing here
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                listaArticulos.Add(
                                    new InventarioArticulos()
                                    {
                                        Id = (int)producto.Id,
                                        BarCode = producto.BarCode != null ? producto.BarCode : "N/A",
                                        ProductName = producto.ProductName != null ? producto.ProductName : "N/A",
                                        Stock = (float)producto.Stock,
                                        SalePricePercent = (float)producto.SalePricePercent,
                                        SalePrice = (float)producto.SalePrice,
                                        PurchasePrice = (float)producto.PucharsePrice,
                                        TradeMark = producto.TradeMark.MarkName != null ? producto.TradeMark.MarkName : "N/A"
                                    }
                                );
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        List<ProductsStore> listaPs = new List<ProductsStore>();

                        listaPs = db.ProductsStores.Where(ps => ps.Store.StoreName.Equals(comboBox)).ToList();

                        foreach (ProductsStore ps in listaPs)
                        {
                            foreach (Product producto in db.Products.LoadWith(t => t.TradeMark).LoadWith(proSto => proSto.ProductsStores).Where(pro => pro.ProductName.Contains(busqueda)))
                            {
                                if (producto.Id == ps.ProductId)
                                {
                                    //Si el checkBox "dispone stock" esta activo
                                    if (checkBox)
                                    {
                                        if (producto.ProductsStores.FirstOrDefault().Stock != 0)
                                        {
                                            try
                                            {
                                                listaArticulos.Add(
                                                    new InventarioArticulos()
                                                    {
                                                        Id = (int)producto.Id,
                                                        BarCode = producto.BarCode != null ? producto.BarCode : "N/A",
                                                        ProductName = producto.ProductName != null ? producto.ProductName : "N/A",
                                                        Stock = (float)producto.ProductsStores.FirstOrDefault().Stock,
                                                        SalePricePercent = (float)producto.ProductsStores.FirstOrDefault().SalePricePercent,
                                                        SalePrice = (float)producto.ProductsStores.FirstOrDefault().PriceByUnit,
                                                        PurchasePrice = (float)producto.ProductsStores.FirstOrDefault().PurchasePrice,
                                                        TradeMark = producto.TradeMark.MarkName != null ? producto.TradeMark.MarkName : "N/A"
                                                    }
                                                );
                                            }
                                            catch
                                            {
                                                //Nothing here
                                            }
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            listaArticulos.Add(
                                                new InventarioArticulos()
                                                {
                                                    Id = (int)producto.Id,
                                                    BarCode = producto.BarCode != null ? producto.BarCode : "N/A",
                                                    ProductName = producto.ProductName != null ? producto.ProductName : "N/A",
                                                    Stock = (float)producto.ProductsStores.FirstOrDefault().Stock,
                                                    SalePricePercent = (float)producto.ProductsStores.FirstOrDefault().SalePricePercent,
                                                    SalePrice = (float)producto.ProductsStores.FirstOrDefault().PriceByUnit,
                                                    PurchasePrice = (float)producto.ProductsStores.FirstOrDefault().PurchasePrice,
                                                    TradeMark = producto.TradeMark.MarkName != null ? producto.TradeMark.MarkName : "N/A"
                                                }
                                            );
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }//End If
                            }//End Foreach
                        }//End Foreach
                    }//End Try
                    catch (Exception e)
                    {
                        MessageBox.Show("Error al llenar la vista \n Exception: \n" + e);
                    }
                }

                return listaArticulos;
            }
            catch
            {
                MessageBox.Show("No se ha encontrado ninguna coincidencia");

                return listaArticulos;
            }
        }
    }

    class ProductosSeleccionados
    {
        public int Id { get; set; }
        public string BarCode { get; set; }
        public string ProductName { get; set; }
        public float SalePricePercent { get; set; }
        public float SalePrice { get; set; }
        public float Amount { get; set; }
        public float PurchasePrice { get; set; }
        public float TotalPrice { get; set; }
    }

    class IngresaProductos
    {
        public int Id { get; set; }
        public string BarCode { get; set; }
        public string ProductName { get; set; }
        public float SalePricePercent { get; set; }
        public float SalePrice { get; set; }
        public float PurchasePrice { get; set; }
        public float TotalPrice { get; set; }
        public float Amount { get; set; }
        public List<IngresaProductos> listaIngresos = new List<IngresaProductos>();

        public List<IngresaProductos> ListaIngresaProductos(List<ProductosSeleccionados> listaProductosSeleccionados)
        {
            if (listaProductosSeleccionados != null)
            {
                foreach (ProductosSeleccionados productoSelect in listaProductosSeleccionados)
                {
                    listaIngresos.Add(
                        new IngresaProductos
                        {
                            Id = productoSelect.Id,
                            BarCode = productoSelect.BarCode,
                            ProductName = productoSelect.ProductName,
                            SalePricePercent = productoSelect.SalePricePercent,
                            SalePrice = productoSelect.SalePrice,
                            Amount = productoSelect.Amount,
                            PurchasePrice = productoSelect.PurchasePrice,
                            TotalPrice = productoSelect.TotalPrice
                        }
                    );
                }
            }

            return listaIngresos;
        }
    }

    //Consulta Ingresos - Egresos
    class ConsultaIngEgr
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Store { get; set; }
        public float Total { get; set; }
        public string Type { get; set; }
        public List<ConsultaIngEgr> listaConsulta = new List<ConsultaIngEgr>();
    }

    class ConsultaRegistroInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Product { get; set; }
        public float Amount { get; set; }
        public float Percent { get; set; }
        public float Purchase { get; set; }
        public float Total { get; set; }
    }
}
