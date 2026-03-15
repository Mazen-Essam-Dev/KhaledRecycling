using Domain.Entities.SMS;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.SMS;

public class SMSVM 
{
    public int Id { get; set; }
    public List<SMSReceiver>? Receivers { get; set; }

    [LocalizedRequired("Required")]
    public string? Text { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime SendingDate { get; set; }
    public string? Status { get; set; }
    [Required]
    public List<int>? SendTo { get; set; }
    
}





