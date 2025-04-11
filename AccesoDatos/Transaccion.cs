using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos
{
    public class Transaccion
    {
        public string Procedimiento { get; set; }
        public List<Parametro> Parametros { get; set; }
    }
}