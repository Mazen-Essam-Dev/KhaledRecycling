using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs
{
    public class MemberDTO
    {
        public int? Code { get; set; }
        public string? FullNameAr { get; set; }
        public string? FullNameEn { get; set; }
        public int? NationalityId { get; set; }

        public string? IdNumber { get; set; }
        [DataType(DataType.Date)]
        public DateOnly? IdExpiryDate { get; set; }
        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }
        public int? Age { get; set; }

    }
}
