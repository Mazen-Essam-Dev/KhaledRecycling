using AutoMapper;
using Domain.Entities.ParticipationsInEventReport;
using FougeraClub.Areas.Admin.ViewModels.ParticipationsInEventReport;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class ParticipationsInEventReportProfile : Profile
    {
        public ParticipationsInEventReportProfile()
        {
            // Parent
            CreateMap<ParticipationsInEventReportVM, ParticipationsInEventReport>()
                .ReverseMap();

            // Child
            CreateMap<ParticipationsInEventReportDetailVM, ParticipationsInEventReportDetail>()
                .ReverseMap();
        }
    }
}