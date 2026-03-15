using Application.Helpers;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Hosting;

namespace Application.Services.Admin
{
    public static class ExcelStaticReport
    {
        private static IWebHostEnvironment? _env;
        private static string folderAr = "ArDailyReport";
        private static string folderEn = "EnDailyReport";
        private static string TempFolder = "TempFolder";
        private static string folderSource = "Source";

        public static void ConfigureExcel( IWebHostEnvironment env)
        {
            _env = env;
        }

        public static (bool, string?) ExcelReportAr_withSave_<T>(List<T> listData, List<string> ListTitles, int stop = 0, string lang = "ar")
        {
            // as Arabic Reverse List
            //listData.Rev();
            //ListTitles.Reverse();

            var bigList = new List<List<string>> { ListTitles };
            var folder = "";
            try
            {
                if (_env == null)
                    throw new InvalidOperationException("ExcelStaticReport is not configured. Call Configure() first.");

                folder = Path.Combine(_env.WebRootPath, "ReportExcel");

                bool folderExistsAr = Directory.Exists(folder + "/" + folderAr);
                if (!folderExistsAr)
                    Directory.CreateDirectory(folder + "/" + folderAr);

                // Original file path old excel Ar
                string originalFilePath_Ar = folder + "/" + folderSource + "/" + "ExcelFormula" + ".xlsx";

                // New version path Ar
                string newFilePath_Ar = folder + "/" + folderAr + "/" + "DailyReportAr" + AppDubaiTime.Now.ToString("d-HHmmss") + ".xlsx";

                // Open Original file
                using (var workbook_Ar = new XLWorkbook(originalFilePath_Ar))
                {
                    var sheets = workbook_Ar.Worksheets.ToList();
                    if (sheets?.Any(x => x.Name == "Sheet1") ?? false)
                        workbook_Ar.Worksheets.Delete("Sheet1");

                    IXLWorksheet? worksheet;
                    IXLRange? rangeData;
                    if (sheets == null || sheets.Count == 0 || !sheets.Any(x => x.Name == "FougeraClubReport"))
                    {
                        //or create a new sheet
                        worksheet = workbook_Ar.AddWorksheet("FougeraClubReport");
                    }
                    else
                    {
                        // Get the required sheet (or add a new sheet if it doesn't exist)
                        worksheet = workbook_Ar.Worksheets.Worksheet("FougeraClubReport");
                    }
                    if (lang == "ar")
                    {
                        // Use RightToLeft for the entire sheet (supports Arabic)
                        worksheet.RightToLeft = true;
                    }
                    else
                    {
                        worksheet.RightToLeft = false;
                    }

                    //// Write to specific cells
                    //worksheet.Cell("D9").Value = start;
                    //worksheet.Cell("F9").Value = end;
                    //worksheet.Cell("D12").Value = credits;

                    // tHead data for first Raw
                    worksheet.Cell("A2").InsertData(bigList);
                    var range_Titles = worksheet.Cell("A2").InsertData(bigList);

                    // Color the background for each Range
                    range_Titles.Style.Fill.BackgroundColor = XLColor.LightBlue;

                    // Coloring the Text + Bold
                    range_Titles.Style.Font.FontColor = XLColor.DarkBlue;
                    range_Titles.Style.Font.Bold = true;

                    // The title has horizontal centering
                    range_Titles.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    // The title has Vertical centering
                    range_Titles.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    if (stop > 0)
                    {
                        // Select columns to stop only
                        var limitedData = listData
                            .Select(item =>
                            {
                                var props = typeof(T).GetProperties();
                                var values = props.Take(stop).Select(p => p.GetValue(item)?.ToString() ?? "");
                                return values.ToList();
                            })
                            .ToList();

                        // Assign data to the sheet starting from A3
                        rangeData = worksheet.Cell("A3").InsertData(limitedData);

                    }
                    else
                    {
                        // Assign data to the sheet starting from A3
                        rangeData = worksheet.Cell("A3").InsertData(listData);
                    }

                    //for horizontal centering data
                    rangeData.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    //for Vertical centering data
                    rangeData.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    //or AutoFit, depending on the data, adjust all columns
                    worksheet.Columns().AdjustToContents();


                    // Save changes as a new file
                    workbook_Ar.SaveAs(newFilePath_Ar);
                }
                var newFilePath = "/ReportExcel" + newFilePath_Ar.Split("ReportExcel")[1].Replace("\\", "/");
                return (true, newFilePath);
            }
            catch (Exception ex)
            {
                return (false, "");
            }
        }

