using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Domain.Entities;

public class UserPoints
{
    //[DatabaseGenerated(DatabaseGeneratedOption.None)] // To prevent auto-increment
    public int Id { get; set; }
    public string? FKUserId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalsReNew { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Totals { get; set; }
    public int? Points { get; set; }
}