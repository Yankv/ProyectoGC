using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Models
{
    public class TipoDocumento
    {
        public int Pk_tipo_doc { get; set; }

        [Required] 
        public string Nombre { get; set; }

        [Required] 
        public string Estado { get; set; }
    }
}
