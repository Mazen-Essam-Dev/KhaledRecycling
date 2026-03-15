using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Admin
{
    public class StatisticsDTO
    {
        public int TotalInActivities { get; set; }
        public int TotalInCourses { get; set; }

        public List<AgeGroupStat> AgeGroupStats { get; set; }
        public List<CourseCategoryStat> CourseDepartmentCategoryStats { get; set; }
        public List<GenderStat> GenderStats { get; set; }
        public List<ResidentsAndCitizensStat> ResidentsAndCitizensStats { get; set; }

    }

    public class AgeGroupStat
    {
        public string Label { get; set; }
        public float Percentage { get; set; }
        public int Count { get; set; }
    }

    public class CourseCategoryStat
    {
        public string Label { get; set; }
        public float Percentage { get; set; }
        public int Count { get; set; }
    }

    public class GenderStat
    {
        public string Label { get; set; }
        public float Percentage { get; set; }
        public int Count { get; set; }
    }
    public class ResidentsAndCitizensStat
    {
        public string Label { get; set; }
        public float Percentage { get; set; }
        public int Count { get; set; }
    }

}


