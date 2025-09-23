using DsiPortal.Core.Entities;
using DsiPortal.Service.IService;
using DsiPortal.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NToastNotify;

namespace DsiPortal.WebUI.Controllers
{
    [Authorize]
    [Authorize(Policy = "AdminPolicy")]
    public class BenefitLinkController : Controller
    {
        private readonly IService<BenefitLinks> _serviceBenefitLinks;
        private readonly IToastNotification _toastNotification;

        public BenefitLinkController(IService<BenefitLinks> serviceBenefitLinks, IToastNotification toastNotification)
        {
            _serviceBenefitLinks = serviceBenefitLinks;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _serviceBenefitLinks.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BenefitLinkViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await viewModel.Image.CopyToAsync(memoryStream);
                    var document = new BenefitLinks
                    {
                        Name = viewModel.Name,
                        Link = viewModel.Link,
                        FileType = viewModel.Image.ContentType,
                        Created = HttpContext.User.FindFirst("Username").Value,
                        CreatedDate = DateTime.Now,
                        Image = memoryStream.ToArray()
                    };

                    _serviceBenefitLinks.Add(document);
                    await _serviceBenefitLinks.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(viewModel);
        }
        public async Task<IActionResult> ViewImage(int id)
        {
            var image = await _serviceBenefitLinks.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            return File(image.Image, image.FileType);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var announcements = await _serviceBenefitLinks.FindAsync(id);
            if (announcements != null)
            {
                _serviceBenefitLinks.Delete(announcements);
            }

            await _serviceBenefitLinks.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }
    }
}
