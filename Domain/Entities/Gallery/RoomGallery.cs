using Domain.Entities.Product;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Gallery;

public class RoomGallery
{
    public int Id { get; set; }
    public string? GenCode { get; set; }
    public int? FkSubProduct { get; set; }
    public int? MaxUnit { get; set; }
    public int? FilledUnits { get; set; }
    public int? ReservedUnits { get; set; }
    public int? FkGallery { get; set; }
    public string? Description { get; set; }

    [ForeignKey("FkSubProduct")]
    public SubProduct? SubProduct { get; set; }

    [ForeignKey("FkGallery")]
    public Gallery? Gallery { get; set; }
}