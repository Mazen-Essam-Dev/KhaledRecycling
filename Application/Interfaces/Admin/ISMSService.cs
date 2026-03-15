using Domain.DTOs.Admin.SMSDTO;
using Domain.Entities.SMS;

namespace Application.Interfaces.Admin
{
    public interface ISMSService
    {
        Task<IEnumerable<SMS>> GetAllAsync();
        Task<bool> SendMessageAsync(SendingDTO dto);
        Task<IEnumerable<MessageLogDTO>> GetAllMessagesLogsAsync();
        Task<(bool success, string message, string response)> SendSMSAsync(string to, string body);

    }

}
