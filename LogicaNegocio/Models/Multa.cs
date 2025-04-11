using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Models
{
    public class Multa
    {
        public int Pk_multa { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaMulta { get; set; }

        [DataType(DataType.Date)]
        public string FechaFin { get; set; }

        public float Costo { get; set; }

        public string Descripcion { get; set; }

        public string Estado { get; set; }

        [Required] 
        public Reserva Pfk_reserva { get; set; }

        [Required] 
        public TipoMulta Fk_tipo_multa { get; set; }

    }
}
