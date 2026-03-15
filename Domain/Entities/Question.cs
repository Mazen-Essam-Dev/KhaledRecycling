using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Question
{
    [Key]
    public int Id { get; set; }

    [MaxLength(200)]
    public string? QuestionAr { get; set; }

    [MaxLength(200)]
    public string? QuestionEn { get; set; }

    [MaxLength(500)]
    public string? AnswerAr { get; set; }

    [MaxLength(500)]
    public string? AnswerEn { get; set; }

    public int? ActivityId { get; set; }

    [ForeignKey(nameof(ActivityId))]
    public virtual Activity? Activity { get; set; }
}
