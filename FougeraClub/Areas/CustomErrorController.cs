using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace KhaledTeamRecycling.Areas
{
    public class CustomErrorController : Controller
    {
        [AllowAnonymous]

        [Route("CustomError/Handle")]
        public IActionResult Handle(int statusCode)
        {
            // ✅ Get the real original path before re-executing Redirection
            var originalPathFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var originalPath = originalPathFeature?.OriginalPath?.ToLower() ?? "";

            if (originalPath.Contains("/member"))
                return Redirect($"/Member/Home/HttpStatusCodeHandler?statusCode={statusCode}");

            if (originalPath.Contains("/admin") || originalPath.Contains("/identity"))
                return Redirect($"/Admin/Home/HttpStatusCodeHandler?statusCode={statusCode}");

            // fallback
            return View();
        }
        
    }
}
