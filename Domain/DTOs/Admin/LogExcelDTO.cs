using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Admin
{
    public class LogExcelDTO
    {
        public int SerialNo { get; set; }                // @no
        public string UserFullName { get; set; }         // log.UserFullName
        public string Date { get; set; }                 // log.RequestTime.Date
        public string Time { get; set; }                 // log.RequestTime.Time
        public string ProcessDescription { get; set; }   // Process and translated texts
    }
}
