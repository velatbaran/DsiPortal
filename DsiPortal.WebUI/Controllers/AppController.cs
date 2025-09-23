
using DsiPortal.Core.Entities;
using DsiPortal.Service.IService;
using DsiPortal.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Text;

namespace DsiPortal.WebUI.Controllers
{
    [Authorize]
    [Authorize(Policy = "AdminPolicy")]
    public class AppController : Controller
    {
        private readonly IService<Apps> _serviceApps;
        private readonly IToastNotification _toastNotification;

        public AppController(IService<Apps> serviceApps, IToastNotification toastNotification)
        {
            _serviceApps = serviceApps;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _serviceApps.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await viewModel.Image.CopyToAsync(memoryStream);
                    var document = new Apps
                    {
                        Name = viewModel.Name,
                        Link = viewModel.Link,
                        FileType = viewModel.Image.ContentType,
                        Created = HttpContext.User.FindFirst("Username").Value,
                        CreatedDate = DateTime.Now,
                        Image = memoryStream.ToArray()
                    };

                    _serviceApps.Add(document);
                    await _serviceApps.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(viewModel);
        }
        public async Task<IActionResult> ViewImage(int id)
        {
            var image = await _serviceApps.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            return File(image.Image,image.FileType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var announcements = await _serviceApps.FindAsync(id);
            if (announcements != null)
            {
                _serviceApps.Delete(announcements);
            }

            await _serviceApps.SaveChangesAsync();
              _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }
    }
}
