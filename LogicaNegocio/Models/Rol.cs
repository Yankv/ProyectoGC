using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Models
{
    public class Rol
    {
        public int PK_rol { get; set; }

        [Required] 
        public string Nombre { get; set; }

        [Required]
        public string Estado { get; set; }
    }
}
