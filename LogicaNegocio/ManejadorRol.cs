using AccesoDatos;
using LogicaNegocio.Models;
using System.Data;

namespace LogicaNegocio
{
    public class ManejadorRol
    {
        private Connection conexion = new Connection();

        public bool CrearRol(string nombre) {
            List<Parametro> parametro = new List<Parametro>()
            { new Parametro("nombre", nombre) };
            return conexion.EjecutarTransaccion("crear_rol", parametro);
        }

        public List<Rol> Consultar_roles(int id)
        {
            List<Parametro> parametros = new List<Parametro>()
                {
                    new Parametro("p_id_rol", id)
                };
            DataTable data = conexion.EjecutarConsulta("consultar_rol", parametros);
            List<Rol> roles = new List<Rol>();
            foreach (DataRow row in data.AsEnumerable())
            {
                roles.Add(new Rol()
                {
                    PK_rol = Convert.ToInt32(row["PK_rol"].ToString()),
                    Nombre = row["nombre"].ToString(),
                    Estado = row["estado"].ToString()
                });
            }
            return roles;
        }
    }
}
