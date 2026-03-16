using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.Employees
{
    public class AttachmentVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [MaxLength(300)]
        public string? Path { get; set; }
        public IFormFile? File { get; set; }
    }
}
