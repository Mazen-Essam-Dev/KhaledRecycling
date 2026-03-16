namespace KhaledTeamRecycling.Areas.Admin.ViewModels.Course;

public class CertificateVM
{
    public int Id { get; set; }
    public string? MemberNameAr { get; set; }
    public string? MemberNameEn { get; set; }
    public int? GenderId { get; set; }

    public string? CourseTitleAr { get; set; }
    public string? CourseTitleEn { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? CertificateSerial { get; set; }
    public string? CertificateSerialHashed { get; set; }
    public string? QrCodeBase64 { get; set; }

    public bool IsValid { get; set; }=false;

}





