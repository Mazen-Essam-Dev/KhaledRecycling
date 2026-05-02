using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace Domain.Entities.Waste;

public class MainWaste
{
    public int Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }

    public ICollection<SubWaste>? SubWastes { get; set; }
}