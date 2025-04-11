using System.ComponentModel.DataAnnotations;

namespace LogicaNegocio.Models
{
    public class Recurso
    {
        public int Pk_recurso { get; set; }

        [Required] public string Nombre { get; set; }

        [Required] public string? Estado { get; set; }

        [Required] public string? Direccion { get; set; }

        [Required] public TipoRecurso Fk_tp_recurso { get; set; }

        [Required] public Usuario? Fk_usuario_encargado { get; set; }
    }
}
