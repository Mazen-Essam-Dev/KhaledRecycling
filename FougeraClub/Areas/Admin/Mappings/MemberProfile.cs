using AutoMapper;
using Domain.Entities;
using KhaledTeamRecycling.Areas.Admin.ViewModels.Member;

namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class MemberProfile : Profile
    {
        public MemberProfile()
        {
            CreateMap<MemberVM, MemberEntity>()
            .ForMember(dest => dest.RegistrationDate, opt => opt.Ignore());


            CreateMap<MemberEntity, MemberVM>();
        }
    }
}