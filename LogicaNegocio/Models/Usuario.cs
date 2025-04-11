using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Models
{
    public class Usuario
    {
        public long Numero_doc { get; set; }

        public long Telefono { get; set; }

        public string? Nombre { get; set; }

        public string? Apellido { get; set; }

        public string? Correo { get; set; }
        
        public string? Contrasenia { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha_registro { get; set; }
        
        public string? Estado { get; set; }
        
        public TipoDocumento FK_tp_documento { get; set; }

        public Rol FK_rol { get; set; }

        public bool Restablecer { get; set; }

        public string? Token { get; set; }


    }
}
