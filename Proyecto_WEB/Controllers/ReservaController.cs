using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using iTextSharp.tool.xml.html;
using LogicaNegocio;
using LogicaNegocio.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Proyecto_WEB.Utilidad;

namespace Proyecto_WEB.Controllers
{

    [Authorize]
    //[Route("reserva/")]
    public class ReservaController : Controller
    {
        ManejadorRecurso manejadorRecurso = new ManejadorRecurso();
        ManejadorHorario ManejadorHorario = new ManejadorHorario();
        ManejadorReserva ManejadorReserva = new ManejadorReserva();
        ManejadorTpMulta ManejadorTpMulta = new ManejadorTpMulta();
        ManejadorMulta ManejadorMulta = new ManejadorMulta();

        private string StyleBlock = "<style>" +
         ".styled-table { width: 100%; color: black; border-collapse: collapse; margin: 20px 0; font-size: 16px; text-align: left; }" +
         ".styled-table th, .styled-table td { border: 1px solid #dddddd; padding: 8px; text-align: left; }" +
         ".styled-table tr { background-color: #f2f2f2; }" +
         ".styled-table th { background-color: #4CAF50; color: white; }" +
         "</style>";

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
            List<ReservasViewModel> reservas = ManejadorReserva.ConsultarReservas(usuario.Numero_doc);
            ViewBag.User = usuario;
            ViewBag.Reservas = reservas;
            return View();
        }

        public IActionResult Reservar()
        {
            List<Recurso> recursos = manejadorRecurso.Consultar_recursos(0);
            ViewBag.Recursos = recursos;
            string estado = User.FindFirst("Estado")?.Value;
            if(estado != "Activo")
            {
                ViewBag.Error = "Usted se encuentra suspendido, no puede realizar reservas hasta\nFinalizar la suspensión.";
            }
            return View();
        }

        [HttpPost]
        public IActionResult Reservar(DisponibilidadViewModel dipmodel)
        {
            List<Recurso> recursos = manejadorRecurso.Consultar_recursos(0);
            ViewBag.Recursos = recursos;
            List<Horario> horarios = ManejadorHorario.Consultar_disponibilidad(dipmodel);
            if (horarios.Count > 0)
            {
                ViewBag.Horarios = horarios;
            }
            ViewBag.Filtro = dipmodel;
            return View();
        }

        [Route("reserva/Crear_reserva/{horario}")]
        [HttpPost]
        public IActionResult Crear_reserva(int horario)
        {
            var usuario = new Usuario();
            if (User.Identity.IsAuthenticated)
            {
                long numeroDocumento = long.Parse(User.FindFirst("Numero_doc")?.Value);
                usuario.Numero_doc = numeroDocumento;
            }
            ManejadorReserva.Crear_reserva(usuario.Numero_doc, horario);
            return RedirectToAction("Reservar");
        }

        [Route("reserva/CancelarReserva/{reserva}")]
        [HttpPost]
        public IActionResult CancelarReserva(int reserva)
        {
            ManejadorReserva.CancelarReserva(reserva);
            return RedirectToAction("Historial");
        }

        public IActionResult Reservas_recurso([FromQuery] int recurso = -1)
        {
            ViewBag.recurso = recurso;
            var usuario = new Usuario();
            if (User.Identity.IsAuthenticated)
            {
                long numeroDocumento = long.Parse(User.FindFirst("Numero_doc")?.Value);
                usuario.Numero_doc = numeroDocumento;
            }
            List<Recurso> recursos = manejadorRecurso.Consultar_recursos(0);
            List<TipoMulta> tpmulta = ManejadorTpMulta.Consultar_tpmulta(0);
            Recurso recurso_u = manejadorRecurso.RecursoUsuario(usuario.Numero_doc);
            List<ReservasViewModel> reservas = null;
            if (recurso == -1)
            {
                if (recurso_u != null)
                {
                    reservas = ManejadorReserva.ConsultarReservasRecurso(recurso_u.Pk_recurso);
                }
                else
                {
                    ViewBag.Error = "Usted no tiene recurso asignado.";
                }
            }
            else
            {
                reservas = ManejadorReserva.ConsultarReservasRecurso(recurso);
            }
            ViewBag.idrec = recurso;
            ViewBag.Tpmulta = tpmulta;
            if (reservas != null && reservas.Count() < 1)
            {
                ViewBag.Error = "No hay reservas en el sistema para este recurso.";
            }
            ViewBag.Recursos = recursos;
            ViewBag.Reservas = reservas;
            return View();
        }

