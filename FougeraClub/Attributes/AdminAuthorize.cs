using FougeraClub.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FougeraClub.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AdminAuthorizeAttribute : TypeFilterAttribute
    {
        public AdminAuthorizeAttribute() : base(typeof(AdminAuthorizeFilter))
        {
        }
    }
}
