using MySql.Data.MySqlClient;
using System.Data;

namespace AccesoDatos
{
    public class Connection
    {
        public MySqlConnection connection;

        public bool Conectar()
        {
            string cadenaConnection = "server=localhost; database=reservas; user=root; password=knz231*;port=3306";
            connection = new MySqlConnection(cadenaConnection);
            try
            {
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DesConectar()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public DataTable EjecutarConsulta(string procedimiento, List<Parametro> parametros = null)
        {
            Conectar();
            DataTable datos = new DataTable();
            try
            {
                MySqlCommand comando = new MySqlCommand(procedimiento, connection);
                comando.CommandType = System.Data.CommandType.StoredProcedure;
                if (parametros != null)
                {
                    foreach (Parametro parametro in parametros)
                    {
                        comando.Parameters.AddWithValue(parametro.Nombre, parametro.Valor);
                    }
                }
                MySqlDataReader lector = comando.ExecuteReader();
                datos.Load(lector);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al traer datos de user " + e.Message);
            }
            finally
            {
                DesConectar();
            }
            return datos;
        }

        public bool EjecutarTransaccion(string procedimiento, List<Parametro> parametros = null)
        {
            Conectar();
            try
            {
                MySqlCommand comando = new(procedimiento, connection);
                comando.CommandType = CommandType.StoredProcedure;
                if (parametros != null)
                {
                    foreach (Parametro parametro in parametros)
                    {
                        comando.Parameters.AddWithValue(parametro.Nombre, parametro.Valor);
                    }
                }
                comando.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al insertar datos de user" + e.Message);
                return false;
            }
            finally
            {
                DesConectar();
            }
        }

        public bool EjecutarTransacciones(List<Transaccion> transacciones)
        {
            Conectar();
            MySqlTransaction transaction = null;
            try
            {
                transaction = connection.BeginTransaction();
                MySqlCommand comando = new MySqlCommand();
                if (transacciones != null)
                {
                    foreach (Transaccion transaccion in transacciones)
                    {
                        comando = new MySqlCommand(transaccion.Procedimiento, connection, transaction);
                        comando.CommandType = System.Data.CommandType.StoredProcedure;

                        foreach (Parametro parametro in transaccion.Parametros)
                        {
                            comando.Parameters.AddWithValue(parametro.Nombre, parametro.Valor);
                        }
                        comando.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    comando.Dispose();
                }
                return true;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Console.WriteLine("Error al insertar datos de user " + e.Message);
                return false;
            }
            finally
            {
                DesConectar();
            }
        }
    }
}