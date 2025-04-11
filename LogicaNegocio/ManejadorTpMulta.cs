using AccesoDatos;
using iTextSharp.text;
using LogicaNegocio.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio
{
    public class ManejadorTpMulta
    {
        private Connection conexion = new Connection();

        public bool Crear_tp(TipoMulta tpmulta)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_nombre", tpmulta.Nombre),
                new Parametro("p_valor", tpmulta.Valor),
                new Parametro("p_dias", tpmulta.Dias)
            };
            return conexion.EjecutarTransaccion("crear_tp_multa", parametros);
        }

        public List<TipoMulta> Consultar_tpmulta(int id)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_id", id)
            };
            DataTable data = conexion.EjecutarConsulta("consultar_tpmulta", parametros);
            List<TipoMulta> tps_multa = new List<TipoMulta>();
            foreach(DataRow row in data.AsEnumerable())
            {
                tps_multa.Add(new TipoMulta()
                {
                    Pk_tipo_multa = Convert.ToInt32(row["PK_tipo_multa"].ToString()),
                    Nombre = row["nombre"].ToString(),
                    Valor = float.Parse(row["valor"].ToString()),
                    Dias = Convert.ToInt32(row["dias"].ToString()),
                    Estado = row["estado"].ToString()
                });
            }
            return tps_multa;
        }
    }
}
