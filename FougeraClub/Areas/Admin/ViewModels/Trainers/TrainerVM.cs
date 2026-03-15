using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.Course;
using FougeraClub.Helpers;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.Trainers;

public class IndexCoursesVM
{
    public bool UserIsTrainer { get; set; }
    public PaginatedList<CourseVM>? CoursesVM_Paginated { get; set; }
    public IQueryable<CourseVM>? CoursesVM { get; set; }
    public List<TrainersNameVM>? TrainersNameVM_List { get; set; }
    [LocalizedRequired("Required")]
    public int trainerSelect { get; set; }
    public Trainer? Trainer { get; set; }
    public List<SelectListItem>? TrainersList { get; set; } = new();

}

public class TrainersNameVM
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? FullNameAr { get; set; }
    public string? FullNameEn { get; set; }
    public string? Email { get; set; }
    public int? DepartmentId { get; set; }
    public string? PhoneNumber { get; set; }
}

public class TrainerIndexVM
{
    public PaginatedList<TrainerWithUserVM>? TrainerWithUserVM_Paginated { get; set; }
    public List<Department>? Departments { get; set; }
}

public class TrainerWithUserVM
{
    public Trainer Trainer { get; set; }
    public ApplicationUser User { get; set; }
}

public class TrainerVM
{
    public int Id { get; set; }
    [LocalizedRequired("Required")]
    public int DepartmentId { get; set; }
    public virtual Department? Department { get; set; }
    public List<SelectListItem>? DepartmentsList { get; set; } = new();
    public virtual ApplicationUser? User { get; set; }
    [LocalizedRequired("Required")]
    public string UserId { get; set; }
    public List<SelectListItem>? UsersList { get; set; } = new();
    public string? Bio { get; set; }
    [MaxLength(255)]
    public string? AttachmentPath { get; set; }
    public IFormFile? Attachment { get; set; }

    // Temporary uploaded file
    public string? Attachment_TempFilePath { get; set; }

    // For Edit: store old file from DB
    public string? Attachment_OldPath { get; set; }
}





