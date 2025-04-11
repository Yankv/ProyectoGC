using ClosedXML.Excel;
using iTextSharp.text.pdf.codec.wmf;
using LogicaNegocio;
using LogicaNegocio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Proyecto_WEB.Controllers
{
    [Authorize]
    public class RecursoController : Controller
    {
        private string StyleBlock = "<style>" +
    ".styled-table { width: 100%; color: black; border-collapse: collapse; margin: 20px 0; font-size: 16px; text-align: left; }" +
    ".styled-table th, .styled-table td { border: 1px solid #dddddd; padding: 8px; text-align: left; }" +
    ".styled-table tr { background-color: #f2f2f2; }" +
    ".styled-table th { background-color: #4CAF50; color: white; }" +
    "</style>";
        ManejadorRecurso ManejadorRecurso = new ManejadorRecurso();
        ManejadorUsuario ManejadorUsuario = new ManejadorUsuario();
        ManejadorHorario ManejadorHorario = new ManejadorHorario();

        public IActionResult Gestionar()
        {
            List<Recurso> recursos = ManejadorRecurso.Consultar_recursos(0);
            ViewBag.Recursos = recursos;
            return View();
        }

        public IActionResult Crear()
        {
            List<Usuario> usuarios = ManejadorUsuario.ObtenerUsuarios(0);
            ViewBag.Usuarios = usuarios;
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Recurso recurso)
        {
            List<Usuario> usuarios = ManejadorUsuario.ObtenerUsuarios(0);
            ViewBag.Usuarios = usuarios;
            if (ManejadorRecurso.Crear_recurso(recurso))
            {
                return RedirectToAction("Gestionar");
            }
            ViewBag.Error = "No se pudo crear el recurso.";
            return View();
        }

        public IActionResult Asignar_horario()
        {
            List<Recurso> recursos = ManejadorRecurso.Consultar_recursos(0);
            ViewBag.Recursos = recursos;
            return View();
        }

        [HttpPost]
        public JsonResult Crear_horarios([FromBody] IEnumerable<Horario> horarios)
        {
            bool result = ManejadorHorario.Crear_horarios(horarios);
            return Json(result);
        }


        [HttpPost]
        public FileResult ExportarReporteRecurso(string btnExportar)
        {
            switch (btnExportar)
            {
                case "Generar reporte (PDF)":
                    return ExportarRecursos();
                case "Generar reporte (EXCEL)":
                    return ExportarRecursoExcel();
            }
            return null;
        }
        public FileResult ExportarRecursos()
        {
            string html = ObtenerHtmlRecursos();
            return File(Reportes.Reporte.Export(html), "application/pdf", "Recursos.pdf");
        }

        private string ObtenerHtmlRecursos()
        {
            List<Recurso> recursos = ManejadorRecurso.Consultar_recursos(0);

            string html = "<style>@media print { size: landscape; }</style>";

            html += "<h2 style='color: #333; text-align: center;'>LISTADO DE RECURSOS DEL SISTEMA</h2>";

            html += "<table style='width:100%;' class='styled-table landscape'>"; 

            html += "<tr class='header-row'><th>N°</th><th>Nombre</th><th>Direccion</th><th>Tipo de recurso</th><th>Encargado</th><th>Estado</th></tr>";

            int cont = 1;
            foreach (Recurso recurso in recursos)
            {
                string bgColor = cont % 2 == 0 ? "#ffffff" : "#f2f2f2";
                html += $@"<tr style='background-color: {bgColor};'>
                        <td>{cont}</td>
                        <td>{recurso.Nombre}</td>
                        <td>{recurso.Direccion}</td>
                        <td>{recurso.Fk_tp_recurso.Nombre}</td>
                        <td>{recurso.Fk_usuario_encargado.Nombre}</td>
                        <td>{recurso.Estado}</td>
                    </tr>";

                cont++;
            }
            html += "</table>";

            return StyleBlock + html;
        }
        public FileResult ExportarRecursoExcel()
        {
            DataTable dt = new DataTable("Recursos");
            dt.Columns.AddRange(new DataColumn[6] {
                                        new DataColumn("N°"),
                                        new DataColumn("Nombre"),
                                        new DataColumn("Direccion"),
                                        new DataColumn("Tipo de recurso"),
                                        new DataColumn("Encargado"),
                                        new DataColumn("Estado"),
            });

            List<Recurso> recursos = ManejadorRecurso.Consultar_recursos(0);

            int cont = 1;
            foreach (Recurso rp in recursos)
            {
                dt.Rows.Add(cont,
                        rp.Nombre,
                        rp.Direccion,
                        rp.Fk_tp_recurso.Nombre,
                        rp.Fk_usuario_encargado.Nombre,
                        rp.Estado);
                cont++;
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Recursos.xlsx");
                }
            }
        }
    }
}
