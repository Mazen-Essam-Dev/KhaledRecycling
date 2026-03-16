namespace KhaledTeamRecycling.Areas.Admin.ViewModels.Statistics
{
    public class StatisticsVM
    {
        public int TotalInActivities { get; set; }
        public int TotalInCourses { get; set; }
        public List<AgeGroupStatVM> AgeGroupStats { get; set; }
        public List<CourseCategoryStatVM> CourseDepartmentCategoryStats { get; set; }
        public List<GenderStatVM> GenderStats { get; set; }
        public List<ResidentsAndCitizensStatVM> ResidentsAndCitizensStats { get; set; }
    }
        

    public class AgeGroupStatVM
    {
        public string Label { get; set; }
        public float Percentage { get; set; }
        public int Count { get; set; }
    }

    public class CourseCategoryStatVM
    {
        public string Label { get; set; }
        public float Percentage { get; set; }
        public int Count { get; set; }
    }

    public class GenderStatVM
    {
        public string Label { get; set; }
        public float Percentage { get; set; }
        public int Count { get; set; }
    }
    public class ResidentsAndCitizensStatVM
    {
        public string Label { get; set; }
        public float Percentage { get; set; }
        public int Count { get; set; }
    }

}