        public static (bool, byte[]?) ExcelReportArEn_<T>(List<T> listData, List<string> ListTitles, int stop = 0, string lang = "ar",string? st20 =null)
        {
            // as Arabic Reverse List
            //listData.Rev();
            //ListTitles.Reverse();

            var bigList = new List<List<string>> { ListTitles };
            var folder = "";
            try
            {
                if (_env == null)
                    throw new InvalidOperationException("ExcelStaticReport is not configured. Call Configure() first.");

                folder = Path.Combine(_env.WebRootPath, "ReportExcel");

                bool folderExistsAr = Directory.Exists(folder + "/" + folderAr);
                if (!folderExistsAr)
                    Directory.CreateDirectory(folder + "/" + folderAr);

                // Original file path old excel Ar
                string originalFilePath_Ar = folder + "/" + folderSource + "/" + "ExcelFormula" + ".xlsx";

                // New version path Ar
                string newFilePath_Ar = folder + "/" + folderAr + "/" + "DailyReportAr" + AppDubaiTime.Now.ToString("d-HHmmss") + ".xlsx";

                // Open Original file path
                using (var workbook_Ar = new XLWorkbook(originalFilePath_Ar))
                {
                    var sheets = workbook_Ar.Worksheets.ToList();
                    if (sheets?.Any(x => x.Name == "Sheet1") ?? false)
                        workbook_Ar.Worksheets.Delete("Sheet1");

                    IXLWorksheet? worksheet;
                    IXLRange? rangeData;
                    if (sheets == null || sheets.Count == 0 || !sheets.Any(x => x.Name == "FougeraClubReport"))
                    {
                        //or create a new sheet
                        worksheet = workbook_Ar.AddWorksheet("FougeraClubReport");
                    }
                    else
                    {
                        // Get the required sheet (or add a new sheet if it doesn't exist)
                        worksheet = workbook_Ar.Worksheets.Worksheet("FougeraClubReport");
                    }
                    if (lang == "ar")
                    {
                        // Use RightToLeft for the entire sheet (supports Arabic)
                        worksheet.RightToLeft = true;
                    }
                    else
                    {
                        worksheet.RightToLeft = false;
                    }
                    var startCellTitles = "A1";
                    var startCellData = "A2";

                    if (!string.IsNullOrEmpty(st20)) {
                        // Merge cells from A1:X1 in One Cell
                        var mergedRange = worksheet.Range("A1:X1").Merge();
                        mergedRange.Value = st20 ?? "";
                        if (st20?.Length<60)
                        {
                            mergedRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }
                        mergedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        mergedRange.Style.Font.Bold = true;
                        mergedRange.Style.Font.FontSize = 16;
                        mergedRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                        startCellTitles = "A2";
                        startCellData = "A3";
                    }


                    //// Write to specific cells
                    //worksheet.Cell("D9").Value = start;
                    //worksheet.Cell("F9").Value = end;

                    // tHead data for first Raw
                    //worksheet.Cell("A2").InsertData(bigList);
                    //var range_Titles = worksheet.Cell("A2").InsertData(bigList);
                    worksheet.Cell(startCellTitles).InsertData(bigList);
                    var range_Titles = worksheet.Cell(startCellTitles).InsertData(bigList);

                    // Color the background for each Range
                    range_Titles.Style.Fill.BackgroundColor = XLColor.LightBlue;

                    // Coloring the Text + Bold
                    range_Titles.Style.Font.FontColor = XLColor.DarkBlue;
                    range_Titles.Style.Font.Bold = true;

                    // The title has horizontal centering
                    range_Titles.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    // The title has Vertically centering
                    range_Titles.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    if (stop > 0)
                    {
                        // Select columns to stop only
                        var limitedData = listData
                            .Select(item =>
                            {
                                var props = typeof(T).GetProperties();
                                var values = props.Take(stop).Select(p => p.GetValue(item)?.ToString() ?? "");
                                return values.ToList();
                            })
                            .ToList();

                        // Assign data to the sheet starting from A3
                        rangeData = worksheet.Cell(startCellData).InsertData(limitedData);

                    }
                    else
                    {
                        // Assign data to the sheet starting from A3
                        rangeData = worksheet.Cell(startCellData).InsertData(listData);
                    }

                    // The title has horizontal centering
                    rangeData.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    // The title has Vertically centering
                    rangeData.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    //or AutoFit, depending on the data, adjust all columns
                    worksheet.Columns().AdjustToContents();



                    // Save to MemoryStream instead of a file
                    using (var stream = new MemoryStream())
                    {
                        workbook_Ar.SaveAs(stream);
                        return (true, stream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, null);
            }
        }

    }

}
