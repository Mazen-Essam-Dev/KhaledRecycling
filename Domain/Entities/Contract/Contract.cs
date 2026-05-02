using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace Domain.Entities.Contract;

public class Contract
{
    public int Id { get; set; }
    public string? GenCode { get; set; }
    public int? FKSignature { get; set; }
    public int? StatusId { get; set; }

    [ForeignKey("StatusId")]
    public Status? Status { get; set; }

    public ICollection<ContractSubWaste>? ContractSubWastes { get; set; }
    public ICollection<ContractSubProduct>? ContractSubProducts { get; set; }
}

