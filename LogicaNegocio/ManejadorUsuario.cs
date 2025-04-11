using AccesoDatos;
using LogicaNegocio.Models;
using System.Data;

namespace LogicaNegocio
{
    public class ManejadorUsuario
    {
        private Connection conexion = new Connection();

        public bool RegistrarUsuario(Usuario usuario)
        {
            int p_rol;
            if (usuario.FK_rol == null)
            {
                p_rol = 0;
            }
            else
            {
                p_rol = usuario.FK_rol.PK_rol;
            }
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("numero_doc", usuario.Numero_doc),
                new Parametro("telefono", usuario.Telefono),
                new Parametro("nombre", usuario.Nombre),
                new Parametro("apellido", usuario.Apellido),
                new Parametro("correo", usuario.Correo),
                new Parametro("contrasenia", usuario.Contrasenia),
                new Parametro("token", usuario.Token),
                new Parametro("Fk_tp_documento", usuario.FK_tp_documento.Pk_tipo_doc),
                new Parametro("Fk_rol", p_rol),
                new Parametro("restablecer", usuario.Restablecer)
            };

            return conexion.EjecutarTransaccion("crear_usuario", parametros);
        }

        public DataTable Login(string correo, string contrasenia)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("u_correo", correo),
                new Parametro("u_contrasenia", contrasenia)
            };
            return conexion.EjecutarConsulta("validar_inicio", parametros);
        }

        public bool RestablecerActualizar(int restablecer, string contrasenia, string token)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("restablecer", restablecer),
                new Parametro("u_contrasenia", contrasenia),
                new Parametro("token", token),
            };
            return conexion.EjecutarTransaccion("restablecer_contrasenia", parametros);
        }

        public Usuario Obtener(string correo)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("email", correo)
            };
            DataTable data = conexion.EjecutarConsulta("obtenerUsuario", parametros);
            Usuario usuario = new Usuario();
            foreach (DataRow row in data.AsEnumerable())
            {
                usuario.Nombre = row["nombre"].ToString();
                usuario.Restablecer = Convert.ToBoolean(row["Restablecer"]);
                usuario.Contrasenia = row["contrasenia"].ToString();
                usuario.Token = row["token"].ToString();
            }
            return usuario;
        }

        public List<Usuario> ObtenerUsuarios(long id)
        {
            List<Parametro> parametros = new List<Parametro>()
                {
                    new Parametro("p_id_usuario", id)
                };
            DataTable data = conexion.EjecutarConsulta("consultar_usuario", parametros);
            List<Usuario> usuarios = new List<Usuario>();
            foreach (DataRow row in data.AsEnumerable())
            {
                usuarios.Add(new Usuario()
                {
                    Numero_doc = Convert.ToInt64(row["numero_doc"].ToString()),
                    Telefono = Convert.ToInt64(row["telefono"].ToString()),
                    Nombre = row["nombre"].ToString(),
                    Apellido = row["apellido"].ToString(),
                    Correo = row["correo"].ToString(),
                    Estado = row["estado"].ToString(),
                    FK_tp_documento = new TipoDocumento()
                    {
                        Nombre = row["nombre_tp_doc"].ToString()
                    },
                    FK_rol = new Rol()
                    {
                        PK_rol = Convert.ToInt32(row["Fk_rol"].ToString()),
                        Nombre = row["nombre_rol"].ToString()
                    }
                });
            }
            return usuarios;
        }

        public bool ActualizarRolEstado(Usuario user)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("p_usuario", user.Numero_doc),
                new Parametro("p_estado", user.Estado),
                new Parametro("p_rol", user.FK_rol.PK_rol)
            };
            return conexion.EjecutarTransaccion("actualizar_RE", parametros);
        }

        public bool ActualizarDatos(Usuario user)
        {
            List<Parametro> parametros = new List<Parametro>()
            {
                new Parametro("u_id", user.Numero_doc),
                new Parametro("u_nombre", user.Nombre),
                new Parametro("u_apellido", user.Apellido),
                new Parametro("u_correo", user.Correo),
                new Parametro("u_telefono", user.Telefono),

            };
            return conexion.EjecutarTransaccion("actualizar_datos", parametros);
        }

    }
}