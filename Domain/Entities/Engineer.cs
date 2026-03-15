using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Engineer
    {
        [Key]
        public int Id { get; set; }



        [MaxLength(100)]
        public string? FullName { get; set; }
        public int? Code { get; set; }



        [MaxLength(100)]
        public string? Specialization { get; set; }
        [Range(1900, 2100, ErrorMessage = "Graduation year is not valid.")]
        public int? GraduationYear { get; set; }



        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; }



        // Foreign key for Nationality
        public int? NationalityId { get; set; }

        [ForeignKey(nameof(NationalityId))]
        public virtual Nationality? Nationality { get; set; }



        [MaxLength(200)]
        public string? WorkAddress { get; set; }
        [MaxLength(200)]
        public string? Address { get; set; }


        [Phone(ErrorMessage = "Phone number is not valid.")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Phone(ErrorMessage = "Phone number is not valid.")]
        [MaxLength(20)]
        public string? Mobile { get; set; }

        [EmailAddress(ErrorMessage = "Email address is not valid.")]
        [MaxLength(100)]
        public string? Email { get; set; }

        public string? Notes { get; set; }

        [MaxLength(255)]
        public string? ProfileImagePath { get; set; }




        [MaxLength(50)]
        public string? PassportNumber { get; set; }

        [Column(TypeName = "date")]
        public DateOnly? PassportExpiryDate { get; set; }
        


        [MaxLength(50)]
        public string? IdNumber { get; set; }

        [Column(TypeName = "date")]
        public DateOnly? IdExpiryDate { get; set; }
        [MaxLength(50)]
        public string? IdReleaseLocation { get; set; }


        [Range(0, 1000000, ErrorMessage = "Salary must be between 0 and 1,000,000.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Salary { get; set; }

        [MaxLength(50)]
        public string? BankAccountNo { get; set; }
        [MaxLength(50)]
        public string? BankName { get; set; }


    }
}


