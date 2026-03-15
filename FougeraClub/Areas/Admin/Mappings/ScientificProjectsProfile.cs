using AutoMapper;
using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.ScientificProject;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class ScientificProjectsProfile : Profile
    {
        public ScientificProjectsProfile()
        {
            CreateMap<ScientificProjectsVM, ScientificProjects>().ReverseMap();
        }
    }
}
