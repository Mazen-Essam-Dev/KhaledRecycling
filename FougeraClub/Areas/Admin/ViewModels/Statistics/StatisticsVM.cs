namespace KhaledTeamRecycling.Areas.Admin.ViewModels.Statistics
{
    public class StatisticsVM
    {
        public int TotalInActivities { get; set; }
        public int TotalInCourses { get; set; }
        public int TotalPendingFinancials { get; set; }
        public int TotalUsersWithPoints { get; set; }
        public List<AgeGroupStatVM> AgeGroupStats { get; set; } = new();
        public List<CourseCategoryStatVM> CourseDepartmentCategoryStats { get; set; } = new();
        public List<GenderStatVM> GenderStats { get; set; } = new();
        public List<ResidentsAndCitizensStatVM> ResidentsAndCitizensStats { get; set; } = new();
        public List<TopUserStatVM> TopUsersByType { get; set; } = new();
        public List<UserTypeTopChartsVM> TopUsersByAmountPerType { get; set; } = new();
        public List<UserTypeTopChartsVM> TopUsersByPendingCountPerType { get; set; } = new();
        public List<SimpleChartItemVM> PendingFinancialsByUserType { get; set; } = new();
        public List<SimpleChartItemVM> PendingFinancialsBySubProduct { get; set; } = new();
        public List<SimpleChartItemVM> PendingFinancialsByMainProduct { get; set; } = new();
        public List<SimpleChartItemVM> PendingFinancialsBySubWaste { get; set; } = new();
        public List<SimpleChartItemVM> PendingFinancialsByMainWaste { get; set; } = new();
    }

    public class TopUserStatVM
    {
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public class SimpleChartItemVM
    {
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class DecimalChartItemVM
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    public class UserTypeTopChartsVM
    {
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; } = string.Empty;
        public List<DecimalChartItemVM> AmountItems { get; set; } = new();
        public List<SimpleChartItemVM> CountItems { get; set; } = new();
    }

    public class AgeGroupStatVM
    {
        public string Label { get; set; } = string.Empty;
        public float Percentage { get; set; }
        public int Count { get; set; }
    }

    public class CourseCategoryStatVM
    {
        public string Label { get; set; } = string.Empty;
        public float Percentage { get; set; }
        public int Count { get; set; }
    }

    public class GenderStatVM
    {
        public string Label { get; set; } = string.Empty;
        public float Percentage { get; set; }
        public int Count { get; set; }
    }

    public class ResidentsAndCitizensStatVM
    {
        public string Label { get; set; } = string.Empty;
        public float Percentage { get; set; }
        public int Count { get; set; }
    }
}
