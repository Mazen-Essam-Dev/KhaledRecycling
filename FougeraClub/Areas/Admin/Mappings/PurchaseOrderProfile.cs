using AutoMapper;
using Domain.DTOs.Admin.CarService;
using Domain.DTOs.Admin.PurchaseOrder;
using Domain.Entities;
using Domain.Entities.PurchaseOrder;
using FougeraClub.Areas.Admin.ViewModels.CarServices;
using FougeraClub.Areas.Admin.ViewModels.PurchaseOrder;


namespace FougeraClub.Areas.Admin.Mappings
{
    public class PurchaseOrderProfile : Profile
    {
        public PurchaseOrderProfile()
        {
            // Parent
            CreateMap<PurchaseOrderVM, PurchaseOrder>()
                .ReverseMap();

            // Child
            CreateMap<PurchaseOrderItemVM, PurchaseOrderItem>()
                .ReverseMap();


            // Attachments
            CreateMap<PurchaseOrderAttachmentsVM, PurchaseOrderAttachmentsDTO>().ReverseMap();
            CreateMap<PurchaseOrderAttachmentVM, PurchaseOrderAttachmentDTO>().ReverseMap();
            CreateMap<PurchaseOrderAttachment, PurchaseOrderAttachmentVM>().ReverseMap();
            CreateMap<PurchaseOrderAttachment, PurchaseOrderAttachmentDTO>().ReverseMap();

        }
    }
}