namespace FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual;
public class PieChartVM
{
    public List<string> MonthsLables { get; set; } = new List<string>();
    public List<int> Month1NoOfParticipations { get; set; } = new List<int>();
    public List<int> Month2NoOfParticipations { get; set; } = new List<int>();
    public List<int> Month3NoOfParticipations { get; set; } = new List<int>();

}