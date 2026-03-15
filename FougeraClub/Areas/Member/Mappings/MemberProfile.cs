using Application.Helpers;
using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.Member.Account;
using Domain.Entities;
using FougeraClub.Areas.Member.ViewModels;

namespace FougeraClub.Areas.Member.Mappings
{
    public class MemberProfile : Profile
    {
        public MemberProfile()
        {
            CreateMap<MemberEditVM, MemberEntity>()
                .ForMember(dest => dest.Nationality, opt => opt.Ignore()) // Ignore navigation properties
                .ForMember(dest => dest.MemberType, opt => opt.Ignore()) // Ignore navigation properties
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore()) // Ignore collections
                                                                            //.ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => AppDubaiTime.ConvertToDubai(src.Da)))
                .ReverseMap();

            CreateMap<MemberRegisterVM, MemberEntity>()
                .ForMember(dest => dest.Nationality, opt => opt.Ignore()) // Ignore navigation properties
                .ForMember(dest => dest.MemberType, opt => opt.Ignore()) // Ignore navigation properties
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore()) // Ignore collections
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => HashHelper.ComputeSha256Hash(src.Password)))
                .ReverseMap();

            CreateMap<MemberVM, MemberEntity>()
                .ForMember(dest => dest.RegistrationDate, opt => opt.Ignore())
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => AppDubaiTime.ConvertToDubaiDateOnly(src.RegistrationDate)));


            CreateMap<MemberEntity, MemberVM>()
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => AppDubaiTime.ConvertToDubaiDateOnly(src.RegistrationDate)));

            CreateMap<MemberDTO, MemberRegisterVM>().ReverseMap();
            CreateMap<MemberDTO, MemberEntity>().ReverseMap();
            CreateMap<FilesDTO, MemberRegisterVM>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.MemberCode))
                .ForMember(dest => dest.ProfileImage, opt => opt.MapFrom(src => src.ProfileImage))
                .ForMember(dest => dest.ProfileImagePath, opt => opt.MapFrom(src => src.ProfileImagePath))
                .ForMember(dest => dest.IdImage, opt => opt.MapFrom(src => src.IdImage))
                .ForMember(dest => dest.IdImagePath, opt => opt.MapFrom(src => src.IdImagePath))
                .ForMember(dest => dest.PassportImage, opt => opt.MapFrom(src => src.PassportImage))
                .ForMember(dest => dest.PassportImagePath, opt => opt.MapFrom(src => src.PassportImagePath))
                .ReverseMap();

        }
    }
}
