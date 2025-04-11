using Microsoft.AspNetCore.Mvc;
using Proyecto_WEB.Models;
using System.Diagnostics;

using Microsoft.AspNetCore.Authorization;
using LogicaNegocio;
using LogicaNegocio.Models;

namespace Proyecto_WEB.Controllers
{

   
    public class HomeController : Controller
    {
        ManejadorInfoEmpresa infoEmpresa = new ManejadorInfoEmpresa();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            InfoEmpresa empresa = infoEmpresa.ConsultarInfo();
            ViewBag.InfoEmpresa = empresa;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Principal()
        {

           InfoEmpresa empresa =  infoEmpresa.ConsultarInfo();
            ViewBag.InfoEmpresa = empresa;
			return View();
        }
    }
}