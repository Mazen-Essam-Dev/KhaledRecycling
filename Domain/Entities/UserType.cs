using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class UserType
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)] // To prevent auto-increment
    public int Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
}