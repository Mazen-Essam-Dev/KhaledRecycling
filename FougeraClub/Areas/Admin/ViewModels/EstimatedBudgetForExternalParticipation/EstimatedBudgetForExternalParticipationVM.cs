using Domain.Entities;
using Domain.Entities.EstimatedBudgetForExternalParticipation;
using FougeraClub.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.EstimatedBudgetForExternalParticipation
{
    public class EstimatedBudgetForExternalParticipationVM : IValidatableObject
    {
        // Display manager full name under signature
        public string? ManagerFullName { get; set; }

        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Signature))]
        public int? SignatureIdApproved { get; set; }
        public virtual Signature? Signature { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? ParticipatingTitle { get; set; }

        [LocalizedRequired("Required")]
        [DataType(DataType.Date)]
        public DateOnly? ParticipatingDate { get; set; }
        [LocalizedRequired("Required")]
        public DateOnly? ParticipatingDateTo { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? Regulator { get; set; }
        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? ParticipatingCountry { get; set; }
        [IntAttribute("Integer")]
        [LocalizedRequired("Required")]
        public int? ParticipatingType { get; set; }
        [IntAttribute("Integer")]
        [LocalizedRequired("Required")]
        public int? RequiredToparticipate { get; set; }
        [LocalizedRequired("Required")]
        [IntAttribute("Integer")]
        public int? ManagersCount { get; set; }
        [LocalizedRequired("Required")]
        [IntAttribute("Integer")]
        public int? TechnicalSupervisorsCount { get; set; }
        [LocalizedRequired("Required")]
        [IntAttribute("Integer")]
        public int? ActivitiesSupervisorsCount { get; set; }
        [LocalizedRequired("Required")]
        [IntAttribute("Integer")]
        public int? MembersCount { get; set; }

        [DecimalAttribute("Decimal_double")]
        public decimal? ParticipationFeesHeadOfDelegation { get; set; } = null;
        [DecimalAttribute("Decimal_double")]
        public decimal? ParticipationFeesForEntireTeam { get; set; } = null;
        [DecimalAttribute("Decimal_double")]
        public decimal? Subsidies { get; set; } = null;
        [LocalizedMaxLength(100, "MaxLength_100")]
        public string? FeeStatement { get; set; } = null;
        [DecimalAttribute("Decimal_double")]
        public decimal? TotalFeesForParticipation { get; set; } = null;
        [DecimalAttribute("Decimal_double")]
        public decimal? ParticipationFeesForAdministrators { get; set; } = null;
        [DecimalAttribute("Decimal_double")]
        public decimal? ReserveAmountForDelegation { get; set; } = null;
        [DecimalAttribute("Decimal_double")]
        public decimal? AllowanceTravelForHeadOfDelegation { get; set; } = null;
        [DecimalAttribute("Decimal_double")]
        public decimal? TotalBudgetRequired1 { get; set; } = null;
        [DecimalAttribute("Decimal_double")]
        public decimal? TotalBudgetRequired2 { get; set; } = null;
        [DecimalAttribute("Decimal_double")]
        public decimal? TravelTicketValue { get; set; } = null;
        [DecimalAttribute("Decimal_double")]
        public decimal? HotelAccommodationFees { get; set; } = null;
        [DecimalAttribute("Decimal_double")]
        public decimal? SupervisorsTravelAllowance { get; set; } = null;
        [DecimalAttribute("Decimal_double")]
        public decimal? MembersAllowance { get; set; } = null;


        [LocalizedMaxLength(500, "MaxLength_500")]
        public string? Notes { get; set; }

        [LocalizedRequired("Required")]
        [DataType(DataType.Date)]
        public DateOnly? CreationDate { get; set; } /*= DateOnly.FromDateTime(AppDubaiTime.Now);*/

        public IEnumerable<EstimatedBudgetForExternalParticipationDetailVM> EstimatedBudgetForExternalParticipationDetailVM { get; set; } = new List<EstimatedBudgetForExternalParticipationDetailVM>();


        public virtual ICollection<EstimatedBudgetForExternalParticipationDetail> EstimatedBudgetForExternalParticipationDetails { get; set; } = new List<EstimatedBudgetForExternalParticipationDetail>();

        #region Check End Date > must be after the Start Date
        // must : IValidatableObject
        // Ex -> public class CourseVM : IValidatableObject
        // add Check End Date > must be after the Start Date
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //// Disallow ParticipatingDate earlier than today
            //var minAllowed = DateOnly.FromDateTime(DateTime.Today);
            //if (ParticipatingDate.HasValue && ParticipatingDate.Value < minAllowed)
            //{
            //    if (SessionHelper.GetCurrentLanguage() == "ar")
            //    {
            //        yield return new ValidationResult(
            //            "لا يمكن أن يكون تاريخ البداية أقدم من اليوم",
            //            new[] { nameof(ParticipatingDate) }
            //        );
            //    }
            //    else
            //    {
            //        yield return new ValidationResult(
            //            "The Start Date cannot be earlier than today",
            //            new[] { nameof(ParticipatingDate) }
            //        );
            //    }
            //}

            if (ParticipatingDate.HasValue && ParticipatingDateTo.HasValue)
            {
                if (ParticipatingDateTo < ParticipatingDate)
                {
                    if (SessionHelper.GetCurrentLanguage() == "ar")
                    {
                        yield return new ValidationResult(
                            "تاريخ النهاية يجب أن يكون بعد تاريخ البداية",
                            new[] { nameof(ParticipatingDateTo) } //so that the error appears under ParticipatingDateTo
                        );
                    }
                    else
                    {
                        yield return new ValidationResult(
                            "The End Date must be after the Start Date",
                            new[] { nameof(ParticipatingDateTo) }  //so that the error appears under ParticipatingDateTo
                        );
                    }

                }
            }
        }
        #endregion
    }
}
