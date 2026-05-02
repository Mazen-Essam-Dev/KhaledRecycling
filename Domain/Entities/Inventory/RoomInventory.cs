using Domain.Entities.Waste;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Inventory;

public class RoomInventory
{
    public int Id { get; set; }
    public string? GenCode { get; set; }
    public int? FKSubWaste { get; set; }
    public double? MaxKilo { get; set; }
    public int? FkInventory { get; set; }
    public string? Description { get; set; }

    [ForeignKey("FKSubWaste")]
    public SubWaste? SubWaste { get; set; }

    [ForeignKey("FkInventory")]
    public Inventory? Inventory { get; set; }
}