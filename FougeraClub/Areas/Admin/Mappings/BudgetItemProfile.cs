using AutoMapper;
using Domain.Entities.BudgetItem;
using FougeraClub.Areas.Admin.ViewModels.BudgetItem;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class BudgetItemProfile : Profile
    {
        public BudgetItemProfile()
        {
            CreateMap<BudgetItemVM, BudgetItem>().ReverseMap();
        }
    }
}
