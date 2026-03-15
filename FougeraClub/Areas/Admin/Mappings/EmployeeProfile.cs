using AutoMapper;
using Domain.DTOs.Admin.Employees;
using Domain.Entities.Employees;
using FougeraClub.Areas.Admin.ViewModels.Employees;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<EmployeeVM, Employee>().ReverseMap();
            
            CreateMap<EmployeeAttachmentsVM, EmployeeAttachmentsDTO>().ReverseMap();
            CreateMap<AttachmentVM, AttachmentDTO>().ReverseMap();

            CreateMap<EmployeeAttachment, AttachmentVM>().ReverseMap();
            CreateMap<EmployeeAttachment, EmployeeAttachmentsVM>().ReverseMap();
        }
    }
}
