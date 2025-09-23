using DsiPortal.Core.Entities;
using DsiPortal.Service.IService;
using DsiPortal.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DsiPortal.WebUI.Controllers
{
    [Authorize]
    [Authorize(Policy = "AdminPolicy")]

    public class CameraController : Controller
    {
        private readonly IService<Cameras> _serviceCameras;
        private readonly IToastNotification _toastNotification;

        public CameraController(IService<Cameras> serviceCameras, IToastNotification toastNotification)
        {
            _serviceCameras = serviceCameras;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _serviceCameras.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CamerasViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                    var document = new Cameras
                    {
                        Name = viewModel.Name,
                        Link = viewModel.Link,
                        Password = viewModel.Password,
                        Created = HttpContext.User.FindFirst("Username").Value,
                        CreatedDate = DateTime.Now,
                    };

                    _serviceCameras.Add(document);
                    await _serviceCameras.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));

            }
            return View(viewModel);
        }

        public async Task<IActionResult> EditAsync(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var camera = await _serviceCameras.FindAsync(id);
            if (camera == null)
            {
                return NotFound();
            }
            return View(camera);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(Cameras camera, int id)
        {
            if (ModelState.IsValid)
            {
                var _camera = await _serviceCameras.FindAsync(id);

                _camera.Name = camera.Name;
                _camera.Link = camera.Link;
                _camera.Password = camera.Password;
                _camera.Created = HttpContext.User.FindFirst("Username").Value;
                _camera.CreatedDate = DateTime.Now;

                _serviceCameras.Update(_camera);
                await _serviceCameras.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                return RedirectToAction(nameof(Index));
            }
            return View(camera);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var announcements = await _serviceCameras.FindAsync(id);
            if (announcements != null)
            {
                _serviceCameras.Delete(announcements);
            }

            await _serviceCameras.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }
    }
}
