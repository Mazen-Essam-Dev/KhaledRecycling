namespace Domain.DTOs.Admin.SMSDTO
{
    public class SendingDTO
    {
        public string? Message { get; set; }
        public List<int>? ReceiverIds { get; set; }
    }
}
