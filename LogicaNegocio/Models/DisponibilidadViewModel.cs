using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Models
{
    public class DisponibilidadViewModel
    {
        public int Recurso { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [DataType(DataType.Time)]
        public DateTime HoraInicio { get; set; }

        [DataType(DataType.Time)]
        public DateTime HoraFin { get; set; }
    }
}
