using Domain.HelperForDomain;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class MemberEntity
    {
        [Key]
        public int Id { get; set; }
        public int? Code { get; set; }

        [MaxLength(100)]
        public string? FullNameAr { get; set; }
        [MaxLength(100)]
        public string? FullNameEn { get; set; }

        public int? NationalityId { get; set; }

        public virtual Nationality? Nationality { get; set; }

        public int? GenderId { get; set; }

        [MaxLength(50)]
        public string? IdNumber { get; set; }

        [Column(TypeName = "date")]
        public DateOnly? IdExpiryDate { get; set; }

        [Column(TypeName = "date")]
        public DateOnly? DateOfBirth { get; set; }

        [MaxLength(10)]
        public int? Age { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(20)]
        public string? FatherPhone { get; set; }

        [MaxLength(20)]
        public string? MotherPhone { get; set; }

        [MaxLength(100)]
        public string? AcademicQualification { get; set; }

        [MaxLength(100)]
        public string? EducationInstitution { get; set; }

        public int? ProfessionId { get; set; }

        public int? CityId { get; set; }


        [MaxLength(50)]
        public string? Profession { get; set; }

        [MaxLength(100)]
        public string? ProfessionPlace { get; set; }

        [MaxLength(50)]
        public string? GuardianProfession { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        public string? Hobby { get; set; }

        public string? Languages { get; set; }



        public int? HeardBy { get; set; }
        public bool? License { get; set; }


        [MaxLength(100)]
        public string? Facebook { get; set; }

        [MaxLength(100)]
        [Column("XPlatform")]
        public string? Xplatform { get; set; }

        [MaxLength(100)]
        public string? Instagram { get; set; }



        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Password { get; set; }


        [MaxLength(300)]
        public string? ProfileImagePath { get; set; }
        [NotMapped]
        public IFormFile? ProfileImage { get; set; }

        [MaxLength(300)]
        public string? IdImagePath { get; set; }
        [NotMapped]
        public IFormFile? IdImage { get; set; }

        [MaxLength(300)]
        public string? PassportImagePath { get; set; }
        [NotMapped]
        public IFormFile? PassportImage { get; set; }
        //public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();



        [DataType(DataType.Date)]
        public DateOnly RegistrationDate { get; set; } = DateOnly.FromDateTime(AppDubaiTime1.Now);
        public bool Suspended { get; set; } = false;

        public int? MemberTypeId { get; set; }
        [ForeignKey("MemberTypeId")]
        public virtual MemberType? MemberType { get; set; }
    }
}
