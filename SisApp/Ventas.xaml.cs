using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataModels;
using LinqToDB;

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para Ventas.xaml
    /// </summary>
    public partial class Ventas : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        BackgroundWorker worker = new BackgroundWorker();

        IngresaProductos editCell;

        //Almacena el valor total de la factura
        float ValTF;
        int IdCliente = 1;
        bool validate = true;
        float ValorTotalFactura;
        Random r = new Random();
        string saleCode;

        public Ventas()
        {
            InitializeComponent();

            Singleton.Instancia.listaIngresos.Clear();

            this.WindowState = WindowState.Maximized;

            LlenaCombos();

            dp_compra.DisplayDateEnd = DateTime.Today;
            dp_compra.SelectedDate = DateTime.Today;

            pbStatus.Visibility = Visibility.Collapsed;
            btn_Guardar.IsEnabled = false;

            txt_descuento.Text = "0";
            txt_valorTotalFactura.Text = "0";

            txt_descuento.IsEnabled = false;
            txt_valorTotalFactura.IsEnabled = false;
            dg_datos.IsReadOnly = true;

            saleCode = r.Next(1000, 9999999).ToString("D7");
            txt_codigoVenta.Text = saleCode;
        }

        private void btn_seleccionaProducto_Click(object sender, RoutedEventArgs e)
        {
            if (cbBox_almacen.SelectedItem != null)
            {
                if (dg_datos.Items.Count == 0)
                {
                    SeleccionaIngreso seleccionaIngreso = new SeleccionaIngreso();
                    //WindowState = WindowState.Minimized;

                    seleccionaIngreso.ShowDialog();
                }
                else
                {
                    SeleccionaIngreso seleccionaIngreso = new SeleccionaIngreso(dg_datos);
                    //WindowState = WindowState.Minimized;

                    seleccionaIngreso.ShowDialog();
                }

                //Si hay un almacen seleccionado, se acomodan los datos
                if (cbBox_almacen.SelectedItem.ToString() != "" & cbBox_almacen.SelectedItem != null)
                {
                    dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;

                    ActualizaDG();

                    txt_descuento.IsEnabled = true;
                    txt_valorTotalFactura.IsEnabled = true;
                }
                else
                {
                    dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;
                    ActualizaDG();

                    txt_descuento.IsEnabled = true;
                    txt_valorTotalFactura.IsEnabled = true;
                }

                if (dg_datos.Items.Count == 0)
                {
                    btn_Guardar.IsEnabled = false;
                }
                else
                {
                    btn_Guardar.IsEnabled = true;
                }
                WindowState = WindowState.Maximized;
            }
            else
            {
                MessageBox.Show("Primero seleccione un almacen");
            }
        }

        private void btn_buscaCliente_Click(object sender, RoutedEventArgs e)
        {
            BuscaCliente();
        }

        private void cbBox_almacen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dg_datos.IsReadOnly = false;
            ActualizaDG();
        }

        private void dg_datos_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Obtiene la celda que fue actualizada
            editCell = e.Row.Item as IngresaProductos;

            //Obtiene el indice de la columna editada
            DataGridColumn col1 = e.Column;
            int col_index = col1.DisplayIndex;

            //Obtiene el valor editado
            TextBox t = e.EditingElement as TextBox;
            string cadena = t.Text;
            cadena = cadena.Replace(".", ",");
            float value;
            if (!float.TryParse(cadena, out value))
            {
                //Busca el producto editado en la lista
                var found = Singleton.Instancia.listaIngresos.FirstOrDefault(fo => fo.Id == editCell.Id);

                //Si se edita la cantidad
                if (col_index == 3)
                {
                    float iva = found.SalePrice * (float)0.12;
                    float subtotal = found.SalePrice - iva;

                    found.Amount = 1;
                    found.TotalPrice = 1 * found.SalePrice;
                }
            }
            else
            {
                //Convierte el valor editado a float
                float edit = float.Parse(cadena);

                //Busca el producto editado en la lista
                var found = Singleton.Instancia.listaIngresos.FirstOrDefault(fo => fo.Id == editCell.Id);
                var almacen = db.Stores.First(foo => foo.StoreName.Equals(cbBox_almacen.SelectedItem.ToString()));

                var cantidad = db.ProductsStores.First(foo => foo.ProductId.Equals(found.Id) & foo.StoreId.Equals(almacen.Id)).Stock;
                //Si se edita la Cantidad
                if (col_index == 3)
                {
                    if (edit <= cantidad)
                    {
                        float totalCant = found.SalePrice * edit;
                        float iva = totalCant * (float)0.12;
                        float subtotal = totalCant - iva;

                        found.Amount = edit;
                        found.SubTotal = subtotal;
                        found.Tax = iva;
                        found.TotalPrice = totalCant;
                    }
                    else
                    {
                        float totalCant = found.SalePrice * 1;
                        float iva = totalCant * (float)0.12;
                        float subtotal = totalCant - iva;

                        found.Amount = 1;
                        found.SubTotal = subtotal;
                        found.Tax = iva;
                        found.TotalPrice = 1 * found.SalePrice;

                        MessageBox.Show("El producto solo posee " + cantidad + " existencias en " + almacen.StoreName);
                    }
                }
            }

            dg_datos.ItemsSource = null;
            dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;

            ValorTotalFactura = 0;

            foreach (var item in dg_datos.Items.OfType<IngresaProductos>())
            {
                ValorTotalFactura += item.TotalPrice;
            }

            ValTF = ValorTotalFactura;

            if (ValorTotalFactura != 0)
            {
                txt_descuento.IsEnabled = true;
                txt_valorTotalFactura.IsEnabled = true;
            }
            else
            {
                txt_descuento.IsEnabled = false;
                txt_valorTotalFactura.IsEnabled = false;
            }

            txt_valorTotalFactura.Text = ValTF.ToString();

            if (dg_datos.Items.Count == 0)
            {
                btn_Guardar.IsEnabled = false;
            }
            else
            {
                btn_Guardar.IsEnabled = true;
            }
        }

        private void dg_datos_UnloadingRow(object sender, DataGridRowEventArgs e)
        {
            ValorTotalFactura = 0;

            foreach (var item in dg_datos.Items.OfType<IngresaProductos>())
            {
                ValorTotalFactura += item.TotalPrice;
            }

            ValTF = ValorTotalFactura;
            txt_valorTotalFactura.Text = ValTF.ToString();

            if (dg_datos.Items.Count != 0)
            {
                txt_descuento.IsEnabled = true;
                txt_valorTotalFactura.IsEnabled = true;

                if (txt_descuento.Text != "0" & txt_descuento.Text != "")
                {
                    float valorDescontado = ValTF - (ValTF * (float.Parse(txt_descuento.Text) / 100));
                    txt_valorTotalFactura.Text = valorDescontado.ToString();
                }
            }
            else
            {
                txt_descuento.Text = "0";
                txt_valorTotalFactura.Text = "0";
                txt_descuento.IsEnabled = false;
                txt_valorTotalFactura.IsEnabled = false;
            }
        }

        private void btn_limpiar_Click(object sender, RoutedEventArgs e)
        {
            cbBox_almacen.SelectedItem = "";
            txt_descuento.Text = "0";
            txt_cliente.Text = "Consumidor Final";
            txt_descuento.IsEnabled = false;
            btn_Guardar.IsEnabled = false;
            txt_valorTotalFactura.IsEnabled = false;

            Singleton.Instancia.listaIngresos.Clear();
            dg_datos.ItemsSource = null;
            dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;
            dg_datos.Items.Refresh();
        }

        private void btn_Guardar_Click(object sender, RoutedEventArgs e)
        {
            //Deshabilita el estado de todos los botones y controles del form
            cbBox_almacen.IsEnabled = false;
            dp_compra.IsEnabled = false;
            btn_seleccionaProducto.IsEnabled = false;
            dg_datos.IsReadOnly = true;
            btn_Guardar.IsEnabled = false;
            btn_buscaCliente.IsEnabled = false;
            txt_descuento.IsEnabled = false;
            txt_valorTotalFactura.IsEnabled = false;

            ValidaForm();

            try
            {
                if (validate)
                {
                    if (dg_datos.Items.Count != 0)
                    {
                        //Muestra el estado del trabajo en segundo plano
                        pbStatus.Visibility = Visibility.Visible;
                        lbl_saveInfo.Content = "Generando nueva venta";

                        //Genera el ingreso

                        float subprice = float.Parse(txt_valorTotalFactura.Text) - (float.Parse(txt_valorTotalFactura.Text) * (float)0.12);
                        float tax = float.Parse(txt_valorTotalFactura.Text) * (float)0.12;
                        var cajero = db.Cashiers.First(foo => foo.CheckerName.Equals(Environment.MachineName));

                        Sale sale = new Sale
                        {
                            UserId = Singleton.Instancia.idUser,
                            CustomerId = IdCliente,
                            SaleDate = dp_compra.Text,
                            TotalPrice = Math.Round(float.Parse(txt_valorTotalFactura.Text), 2),
                            SubPrice = Math.Round(subprice, 2),
                            Tax = Math.Round(tax, 2),
                            Discount = Math.Round(float.Parse(txt_descuento.Text), 2),
                            Cash = 0,
                            RemainingCash = 0,
                            CashierId = cajero != null ? cajero.Id : 1,
                            StoreId = db.Stores.First(sto => sto.StoreName.Equals(cbBox_almacen.SelectedItem.ToString().ToUpper())).Id,
                            SaleCode = txt_codigoVenta.Text == "" ? saleCode : txt_codigoVenta.Text
                        };

                        db.Insert(sale);

                        //Resta las tareas que se hayan asignado al segundo plano anteriormente
                        worker.DoWork -= worker_DoWork;
                        worker.ProgressChanged -= worker_ProgressChanged;
                        worker.RunWorkerCompleted -= Worker_RunWorkerCompleted;

                        //Agrega una tarea al segundo plano
                        worker.WorkerReportsProgress = true;
                        worker.DoWork += worker_DoWork;
                        worker.ProgressChanged += worker_ProgressChanged;
                        worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                        //Empieza una tarea en segundo plano
                        worker.RunWorkerAsync();

                    }
                    else
                    {
                        MessageBox.Show("Error, el codigo de factura proporcionado ya existe o fue ingresado anteriormente");

                        saleCode = r.Next(1000, 9999999).ToString("D7");
                        txt_codigoVenta.Text = saleCode;
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error: " + exc);

                cbBox_almacen.IsEnabled = true;
                dp_compra.IsEnabled = true;
                btn_seleccionaProducto.IsEnabled = true;
                dg_datos.IsReadOnly = false;
                btn_Guardar.IsEnabled = true;
                btn_buscaCliente.IsEnabled = true;
                txt_descuento.IsEnabled = true;
                txt_valorTotalFactura.IsEnabled = true;
            }
        }

        public void LlenaCombos()
        {
            foreach (Store almacen in db.Stores)
            {
                cbBox_almacen.Items.Add(almacen.StoreName);
            }
        }

        public void BuscaCliente()
        {
            Clientes clientes = new Clientes();

            clientes.ShowDialog();

            ClienteFactura clienteFactura = new ClienteFactura(Singleton.Instancia.selectedCliente);

            txt_cliente.Text = clienteFactura.Nombre + ' ' + clienteFactura.Apellido;
            IdCliente = clienteFactura.Id;
        }

        public void ActualizaDG()
        {
            /*
             * Esta Funcion usa recursividad.
             * Cuando se selecciona un almacen en el combobox, se llama a la funcion
             * esta se encarga de verificar que el producto que se desea egresar exista en el almacen seleccionado,
             * si no existe, se elimina el producto de la lista. Pero al modificar la lista no se puede iterar mas sobre ella
             * por lo que manda una excepcion (catch) que vuelve a llamar a la funcion en bucle hasta que la lista
             * este vacia o hasta que solo queden los productos que si esten disponibles en el almacen seleccionado
             */
            try
            {
                var Almacen = db.Stores.First(alm => alm.StoreName.Equals(cbBox_almacen.SelectedItem.ToString().ToUpper()));
                var ProductStore = db.ProductsStores.Where(ps => ps.StoreId.Equals(Almacen.Id));

                foreach (var item in dg_datos.Items.OfType<IngresaProductos>())
                {
                    if (dg_datos.Items.Count > 0)
                    {
                        var vinculo = ProductStore.FirstOrDefault(ps => ps.ProductId.Equals(item.Id));

                        if (vinculo != null)
                        {
                            var found = Singleton.Instancia.listaIngresos.FirstOrDefault(fo => fo.Id == item.Id);

                            float totalCant = found.SalePrice * found.Amount;
                            float iva = totalCant * (float)0.12;
                            float subtotal = totalCant - iva;

                            found.SubTotal = subtotal;
                            found.Tax = iva;
                            found.SalePrice = (float)vinculo.PriceByUnit;
                            found.TotalPrice = item.Amount * (float)vinculo.PriceByUnit;
                        }
                        else
                        {
                            var found = Singleton.Instancia.listaIngresos.FirstOrDefault(fo => fo.Id == item.Id);

                            Singleton.Instancia.listaIngresos.Remove(found);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch
            {
                ActualizaDG();
            }

            dg_datos.ItemsSource = null;

            dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;
        }

        private void txt_descuento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter | e.Key == Key.Tab)
            {
                int value;
                //Si ingresa un caracter no numerico se reestablece a 0
                if (!int.TryParse(txt_descuento.Text, out value))
                {
                    txt_descuento.Text = "0";
                    txt_valorTotalFactura.Text = ValTF.ToString();
                }
                else
                {
                    //Si se ingresa el 0 o un vacio, se reestablece a 0
                    if (txt_descuento.Text == "0" | txt_descuento.Text == "")
                    {
                        txt_descuento.Text = "0";
                        txt_valorTotalFactura.Text = ValTF.ToString();
                    }
                    else
                    {
                        float valorDescontado = ValTF - (ValTF * ((float)value / 100));
                        txt_valorTotalFactura.Text = valorDescontado.ToString();
                    }
                }
            }
        }

        public void ValidaForm()
        {
            validate = true;

            if (cbBox_almacen.SelectedItem == "" | cbBox_almacen.SelectedItem == null)
            {
                validate = false;
            }

            if (dp_compra.Text == "" | dp_compra.Text == null)
            {
                validate = false;
            }

            if (dg_datos.Items.Count == 0)
            {
                validate = false;
            }

            if (txt_valorTotalFactura.Text == "" | txt_valorTotalFactura.Text == "0")
            {
                validate = false;
            }
        }

        //Trabajo que ejecutara la tarea en segundo plano
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var lastSale = db.Sales.OrderByDescending(sale => sale.Id).FirstOrDefault();
                float totalPriceSale = 0;

                var productStore = db.ProductsStores.Where(ps => ps.StoreId.Equals(lastSale.StoreId));

                //Variables de control de progreso de la tarea
                float xd = 100 / dg_datos.Items.Count;
                int i = (int)Math.Round(xd);
                int i2 = 0;
                int anterior = 0;

                foreach (var item in dg_datos.Items.OfType<IngresaProductos>())
                {
                    anterior = i2;
                    i2 = anterior + i;

                    ProductsSale productsSale = new ProductsSale
                    {
                        SaleId = lastSale.Id,
                        ProductId = item.Id,
                        Amount = Math.Round(item.Amount, 2),
                        SalePrice = Math.Round(item.SalePrice, 2),
                        TotalPrice = 0
                    };

                    db.Insert(productsSale);

                    //Resta los productos vendidos al inventario total del producto
                    var producto = db.Products.First(pro => pro.Id.Equals(item.Id));

                    if (producto.Stock >= item.Amount)
                    {
                        producto.Stock = producto.Stock - Math.Round(item.Amount, 2);
                    }
                    else
                    {
                        producto.Stock = 0;
                    }

                    db.Update(producto);

                    //Resta los productos vendidos del inventario del almacen correspondiente
                    var vinculo = productStore.FirstOrDefault(ps => ps.ProductId.Equals(item.Id));

                    if (vinculo != null)
                    {
                        if (vinculo.Stock >= item.Amount)
                        {
                            vinculo.Stock = vinculo.Stock - Math.Round(item.Amount, 2);
                        }
                        else
                        {
                            vinculo.Stock = 0;
                        }

                        db.Update(vinculo);
                    }

                    totalPriceSale = totalPriceSale + item.TotalPrice;

                    (sender as BackgroundWorker).ReportProgress(i2);
                }

                //Envia el reporte de progreso de la tarea en segundo plano
                (sender as BackgroundWorker).ReportProgress(i2 + i);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }
        //Cuando se completa la tarea en segundo plano
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Habilita todos los botones y controles
            cbBox_almacen.IsEnabled = true;
            dp_compra.IsEnabled = true;
            btn_seleccionaProducto.IsEnabled = true;
            dg_datos.IsReadOnly = false;
            btn_Guardar.IsEnabled = false;
            txt_descuento.IsEnabled = false;
            txt_valorTotalFactura.IsEnabled = false;

            cbBox_almacen.SelectedItem = "";
            txt_descuento.Text = "0";
            txt_valorTotalFactura.Text = "0";

            //Resetea la lista de los ingresos
            Singleton.Instancia.listaIngresos.Clear();
            dg_datos.ItemsSource = null;
            dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;
            dg_datos.Items.Refresh();
            //Resetea la informacion del estado de almacenamiento
            lbl_saveInfo.Content = "";
            pbStatus.Value = 0;
            pbStatus.Visibility = Visibility.Collapsed;

            saleCode = r.Next(1000, 9999999).ToString("D7");
            txt_codigoVenta.Text = saleCode;
        }
        //Muestra el reporte de la tarea en segundo plano
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbl_saveInfo.Content = "Vinculando productos a la compra: " + e.ProgressPercentage + "%";

            pbStatus.Value = e.ProgressPercentage;
        }

        private void txt_valorTotalFactura_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter | e.Key == Key.Tab)
            {
                float value;
                string cadena = txt_valorTotalFactura.Text;
                cadena = cadena.Replace(".", ",");
                //Si ingresa un caracter no numerico se reestablece a 0
                if (!float.TryParse(cadena, out value))
                {
                    txt_valorTotalFactura.Text = "0";
                    txt_descuento.Text = "0";
                }
                else
                {
                    //Si se ingresa el 0 o un vacio, se reestablece a 0
                    if (txt_valorTotalFactura.Text == "0" | txt_valorTotalFactura.Text == "")
                    {
                        txt_valorTotalFactura.Text = "0";
                        txt_descuento.Text = "0";
                    }
                    else
                    {/*
                        if (col_index == 3)
                        {
                            found.SalePrice = edit;
                            found.SalePricePercent = ((edit - found.PurchasePrice) / found.PurchasePrice);
                        }*/
                        float descuento = ((ValTF - value) / ValTF) * 100;

                        if (descuento < 0) descuento = 0;

                        txt_valorTotalFactura.Text = cadena;

                        txt_descuento.Text = Math.Round(descuento).ToString();
                    }
                }
            }
        }
    }
}
