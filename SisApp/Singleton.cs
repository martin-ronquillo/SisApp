using System.Collections.Generic;

namespace SisApp
{
    class Singleton
    {
        public static Singleton Instancia = new Singleton();

        private Singleton() { }

        //Se usan para el loggin
        public bool estado = true;
        public int idUser;

        //Mantiene el estado del modal "confirmar"
        public bool confirma = false;

        //Se usan para seleccionar cliente a facturar
        public int selectedCliente = 1;

        public List<IngresaProductos> listaIngresos = new List<IngresaProductos>();
    }
}
