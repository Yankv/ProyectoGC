using LogicaNegocio;
using LogicaNegocio.Models;
using LogicaNegocio.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto_WEB.Utilidad;
using System.Data;
using System.Security.Claims;

namespace Proyecto_WEB.Controllers
{
    public class LoginController : Controller
    {
        public static int rol = 0;

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            ManejadorUsuario manejador = new ManejadorUsuario();
            DataTable data = manejador.Login(email, password);
            if (data.Rows.Count > 0)
            {
                Usuario usuario = new Usuario()
                {
                    Numero_doc = Convert.ToInt64(data.Rows[0]["numero_doc"].ToString()),
                    Telefono = Convert.ToInt64(data.Rows[0]["telefono"].ToString()),
                    Nombre = data.Rows[0]["nombre"].ToString(),
                    Apellido = data.Rows[0]["apellido"].ToString(),
                    Correo = data.Rows[0]["correo"].ToString(),
                    Contrasenia = data.Rows[0]["contrasenia"].ToString(),
                    Estado = data.Rows[0]["estado"].ToString(),
                    FK_rol = new Rol
                    {
                        PK_rol = Convert.ToInt32(data.Rows[0]["Fk_rol"]),
                        Nombre = data.Rows[0]["rol"].ToString()
                    },
                    FK_tp_documento = new TipoDocumento()
                    {
                        Pk_tipo_doc = Convert.ToInt32(data.Rows[0]["Fk_tp_documento"]),
                        Nombre = data.Rows[0]["tp_document"].ToString()
                    }
                };
                rol = usuario.FK_rol.PK_rol;
                GuardarPermisosRol(rol);


                #region AUTENTICACTION
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, InfoUsuario.GuardarInfo(usuario));
                #endregion

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View();
            }
        }

        public IActionResult Restablecer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Restablecer(string correo, [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            ManejadorUsuario manejador = new ManejadorUsuario();

            Usuario usuario = manejador.Obtener(correo);
            ViewBag.Correo = correo;
            if (usuario != null)
            {
                bool respuesta = manejador.RestablecerActualizar(1, usuario.Contrasenia, usuario.Token);
                if (respuesta)
                {
                    string path = Path.Combine(webHostEnvironment.ContentRootPath, "Plantilla", "Restablecer.html");
                    string content = System.IO.File.ReadAllText(path);
                    string url = $"{Request.Scheme}://{Request.Host}/Login/Actualizar?token={usuario.Token}";

                    string htmlBody = string.Format(content, usuario.Nombre, url);

                    CorreoDTO correoDTO = new CorreoDTO()
                    {
                        Para = correo,
                        Asunto = "Restablecer cuenta",
                        Contenido = htmlBody
                    };

                    bool enviado = CorreoServicio.Enviar(correoDTO);
                    ViewBag.Restablecido = true;
                }
                else
                {
                    ViewBag.Mensaje = "No se pudo restablecer la cuenta";
                }

            }
            else
            {
                ViewBag.Mensaje = "No se encontraron coincidencias con el correo";
            }

            return View();
        }

        public ActionResult Actualizar(string token)
        {
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public ActionResult Actualizar(string token, string clave, string confirmarClave)
        {

            ManejadorUsuario manejador = new ManejadorUsuario();
            ViewBag.Token = token;
            if (clave != confirmarClave)
            {
                ViewBag.Mensaje = "Las contraseñas no coinciden";
                return View();
            }

            bool respuesta = manejador.RestablecerActualizar(0, clave, token);

            if (respuesta)
                ViewBag.Restablecido = true;
            else
                ViewBag.Mensaje = "No se pudo actualizar";

            return View();
        }

        public async Task<IActionResult> Salir()
        {
            //3.- CONFIGURACION DE LA AUTENTICACION
            #region AUTENTICACTION
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            #endregion

            return RedirectToAction("Principal", "Home");

        }

        private void GuardarPermisosRol(int idRol)
        {
            ManejadorPermisos manejadorP = new ManejadorPermisos();
            HttpContext.Session.SetString("idRol", rol.ToString());
            Dictionary<string, bool> permisos = manejadorP.ObtenerPermisos(rol).ToDictionary(c => c.Nombre, c => c.Estado);
            HttpContext.Session.SetString("permisos", JsonConvert.SerializeObject(permisos));
            var d = JsonConvert.DeserializeObject<Dictionary<string, bool>>(HttpContext.Session.GetString("permisos"));
        }
    }
}


