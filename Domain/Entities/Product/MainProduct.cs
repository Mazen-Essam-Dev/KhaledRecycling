using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Product;

public class MainProduct
{
    public int Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }

    public ICollection<SubProduct>? SubProducts { get; set; }
}