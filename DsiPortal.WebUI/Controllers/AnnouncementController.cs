using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Permissions;
using DsiPortal.Core.Entities;
using DsiPortal.WebUI.Models;
using DsiPortal.Service.IService;
using NToastNotify;
using Microsoft.AspNetCore.Authorization;


namespace DsiPortal.WebUI.Controllers
{
    [Authorize]
    [Authorize(Policy = "AdminPolicy")]
    public class AnnouncementController : Controller
    {
        private readonly IService<Announcements> _serviceAnnouncements;
        private readonly IToastNotification _toastNotification;

        public AnnouncementController(IService<Announcements> serviceAnnouncements, IToastNotification toastNotification)
        {
            _serviceAnnouncements = serviceAnnouncements;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _serviceAnnouncements.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnnocumentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await viewModel.File.CopyToAsync(memoryStream);

                    var document = new Announcements
                    {
                        Name = viewModel.Name,
                        FileType = Path.GetExtension(viewModel.File.FileName),
                        Created = HttpContext.User.FindFirst("Username").Value,
                        CreatedDate = DateTime.Now,
                        Content = memoryStream.ToArray()
                    };

                    _serviceAnnouncements.Add(document);
                    await _serviceAnnouncements.SaveChangesAsync();
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

            var document = await _serviceAnnouncements.FindAsync(id.Value);
            if (document == null)
            {
                return NotFound();
            }

            return File(document.Content, GetMimeType(document.FileType), document.Name + document.FileType);
        }

        public IActionResult ViewPdf(int id)
        {
            var document = _serviceAnnouncements.Find(id);
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
            var announcements = await _serviceAnnouncements.FindAsync(id);
            if (announcements != null)
            {
               _serviceAnnouncements.Delete(announcements);
            }

            await _serviceAnnouncements.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }
    }
}
