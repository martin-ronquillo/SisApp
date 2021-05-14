using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para AlmacenesCRUD.xaml
    /// </summary>
    public partial class AlmacenesCRUD : Window
    {
        SisAppCompactDB db = new SisAppCompactDB("ConnStr");
        private int Id = 0;

        public AlmacenesCRUD()
        {
            InitializeComponent();
        }

        public AlmacenesCRUD(int Id)
        {
            this.Id = Id;
            InitializeComponent();

            LlenaInfo();
        }

        public void LlenaInfo()
        {
            try
            {
                var store = db.Stores.First(sto => sto.Id.Equals(Id));

                txt_ruc.Text = store.Ruc;
                txt_storeName.Text = store.StoreName;
                txt_address.Text = store.Direction;
                txt_email.Text = store.Email;
                txt_telephoneOne.Text = store.Telephone;
                txt_telephoneTwo.Text = store.OtherTelephone;
            }
            catch (Exception e)
            {
                MessageBox.Show("No se pudo mostrar los datos. \nExcepcion controlada: \n" + e);
                this.Close();
            }
        }

        private void btn_Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (Valida())
            {
                try
                {
                    if (Id == 0)
                    {
                        Store store = new Store
                        {
                            Ruc = txt_ruc.Text,
                            StoreName = txt_storeName.Text.ToUpper(),
                            Direction = txt_address.Text.ToUpper(),
                            Email = txt_email.Text.ToUpper(),
                            Telephone = txt_telephoneOne.Text.ToUpper(),
                            OtherTelephone = txt_telephoneTwo.Text.ToUpper()
                        };

                        db.Insert(store);
                    }
                    else
                    {
                        var store = db.Stores.First(sto => sto.Id.Equals(Id));

                        store.Ruc = txt_ruc.Text;
                        store.StoreName = txt_storeName.Text.ToUpper();
                        store.Direction = txt_address.Text.ToUpper();
                        store.Email = txt_email.Text.ToUpper();
                        store.Telephone = txt_telephoneOne.Text.ToUpper();
                        store.OtherTelephone = txt_telephoneTwo.Text.ToUpper();

                        db.Update(store);
                    }

                    this.Close();
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Error al guardar los datos. \nExepcion controlada: \n" + exc);
                }
            }
        }

        private void btn_Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool Valida()
        {
            bool valida = true;

            //Valida Ruc
            if (Regex.IsMatch(txt_ruc.Text, "[^0-9]") | string.IsNullOrEmpty(txt_ruc.Text) | txt_ruc.Text.Length != 10 & txt_ruc.Text.Length != 13)
            {
                MessageBox.Show("El campo 'Ruc' es invalido");

                valida = false;
            }
            //Valida el telefono_One
            if (Regex.IsMatch(txt_telephoneOne.Text, "[^0-9]") | txt_telephoneOne.Text.Length < 7 | txt_telephoneOne.Text.Length > 10)
            {
                if (string.IsNullOrEmpty(txt_telephoneOne.Text))
                {

                }
                else
                {
                    MessageBox.Show("El campo 'Telefono 1' debe contener entre 7 y 10 numeros");

                    valida = false;
                }
            }
            //Valida el telefono_Two
            if (Regex.IsMatch(txt_telephoneTwo.Text, "[^0-9]") | txt_telephoneTwo.Text.Length < 7 | txt_telephoneTwo.Text.Length > 10)
            {
                if (string.IsNullOrEmpty(txt_telephoneTwo.Text))
                {

                }
                else
                {
                    MessageBox.Show("El campo 'Telefono 2' debe contener entre 7 y 10 numeros");

                    valida = false;
                }
            }
            //Valida Email
            if (txt_email.Text.Contains("@") & txt_email.Text.Contains(".com"))
            {
                
            }
            else
            {
                if (txt_email.Text == "")
                {

                }
                else
                {
                    MessageBox.Show("El email debe tener el siguiente formato: \nExample@email.com");

                    valida = false;
                }
            }

            return valida;
        }
    }
}
