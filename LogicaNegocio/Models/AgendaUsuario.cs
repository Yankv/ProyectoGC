using System.ComponentModel.DataAnnotations;

namespace LogicaNegocio.Models
{
    public class AgendaUsuario
    {
        [Required]
        public string? dia { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Fecha { get; set; }

        [DataType(DataType.Time)]
        public DateTime hora { get; set; }

        [Required]
        public string estado { get; set; }
    }
}
