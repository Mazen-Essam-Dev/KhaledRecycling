using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Status
{
    public int Id { get; set; }
    public string? ShortChar { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
}