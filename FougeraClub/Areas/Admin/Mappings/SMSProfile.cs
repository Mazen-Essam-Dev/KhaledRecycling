using AutoMapper;
using Domain.DTOs.Admin.SMSDTO;
using Domain.Entities.SMS;
using FougeraClub.Areas.Admin.ViewModels.SMS;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class SMSProfile : Profile
    {
        public SMSProfile()
        {
            CreateMap<SMS, SMSVM>().ReverseMap();

            CreateMap<SendingVM, SendingDTO>().ReverseMap();

            CreateMap<MessageLogVM, MessageLogDTO>().ReverseMap();
        }
    }
}