using AutoMapper;
using Domain.DTOs.Admin.CashDisbursementVoucher;
using Domain.Entities.CashDisbursementVoucher;
using FougeraClub.Areas.Admin.ViewModels.CashDisbursementVoucher;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class CashDisbursementVoucherProfile : Profile
    {
        public CashDisbursementVoucherProfile()
        {
            // Parent
            CreateMap<CashDisbursementVoucherVM, CashDisbursementVoucher>()
                .ReverseMap();

            // Child
            CreateMap<CashDisbursementVoucherDetailVM, CashDisbursementVoucherDetail>()
                .ReverseMap();            
            
            CreateMap<ExchangeProofVM, ExchangeProof>().ReverseMap();            
            CreateMap<ExchangeProofDetailVM, ExchangeProofDetail>().ReverseMap();            
            CreateMap<AcknowledgmentReceiptVM, AcknowledgmentReceipt>().ReverseMap();

            // Attachments
            CreateMap<CashDisbursementVoucherAttachmentsVM, CashDisbursementVoucherAttachmentsDTO>().ReverseMap();
            CreateMap<CashDisbursementVoucherAttachmentVM, CashDisbursementVoucherAttachmentDTO>().ReverseMap();
            CreateMap<CashDisbursementVoucherAttachment, CashDisbursementVoucherAttachmentVM>().ReverseMap();
            CreateMap<CashDisbursementVoucherAttachment, CashDisbursementVoucherAttachmentsVM>().ReverseMap();
        }
    }
}