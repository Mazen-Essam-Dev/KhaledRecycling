using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace Domain.Entities.Waste;

public class SubWaste
{
    public int Id { get; set; }
    public int? FKMainWasteId { get; set; }

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

    [ForeignKey("FKMainWasteId")]
    public MainWaste? MainWaste { get; set; }

    [ForeignKey("StatusId")]
    public Status? Status { get; set; }
}