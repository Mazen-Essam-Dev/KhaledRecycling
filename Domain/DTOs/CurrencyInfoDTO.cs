using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CurrencyInfoDTO
    {
        public string Singular { get; set; }      // واحد: Pound / جنيه
        public string Dual { get; set; }          // مثنى: Not used in English (لكن في العربي مهم)
        public string Plural { get; set; }        // جمع: Pounds / جنيهات
        public bool IsMasculine { get; set; }     // مذكر/مؤنث (للغة العربية)
        public string Suffix { get; set; }        // لاحقة: مصري / Egyptian
        public string Language { get; set; }      // "ar" أو "en"
    }
}
