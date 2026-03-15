using AutoMapper;
using Domain.DTOs.Admin.ExpenseAndReceipt;
using Domain.Entities.ExpenseAndReceipt;
using FougeraClub.Areas.Admin.ViewModels.ExpenseAndReceipts;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class ExpenseAndReceiptProfile : Profile
    {
        public ExpenseAndReceiptProfile()
        {
            CreateMap<ExpenseAndReceiptVM, ExpenseAndReceiptAndOther>()
                //.ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ExpensesReportVM, ExpensesReportDTO>().ReverseMap();
            CreateMap<ExpensesReportElementVM, ExpensesReportElementDTO>().ReverseMap();

            CreateMap<ExpensesAndReceiptsReportVM, ExpensesAndReceiptsReportDTO>().ReverseMap();
            CreateMap<ExpensesAndReceiptsReportElementVM, ExpensesAndReceiptsReportElementDTO>().ReverseMap();

            CreateMap<ReceivingReceiptVM, ReceivingReceipt>().ReverseMap();
        }
    }
}
