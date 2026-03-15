using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Admin.AdministrativeReportQuarterlyAnnual
{
    public class MonthsOfYearsAnnualyActivitiesAndAdministrativeDTO
    {
        public List<MonthsOfYearsAnnualyWithDetailsDTO>? model_Activities { get; set; }
        public List<MonthsOfYearsAnnualyWithDetailsDTO>? model_Administrative { get; set; }
        public List<int>? listOfQuarter { get; set; }
        public List<string>? labelsMonths { get; set; }
        public List<int>? adminstrative { get; set; }
        public List<int>? activities { get; set; }
    }
    
}
