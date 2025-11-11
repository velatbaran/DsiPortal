
using DocumentFormat.OpenXml.InkML;
using DsiPortal.Core.Entities;
using DsiPortal.Service.IService;
using DsiPortal.WebUI.Filters;
using DsiPortal.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NToastNotify;

namespace DsiPortal.WebUI.Controllers
{
    [Authorize]
    public class FoodListController : Controller
    {
        private readonly IService<FoodList> _serviceFoodList;
        private readonly IService<UserEmails> _serviceUserEmails;
        private readonly IToastNotification _toastNotification;
        private readonly IMailService _mailService;


        public FoodListController(IService<FoodList> serviceFoodList, IToastNotification toastNotification, IMailService mailService, IService<UserEmails> serviceUserEmails)
        {
            _serviceFoodList = serviceFoodList;
            _toastNotification = toastNotification;
            _mailService = mailService;
            _serviceUserEmails = serviceUserEmails;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _serviceFoodList.GetQueryable().OrderByDescending(x=>x.CreatedDate).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FoodListViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await viewModel.File.CopyToAsync(memoryStream);

                    var document = new FoodList
                    {
                        Name = viewModel.Name,
                        FileType = Path.GetExtension(viewModel.File.FileName),
                        Created = HttpContext.User.FindFirst("Username").Value,
                        CreatedDate = DateTime.Now,
                        Content = memoryStream.ToArray()
                    };

                    _serviceFoodList.Add(document);
                    await _serviceFoodList.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Download(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _serviceFoodList.FindAsync(id.Value);
            if (document == null)
            {
                return NotFound();
            }

            return File(document.Content, GetMimeType(document.FileType), document.Name + document.FileType);
        }

        public IActionResult ViewPdf(int id)
        {
            var document = _serviceFoodList.Find(id);
            if (document == null)
            {
                return NotFound();
            }

            // Content-Disposition header'ını kaldırarak tarayıcının PDF'yi açmasını sağlıyoruz
            Response.Headers.Add("Content-Disposition", "inline; filename=" + document.Name + ".pdf");

            return File(document.Content, "application/pdf");
        }

        private string GetMimeType(string fileType)
        {
            switch (fileType.ToLower())
            {
                case ".pdf":
                    return "application/pdf";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".doc":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".xls":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                default:
                    return "application/octet-stream";
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var announcements = await _serviceFoodList.FindAsync(id);
            if (announcements != null)
            {
                _serviceFoodList.Delete(announcements);
            }

            await _serviceFoodList.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SendEposta()
        {
            var allUsers = await _serviceUserEmails.GetAllAsync();
            var userEmails = allUsers.Select(x=>x.Eposta).ToList();

            if (!userEmails.Any())
            {
                _toastNotification.AddSuccessToastMessage("Veritabanında e-posta adresi bulunamadı.", new ToastrOptions { Title = "Başarılı" });
                return RedirectToAction(nameof(Index));
            }

            var foodList = _serviceFoodList.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();

            var mail = new MailRequest
            {
                ToEmail = userEmails,
                Subject = $"Dsi 24.Bölge Müdürlüğü - {foodList.Name}",
                Body = @"
<div style='font-family: Arial, sans-serif; font-size: 14px;'>
    <h2 style='border-bottom: 2px solid #2E86C1; padding-bottom: 5px;'>
        Merhaba,
    </h2>
    <h4 style='font-size: 16px; line-height: 1.5;'>
        Ek'te güncel <strong>yemek listesini</strong> bulabilirsiniz.
    </h4>
    <h4 style='margin-top: 20px;'>
        İyi günler dileriz,<br/>
        <strong>DSİ 24. Bölge Müdürlüğü</strong>
    </h4>
</div>"
            };

            await _mailService.SendEmailWithAttachmentAsync(mail, foodList.Content, foodList.Name);

            _toastNotification.AddSuccessToastMessage("E -posta başarıyla gönderildi", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }

    }
}
