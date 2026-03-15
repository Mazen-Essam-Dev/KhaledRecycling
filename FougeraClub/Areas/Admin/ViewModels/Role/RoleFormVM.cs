using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.Role
{
    public class RoleFormVM
    {
        [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
        public string Name { get; set; }
    }
}
