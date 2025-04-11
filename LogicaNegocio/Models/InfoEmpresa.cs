using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Models
{
    public class InfoEmpresa
    {
        public int Pk_infoEmpresa { get; set; }

        public string Nombre { get; set; }

        public long Telefono { get; set; }

        public string Correo { get; set; }

        public string Direccion { get; set; }

        public string Descripcion { get; set; }

        public Usuario Fk_usuario { get; set; }
    }
}
