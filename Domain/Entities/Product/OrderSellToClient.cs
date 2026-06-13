using Domain.Entities.Product;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Product;

public class OrderSellToClient
{
    public int Id { get; set; }
    public string? FKUserId { get; set; }
    public int? FKSubProductId { get; set; }

    public int? CountUnits { get; set; }

    public DateTime? OrderDate { get; set; }
    public DateTime? ApprovalDate { get; set; }

    public double? DiscountRatio { get; set; }
    public double? DiscountValue { get; set; }
    public double? Total { get; set; }
    public string? Address { get; set; }
    public string? StoreNotes { get; set; }

    public int? StatusId { get; set; }

    [ForeignKey("FKSubProductId")]
    public SubProduct? SubProduct { get; set; }

    [ForeignKey("StatusId")]
    public Status? Status { get; set; }
    public int? FKUserType { get; set; }

    [ForeignKey("FKUserType")]
    public UserType? UserType { get; set; }
}