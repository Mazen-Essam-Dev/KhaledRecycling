namespace Domain.DTOs.Admin
{
    public class TrainersNameDTO
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? FullNameAr { get; set; }
        public string? FullNameEn { get; set; }
        public string? Email { get; set; }
        public int? DepartmentId { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
