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
    /// Lógica de interacción para Compras.xaml
    /// </summary>
    public partial class Compras : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        BackgroundWorker worker = new BackgroundWorker();

        IngresaProductos editCell;
        bool ivaChecked = false;

        //Almacena el valor total de la factura
        float ValTF;

        bool validate = true;

        float ValorTotalFactura;

        public Compras()
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

            chck_boxIva.IsEnabled = false;
            txt_descuento.IsEnabled = false;
            dg_datos.IsReadOnly = true;
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
                if (cbBox_almacen.SelectedItem != "" & cbBox_almacen.SelectedItem != null)
                {
                    dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;

                    ActualizaDG();

                    chck_boxIva.IsEnabled = true;
                    txt_descuento.IsEnabled = true;
                }
                else
                {
                    dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;
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
                
                //Si se edita la Ganancia %
                if (col_index == 2)
                {
                    found.SalePricePercent = 0;
                }
                //Si se edita el Valor de venta
                if (col_index == 3)
                {
                    found.SalePrice = 0;
                }
                //Si se edita la cantidad
                if (col_index == 4)
                {
                    found.Amount = 1;
                }
                //Si se edita el valor de compra
                if (col_index == 5)
                {
                    found.PurchasePrice = 0;
                    found.TotalPrice = 0 * found.Amount;
                }
            }
            else
            {
                //Convierte el valor editado a float
                float edit = float.Parse(cadena);

                //Busca el producto editado en la lista
                var found = Singleton.Instancia.listaIngresos.FirstOrDefault(fo => fo.Id == editCell.Id);

                //Si se edita la Ganancia %
                if (col_index == 2)
                {
                    found.SalePricePercent = edit;
                    found.SalePrice = (found.PurchasePrice * edit) + found.PurchasePrice;
                }
                //Si se edita el V. Venta
                if (col_index == 3)
                {
                    found.SalePrice = edit;
                    found.SalePricePercent = ((edit - found.PurchasePrice) / found.PurchasePrice);
                }
                //Si se edita la Cantidad
                if (col_index == 4)
                {
                    found.Amount = edit;
                    found.TotalPrice = found.Amount * found.PurchasePrice;
                }
                //Si se edita el Valor de Compra
                if (col_index == 5)
                {
                    found.PurchasePrice = edit;
                    found.TotalPrice = found.Amount * edit;

                    float iva = edit * (float)0.12;
                    float subtotal = edit - iva;

                    found.SubTotal = subtotal;
                    found.Tax = iva;

                    found.SalePrice = (found.PurchasePrice * found.SalePricePercent) + found.PurchasePrice;

                    ValorTotalFactura = 0;
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
                chck_boxIva.IsEnabled = true;
            }
            else
            {
                txt_descuento.IsEnabled = false;
                chck_boxIva.IsEnabled = false;
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

        private void txt_descuento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter | e.Key == Key.Tab)
            {
                int value;
                //Si ingresa un caracter no numerico se reestablece a 0
                if (!int.TryParse(txt_descuento.Text, out value))
                {
                    txt_descuento.Text = "0";
                }
                else
                {
                    //Si se ingresa el 0 o un vacio, se reestablece a 0
                    if (txt_descuento.Text == "0" | txt_descuento.Text == "")
                    {
                        txt_descuento.Text = "0"; 
                        
                        if (ivaChecked)
                        {
                            float valorSinIva = ValTF - (ValTF * (float)0.12);
                            txt_valorTotalFactura.Text = valorSinIva.ToString();
                        }
                        else
                        {
                            txt_valorTotalFactura.Text = ValTF.ToString();
                        }
                    }
                    else
                    {
                        //Si el check "documento sin iva" esta marcado, se calcula el documento con descuento y sin iva,
                        //caso contrario se calcula el documento con descuento y con iva
                        if (ivaChecked)
                        {
                            float valorDescontado = ValTF - (ValTF * ((float)value / 100));
                            float valorDescontadoSinIva = valorDescontado - (valorDescontado * (float)0.12);
                            txt_valorTotalFactura.Text = valorDescontadoSinIva.ToString();
                        }
                        else
                        {
                            float valorDescontado = ValTF - (ValTF * ((float)value / 100));
                            txt_valorTotalFactura.Text = valorDescontado.ToString();
                        }
                    }
                }
            }
        }

        private void btn_Guardar_Click(object sender, RoutedEventArgs e)
        {
            //Deshabilita el estado de todos los botones y controles del form
            cbBox_almacen.IsEnabled = false;
            cbBox_proveedor.IsEnabled = false;
            dp_compra.IsEnabled = false;
            btn_seleccionaProducto.IsEnabled = false;
            dg_datos.IsReadOnly = true;
            chck_boxIva.IsEnabled = false;
            txt_codigoCompra.IsEnabled = false;
            btn_Guardar.IsEnabled = false;

            ValidaForm();
            if (validate)
            {
                var code = db.Purchases.FirstOrDefault(cod => cod.PurchaseCode.Equals(txt_codigoCompra.Text));

                if (code == null)
                {
                    if (dg_datos.Items.Count != 0)
                    {
                        //Muestra el estado del trabajo en segundo plano
                        pbStatus.Visibility = Visibility.Visible;
                        lbl_saveInfo.Content = "Generando nueva compra";

                        //Genera el ingreso
                        try
                        {
                            float subprice = (float.Parse(txt_valorTotalFactura.Text)) - (float.Parse(txt_valorTotalFactura.Text) * (float)0.12);
                            float tax = float.Parse(txt_valorTotalFactura.Text) * (float)0.12;

                            Purchase purchase = new Purchase
                            {
                                PurchaseCode = txt_codigoCompra.Text.ToUpper(),
                                PurchaseDate = dp_compra.Text,
                                TotalPrice = Math.Round((float.Parse(txt_valorTotalFactura.Text)), 2),
                                SubPrice = Math.Round(subprice, 2),
                                Tax = Math.Round(tax, 2),
                                Discount = (float.Parse(txt_descuento.Text) / 100),
                                ProviderId = db.Providers.First(pro => pro.ProviderName.Equals(cbBox_proveedor.SelectedItem.ToString().ToUpper())).Id,
                                StoreId = db.Stores.First(sto => sto.StoreName.Equals(cbBox_almacen.SelectedItem.ToString().ToUpper())).Id
                            };

                            db.Insert(purchase);
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show("Error: " + exc);
                        }

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
                }
                else
                {
                    MessageBox.Show("Error, el codigo de factura proporcionado ya existe o fue ingresado anteriormente");
                }
            }
            else
            {
                MessageBox.Show("No se puede guardar los datos, Ingrese un Codigo de Factura, seleccione un Almacen, un Proveedor y una Fecha. Recuerde que el valor total no puede ser 0");

                //Habilita todos los botones y controles
                cbBox_almacen.IsEnabled = true;
                cbBox_proveedor.IsEnabled = true;
                dp_compra.IsEnabled = true;
                btn_seleccionaProducto.IsEnabled = true;
                dg_datos.IsReadOnly = false;
                chck_boxIva.IsEnabled = true;
                txt_codigoCompra.IsEnabled = true;
                btn_Guardar.IsEnabled = true;
            }
        }

        private void chck_boxIva_Checked(object sender, RoutedEventArgs e)
        {
            ivaChecked = !ivaChecked;

            if (ivaChecked)
            {
                if (txt_descuento.Text != "0" | txt_descuento.Text != "")
                {
                    float valorDescontado = ValTF - (ValTF * (float.Parse(txt_descuento.Text) / 100));
                    float valorDescontadoSinIva = valorDescontado - (valorDescontado * (float)0.12);
                    txt_valorTotalFactura.Text = valorDescontadoSinIva.ToString();
                }
                else
                {
                    float valorSinIva = ValTF - (ValTF * (float)0.12);
                    txt_valorTotalFactura.Text = valorSinIva.ToString();
                }
            }
            else
            {
                if (txt_descuento.Text != "0" | txt_descuento.Text != "")
                {
                    float valorDescontado = ValTF - (ValTF * (float.Parse(txt_descuento.Text) / 100));
                    txt_valorTotalFactura.Text = valorDescontado.ToString();
                }
                else
                {
                    txt_valorTotalFactura.Text = ValTF.ToString();
                }
            }
        }

        public void LlenaCombos()
        {
            foreach (Store almacen in db.Stores)
            {
                cbBox_almacen.Items.Add(almacen.StoreName);
            }

            foreach (Provider proveedor in db.Providers)
            {
                cbBox_proveedor.Items.Add(proveedor.ProviderName);
            }
        }

        public void ActualizaDG()
        {
            try
            {
                ValorTotalFactura = 0;

                var Almacen = db.Stores.First(alm => alm.StoreName.Equals(cbBox_almacen.SelectedItem.ToString().ToUpper()));
                var ProductStore = db.ProductsStores.Where(ps => ps.StoreId.Equals(Almacen.Id));
                List<String> listaProductosSinVinculo = new List<String>();

                foreach (var item in dg_datos.Items.OfType<IngresaProductos>())
                {
                    var vinculo = ProductStore.FirstOrDefault(ps => ps.ProductId.Equals(item.Id));

                    if (vinculo != null)
                    {
                        var found = Singleton.Instancia.listaIngresos.FirstOrDefault(fo => fo.Id == item.Id);

                        found.PurchasePrice = (float)vinculo.PurchasePrice;
                        found.SalePricePercent = (float)vinculo.SalePricePercent;
                        found.SalePrice = (float)vinculo.PriceByUnit;
                        found.SubTotal = (float)vinculo.PurchasePrice - (float)(vinculo.PurchasePrice * 0.12);
                        found.Tax = (float)(vinculo.PurchasePrice * 0.12);
                        found.TotalPrice = item.Amount * found.PurchasePrice;

                        ValorTotalFactura += item.Amount * found.PurchasePrice;
                    }
                    else
                    {
                        listaProductosSinVinculo.Add(item.ProductName);
                    }
                }

                if (listaProductosSinVinculo.Count() > 0)
                {

                    MessageBox.Show("Los siguientes productos no possen inventario en el almacen " + cbBox_almacen.SelectedItem.ToString() + "." +
                        " No olvide cambiar los valores del producto en caso de ser necesario. \n\n" + 
                        String.Join(Environment.NewLine, listaProductosSinVinculo.Where(x => x != null)));
                }

                ValTF = ValorTotalFactura;

                txt_valorTotalFactura.Text = ValTF.ToString();

                dg_datos.ItemsSource = null;

                dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error, excepcion controlada: \n" + exc);
            }
        }

        public void ValidaForm()
        {
            validate = true;

            if (cbBox_almacen.SelectedItem == "" | cbBox_almacen.SelectedItem == null)
            {
                validate = false;
            }

            if (cbBox_proveedor.SelectedItem == "" | cbBox_proveedor.SelectedItem == null)
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

            if (txt_codigoCompra.Text == "")
            {
                validate = false;
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

            chck_boxIva.IsEnabled = true;
            txt_descuento.IsEnabled = true;
        }

        private void btn_limpiar_Click(object sender, RoutedEventArgs e)
        {
            cbBox_almacen.SelectedItem = "";
            cbBox_proveedor.SelectedItem = "";
            chck_boxIva.IsChecked = false;
            chck_boxIva.IsEnabled = false;
            txt_descuento.Text = "0";
            txt_descuento.IsEnabled = false;
            btn_Guardar.IsEnabled = false;
            txt_codigoCompra.Text = "";

            Singleton.Instancia.listaIngresos.Clear();
            dg_datos.ItemsSource = null;
            dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;
            dg_datos.Items.Refresh();
        }

        //Trabajo que ejecutara la tarea en segundo plano
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var lastPurchase = db.Purchases.OrderByDescending(pur => pur.Id).FirstOrDefault();
                float totalPricePurchase = 0;

                var productStore = db.ProductsStores.Where(ps => ps.StoreId.Equals(lastPurchase.StoreId));

                //Variables de control de progreso de la tarea
                float xd = 100 / dg_datos.Items.Count;
                int i = (int)Math.Round(xd);
                int i2 = 0;
                int anterior = 0;

                foreach (var item in dg_datos.Items.OfType<IngresaProductos>())
                {
                    anterior = i2;
                    i2 = anterior + i;

                    ProductsPurchase productsPurchase = new ProductsPurchase
                    {
                        ProductId = item.Id,
                        PurchaseId = lastPurchase.Id,
                        Amount = Math.Round(item.Amount, 2),
                        PurchasePrice = Math.Round(item.PurchasePrice, 2)
                    };

                    db.Insert(productsPurchase);
                    
                    //Suma los productos ingresados al inventario total del producto
                    var producto = db.Products.First(pro => pro.Id.Equals(item.Id));

                    producto.Stock = producto.Stock + Math.Round(item.Amount, 2);

                    if (producto.PucharsePrice <= 0)
                    {
                        producto.PucharsePrice = Math.Round(item.PurchasePrice, 2);
                    }
                    if (producto.SalePrice <= 0)
                    {
                        producto.SalePrice = Math.Round(item.SalePrice, 2);
                    }
                    if (producto.SalePricePercent <= 0)
                    {
                        producto.SalePricePercent = Math.Round(item.SalePricePercent, 2);
                    }

                    db.Update(producto);

                    //Suma los productos ingresados al inventario del almacen correspondiente
                    var vinculo = productStore.FirstOrDefault(ps => ps.ProductId.Equals(item.Id));

                    if (vinculo != null)
                    {
                        vinculo.Stock = vinculo.Stock + Math.Round(item.Amount, 2);

                        vinculo.PriceByUnit = Math.Round(item.SalePrice, 2);
                        vinculo.SalePricePercent = Math.Round(item.SalePricePercent, 2);
                        vinculo.PurchasePrice = Math.Round(item.PurchasePrice, 2);

                        db.Update(vinculo);
                    }
                    else
                    {
                        ProductsStore proSto = new ProductsStore()
                        {
                            StoreId = lastPurchase.StoreId,
                            ProductId = item.Id,
                            Stock = Math.Round(item.Amount, 2),
                            PriceByUnit = Math.Round(item.SalePrice, 2),
                            PurchasePrice = Math.Round(item.PurchasePrice, 2),
                            SalePricePercent = Math.Round(item.SalePricePercent, 2)
                        };

                        db.Insert(proSto);
                    }

                    totalPricePurchase = totalPricePurchase + item.TotalPrice;

                    (sender as BackgroundWorker).ReportProgress(i2);
                }
                lastPurchase.TotalPrice = Math.Round(totalPricePurchase, 2);

                db.Update(lastPurchase);

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
            cbBox_proveedor.IsEnabled = true;
            dp_compra.IsEnabled = true;
            btn_seleccionaProducto.IsEnabled = true;
            dg_datos.IsReadOnly = false;
            chck_boxIva.IsEnabled = false;
            txt_codigoCompra.IsEnabled = true;
            btn_Guardar.IsEnabled = false;
            txt_descuento.IsEnabled = false;

            cbBox_almacen.SelectedItem = "";
            cbBox_proveedor.SelectedItem = "";
            txt_descuento.Text = "0";
            txt_valorTotalFactura.Text = "0";
            txt_codigoCompra.Text = "";
            chck_boxIva.IsChecked = false;

            //Resetea la lista de los ingresos
            Singleton.Instancia.listaIngresos.Clear();
            dg_datos.ItemsSource = null;
            dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;
            dg_datos.Items.Refresh();
            //Resetea la informacion del estado de almacenamiento
            lbl_saveInfo.Content = "";
            pbStatus.Value = 0;
            pbStatus.Visibility = Visibility.Collapsed;

        }
        //Muestra el reporte de la tarea en segundo plano
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbl_saveInfo.Content = "Vinculando productos a la compra: " + e.ProgressPercentage + "%";

            pbStatus.Value = e.ProgressPercentage;
        }
    }
}
