using Application.Interfaces.Admin;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.Role;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Infrastructure.Attributes;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRolesService _rolePermissionService;

        public RolesController(RoleManager<ApplicationRole> roleManager, IRolesService rolePermissionService)
        {
            _roleManager = roleManager;
            _rolePermissionService = rolePermissionService;
        }
        [YesGet]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleFormVM model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Name", Resource1.PleaseInsertRole);
                return View("Index", await _roleManager.Roles.ToListAsync());
            }
            if (await _roleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError("Name", Resource1.RoleIsExists);
                return View("Index", await _roleManager.Roles.ToListAsync());
            }
            await _roleManager.CreateAsync(new ApplicationRole { Name = model.Name.Trim() });
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Manage(string roleName)
        {
            var allPermissions = PermissionScanner.GetAllActionPermissions();
            var currentPermissions = await _rolePermissionService.GetPermissionsByRoleAsync(roleName);

            ViewBag.Role = roleName;

            var model = allPermissions.Select(p => new PermissionVM
            {
                Name = p,
                Selected = currentPermissions.Contains(p)
            }).ToList();

            return View(model);
        }
        [IgnoreAction]
        [HttpPost]
        public async Task<IActionResult> Manage(string roleName, string? newRoleName, List<PermissionVM> permissions)
        {
            var selected = permissions.Where(p => p.Selected).Select(p => p.Name).ToList();
            await _rolePermissionService.UpdateRolePermissionsAsync(roleName, selected, newRoleName);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string roleName)
        {
            if(roleName == null)
                return RedirectToAction(nameof(Index));

            var allRoles = _roleManager.Roles.ToList();

            var roleNumber = allRoles.FirstOrDefault(r => r.Name == roleName)?.RoleNumber;

            if(allRoles != null && allRoles?.Count > 1 && !(roleName.Trim() == Role.SuperAdmin.ToString()) && (roleNumber == (int)RoleNumber.NormalUser || roleNumber > (int)RoleNumber.Accountant)) // RoleNumber==1 or more than 4
            {
                // will Delete This Role
                if (await _rolePermissionService.UpdateRolePermissionsAsync(roleName, new List<string>(), null))
                    await _rolePermissionService.RemoveRoleAsync(roleName);
            }
            else
            {
                // Can't Delete Last Role
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
