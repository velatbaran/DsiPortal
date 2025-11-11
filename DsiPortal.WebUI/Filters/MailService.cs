namespace DsiPortal.WebUI.Filters
{
    using Microsoft.Extensions.Options;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    public class MailService : IMailService
    {
        private readonly MailSettings _settings;
        private readonly ILogger<MailService> _logger;

        public MailService(IOptions<MailSettings> settings, ILogger<MailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendEmailWithAttachmentAsync(MailRequest request, byte[] fileBytes, string fileName)
        {
            // SSL sertifika bypass (iç ağ testleri için)
            ServicePointManager.ServerCertificateValidationCallback =
                delegate { return true; };

            using var smtp = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.UserName, _settings.Password),
                EnableSsl = _settings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.From, _settings.DisplayName),
                Subject = request.Subject,
                Body = request.Body,
                IsBodyHtml = true
            };

            foreach (var to in request.ToEmail)
                message.To.Add(to);

            // MemoryStream using dışında olmalı
            var ms = new MemoryStream(fileBytes);
            var attachment = new Attachment(ms, fileName,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            message.Attachments.Add(attachment);

            try
            {
                await smtp.SendMailAsync(message);
                _logger.LogInformation("E-posta gönderildi: {To}", string.Join(", ", request.ToEmail));
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP hatası: {Error}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Genel mail hatası: {Error}", ex.Message);
                throw;
            }
        }

    }
}

