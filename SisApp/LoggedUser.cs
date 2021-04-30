using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SQLite;
using DataModels;

namespace SisApp
{
    class LoggedUser
    {
        readonly SisAppCompactDB db = new SisAppCompactDB("ConnStr");

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public static string Rol { get; set; }

        public LoggedUser(int id)
        {
            try
            {
                var user = db.Users.First(us => us.Id.Equals(id));
                var roles = db.Rols.First(rol => rol.Id.Equals(user.RoleId));
                
                Id = (int)user.Id;
                Nombre = user.Name;
                Apellido = user.LastName;
                Rol = roles.RolColumn;
            }
            catch
            {
                Id = 0;
                Nombre = null;
                Apellido = null;
                Rol = null;
            }
        }
    }
}
