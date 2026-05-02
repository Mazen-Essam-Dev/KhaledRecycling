using Domain.Entities.Product;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace Domain.Entities.Contract;

public class ContractSubProduct
{
    public int Id { get; set; }
    public int? FKSubProduct { get; set; }
    public int? FKContract { get; set; }
    public int? CountUnits { get; set; }
    public double? Kilo { get; set; }
    public int? StatusId { get; set; }

    [ForeignKey("FKSubProduct")]
    public SubProduct? SubProduct { get; set; }

    [ForeignKey("FKContract")]
    public Contract? Contract { get; set; }

    [ForeignKey("StatusId")]
    public Status? Status { get; set; }
}