using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Domain.Entities.EstimatedBudgetForExternalParticipation
{
    public class EstimatedBudgetForExternalParticipation
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Signature))]
        public int? SignatureIdApproved { get; set; }
        public virtual Signature? Signature { get; set; }

        [MaxLength(100)]
        public string? ParticipatingTitle { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? ParticipatingDate { get; set; }
        [DataType(DataType.Date)]
        public DateOnly? ParticipatingDateTo { get; set; }
        [MaxLength(100)]
        public string? Regulator { get; set; }
        [MaxLength(100)]
        public string? ParticipatingCountry { get; set; }
        public int? ParticipatingType { get; set; }
        public int? RequiredToparticipate { get; set; }
        public int? ManagersCount { get; set; }
        public int? TechnicalSupervisorsCount { get; set; }
        public int? ActivitiesSupervisorsCount { get; set; }
        public int? MembersCount { get; set; }

        public decimal? ParticipationFeesHeadOfDelegation { get; set; }
        public decimal? ParticipationFeesForEntireTeam { get; set; }
        public decimal? Subsidies { get; set; }
        [MaxLength(100)]
        public string? FeeStatement { get; set; }
        public decimal? TotalFeesForParticipation { get; set; }
        public decimal? ParticipationFeesForAdministrators { get; set; }
        public decimal? ReserveAmountForDelegation { get; set; }
        public decimal? AllowanceTravelForHeadOfDelegation { get; set; }
        public decimal? TotalBudgetRequired1 { get; set; }
        public decimal? TotalBudgetRequired2 { get; set; }
        public decimal? TravelTicketValue { get; set; }
        public decimal? HotelAccommodationFees { get; set; }
        public decimal? SupervisorsTravelAllowance { get; set; }
        public decimal? MembersAllowance { get; set; }


        [MaxLength(500)]
        public string? Notes { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? CreationDate { get; set; } /*= DateOnly.FromDateTime(AppDubaiTime1.Now);*/

        public virtual ICollection<EstimatedBudgetForExternalParticipationDetail> EstimatedBudgetForExternalParticipationDetails { get; set; } = new List<EstimatedBudgetForExternalParticipationDetail>();

    }
}
