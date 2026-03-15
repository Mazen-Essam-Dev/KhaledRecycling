using AutoMapper;
using Domain.DTOs.Admin;
using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.Trainers;
using Infrastructure.Identity;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class TrainerProfile : Profile
    {
        public TrainerProfile()
        {
            // ViewModel → Entity
            CreateMap<TrainerVM, Trainer>()
                .ForMember(dest => dest.AttachmentPath, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio));

            // Entity → ViewModel
            CreateMap<Trainer, TrainerVM>()
                .ForMember(dest => dest.User, opt => opt.Ignore()) // ❗ Required
                .ForMember(dest => dest.DepartmentsList, opt => opt.Ignore())
                .ForMember(dest => dest.UsersList, opt => opt.Ignore())
                .ForMember(dest => dest.Attachment, opt => opt.Ignore())
                .ForMember(dest => dest.Department, opt => opt.Ignore());

            // Entity → ViewModel
            CreateMap<TrainersNameDTO, TrainersNameVM>().ReverseMap();
            
        }
    }



}