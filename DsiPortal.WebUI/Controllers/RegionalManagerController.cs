using DsiPortal.Core.Entities;
using DsiPortal.Service.IService;
using DsiPortal.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsiPortal.WebUI.Controllers
{
    [Authorize]
    public class RegionalManagerController : Controller
    {
        private readonly IService<RegionalManagers> _serviceRegionalManagers;

        public RegionalManagerController(IService<RegionalManagers> serviceRegionalManagers)
        {
            _serviceRegionalManagers = serviceRegionalManagers;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _serviceRegionalManagers.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegionalManagerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await viewModel.Image.CopyToAsync(memoryStream);
                    var document = new RegionalManagers
                    {
                        Name = viewModel.Name,
                        Title = viewModel.Title,
                        FileType = viewModel.Image.ContentType,
                        Created = HttpContext.User.FindFirst("Username").Value,
                        CreatedDate = DateTime.Now,
                        Image = memoryStream.ToArray()
                    };

                    _serviceRegionalManagers.Add(document);
                    await _serviceRegionalManagers.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(viewModel);
        }

        public async Task<IActionResult> ViewImage(int id)
        {
            var image = await _serviceRegionalManagers.FindAsync(id);
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
            var announcements = await _serviceRegionalManagers.FindAsync(id);
            if (announcements != null)
            {
                _serviceRegionalManagers.Delete(announcements);
            }

            await _serviceRegionalManagers.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
