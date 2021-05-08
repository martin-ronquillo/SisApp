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
using System.Windows.Threading;
using DataModels;
using LinqToDB;

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para ConsultaIE.xaml
    /// </summary>
    public partial class ConsultaIE : Window
    {
        //Id del ingreso o egreso
        private int Consulta;
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        List<ConsultaIngEgr> listaRegistros = new List<ConsultaIngEgr>();
        GeneraExcel GeneraExcel = new GeneraExcel();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public ConsultaIE(int Consulta)
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            //Si el valor recibido es 1, las consultas seran sobre Ingresos, sino, seran en Egresos
            this.Consulta = Consulta;

            if (Consulta == 1)
            {
                lbl_tipoConsulta.Content = "CONSULTAR INGRESOS";
            }
            else
            {
                lbl_tipoConsulta.Content = "CONSULTAR EGRESOS";
            }
        }

        private void btn_consulta_Click(object sender, RoutedEventArgs e)
        {
            ConsultaRegistros();
        }

        private void lv_registros_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MuestraRegistroSeleccionado();
        }

        private void lv_registros_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MuestraRegistroSeleccionado();
            }
        }

        public void ConsultaRegistros()
        {
            listaRegistros.Clear();

            if (Consulta == 1)
            {
                var registros = db.Receipts.LoadWith(t => t.Store).Where(re => re.ReceiptDate.Equals(dp_fechaConsulta.Text));

                foreach (var registro in registros)
                {
                    listaRegistros.Add(
                        new ConsultaIngEgr
                        {
                            Id = (int)registro.Id,
                            Code = registro.ReceiptCode,
                            Store = registro.Store.StoreName,
                            Total = (float)registro.TotalPriceReceipt,
                            Type = registro.Type
                        }
                    );
                }
            }
            else
            {
                var registros = db.Egresses.LoadWith(t => t.Store).Where(re => re.EgressDate.Equals(dp_fechaConsulta.Text));

                foreach (var registro in registros)
                {
                    listaRegistros.Add(
                        new ConsultaIngEgr
                        {
                            Id = (int)registro.Id,
                            Code = registro.EgressCode,
                            Store = registro.Store.StoreName,
                            Total = (float)registro.TotalPriceEgress,
                            Type = registro.Type
                        }
                    );
                }
            }

            lv_registros.ItemsSource = null;
            lv_registros.ItemsSource = listaRegistros;
        }

        public void MuestraRegistroSeleccionado()
        {
            ConsultaIngEgr selectedRegistro = (ConsultaIngEgr)lv_registros.SelectedItem;

            if (selectedRegistro != null)
            {
                MuestraRegistro muestraRegistro = new MuestraRegistro(selectedRegistro.Id, Consulta);

                muestraRegistro.Show();
            }
        }

        private void btn_generaExcel_Click(object sender, RoutedEventArgs e)
        {
            if (dp_fechaConsulta.Text != "" & dp_fechaConsulta.Text != null)
            {
                //Desactiva el boton de "generar excel" durante 5 segundos luego de pulsarlo
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 5);

                btn_generaExcel.IsEnabled = false;

                dispatcherTimer.Start();

                if (Consulta == 1)
                {
                    GeneraExcel.CreaExcel(1, dp_fechaConsulta.Text);
                }
                else
                {
                    GeneraExcel.CreaExcel(2, dp_fechaConsulta.Text);
                }
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //Things which happen after 1 timer interval
            btn_generaExcel.IsEnabled = true;
            //Disable the timer
            dispatcherTimer.IsEnabled = false;
        }
    }
}
