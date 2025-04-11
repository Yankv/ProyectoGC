using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Models
{
    public class Permiso
    {
        public int Pk_permiso { get; set; }

        [Required]
        public string Nombre { get; set; }

        public bool Estado { get; set; }
    }
}
