using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface IExcelReportService<T> where T : class
    {
        Task<(bool,string?)> ExcelReportAr_Async(List<T> listData, List<string> ListTitles);
        Task<(bool, string?)> ExcelReportEn_Async(List<T> listData, List<string> ListTitles);
    }

}
