using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class DocumentCategory
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string? NameAr { get; set; }

        [StringLength(200)]
        public string? NameEn { get; set; }

        public ICollection<ArchivingDocument>? ArchivingDocuments { get; set; }
    }
}
