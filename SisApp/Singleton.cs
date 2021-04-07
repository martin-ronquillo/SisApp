using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisApp
{
    class Singleton
    {
        public static Singleton Instancia = new Singleton();
    
        private Singleton() { }

        public bool estado = true;
        public int idUser;
    }
}
