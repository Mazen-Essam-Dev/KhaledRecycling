using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Domain.Entities;

public class MoneyPushed
{
    //[DatabaseGenerated(DatabaseGeneratedOption.None)] // To prevent auto-increment
    public int Id { get; set; }
    public string? FKUserId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Money { get; set; }
    public char? TypeTransaction { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? ItemName { get; set; }
    public string? Notes { get; set; }

}