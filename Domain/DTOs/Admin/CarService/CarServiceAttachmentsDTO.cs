namespace Domain.DTOs.Admin.CarService
{
    public class CarServiceAttachmentsDTO
    {
        public int CarServiceId { get; set; }
        public List<CarServiceAttachmentDTO> Attachments { get; set; } = new List<CarServiceAttachmentDTO>();
    }
}
