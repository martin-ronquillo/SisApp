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
using LinqToDB;
using DataModels;

namespace SisApp
{
    /// <summary>
    /// Lógica de interacción para Categorias.xaml
    /// </summary>
    public partial class Categorias : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        bool Edit = false;

        public Categorias()
        {
            InitializeComponent();

            lbl_Categoria.Content = "NUEVA CATEGORIA";

            try
            {
                int nCat = db.Categories.Count() + 1;

                txt_nCategoria.Text = nCat.ToString("D7");
            }
            catch
            {
                int nCat = 1;

                txt_nCategoria.Text = nCat.ToString("D7");
            }
        }

        private void btn_limpiar_Click(object sender, RoutedEventArgs e)
        {
            txt_descripcionCategoria.Text = "";
        }

        private void btn_guardar_Click(object sender, RoutedEventArgs e)
        {
            if (txt_descripcionCategoria.Text != "")
            {
                try
                {
                    Category category = new Category
                    {
                        CategoryName = txt_descripcionCategoria.Text.ToUpper()
                    };

                    db.Insert(category);

                    this.Close();
                }
                catch
                {
                    MessageBox.Show("No se pudo guardar la informacion");
                }
            }
            else
            {
                MessageBox.Show("El campo 'Descripcion de la categoria' no puede estar vacio");
            }
        }

        private void btn_cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
