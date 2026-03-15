using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class IDCardExtractedDataDTO
    {
        public string? lable { get; set; }
        public string? ProbabilityString { get; set; }
        public double? Probability_double { get; set; } = 0;
        public bool? doneAI_bool { get; set; }
        public bool? doneOCR_bool { get; set; }
        public bool? doneValidation_bool { get; set; }
        public string? DoneTextExtracted_Error_Str { get; set; }
        public string? textExtracted { get; set; }
        public string? matchIDNumber { get; set; }
        public string? matchBirth { get; set; }
        public string? matchExpiryDate { get; set; }
        public string? matchFullEnName { get; set; }
        public string? matchFullArName { get; set; }
        public string? matchGender { get; set; }
        public string? matchNationality { get; set; }

    }
}
