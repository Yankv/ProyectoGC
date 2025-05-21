using AccesoDatos;
using LogicaNegocio.Models;
using LogicaNegocio.Utilidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio
{
    public class ManejadorRecurso
    {
        private Connection conexion = new Connection();
        private ConexionJena conexionJena = new ConexionJena();

        public bool Crear_recurso(Recurso recurso)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_nombre", recurso.Nombre),
                new Parametro("p_direccion", recurso.Direccion),
                new Parametro("p_fk_tp_recurso", recurso.Fk_tp_recurso.Pk_tp_recurso),
                new Parametro("p_fk_usuario_encargado", recurso.Fk_usuario_encargado.Numero_doc)
            };

            bool respuesta = conexion.EjecutarTransaccion("crear_recurso", parametros);

            if (respuesta)
            {
                var ultimoId = Consultar_recursos2(0);
                int id = ultimoId[ultimoId.Count - 1].Pk_recurso;
                var consulta = @"
                    INSERT DATA {
                        :Recurso" + id + @" a :Recurso ;
                            :idRecurso " + id + @" ;
                            :nombreRecurso '" + recurso.Nombre + @"' ;
                            :direccionRecurso '" + recurso.Direccion + @"' ;
                            :estadoRecurso 'Activo' ;
                            :tieneTpRecurso :TpRec" + recurso.Fk_tp_recurso.Pk_tp_recurso + @" ;
                            :administradoPor :User" + recurso.Fk_usuario_encargado.Numero_doc + @" .
                    }
                ";

                Console.WriteLine(consulta);

                conexionJena.EjecutarUpdate(consulta);
            }

            return respuesta;
        }

        public List<Recurso> Consultar_recursos(int id)
        {
            string finConsulta;
            if (id == 0)
            {
                finConsulta = @"}
                    ORDER BY ASC(?Id)";
            }
            else
            {
                finConsulta = "FILTER (?Id = " + id + ")}";
            }

            var consulta = @"
                SELECT (STR(?Id) AS ?Pk_recurso)
		                (STR(?Nombre) AS ?nombre)
                        (STR(?Estado) AS ?estado)
                        (STR(?Direccion) AS ?direccion)
                        (STR(?NombreRec) AS ?nombre_recurso)
                        (STR(?NombreUsua) AS ?nombre_usuario)
                WHERE {
                  ?recurso a :Recurso;
  	                :idRecurso ?Id;
  	                :nombreRecurso ?Nombre;
   	                :estadoRecurso ?Estado;
                    :direccionRecurso ?Direccion;
                    :tieneTpRecurso ?tprecurso;
                    :administradoPor ?usuario.
  
                  ?tprecurso a :TipoRecurso;
                    :nombreTpRecurso ?NombreRec.
  
                  ?usuario a :Usuario;
                    :nombreUsuario ?NombreUsua.
                " + finConsulta;

            var json = conexionJena.EjecutarConsultaAsync(consulta).Result;
            DataTable data = SparqlJsonParser.ParseToDataTable(json);

            List<Recurso> recursos = new List<Recurso>();
            foreach (DataRow row in data.AsEnumerable())
            {
                recursos.Add(new Recurso()
                {
                    Pk_recurso = Convert.ToInt32(row["PK_recurso"].ToString()),
                    Nombre = row["nombre"].ToString(),
                    Estado = row["estado"].ToString(),
                    Direccion = row["direccion"].ToString(),
                    Fk_tp_recurso = new TipoRecurso()
                    {
                        Nombre = row["nombre_recurso"].ToString()
                    },
                    Fk_usuario_encargado = new Usuario()
                    {
                        Nombre = row["nombre_usuario"].ToString()
                    }
                });
            }
            return recursos;
        }

        public List<Recurso> Consultar_recursos2(int id)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_id_recurso", id)
            };
            DataTable data = conexion.EjecutarConsulta("consultar_recurso", parametros);
            List<Recurso> recursos = new List<Recurso>();
            foreach (DataRow row in data.AsEnumerable())
            {
                recursos.Add(new Recurso()
                {
                    Pk_recurso = Convert.ToInt32(row["PK_recurso"].ToString()),
                    Nombre = row["nombre"].ToString(),
                    Estado = row["estado"].ToString(),
                    Direccion = row["direccion"].ToString(),
                    Fk_tp_recurso = new TipoRecurso()
                    {
                        Nombre = row["nombre_recurso"].ToString()
                    },
                    Fk_usuario_encargado = new Usuario()
                    {
                        Nombre = row["nombre_usuario"].ToString()
                    }
                });
            }
            return recursos;
        }

        public Recurso RecursoUsuario(long usuario)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_id_usuario", usuario)
            };
            DataTable data = conexion.EjecutarConsulta("tiene_recurso", parametros);
            List<Recurso> recursos = new List<Recurso>();
            foreach (DataRow row in data.AsEnumerable())
            {
                recursos.Add(new Recurso()
                {
                    Pk_recurso = Convert.ToInt32(row["PK_recurso"].ToString()),
                });
            }
            if (recursos.Count() > 0)
            {
                return recursos.First();
            }
            else
            {
                return null;
            }

        }
    }
}