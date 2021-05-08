using DataModels;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;

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
        //Prueba de parametros para compras
        public float Discount { get; set; }
        public float SubTotal { get; set; }
        public float Tax { get; set; }
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
        //Prueba de parametros para compras
        public float Discount { get; set; }
        public float SubTotal { get; set; }
        public float Tax { get; set; }
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
                            TotalPrice = productoSelect.TotalPrice,
                            Discount = 0,
                            SubTotal = 0,
                            Tax = 0,
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

    //Genera un excel de todos los ingresos o egresos de una fecha especifica
    class GeneraExcel
    {
        private string tipoConsulta { get; set; }
        private int Id { get; set; }
        readonly SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        //Recibe la fecha consultada y el tipo de la consulta "ingreso o egreso"
        public void CreaExcel(int tipoConsulta, string fechaConsulta)
        {
            if (tipoConsulta == 1)
            {
                try
                {
                    this.tipoConsulta = "INGRESOS";

                    var registros = db.Receipts.LoadWith(t => t.Store).LoadWith(foo => foo.User).LoadWith(t => t.ProductsReceipts).Where(re => re.ReceiptDate.Equals(fechaConsulta)).ToList();

                    // Creamos un objeto Excel.
                    Excel.Application Mi_Excel = default(Excel.Application);
                    // Creamos un objeto WorkBook. Para crear el documento Excel.           
                    Excel.Workbook LibroExcel = default(Excel.Workbook);
                    // Creamos un objeto WorkSheet. Para crear la hoja del documento.
                    Excel.Worksheet HojaExcel = default(Excel.Worksheet);

                    // Iniciamos una instancia a Excel, y Hacemos visibles para ver como se va creando el reporte, 
                    // podemos hacerlo visible al final si se desea.
                    Mi_Excel = new Excel.Application();

                    /* Ahora creamos un nuevo documento y seleccionamos la primera hoja del 
                        * documento en la cual crearemos nuestro informe. 
                        */
                    // Creamos una instancia del Workbooks de excel.            
                    LibroExcel = Mi_Excel.Workbooks.Add();

                    // Creamos una instancia de la primera hoja de trabajo de excel            
                    HojaExcel = LibroExcel.Worksheets[1];
                    HojaExcel.Visible = Excel.XlSheetVisibility.xlSheetVisible;
                    HojaExcel.Activate();

                    // Crear el encabezado de nuestro informe.
                    // La primera línea une las celdas y las convierte un en una sola.            
                    HojaExcel.Range["A2:G2"].Merge();
                    // La segunda línea Asigna el nombre del encabezado.
                    HojaExcel.Range["A2:G2"].Value = "REGISTROS DE " + this.tipoConsulta + " DEL: " + fechaConsulta;
                    // La tercera línea asigna negrita al titulo.
                    HojaExcel.Range["A2:G2"].Font.Bold = true;
                    // La cuarta línea signa un Size a titulo de 15.
                    HojaExcel.Range["A2:G2"].Font.Size = 15;
                    HojaExcel.Range["A2:G2"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    int documentInit = 4;

                    foreach (var registro in registros)
                    {
                        var proRec = db.ProductsReceipts.LoadWith(foo => foo.Product).Where(re => re.ReceiptId.Equals(registro.Id));

                        /*// Creamos una instancia de la primera hoja de trabajo de excel            
                        HojaExcel = LibroExcel.Worksheets[sheet];
                        HojaExcel.Visible = Excel.XlSheetVisibility.xlSheetVisible;
                        HojaExcel.Activate();

                        // Crear el encabezado de nuestro informe.
                        // La primera línea une las celdas y las convierte un en una sola.            
                        HojaExcel.Range["A" + documentInit + ":G" + documentInit].Merge();
                        // La segunda línea Asigna el nombre del encabezado.
                        HojaExcel.Range["A" + documentInit + ":G" + documentInit].Value = "REGISTROS DE " + this.tipoConsulta;
                        // La tercera línea asigna negrita al titulo.
                        HojaExcel.Range["A" + documentInit + ":G" + documentInit].Font.Bold = true;
                        // La cuarta línea signa un Size a titulo de 15.
                        HojaExcel.Range["A" + documentInit + ":G" + documentInit].Font.Size = 15;
                        HojaExcel.Range["A" + documentInit + ":G" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;*/

                        //Se aumenta la fila del documento
                        //documentInit ++;

                        //Escribe el Codigo del Ingreso
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Merge();
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Value = "Codigo Ingreso:";
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Font.Bold = true;
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["C" + documentInit].Value = registro.ReceiptCode;
                        HojaExcel.Range["C" + documentInit].Font.Italic = true;

                        //Escribe el nombre de la persona que realizo la accion
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Merge();
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Value = "Personal:";
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Font.Bold = true;
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Merge();
                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Value = registro.User.Name + " " + registro.User.LastName;
                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Font.Italic = true;
                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        //Se aumenta la fila del documento
                        documentInit++;

                        //Almacen donde se realizo la accion
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Merge();
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Value = "Almacen:";
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Font.Bold = true;
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["C" + documentInit].Value = registro.Store.StoreName;
                        HojaExcel.Range["C" + documentInit].Font.Italic = true;
                        HojaExcel.Range["C" + documentInit].Font.Size = 10;
                        HojaExcel.Range["C" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        //Tipo de ingreso
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Merge();
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Value = "Tipo:";
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Font.Bold = true;
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Merge();
                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Value = registro.Type;
                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Font.Italic = true;
                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        //Se aumenta la fila del documento
                        documentInit++;

                        //Separacion
                        HojaExcel.Range["A" + documentInit + ":G" + documentInit].Merge();

                        //Se aumenta la fila del documento
                        documentInit++;

                        //Encabezados de apartados
                        HojaExcel.Range["A" + documentInit].Value = "Codigo";
                        HojaExcel.Range["A" + documentInit].Font.Bold = true;
                        HojaExcel.Range["A" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["B" + documentInit + ":D" + documentInit].Merge();
                        HojaExcel.Range["B" + documentInit + ":D" + documentInit].Value = "Producto";
                        HojaExcel.Range["B" + documentInit + ":D" + documentInit].Font.Bold = true;
                        HojaExcel.Range["B" + documentInit + ":D" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["E" + documentInit].Value = "Cantidad";
                        HojaExcel.Range["E" + documentInit].Font.Bold = true;
                        HojaExcel.Range["E" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["F" + documentInit].Value = "Val. Compra";
                        HojaExcel.Range["F" + documentInit].Font.Bold = true;
                        HojaExcel.Range["F" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["G" + documentInit].Value = "Val. Total";
                        HojaExcel.Range["G" + documentInit].Font.Bold = true;
                        HojaExcel.Range["G" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        int i = documentInit + 1;

                        /*foreach (var con in xd)
                        {
                            MessageBox.Show("ahber");
                        }*/
                        
                        foreach (var item in proRec)
                        {
                            //Configura las celdas desde B hasta D
                            HojaExcel.Range["B" + i + ":D" + i].Merge();
                            HojaExcel.Range["B" + i + ":D" + i].Font.Size = 10;

                            //Codigo
                            HojaExcel.Cells[i, "A"] = item.Product.BarCode;
                            //Producto
                            HojaExcel.Cells[i, "B"] = item.Product.ProductName;
                            //Cantidad
                            HojaExcel.Cells[i, "E"] = item.Amount;
                            //Precio Compra
                            HojaExcel.Cells[i, "F"] = item.PurchasePrice;
                            //Total por cada producto
                            //HojaExcel.Range["G" + i].Formula = "=PRODUCTO(E" + i + ":F" + i + ")";
                            //HojaExcel.Cells[i, "G"] = "=PRODUCTO(E" + i + ":F" + i + ")";
                            HojaExcel.Cells[i, "G"] = item.Amount * item.PurchasePrice;

                            // Avanzamos una fila
                            i++;
                        }

                        i += 2;

                        //Escribe el total del ingreso
                        HojaExcel.Range["F" + i].Value = "Total:";
                        HojaExcel.Range["F" + i].Font.Bold = true;
                        HojaExcel.Range["F" + i].Font.Size = 14;
                        HojaExcel.Range["F" + i].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["G" + i].Value = registro.TotalPriceReceipt;
                        HojaExcel.Range["G" + i].Font.Bold = true;
                        HojaExcel.Range["G" + i].Font.Size = 12;

                        i += 3;

                        //Marca el fin de un registro
                        HojaExcel.Range["A" + i + ":G" + i].Merge();
                        HojaExcel.Range["A" + i + ":G" + i].Value = "*****************************************************************************";
                        HojaExcel.Range["A" + i + ":G" + i].Font.Bold = true;
                        HojaExcel.Range["A" + i + ":G" + i].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        // Seleccionar todo el bloque desde A1 hasta G #de filas.
                        //Excel.Range Rango = HojaExcel.Range["A3:G" + (i - 1).ToString()];

                        // Selecionado todo el rango especificado
                        //Rango.Select();

                        // Ajustamos el ancho de las columnas al ancho máximo del
                        // contenido de sus celdas
                        //Rango.Columns.AutoFit();

                        // Asignar filtro por columna
                        //Rango.AutoFilter(1);

                        //Muestra la ventana de excel
                        //Mi_Excel.Visible = true;

                        //Bloquea la hoja ante edicion
                        //HojaExcel.Protect();

                        // Hacemos esta hoja la visible en pantalla 
                        // (como seleccionamos la primera esto no es necesario
                        // si seleccionamos una diferente a la primera si lo
                        // necesitariamos).
                        //HojaExcel.Activate();


                        // Crear un total general
                        //LibroExcel.PrintPreview();
                        documentInit = i + 3;
                    }
                    //Muestra la ventana de excel
                    Mi_Excel.Visible = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("No se pudo crear el documento. Excepcion controlada: \n" + e);
                }
            }
            else
            {
                try
                {
                    this.tipoConsulta = "EGRESOS";

                    var registros = db.Egresses.LoadWith(t => t.Store).LoadWith(foo => foo.User).LoadWith(t => t.EgressProducts).Where(re => re.EgressDate.Equals(fechaConsulta)).ToList();

                    // Creamos un objeto Excel.
                    Excel.Application Mi_Excel = default(Excel.Application);
                    // Creamos un objeto WorkBook. Para crear el documento Excel.           
                    Excel.Workbook LibroExcel = default(Excel.Workbook);
                    // Creamos un objeto WorkSheet. Para crear la hoja del documento.
                    Excel.Worksheet HojaExcel = default(Excel.Worksheet);

                    // Iniciamos una instancia a Excel, y Hacemos visibles para ver como se va creando el reporte, 
                    // podemos hacerlo visible al final si se desea.
                    Mi_Excel = new Excel.Application();

                    /* Ahora creamos un nuevo documento y seleccionamos la primera hoja del 
                        * documento en la cual crearemos nuestro informe. 
                        */
                    // Creamos una instancia del Workbooks de excel.            
                    LibroExcel = Mi_Excel.Workbooks.Add();

                    // Creamos una instancia de la primera hoja de trabajo de excel            
                    HojaExcel = LibroExcel.Worksheets[1];
                    HojaExcel.Visible = Excel.XlSheetVisibility.xlSheetVisible;
                    HojaExcel.Activate();

                    // Crear el encabezado de nuestro informe.
                    // La primera línea une las celdas y las convierte un en una sola.            
                    HojaExcel.Range["A2:G2"].Merge();
                    // La segunda línea Asigna el nombre del encabezado.
                    HojaExcel.Range["A2:G2"].Value = "REGISTROS DE " + this.tipoConsulta + " DEL: " + fechaConsulta;
                    // La tercera línea asigna negrita al titulo.
                    HojaExcel.Range["A2:G2"].Font.Bold = true;
                    // La cuarta línea signa un Size a titulo de 15.
                    HojaExcel.Range["A2:G2"].Font.Size = 15;
                    HojaExcel.Range["A2:G2"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    int documentInit = 4;

                    foreach (var registro in registros)
                    {
                        var proRec = db.EgressProducts.LoadWith(foo => foo.Product).Where(re => re.EgressId.Equals(registro.Id));

                        /*// Creamos una instancia de la primera hoja de trabajo de excel            
                        HojaExcel = LibroExcel.Worksheets[sheet];
                        HojaExcel.Visible = Excel.XlSheetVisibility.xlSheetVisible;
                        HojaExcel.Activate();

                        // Crear el encabezado de nuestro informe.
                        // La primera línea une las celdas y las convierte un en una sola.            
                        HojaExcel.Range["A" + documentInit + ":G" + documentInit].Merge();
                        // La segunda línea Asigna el nombre del encabezado.
                        HojaExcel.Range["A" + documentInit + ":G" + documentInit].Value = "REGISTROS DE " + this.tipoConsulta;
                        // La tercera línea asigna negrita al titulo.
                        HojaExcel.Range["A" + documentInit + ":G" + documentInit].Font.Bold = true;
                        // La cuarta línea signa un Size a titulo de 15.
                        HojaExcel.Range["A" + documentInit + ":G" + documentInit].Font.Size = 15;
                        HojaExcel.Range["A" + documentInit + ":G" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;*/

                        //Se aumenta la fila del documento
                        //documentInit ++;

                        //Escribe el Codigo del Egreso
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Merge();
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Value = "Codigo Egreso:";
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Font.Bold = true;
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["C" + documentInit].Value = registro.EgressCode;
                        HojaExcel.Range["C" + documentInit].Font.Italic = true;

                        //Escribe el nombre de la persona que realizo la accion
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Merge();
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Value = "Personal:";
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Font.Bold = true;
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Merge();
                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Value = registro.User.Name + " " + registro.User.LastName;
                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Font.Italic = true;
                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        //Se aumenta la fila del documento
                        documentInit++;

                        //Almacen donde se realizo la accion
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Merge();
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Value = "Almacen:";
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Font.Bold = true;
                        HojaExcel.Range["A" + documentInit + ":B" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["C" + documentInit].Value = registro.Store.StoreName;
                        HojaExcel.Range["C" + documentInit].Font.Italic = true;
                        HojaExcel.Range["C" + documentInit].Font.Size = 10;
                        HojaExcel.Range["C" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        //Tipo de ingreso
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Merge();
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Value = "Tipo:";
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Font.Bold = true;
                        HojaExcel.Range["D" + documentInit + ":E" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Merge();
                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Value = registro.Type;
                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Font.Italic = true;
                        HojaExcel.Range["F" + documentInit + ":G" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        //Se aumenta la fila del documento
                        documentInit++;

                        //Separacion
                        HojaExcel.Range["A" + documentInit + ":G" + documentInit].Merge();

                        //Se aumenta la fila del documento
                        documentInit++;

                        //Encabezados de apartados
                        HojaExcel.Range["A" + documentInit].Value = "Codigo";
                        HojaExcel.Range["A" + documentInit].Font.Bold = true;
                        HojaExcel.Range["A" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["B" + documentInit + ":D" + documentInit].Merge();
                        HojaExcel.Range["B" + documentInit + ":D" + documentInit].Value = "Producto";
                        HojaExcel.Range["B" + documentInit + ":D" + documentInit].Font.Bold = true;
                        HojaExcel.Range["B" + documentInit + ":D" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["E" + documentInit].Value = "Cantidad";
                        HojaExcel.Range["E" + documentInit].Font.Bold = true;
                        HojaExcel.Range["E" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["F" + documentInit].Value = "Val. Compra";
                        HojaExcel.Range["F" + documentInit].Font.Bold = true;
                        HojaExcel.Range["F" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["G" + documentInit].Value = "Val. Total";
                        HojaExcel.Range["G" + documentInit].Font.Bold = true;
                        HojaExcel.Range["G" + documentInit].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        int i = documentInit + 1;

                        /*foreach (var con in xd)
                        {
                            MessageBox.Show("ahber");
                        }*/

                        foreach (var item in proRec)
                        {
                            //Configura las celdas desde B hasta D
                            HojaExcel.Range["B" + i + ":D" + i].Merge();
                            HojaExcel.Range["B" + i + ":D" + i].Font.Size = 10;

                            //Codigo
                            HojaExcel.Cells[i, "A"] = item.Product.BarCode;
                            //Producto
                            HojaExcel.Cells[i, "B"] = item.Product.ProductName;
                            //Cantidad
                            HojaExcel.Cells[i, "E"] = item.Amount;
                            //Precio Compra
                            HojaExcel.Cells[i, "F"] = item.PurchasePrice;
                            //Total por cada producto
                            //HojaExcel.Range["G" + i].Formula = "=PRODUCTO(E" + i + ":F" + i + ")";
                            //HojaExcel.Cells[i, "G"] = "=PRODUCTO(E" + i + ":F" + i + ")";
                            HojaExcel.Cells[i, "G"] = item.Amount * item.PurchasePrice;

                            // Avanzamos una fila
                            i++;
                        }

                        i += 2;

                        //Escribe el total del ingreso
                        HojaExcel.Range["F" + i].Value = "Total:";
                        HojaExcel.Range["F" + i].Font.Bold = true;
                        HojaExcel.Range["F" + i].Font.Size = 14;
                        HojaExcel.Range["F" + i].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        HojaExcel.Range["G" + i].Value = registro.TotalPriceEgress;
                        HojaExcel.Range["G" + i].Font.Bold = true;
                        HojaExcel.Range["G" + i].Font.Size = 12;

                        i += 3;

                        //Marca el fin de un registro
                        HojaExcel.Range["A" + i + ":G" + i].Merge();
                        HojaExcel.Range["A" + i + ":G" + i].Value = "*****************************************************************************";
                        HojaExcel.Range["A" + i + ":G" + i].Font.Bold = true;
                        HojaExcel.Range["A" + i + ":G" + i].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                        // Seleccionar todo el bloque desde A1 hasta G #de filas.
                        //Excel.Range Rango = HojaExcel.Range["A3:G" + (i - 1).ToString()];

                        // Selecionado todo el rango especificado
                        //Rango.Select();

                        // Ajustamos el ancho de las columnas al ancho máximo del
                        // contenido de sus celdas
                        //Rango.Columns.AutoFit();

                        // Asignar filtro por columna
                        //Rango.AutoFilter(1);

                        //Muestra la ventana de excel
                        //Mi_Excel.Visible = true;

                        //Bloquea la hoja ante edicion
                        //HojaExcel.Protect();

                        // Hacemos esta hoja la visible en pantalla 
                        // (como seleccionamos la primera esto no es necesario
                        // si seleccionamos una diferente a la primera si lo
                        // necesitariamos).
                        //HojaExcel.Activate();


                        // Crear un total general
                        //LibroExcel.PrintPreview();
                        documentInit = i + 3;
                    }
                    //Muestra la ventana de excel
                    Mi_Excel.Visible = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("No se pudo crear el documento. Excepcion controlada: \n" + e);
                }
            }
        }
    }

    //Genera un excel de un ingreso o egreso especifico
    class GeneraExcelEspec
    {
        private string tipoConsulta { get; set; }
        private int Id { get; set; }
        readonly SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        public void CreaExcel(int tipoConsulta, int Id)
        {
            this.Id = Id;

            if (tipoConsulta == 1)
            {
                try
                {
                    this.tipoConsulta = "INGRESOS";

                    var ingreso = db.Receipts.LoadWith(foo => foo.User).First(ing => ing.Id.Equals(Id));
                    var registros = db.ProductsReceipts.LoadWith(foo => foo.Product).LoadWith(foo => foo.Receipt).Where(re => re.ReceiptId.Equals(Id));

                    // Creamos un objeto Excel.
                    Excel.Application Mi_Excel = default(Excel.Application);
                    // Creamos un objeto WorkBook. Para crear el documento Excel.           
                    Excel.Workbook LibroExcel = default(Excel.Workbook);
                    // Creamos un objeto WorkSheet. Para crear la hoja del documento.
                    Excel.Worksheet HojaExcel = default(Excel.Worksheet);

                    // Iniciamos una instancia a Excel, y Hacemos visibles para ver como se va creando el reporte, 
                    // podemos hacerlo visible al final si se desea.
                    Mi_Excel = new Excel.Application();

                    /* Ahora creamos un nuevo documento y seleccionamos la primera hoja del 
                        * documento en la cual crearemos nuestro informe. 
                        */
                    // Creamos una instancia del Workbooks de excel.            
                    LibroExcel = Mi_Excel.Workbooks.Add();
                    // Creamos una instancia de la primera hoja de trabajo de excel            
                    HojaExcel = LibroExcel.Worksheets[1];
                    HojaExcel.Visible = Excel.XlSheetVisibility.xlSheetVisible;

                    // Crear el encabezado de nuestro informe.
                    // La primera línea une las celdas y las convierte un en una sola.            
                    HojaExcel.Range["A2:G2"].Merge();
                    // La segunda línea Asigna el nombre del encabezado.
                    HojaExcel.Range["A2:G2"].Value = "REGISTROS DE " + this.tipoConsulta;
                    // La tercera línea asigna negrita al titulo.
                    HojaExcel.Range["A2:G2"].Font.Bold = true;
                    // La cuarta línea signa un Size a titulo de 15.
                    HojaExcel.Range["A2:G2"].Font.Size = 15;
                    HojaExcel.Range["A2:G2"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    //Escribe el Codigo del Ingreso
                    HojaExcel.Range["A3:B3"].Merge();
                    HojaExcel.Range["A3:B3"].Value = "Codigo Ingreso:";
                    HojaExcel.Range["A3:B3"].Font.Bold = true;
                    HojaExcel.Range["A3:B3"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["C3"].Value = ingreso.ReceiptCode;
                    HojaExcel.Range["C3"].Font.Italic = true;

                    //Escribe el nombre de la persona que realizo la accion
                    HojaExcel.Range["D3:E3"].Merge();
                    HojaExcel.Range["D3:E3"].Value = "Personal:";
                    HojaExcel.Range["D3:E3"].Font.Bold = true;
                    HojaExcel.Range["D3:E3"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["F3:G3"].Merge();
                    HojaExcel.Range["F3:G3"].Value = ingreso.User.Name + " " + ingreso.User.LastName;
                    HojaExcel.Range["F3:G3"].Font.Italic = true;
                    HojaExcel.Range["F3:G3"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    //Fecha del Ingreso
                    HojaExcel.Range["A4:B4"].Merge();
                    HojaExcel.Range["A4:B4"].Value = "Fecha De Ingreso:";
                    HojaExcel.Range["A4:B4"].Font.Bold = true;
                    HojaExcel.Range["A4:B4"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["C4"].Value = ingreso.ReceiptDate;
                    HojaExcel.Range["C4"].Font.Italic = true;
                    HojaExcel.Range["C4"].Font.Size = 11;

                    //Tipo de ingreso
                    HojaExcel.Range["D4:E4"].Merge();
                    HojaExcel.Range["D4:E4"].Value = "Tipo:";
                    HojaExcel.Range["D4:E4"].Font.Bold = true;
                    HojaExcel.Range["D4:E4"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["F4:G4"].Merge();
                    HojaExcel.Range["F4:G4"].Value = ingreso.Type;
                    HojaExcel.Range["F4:G4"].Font.Italic = true;
                    HojaExcel.Range["F4:G4"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    //Separacion
                    HojaExcel.Range["A5:G5"].Merge();

                    //Encabezados de apartados
                    HojaExcel.Range["A6"].Value = "Codigo";
                    HojaExcel.Range["A6"].Font.Bold = true;
                    HojaExcel.Range["A6"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["B6:D6"].Merge();
                    HojaExcel.Range["B6:D6"].Value = "Producto";
                    HojaExcel.Range["B6:D6"].Font.Bold = true;
                    HojaExcel.Range["B6:D6"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["E6"].Value = "Cantidad";
                    HojaExcel.Range["E6"].Font.Bold = true;
                    HojaExcel.Range["E6"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["F6"].Value = "Val. Compra";
                    HojaExcel.Range["F6"].Font.Bold = true;
                    HojaExcel.Range["F6"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["G6"].Value = "Val. Total";
                    HojaExcel.Range["G6"].Font.Bold = true;
                    HojaExcel.Range["G6"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    int i = 7;
                    foreach (var item in registros)
                    {
                        //Configura las celdas desde B hasta D
                        HojaExcel.Range["B" + i + ":D" + i].Merge();
                        HojaExcel.Range["B" + i + ":D" + i].Font.Size = 10;

                        //Codigo
                        HojaExcel.Cells[i, "A"] = item.Product.BarCode;
                        //Producto
                        HojaExcel.Cells[i, "B"] = item.Product.ProductName;
                        //Cantidad
                        HojaExcel.Cells[i, "E"] = item.Amount;
                        //Precio Compra
                        HojaExcel.Cells[i, "F"] = item.PurchasePrice;
                        //Total por cada producto
                        //HojaExcel.Range["G" + i].Formula = "=PRODUCTO(E" + i + ":F" + i + ")";
                        //HojaExcel.Cells[i, "G"] = "=PRODUCTO(E" + i + ":F" + i + ")";
                        HojaExcel.Cells[i, "G"] = item.Amount * item.PurchasePrice;

                        // Avanzamos una fila
                        i++;
                    }

                    i += 2;

                    //Escribe el total del ingreso
                    HojaExcel.Range["F" + i].Value = "Total:";
                    HojaExcel.Range["F" + i].Font.Bold = true;
                    HojaExcel.Range["F" + i].Font.Size = 14;
                    HojaExcel.Range["F" + i].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["G" + i].Value = ingreso.TotalPriceReceipt;
                    HojaExcel.Range["G" + i].Font.Bold = true;
                    HojaExcel.Range["G" + i].Font.Size = 12;

                    // Seleccionar todo el bloque desde A1 hasta G #de filas.
                    //Excel.Range Rango = HojaExcel.Range["A3:G" + (i - 1).ToString()];

                    // Selecionado todo el rango especificado
                    //Rango.Select();

                    // Ajustamos el ancho de las columnas al ancho máximo del
                    // contenido de sus celdas
                    //Rango.Columns.AutoFit();

                    // Asignar filtro por columna
                    //Rango.AutoFilter(1);

                    //Muestra la ventana de excel
                    Mi_Excel.Visible = true;

                    //Bloquea la hoja ante edicion
                    HojaExcel.Protect();

                    // Hacemos esta hoja la visible en pantalla 
                    // (como seleccionamos la primera esto no es necesario
                    // si seleccionamos una diferente a la primera si lo
                    // necesitariamos).
                    HojaExcel.Activate();


                    // Crear un total general
                    //LibroExcel.PrintPreview();
                }
                catch (Exception e)
                {
                    MessageBox.Show("No se pudo crear el documento. Excepcion controlada: \n" + e);
                }
            }
            else
            {
                try
                {
                    this.tipoConsulta = "EGRESOS";

                    var egreso = db.Egresses.LoadWith(foo => foo.User).First(ing => ing.Id.Equals(Id));
                    var registros = db.EgressProducts.LoadWith(foo => foo.Product).LoadWith(foo => foo.Egress).Where(re => re.EgressId.Equals(Id));

                    // Creamos un objeto Excel.
                    Excel.Application Mi_Excel = default(Excel.Application);
                    // Creamos un objeto WorkBook. Para crear el documento Excel.           
                    Excel.Workbook LibroExcel = default(Excel.Workbook);
                    // Creamos un objeto WorkSheet. Para crear la hoja del documento.
                    Excel.Worksheet HojaExcel = default(Excel.Worksheet);

                    // Iniciamos una instancia a Excel, y Hacemos visibles para ver como se va creando el reporte, 
                    // podemos hacerlo visible al final si se desea.
                    Mi_Excel = new Excel.Application();

                    /* Ahora creamos un nuevo documento y seleccionamos la primera hoja del 
                        * documento en la cual crearemos nuestro informe. 
                        */
                    // Creamos una instancia del Workbooks de excel.            
                    LibroExcel = Mi_Excel.Workbooks.Add();
                    // Creamos una instancia de la primera hoja de trabajo de excel            
                    HojaExcel = LibroExcel.Worksheets[1];
                    HojaExcel.Visible = Excel.XlSheetVisibility.xlSheetVisible;

                    // Crear el encabezado de nuestro informe.
                    // La primera línea une las celdas y las convierte un en una sola.            
                    HojaExcel.Range["A2:G2"].Merge();
                    // La segunda línea Asigna el nombre del encabezado.
                    HojaExcel.Range["A2:G2"].Value = "REGISTROS DE " + this.tipoConsulta;
                    // La tercera línea asigna negrita al titulo.
                    HojaExcel.Range["A2:G2"].Font.Bold = true;
                    // La cuarta línea signa un Size a titulo de 15.
                    HojaExcel.Range["A2:G2"].Font.Size = 15;
                    HojaExcel.Range["A2:G2"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    //Escribe el Codigo del egreso
                    HojaExcel.Range["A3:B3"].Merge();
                    HojaExcel.Range["A3:B3"].Value = "Codigo Egreso:";
                    HojaExcel.Range["A3:B3"].Font.Bold = true;
                    HojaExcel.Range["A3:B3"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["C3"].Value = egreso.EgressCode;
                    HojaExcel.Range["C3"].Font.Italic = true;

                    //Escribe el nombre de la persona que realizo la accion
                    HojaExcel.Range["D3:E3"].Merge();
                    HojaExcel.Range["D3:E3"].Value = "Personal:";
                    HojaExcel.Range["D3:E3"].Font.Bold = true;
                    HojaExcel.Range["D3:E3"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["F3:G3"].Merge();
                    HojaExcel.Range["F3:G3"].Value = egreso.User.Name + " " + egreso.User.LastName;
                    HojaExcel.Range["F3:G3"].Font.Italic = true;
                    HojaExcel.Range["F3:G3"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    //Fecha del egreso
                    HojaExcel.Range["A4:B4"].Merge();
                    HojaExcel.Range["A4:B4"].Value = "Fecha De Egreso:";
                    HojaExcel.Range["A4:B4"].Font.Bold = true;
                    HojaExcel.Range["A4:B4"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["C4"].Value = egreso.EgressDate;
                    HojaExcel.Range["C4"].Font.Italic = true;
                    HojaExcel.Range["C4"].Font.Size = 11;

                    //Tipo de egreso
                    HojaExcel.Range["D4:E4"].Merge();
                    HojaExcel.Range["D4:E4"].Value = "Tipo:";
                    HojaExcel.Range["D4:E4"].Font.Bold = true;
                    HojaExcel.Range["D4:E4"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["F4:G4"].Merge();
                    HojaExcel.Range["F4:G4"].Value = egreso.Type;
                    HojaExcel.Range["F4:G4"].Font.Italic = true;
                    HojaExcel.Range["F4:G4"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    //Separacion
                    HojaExcel.Range["A5:G5"].Merge();

                    //Encabezados de apartados
                    HojaExcel.Range["A6"].Value = "Codigo";
                    HojaExcel.Range["A6"].Font.Bold = true;
                    HojaExcel.Range["A6"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["B6:D6"].Merge();
                    HojaExcel.Range["B6:D6"].Value = "Producto";
                    HojaExcel.Range["B6:D6"].Font.Bold = true;
                    HojaExcel.Range["B6:D6"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["E6"].Value = "Cantidad";
                    HojaExcel.Range["E6"].Font.Bold = true;
                    HojaExcel.Range["E6"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["F6"].Value = "Val. Compra";
                    HojaExcel.Range["F6"].Font.Bold = true;
                    HojaExcel.Range["F6"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["G6"].Value = "Val. Total";
                    HojaExcel.Range["G6"].Font.Bold = true;
                    HojaExcel.Range["G6"].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    int i = 7;
                    foreach (var item in registros)
                    {
                        //Configura las celdas desde B hasta D
                        HojaExcel.Range["B" + i + ":D" + i].Merge();
                        HojaExcel.Range["B" + i + ":D" + i].Font.Size = 10;

                        //Codigo
                        HojaExcel.Cells[i, "A"] = item.Product.BarCode;
                        //Producto
                        HojaExcel.Cells[i, "B"] = item.Product.ProductName;
                        //Cantidad
                        HojaExcel.Cells[i, "E"] = item.Amount;
                        //Precio Compra
                        HojaExcel.Cells[i, "F"] = item.PurchasePrice;
                        //Total por cada producto
                        //HojaExcel.Range["G" + i].Formula = "=PRODUCTO(E" + i + ":F" + i + ")";
                        //HojaExcel.Cells[i, "G"] = "=PRODUCTO(E" + i + ":F" + i + ")";
                        HojaExcel.Cells[i, "G"] = item.Amount * item.PurchasePrice;

                        // Avanzamos una fila
                        i++;
                    }

                    i += 2;

                    //Escribe el total del ingreso
                    HojaExcel.Range["F" + i].Value = "Total:";
                    HojaExcel.Range["F" + i].Font.Bold = true;
                    HojaExcel.Range["F" + i].Font.Size = 14;
                    HojaExcel.Range["F" + i].Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    HojaExcel.Range["G" + i].Value = egreso.TotalPriceEgress;
                    HojaExcel.Range["G" + i].Font.Bold = true;
                    HojaExcel.Range["G" + i].Font.Size = 12;

                    // Seleccionar todo el bloque desde A1 hasta G #de filas.
                    //Excel.Range Rango = HojaExcel.Range["A3:G" + (i - 1).ToString()];

                    // Selecionado todo el rango especificado
                    //Rango.Select();

                    // Ajustamos el ancho de las columnas al ancho máximo del
                    // contenido de sus celdas
                    //Rango.Columns.AutoFit();

                    // Asignar filtro por columna
                    //Rango.AutoFilter(1);

                    //Muestra la ventana de excel
                    Mi_Excel.Visible = true;

                    //Bloquea la hoja ante edicion
                    HojaExcel.Protect();

                    // Hacemos esta hoja la visible en pantalla 
                    // (como seleccionamos la primera esto no es necesario
                    // si seleccionamos una diferente a la primera si lo
                    // necesitariamos).
                    HojaExcel.Activate();


                    // Crear un total general
                    //LibroExcel.PrintPreview();
                }
                catch (Exception e)
                {
                    MessageBox.Show("No se pudo crear el documento. Excepcion controlada: \n" + e);
                }
            }
        }
    }

    class ProveedoresController
    {
        public int Id { get; set; }
        public string Ruc { get; set; }
        public string ProviderName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string TelephoneOne { get; set; }
        public string TelephoneTwo { get; set; }
        public string WebSite { get; set; }
        public string SalesManName { get; set; }
        public string SalesManTelephone { get; set; }
        public string SalesManEmail { get; set; }
        public string Bank { get; set; }
        public string AccountType { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
    }
}
