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
    public class ManejadorMulta
    {
        private Connection conexion = new Connection();

        public bool Crear_multa(Multa multa)
        {
            Console.WriteLine(multa);
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_descripcion", multa.Descripcion),
                new Parametro("p_pfk_reserva", multa.Pfk_reserva.Pk_reserva),
                new Parametro("p_fk_tp_multa", multa.Fk_tipo_multa.Pk_tipo_multa)
            };
            return conexion.EjecutarTransaccion("crear_multa", parametros);
        }

        public List<Multa> Colsultar_multa(int id)
        {
            List<Parametro> parametros = new List<Parametro>()
                {
                    new Parametro("p_id_multa", id)
                };
            DataTable data = conexion.EjecutarConsulta("consultar_multa", parametros);
            List<Multa> multas = new List<Multa>();
            foreach (DataRow row in data.AsEnumerable())
            {
                DateTime fFin;
                string fechaFin = "";
                if (row["fecha_fin"].ToString() != "")
                {
                    fFin = DateTime.Parse(row["fecha_fin"].ToString());
                    fechaFin = fFin.ToShortDateString();
                }
                multas.Add(new Multa()
                {
                    Pk_multa = Convert.ToInt32(row["Pk_multa"].ToString()),
                    FechaMulta = DateTime.Parse(row["fecha_multa"].ToString()),
                    FechaFin = fechaFin,
                    Costo = float.Parse(row["costo"].ToString()),
                    Descripcion = row["descripcion"].ToString(),
                    Estado = row["estado"].ToString(),
                    Pfk_reserva = new Reserva()
                    {
                        Pk_reserva = Convert.ToInt32(row["PFK_reserva"].ToString())
                    },
                    Fk_tipo_multa = new TipoMulta()
                    {
                        Pk_tipo_multa = Convert.ToInt32(row["fk_tp_Multa"].ToString())
                    }
                });
            }
            return multas;
        }

        public bool ActualizarMulta(Multa multa)
        {
            List<Parametro> parametros = new List<Parametro>()
                {
                    new Parametro("p_id_multa", multa.Pk_multa),
                    new Parametro("p_fk_tipo", multa.Fk_tipo_multa.Pk_tipo_multa),
                    new Parametro("p_descripcion", multa.Descripcion)
                };
            bool resp = conexion.EjecutarTransaccion("actualizar_multa", parametros);
            return resp;
        }

        public bool EliminarMulta(int idMulta, int pagar)
        {
            List<Parametro> parametros = new List<Parametro>()
                {
                    new Parametro("p_id_multa", idMulta),
                    new Parametro("pagar", pagar)
                };
            bool resp = conexion.EjecutarTransaccion("eliminar_multa", parametros);
            return resp;
        }

        public List<Multa> MultasUsuario(long is_Usuario)
        {
            List<Parametro> parametros = new List<Parametro>()
                {
                    new Parametro("p_id_usuario", is_Usuario)
                };
            DataTable data = conexion.EjecutarConsulta("mis_multas", parametros);
            List<Multa> multas = new List<Multa>();
            foreach (DataRow row in data.AsEnumerable())
            {
                DateTime fFin;
                string fechaFin = "";
                if (row["fecha_fin"].ToString() != "")
                {
                    fFin = DateTime.Parse(row["fecha_fin"].ToString());
                    fechaFin = fFin.ToShortDateString();
                }
                multas.Add(new Multa()
                {
                    Pk_multa = Convert.ToInt32(row["Pk_multa"].ToString()),
                    FechaMulta = DateTime.Parse(row["fecha_multa"].ToString()),
                    FechaFin = fechaFin,
                    Costo = float.Parse(row["costo"].ToString()),
                    Descripcion = row["descripcion"].ToString(),
                    Estado = row["estado"].ToString(),
                    Pfk_reserva = new Reserva()
                    {
                        Pk_reserva = Convert.ToInt32(row["PFK_reserva"].ToString())
                    },
                    Fk_tipo_multa = new TipoMulta()
                    {
                        Pk_tipo_multa = Convert.ToInt32(row["fk_tp_Multa"].ToString())
                    }
                });
            }
            return multas;
        }

        public bool TieneMulta(int id_recurso)
        {
            List<Parametro> parametros = new List<Parametro>()
                {
                    new Parametro("p_reserva", id_recurso)
                };
            DataTable data = conexion.EjecutarConsulta("tiene_multa", parametros);
            if(data.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
