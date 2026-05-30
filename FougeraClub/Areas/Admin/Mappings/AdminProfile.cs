using AutoMapper;
using Domain.Entities;
using KhaledTeamRecycling.Areas.Admin.ViewModels.Account;
using Infrastructure.Identity;

namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<AdminVM, ApplicationUser>()
            .ForMember(dest => dest.PasswordHash, opt =>
                opt.Condition((src, dest, srcMember, destMember, context) => src.Id == null));


            // Allow reverse mapping normally (e.g., ApplicationUser to AdminVM)
            CreateMap<ApplicationUser, AdminVM>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ReverseMap();

            CreateMap<Signature, SignatureVM>().ReverseMap();

            CreateMap<ApplicationUser, ResetPasswordVM>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ReverseMap();


        }
    }
}
