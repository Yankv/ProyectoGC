using LogicaNegocio.Models;

using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace LogicaNegocio.Servicios

{
    public class CorreoServicio
    {
        private static string _Host = "smtp.gmail.com";
        private static int _Puerto = 587;

        private static string _NombreEnvia = "Juan Nicolas Vela Tovar";
        private static string _Correo = "nicolasvela63@gmail.com";
        private static string _Clave = "rgus zeln ryyw khpb";

        public static bool Enviar(CorreoDTO correodto)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_NombreEnvia, _Correo));
                email.Subject = correodto.Asunto;
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = correodto.Contenido
                };


                foreach (var destinatario in correodto.Para.Split(','))
                {
                    email.To.Add(MailboxAddress.Parse(destinatario.Trim()));
                }
                // ... Configuración del mensaje

                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    smtp.ServerCertificateValidationCallback = (s, c, h, e) => true; // Deshabilitar la validación del certificado

                    smtp.Connect(_Host, _Puerto, SecureSocketOptions.StartTls);
                    smtp.Authenticate(_Correo, _Clave);
                    smtp.Send(email);
                    smtp.Disconnect(true);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex}");
                return false;
            }
        }
    }
}

