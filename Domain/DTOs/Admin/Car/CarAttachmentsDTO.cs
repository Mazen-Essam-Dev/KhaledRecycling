namespace Domain.DTOs.Admin.Car
{
    public class CarAttachmentsDTO
    {
        public int CarId { get; set; }
        public List<CarAttachmentDTO> Attachments { get; set; } = new List<CarAttachmentDTO>();
    }
}
