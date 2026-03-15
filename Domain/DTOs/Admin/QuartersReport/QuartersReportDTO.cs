using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Admin.QuartersReport
{
    public class QuartersReportDTO
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
