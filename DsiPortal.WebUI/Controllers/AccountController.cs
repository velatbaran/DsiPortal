using DsiPortal.Core.Entities;
using DsiPortal.Service.IService;
using DsiPortal.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Claims;

namespace DsiPortal.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IService<Users> _serviceUser;
        private readonly IToastNotification _toastNotification;

        public AccountController(IService<Users> serviceUser, IToastNotification toastNotification)
        {
            _serviceUser = serviceUser;
            _toastNotification = toastNotification;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> MyProfileAsync()
        {
            Users user = await _serviceUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value.ToString());
            if (user == null)
            {
                return NotFound();
            }
            var model = new MyProfileViewModel()
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Surname = user.Surname,
            };
            return View(model);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> MyProfileAsync(MyProfileViewModel model,int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _serviceUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
                    if (await _serviceUser.AnyAsync(x => x.Username == model.Username && x.Id != id))
                    {
                        _toastNotification.AddWarningToastMessage("Aynı kullanıcı adı sistemde kayıtlı. Lütfen başka bir kullanıcı adı giriniz!", new ToastrOptions { Title = "Uyarı" });
                        return View(model);
                    }

                    user.Username = model.Username;
                    user.Name = model.Name;
                    user.Surname = model.Surname;
                    _serviceUser.Update(user);
                    var sonuc = await _serviceUser.SaveChangesAsync();
                    if (sonuc > 0)
                    {
                        _toastNotification.AddSuccessToastMessage("Bilgileriniz başarıyla güncellenmiştir", new ToastrOptions { Title = "Başarılı" });
                        return View(model);
                    }
                }
                catch (Exception)
                {
                    _toastNotification.AddErrorToastMessage("Bilgileriniz güncellenirken hata", new ToastrOptions { Title = "Hata" });
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgetPasswordAsync()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    Users user = await _serviceUser.GetAsync(x => x.Username == model.Username);
                    if (user is null)
                    {
                        _toastNotification.AddErrorToastMessage("Sistemde kayıtlı böyle bir kullanıcı adı yok!", new ToastrOptions { Title = "Hata" });
                        return View(model);
                    }

                    user.Password = model.Password;
                    _serviceUser.Update(user);
                    var sonuc = await _serviceUser.SaveChangesAsync();
                    if (sonuc > 0)
                    {
                        _toastNotification.AddSuccessToastMessage("Şifreniz başarıyla değişti.", new ToastrOptions { Title = "Başarılı" });
                        return View(model);
                    }

                }
                catch (Exception)
                {
                    _toastNotification.AddErrorToastMessage("Şifreniz değiştirilirken hata oluştu!", new ToastrOptions { Title = "Hata" });
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Users user = await _serviceUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
                    user.Password = model.Password;
                    _serviceUser.Update(user);
                   await _serviceUser.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Şifre değiştirme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return View(model);
                }
                catch (Exception)
                {
                    // loglama
                    _toastNotification.AddErrorToastMessage("Lütfen şifre bilgilerini kontrol ediniz", new ToastrOptions { Title = "Hatalı" });
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var account = await _serviceUser.GetAsync(x => x.Username == model.Username & x.Password == model.Password & x.IsActive);
                    if (account == null)
                    {

                        _toastNotification.AddErrorToastMessage("Kullanıcı adı veya şifre hatalı", new ToastrOptions { Title = "Hatalı" });
                        return View(model);
                    }
                    else
                    {
                        var claims = new List<Claim>()
                        {
                            new(ClaimTypes.Name,account.Name),
                            new(ClaimTypes.Surname,account.Surname),
                            new(ClaimTypes.Role,account.IsAdmin ? "Admin" : "Standart"),
                            new("Username",account.Username.ToString()),
                            new("UserId",account.Id.ToString()),
                            new("UserGuid",account.UserGuid.ToString())
                        };

                        var userIdentity = new ClaimsIdentity(claims, "Login");
                        ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);
                        await HttpContext.SignInAsync(userPrincipal);
                        _toastNotification.AddSuccessToastMessage("Giriş işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                        return Redirect(string.IsNullOrEmpty(model.ReturnUrl) ? "/Account/Index" : model.ReturnUrl);
                    }
                }
                catch (Exception)
                {
                    // loglama
                    _toastNotification.AddErrorToastMessage("Lütfen kullanıcı bilgilerini kontrol ediniz", new ToastrOptions { Title = "Hatalı" });
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> LogOutAsync()
        {
            await HttpContext.SignOutAsync();
            _toastNotification.AddSuccessToastMessage("Çıkış işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction("Login");
        }
        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
