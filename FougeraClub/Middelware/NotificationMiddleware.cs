using Domain.Resources;
using Microsoft.Extensions.Localization;

namespace KhaledTeamRecycling.Middelware
{
    public class NotificationMiddleware
    {
        private readonly RequestDelegate _next;

        public NotificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IStringLocalizer<Resource2> localizer)
        {
            await _next(context); // run action first
            if (context.Request.RouteValues["action"] != null && context.Request.RouteValues["action"].ToString().Contains("Login"))
            {
                //Skip Login Action For Member Dashboard
            }
            else if (context.Request.RouteValues.Count != 0 && context.Request.RouteValues["area"] != null && !context.Request.RouteValues["area"].ToString().Contains("Identity") && context.Request.Method == HttpMethods.Post && (context.Response.StatusCode == 302 /*|| context.Response.StatusCode == 200*/))
            {
                var routeValues = context.Request.RouteValues;
                var action = routeValues["action"]?.ToString();
                var id = routeValues["id"]?.ToString();

                if (action == "AddEdit")
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        context.Session.SetString("ToastMessage", "ToastAdd");
                        context.Session.SetString("ToastType", "success");
                        context.Session.SetString("ToastKey", "ToastAdd");
                    }
                    else
                    {
                        context.Session.SetString("ToastMessage", "ToastEdit");
                        context.Session.SetString("ToastType", "success");
                        context.Session.SetString("ToastKey", "ToastEdit");
                    }
                }
                else if (action == "Create")
                {
                    context.Session.SetString("ToastMessage", "ToastAdd");
                    context.Session.SetString("ToastType", "success");
                    context.Session.SetString("ToastKey", "ToastAdd");
                }
                else if (action == "Edit")
                {
                    context.Session.SetString("ToastMessage", "ToastEdit");
                    context.Session.SetString("ToastType", "success");
                    context.Session.SetString("ToastKey", "ToastEdit");
                }
                else if (action == "Delete")
                {
                    context.Session.SetString("ToastMessage", "ToastDone");
                    context.Session.SetString("ToastType", "success");
                    context.Session.SetString("ToastKey", "ToastAdd");
                }
                else
                {
                    context.Session.SetString("ToastMessage", "ToastDone");
                    context.Session.SetString("ToastType", "success");
                    context.Session.SetString("ToastKey", "ToastEdit");
                }
            }
            else if (context.Response.StatusCode == 500)
            {
                context.Session.SetString("ToastMessage", "ToastFailed");
                context.Session.SetString("ToastType", "error");
                context.Session.SetString("ToastKey", "ToastDelete");
            }
        }
    }
}
