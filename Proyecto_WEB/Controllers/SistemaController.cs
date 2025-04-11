using LogicaNegocio;
using LogicaNegocio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto_WEB.Controllers
{
    [Authorize]
    public class SistemaController : Controller
    {
        ManejadorInfoEmpresa ManejadorInfoEmpresa = new ManejadorInfoEmpresa();


        public IActionResult Empresa()
        {
            InfoEmpresa info = ManejadorInfoEmpresa.ConsultarInfo();
            ViewBag.Info = info;
            return View();
        }

        [HttpPost]
        public IActionResult ActualizarInfo(InfoEmpresa info)
        {
            Usuario usuario = new Usuario();
            if (User.Identity.IsAuthenticated)
            {
                long numeroDocumento = long.Parse(User.FindFirst("Numero_doc")?.Value);
                usuario.Numero_doc = numeroDocumento;
                info.Fk_usuario = usuario;
            }
            ManejadorInfoEmpresa.Actualizar(info);
            return RedirectToAction("Empresa");
        }
    }
}
