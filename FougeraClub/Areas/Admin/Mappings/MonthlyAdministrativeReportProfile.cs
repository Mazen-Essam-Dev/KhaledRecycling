using AutoMapper;
using Domain.DTOs.Admin.AdministrativeReportQuarterlyAnnual;
using Domain.Entities.MonthlyAdministrativeReport;
using FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual;
using FougeraClub.Areas.Admin.ViewModels.MonthlyAdministrativeReport;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class MonthlyAdministrativeReportProfile : Profile
    {
        public MonthlyAdministrativeReportProfile()
        {
            // Parent
            CreateMap<MonthlyAdministrativeReportVM, MonthlyAdministrativeReport>()
                .ReverseMap();

            // Child
            CreateMap<MonthlyAdministrativeReportDetailVM, MonthlyAdministrativeReportDetail>()
                .ReverseMap();

            // Parent
            CreateMap<MonthsOfYearsAnnualyVM, MonthsOfYearsAnnualyDTO>()
                .ReverseMap();

            // Child
            CreateMap<MonthsOfYearsAnnualyWithDetailsVM, MonthsOfYearsAnnualyWithDetailsDTO>()
                .ReverseMap();

            // Parent of Parent
            CreateMap<MonthsOfYearsAnnualyActivitiesAndAdministrative, MonthsOfYearsAnnualyActivitiesAndAdministrativeDTO>()
                .ReverseMap();
            
        }
    }
}