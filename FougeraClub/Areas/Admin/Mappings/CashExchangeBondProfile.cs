using AutoMapper;
using Domain.DTOs.Admin.CashExchangeBond;
using Domain.Entities;
using Domain.Entities.CashExchangeBond;
using Domain.Entities.MonthlyAdministrativeReport;
using FougeraClub.Areas.Admin.ViewModels.CashExchangeBond;
using FougeraClub.Areas.Admin.ViewModels.Member;
using FougeraClub.Areas.Admin.ViewModels.MonthlyAdministrativeReport;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class CashExchangeBondProfile : Profile
    {
        public CashExchangeBondProfile()
        {
            // Parent
            CreateMap<CashExchangeBond, CashExchangeBondVM>()
                .ReverseMap();

            // Child
            CreateMap<CashExchangeBondDetail, CashExchangeBondDetailVM>()
                .ReverseMap();

            // Attachments
            CreateMap<CashExchangeBondAttachmentsVM, CashExchangeBondAttachmentsDTO>().ReverseMap();
            CreateMap<CashExchangeBondAttachmentVM, CashExchangeBondAttachmentDTO>().ReverseMap();
            CreateMap<CashExchangeBondAttachment, CashExchangeBondAttachmentVM>().ReverseMap();
            CreateMap<CashExchangeBondAttachment, CashExchangeBondAttachmentDTO>().ReverseMap();
            CreateMap<CashExchangeBondAttachment, CashExchangeBondAttachmentsVM>().ReverseMap();
        }
    }
}