using Application.Helpers;
using Application.Interfaces.Admin;
using ClosedXML.Excel;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System;

namespace Application.Services.Admin
{
    public class ExcelReportService<T> : IExcelReportService<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private static IWebHostEnvironment? _env;
        private readonly string folderAr = "ArDailyReport";
        private readonly string folderEn = "EnDailyReport";
        private readonly string TempFolder = "TempFolder";
        private readonly string folderSource = "Source";

        public ExcelReportService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public async Task<(bool, string?)> ExcelReportAr_Async(List<T> listData, List<string> ListTitles)
        {
            // as Arabic Reverse List
            listData.Reverse();
            ListTitles.Reverse();
            var bigList = new List<List<string>> { ListTitles };
            var folder = "";
            try
            {
                if (_env == null)
                    throw new InvalidOperationException("FileHelper is not configured. Call Configure() first.");

                folder = Path.Combine(_env.WebRootPath, "ReportExcel");

                bool folderExistsAr = Directory.Exists(folder+"/"+folderAr);
                if (!folderExistsAr)
                    Directory.CreateDirectory(folder + "/"+folderAr);

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
                    if (sheets == null || sheets.Count == 0 || !sheets.Any(x => x.Name == "KhaledTeamRecyclingReport"))
                    {
                        //or create a new sheet
                        worksheet = workbook_Ar.AddWorksheet("KhaledTeamRecyclingReport");
                    }
                    else
                    {
                        // Get the required sheet (or add a new sheet if it doesn't exist)
                        worksheet = workbook_Ar.Worksheets.Worksheet("KhaledTeamRecyclingReport");
                    }

                    //// Write to specific cells
                    //worksheet.Cell("D9").Value = start;
                    //worksheet.Cell("F9").Value = end;
                    //worksheet.Cell("D12").Value = credits;

                    worksheet.Cell("B14").InsertData(bigList);
                    // Assign data to the sheet starting from D15
                    worksheet.Cell("B15").InsertData(listData);

                    // Save changes as a new file
                    workbook_Ar.SaveAs(newFilePath_Ar);
                }
                var newFilePath = "/ReportExcel" + newFilePath_Ar.Split("ReportExcel")[1].Replace("\\","/");
                return (true, newFilePath);
            }
            catch (Exception ex)
            {
                return (false,"");
            }
        }



        public async Task<(bool, string?)> ExcelReportEn_Async(List<T> listData, List<string> ListTitles)
        {
            throw new NotImplementedException();
        }

    }

}
