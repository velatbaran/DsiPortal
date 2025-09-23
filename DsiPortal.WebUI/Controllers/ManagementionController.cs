using DsiPortal.Core.Entities;
using DsiPortal.Service.IService;
using DsiPortal.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DsiPortal.WebUI.Controllers
{
    [Authorize]
    public class ManagementionController : Controller
    {
        private readonly IService<Managemention> _serviceManagemention;

        public ManagementionController(IService<Managemention> serviceManagemention)
        {
            _serviceManagemention = serviceManagemention;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _serviceManagemention.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ManagementionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await viewModel.File.CopyToAsync(memoryStream);

                    var document = new Managemention
                    {
                        Name = viewModel.Name,
                        FileType = Path.GetExtension(viewModel.File.FileName),
                        Created = HttpContext.User.FindFirst("Username").Value,
                        CreatedDate = DateTime.Now,
                        Content = memoryStream.ToArray()
                    };

                    _serviceManagemention.Add(document);
                    await _serviceManagemention.SaveChangesAsync();
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

            var document = await _serviceManagemention.FindAsync(id.Value);
            if (document == null)
            {
                return NotFound();
            }

            return File(document.Content, GetMimeType(document.FileType), document.Name + document.FileType);
        }
        public IActionResult ViewPdf(int id)
        {
            var document = _serviceManagemention.Find(id);
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
            var announcements = await _serviceManagemention.FindAsync(id);
            if (announcements != null)
            {
                _serviceManagemention.Delete(announcements);
            }

            await _serviceManagemention.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
