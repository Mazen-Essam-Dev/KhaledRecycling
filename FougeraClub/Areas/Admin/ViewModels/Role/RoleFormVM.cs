using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.Role
{
    public class RoleFormVM
    {
        [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
        public string Name { get; set; }
    }
}
