using System.ComponentModel.DataAnnotations;

namespace LogicaNegocio.Models
{
    public class Horario
    {
        public int Pk_horario { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public string Hora_inicio { get; set; }

        [Required]
        public float Costo { get; set; }

        [Required]
        public int Duracion { get; set; }

        public string Estado { get; set; }

        [Required]
        public Dia Fk_dia { get; set; }

        [Required]
        public Recurso Fk_recurso { get; set; }

        public Reserva? Fk_reserva { get; set; }

    }
}
