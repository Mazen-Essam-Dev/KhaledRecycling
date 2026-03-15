namespace Domain.DTOs.Admin.Course;

public class CertificateDTO
{
    public string? MemberNameAr { get; set; }
    public string? MemberNameEn { get; set; }
    public int? GenderId { get; set; }
    
    public string? CourseTitleAr { get; set; }
    public string? CourseTitleEn { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? CertificateSerial { get; set; }

}