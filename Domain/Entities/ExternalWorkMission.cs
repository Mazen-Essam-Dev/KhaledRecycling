using Domain.Entities.Employees;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Domain.Entities
{
    public class ExternalWorkMission
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Employee))]
        public int? EmployeeId { get; set; }


        [ForeignKey(nameof(Signature))]
        public int? SignatureIdApproved { get; set; }
        public virtual Signature? Signature { get; set; }

        public DateOnly? MissionDate { get; set; }

        [MaxLength(200)]
        public string? MissionLocation { get; set; }
        [MaxLength(200)]
        public string? MissionCountry { get; set; }
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
}
