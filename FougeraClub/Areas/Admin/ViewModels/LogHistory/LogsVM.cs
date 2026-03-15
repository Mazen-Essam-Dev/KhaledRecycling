using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.LogHistory
{
    public class LogsVM
    {
        public int Id { get; set; }
        [MaxLength(450)]
        public string? UserId { get; set; }
        public string? Path { get; set; }
        public string? Method { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        //for Process Name
        public string? NameAr { get; set; }
        //for Process Name
        public string? NameEn { get; set; }

        // id for Targeted 
        public string? LogTarget { get; set; }
        public DateTime RequestTime { get; set; }
        public string? UserFullName { get; set; }
        public string? UserFullNameEn { get; set; }
        public string? UserFullNameAr { get; set; }
        // Indicates whether the log belongs to a member or a system user. Values: "Member" or "SystemUser"
        public string? UserKind { get; set; }
    }
}
