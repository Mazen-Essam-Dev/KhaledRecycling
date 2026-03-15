using Domain.Entities;
using Domain.Entities.quote;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.quote;

public class quoteVM
{
   
    public int Id { get; set; }

    [LocalizedRequired("Required")]
    [LocalizedMaxLength(50, "MaxLength_50")]

    public string? quoteCode { get; set; } = null!;

    [LocalizedRequired("Required")]
    [LocalizedMaxLength(200, "MaxLength_200")]
    public string? OrderText { get; set; }

    [LocalizedMaxLength(500, "MaxLength_500")]
    public string? TextArea { get; set; }

    [LocalizedRequired("Required")]
    [Column(TypeName = "date")]
    public DateOnly? Date { get; set; }

    /// <summary>
    /// Sum of (Quantity * SinglePrice) for items.
    /// Maintained by service or computed at runtime (NotMapped).
    /// </summary>
    //[Column(TypeName = "decimal(18,2)")]
    [Range(0.00, double.MaxValue, ErrorMessage = "القيمة يجب أن تكون رقم")]
    public decimal? OrderTotal { get; set; } = 0.00m;

    // Navigation
    public virtual ICollection<quotesItem> quotesItems { get; set; } = new List<quotesItem>();

    [ForeignKey(nameof(SignatureSuperVisor))]
    public int? SignatureSuperVisorId { get; set; }
    public virtual Signature? SignatureSuperVisor { get; set; }


    [ForeignKey(nameof(SignatureAccountant))]
    public int? SignatureAccountantId { get; set; }
    public virtual Signature? SignatureAccountant { get; set; }


    [ForeignKey(nameof(SignatureUserSecetary))]
    public int? SignatureUserSecetaryId { get; set; }
    public virtual Signature? SignatureUserSecetary { get; set; }


    [ForeignKey(nameof(SignatureManager))]
    public int? SignatureManagerId { get; set; }
    public virtual Signature? SignatureManager { get; set; }
}

