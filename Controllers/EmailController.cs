using DataBase_RH_BanderaBlanca.Models;
using System;
using System.Collections.Generic; using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace RH_BanderaBlanca.Controllers
{
    public class EmailController : Controller
    {

        private string correoCredencial = "johans08lol@gmail.com";
        private string contrasenaCredencial = "hrko hhgq udbo rveu";
        // Enviar Email
        [HttpPost]
        public ActionResult EnviarEmail(string image, string usuario, string contrasena)
        {
            string tempImagePath = Path.Combine(Path.GetTempPath(), "credenciales.jpg");

            try
            {
                Persona _persona = new Persona();
                string correoDestino = _persona.ObtenerCorreo(usuario);

                // Decodificar la imagen de Base64
                byte[] imageBytes = Convert.FromBase64String(image);

                // Guardar la imagen como archivo temporal
                System.IO.File.WriteAllBytes(tempImagePath, imageBytes);

                // Configurar el cliente SMTP con el servidor SMTP correcto de Gmail
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // Puerto para conexiones TLS/SSL
                    Credentials = new NetworkCredential(correoCredencial, contrasenaCredencial), // Credenciales de Gmail
                    EnableSsl = true, // Aseguramos la conexión con SSL
                };

                // Crear el mensaje
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(correoCredencial),
                    Subject = "Credenciales de Usuario",
                    Body = $"Usuario: {usuario}<br/>Contraseña: {contrasena}",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(correoDestino);

                // Adjuntar la imagen usando `using` para liberar recursos
                using (var attachment = new Attachment(tempImagePath))
                {
                    mailMessage.Attachments.Add(attachment);
                    smtpClient.Send(mailMessage);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                // Eliminar el archivo temporal después de usarlo
                if (System.IO.File.Exists(tempImagePath))
                {
                    System.IO.File.Delete(tempImagePath);
                }
            }
        }

    }
}
