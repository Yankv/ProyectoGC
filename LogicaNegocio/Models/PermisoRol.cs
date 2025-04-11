using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Models
{
    public class PermisoRol
    {
        [Required] 
        public Permiso Pfk_permiso { get; set; }

        [Required] 
        public Rol Pfk_rol { get; set;}

        [Required] 
        public string? Estado { get;set; }
    }
}
