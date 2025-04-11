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
    public class ManejadorHorario
    {
        private Connection conexion = new Connection();

        public bool Crear_horarios(IEnumerable<Horario> horarios)
        {
            List<Transaccion> transacciones = new List<Transaccion>();
            foreach (Horario horario in horarios)
            {
                transacciones.Add(new Transaccion()
                {
                    Procedimiento = "crear_horario",
                    Parametros = new List<Parametro>()
                    {
                        new Parametro("p_fecha", horario.Fecha),
                        new Parametro("p_hora_inicio", DateTime.Parse(horario.Hora_inicio).ToString("HH:mm:ss")),
                        new Parametro("p_duracion", horario.Duracion),
                        new Parametro("p_costo", horario.Costo),
                        new Parametro("p_fk_dia", horario.Fk_dia.Pk_dia),
                        new Parametro("p_fk_recurso", horario.Fk_recurso.Pk_recurso)
                    }
                });
            }
            return conexion.EjecutarTransacciones(transacciones);
        }
        public List<Horario> Consultar_disponibilidad(DisponibilidadViewModel dipmodel)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_id_recurso", dipmodel.Recurso),
                new Parametro("p_fecha_inicio", dipmodel.FechaInicio),
                new Parametro("p_fecha_fin", dipmodel.FechaFin),
                new Parametro("p_hora_inicio", dipmodel.HoraInicio.ToString("HH:mm:ss")),
                new Parametro("p_hora_fin", dipmodel.HoraFin.ToString("HH:mm:ss"))
            };
            DataTable data = conexion.EjecutarConsulta("consultar_disponibilidad", parametros);
            List<Horario> horarios = new List<Horario>();
            foreach (DataRow row in data.AsEnumerable())
            {
                horarios.Add(new Horario()
                {
                    Pk_horario = Convert.ToInt32(row["PK_horario"].ToString()),
                    Fecha = DateTime.Parse(row["fecha"].ToString()),
                    Hora_inicio = row["hora_inicio"].ToString(),
                    Costo = float.Parse(row["costo"].ToString()),
                    Fk_dia = new Dia()
                    {
                        Pk_dia = Convert.ToInt32(row["fk_dia"].ToString())
                    }
                });
            }
            return horarios;
        }

        public List<Horario> Ver_agenda(long pk_usuario, DateTime? fecha = null) {
            List<Parametro> parametros = new List<Parametro>() {
            new Parametro("pk_usuario",pk_usuario),
            new Parametro("fecha",fecha),
            };
            DataTable data = conexion.EjecutarConsulta("consulta_agenda", parametros);
            List<Horario> agenda = new List<Horario>();
            foreach (DataRow row in data.AsEnumerable())
            {
                agenda.Add(new Horario()
                {
                    Pk_horario = Convert.ToInt32(row["PK_horario"].ToString()),
                    Fk_dia = new Dia()
                    {
                        nombre = row["dia"].ToString()
                    },
                    Fecha = DateTime.Parse(row["fecha"].ToString()),
                    Hora_inicio = row["hora"].ToString(),
                    Duracion = Convert.ToInt32(row["duracion"].ToString()),
                    Costo = float.Parse(row["costo"].ToString()),
                    Estado = row["estado"].ToString()
                });
            }
            return agenda;
        }

        public List<Horario> Consultar_agenda_recursos(int pk_recurso) {
            List<Parametro> parametros = new List<Parametro>() {
            new Parametro("p_id_recurso",pk_recurso)
            };
            List<Horario> agenda = new List<Horario>();
            DataTable data = conexion.EjecutarConsulta("consultar_agenda_recurso", parametros);
            foreach (DataRow row in data.AsEnumerable())
            {
                agenda.Add(new Horario()
                {
                    Fk_recurso = new Recurso() { 
                    Pk_recurso = Convert.ToInt32(row["PK_recurso"].ToString())
                    },
                    Pk_horario = Convert.ToInt32(row["PK_horario"].ToString()),
                    Fk_dia = new Dia()
                    {
                        nombre = row["dia"].ToString()
                    },
                    Fecha = DateTime.Parse(row["fecha"].ToString()),
                    Hora_inicio = row["hora"].ToString(),
                    Duracion = Convert.ToInt32(row["duracion"].ToString()),
                    Costo = float.Parse(row["costo"].ToString()),
                    Estado = row["estado"].ToString()
                });
            }
            return agenda;
        }

        public bool EliminarHorario(int id_horario)
        {
            List<Parametro> parametros = new List<Parametro>()
                {
                    new Parametro("p_horario", id_horario),
                };
            bool resp = conexion.EjecutarTransaccion("eliminar_horario", parametros);
            return resp;
        }

    }
}
