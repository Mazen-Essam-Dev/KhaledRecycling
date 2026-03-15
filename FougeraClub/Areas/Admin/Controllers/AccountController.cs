using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.Account;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Infrastructure.Attributes;
using Infrastructure.Identity;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class AccountController : Controller
    {
        #region properties
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAccountService _accountService;
        private readonly ITrainerService _trainerService;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region constructor
        public AccountController(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<ApplicationRole> roleManager, IAccountService accountService, ITrainerService trainerService,IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _accountService = accountService;
            _trainerService = trainerService;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region actions
        // List All Users
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 50)
        {
            var users = _userManager.Users.ToList();
            var model = _mapper.Map<List<AdminVM>>(users);

            var Roles = _roleManager.Roles.ToList();
            foreach (var user in model)
            {
                var isTrainer = await _trainerService.GetThisTrainerId_IfTrainer_else_0(user.Username);
                user.IsTrainer = isTrainer.HasValue && isTrainer.Value != 0;
                var userRoles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.Id));
                user.Role = userRoles.FirstOrDefault();
                user.RoleNumber = Roles.FirstOrDefault(r => r.Name == user.Role)?.RoleNumber;
            }

            // Filtering
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                model = model.Where(c =>
                    (!string.IsNullOrEmpty(c.FullNameAr) && c.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.FullNameEn) && c.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Username) && c.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Email) && c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).OrderByDescending(m=>m.Id).ToList();
            }


            var paginated = PaginatedList<AdminVM>.Create(model, page, pageSize, searchTerm);

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }


        public async Task<IActionResult> AddEdit(string? id)
        {
            var allRoles = _roleManager.Roles.ToList();

            var vm = new AdminVM();
            if (!string.IsNullOrEmpty(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();
                vm = _mapper.Map<AdminVM>(user);

                var userRole = await _userManager.GetRolesAsync(user);
                vm.RoleId = allRoles
                    .Where(r => userRole.Contains(r.Name))
                    .Select(r => r.Id)
                    .FirstOrDefault();

                // Retrieve the latest signature for the user
                var allSignatures = await _accountService.GetAllSignaturesAsync(id);
                var latestSignature = allSignatures
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefault();
                vm.Signature = _mapper.Map<SignatureVM>(latestSignature);
            }

            vm.RolesList = SelectListHelper.BindSelectListIdString(allRoles, vm.RoleId, "Id", "Name", "Name");

            if (vm.PhoneNumber != null && vm.PhoneNumber.StartsWith("971")) // Is Phone StartsWith 971 Remove it
                vm.PhoneNumber = vm.PhoneNumber.Substring(3);

            return View(vm);
        }


        // Add or Edit (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(AdminVM model)
        {
            var allRoles = _roleManager.Roles.ToList();
            model.RolesList = SelectListHelper.BindSelectListIdString(allRoles, model.RoleId, "Id", "Name", "Name");

            bool isAdd = string.IsNullOrEmpty(model.Id);
            if (isAdd)
            {
                if (string.IsNullOrWhiteSpace(model.PasswordHash))
                    ModelState.AddModelError(nameof(model.PasswordHash), Resource1.PasswordRequired);

                if (string.IsNullOrWhiteSpace(model.ConfirmPassword))
                    ModelState.AddModelError(nameof(model.ConfirmPassword), Resource1.PasswordRequiredConfirm);
            }

            if (!ModelState.IsValid || model==null)
                return View(model);

            //// //Phone Dubai
            //model.PhoneNumber = model.PhoneNumber?.Replace(" ", "");
            //if (!string.IsNullOrEmpty(model.PhoneNumber) && model.PhoneNumber.StartsWith("0")){model.PhoneNumber = model.PhoneNumber.Substring(1);}
            //if (model.PhoneNumber != null && !model.PhoneNumber.StartsWith("971"))
            //{
            //    if (!string.IsNullOrEmpty(model.PhoneNumber) && model.PhoneNumber.StartsWith("9710")) { model.PhoneNumber = model.PhoneNumber.Substring(4); }
            //    model.PhoneNumber = "971" + model.PhoneNumber;
            //}
            // //Phone Dubai
            model.PhoneNumber = await PhoneHelper.CheckAndDoPhoneStart971(model.PhoneNumber);

            if (string.IsNullOrEmpty(model.Id)) // Create
            {
                model.Id = Guid.NewGuid().ToString();
                var newUser = _mapper.Map<ApplicationUser>(model);

                var result = await _userManager.CreateAsync(newUser, model.PasswordHash);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                    model.Id = "";
                    if (model.PhoneNumber != null && model.PhoneNumber.StartsWith("971")) // Is Phone StartsWith 971 Remove it
                        model.PhoneNumber = model.PhoneNumber.Substring(3);

                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.RoleId))
                {
                    var roleName = model.RolesList.FirstOrDefault(r => r.Selected)?.Text;
                    if (string.IsNullOrEmpty(roleName))
                        ModelState.AddModelError(nameof(model.RoleId), Resource1.RoleRequired);
                    else
                        await _userManager.AddToRoleAsync(newUser, roleName);
                }
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else // Update
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                    return NotFound();


                // Do not copy PasswordHash from the model because this field is for the encrypted password
                var oldPasswordHash = user.PasswordHash; // Keep the old password

                _mapper.Map(model, user);
                user.PasswordHash = oldPasswordHash; // Do not change it unless the user enters a new password

                // If User Update Password
                if (!string.IsNullOrEmpty(model.PasswordHash))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.PasswordHash);

                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                            ModelState.AddModelError("", error.Description);

                        if (model.PhoneNumber != null && model.PhoneNumber.StartsWith("971")) // Is Phone StartsWith 971 Remove it
                            model.PhoneNumber = model.PhoneNumber.Substring(3);

                        return View(model);
                    }
                }

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    if (model.PhoneNumber != null && model.PhoneNumber.StartsWith("971")) // Is Phone StartsWith 971 Remove it
                        model.PhoneNumber = model.PhoneNumber.Substring(3);

                    return View(model);
                }

                // Update Roles
                var existingRole = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, existingRole);

                var selectedRole = model.RolesList
                    .Where(r => r.Selected)
                    .Select(r => r.Text)
                    .ToList();


                await _userManager.AddToRolesAsync(user, selectedRole);

                return RedirectToAction(nameof(AddEdit), new { id = model.Id });  // After Edit 
            }

        }


        // Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var Roles = _roleManager.Roles.ToList();

            var userRoles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.Id));
            var thisRoleName = userRoles.FirstOrDefault();
            var roleNumber = Roles.FirstOrDefault(r => r.Name == thisRoleName)?.RoleNumber;

            var isTrainer2 = await _trainerService.GetThisTrainerId_IfTrainer_else_0(user.UserName);
            bool isTrainer = isTrainer2.HasValue && isTrainer2.Value != 0;

            var loggedInUserId = @User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (loggedInUserId != user.Id && !isTrainer && !(thisRoleName == Role.SuperAdmin.ToString()) && (roleNumber == (int)RoleNumber.NormalUser || roleNumber > (int)RoleNumber.Accountant)) // RoleNumber==1 or more than 4
            {
                var signatures = await  _unitOfWork.Signatures.Table.Where(s => s.UserId == user.Id).ToListAsync();
                foreach (var sign in signatures)
                {
                    FileHelper.DeleteImageFile(sign.ImagePath);
                }
                _unitOfWork.Signatures.RemoveRange(signatures);
                await _unitOfWork.CompleteAsync();
                await _userManager.DeleteAsync(user); 
            }

            return RedirectToAction(nameof(Index));
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm)
        {
            var users = _userManager.Users.ToList();
            var model = _mapper.Map<List<AdminVM>>(users);
            // Filtering
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                model = model.Where(c =>
                    (!string.IsNullOrEmpty(c.FullNameAr) && c.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.FullNameEn) && c.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Username) && c.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Email) && c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).OrderByDescending(m => m.Id).ToList();
            }
            foreach (var item in model)
            {
                var user = await _userManager.FindByIdAsync(item.Id);
                if (user == null) return NotFound();

                var userRoles = await _userManager.GetRolesAsync(user);
                item.Role = userRoles.FirstOrDefault();
            }

            return View(model);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo)
        {
            // ---- Start Get Data As Print
            var users = _userManager.Users.ToList();
            var model = _mapper.Map<List<AdminVM>>(users);
            // Filtering
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                model = model.Where(c =>
                    (!string.IsNullOrEmpty(c.FullNameAr) && c.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.FullNameEn) && c.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Username) && c.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Email) && c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).OrderByDescending(m => m.Id).ToList();
            }
            foreach (var item in model)
            {
                var user = await _userManager.FindByIdAsync(item.Id);
                if (user == null) return NotFound();

                var userRoles = await _userManager.GetRolesAsync(user);
                item.Role = userRoles.FirstOrDefault();
            }
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                var lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = model;
                var ListTitles = new List<string>
        {
            Resource2.FullName,Resource2.Username,Resource2.Email,Resource2.PhoneNumber,Resource2.Role
        };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = (lang == "ar" ? single.FullNameAr : single.FullNameEn),
                        t2 = single.Username,
                        t3 = single.Email,
                        t4 = single.PhoneNumber,
                        t5 = single.Role,
                    }).ToList();

                    if (lang == "ar")
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_<ExcelDataDTO>(excelDataDTO, ListTitles, 0, "ar");
                    }
                    else
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_<ExcelDataDTO>(excelDataDTO, ListTitles, 0, "en");
                    }
                }

                FileContentResult? Excelfile = null;
                if (fileBytes != null && fileBytes.Length > 0 && boolStatus == true)
                {
                    var fileExcelName = Resource1.UsersList;
                    Excelfile = File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"{fileExcelName}_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
                }
                return Excelfile;


            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveSignature(SignatureVM model)
        {
            if (model == null) return Json(new { success = false, message = Resource1.ErrorOccurred });

            var SignatureFile_Text = await FileHelper.CheckFileIsImage_3Mg_Async(model.SignatureFile);
            if (SignatureFile_Text != "OK" && SignatureFile_Text != "null") ModelState.AddModelError("SignatureFile", SignatureFile_Text);

            if (!ModelState.IsValid)
                return Json(new { success = false, message = SignatureFile_Text });

            var signature = _mapper.Map<Signature>(model);
            var result = await _accountService.SaveSignatureAsync(signature);
            if (result)
                return Json(new { success = result, message = Resource2.Done });
            return Json(new { success = false, message = Resource1.ErrorOccurred });
        }

        public async Task<IActionResult> ResetPassword(string? id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            var vm = _mapper.Map<ResetPasswordVM>(user);

            return View(vm);
        }
        [HttpPost]
        [IgnoreAction]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // Generate reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset password
            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region remote attributes
        [AcceptVerbs("GET", "POST")]
        [IgnoreAction]
        public async Task<IActionResult> CheckUsernameIfExists(string username, string id)
        {
            bool exists = await _accountService.CheckUsernameIfExistsAsync(username, id);

            if (exists)
                return Json(Resource1.UsernameAlreadyExists);

            return Json(true);
        }
        #endregion

    }
}
