using Application.Helpers;
using AutoMapper;
using Domain.Enums;
using Domain.Resources;
using Infrastructure.Identity;
using KhaledTeamRecycling.Areas.Admin.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KhaledTeamRecycling.Areas
{
    [AllowAnonymous]
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        // GET
        public IActionResult Register()
        {
            var roles = _roleManager.Roles.ToList();

            var excludedRoles = new[]
            {
                Role.Master.ToString(),
                Role.SuperAdmin.ToString()
            };

            roles = roles.Where(r => !excludedRoles.Contains(r.Name)).ToList();

            var vm = new AdminVM
            {
                RolesList = roles.Select(r => new SelectListItem
                {
                    Value = r.Id,
                    Text = r.Name
                }).ToList()
            };

            return View(vm);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Register(AdminVM model)
        {
            var roles = _roleManager.Roles.ToList();

            var excludedRoles = new[]
            {
                Role.Master.ToString(),
                Role.SuperAdmin.ToString()
            };

            roles = roles.Where(r => !excludedRoles.Contains(r.Name)).ToList();

            model.RolesList = roles.Select(r => new SelectListItem
            {
                Value = r.Id,
                Text = r.Name
            }).ToList();

            if (!ModelState.IsValid)
                return View(model);

            model.Id = Guid.NewGuid().ToString();

            var user = _mapper.Map<ApplicationUser>(model);

            var result = await _userManager.CreateAsync(user, model.PasswordHash);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            // إضافة الدور المختار
            var role = roles.FirstOrDefault(r => r.Id == model.RoleId);

            if (role != null)
                await _userManager.AddToRoleAsync(user, role.Name);

            return RedirectToAction("Index", "Home", new { area = "Admin" }); // For Login page
        }
    }
}