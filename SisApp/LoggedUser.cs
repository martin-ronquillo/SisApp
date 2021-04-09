using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SisApp
{
    class LoggedUser
    {
        public static string miConexion = ConfigurationManager.ConnectionStrings["SisApp.Properties.Settings.SisAppConnectionString"].ConnectionString;

        DataClasses1DataContext dataContext = new DataClasses1DataContext(miConexion);

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Cargo { get; set; }

        public LoggedUser(int id)
        {
            try
            {
                User user = dataContext.User.First(us => us.Id.Equals(id));

                Id = user.Id;
                Nombre = user.nombre;
                Apellido = user.apellido;
                Cargo = user.cargo;
            }
            catch
            {
                Id = 0;
                Nombre = null;
                Apellido = null;
                Cargo = 0;
            }
        }
    }
}
