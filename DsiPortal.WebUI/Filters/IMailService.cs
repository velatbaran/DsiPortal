namespace DsiPortal.WebUI.Filters
{
    public interface IMailService
    {
        /// <summary>
        /// Excel veya başka bir dosya ekli e-posta gönderir.
        /// </summary>
        /// <param name="request">E-posta içeriği (alıcılar, konu, içerik, HTML vs.)</param>
        /// <param name="fileBytes">Veritabanında saklanan dosyanın byte[] verisi</param>
        /// <param name="fileName">Dosya adı (örnek: Rapor.xlsx)</param>
        Task SendEmailWithAttachmentAsync(MailRequest request, byte[] fileBytes, string fileName);
    }

}
