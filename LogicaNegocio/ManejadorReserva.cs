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
    public class ManejadorReserva
    {
        private Connection conexion = new Connection();

        public bool Crear_reserva(long usuario, int horario)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_fk_usuario", usuario),
                new Parametro("p_fk_horario", horario)
            };
            return conexion.EjecutarTransaccion("crear_reserva", parametros);
        }

        public List<ReservasViewModel> ConsultarReservas(long IdUsuario)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_id_usuario", IdUsuario)
            };
            DataTable data = conexion.EjecutarConsulta("mis_reservas", parametros);
            List<ReservasViewModel> reservas = new List<ReservasViewModel>();
            foreach (DataRow row in data.AsEnumerable())
            {
                reservas.Add(new ReservasViewModel()
                {
                    ReservaView = new Reserva()
                    {
                        Pk_reserva = Convert.ToInt32(row["ID"].ToString()),
                        FechaRegistro = DateTime.Parse(row["fecha_registro"].ToString()),
                        Fk_estado_reserva = new EstadoReserva()
                        {
                            Pk_estado_reserva = Convert.ToInt32(row["pk_estado"].ToString()),
                            Nombre = row["estado"].ToString()
                        }
                    },
                    RecursoView = new Recurso()
                    {
                        Pk_recurso = Convert.ToInt32(row["fk_recurso"].ToString()),
                        Nombre = row["recurso"].ToString()
                    },
                    horarioView = new Horario()
                    {
                        Pk_horario = Convert.ToInt32(row["PK_horario"].ToString()),
                        Fecha = DateTime.Parse(row["fecha"].ToString()),
                        Hora_inicio = row["hora_inicio"].ToString(),
                        Duracion = Convert.ToInt32(row["duracion"].ToString()),
                        Costo = float.Parse(row["costo"].ToString()),
                    }
                });
            }
            return reservas;
        }

        public List<ReservasViewModel> ConsultarReserva(int reserva)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_id_reserva", reserva)
            };
            DataTable data = conexion.EjecutarConsulta("consultar_reserva", parametros);
            List<ReservasViewModel> reservas = new List<ReservasViewModel>();
            foreach (DataRow row in data.AsEnumerable())
            {
                reservas.Add(new ReservasViewModel()
                {
                    ReservaView = new Reserva()
                    {
                        Pk_reserva = Convert.ToInt32(row["ID"].ToString()),
                        FechaRegistro = DateTime.Parse(row["fecha_registro"].ToString()),
                        Fk_estado_reserva = new EstadoReserva()
                        {
                            Pk_estado_reserva = Convert.ToInt32(row["fk_estado_reserva"].ToString())
                        }
                    },
                    horarioView = new Horario()
                    {
                        //Pk_horario = Convert.ToInt32(row["PK_horario"].ToString()),
                        //Fecha = DateTime.Parse(row["fecha"].ToString()),
                        //Hora_inicio = row["hora_inicio"].ToString(),
                        //Duracion = Convert.ToInt32(row["duracion"].ToString()),
                        Costo = float.Parse(row["costo"].ToString()),
                    },
                    usuario = new Usuario()
                    {
                        Numero_doc = Convert.ToInt32(row["numero_doc"].ToString()),
                        Nombre = row["nombre"].ToString(),
                        Apellido = row["apellido"].ToString(),
                        Telefono = Convert.ToInt64(row["telefono"].ToString()),
                        Correo = row["correo"].ToString()
                    }
                });
            }
            return reservas;
        }

        public List<ReservasViewModel> ConsultarReservasRecurso(int Recurso)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_id_recurso", Recurso)
            };
            DataTable data = conexion.EjecutarConsulta("consultar_reser_recurso", parametros);
            List<ReservasViewModel> reservas = new List<ReservasViewModel>();
            foreach (DataRow row in data.AsEnumerable())
            {
                reservas.Add(new ReservasViewModel()
                {
                    ReservaView = new Reserva()
                    {
                        Pk_reserva = Convert.ToInt32(row["ID"].ToString()),
                        FechaRegistro = DateTime.Parse(row["fecha_registro"].ToString()),
                        Fk_estado_reserva = new EstadoReserva()
                        {
                            Nombre = row["estado"].ToString()
                        }
                    },
                    RecursoView = new Recurso()
                    {
                        Pk_recurso = Convert.ToInt32(row["PK_recurso"].ToString()),
                        Nombre = row["recurso"].ToString()
                    },
                    horarioView = new Horario()
                    {
                        Pk_horario = Convert.ToInt32(row["PK_horario"].ToString()),
                        Fecha = DateTime.Parse(row["fecha"].ToString()),
                        Hora_inicio = row["hora_inicio"].ToString(),
                        Duracion = Convert.ToInt32(row["duracion"].ToString()),
                        Costo = float.Parse(row["costo"].ToString()),
                    },
                    usuario = new Usuario()
                    {
                        Numero_doc = Convert.ToInt32(row["numero_doc"].ToString()),
                        Nombre = row["nombre"].ToString(),
                        Apellido = row["apellido"].ToString()
                    }
                });
            }
            return reservas;
        }

        public bool CancelarReserva(int reserva)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_id_reserva", reserva)
            };
            return conexion.EjecutarTransaccion("cancelar_reserva", parametros);
        }

        public bool ActualizarReserva(int reserva, int estado)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_reserva", reserva),
                new Parametro("p_estado", estado),
            };
            return conexion.EjecutarTransaccion("actualizar_reserva", parametros);
        }

    }
}
