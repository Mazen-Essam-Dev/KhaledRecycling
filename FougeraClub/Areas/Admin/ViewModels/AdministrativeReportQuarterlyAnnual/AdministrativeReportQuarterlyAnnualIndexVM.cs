using Microsoft.AspNetCore.Mvc.Rendering;

namespace FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual
{
    public class AdministrativeReportQuarterlyAnnualIndexVM
    {
        public List<SelectListItem> Quarters { get; set; } = default!;
        public List<int> AdministrativeCounts { get; set; } = new List<int> { 0,0,0,0,0};
        public List<int> ActivitiesCounts { get; set; } = new List<int> { 0, 0, 0, 0, 0 };

        public bool CollaborativeIsSiggned { get; set; } = false;
    }
}
