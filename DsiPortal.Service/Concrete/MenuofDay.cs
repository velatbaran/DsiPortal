using DsiPortal.Core.Entities;
using DsiPortal.Service.IService;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Spreadsheet;
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
        //private DateTime? GetDateFromCell(string? cellValue)
        //{
        //    if (string.IsNullOrWhiteSpace(cellValue))
        //        return null;

        //    string dateText = cellValue;

        //    if (cellValue.Contains("-"))
        //    {
        //        dateText = cellValue.Split('-')[0].Trim();
        //    }

        //    if (DateTime.TryParseExact(
        //        dateText,
        //        "dd.MM.yyyy",
        //        CultureInfo.InvariantCulture,
        //        DateTimeStyles.None,
        //        out DateTime result))
        //    {
        //        return result.Date;
        //    }

        //    return null;
        //}

        public (string?, string?, string?, string?, string?) IListMenuofDay()
        {
            var excelEntity = _serviceFoodList.GetQueryable().OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (excelEntity == null || excelEntity.Content == null)
            {
                //   _toastNotification.AddErrorToastMessage("Yemek listesi bulunamadı", new ToastrOptions { Title = "Hata" });
                return (null, null, null, null, null);
            }

            using var stream = new MemoryStream(excelEntity.Content);

            IWorkbook workbook;
            if (Path.GetExtension(excelEntity.Name).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                workbook = new HSSFWorkbook(stream); // eski format
            else
                workbook = new XSSFWorkbook(stream); // yeni format

            var sheet = workbook.GetSheetAt(0); // ilk sayfa

            string? eat1 = null, eat2 = null, eat3 = null, eat4 = null, eat5 = null;
            var name = excelEntity?.Name;
            if (string.IsNullOrEmpty(name) ||
                (!name.ToUpper(new CultureInfo("tr-TR")).Contains("RAMAZAN") &&
                 !name.ToUpper(new CultureInfo("tr-TR")).Contains("İFTAR")))
            {
                for (int row = 1; row <= sheet.LastRowNum; row++) // 0: başlık
                {
                    var currentRow = sheet.GetRow(row);
                    if (currentRow == null) continue;
                    var dateText = currentRow.GetCell(0)?.ToString();
                    var days = currentRow.GetCell(1)?.ToString();
                    if (!string.IsNullOrEmpty(dateText) && dateText == DateTime.Now.ToString("dd-MMM-yyyy", new CultureInfo("tr-TR")))
                    {
                        eat1 = currentRow.GetCell(2)?.ToString();
                        eat2 = currentRow.GetCell(3)?.ToString();
                        eat3 = currentRow.GetCell(4)?.ToString();
                        eat4 = currentRow.GetCell(5)?.ToString();
                        break;
                    }
                }

                if (string.IsNullOrEmpty(eat1) || string.IsNullOrEmpty(eat2) || string.IsNullOrEmpty(eat3) || string.IsNullOrEmpty(eat4))
                {
                    //_toastNotification.AddWarningToastMessage("Bugün için yemek listesi bulunamadı", new ToastrOptions { Title = "Uyarı" });
                    return (null, null, null, null, null);
                }
                return (eat1, eat2, eat3, eat4, eat5);
            }
            else
            {
                // ================= RAMAZAN =================
                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    var currentRow = sheet.GetRow(row);
                    if (currentRow == null) continue;

                    DateTime? rowDate = null;
                    var dateCell = currentRow.GetCell(0); // TARİH
                    var days = currentRow.GetCell(1)?.ToString();
                    if (dateCell != null)
                    {
                        // Excel Date (Numeric)
                        if (dateCell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(dateCell))
                        {
                            rowDate = DateTime.FromOADate(dateCell.NumericCellValue).Date;
                        }
                        // String Date (örn: 18.02.2026)
                        else if (dateCell.CellType == CellType.String &&
                                 DateTime.TryParseExact(
                                     dateCell.StringCellValue.Trim(),
                                     "dd.MM.yyyy",
                                     new CultureInfo("tr-TR"),
                                     DateTimeStyles.None,
                                     out DateTime parsedDate))
                        {
                            rowDate = parsedDate.Date;
                        }
                    }

                    // 👉 BUGÜNÜN TARİHİ Mİ?
                    if (rowDate.HasValue && rowDate.Value == DateTime.Today)
                    {
                        eat1 = currentRow.GetCell(2)?.ToString();
                        eat2 = currentRow.GetCell(3)?.ToString();
                        eat3 = currentRow.GetCell(4)?.ToString();
                        eat4 = currentRow.GetCell(5)?.ToString();
                        eat5 = currentRow.GetCell(6)?.ToString();
                        break;
                    }
                }

                if (string.IsNullOrEmpty(eat1) ||
                    string.IsNullOrEmpty(eat2) ||
                    string.IsNullOrEmpty(eat3) ||
                    string.IsNullOrEmpty(eat4) )
                {
                    return (null, null, null, null, null);

        }
                return (eat1, eat2, eat3, eat4, eat5);

            }

        }
    }
}