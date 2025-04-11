using ClosedXML.Excel;
using LogicaNegocio;
using LogicaNegocio.Models;
using LogicaNegocio.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Proyecto_WEB.Controllers
{

    public class UsuarioController : Controller
    {
        private string StyleBlock = "<style>" +
    ".styled-table { width: 100%; color: black; border-collapse: collapse; margin: 20px 0; font-size: 16px; text-align: left; }" +
    ".styled-table th, .styled-table td { border: 1px solid #dddddd; padding: 8px; text-align: left; }" +
    ".styled-table tr { background-color: #f2f2f2; }" +
    ".styled-table th { background-color: #4CAF50; color: white; }" +
    "</style>";
        ManejadorUsuario manejador = new ManejadorUsuario();
        ManejadorRol roles = new ManejadorRol();

        [Authorize]
        public IActionResult Crear()
        {
            List<Rol> rol = roles.Consultar_roles(0);
            ViewBag.rol = rol;
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Crear(Usuario usuario)
        {
            List<Rol> rol = roles.Consultar_roles(0);
            ViewBag.rol = rol;
            usuario.Contrasenia = usuario.Numero_doc.ToString();
            usuario.Token = UtilidadServicio.GenerarToken();
            usuario.Restablecer = false;
            if (manejador.RegistrarUsuario(usuario))
            {
                return RedirectToAction("Gestionar");
            }
            return View();
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(Usuario usuario, string validar)
        {
            usuario.Token = UtilidadServicio.GenerarToken();
            usuario.Restablecer = false;
            if (usuario.Contrasenia != validar)
            {
                ViewBag.Mensaje = "Las contraseñas no coinciden";
                return View();
            }
            if (manejador.RegistrarUsuario(usuario))
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                ViewBag.Error = "¡Credenciales no validas!\nYa exite un usuario registrado con estas credenciales.";
            }
            return View();
        }

        [Authorize]
        public IActionResult Gestionar(int iduser = 0)
        {
            List<Usuario> usuarios = manejador.ObtenerUsuarios(iduser);
            List<Rol> rols = roles.Consultar_roles(0);
            ViewBag.Roles = rols;
            ViewBag.Usuarios = usuarios;
            return View();
        }

        [HttpGet]
        public IActionResult ObtenerUser(int id)
        {
            Usuario user = manejador.ObtenerUsuarios(id).First();
            return Json(user);
        }

        [HttpPost]
        public IActionResult ActualizarRolEstado(Usuario user)
        {
            bool result = manejador.ActualizarRolEstado(user);
            return RedirectToAction("Gestionar");
        }

        [Authorize]
        public IActionResult Actualizar()
        {
            var modelo = new Usuario();
            if (User.Identity.IsAuthenticated)
            {
                string nombre = User.Identity.Name;
                string apellido = User.FindFirst("Apellido")?.Value;
                string tipoDocumento = User.FindFirst("FK_tp_documento")?.Value;
                string nombreDocumento = User.FindFirst("tp_document")?.Value;
                string numeroDocumento = User.FindFirst("Numero_doc")?.Value;
                string correo = User.FindFirst("Correo")?.Value;
                string telefono = User.FindFirst("Telefono")?.Value;
                string rol = User.FindFirst("Fk_rol")?.Value;
                string nombre_rol = User.FindFirst("rol")?.Value;

                modelo = new Usuario
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    FK_tp_documento = new TipoDocumento
                    {
                        Pk_tipo_doc = Convert.ToInt32(tipoDocumento),
                        Nombre = nombreDocumento.ToString()
                    },
                    FK_rol = new Rol
                    {
                        PK_rol = Convert.ToInt32(rol),
                        Nombre = nombre_rol.ToString()
                    },
                    Numero_doc = long.Parse(numeroDocumento),
                    Correo = correo,
                    Telefono = long.Parse(telefono)
                };
                return View(modelo);

            }
            return View(modelo);
        }

        [HttpPost]
        public IActionResult ActualizarDatos(Usuario usuario)
        {
            if (manejador.ActualizarDatos(usuario))
            {
                //User.Identity.Name = usuario.Nombre;
                return RedirectToAction("Actualizar");
            }
            return View();
        }

        [HttpPost]
        public FileResult ExportarReporteUsuario(string btnExportar)
        {
            switch (btnExportar)
            {
                case "Generar reporte (PDF)":
                    return ExportarUsuario();
                case "Generar reporte (EXCEL)":
                    return ExportarUsuarioExcel();
            }
            return null;
        }

        [HttpPost]
        public FileResult ExportarUsuario()
        {
            string html = ObtenerHtmlUsuarios();
            return File(Reportes.Reporte.Export(html), "application/pdf", "Usuarios.pdf");
        }

        private string ObtenerHtmlUsuarios()
        {
            List<Usuario> usuarios = manejador.ObtenerUsuarios(0);

            string html = "<style>@media print { size: landscape; }</style>";

            html += "<h2 style='color: #333; text-align: center;'>LISTADO DE USUARIOS DEL SISTEMA</h2>";

            html += "<table style='width:100%;' class='styled-table landscape'>";

            html += "<tr class='header-row'><th>N°</th><th>Identificación</th><th>Nombre</th><th>Apellido</th><th>Teléfono</th><th>Correo</th><th>Tipo de Documento</th><th>Rol</th></tr>";

            int cont = 1;
            foreach (Usuario usuario in usuarios)
            {
                string bgColor = cont % 2 == 0 ? "#ffffff" : "#f2f2f2";
                html += $@"<tr style='background-color: {bgColor};'>
                        <td>{cont}</td>
                        <td>{usuario.Numero_doc}</td>
                        <td>{usuario.Nombre}</td>
                        <td>{usuario.Apellido}</td>
                        <td>{usuario.Telefono}</td>
                        <td>{usuario.Correo}</td>
                        <td>{usuario.FK_tp_documento.Nombre}</td>
                        <td>{usuario.FK_rol.Nombre}</td>
                    </tr>";

                cont++;
            }
            html += "</table>";

            return StyleBlock + html;
        }

        public FileResult ExportarUsuarioExcel()
        {
            DataTable dt = new DataTable("Usuarios");
            dt.Columns.AddRange(new DataColumn[8] {
                                        new DataColumn("N°"),
                                        new DataColumn("Identificación"),
                                        new DataColumn("Nombre"),
                                        new DataColumn("Apellido"),
                                        new DataColumn("Teléfono"),
                                        new DataColumn("Correo"),
                                        new DataColumn("Tipo de Documento"),
                                        new DataColumn("Rol")
            });

            List<Usuario> usuario = manejador.ObtenerUsuarios(0);
            int cont = 1;
            foreach (Usuario rp in usuario)
            {
                dt.Rows.Add(cont,
                       rp.Numero_doc,
                        rp.Nombre,
                        rp.Apellido,
                        rp.Telefono,
                        rp.Correo,
                        rp.FK_tp_documento.Nombre,
                        rp.FK_rol.Nombre);
                cont++;
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Usuarios.xlsx");
                }
            }
        }
    }
}

