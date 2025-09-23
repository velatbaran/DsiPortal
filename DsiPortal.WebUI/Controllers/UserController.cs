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
    public class UserController : Controller
    {
        private readonly IService<Users> _serviceUser;
        private readonly IToastNotification _toastNotification;

        public UserController(IService<Users> serviceUser, IToastNotification toastNotification)
        {
            _serviceUser = serviceUser;
            _toastNotification = toastNotification;
        }

        public IActionResult Index()
        {
            return View(_serviceUser.GetAll(x=>x.IsActive == true).OrderByDescending(x => x.CreatedDate));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var _user = _serviceUser.GetQueryable().Where(x=>x.Username == viewModel.Username).FirstOrDefault();
                if (_user != null)
                {
                    _toastNotification.AddWarningToastMessage("Aynı kullanıcı adı sistemde kayıtlı. Lütfen başka bir kullanıcı adı giriniz!", new ToastrOptions { Title = "Uyarı" });
                    return View(viewModel);
                }

                var user = new Users
                {
                    Name = viewModel.Name,
                    Surname = viewModel.Surname,
                    Username = viewModel.Username,
                    Password = viewModel.Password,
                    Created = HttpContext.User.FindFirst("Username").Value,
                    CreatedDate = DateTime.Now,
                    UserGuid = Guid.NewGuid(),
                    IsAdmin = false,
                    IsActive = true
                };

                _serviceUser.Add(user);
                await _serviceUser.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                return RedirectToAction(nameof(Index));
            }
            _toastNotification.AddWarningToastMessage("Lütfen gerekli alanları doldurun", new ToastrOptions { Title = "Uyarı" });
            return View(viewModel);
        }

        public async Task<IActionResult> EditAsync(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _serviceUser.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(Users user,int id)
        {
            if (ModelState.IsValid)
            {
                var _user = await _serviceUser.FindAsync(id);
                if (await _serviceUser.AnyAsync(x=>x.Username == user.Username && x.Id != id))
                {
                    _toastNotification.AddWarningToastMessage("Aynı kullanıcı adı sistemde kayıtlı. Lütfen başka bir kullanıcı adı giriniz!", new ToastrOptions { Title = "Uyarı" });
                    return View(user);
                }

                _user.Name = user.Name;
                _user.Surname = user.Surname;
                _user.Username = user.Username;
                _user.Password = user.Password;
                _user.Created = HttpContext.User.FindFirst("Username").Value;
                _user.CreatedDate = DateTime.Now;
                _user.UserGuid = Guid.NewGuid();
                _user.IsAdmin = user.IsAdmin;
                _user.IsActive = user.IsActive;

                _serviceUser.Update(_user);
                await _serviceUser.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _serviceUser.FindAsync(id);
            if (user != null)
            {
                _serviceUser.Delete(user);
            }

            await _serviceUser.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }
    }
}
