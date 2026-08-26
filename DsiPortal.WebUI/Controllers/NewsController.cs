using DsiPortal.Core.Entities;
using DsiPortal.Service.IService;
using DsiPortal.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace DsiPortal.WebUI.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly IService<WorksConducteds> _serviceWorksConducteds;
        private readonly IToastNotification _toastNotification;

        public NewsController(IService<WorksConducteds> serviceWorksConducteds, IToastNotification toastNotification)
        {
            _serviceWorksConducteds = serviceWorksConducteds;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _serviceWorksConducteds.GetQueryable().OrderByDescending(x=>x.CreatedDate).ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorksConductedViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.ImageFiles != null && viewModel.ImageFiles.Count > 0)
                {
                    bool firstImage = true;

                    foreach (var file in viewModel.ImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await file.CopyToAsync(memoryStream);

                                var image = new WorksConducteds
                                {
                                    WorkingName = viewModel.WorkingName,
                                    Description = viewModel.Description,
                                    CreatedDate = DateTime.Now,
                                    Created = HttpContext.User.FindFirst("Username").Value,
                                    Images = memoryStream.ToArray(),
                                    FileType = file.ContentType,
                                    FileName = file.FileName,
                                    IsMainImage = firstImage
                                };

                                await _serviceWorksConducteds.AddAsync(image);
                            }
                        }
                        firstImage = false;
                    }

                    await _serviceWorksConducteds.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return Redirect(nameof(Index));
                }
                _toastNotification.AddErrorToastMessage("Kayıt işlemi yapılırken hata oluştu.", new ToastrOptions { Title = "Hata" });
                return View(viewModel);
            }
            _toastNotification.AddErrorToastMessage("Lütfen zorunlu alanları doldurun.", new ToastrOptions { Title = "Hata" });
            return View(viewModel);
        }

        public async Task<IActionResult> ViewImage(int id)
        {
            var image = await _serviceWorksConducteds.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            return File(image.Images, image.FileType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var announcements = await _serviceWorksConducteds.FindAsync(id);
            if (announcements != null)
            {
                _serviceWorksConducteds.Delete(announcements);

                _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Silme işlemi yapılırken hata oluştu.", new ToastrOptions { Title = "Hata" });
            }

            await _serviceWorksConducteds.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSelected(List<int> ids)
        {
            if (ids != null && ids.Any())
            {
                foreach (var id in ids)
                {
                    var announcement = await _serviceWorksConducteds.FindAsync(id);
                    if (announcement != null)
                    {
                        _serviceWorksConducteds.Delete(announcement);
                        _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Silme işlemi yapılırken hata oluştu.", new ToastrOptions { Title = "Hata" });
                    }
                }
                await _serviceWorksConducteds.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
