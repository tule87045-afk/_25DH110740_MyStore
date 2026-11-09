using System.Linq;
using System.Web.Mvc;
using _25DH110740_MyStore.Helpers;
using _25DH110740_MyStore.Models;
using _25DH110740_MyStore.Models.ViewModel;

namespace _25DH110740_MyStore.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login() { return View(new LoginVM()); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);
            using (var db = new _25DH110740_MyStoreContext())
            {
                var u = db.Users.SingleOrDefault(x => x.Username == model.Username && x.Role == "Admin");
                if (u == null || !PasswordHelper.Verify(model.Password, u.PasswordHash))
                {
                    ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu quản trị");
                    return View(model);
                }
                Session["AdminUser"] = u.Username;
            }
            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult Logout()
        {
            Session.Remove("AdminUser");
            return RedirectToAction("Login");
        }
    }
}
