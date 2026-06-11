using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace Domain.Entities.Product;

public class SubProduct
{
    public int Id { get; set; }
    public int? FKMainProductId { get; set; }

    public string? NameAr { get; set; }
    public string? NameEn { get; set; }

    public double? SellPriceUnit { get; set; }
    public double? SellPriceKilo { get; set; }
    public double? SellPriceTon { get; set; }

    public double? BuyPriceUnit { get; set; }
    public double? BuyPriceKilo { get; set; }
    public double? BuyPriceTon { get; set; }

    public double? RatioCountFor1Kilo { get; set; }
    public int? StatusId { get; set; }
    public Char? StatusChar { get; set; }


    [ForeignKey("FKMainProductId")]
    public MainProduct? MainProduct { get; set; }

    [ForeignKey("StatusId")]
    public Status? Status { get; set; }
}