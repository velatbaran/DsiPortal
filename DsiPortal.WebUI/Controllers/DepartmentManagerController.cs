using DsiPortal.Core.Entities;
using DsiPortal.Service.IService;
using DsiPortal.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NToastNotify;
using System.IO;

namespace DsiPortal.WebUI.Controllers
{
    [Authorize]
    public class DepartmentManagerController : Controller
    {
        private readonly IService<DepartmentManagers> _serviceDepartmentManagers;
        private readonly IService<Titles> _serviceTitles;
        private readonly IToastNotification _toastNotification;
        public DepartmentManagerController(IService<DepartmentManagers> serviceDepartmentManagers, IToastNotification toastNotification, IService<Titles> serviceTitles)
        {
            _serviceDepartmentManagers = serviceDepartmentManagers;
            _toastNotification = toastNotification;
            _serviceTitles = serviceTitles;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _serviceDepartmentManagers.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Unvanlar = _serviceTitles.GetAll().ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentManagerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                DepartmentManagers manager = new DepartmentManagers();
                if (viewModel.Image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await viewModel.Image.CopyToAsync(memoryStream);
                        manager = new DepartmentManagers
                        {
                            Name = viewModel.Name,
                            Title = viewModel.Title,
                            Eposta = viewModel.Eposta,
                            Phone = viewModel.Phone,
                            FileType = viewModel.Image.ContentType,
                            Created = HttpContext.User.FindFirst("Username").Value,
                            CreatedDate = DateTime.Now,
                            Image = memoryStream.ToArray()
                        };
                    }
                }
                else
                {
                    manager = new DepartmentManagers
                    {
                        Name = viewModel.Name,
                        Title = viewModel.Title,
                        Eposta = viewModel.Eposta,
                        Phone = viewModel.Phone,
                        FileType = null,
                        Created = HttpContext.User.FindFirst("Username").Value,
                        CreatedDate = DateTime.Now,
                        Image = null
                    };
                }
                _serviceDepartmentManagers.Add(manager);
                await _serviceDepartmentManagers.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Kayıt işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Unvanlar = _serviceTitles.GetAll().ToList();
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Unvanlar = _serviceTitles.GetAll().ToList();

            var serviceDepartmentManagers = await _serviceDepartmentManagers.FindAsync(id);
            if (serviceDepartmentManagers == null)
            {
                return NotFound();
            }
            var departmentManager = new DepartmentManagerEditViewModel
            {
                Id = serviceDepartmentManagers.Id,
                Name = serviceDepartmentManagers.Name,
                Title = serviceDepartmentManagers.Title,
                Eposta = serviceDepartmentManagers.Eposta,
                Phone = serviceDepartmentManagers.Phone,
                Image = null
            };
            return View(departmentManager);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(DepartmentManagerEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var departmentManager = await _serviceDepartmentManagers.FindAsync(viewModel.Id);
                    if (viewModel.Image == null)
                    {
                        departmentManager.Name = viewModel.Name;
                        departmentManager.Title = viewModel.Title;
                        departmentManager.Eposta = viewModel.Eposta;
                        departmentManager.Phone = viewModel.Phone;
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await viewModel.Image.CopyToAsync(memoryStream);
                            departmentManager.Name = viewModel.Name;
                            departmentManager.Title = viewModel.Title;
                            departmentManager.Eposta = viewModel.Eposta;
                            departmentManager.Phone = viewModel.Phone;
                            departmentManager.FileType = viewModel.Image.ContentType;
                            departmentManager.Image = memoryStream.ToArray();
                        }
                    }
                    _serviceDepartmentManagers.Update(departmentManager);
                    await _serviceDepartmentManagers.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Güncelleme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    _toastNotification.AddErrorToastMessage("Bilgileriniz güncellenirken hata", new ToastrOptions { Title = "Hata" });
                }
            }
            ViewBag.Unvanlar = _serviceTitles.GetAll().ToList();
            return View(viewModel);
        }

        public async Task<IActionResult> ViewImage(int id)
        {
            var image = await _serviceDepartmentManagers.FindAsync(id);
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
            var serviceDepartmentManagers = await _serviceDepartmentManagers.FindAsync(id);
            if (serviceDepartmentManagers != null)
            {
                _serviceDepartmentManagers.Delete(serviceDepartmentManagers);
            }

            await _serviceDepartmentManagers.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Silme işlemi başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction(nameof(Index));
        }
    }
}
