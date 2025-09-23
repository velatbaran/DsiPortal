
using DsiPortal.Core.Entities;
using DsiPortal.Service.IService;
using DsiPortal.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;

namespace Dsi24Portal.WebUI.Controllers
{
    [Authorize]
    [Authorize(Policy = "AdminPolicy")]
    public class FormController : Controller
    {
        private readonly IService<Forms> _serviceForms;

        public FormController(IService<Forms> serviceForms)
        {
            _serviceForms = serviceForms;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _serviceForms.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Birimler = new List<string>
            {
                "HAVZA YÖNETİMİ,İZLEME VE TAHSİSLER ŞUBE MÜDÜRLÜĞÜ",
"SULAMA ŞUBE MÜDÜRLÜĞÜ",
"STRATEJİ GELİŞTİRME ŞUBE MÜDÜRLÜĞÜ",
"BARAJLAR VE HES ŞUBE MÜDÜRLÜĞÜ",
"KALİTE KONTROL VE LABORATUVAR ŞUBE MÜDÜRLÜĞÜ",
"JEOTEKNİK HİZMETLER ŞUBE MÜDÜRLÜĞÜ",
"YAS ŞUBE MÜDÜRLÜĞÜ",
"TAŞKIN KONTROL ŞUBE MÜDÜRLÜĞÜ",
"MAKİNA İMALAT VE DONATIM ŞUBE MÜDÜRLÜĞÜ",
"PERSONEL ŞUBE MÜDÜRLÜĞÜ",
"HUKUK İŞLERİ ŞUBE MÜDÜRLÜĞÜ",
"EMLAK VE KAMULAŞTIRMA ŞUBE MÜDÜRLÜĞÜ",
"AT VE TİGH ŞUBE MÜDÜRLÜĞÜ",
"İŞLETME VE BAKIM ŞUBE MÜDÜRLÜĞÜ",
"BİLGİ TEKNOLOJİLERİ ŞUBE MÜDÜRLÜĞÜ",
"DESTEK HİZMETLERİ ŞUBE MÜDÜRLÜĞÜ",
"ELEKTROMEKANİK ŞUBE MÜDÜRLÜĞÜ",
"MUHASEBE ŞUBE MÜDÜRLÜĞÜ",
"241. ŞUBE MÜDÜRLÜĞÜ",
"242. ŞUBE MÜDÜRLÜĞÜ",
"243. ŞUBE MÜDÜRLÜĞÜ",
            };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FormViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await viewModel.File.CopyToAsync(memoryStream);

                    var document = new Forms
                    {
                        Department = viewModel.Department,
                        Name = viewModel.Name,
                        FileType = Path.GetExtension(viewModel.File.FileName),
                        Created = HttpContext.User.FindFirst("Username").Value,
                        CreatedDate = DateTime.Now,
                        Content = memoryStream.ToArray()
                    };

                    _serviceForms.Add(document);
                    await _serviceForms.SaveChangesAsync();
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

            var document = await _serviceForms.FindAsync(id.Value);
            if (document == null)
            {
                return NotFound();
            }

            return File(document.Content, GetMimeType(document.FileType), document.Name + document.FileType);
        }
        public IActionResult ViewPdf(int id)
        {
            var document = _serviceForms.Find(id);
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
            var announcements = await _serviceForms.FindAsync(id);
            if (announcements != null)
            {
                _serviceForms.Delete(announcements);
            }

            await _serviceForms.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
