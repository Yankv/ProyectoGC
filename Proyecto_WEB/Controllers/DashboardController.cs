using ClosedXML.Excel;
using LogicaNegocio;
using LogicaNegocio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Proyecto_WEB.Controllers
{

	[Authorize]
	public class DashboardController : Controller
	{
		ManejadorInfoEmpresa ManejadorInfoEmpresa = new ManejadorInfoEmpresa();
		ManejadorRecurso ManejadorRecurso = new ManejadorRecurso();
		ManejadorHorario manejadorHorario = new ManejadorHorario();
		private string StyleBlock = "<style>" +
	  ".styled-table { width: 100%; color: black; border-collapse: collapse; margin: 20px 0; font-size: 16px; text-align: left; }" +
	  ".styled-table th, .styled-table td { border: 1px solid #dddddd; padding: 8px; text-align: left; }" +
	  ".styled-table tr { background-color: #f2f2f2; }" +
	  ".styled-table th { background-color: #4CAF50; color: white; }" +
	  "</style>";
		public IActionResult Dashboard(AgendaUsuario fechaInicio)
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
			List<Horario> agenda;
			if (fechaInicio.Fecha.ToString() == "1/01/0001 12:00:00 a. m.")
			{
				agenda = manejadorHorario.Ver_agenda(usuario.Numero_doc);
			}
			else
			{
				agenda = manejadorHorario.Ver_agenda(usuario.Numero_doc, fechaInicio.Fecha);
			}
			ViewBag.User = usuario;
			if (agenda.Count > 0)
			{
				ViewBag.Agenda = agenda;
			}
			return View("Dashboard");
		}
		public IActionResult GestionarAgenda([FromQuery] int pkRecurso = 0)
		{
			ViewBag.recurso = pkRecurso;
			List<Recurso> recursos = ManejadorRecurso.Consultar_recursos(0);
			List<Horario> agenda_recursos;
			int valorRecurso = pkRecurso;
			if (valorRecurso == 0)
			{
				agenda_recursos = manejadorHorario.Consultar_agenda_recursos(0);
			}
			else
			{
				agenda_recursos = manejadorHorario.Consultar_agenda_recursos(valorRecurso);
			}
			ViewBag.Agenda = agenda_recursos;
			ViewBag.Recursos = recursos;
			return View();

		}

		[Route("Dashboard/EliminarHorario/{idHorario}")]
		[HttpPost]
		public IActionResult EliminarHorario(int idHorario)
		{
			bool resp = manejadorHorario.EliminarHorario(idHorario);
			return RedirectToAction("GestionarAgenda");
		}

		[HttpPost]
		public FileResult ExportarAgendaRecursos(int idRecurso)
		{
			string html = ObtenerHtmlAgendas(idRecurso);
			return File(Reportes.Reporte.Export(html), "application/pdf", "Agenda de recursos.pdf");
		}

		private string ObtenerHtmlAgendas(int pkRecurso)
		{
			List<Horario> agenda_recursos;
			int valorRecurso = pkRecurso;
			if (valorRecurso == 0)
			{
				agenda_recursos = manejadorHorario.Consultar_agenda_recursos(0);
			}
			else
			{
				agenda_recursos = manejadorHorario.Consultar_agenda_recursos(valorRecurso);
			}
			ViewBag.Agenda = agenda_recursos;
			string html = "<style>@media print { size: landscape; }</style>";
            html += "<h2 style='color: #333; text-align: center;margin-top: 20px;'>LISTADO DE AGENDAS </h2> ";
			html += "<br>";
			html += "<table style='width:100%;' class='styled-table landscape'>";
			html += "<tr class='header-row'><th>Recurso</th><th>Dia</th><th>Fecha</th><th>Duración</th><th>Hora</th><th>Costo</th><th>Estado</th></tr>";

			int cont = 1;
			foreach (Horario agenda in agenda_recursos)
			{
				string bgColor = cont % 2 == 0 ? "#ffffff" : "#f2f2f2";
				html += $@"<tr style='background-color: {bgColor};'>
                        <td>{agenda.Fk_recurso.Pk_recurso}</td>
                        <td>{agenda.Fk_dia.nombre}</td>
                        <td>{agenda.Fecha.ToShortDateString()}</td>
                        <td>{agenda.Duracion}</td>
                        <td>{agenda.Hora_inicio}</td>
                        <td>{agenda.Costo}</td>
                        <td>{agenda.Estado}</td>
                    </tr>";
				cont++;
			}
			html += "</table>";

            return StyleBlock + html;
		}

		public FileResult ExportarAgendaRecursoExcel(int idRecurso)
		{
			DataTable dt = new DataTable("Agenda de recursos");
			dt.Columns.AddRange(new DataColumn[7] {
										new DataColumn("Recurso"),
										new DataColumn("Dia"),
										new DataColumn("Fecha"),
										new DataColumn("Duración"),
										new DataColumn("Hora"),
										new DataColumn("Costo"),
										new DataColumn("Estado")
			});

			List<Horario> agenda_recursos;
			int valorRecurso = idRecurso;
			if (valorRecurso == 0)
			{
				agenda_recursos = manejadorHorario.Consultar_agenda_recursos(0);
			}
			else
			{
				agenda_recursos = manejadorHorario.Consultar_agenda_recursos(valorRecurso);
			}
			foreach (Horario rp in agenda_recursos)
			{
				dt.Rows.Add(
					   rp.Fk_recurso.Pk_recurso,
						rp.Fk_dia.nombre,
						rp.Fecha.ToShortDateString(),
						rp.Duracion,
						rp.Hora_inicio,
						rp.Costo,
						rp.Estado);
			}

			using (XLWorkbook wb = new XLWorkbook())
			{
				wb.Worksheets.Add(dt);
				using (MemoryStream stream = new MemoryStream())
				{
					wb.SaveAs(stream);
					return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Agenda de recursos.xlsx");
				}
			}
		}

	}
}
