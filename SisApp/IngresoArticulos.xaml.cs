using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para IngresoArticulos.xaml
    /// </summary>
    public partial class IngresoArticulos : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        IngresaProductos editCell;
        bool validate = true;

        public IngresoArticulos()
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;

            LlenaCombos();

            dp_ingreso.DisplayDateEnd = DateTime.Today;
            dp_ingreso.SelectedDate = DateTime.Today;

            int nIng = db.Receipts.Count() + 1;
            txt_nIngreso.Text = nIng.ToString("D7");

            btn_Guardar.IsEnabled = false;
        }

        private void btn_seleccionaProducto_Click(object sender, RoutedEventArgs e)
        {
            if (dg_datos.Items.Count == 0)
            {
                SeleccionaIngreso seleccionaIngreso = new SeleccionaIngreso();
                WindowState = WindowState.Minimized;

                seleccionaIngreso.ShowDialog();
            }
            else
            {
                SeleccionaIngreso seleccionaIngreso = new SeleccionaIngreso(dg_datos);
                WindowState = WindowState.Minimized;

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
            LimpiaForm();
        }

        public void LlenaCombos()
        {
            foreach (Store almacen in db.Stores)
            {
                cbBox_almacen.Items.Add(almacen.StoreName);
            }

            cbBox_tipoIngreso.Items.Add("INVENTARIO INICIAL");
            cbBox_tipoIngreso.Items.Add("REGALO");
        }

        private void btn_Guardar_Click(object sender, RoutedEventArgs e)
        {
            btn_Guardar.IsEnabled = false;

            ValidaForm();
            if (validate)
            {
                if (dg_datos.Items.Count != 0)
                {
                    try
                    {
                        Random r = new Random();
                        string receiptCode = r.Next(1000,9999999).ToString("D7");

                        //Genera el ingreso
                        Receipt receipt = new Receipt
                        {
                            Type = cbBox_tipoIngreso.SelectedItem.ToString().ToUpper(),
                            ReceiptDate = dp_ingreso.Text,
                            StoreId = db.Stores.First(sto => sto.StoreName.Equals(cbBox_almacen.SelectedItem.ToString().ToUpper())).Id,
                            ReceiptCode = receiptCode,
                            UserId = Singleton.Instancia.idUser
                        };

                        db.Insert(receipt);

                        var lastReceipt = db.Receipts.OrderByDescending(re => re.Id).FirstOrDefault();
                        float totalPriceReceipt = 0;

                        var productStore = db.ProductsStores.Where(ps => ps.StoreId.Equals(lastReceipt.StoreId));

                        foreach (var item in dg_datos.Items.OfType<IngresaProductos>())
                        {
                            //Vincula el ingreso con los productos ingresados
                            ProductsReceipt productsReceipt = new ProductsReceipt
                            {
                                ProductId = item.Id,
                                ReceiptId = lastReceipt.Id,
                                Amount = item.Amount,
                                PurchasePrice = Math.Round(item.PurchasePrice, 2)
                            };

                            db.Insert(productsReceipt);

                            //Suma los productos ingresados al inventario total del producto
                            var producto = db.Products.First(pro => pro.Id.Equals(item.Id));

                            producto.Stock = producto.Stock + item.Amount;

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
                                vinculo.Stock = vinculo.Stock + item.Amount;
                                vinculo.PriceByUnit = Math.Round(item.SalePrice, 2);
                                vinculo.PurchasePrice = Math.Round(item.PurchasePrice, 2);
                                vinculo.SalePricePercent = Math.Round(item.SalePricePercent, 2);

                                db.Update(vinculo);
                            }
                            else
                            {
                                ProductsStore proSto = new ProductsStore()
                                {
                                    StoreId = lastReceipt.StoreId,
                                    ProductId = item.Id,
                                    Stock = item.Amount,
                                    PriceByUnit = Math.Round(item.SalePrice, 2),
                                    PurchasePrice = Math.Round(item.PurchasePrice, 2),
                                    SalePricePercent = Math.Round(item.SalePricePercent, 2)
                            };

                                db.Insert(proSto);
                            }

                            totalPriceReceipt = totalPriceReceipt + item.TotalPrice;
                        }
                        //Almacena el valor total del ingreso en la tabla Receipts
                        lastReceipt.TotalPriceReceipt = Math.Round(totalPriceReceipt, 2);
                        db.Update(lastReceipt);

                        LimpiaForm();

                        btn_Guardar.IsEnabled = true;
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("No se pudo generar el ingreso \n Excepcion COntrolada: \n" + exc);
                        btn_Guardar.IsEnabled = true;
                    }
                }
            }
            else
            {
                MessageBox.Show("No se puede guardar los datos, seleccione un Almacen, un Tipo de Ingreso y una Fecha");
                btn_Guardar.IsEnabled = true;
            }
        }

        public void LimpiaForm()
        {
            dg_datos.ItemsSource = null;
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

                //Si se edita la Ganancia
                if (col_index == 2)
                {
                    found.SalePricePercent = edit;
                    found.SalePrice = (found.PurchasePrice * edit) + found.PurchasePrice;
                }
                //Si se edita el P.v.P
                if (col_index == 3)
                {
                    found.SalePrice = edit;
                    found.SalePricePercent = ((edit - found.PurchasePrice) / found.PurchasePrice);
                }
                //Si se edita la cantidad
                if (col_index == 4)
                {
                    found.Amount = edit;
                    found.TotalPrice = edit * found.PurchasePrice;
                }
                //Si se edita el valor de compra
                if (col_index == 5)
                {
                    found.PurchasePrice = edit;
                    found.TotalPrice = edit * found.Amount;
                    found.SalePricePercent = ((found.SalePrice - edit) / edit);
                    //found.SalePrice = (edit * found.SalePricePercent) + edit;
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
            try
            {
                var Almacen = db.Stores.First(alm => alm.StoreName.Equals(cbBox_almacen.SelectedItem.ToString().ToUpper()));
                var ProductStore = db.ProductsStores.Where(ps => ps.StoreId.Equals(Almacen.Id));

                foreach (var item in dg_datos.Items.OfType<IngresaProductos>())
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

                        found.SalePricePercent = (float)db.Products.First(pro => pro.Id.Equals(item.Id)).SalePricePercent;
                        found.SalePrice = (float)db.Products.First(pro => pro.Id.Equals(item.Id)).SalePrice;
                        found.TotalPrice = item.Amount * item.PurchasePrice;
                    }
                }
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

            if (cbBox_tipoIngreso.SelectedItem == "" | cbBox_tipoIngreso.SelectedItem == null)
            {
                validate = false;
            }

            if (dp_ingreso.Text == "" | dp_ingreso.Text == null)
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
