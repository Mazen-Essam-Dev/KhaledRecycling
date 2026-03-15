using Domain.Entities;
using Domain.Entities.Employees;
using FougeraClub.Areas.Admin.ViewModels.EstimatedBudgetForExternalParticipation;
using FougeraClub.Helpers;
using Humanizer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.ExternalWorkMission;

public class ExternalWorkMissionVM
{
    // Display manager full name under signature
    public string? ManagerFullName { get; set; }

    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Employee))]
    [LocalizedRequired("Required")]
    public int? EmployeeId { get; set; }


    [ForeignKey(nameof(Signature))]
    public int? SignatureIdApproved { get; set; }
    public virtual Signature? Signature { get; set; }

    [LocalizedRequired("Required")]
    public DateOnly? MissionDate { get; set; }

    [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
    public string? MissionLocation { get; set; }
    [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
    [MaxLength(200)]
    public string? MissionCountry { get; set; }
    [LocalizedRequired("Required")]
    [LocalizedMaxLength(300, "MaxLength_300")]
    [MaxLength(300)]
    public string? MissionWorkDescription { get; set; }

    public decimal? PetroleumFees { get; set; }
    public decimal? FoodFees { get; set; }
    public decimal? MissionAllowance { get; set; }


    [MaxLength(200)]
    public string? CandidateName1 { get; set; }

    [MaxLength(200)]
    public string? CandidateName2 { get; set; }

    [MaxLength(200)]
    public string? CandidateName3 { get; set; }

    [MaxLength(200)]
    public string? CandidateName4 { get; set; }

    [MaxLength(200)]
    public string? CandidateAdj1 { get; set; }

    [MaxLength(200)]
    public string? CandidateAdj2 { get; set; }

    [MaxLength(200)]
    public string? CandidateAdj3 { get; set; }

    [MaxLength(200)]
    public string? CandidateAdj4 { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<Mission> Missions { get; set; } = new List<Mission>();

}



