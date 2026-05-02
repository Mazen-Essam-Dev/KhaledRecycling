using Domain.Entities.Waste;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace Domain.Entities.Contract;

public class ContractSubWaste
{
    public int Id { get; set; }
    public int? FKSubWaste { get; set; }
    public int? FKContract { get; set; }
    public int? CountUnits { get; set; }
    public double? Kilo { get; set; }
    public int? StatusId { get; set; }

    [ForeignKey("FKSubWaste")]
    public SubWaste? SubWaste { get; set; }

    [ForeignKey("FKContract")]
    public Contract? Contract { get; set; }

    [ForeignKey("StatusId")]
    public Status? Status { get; set; }
}