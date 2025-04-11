using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Servicios
{
    public static class RolPermisos
    {
        public static bool TienePermiso(string permisosBuscar, string permisos)
        {
            bool result = false;
            string[] arregloPermisos = permisosBuscar.Split(",");
            Dictionary<string, bool> diccionarioPermisos = JsonConvert.DeserializeObject<Dictionary<string, bool>>(permisos);
            foreach (string permiso in arregloPermisos)
            {
                diccionarioPermisos.TryGetValue(permiso, out result);
                if (result == true)
                {
                    break;
                }
            }
            return result;
        }
    }
}
