using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Servicios
{
    public static class MultaServicio
    {
        public static bool TieneMulta(int id_reserva)
        {
            ManejadorMulta manejador = new ManejadorMulta();
            return manejador.TieneMulta(id_reserva);
        }
    }
}
