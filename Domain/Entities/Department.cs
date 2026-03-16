using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Department
{
    [Key]
    public int Id { get; set; }

    [MaxLength(200)]
    public string? NameAr { get; set; }

    [MaxLength(200)]
    public string? NameEn { get; set; }
    public virtual ICollection<MemberEntity> Members { get; set; } = new List<MemberEntity>();

}
