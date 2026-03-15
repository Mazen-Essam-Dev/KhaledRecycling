using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using FougeraClub.Areas.Member.ViewModels;

namespace FougeraClub.Areas.Member.Mappings
{
    public class IDCardExtractedDataProfile : Profile
    {
        public IDCardExtractedDataProfile()
        {
            CreateMap<IDCardExtractedDataVM, IDCardExtractedDataDTO>().ReverseMap();
        }
    }
}
