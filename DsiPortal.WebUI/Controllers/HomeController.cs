using ClosedXML.Excel;
using DsiPortal.Core.Entities;
using DsiPortal.Data;
using DsiPortal.Service.Concrete;
using DsiPortal.Service.IService;
using DsiPortal.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NToastNotify;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;


namespace DsiPortal.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IService<Managemention> _serviceManagemention;
        private readonly IService<RegionalManagers> _serviceRegionalManagers;
        private readonly IService<WorksConducteds> _serviceWorksConducteds;
        private readonly IService<Apps> _serviceApps;
        private readonly IService<Announcements> _serviceAnnouncements;
        private readonly IService<Forms> _serviceForms;
        private readonly IService<GuestFeeChart> _serviceGuestFeeChart;
        private readonly IService<FoodList> _serviceFoodList;
        private readonly IService<FoodPriceList> _serviceFoodPriceList;
        private readonly IService<BenefitLinks> _serviceBenefitLinks;
        private readonly IService<DepartmentManagers> _serviceDepartmentManagers;
        private readonly IService<Cameras> _serviceCameras;
        private readonly IToastNotification _toastNotification;
        private readonly IMenuofDay _menuofday;

        public HomeController(IService<Managemention> serviceManagemention, IService<RegionalManagers> serviceRegionalManagers, IService<WorksConducteds> serviceWorksConducteds, AppDbContext context, IService<Apps> serviceApps, IService<Announcements> serviceAnnouncementss, IService<Forms> serviceForms, IService<FoodList> serviceFoodList, IToastNotification toastNotification, IMenuofDay menuofday, IService<GuestFeeChart> serviceGuestFeeChart, IService<BenefitLinks> serviceBenefitLinks, IService<FoodPriceList> serviceFoodPriceList, IService<DepartmentManagers> serviceDepartmentManagers, IService<Cameras> serviceCameras)
        {
            _serviceManagemention = serviceManagemention;
            _serviceRegionalManagers = serviceRegionalManagers;
            _serviceWorksConducteds = serviceWorksConducteds;
            _serviceApps = serviceApps;
            _serviceAnnouncements = serviceAnnouncementss;
            _serviceForms = serviceForms;
            _serviceFoodList = serviceFoodList;
            _toastNotification = toastNotification;
            _menuofday = menuofday;
            _serviceGuestFeeChart = serviceGuestFeeChart;
            _serviceBenefitLinks = serviceBenefitLinks;
            _serviceFoodPriceList = serviceFoodPriceList;
            _serviceDepartmentManagers = serviceDepartmentManagers;
            _serviceCameras = serviceCameras;
        }

        [Route("anasayfa")]
        public IActionResult Index()
        {

            var regionalManagaer = _serviceRegionalManagers.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            var guest = _serviceGuestFeeChart.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            var foodList = _serviceFoodList.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            var foodPriceList = _serviceFoodPriceList.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            var apps = _serviceApps.GetQueryable().OrderBy(x => x.CreatedDate);
            var benefitLinks = _serviceBenefitLinks.GetQueryable().OrderBy(x => x.CreatedDate);
            var worksConducteds = _serviceWorksConducteds.GetQueryable().Where(x => x.IsMainImage == true).OrderByDescending(x => x.CreatedDate).Take(6);
            var oldworksConducteds = _serviceWorksConducteds.GetQueryable().Where(x => x.IsMainImage == true).OrderByDescending(x => x.CreatedDate).Skip(6).Take(6);
            var annocuments = _serviceAnnouncements.GetQueryable().OrderByDescending(x => x.CreatedDate).Take(10);
            var cameras = _serviceCameras.GetQueryable().OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.Apps = _serviceApps.GetQueryable().OrderBy(x => x.CreatedDate).Take(15).ToList();
            ViewBag.OrganizationalChart = _serviceManagemention.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            ViewBag.BenefitLinks = _serviceBenefitLinks.GetQueryable().OrderBy(x => x.CreatedDate).Take(10).ToList();
            ViewBag.Cameras = _serviceCameras.GetAll();
            var (eat1, eat2, eat3, eat4) = _menuofday.IListMenuofDay();
            var listMenuOfDayViewModel = new ListMenuOfDayViewModel();
            if (eat1 == null || eat2 == null || eat3 == null || eat4 == null)
            {
                ViewData["FoodList"] = "Güncel yemek listesi yükleyin";
                listMenuOfDayViewModel = new ListMenuOfDayViewModel()
                {
                    Eat1 = null,
                    Eat2 = null,
                    Eat3 = null,
                    Eat4 = null
                };
            }
            else
            {
                listMenuOfDayViewModel = new ListMenuOfDayViewModel()
                {
                    Eat1 = eat1,
                    Eat2 = eat2,
                    Eat3 = eat3,
                    Eat4 = eat4
                };
            }


            var model = new AllViewModel()
            {
                RegionalManager = regionalManagaer,
                WorksConducteds = worksConducteds.ToList(),
                Apps = apps.ToList(),
                Announcements = annocuments.ToList(),
                ListMenuOfDayViewModel = listMenuOfDayViewModel,
                GuestFeeChart = guest,
                FoodList = foodList,
                FoodPriceList = foodPriceList,
                OldWorksConducteds = oldworksConducteds.ToList(),
                BenefitLinks = benefitLinks.ToList(),
            };
            return View(model);
        }

        [Route("organizasyonsemasii")]
        public async Task<IActionResult> ViewImageOrganizationalChart()
        {
            var image = _serviceManagemention.GetQueryable()
        .OrderByDescending(x => x.CreatedDate)
        .FirstOrDefault();

            if (image == null) return NotFound();

            // image.FileType ".png" gibi geliyor
            var extension = image.FileType.StartsWith(".") ? image.FileType : "." + image.FileType;

            var contentType = extension.ToLower() switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

            // Tarayýcýda açýlmasý için filename parametresini kaldýr
            return File(image.Content, contentType);
        }

        public async Task<IActionResult> ViewImageApps(int id)
        {
            var image = await _serviceApps.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            return File(image.Image, image.FileType);
        }

        public async Task<IActionResult> ViewImageDepartmentManagers(int id)
        {
            var image = await _serviceDepartmentManagers.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            return File(image.Image, image.FileType);
        }

        public async Task<IActionResult> ViewImageBnefitLinks(int id)
        {
            var image = await _serviceBenefitLinks.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            return File(image.Image, image.FileType);
        }

        public async Task<IActionResult> ViewImageRegionalManager(int id)
        {
            //var image = _serviceRegionalManagers.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            var image = _serviceRegionalManagers.Find(id);
            if (image == null)
            {
                return NotFound();
            }

            return File(image.Image, image.FileType);
        }

        public async Task<IActionResult> ViewImageNews(int id)
        {
            var image = await _serviceWorksConducteds.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            return File(image.Images, image.FileType);
        }

        [Route("organizasyonsemasi")]
        public IActionResult ViewPdfOrganizationalChart()
        {
            var document = _serviceManagemention.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (document == null)
            {
                return NotFound();
            }

            // Content-Disposition header'ýný kaldýrarak tarayýcýnýn PDF'yi açmasýný saðlýyoruz
            Response.Headers.Add("Content-Disposition", "inline; filename=" + document.Name + ".pdf");

            return File(document.Content, "application/pdf");
        }

        [Route("misafirhaneucretcizelgesi")]
        public IActionResult ViewPdfGuestFeeChart()
        {
            var document = _serviceGuestFeeChart.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (document == null)
            {
                return NotFound();
            }

            // Content-Disposition header'ýný kaldýrarak tarayýcýnýn PDF'yi açmasýný saðlýyoruz
            Response.Headers.Add("Content-Disposition", "inline; filename=" + document.Name + ".pdf");

            return File(document.Content, "application/pdf");
        }

        public IActionResult ViewPdfFoodList()
        {
            var document = _serviceFoodList.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (document == null)
            {
                return NotFound();
            }

            // Content-Disposition header'ýný kaldýrarak tarayýcýnýn PDF'yi açmasýný saðlýyoruz
            Response.Headers.Add("Content-Disposition", "inline; filename=" + document.Name + ".pdf");

            return File(document.Content, "application/pdf");
        }

        public IActionResult ViewPdfFoodPriceList()
        {
            var document = _serviceFoodPriceList.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (document == null)
            {
                return NotFound();
            }

            // Content-Disposition header'ýný kaldýrarak tarayýcýnýn PDF'yi açmasýný saðlýyoruz
            Response.Headers.Add("Content-Disposition", "inline; filename=" + document.Name + ".pdf");

            return File(document.Content, "application/pdf");
        }

        public IActionResult ViewPdfForm(int id)
        {
            var document = _serviceForms.Find(id);
            if (document == null)
            {
                return NotFound();
            }

            // Content-Disposition header'ýný kaldýrarak tarayýcýnýn PDF'yi açmasýný saðlýyoruz
            Response.Headers.Add("Content-Disposition", "inline; filename=" + document.Name + ".pdf");

            return File(document.Content, "application/pdf");
        }

        [Route("duyurular/{id?}")]
        public IActionResult ViewPdfAnnocumentn(int id)
        {
            var document = _serviceAnnouncements.Find(id);
            if (document == null)
            {
                return NotFound();
            }

            // Content-Disposition header'ýný kaldýrarak tarayýcýnýn PDF'yi açmasýný saðlýyoruz
            Response.Headers.Add("Content-Disposition", "inline; filename=" + document.Name + ".pdf");

            return File(document.Content, "application/pdf");
        }

        public async Task<IActionResult> DownloadAnnouncement(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _serviceAnnouncements.FindAsync(id.Value);
            if (document == null)
            {
                return NotFound();
            }

            return File(document.Content, GetMimeType(document.FileType), document.Name + document.FileType);
        }

        public async Task<IActionResult> DownloadFoodList()
        {
            var document = await _serviceFoodList.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
            if (document == null)
            {
                return NotFound();
            }

            return File(document.Content, GetMimeType(document.FileType), document.Name + document.FileType);
        }

        public async Task<IActionResult> DownloadFoodPriceList()
        {
            var document = await _serviceFoodPriceList.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
            if (document == null)
            {
                return NotFound();
            }

            return File(document.Content, GetMimeType(document.FileType), document.Name + document.FileType);
        }
        public async Task<IActionResult> DownloadGuestFeeChart()
        {
            var document = await _serviceGuestFeeChart.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();
            if (document == null)
            {
                return NotFound();
            }

            return File(document.Content, GetMimeType(document.FileType), document.Name + document.FileType);
        }

        public async Task<IActionResult> DownloadForm(int? id)
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

        private string GetMimeType(string fileType)
        {
            switch (fileType.ToLower())
            {
                case ".pdf":
                    return "application/pdf";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".xls":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                default:
                    return "application/octet-stream";
            }
        }

        [Route("iletisim")]
        public IActionResult Contact()
        {
            ViewBag.BenefitLinks = _serviceBenefitLinks.GetQueryable().OrderBy(x => x.CreatedDate).Take(10).ToList();
            ViewBag.Apps = _serviceApps.GetQueryable().OrderBy(x => x.CreatedDate).Take(15).ToList();
            ViewBag.Cameras = _serviceCameras.GetQueryable().OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.OrganizationalChart = _serviceManagemention.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            return View();
        }

        [Route("tarihcemiz")]
        public IActionResult MyStory()
        {
            ViewBag.BenefitLinks = _serviceBenefitLinks.GetQueryable().OrderBy(x => x.CreatedDate).Take(10).ToList();
            ViewBag.Apps = _serviceApps.GetQueryable().OrderBy(x => x.CreatedDate).Take(15).ToList();
            ViewBag.Cameras = _serviceCameras.GetQueryable().OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.OrganizationalChart = _serviceManagemention.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            return View();
        }

        [Route("yonetim")]
        public IActionResult Management()
        {
            ViewBag.BenefitLinks = _serviceBenefitLinks.GetQueryable().OrderBy(x => x.CreatedDate).Take(10).ToList();
            ViewBag.Apps = _serviceApps.GetQueryable().OrderBy(x => x.CreatedDate).Take(15).ToList();
            ViewBag.Cameras = _serviceCameras.GetQueryable().OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.OrganizationalChart = _serviceManagemention.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            return View(_serviceDepartmentManagers.GetQueryable().OrderBy(x => x.CreatedDate).ToList());
        }

        [Route("formlar")]
        public IActionResult Forms()
        {
            ViewBag.BenefitLinks = _serviceBenefitLinks.GetQueryable().OrderBy(x => x.CreatedDate).Take(10).ToList();
            ViewBag.Apps = _serviceApps.GetQueryable().OrderBy(x => x.CreatedDate).Take(15).ToList();
            ViewBag.Cameras = _serviceCameras.GetQueryable().OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.OrganizationalChart = _serviceManagemention.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            var forms = _serviceForms.GetQueryable().GroupBy(x => x.Department).Select(x => new FormDepartmentsViewModel
            {
                DepartmentName = x.Key,
                Count = x.Count(),
                Forms = x.ToList()
            }).OrderBy(x => x.DepartmentName).ToList();
            return View(forms);
        }

        [Route("tumduyurular")]
        public async Task<IActionResult> AllAnnocuments()
        {
            ViewBag.BenefitLinks = _serviceBenefitLinks.GetQueryable().OrderBy(x => x.CreatedDate).Take(10).ToList();
            ViewBag.Apps = _serviceApps.GetQueryable().OrderBy(x => x.CreatedDate).Take(15).ToList();
            ViewBag.Cameras = _serviceCameras.GetQueryable().OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.OrganizationalChart = _serviceManagemention.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            var annocuments = await _serviceAnnouncements.GetQueryable().OrderByDescending(x => x.CreatedDate).ToListAsync();
            return View(annocuments);
        }

        [HttpGet]
        [Route("haber/{WorkingName}")]
        public async Task<IActionResult> NewsDetailAsync(string WorkingName)
        {
            ViewBag.BenefitLinks = _serviceBenefitLinks.GetQueryable().OrderBy(x => x.CreatedDate).Take(10).ToList();
            ViewBag.Apps = _serviceApps.GetQueryable().OrderBy(x => x.CreatedDate).Take(15).ToList();
            ViewBag.Cameras = _serviceCameras.GetQueryable().OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.OrganizationalChart = _serviceManagemention.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            var result = await _serviceWorksConducteds.GetQueryable().Where(x => x.IsMainImage == true && x.WorkingName == WorkingName).FirstOrDefaultAsync();
            var works = await _serviceWorksConducteds.GetQueryable().Where(x => x.WorkingName == WorkingName).ToListAsync();
            if (result == null || works == null)
            {
                return NotFound();
            }
            ViewData["Id"] = result.Id;
            ViewData["Description"] = result.Description;
            ViewData["WorkingName"] = result.WorkingName;
            ViewData["CreateDate"] = result.CreatedDate.ToShortDateString();
            return View(works);
        }

        [Route("tumhaberler")]
        public async Task<ActionResult> AllNews()
        {
            ViewBag.BenefitLinks = _serviceBenefitLinks.GetQueryable().OrderBy(x => x.CreatedDate).Take(10).ToList();
            ViewBag.Apps = _serviceApps.GetQueryable().OrderBy(x => x.CreatedDate).Take(15).ToList();
            ViewBag.Cameras = _serviceCameras.GetQueryable().OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.OrganizationalChart = _serviceManagemention.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            // var total = _serviceWorksConducteds.GetAll().ToList().Count();
            var items = await _serviceWorksConducteds.GetQueryable().Where(x => x.IsMainImage == true)
                .OrderByDescending(x => x.CreatedDate)
                .Take(5).ToListAsync();
            ViewBag.TotalCount = _serviceWorksConducteds.GetQueryable().Where(x => x.IsMainImage == true).ToList().Count();

            return View(items);
        }

        public async Task<ActionResult> GetNews(int skip = 0, int take = 5)
        {
            ViewBag.BenefitLinks = _serviceBenefitLinks.GetQueryable().OrderBy(x => x.CreatedDate).Take(10).ToList();
            ViewBag.Apps = _serviceApps.GetQueryable().OrderBy(x => x.CreatedDate).Take(15).ToList();
            ViewBag.Cameras = _serviceCameras.GetQueryable().OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.OrganizationalChart = _serviceManagemention.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            // var total = _serviceWorksConducteds.GetAll().ToList().Count();
            var items = await _serviceWorksConducteds.GetQueryable().Where(x => x.IsMainImage == true)
                .OrderByDescending(x => x.CreatedDate)
                .Skip(skip)
                .Take(take).ToListAsync();

            return PartialView("_NewsPartial",items);
        }

        [Route("rehberimiz")]
        public IActionResult MyGuide()
        {
            ViewBag.BenefitLinks = _serviceBenefitLinks.GetQueryable().OrderBy(x => x.CreatedDate).Take(10).ToList();
            ViewBag.Apps = _serviceApps.GetQueryable().OrderBy(x => x.CreatedDate).Take(15).ToList();
            ViewBag.Cameras = _serviceCameras.GetQueryable().OrderByDescending(x => x.CreatedDate).ToList();
            ViewBag.OrganizationalChart = _serviceManagemention.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            var list = new List<MyGuideViewModel>();
            using (var workbook = new XLWorkbook("\\\\10.124.1.38\\ftp24\\Bilgi Teknolojileri Þube Müdürlüðü\\Dahili Rehber\\dsi24bolge_dahili.xlsx"))
            {
                var worksheet = workbook.Worksheet(1); // ilk sayfa
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // ilk satýr baþlýk olsun

                foreach (var row in rows)
                {
                    string isDeleted = row.Cell(7).GetString();
                    if (isDeleted == "False")
                    {
                        var myGuide = new MyGuideViewModel
                        {
                            AdSoyad = row.Cell(2).GetString(),
                            Unvan = row.Cell(3).GetString(),
                            Sube = row.Cell(4).GetString(),
                            DahiliNo = row.Cell(5).GetString(),
                            CepNo = row.Cell(6).GetString(),
                        };
                        list.Add(myGuide);
                    }
                }
            }
            return View(list);
        }

    }
}
