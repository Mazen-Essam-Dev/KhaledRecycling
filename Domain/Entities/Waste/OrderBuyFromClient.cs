using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Waste;

public class OrderBuyFromClient
{
    public int Id { get; set; }
    public string? FKUserId { get; set; }
    public string? Address { get; set; }
    public int? FKSubWasteId { get; set; }

    public int? CountUnits { get; set; }
    public double? Kilo { get; set; }

    public DateTime? OrderDate { get; set; }
    public DateTime? ApprovalDate { get; set; }

    public double? DiscountRatio { get; set; }
    public double? DiscountValue { get; set; }
    public double? Total { get; set; }

    public int? StatusId { get; set; }

    [ForeignKey("FKSubWasteId")]
    public SubWaste? SubWaste { get; set; }

    [ForeignKey("StatusId")]
    public Status? Status { get; set; }

    public int? FKUserType { get; set; }

    [ForeignKey("FKUserType")]
    public UserType? UserType { get; set; }
}