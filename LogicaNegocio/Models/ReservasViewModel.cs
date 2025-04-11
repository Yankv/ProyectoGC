using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Models
{
    public class ReservasViewModel
    {
        public Reserva ReservaView { get; set; }

        public Recurso RecursoView { get; set; }

        public Horario horarioView { get; set; }

        public Usuario usuario { get; set; }
    }
}