        [HttpPost]
        public IActionResult ActualizarReserva(int reserva, int estado)
        {
            bool result = ManejadorReserva.ActualizarReserva(reserva, estado);
            //if (result)
            //{
            return RedirectToAction("Reservas_recurso");
            //}
        }

        [HttpGet]
        public IActionResult ObtenerReserva(int id)
        {
            ReservasViewModel reserva = ManejadorReserva.ConsultarReserva(id).First();
            return Json(reserva);
        }

        [HttpPost]
        public FileResult ExportarReporteMisReservas(string btnExportar)
        {
            switch (btnExportar)
            {
                case "Generar reporte (PDF)":
                    return ExportarMisReservas();
                case "Generar reporte (EXCEL)":
                    return ExportarMisReservasExcel();
            }
            return null;
        }
        public FileResult ExportarMisReservas()
        {
            string html = ObtenerHtmlMisReservas();
            return File(Reportes.Reporte.Export(html), "application/pdf", "MisReservas.pdf");
        }

        private string ObtenerHtmlMisReservas()
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
            List<ReservasViewModel> reservas = ManejadorReserva.ConsultarReservas(usuario.Numero_doc);

            string html = "<style>@media print { size: landscape; }</style>";

            html += "<h2 style='color: #333; text-align: center;margin-top: 20px;'>LISTADO DE RESERVAS </h2> ";
            html += $"<div style='text-align: center;'><p>Usuario: {usuario.Nombre} {usuario.Apellido}  -  Número de documento: {usuario.Numero_doc}</p></div>";
            html += "<br>";
            html += "<table style='width:100%;' class='styled-table landscape'>";
            html += "<tr class='header-row'><th>N°</th><th>Recurso</th><th>Fecha</th><th>Hora</th><th>Costo</th><th>Estado</th></tr>";

            int cont = 1;
            foreach (ReservasViewModel reserva in reservas)
            {
                string bgColor = cont % 2 == 0 ? "#ffffff" : "#f2f2f2";
                html += $@"<tr style='background-color: {bgColor};'>
                        <td>{cont}</td>
                        <td>{reserva.RecursoView.Nombre}</td>
                        <td>{reserva.horarioView.Fecha.ToShortDateString()}</td>
                        <td>{reserva.horarioView.Hora_inicio}</td>
                        <td>{reserva.horarioView.Costo}</td>
                        <td>{reserva.ReservaView.Fk_estado_reserva.Nombre}</td>
                    </tr>";
                cont++;
            }
            html += "</table>";

