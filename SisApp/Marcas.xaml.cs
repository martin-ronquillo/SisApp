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
    /// Lógica de interacción para Marcas.xaml
    /// </summary>
    public partial class Marcas : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        bool Edit = false;

        public Marcas()
        {
            InitializeComponent();

            lbl_Marcas.Content = "NUEVA MARCA";

            int nMar = db.TradeMarks.Count() + 1;

            txt_nMarca.Text = nMar.ToString("D7");
        }

        private void btn_limpiar_Click(object sender, RoutedEventArgs e)
        {
            txt_descripcionMarca.Text = "";
        }

        private void btn_guardar_Click(object sender, RoutedEventArgs e)
        {
            if (txt_descripcionMarca.Text == "")
            {
                try
                {
                    TradeMark tradeMark = new TradeMark
                    {
                        MarkName = txt_descripcionMarca.Text.ToUpper()
                    };

                    db.Insert(tradeMark);
                }
                catch(Exception exc)
                {
                    MessageBox.Show("No se guardaron los datos. Error: \n" + exc);
                }
            }
            else
            {
                MessageBox.Show("El campo 'Descripcion de la marca' no puede estar vacio");
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
