using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace Domain.Entities;

public class Financial
{
    public int Id { get; set; }
    public string? TableType { get; set; }
    public int? ItsId { get; set; }
    public char? TypeTransaction { get; set; }
    public double? Total { get; set; }
    public int? StatusId { get; set; }

    [ForeignKey("StatusId")]
    public Status? Status { get; set; }
}