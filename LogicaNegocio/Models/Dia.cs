using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Models
{
    public class Dia
    {
        public int Pk_dia { get; set; }

        [Required] public string nombre { get; set; }
    }
}
