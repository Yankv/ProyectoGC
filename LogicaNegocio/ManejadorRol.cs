using AccesoDatos;
using LogicaNegocio.Models;
using LogicaNegocio.Utilidad;
using System.Data;

namespace LogicaNegocio
{
    public class ManejadorRol
    {
        private Connection conexion = new Connection();
        private ConexionJena conexionJena = new ConexionJena();

        public bool CrearRol(string nombre)
        {
            List<Parametro> parametro = new List<Parametro>()
            { new Parametro("nombre", nombre) };
            var resultado = conexion.EjecutarTransaccion("crear_rol", parametro);
            if (resultado)
            {
                var ultimoId = Consultar_roles2(0);
                int id = ultimoId[ultimoId.Count - 1].PK_rol;
                var consulta = @"
                INSERT DATA {
                    :Rol" + id + @" a :Rol ;
                        :idRol " + id + @" ;
                        :estadoRol 'Activo' ;
                        :nombreRol '" + nombre + @"' .
                }";
                conexionJena.EjecutarUpdate(consulta);
            }
            return resultado;
        }

        public List<Rol> Consultar_roles(int id)
        {
            string finConsulta;
            if (id == 0)
            {
                finConsulta = @"}
                    ORDER BY ASC(?IdRol)";
            }
            else
            {
                finConsulta = "FILTER (?idRol = " + id + ")}";
            }
            string consulta = @"
                SELECT (STR(?IdRol) AS ?Pk_rol)
		                (STR(?EstadoRol) AS ?estado)
  		                (STR(?NombreRol) AS ?nombre)
                WHERE {
                  ?rol a :Rol;
                    :idRol ?IdRol;
                    :estadoRol ?EstadoRol;
                    :nombreRol ?NombreRol.
                " + finConsulta;

            var json = conexionJena.EjecutarConsultaAsync(consulta).Result;
            DataTable data = SparqlJsonParser.ParseToDataTable(json);

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

        public List<Rol> Consultar_roles2(int id)
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