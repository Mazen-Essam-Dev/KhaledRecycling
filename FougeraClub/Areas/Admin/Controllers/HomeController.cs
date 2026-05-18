using Domain.Entities;
using Domain.Resources;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace KhaledTeamRecycling.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class HomeController : Controller
    {
        [NoLogging]
        [IgnoreAction]
        public IActionResult Index()
        {
            //return RedirectToAction("Index", "Statistics");
            return View();
        }

        // Used For Change Language [ Ar - En ]
        [IgnoreAction]
        [HttpGet]
        [NoLogging]
        public IActionResult ChangeLanguage(string lang)
        {
            HttpContext.Session.SetString("CurrentCulture", lang);

            // Redirect to previous page (Referer)
            var referer = Request.Headers["Referer"].ToString();
            return Redirect(referer ?? "/");
        }

        [IgnoreAction]
        [NoLogging]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]  
        // Error 500
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature != null)
            {
                ViewBag.Path = exceptionHandlerPathFeature.Path;
                ViewBag.Message = exceptionHandlerPathFeature.Error.Message;
            }

            return View("Error500"); // Error 500
        }

        [IgnoreAction]
        //[NoLogging]
        [YesGet]
        [AllowAnonymous]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = Resource1.pageNotFound;  //"page Not Found"
                    return View("Error404");
                case 403:
                    ViewBag.ErrorMessage =  Resource1.YouNotAuthorizedAccessPage;//"You are not authorized to access this page"
                    return View("AccessDeniedError403");
                case 401:
                    ViewBag.ErrorMessage = Resource1.YouNotAuthorizedAccessPage; // Not Loggined in now UN Authorization
                    return View("AccessDeniedError403");
                case 500:
                    return View("Error500"); // Server error
                case 503:
                    return View("Error503"); // The server is not available now
                default:
                    ViewBag.ErrorMessage = Resource1.AnUnexpectedErrorOccurred;  //"An unexpected error occurred"
                    return View("Error500");
            }
        }
        [IgnoreAction]
        //[NoLogging]
        [YesGet]
        //[AllowAnonymous] // AccessDenied
        public IActionResult AccessDeniedError403()
        {
            return View("AccessDeniedError403");
        }
        //[Route("Error")]
        //public IActionResult Error()
        //{
        //    var exceptionHandlerPathFeature =
        //        HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        //    // يمكنك تسجيل الخطأ في Logs هنا مثلاً:
        //    // _logger.LogError(exceptionHandlerPathFeature.Error, "Error at path: " + exceptionHandlerPathFeature.Path);

        //    return View("Error");
        //}
    }
}
