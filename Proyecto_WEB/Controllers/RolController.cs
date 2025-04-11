using LogicaNegocio;
using LogicaNegocio.Models;
using LogicaNegocio.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Relational;

namespace Proyecto_WEB.Controllers
{
    [Authorize]
    public class RolController : Controller
    {
        ManejadorRol manejadorRol = new ManejadorRol();
        ManejadorUsuario manejadorU = new ManejadorUsuario();
        ManejadorPermisos manejadorP = new ManejadorPermisos();

        public IActionResult Rol([FromQuery] int idRol = 0)
        {
            ViewBag.idRol = idRol;
            List<Permiso> permisos = manejadorP.ObtenerPermisos(idRol);
            List<Rol> roles = manejadorRol.Consultar_roles(0);
            ViewBag.permisos = permisos;
            ViewBag.Roles = roles;
            ViewBag.rol = idRol;
            return View();
        }

        [HttpPost]
        public IActionResult Crear(string nombre)
        {
            bool result = manejadorRol.CrearRol(nombre);
            return RedirectToAction("Rol");
        }

		public IActionResult AsignarPermisoRol([FromQuery] int idRol, [FromQuery] int idPermiso)
		{
			bool result = manejadorP.AsignarPermiso(idRol, idPermiso);
			return RedirectToAction("Rol", new { idRol = idRol });
		}
	}
}
