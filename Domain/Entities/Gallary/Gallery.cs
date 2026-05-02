using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Gallary;

public class Gallery
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? NoRooms { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }

    public ICollection<RoomGallery>? Rooms { get; set; }
}