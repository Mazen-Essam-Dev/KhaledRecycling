using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.QuartersReport
{
    public class QuartersReport
    {
        [Key]
        public int Id { get; set; }
        public QuarterlyReportType? Type { get; set; }
        public QuartersYear? Quarter { get; set; }
        public int? Year { get; set; }

        public int? ManagerSignitureId { get; set; }
        [ForeignKey(nameof(ManagerSignitureId))]
        public virtual Signature? ManagerSignature { get; set; }

    }
}
