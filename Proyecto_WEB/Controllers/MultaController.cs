using LogicaNegocio;
using LogicaNegocio.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_WEB.Utilidad;

namespace Proyecto_WEB.Controllers
{
    [Authorize]
    public class MultaController : Controller
    {
        ManejadorMulta ManejadorMulta = new ManejadorMulta();
        ManejadorTpMulta ManejadorTpMulta = new ManejadorTpMulta();

        public IActionResult Gestionar()
        {
            List<Multa> multas = ManejadorMulta.Colsultar_multa(0);
            ViewBag.Multas = multas;
            List<TipoMulta> tipoMultas = ManejadorTpMulta.Consultar_tpmulta(0);
            ViewBag.TpMultas = tipoMultas;
            return View();
        }

        public IActionResult Historial()
        {
            var usuario = new Usuario();
            if (User.Identity.IsAuthenticated)
            {
                long numeroDocumento = long.Parse(User.FindFirst("Numero_doc")?.Value);
                string nombre = User.Identity.Name;
                string apellido = User.FindFirst("Apellido")?.Value;
                usuario.Numero_doc = numeroDocumento;
                usuario.Nombre = nombre;
                usuario.Apellido = apellido;
            }
            ViewBag.Usuario = usuario;
            List<Multa> multas = ManejadorMulta.MultasUsuario(usuario.Numero_doc);
            ViewBag.Multas = multas;
            return View();
        }

        [HttpPost]
        public IActionResult Crear_tp(TipoMulta tipoMulta)
        {
            ManejadorTpMulta.Crear_tp(tipoMulta);
            return RedirectToAction("Gestionar");
        }

        [HttpGet]
        public IActionResult ObtenerMulta(int id)
        {
            Multa multa = ManejadorMulta.Colsultar_multa(id).First();
            return Json(multa);
        }

        [HttpPost]
        public IActionResult ActualizarMulta(Multa multa)
        {
            ManejadorMulta.ActualizarMulta(multa);
            return RedirectToAction("Gestionar");
        }

        [Route("Multa/EliminarMulta/{idMulta}")]
        [HttpPost]
        public async Task<IActionResult> EliminarMulta(int idMulta, int pagar)
        {
            ManejadorMulta.EliminarMulta(idMulta, pagar);
            if(pagar == 0)
            {
                return RedirectToAction("Gestionar");
            }
            else
            {
                long id_u = long.Parse(User.FindFirst("Numero_doc")?.Value);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, InfoUsuario.ActualizarInfo(id_u));
                return RedirectToAction("Historial");
            }
        }
    }
}
