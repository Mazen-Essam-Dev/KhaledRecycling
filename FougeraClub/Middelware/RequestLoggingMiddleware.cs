using Azure.Core;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entities;
using Domain.Enums;
using Domain.Resources;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace KhaledTeamRecycling.Middelware
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NoLoggingAttribute : Attribute
    {
    }
    public class YesGetAttribute : Attribute
    {
    }

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext db)
        {
            // // use it for add Target Id For Logging
            // HttpContext.Items["LogTarget"] = "User created successfully with id 5";

            try
            {
                // Get Route Data (Controller + Action)
                var endpoint = context.GetEndpoint();

                // Check if [NoLogging] is applied
                if (endpoint?.Metadata.GetMetadata<NoLoggingAttribute>() != null)
                {
                    await _next(context); // skip logging
                    return;
                }
                var AreaName = context.Request.RouteValues["area"]?.ToString();
                string? userId;
                

                if (AreaName =="Member")
                {
                    // Get UserId if (Member) from Session
                    var sessionMember = _httpContextAccessor?.HttpContext?.Session;
                    var Member_username = sessionMember?.GetString("Email");
                    userId = Member_username;
                    //var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Email == username);
                }
                else  // Get UserId from Identity
                {
                    // Get UserId from Identity
                    userId = context.User?.Identity?.IsAuthenticated == true
                    ? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    : null;

                    // if no user Loggined skip logging
                    if (userId is null) { await _next(context); return; } // Continue pipeline skip logging

                    #region Get And Check if This Role is "Master" And Skipping Logging
                        // Get His Role From DB
                        var thisRole = await db.UserRoles.FirstOrDefaultAsync(u => u.UserId == userId);
                        if (thisRole is null) { await _next(context); return; } // Continue pipeline skip logging As No Role 
                        var role = await db.Roles.FirstOrDefaultAsync(r => r.Id == thisRole.RoleId);
                        if (role is null) { await _next(context); return; } // Continue pipeline skip logging As No Role 
                        if (role != null && role.Name == Role.Master.ToString()) { await _next(context); return; } // Continue pipeline skip logging As Role is ("Master")
                    #endregion Get And Check if This Role is "Master" And Skipping Logging

                }

                // if no user Loggined skip logging
                if (userId is null) { await _next(context); return; } // Continue pipeline skip logging

                // Continue pipeline first to get status code
                await _next(context);

                // Not Enter if Has .... But Continue this Save without return;
                if (!(endpoint != null && endpoint.DisplayName != null && 
                    (
                    endpoint.DisplayName.ToString().Contains("CourseController.Attendance") || endpoint.DisplayName.ToString().Contains("CourseController.Rate")
                    ) 
                    ))
                {
                    // If response status code not 200 skip logging
                    if (context.Response.StatusCode != 200 && context.Response.StatusCode != 302)
                    {
                        return;
                    }

                    // if Method is 'Get' skip logging but logging only if it has [YesGet] Attribute -- And Ignore Logging for SignalR Hubs Notifications 
                    if ((context.Request.Method.ToLower() == "get" && endpoint?.Metadata.GetMetadata<YesGetAttribute>() == null) || (endpoint != null && endpoint.DisplayName != null && endpoint.DisplayName.ToString().ToLower().Contains("/hubs/notifications")))
                    {
                        return;  // skip logging
                    }
                }
                

                string? controllerName = null;
                string? actionName = null;
                string? routeId = null;

                if (endpoint != null)
                {
                    var routeValues = context.Request.RouteValues;
                    controllerName = routeValues["controller"]?.ToString();
                    actionName = routeValues["action"]?.ToString();
                    routeId = routeValues["id"]?.ToString();
                }
                var thisPath = context.Request.Path;
                if ((controllerName is null || controllerName == "" || actionName is null) && thisPath != null)
                {
                    actionName = thisPath.ToString().Split('/').LastOrDefault();
                    if (actionName?.Contains('?') ?? false) actionName = actionName?.Split('?')[0];
                    controllerName = thisPath.ToString().Replace(actionName ?? "", "").Remove(0, 1).Replace("/", ".");
                    if (controllerName.EndsWith(".")) controllerName = controllerName.Remove(controllerName.LastIndexOf("."), 1);

                    // Extract last segment as route value (could be ID)
                    var segments = thisPath.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries);
                    routeId = segments.LastOrDefault();
                }

                //// all get methods Named in Resource file with "view.ControllerName.ActionName"
                //if (context.Request.Method.ToLower() == "get")
                //{
                //    controllerName = "view." + controllerName;
                //}

                var culture_ar = new CultureInfo("ar"); // ex: "ar" أو "en"
                var culture_en = new CultureInfo("en"); // ex: "ar" أو "en"


                // Read the log Target ID value from HttpContext.Items
                string? logTarget = context.Items["LogTarget"]?.ToString();

                var editRequest = false;
                if (routeId != "" && routeId != null && actionName == "AddEdit")
                {
                    editRequest = true;
                }
                // Create log entry
                var log = new RequestLog
                {
                    Path = thisPath,
                    Method = context.Request.Method,
                    Controller = controllerName,
                    Action = editRequest ? "Edit" : actionName, // Action Name
                    UserId = userId,
                    LogTarget = logTarget,
                    RequestTime = DateTime.UtcNow
                };
                var titleKey = editRequest ? "Edit" : actionName;

                #region old Ar,En in db
                //// Page title from the translation
                //log.NameAr =
                //    (Resource1.ResourceManager.GetString(controllerName + "s", culture_ar)
                //        ?? Resource2.ResourceManager.GetString(controllerName + "s", culture_ar))
                //    + " . " +
                //    (Resource1.ResourceManager.GetString(titleKey, culture_ar)
                //        ?? Resource2.ResourceManager.GetString(titleKey, culture_ar));

                //// Page title from the translation
                //log.NameEn =
                //    (Resource1.ResourceManager.GetString(controllerName + "s", culture_en)
                //        ?? Resource2.ResourceManager.GetString(controllerName + "s", culture_en))
                //    + " . " +
                //    (Resource1.ResourceManager.GetString(titleKey, culture_en)
                //        ?? Resource2.ResourceManager.GetString(titleKey, culture_en));
                #endregion old Ar,En in db

                #region New Ar,En in db
                // Page title from the translation
                string key_controllerName = controllerName;

                //Exceptions
                var sessionMember2 = _httpContextAccessor?.HttpContext?.Session;
                var Member_isLoginSuccesfully = sessionMember2?.GetString("ShowToastrLoginSuccesfullyLoggedIn");
                if (Member_isLoginSuccesfully == "True")
                {
                    key_controllerName = "-";
                    titleKey = "Login";
                    _httpContextAccessor?.HttpContext?.Session.SetString("ShowToastrLoginSuccesfullyLoggedIn", "false");
                }
                var admin_isLoginSuccesfully = sessionMember2?.GetString("ShowToastrLoginSuccesfullyLoggedIn_Admin");
                if (admin_isLoginSuccesfully == "True")
                {
                    key_controllerName = "-";
                    titleKey = "Login";
                    _httpContextAccessor?.HttpContext?.Session.SetString("ShowToastrLoginSuccesfullyLoggedIn_Admin", "false");
                }
                if (AreaName== "Member" && controllerName== "Account")
                {
                    if(actionName == "Login")
                    {
                        key_controllerName = "-";
                        titleKey = "Login";
                    }
                    else if (actionName == "Logout")
                    {
                        key_controllerName = "-";
                        titleKey = "Logout3";
                    }
                    else if(actionName == "Edit")
                    {
                        key_controllerName = "MemberPageDate";
                        titleKey = "UpdateMyData";
                    }
                }


                    if (!string.IsNullOrEmpty(controllerName) && controllerName.Contains("."))
                {
                    key_controllerName = controllerName.Substring(0, controllerName.IndexOf('.'));
                }

                log.NameAr =
                    (Resource2.ResourceManager.GetString("PerformedAnOperation", culture_ar))+" "+

                    (Resource1.ResourceManager.GetString(titleKey, culture_ar)
                      ?? Resource2.ResourceManager.GetString(titleKey, culture_ar)) + " " +

                     (Resource2.ResourceManager.GetString("InPage", culture_ar)) + " " +

                     (Resource1.ResourceManager.GetString(key_controllerName, culture_ar)??
                     Resource1.ResourceManager.GetString(key_controllerName+"s", culture_ar)??
                     Resource2.ResourceManager.GetString(key_controllerName, culture_ar) ??
                     Resource2.ResourceManager.GetString(key_controllerName + "s", culture_ar));

                // Page title from the translation
                log.NameEn =
                    (Resource2.ResourceManager.GetString("PerformedAnOperation", culture_en)) + " " +

                    (Resource1.ResourceManager.GetString(titleKey, culture_en)
                        ?? Resource2.ResourceManager.GetString(titleKey, culture_en)) + " " +

                    (Resource2.ResourceManager.GetString("InPage", culture_en)) + " " +

                     (Resource1.ResourceManager.GetString(key_controllerName, culture_en) ??
                     Resource1.ResourceManager.GetString(key_controllerName + "s", culture_en) ??
                     Resource2.ResourceManager.GetString(key_controllerName, culture_en) ??
                     Resource2.ResourceManager.GetString(key_controllerName + "s", culture_en));

                #endregion New Ar,En in db

                // Don't log generic error/status pages (avoid noisy "عرض صفحة خطأ" entries)
                var isErrorPageLogging = string.Equals(actionName, "HttpStatusCodeHandler", StringComparison.OrdinalIgnoreCase)
                                     || string.Equals(actionName, "Error", StringComparison.OrdinalIgnoreCase)
                                     || (actionName != null && actionName.ToLower().Contains("error"));

                if (isErrorPageLogging)
                {
                    return; // skip saving this log
                }

                // Save to DB
                db.RequestLogs.Add(log);
                await db.SaveChangesAsync();

        }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RequestLoggingMiddleware");
                await _next(context); // Even if an error occurs, complete the pipeline
            }
}
    }
}
