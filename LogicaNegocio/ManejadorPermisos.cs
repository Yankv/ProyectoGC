using AccesoDatos;
using LogicaNegocio.Models;
using System.Data;

namespace LogicaNegocio
{
    public class ManejadorPermisos
    {
        private Connection conexion = new Connection();

        public List<Permiso> ObtenerPermisos(int idRol)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_id_rol", idRol)
            };
            DataTable data = conexion.EjecutarConsulta("consultar_permisos", parametros);
            List<Permiso> permisos = new List<Permiso>();
            foreach (DataRow row in data.AsEnumerable())
            {
                permisos.Add(new Permiso()
                {
                    Pk_permiso = Convert.ToInt32(row["Pk_permiso"].ToString()),
                    Nombre = row["nombre"].ToString(),
                    Estado = row["ESTADO_PERMISO_ROL"].ToString() == "1"
                });
            }
            return permisos;
        }

        public bool AsignarPermiso(int idRol, int idPermiso)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_id_rol", idRol),
                new Parametro("p_id_permiso", idPermiso)
            };
            return conexion.EjecutarTransaccion("asignar_permiso_rol", parametros);
        }
    }
}
