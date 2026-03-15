using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MemberType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // To prevent auto-increment
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "Arabic name cannot exceed 100 characters.")]
        public string? NameAr { get; set; }

        [MaxLength(100, ErrorMessage = "English name cannot exceed 100 characters.")]
        public string? NameEn { get; set; }
    }
}
