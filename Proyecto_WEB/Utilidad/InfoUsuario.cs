using LogicaNegocio;
using LogicaNegocio.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Org.BouncyCastle.Security;
using System.Security.Claims;

namespace Proyecto_WEB.Utilidad
{
    static class InfoUsuario
    {
        public static ClaimsPrincipal GuardarInfo(Usuario usuario = null)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim("Numero_doc", usuario.Numero_doc.ToString()),
                    new Claim("Telefono", usuario.Telefono.ToString()),
                    new Claim("Apellido", usuario.Apellido),
                    new Claim("Correo", usuario.Correo),
                    new Claim("Contrasenia", usuario.Contrasenia),
                    new Claim("Estado", usuario.Estado),
                    new Claim("Fk_rol", usuario.FK_rol.PK_rol.ToString()),
                    new Claim("rol", usuario.FK_rol.Nombre.ToString()),
                    new Claim("FK_tp_documento", usuario.FK_tp_documento.Pk_tipo_doc.ToString()),
                    new Claim("tp_document", usuario.FK_tp_documento.Nombre.ToString())
                };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }

        public static ClaimsPrincipal ActualizarInfo(long id_u)
        {
            ManejadorUsuario ManejadorUsuario = new ManejadorUsuario();
            Usuario usuario = ManejadorUsuario.ObtenerUsuarios(id_u).First();
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim("Numero_doc", usuario.Numero_doc.ToString()),
                    new Claim("Telefono", usuario.Telefono.ToString()),
                    new Claim("Apellido", usuario.Apellido),
                    new Claim("Correo", usuario.Correo),
                    new Claim("Estado", usuario.Estado),
                    new Claim("Fk_rol", usuario.FK_rol.PK_rol.ToString()),
                    new Claim("rol", usuario.FK_rol.Nombre.ToString()),
                    new Claim("FK_tp_documento", usuario.FK_tp_documento.Pk_tipo_doc.ToString())
                };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }
    }
}
