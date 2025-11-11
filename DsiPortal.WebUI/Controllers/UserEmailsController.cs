using DsiPortal.Core.Entities;
using DsiPortal.Service.IService;
using DsiPortal.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace DsiPortal.WebUI.Controllers
{
    [Authorize]
    public class UserEmailsController : Controller
    {
        private readonly IService<UserEmails> _serviceUserEmails;
        private readonly IToastNotification _toastNotification;

        public UserEmailsController(IService<UserEmails> serviceUserEmails, IToastNotification toastNotification)
        {
            _serviceUserEmails = serviceUserEmails;
            _toastNotification = toastNotification;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _serviceUserEmails.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserEmailViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userEmail = new UserEmails()
                {
                    Eposta = viewModel.Eposta,
                    Created = HttpContext.User.FindFirst("Username").Value,
                    CreatedDate = DateTime.Now,
                };
                _serviceUserEmails.Add(userEmail);
                    await _serviceUserEmails.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userEmail = await _serviceUserEmails.FindAsync(id);
            if (userEmail == null)
            {
                return NotFound();
            }
            return View(userEmail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(UserEmails userEmails, int id)
        {
            userEmails.Created = HttpContext.User.FindFirst("Username").Value;
            if (ModelState.IsValid)
            {
                var _userEmails = await _serviceUserEmails.FindAsync(id);
                if (await _serviceUserEmails.AnyAsync(x => x.Eposta == userEmails.Eposta && x.Id != id))
                {
                    _toastNotification.AddWarningToastMessage("Aynı eposta sistemde kayıtlı. Lütfen başka bir kullanıcı adı giriniz!", new ToastrOptions { Title = "Uyarı" });
                    return View(userEmails);
                }

                _userEmails.Eposta = userEmails.Eposta;
                _userEmails.Created = userEmails.Created;

                _serviceUserEmails.Update(_userEmails);
                await _serviceUserEmails.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                return RedirectToAction(nameof(Index));
            }
            return View(userEmails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userEmails = await _serviceUserEmails.FindAsync(id);
            if (userEmails != null)
            {
                _serviceUserEmails.Delete(userEmails);
            }

            await _serviceUserEmails.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }
    }
}
