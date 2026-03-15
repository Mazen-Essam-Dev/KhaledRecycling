using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Admin.AdministrativeReportQuarterlyAnnual
{
    public class MonthlyAdministrativeReportDetailDTO
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? ActivityStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? ActivityEndDate { get; set; }

        [MaxLength(200)]
        public string? ActivityName { get; set; }

        public int? NumberOfParticipants { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
