using AutoMapper;
using Domain.DTOs.Admin.SalaryManagement;
using Domain.DTOs.Admin.SalaryManagement;
using Domain.Entities.SalaryManage;
using FougeraClub.Areas.Admin.ViewModels.SalaryManagement;
using FougeraClub.Areas.Admin.ViewModels.SalaryManagement;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class SalaryManagementProfile : Profile
    {
        public SalaryManagementProfile()
        {
            CreateMap<SalaryManagementVM, SalaryManagement>().ReverseMap();

            CreateMap<AbsenceVM, AbsenceDTO>().ReverseMap();

            // Attachments
            CreateMap<SalaryManagementAttachmentsVM, SalaryManagementAttachmentsDTO>().ReverseMap();
            CreateMap<SalaryManagementAttachmentVM, SalaryManagementAttachmentDTO>().ReverseMap();
            CreateMap<SalaryManagementAttachment, SalaryManagementAttachmentVM>().ReverseMap();
            CreateMap<SalaryManagementAttachment, SalaryManagementAttachmentDTO>().ReverseMap();
        }
    }
}
