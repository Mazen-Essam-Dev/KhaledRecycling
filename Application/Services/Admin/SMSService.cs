using Application.Interfaces.Admin;
using Domain.DTOs.Admin.SMSDTO;
using Domain.Entities.SMS;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.Extensions.Options;

namespace Application.Services.Admin
{
    public class SMSService : ISMSService
    {
        private readonly SmsSettings _smsSettings;
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;

        public SMSService(IOptions<SmsSettings> smsSettings, HttpClient httpClient, IUnitOfWork unitOfWork)
        {
            _smsSettings = smsSettings.Value;
            _httpClient = httpClient;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SMS>> GetAllAsync()
        {
            return await _unitOfWork.SMS.GetAllAsync(t => t.Receivers);
        }
        public async Task<bool> SendMessageAsync(SendingDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Message) || string.IsNullOrWhiteSpace(dto.Message))
                    return false;

                SMS sms = new SMS
                {
                    Text = dto.Message,
                    SendingDate = DateTime.UtcNow,
                    Receivers = new List<SMSReceiver>()
                };

                // Case 1: If no specific receivers were chosen, send to all members
                if (dto.ReceiverIds == null || !dto.ReceiverIds.Any())
                {
                    var allMembers = await _unitOfWork.Members.GetAllAsync();
                    sms.Receivers = allMembers.Select(m => new SMSReceiver { ReceiverId = m.Id ,PhoneNumber =m.PhoneNumber}).ToList();
                }
                // Case 2: Send only to specified receivers
                else
                {
                    var allMembers = await _unitOfWork.Members.GetAllAsync();
                    foreach (var receiverId in dto.ReceiverIds)
                    {
                        var thisPhoneNumber = allMembers.FirstOrDefault(m => m.Id == receiverId)?.PhoneNumber;

                        sms.Receivers.Add(new SMSReceiver
                        {
                            ReceiverId = receiverId,
                            PhoneNumber = thisPhoneNumber
                        });
                    }
                }


                // ✅ optional safeguard
                if (!sms.Receivers.Any())
                    return false;

                foreach (var singleReciver in sms.Receivers)
                {
                    if (!string.IsNullOrEmpty(singleReciver.PhoneNumber))
                    {
                        //Sending SMS through service
                        var (success, message, response) = await SendSMSAsync(singleReciver.PhoneNumber, sms.Text ?? " ");
                        singleReciver.IsDelivered = success == true ? Domain.Enums.SMSStatus.Success : Domain.Enums.SMSStatus.Fail;
                        singleReciver.ServMessage = message;
                        singleReciver.ServResponse = response;
                    }
                }

                // continue saving or sending...
                await _unitOfWork.SMS.AddAsync(sms);
                await _unitOfWork.CompleteAsync();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<IEnumerable<MessageLogDTO>> GetAllMessagesLogsAsync()
        {
            var allSms = await _unitOfWork.SMSReceivers.GetAllAsync(s => s.Receiver, s => s.SMS);
            var messagesLogs = allSms.Select(s => new MessageLogDTO
            {
                MemberNameAr = s.Receiver.FullNameAr,
                MemberNameEn = s.Receiver.FullNameEn,
                Message = s.SMS.Text,
                PhoneNumber = s.Receiver.PhoneNumber,
                DateAndTime = s.SMS.SendingDate,
                IsDelivered = s.IsDelivered ?? Domain.Enums.SMSStatus.Fail,
                IdNumber = s.Receiver.IdNumber
            });
            return messagesLogs;
        }
        public async Task<(bool success, string message, string response)> SendSMSAsync(string to, string body)
        {
            var url = $"{_smsSettings.BaseUrl}" +
                      $"?userid={Uri.EscapeDataString(_smsSettings.UserId)}" +
                      $"&pwd={Uri.EscapeDataString(_smsSettings.Password)}" +
                      $"&mobile={Uri.EscapeDataString(to)}" +
                      $"&sender={Uri.EscapeDataString(_smsSettings.Sender)}" +
                      $"&msg={Uri.EscapeDataString(body)}" +
                      $"&msgtype={Uri.EscapeDataString(_smsSettings.MsgType)}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return (true, "SMS sent successfully!", responseBody);

                return (false, "Failed to send SMS", responseBody);
            }
            catch (HttpRequestException e)
            {
                return (false, e.Message, string.Empty);
            }
        }
    }

}
