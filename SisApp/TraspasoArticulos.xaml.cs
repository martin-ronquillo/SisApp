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
    /// Lógica de interacción para TraspasoArticulos.xaml
    /// </summary>
    public partial class TraspasoArticulos : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        BackgroundWorker worker = new BackgroundWorker();
        IngresaProductos editCell;

        //Almacena el valor total de la factura
        float ValTF;

        bool validate = true;

        float ValorTotalFactura;

        public TraspasoArticulos()
        {
            InitializeComponent();
            
            Singleton.Instancia.listaIngresos.Clear();

            this.WindowState = WindowState.Maximized;

            LlenaCombos();

            dp_traspaso.DisplayDateEnd = DateTime.Today;
            dp_traspaso.SelectedDate = DateTime.Today;

            pbStatus.Visibility = Visibility.Collapsed;

            btn_Guardar.IsEnabled = false;
        }

        private void LlenaCombos()
        {
            foreach (var item in db.Stores)
            {
                cbBox_almacenFrom.Items.Add(item.StoreName);
                cbBox_almacenTo.Items.Add(item.StoreName);
            }
        }

        private void cbBox_almacenFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActualizaDG();
        }

        private void btn_seleccionaProducto_Click(object sender, RoutedEventArgs e)
        {
            if (cbBox_almacenFrom.SelectedItem != null & cbBox_almacenTo.SelectedItem != null)
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
                if (cbBox_almacenFrom.SelectedItem != "" & cbBox_almacenFrom.SelectedItem != null)
                {
                    dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;

                    ActualizaDG();
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
                if (col_index == 2)
                {
                    found.Amount = 1;
                    found.TotalPrice = found.PurchasePrice * found.Amount;
                }
            }
            else
            {
                //Convierte el valor editado a float
                float edit = float.Parse(cadena);

                var almacenFrom = db.Stores.First(alm => alm.StoreName.Equals(cbBox_almacenFrom.SelectedItem.ToString()));

                //Busca el producto editado en la lista
                var found = Singleton.Instancia.listaIngresos.FirstOrDefault(fo => fo.Id == editCell.Id);
                var productoStore = db.ProductsStores.FirstOrDefault(pro => pro.StoreId.Equals(almacenFrom.Id) & pro.ProductId.Equals(found.Id));

                //Si se edita la Cantidad
                if (col_index == 2)
                {
                    if (productoStore.Stock >= edit)
                    {
                        found.Amount = edit;
                        found.TotalPrice = edit * found.PurchasePrice;
                    }
                    else
                    {
                        MessageBox.Show("El producto solo dispone de: " + productoStore.Stock + " mercancias en stock");
                        found.Amount = 1;
                        found.TotalPrice = found.Amount * found.PurchasePrice;
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
        }

        private void btn_limpiar_Click(object sender, RoutedEventArgs e)
        {
            cbBox_almacenFrom.SelectedItem = "";
            cbBox_almacenTo.SelectedItem = "";
            btn_Guardar.IsEnabled = false;

            Singleton.Instancia.listaIngresos.Clear();
            dg_datos.ItemsSource = null;
            dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;
            dg_datos.Items.Refresh();
        }

        private void btn_Guardar_Click(object sender, RoutedEventArgs e)
        {
            //Deshabilita el estado de todos los botones y controles del form
            cbBox_almacenFrom.IsEnabled = false;
            cbBox_almacenTo.IsEnabled = false;
            dp_traspaso.IsEnabled = false;
            btn_seleccionaProducto.IsEnabled = false;
            dg_datos.IsReadOnly = true;
            btn_Guardar.IsEnabled = false;

            ValidaForm();
            if (validate)
            {
                if (dg_datos.Items.Count != 0)
                {
                    //Muestra el estado del trabajo en segundo plano
                    pbStatus.Visibility = Visibility.Visible;
                    lbl_saveInfo.Content = "Generando nueva compra";

                    Random r = new Random();
                    string transferCode = r.Next(1000, 9999999).ToString("D7");

                    //Genera el ingreso
                    try
                    {
                        Transfer transfer = new Transfer
                        {
                            TransferCode = transferCode,
                            TransferDate = dp_traspaso.Text,
                            TotalPriceTransfer = Math.Round((float.Parse(txt_valorTotalFactura.Text)), 2),
                            FromStoreId = db.Stores.First(sto => sto.StoreName.Equals(cbBox_almacenFrom.SelectedItem.ToString().ToUpper())).Id,
                            ToStoreId = db.Stores.First(sto => sto.StoreName.Equals(cbBox_almacenTo.SelectedItem.ToString().ToUpper())).Id
                        };

                        db.Insert(transfer);
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

        public void ValidaForm()
        {
            validate = true;

            if (cbBox_almacenFrom.SelectedItem == "" | cbBox_almacenFrom.SelectedItem == null)
            {
                validate = false;
            }

            if (cbBox_almacenTo.SelectedItem == "" | cbBox_almacenTo.SelectedItem == null)
            {
                validate = false;
            }

            if (dp_traspaso.Text == "" | dp_traspaso.Text == null)
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

        public void ActualizaDG()
        {
            try
            {
                ValorTotalFactura = 0;

                var Almacen = db.Stores.First(alm => alm.StoreName.Equals(cbBox_almacenFrom.SelectedItem.ToString().ToUpper()));
                var ProductStore = db.ProductsStores.Where(ps => ps.StoreId.Equals(Almacen.Id));
                List<String> listaProductosSinVinculo = new List<String>();

                foreach (var item in dg_datos.Items.OfType<IngresaProductos>())
                {
                    var vinculo = ProductStore.FirstOrDefault(ps => ps.ProductId.Equals(item.Id));

                    if (vinculo != null)
                    {
                        var found = Singleton.Instancia.listaIngresos.FirstOrDefault(fo => fo.Id == item.Id);

                        found.PurchasePrice = (float)vinculo.PurchasePrice;
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

                    MessageBox.Show("Los siguientes productos no possen inventario en el almacen " + cbBox_almacenFrom.SelectedItem.ToString() + "." +
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

        //Trabajo que ejecutara la tarea en segundo plano
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var lastTransfer = db.Transfers.OrderByDescending(tra => tra.Id).FirstOrDefault();
                float totalPriceTransfer = 0;

                var productStoreSustract = db.ProductsStores.Where(ps => ps.StoreId.Equals(lastTransfer.FromStoreId));
                var productStoreAdd = db.ProductsStores.Where(ps => ps.StoreId.Equals(lastTransfer.ToStoreId));

                //Variables de control de progreso de la tarea
                float xd = 100 / dg_datos.Items.Count;
                int i = (int)Math.Round(xd);
                int i2 = 0;
                int anterior = 0;

                foreach (var item in dg_datos.Items.OfType<IngresaProductos>())
                {
                    anterior = i2;
                    i2 = anterior + i;

                    ProductsTransfer productsTransfer = new ProductsTransfer
                    {
                        ProductId = item.Id,
                        TransferId = lastTransfer.Id,
                        Amount = Math.Round(item.Amount, 2),
                        PurchasePrice = Math.Round(item.PurchasePrice, 2)
                    };

                    db.Insert(productsTransfer);

                    //Suma los productos al almacen al que se transfirieron
                    var vinculo = productStoreAdd.LoadWith(t => t.Store).FirstOrDefault(ps => ps.ProductId.Equals(item.Id));

                    if (vinculo != null)
                    {
                        vinculo.Stock = vinculo.Stock + Math.Round(item.Amount, 2);

                        db.Update(vinculo);
                    }
                    else
                    {
                        ProductsStore productsStore = new ProductsStore
                        {
                            StoreId = lastTransfer.ToStoreId,
                            ProductId = item.Id,
                            Stock = Math.Round(item.Amount, 2),
                            SalePricePercent = 1.35,
                            PriceByUnit = Math.Round(item.PurchasePrice * 1.35, 2),
                            PurchasePrice = Math.Round(item.PurchasePrice, 2)
                        };

                        db.Insert(productsStore);
                    }

                    //Resta los productos del almacen del que se transfirieron
                    var vinculo2 = productStoreSustract.FirstOrDefault(ps => ps.ProductId.Equals(item.Id));

                    if (vinculo2 != null)
                    {
                        if (vinculo2.Stock >= item.Amount)
                        {
                            vinculo2.Stock = vinculo2.Stock - Math.Round(item.Amount, 2);
                        }
                        else
                        {
                            vinculo2.Stock = 0;
                        }

                        db.Update(vinculo2);
                    }

                    totalPriceTransfer = totalPriceTransfer + item.TotalPrice;

                    (sender as BackgroundWorker).ReportProgress(i2);
                }
                lastTransfer.TotalPriceTransfer = Math.Round(totalPriceTransfer, 2);

                db.Update(lastTransfer);

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
            cbBox_almacenFrom.IsEnabled = true;
            cbBox_almacenTo.IsEnabled = true;
            dp_traspaso.IsEnabled = true;
            btn_seleccionaProducto.IsEnabled = true;
            dg_datos.IsReadOnly = false;
            btn_Guardar.IsEnabled = false;

            cbBox_almacenFrom.SelectedItem = "";
            cbBox_almacenTo.SelectedItem = "";
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

        }
        //Muestra el reporte de la tarea en segundo plano
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbl_saveInfo.Content = "Moviendo la mercaderia: " + e.ProgressPercentage + "%";

            pbStatus.Value = e.ProgressPercentage;
        }
    }
}
