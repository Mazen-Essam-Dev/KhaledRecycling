using AutoMapper;
using Domain.Entities.EstimatedBudgetForExternalParticipation;
using FougeraClub.Areas.Admin.ViewModels;
using FougeraClub.Areas.Admin.ViewModels.EstimatedBudgetForExternalParticipation;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class EstimatedBudgetForExternalParticipationProfile : Profile
    {
        public EstimatedBudgetForExternalParticipationProfile()
        {
            CreateMap<EstimatedBudgetForExternalParticipationVM, EstimatedBudgetForExternalParticipation>().ReverseMap();
            CreateMap<EstimatedBudgetForExternalParticipationDetailVM, EstimatedBudgetForExternalParticipationDetail>().ReverseMap();
        }
    }
}