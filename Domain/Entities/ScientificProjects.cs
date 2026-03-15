using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ScientificProjects
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? SerialCode { get; set; }

        public DateOnly? DateByCalander { get; set; }
        [ForeignKey(nameof(Department))]
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }

        [ForeignKey(nameof(Trainer))]
        public int? TrainerId { get; set; }
        public virtual Trainer? Trainer { get; set; }

        [MaxLength(200)]
        public string? ProjectNameAr { get; set; }
        [MaxLength(200)]
        public string? ProjectNameEn { get; set; }

        [MaxLength(200)]
        public string? IdeaOwnerAr { get; set; }
        [MaxLength(200)]
        public string? IdeaOwnerEn { get; set; }

        public string? ProjectIdeaAr { get; set; }

        public string? ProjectIdeaEN { get; set; }

        [MaxLength(200)]
        public string? ProjectElement1 { get; set; }

        [MaxLength(200)]
        public string? ProjectElement2 { get; set; }

        [MaxLength(200)]
        public string? ProjectElement3 { get; set; }

        [MaxLength(200)]
        public string? ProjectElement4 { get; set; }
        [MaxLength(200)]
        public string? ProjectElement5 { get; set; }
        [MaxLength(200)]
        public string? ProjectElement6 { get; set; }
        [MaxLength(200)]
        public string? ProjectElement7 { get; set; }
        [MaxLength(200)]
        public string? ProjectElement8 { get; set; }

        public string? InstallationRecommendations { get; set; }

        public string? DeliveryData { get; set; }

        [MaxLength(200)]
        public string? Participant1 { get; set; }

        [MaxLength(200)]
        public string? Participant2 { get; set; }

        [MaxLength(200)]
        public string? Participant3 { get; set; }

        [MaxLength(200)]
        public string? Participant4 { get; set; }
        [MaxLength(200)]
        public string? Supervisor1 { get; set; }
        [MaxLength(200)]
        public string? Supervisor2 { get; set; }
        [MaxLength(200)]
        public string? Supervisor3 { get; set; }
        [MaxLength(200)]
        public string? Supervisor4 { get; set; }
        [MaxLength(200)]
        public string? Supervisor5 { get; set; }
        [MaxLength(200)]
        public string? Supervisor6 { get; set; }

        public double? ExpectedCost { get; set; }

        [MaxLength(200)]
        public string? FilePath1 { get; set; }

        [MaxLength(200)]
        public string? FilePath2 { get; set; }

        [MaxLength(200)]
        public string? FilePath3 { get; set; }

        [MaxLength(200)]
        public string? FilePath4 { get; set; }
        [MaxLength(200)]
        public string? FilePath5 { get; set; }

        [MaxLength(200)]
        public string? FilePath6 { get; set; }

        [MaxLength(200)]
        public string? FilePreliminaryPath1 { get; set; }

        [MaxLength(200)]
        public string? FilePreliminaryPath2 { get; set; }

        [MaxLength(200)]
        public string? FilePreliminaryPath3 { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int? Trainer1SignitureId { get; set; }
        [ForeignKey(nameof(Trainer1SignitureId))]
        public virtual Signature? Trainer1Signiture { get; set; }

        public int? ActivityMonitorSignitureId { get; set; }
        [ForeignKey(nameof(ActivityMonitorSignitureId))]
        public virtual Signature? ActivityMonitorSigniture { get; set; }
        public int? ManagerSignitureId { get; set; }
        [ForeignKey(nameof(ManagerSignitureId))]
        public virtual Signature? ManagerSignature { get; set; }

        [MaxLength(100)]
        public string? Innovation_Individual_TeamAr { get; set; }
        [MaxLength(100)]
        public string? Innovation_Individual_TeamEn { get; set; }
        [MaxLength(100)]
        public string? Student_EmployeeAr { get; set; }
        [MaxLength(100)]
        public string? Student_EmployeeEn { get; set; }
        [MaxLength(200)]
        public string? InstitutionAr { get; set; }
        [MaxLength(200)]
        public string? InstitutionEn { get; set; }
        [MaxLength(200)]
        public string? OrganizationAr { get; set; }
        [MaxLength(200)]
        public string? OrganizationEn { get; set; }
        [MaxLength(20)]
        public double? Sponsorship { get; set; }
        [MaxLength(250)]
        public string? ApplicationAr { get; set; }
        [MaxLength(250)]
        public string? ApplicationEn { get; set; }
        public DateOnly? ClosedDate { get; set; }
        
        public virtual ICollection<ScientificProjectGoals>? ScientificProjectGoalsNavigation { get; set; }
        public virtual ICollection<ScientificProjectTools>? ScientificProjectToolsNavigation { get; set; }
        public virtual ICollection<ScientificProjectIndividuals>? ScientificProjectIndividualsNavigation { get; set; }

    }
}
