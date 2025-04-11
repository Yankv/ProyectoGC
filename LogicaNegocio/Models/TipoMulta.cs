using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Models
{
    public class TipoMulta
    {
        public int Pk_tipo_multa { get; set; }

        [Required]
        public string Nombre { get; set; }

        public float Valor { get; set; }

        public int Dias { get; set; }

        [Required]
        public string? Estado { get; set; }
    }
}
