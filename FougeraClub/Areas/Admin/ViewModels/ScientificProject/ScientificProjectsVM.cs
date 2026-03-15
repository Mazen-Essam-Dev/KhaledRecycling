using Domain.Entities;
using Domain.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.ScientificProject
{
    public class ScientificProjectsVM
    {
        // Display full names under signatures
        public string? SupervisorFullName { get; set; }
        public string? TrainerFullName { get; set; }
        public string? ManagerFullName { get; set; }

        // to check if trainer is loggined for showing sign button
        public bool IsTrainerLogginedForSign { get; set; } = false;

        // for index page 
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public IEnumerable<ScientificProjects>? all_ScientificProjectListVM { get; set; }
        public string? SearchString { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int? TotalCount { get; set; }
        public int PageSize { get; set; }

        // end index

        public string? CurrentDate { get; set; }
        public int? Id { get; set; }

        [LocalizedRequired("Required")]
        public int? TrainerId { get; set; }
        public virtual Trainer? Trainer { get; set; }
        public List<SelectListItem>? TrainersList { get; set; } = new();
        public List<SelectListItem>? DepartmentsList { get; set; } = new();

        [MaxLength(50)]
        public string? SerialCode { get; set; }
        public DateOnly? DateByCalander { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")]
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }

        public IEnumerable<Department>? all_Departments { get; set; }
        public Department? SingleDepartment { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")
        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        //[RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        public string? ProjectNameAr { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")
        , MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        //[RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        public string? ProjectNameEn { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")
        //, MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        //[RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        public string? IdeaOwnerAr { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")
        //, MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        //[RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        public string? IdeaOwnerEn { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")
        , MaxLength(500, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_500")]
        [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        public string? ProjectIdeaAr { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")
        , MaxLength(500, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_500")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        public string? ProjectIdeaEN { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? ProjectElement1 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? ProjectElement2 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? ProjectElement3 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? ProjectElement4 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? ProjectElement5 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? ProjectElement6 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? ProjectElement7 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? ProjectElement8 { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")]
        [MaxLength(500, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_500")]
        public string? InstallationRecommendations { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")]
        [MaxLength(500, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_500")]
        public string? DeliveryData { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? Participant1 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? Participant2 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? Participant3 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? Participant4 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? Supervisor1 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? Supervisor2 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? Supervisor3 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? Supervisor4 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? Supervisor5 { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? Supervisor6 { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")]
        public double? ExpectedCost { get; set; }

        public IFormFile? File1 { get; set; }
        public string? FilePath1 { get; set; }

        public IFormFile? File2 { get; set; }
        public string? FilePath2 { get; set; }

        public IFormFile? File3 { get; set; }
        public string? FilePath3 { get; set; }

        public IFormFile? File4 { get; set; }
        public string? FilePath4 { get; set; }

        public IFormFile? File5 { get; set; }
        public string? FilePath5 { get; set; }

        public IFormFile? File6 { get; set; }
        public string? FilePath6 { get; set; }

        public IFormFile? FilePreliminary1 { get; set; }
        public string? FilePreliminaryPath1 { get; set; }

        public IFormFile? FilePreliminary2 { get; set; }
        public string? FilePreliminaryPath2 { get; set; }

        public IFormFile? FilePreliminary3 { get; set; }
        public string? FilePreliminaryPath3 { get; set; }

        // Temporary uploaded file 
        public string? TempFilePreliminary1_Path { get; set; }
        public string? TempFilePreliminary2_Path { get; set; }
        public string? TempFilePreliminary3_Path { get; set; }
        public string? TempFile1_Path { get; set; }
        public string? TempFile2_Path { get; set; }
        public string? TempFile3_Path { get; set; }
        public string? TempFile4_Path { get; set; }
        public string? TempFile5_Path { get; set; }
        public string? TempFile6_Path { get; set; }

        // For Edit: store old file from DB
        public string? OldFilePreliminary1_Path { get; set; }
        public string? OldFilePreliminary2_Path { get; set; }
        public string? OldFilePreliminary3_Path { get; set; }
        public string? Old1_Path { get; set; }
        public string? Old2_Path { get; set; }
        public string? Old3_Path { get; set; }
        public string? Old4_Path { get; set; }
        public string? Old5_Path { get; set; }
        public string? Old6_Path { get; set; }

        [MaxLength(500, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_500")]
        public string? Notes { get; set; }

        public int? ActivityMonitorSignitureId { get; set; }
        //[ForeignKey(nameof(ActivityMonitorSignitureId))]
        public virtual Signature? ActivityMonitorSigniture { get; set; }
        public int? ManagerSignitureId { get; set; }
        //[ForeignKey(nameof(ManagerSignitureId))]
        public virtual Signature? ManagerSignature { get; set; }

        public int? Trainer1SignitureId { get; set; }
        //[ForeignKey(nameof(Trainer1SignitureId))]
        public virtual Signature? Trainer1Signiture { get; set; }

        //[LocalizedRequired("Required")]
        [LocalizedMaxLength(100, "MaxLength_100")]
        [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        public string? Innovation_Individual_TeamAr { get; set; }

        //[LocalizedRequired("Required")]
        [LocalizedMaxLength(100, "MaxLength_100")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        public string? Innovation_Individual_TeamEn { get; set; }


        [LocalizedRequired("Required")]
        [LocalizedMaxLength(100, "MaxLength_100")]
        [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        public string? Student_EmployeeAr { get; set; }

        [LocalizedRequired("Required")]
        [LocalizedMaxLength(100, "MaxLength_100")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        public string? Student_EmployeeEn { get; set; }



        [LocalizedRequired("Required")]
        [LocalizedMaxLength(200, "MaxLength_200")]
        [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        public string? InstitutionAr { get; set; }

        [LocalizedRequired("Required")]
        [LocalizedMaxLength(200, "MaxLength_200")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        public string? InstitutionEn { get; set; }


        [LocalizedRequired("Required")]
        [LocalizedMaxLength(200, "MaxLength_200")]
        [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        public string? OrganizationAr { get; set; }
        [LocalizedRequired("Required")]
        [LocalizedMaxLength(200, "MaxLength_200")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        public string? OrganizationEn { get; set; }


        [LocalizedRequired("Required")]
        public double? Sponsorship { get; set; }

        [LocalizedRequired("Required")]
        [LocalizedMaxLength(250, "MaxLength_250")]
        [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        public string? ApplicationAr { get; set; }

        [LocalizedRequired("Required")]
        [LocalizedMaxLength(250, "MaxLength_250")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        public string? ApplicationEn { get; set; }


        public DateOnly? ClosedDate { get; set; }

        public virtual ICollection<ScientificProjectGoals>? ScientificProjectGoalsNavigation { get; set; }
        public virtual ICollection<ScientificProjectTools>? ScientificProjectToolsNavigation { get; set; }
        public virtual ICollection<ScientificProjectIndividuals>? ScientificProjectIndividualsNavigation { get; set; }
        public List<ScientificProjectGoals>? GoalsListAr { get; set; }
        public List<ScientificProjectGoals>? GoalsListEn { get; set; }
        public List<ScientificProjectTools>? ToolsListAr { get; set; }
        public List<ScientificProjectTools>? ToolsListEn { get; set; }
        public List<ScientificProjectIndividuals>? IndividualsListAr { get; set; }
        public List<ScientificProjectIndividuals>? IndividualsListEn { get; set; }

        public int? MaxRowsGoals { get; set; }
        public int? MaxRowsTools { get; set; }
        public int? MaxRowsIndividuals { get; set; }
    }
}