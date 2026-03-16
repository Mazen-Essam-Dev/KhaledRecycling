using KhaledTeamRecycling.Attributes;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace KhaledTeamRecycling.Filters
{
    public class AdminAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminAuthorizeFilter(IAuthorizationService authorizationService, SignInManager<ApplicationUser> signInManager)
        {
            _authorizationService = authorizationService;
            _signInManager = signInManager;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "Identity" });
                return;
            }

            if (!_signInManager.IsSignedIn(context.HttpContext.User))
            {
                context.Result = new ChallengeResult();
                return;
            }

            var routeValues = context.RouteData.Values;

            var controller = routeValues["controller"]?.ToString();
            var action = routeValues["action"]?.ToString();

            if (controller == null || action == null)
                return;

            // Handle AddEdit -> Add or Edit
            if (action.Equals("AddEdit", StringComparison.OrdinalIgnoreCase))
            {
                action = string.IsNullOrEmpty(routeValues["id"]?.ToString()) ? "Add" : "Edit";
            }

            // Find actual controller 
            var actualController = Assembly.GetEntryAssembly()?
             .GetTypes()
             .FirstOrDefault(t =>
                 typeof(Controller).IsAssignableFrom(t) &&
                 t.Name.Equals(controller + "Controller", StringComparison.OrdinalIgnoreCase) &&
                 t.GetCustomAttribute<AreaAttribute>()?.RouteValue == "Admin");


            if (actualController != null)
            {
                var method = actualController
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(m =>
                    string.Equals(m.Name, action, StringComparison.OrdinalIgnoreCase));

                if (method != null && method.IsDefined(typeof(IgnoreActionAttribute), true))
                {
                    return; // ✅ Allowed without policy
                }
            }

            // Handle allow user edit his data
            if (action.Equals("Edit", StringComparison.OrdinalIgnoreCase) && routeValues.ContainsKey("id"))
            {
                var userId = context.HttpContext.User.Claims.FirstOrDefault()?.Value;
                var id = routeValues["id"]?.ToString();
                if (userId != null && id != null && userId == id)
                {
                    return; // ✅ Allowed to edit own data
                }
            }

            // Build policy name
            var policyName = $"{controller}.{action}";

            // Perform authorization check
            var result = await _authorizationService.AuthorizeAsync(context.HttpContext.User, null, policyName);

            if (!result.Succeeded)
            {
                context.Result = new ForbidResult(); // Or RedirectToAction("AccessDenied")
            }
        }

    }

}
