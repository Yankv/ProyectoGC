using System.ComponentModel.DataAnnotations;

namespace LogicaNegocio.Models
{
    public class EstadoReserva
    {
        public int Pk_estado_reserva { get; set; }

       [Required] public string? Nombre { get; set; }
    }
}
