using System.Web.Mvc;
using _25DH110740_MyStore.Models;

namespace _25DH110740_MyStore.Areas.Admin.Controllers
{
    // Chỉ cho phép tài khoản Admin (lưu trong Session["AdminUser"])
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            return httpContext.Session["AdminUser"] != null;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Admin/Account/Login");
        }
    }

    [AdminAuthorize]
    public abstract class AdminBaseController : Controller
    {
        protected _25DH110740_MyStoreContext db = new _25DH110740_MyStoreContext();
        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
