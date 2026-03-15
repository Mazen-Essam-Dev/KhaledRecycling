namespace Domain.DTOs.Admin.SMSDTO
{
    public class SmsSettings
    {
        public string BaseUrl { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Sender { get; set; }
        public string MsgType { get; set; }
    }
}
