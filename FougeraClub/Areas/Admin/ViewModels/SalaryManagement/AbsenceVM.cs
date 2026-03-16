namespace KhaledTeamRecycling.Areas.Admin.ViewModels.SalaryManagement
{
    public class AbsenceVM
    {
        public int Id { get; set; }
        public int? EmpId { get; set; }
        public string? FullNameAr { get; set; }
        public string? FullNameEn { get; set; }
        public string? JobTitle { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? NoOfDays { get; set; }
    }
}
