using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Infrastructure.Attributes
{
    public class MemberAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _role;
        private readonly string _requiredTokenKey;

        public MemberAuthorizeAttribute(string role = "", string requiredTokenKey = "AuthToken")
        {
            _role = role;
            _requiredTokenKey = requiredTokenKey;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var username = session.GetString("Email");
            var role = session.GetString("Role");
            var token = session.GetString(_requiredTokenKey);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (!string.IsNullOrEmpty(_role) && role != _role)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 403,
                    Content = "Forbidden: insufficient role"
                };
                return;
            }

            // Additional check: token format/validity can be added here
        }
    }

}
