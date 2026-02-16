using DsiPortal.Core.Entities;
using DsiPortal.Service.IService;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiPortal.Service.Concrete
{

    public class MenuofDay : IMenuofDay
    {
        private readonly IService<FoodList> _serviceFoodList;
        //  private readonly IToastNotification _toastNotification;

        public MenuofDay(IService<FoodList> serviceFoodList)
        {
            _serviceFoodList = serviceFoodList;
            // _toastNotification = toastNotification;
        }


        /// <summary>
        /// Excel hücresinden "11.03.2024 - PAZARTESİ" gibi değerden sadece tarihi alır
        /// </summary>
        private DateTime? GetDateFromCell(string? cellValue)
        {
            if (string.IsNullOrWhiteSpace(cellValue))
                return null;

            string dateText = cellValue;

            // Ramazan Excel: "11.03.2024 - PAZARTESİ"
            if (cellValue.Contains("-"))
            {
                dateText = cellValue.Split('-')[0].Trim();
            }

            // Normal Excel: "11.03.2024"
            if (DateTime.TryParseExact(
                dateText,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime result))
            {
                return result.Date;
            }

            return null;
        }

        public (string?, string?, string?, string?, string?, string?) IListMenuofDay()
        {
            var excelEntity = _serviceFoodList.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (excelEntity == null || excelEntity.Content == null)
            {
                //   _toastNotification.AddErrorToastMessage("Yemek listesi bulunamadı", new ToastrOptions { Title = "Hata" });
                return (null, null, null, null, null, null);
            }

            using var stream = new MemoryStream(excelEntity.Content);

            IWorkbook workbook;
            if (Path.GetExtension(excelEntity.Name).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                workbook = new HSSFWorkbook(stream); // eski format
            else
                workbook = new XSSFWorkbook(stream); // yeni format

            var sheet = workbook.GetSheetAt(0); // ilk sayfa

            string? eat1 = null, eat2 = null, eat3 = null, eat4 = null, eat5 = null, eat6 = null;

            if (!excelEntity.Name.ToUpper().Contains("RAMAZAN"))
            {

                for (int row = 1; row <= sheet.LastRowNum; row++) // 0: başlık
                {
                    var currentRow = sheet.GetRow(row); 
                    if (currentRow == null) continue;
                    var _datetime = currentRow.GetCell(0)?.ToString();
                    var _days = currentRow.GetCell(1)?.ToString(); 
                    var _eat1 = currentRow.GetCell(2)?.ToString(); 
                    var _eat2 = currentRow.GetCell(3)?.ToString(); 
                    var _eat3 = currentRow.GetCell(4)?.ToString();
                    var _eat4 = currentRow.GetCell(5)?.ToString();
                    if (_datetime.ToString() == DateTime.Now.ToString("dd-MMM-yyyy", new CultureInfo("tr-TR"))) 
                    { 
                        eat1 = _eat1; eat2 = _eat2; eat3 = _eat3; eat4 = _eat4;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(eat1) || string.IsNullOrEmpty(eat2) || string.IsNullOrEmpty(eat3) || string.IsNullOrEmpty(eat4))
                {
                    //_toastNotification.AddWarningToastMessage("Bugün için yemek listesi bulunamadı", new ToastrOptions { Title = "Uyarı" });
                    return (null, null, null, null, null, null);
                }
                return (eat1, eat2, eat3, eat4, eat5, eat6);
            }
            // ================= RAMAZAN =================
            for (int row = 1; row <= sheet.LastRowNum; row++)
            {
                var currentRow = sheet.GetRow(row);
                if (currentRow == null) continue;

                var dateText = currentRow.GetCell(0)?.ToString();
                var excelDate = GetDateFromCell(dateText);

                if (excelDate.HasValue && excelDate.Value == DateTime.Today)
                {
                    eat1 = currentRow.GetCell(1)?.ToString();
                    eat2 = currentRow.GetCell(2)?.ToString();
                    eat3 = currentRow.GetCell(3)?.ToString();
                    eat4 = currentRow.GetCell(4)?.ToString();
                    eat5 = currentRow.GetCell(5)?.ToString();
                    eat6 = currentRow.GetCell(6)?.ToString();
                    break;
                }
            }

            if (string.IsNullOrEmpty(eat1) || string.IsNullOrEmpty(eat2) || string.IsNullOrEmpty(eat3) || string.IsNullOrEmpty(eat5) || string.IsNullOrEmpty(eat4) || string.IsNullOrEmpty(eat6))
            {
                //_toastNotification.AddWarningToastMessage("Bugün için yemek listesi bulunamadı", new ToastrOptions { Title = "Uyarı" });
                return (null, null, null, null, null, null);
            }
            return (eat1, eat2, eat3, eat4, eat5, eat6);

        }


    }

}
