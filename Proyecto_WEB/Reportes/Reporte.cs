using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using LogicaNegocio;
using LogicaNegocio.Models;

namespace Proyecto_WEB.Reportes
{
    public class Reporte
    {

        public static byte[] Export(string html)
        {

            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(html);
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 30f, 30f, 40f, 40f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);

                HeaderFooterEvent events = new HeaderFooterEvent();
                writer.PageEvent = events;

                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return stream.ToArray();
            }
        }
        public class HeaderFooterEvent : PdfPageEventHelper
        {
            public ManejadorInfoEmpresa ManejadorEmpresa = new ManejadorInfoEmpresa();

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);

                InfoEmpresa empresa = ManejadorEmpresa.ConsultarInfo();

                // Calcular la altura del footer
                PdfPTable footer = new PdfPTable(1);
                PdfPCell cell = new PdfPCell(new Phrase("Datos de la Empresa", new Font(Font.FontFamily.HELVETICA, 8)));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                footer.AddCell(cell);

                cell = new PdfPCell(new Phrase($"Contacto: {empresa.Correo} - {empresa.Telefono}", new Font(Font.FontFamily.HELVETICA, 8)));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                footer.AddCell(cell);

                cell = new PdfPCell(new Phrase($"Dirección: {empresa.Direccion}", new Font(Font.FontFamily.HELVETICA, 8)));
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                footer.AddCell(cell);

                float footerHeight = footer.TotalHeight;

                // Posicionamiento del footer
                float pageWidth = document.PageSize.Width;
                float pageHeight = document.PageSize.Height;

                // Alineación a la izquierda, inferior
                float x = document.LeftMargin;
                float y = document.BottomMargin;

                // Dibujar el footer
                footer.TotalWidth = pageWidth - document.LeftMargin - document.RightMargin;
                footer.WriteSelectedRows(0, -1, x, y, writer.DirectContent);
            }
            public override void OnStartPage(PdfWriter writer, Document document)
            {
                base.OnStartPage(writer, document);

                InfoEmpresa empresa = ManejadorEmpresa.ConsultarInfo();

                if (writer.PageNumber == 1)
                {
                    ColumnText.ShowTextAligned(
                    writer.DirectContent,
                    Element.ALIGN_CENTER,
                    new Phrase(empresa.Nombre, new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD)),
                    document.PageSize.Width / 2,
                    document.PageSize.Height - document.TopMargin + 5, // Ajusta esta coordenada según sea necesario
                    0
                    );
                }

            }
        }



    }
}
