using System.ComponentModel.DataAnnotations;

namespace LogicaNegocio.Models
{
    public class Reserva
    {
        public int Pk_reserva { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; }

        [Required] 
        public EstadoReserva Fk_estado_reserva { get; set; }

        [Required] 
        public Usuario Fk_usuario { get; set; }
    }
}
