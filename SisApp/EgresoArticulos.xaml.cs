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
    /// Lógica de interacción para EgresoArticulos.xaml
    /// </summary>
    public partial class EgresoArticulos : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        BackgroundWorker worker = new BackgroundWorker();

        IngresaProductos editCell;
        bool validate = true;

        public EgresoArticulos()
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;

            LlenaCombos();

            dp_egreso.DisplayDateEnd = DateTime.Today;
            dp_egreso.SelectedDate = DateTime.Today;

            int nEgr = db.Egresses.Count() + 1;
            txt_nEgreso.Text = nEgr.ToString("D7");

            pbStatus.Visibility = Visibility.Collapsed;
            btn_Guardar.IsEnabled = false;
        }

        private void btn_seleccionaProducto_Click(object sender, RoutedEventArgs e)
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

        private void btn_limpiar_Click(object sender, RoutedEventArgs e)
        {
            Singleton.Instancia.listaIngresos.Clear();
            dg_datos.ItemsSource = null;
            dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;
            dg_datos.Items.Refresh();
        }

        public void LlenaCombos()
        {
            foreach (Store almacen in db.Stores)
            {
                cbBox_almacen.Items.Add(almacen.StoreName);
            }

            cbBox_tipoEgreso.Items.Add("INVENTARIO INICIAL");
            cbBox_tipoEgreso.Items.Add("REGALO");
            cbBox_tipoEgreso.Items.Add("DAÑADO");
        }

        private void btn_Guardar_Click(object sender, RoutedEventArgs e)
        {
            btn_Guardar.IsEnabled = false;

            ValidaForm();
            if (validate)
            {
                if (dg_datos.Items.Count != 0)
                {
                    //Muestra el estado del trabajo en segundo plano
                    pbStatus.Visibility = Visibility.Visible;
                    lbl_saveInfo.Content = "Generando egreso";

                    Random r = new Random();
                    string egressCode = r.Next(1000, 9999999).ToString("D7");

                    //Genera el egreso
                    try
                    {
                        Egress egress = new Egress
                        {
                            Type = cbBox_tipoEgreso.SelectedItem.ToString().ToUpper(),
                            EgressDate = dp_egreso.Text,
                            StoreId = db.Stores.First(sto => sto.StoreName.Equals(cbBox_almacen.SelectedItem.ToString().ToUpper())).Id,
                            EgressCode = egressCode,
                            UserId = Singleton.Instancia.idUser
                        };

                        db.Insert(egress);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("Error: " + exc);
                    }

                    //Deshabilita el estado de todos los botones y controles del form
                    cbBox_almacen.IsEnabled = false;
                    cbBox_tipoEgreso.IsEnabled = false;
                    dp_egreso.IsEnabled = false;
                    btn_seleccionaProducto.IsEnabled = false;
                    dg_datos.IsReadOnly = true;
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
                    MessageBox.Show("No se puede guardar los datos, seleccione un Almacen, un Tipo de Egreso y una Fecha");
                    btn_Guardar.IsEnabled = true;
                }
            }
        }
        //Cuando se completa la tarea en segundo plano
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Habilita todos los botones y controles
            cbBox_almacen.IsEnabled = true;
            cbBox_tipoEgreso.IsEnabled = true;
            dp_egreso.IsEnabled = true;
            btn_seleccionaProducto.IsEnabled = true;
            dg_datos.IsReadOnly = false;
            //Resetea la lista de los ingresos
            Singleton.Instancia.listaIngresos.Clear();
            dg_datos.ItemsSource = null;
            dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;
            dg_datos.Items.Refresh();
            //Resetea la informacion del estado de almacenamiento
            lbl_saveInfo.Content = "";
            pbStatus.Value = 0;
            pbStatus.Visibility = Visibility.Collapsed;

            int nEgr = db.Egresses.Count() + 1;
            txt_nEgreso.Text = nEgr.ToString("D7");
        }
        //Trabajo que ejecutara la tarea en segundo plano
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var lastEgress = db.Egresses.OrderByDescending(eg => eg.Id).FirstOrDefault();
                float totalPriceEgress = 0;

                var productStore = db.ProductsStores.Where(ps => ps.StoreId.Equals(lastEgress.StoreId));

                //Variables de control de progreso de la tarea
                float xd = 100 / dg_datos.Items.Count;
                int i = (int)Math.Round(xd);
                int i2 = 0;
                int anterior = 0;

                foreach (var item in dg_datos.Items.OfType<IngresaProductos>())
                {
                    anterior = i2;
                    i2 = anterior + i;

                    EgressProduct egressProduct = new EgressProduct
                    {
                        ProductId = item.Id,
                        EgressId = lastEgress.Id,
                        Amount = Math.Round(item.Amount, 2),
                        PurchasePrice = Math.Round(item.PurchasePrice, 2)
                    };

                    db.Insert(egressProduct); //Suma los productos ingresados al inventario total del producto

                    var producto = db.Products.First(pro => pro.Id.Equals(item.Id));

                    if (producto.Stock >= item.Amount)
                    {
                        producto.Stock = producto.Stock - Math.Round(item.Amount, 2);
                    }

                    db.Update(producto);

                    //Resta los productos egresados al inventario del almacen correspondiente
                    var vinculo = productStore.FirstOrDefault(ps => ps.ProductId.Equals(item.Id));

                    if (vinculo != null)
                    {
                        if (vinculo.Stock >= item.Amount)
                        {
                            vinculo.Stock = vinculo.Stock - Math.Round(item.Amount, 2);
                            db.Update(vinculo);
                        }
                    }

                    totalPriceEgress = totalPriceEgress + item.TotalPrice;

                    (sender as BackgroundWorker).ReportProgress(i2);
                }
                //Almacena el valor total del ingreso en la tabla Receipts
                lastEgress.TotalPriceEgress = Math.Round(totalPriceEgress, 2);
                db.Update(lastEgress);

                //Envia el reporte de progreso de la tarea en segundo plano
                (sender as BackgroundWorker).ReportProgress(i2 + i);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }
        //Muestra el reporte de la tarea en segundo plano
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbl_saveInfo.Content = "Vinculando productos al egreso: " + e.ProgressPercentage + "%";

            pbStatus.Value = e.ProgressPercentage;
        }

        private void cbBox_almacen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
                if (col_index == 4)
                {
                    found.Amount = 1;
                }
            }
            else
            {
                //Convierte el valor editado a float
                float edit = float.Parse(cadena);

                //Busca el producto editado en la lista
                var found = Singleton.Instancia.listaIngresos.FirstOrDefault(fo => fo.Id == editCell.Id);

                //Si se edita la cantidad
                if (col_index == 4)
                {
                    found.Amount = edit;
                    found.TotalPrice = edit * found.PurchasePrice;
                }
            }

            //Obtiene el indice de la fila editada
            //DataGridRow row1 = e.Row;
            //int row_index = ((DataGrid)sender).ItemContainerGenerator.IndexFromContainer(row1);

            dg_datos.ItemsSource = null;
            dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;

            if (dg_datos.Items.Count == 0)
            {
                btn_Guardar.IsEnabled = false;
            }
            else
            {
                btn_Guardar.IsEnabled = true;
            }
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

                            found.SalePricePercent = (float)vinculo.SalePricePercent;
                            found.SalePrice = (float)vinculo.PriceByUnit;
                            found.TotalPrice = item.Amount * found.PurchasePrice;
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
            catch (Exception exc)
            {
                ActualizaDG();
            }
            dg_datos.ItemsSource = null;

            dg_datos.ItemsSource = Singleton.Instancia.listaIngresos;
        }

        public void ValidaForm()
        {
            validate = true;

            if (cbBox_almacen.SelectedItem == "" | cbBox_almacen.SelectedItem == null)
            {
                validate = false;
            }

            if (cbBox_tipoEgreso.SelectedItem == "" | cbBox_tipoEgreso.SelectedItem == null)
            {
                validate = false;
            }

            if (dp_egreso.Text == "" | dp_egreso.Text == null)
            {
                validate = false;
            }

            if (dg_datos.Items.Count == 0)
            {
                validate = false;
            }
        }
    }
}
