using AccesoDatos;
using LogicaNegocio.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio
{
    public class ManejadorInfoEmpresa
    {
        private Connection conexion = new Connection();

        public InfoEmpresa ConsultarInfo()
        {
            DataTable data = conexion.EjecutarConsulta("info_empresa");
            List<InfoEmpresa> info = new List<InfoEmpresa>();
            foreach (DataRow row in data.AsEnumerable())
            {
                info.Add(new InfoEmpresa()
                {
                    Pk_infoEmpresa = Convert.ToInt32(row["id_infoEmpresa"].ToString()),
                    Nombre = row["nombre"].ToString(),
                    Telefono = Convert.ToInt64(row["telefono"].ToString()),
                    Correo = row["correo"].ToString(),
                    Direccion = row["direccion"].ToString(),
                    Descripcion = row["descripcion"].ToString(),
                    Fk_usuario = new Usuario()
                    {
                        Numero_doc = Convert.ToInt64(row["fk_usuario"].ToString())
                    }
                });
            }
            return info.First();
        }

        public bool Actualizar(InfoEmpresa info)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_nombre", info.Nombre),
                new Parametro("p_telefono", info.Telefono),
                new Parametro("p_correo", info.Correo),
                new Parametro("p_direccion", info.Direccion),
                new Parametro("p_descripcion", info.Descripcion),
                new Parametro("p_fk_user", info.Fk_usuario.Numero_doc)
            };
            bool result = conexion.EjecutarTransaccion("actualizar_infoEmpresa", parametros);
            return result;
        }
    }
}
