using AutoMapper;
using Domain.DTOs.Admin.QuartersReport;
using Domain.Entities.QuartersReport;
using FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class QuartersReportProfile : Profile
    {
        public QuartersReportProfile()
        {
            CreateMap<QuartersReport, QuartersReportAdminstrativeDetailsVM>().ReverseMap();
            CreateMap<QuartersReportDTO, QuartersReportAdminstrativeDetailsVM>().ReverseMap();
            CreateMap<QuartersReport, QuartersReportActivitiesDetailsVM>().ReverseMap();
            CreateMap<QuartersReportDTO, QuartersReportActivitiesDetailsVM>().ReverseMap();
            CreateMap<QuartersReport, QuartersReportCollaborativeDetailsVM>().ReverseMap();
            CreateMap<QuartersReportDTO, QuartersReportCollaborativeDetailsVM>().ReverseMap();
            CreateMap<QuartersReport, QuartersReportDTO>().ReverseMap();
        }
    }
}