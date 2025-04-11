using AccesoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio
{
    internal class Main
    {
        public bool ConectarBD()
        {
            Connection connection = new Connection();
            return connection.Conectar();
        }
    }
}