            return StyleBlock + html;
        }
        public FileResult ExportarMisReservasExcel()
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
            DataTable dt = new DataTable("Mis reservas");
            dt.Columns.AddRange(new DataColumn[6] {
                                        new DataColumn("N°"),
                                        new DataColumn("Recurso"),
                                        new DataColumn("Fecha"),
                                        new DataColumn("Costo"),
                                        new DataColumn("Hora"),
                                        new DataColumn("Estado"),
            });

            List<ReservasViewModel> reservas = ManejadorReserva.ConsultarReservas(usuario.Numero_doc);

            int cont = 1;
            foreach (ReservasViewModel rp in reservas)
            {
                dt.Rows.Add(cont,
                        rp.RecursoView.Nombre,
                        rp.horarioView.Fecha.ToShortDateString(),
                        rp.horarioView.Costo,
                        rp.horarioView.Hora_inicio,
                        rp.ReservaView.Fk_estado_reserva.Nombre);
                cont++;
                Console.WriteLine(rp.RecursoView.Nombre);
                Console.WriteLine(rp.horarioView.Fecha.ToShortDateString());
                Console.WriteLine(rp.horarioView.Costo);
                Console.WriteLine(rp.horarioView.Hora_inicio);
                Console.WriteLine(rp.ReservaView.Fk_estado_reserva.Nombre);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Mis reservas.xlsx");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear_multa(Multa multa_m)
        {
            bool result = ManejadorMulta.Crear_multa(multa_m);
            long id_u = long.Parse(User.FindFirst("Numero_doc")?.Value);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, InfoUsuario.ActualizarInfo(id_u));
            return RedirectToAction("Reservas_recurso");
        }

        [HttpPost]
        public FileResult ExportarReservasRecurso(int idRecurso)
        {
            string html = ObtenerHtmlReservasRecurso(idRecurso);
            return File(Reportes.Reporte.Export(html), "application/pdf", "Reserva de recursos.pdf");
        }

        private string ObtenerHtmlReservasRecurso(int recurso)
        {
            var usuario = new Usuario();
            if (User.Identity.IsAuthenticated)
            {
                long numeroDocumento = long.Parse(User.FindFirst("Numero_doc")?.Value);
                usuario.Numero_doc = numeroDocumento;
            }
            Recurso recurso_u = manejadorRecurso.RecursoUsuario(usuario.Numero_doc);
            List<ReservasViewModel> reservas = null;
            if (recurso == -1)
            {
                if (recurso_u != null)
                {
                    reservas = ManejadorReserva.ConsultarReservasRecurso(recurso_u.Pk_recurso);
                }
                else
                {
                    ViewBag.Error = "Usted no tiene recurso asignado.";
                }
            }
            else
            {
                reservas = ManejadorReserva.ConsultarReservasRecurso(recurso);
            }
            ViewBag.idrec = recurso;
            if (reservas != null && reservas.Count() < 1)
            {
                ViewBag.Error = "No hay reservas en el sistema para este recurso.";
            }
            ViewBag.Reservas = reservas;
            string html = "<style>@media print { size: landscape; }</style>";

            html += "<h2 style='color: #333; text-align: center;margin-top: 20px;'>RESERVA DE RECURSOS</h2> ";
            html += "<br>";
            html += "<table style='width:100%;' class='styled-table landscape'>";
            html += "<tr class='header-row'><th>ID</th><th>Recurso</th><th>Usuario</th><th>Fecha</th><th>Hora</th><th>Costo</th><th>Estado</th></tr>";

            int cont = 1;
            foreach (ReservasViewModel reserva in reservas)
            {
                string bgColor = cont % 2 == 0 ? "#ffffff" : "#f2f2f2";
                html += $@"<tr style='background-color: {bgColor};'>
                        <td>{reserva.ReservaView.Pk_reserva}</td>
                        <td>{reserva.RecursoView.Pk_recurso}</td>
                        <td>{reserva.usuario.Nombre} {reserva.usuario.Apellido}</td>
                        <td>{reserva.horarioView.Fecha.ToShortDateString()}</td>
                        <td>{reserva.horarioView.Hora_inicio}</td>
                        <td>{reserva.horarioView.Costo}</td>
                        <td>{reserva.ReservaView.Fk_estado_reserva.Nombre}</td>
                    </tr>";
                cont++;
            }
            html += "</table>";

            return StyleBlock + html;
        }

        public FileResult ExportarReservaRecursosExcel(int recurso)
        {
            var usuario = new Usuario();
            if (User.Identity.IsAuthenticated)
            {
                long numeroDocumento = long.Parse(User.FindFirst("Numero_doc")?.Value);
                usuario.Numero_doc = numeroDocumento;
            }
            Recurso recurso_u = manejadorRecurso.RecursoUsuario(usuario.Numero_doc);
            List<ReservasViewModel> reservas = null;
            if (recurso == -1)
            {
                if (recurso_u != null)
                {
                    reservas = ManejadorReserva.ConsultarReservasRecurso(recurso_u.Pk_recurso);
                }
                else
                {
                    ViewBag.Error = "Usted no tiene recurso asignado.";
                }
            }
            else
            {
                reservas = ManejadorReserva.ConsultarReservasRecurso(recurso);
            }
            ViewBag.idrec = recurso;
            if (reservas != null && reservas.Count() < 1)
            {
                ViewBag.Error = "No hay reservas en el sistema para este recurso.";
            }
            ViewBag.Reservas = reservas;
            DataTable dt = new DataTable("Reservas de recursos");
            dt.Columns.AddRange(new DataColumn[7] {
                                        new DataColumn("ID reserva°"),
                                        new DataColumn("Recurso"),
                                        new DataColumn("Usuario"),
                                        new DataColumn("Fecha"),
                                        new DataColumn("Hora"),
                                        new DataColumn("Costo"),
                                        new DataColumn("Estado"),
            });

            int cont = 1;
            foreach (ReservasViewModel rp in reservas)
            {
                dt.Rows.Add(
                        rp.ReservaView.Pk_reserva,
                        rp.RecursoView.Pk_recurso,
                        rp.usuario.Nombre+" "+rp.usuario.Apellido,
                        rp.horarioView.Fecha.ToShortDateString(),
                        rp.horarioView.Hora_inicio,
                        rp.horarioView.Costo,
                        rp.ReservaView.Fk_estado_reserva.Nombre);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reservas de recurso.xlsx");
                }
            }
        }
    }
}

