using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class SupplierCategory
    {
        public int Id { get; set; }
        [StringLength(200)]
        public string? NameAr { get; set; }
        [StringLength(200)]
        public string? NameEn { get; set; }

        public ICollection<Supplier>? Suppliers { get; set; }
    }
}
