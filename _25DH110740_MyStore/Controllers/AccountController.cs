using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using _25DH110740_MyStore.Helpers;
using _25DH110740_MyStore.Models;
using _25DH110740_MyStore.Models.ViewModel;

namespace _25DH110740_MyStore.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet]
        public ActionResult Register() { return View(new RegisterVM()); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);
            if (db.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                return View(model);
            }
            db.Users.Add(new User { Username = model.Username, PasswordHash = PasswordHelper.Hash(model.Password), Role = "Customer" });
            db.Customers.Add(new Customer
            {
                Username = model.Username,
                CustomerName = model.CustomerName,
                CustomerEmail = model.CustomerEmail,
                CustomerPhone = model.CustomerPhone,
                CustomerAddress = model.CustomerAddress
            });
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginVM());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);
            var user = db.Users.SingleOrDefault(u => u.Username == model.Username && u.Role == "Customer");
            if (user == null || !PasswordHelper.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(model);
            }
            Session["Username"] = user.Username;
            FormsAuthentication.SetAuthCookie(user.Username, model.RememberMe);
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult ProfileInfo()
        {
            return View(BuildProfile());
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateProfile([Bind(Prefix = "Profile")] ProfileVM model)
        {
            if (!ModelState.IsValid)
            {
                var vm = BuildProfile(); vm.Profile = model;
                return View("ProfileInfo", vm);
            }
            var c = db.Customers.Single(x => x.Username == User.Identity.Name);
            c.CustomerName = model.CustomerName; c.CustomerEmail = model.CustomerEmail;
            c.CustomerPhone = model.CustomerPhone; c.CustomerAddress = model.CustomerAddress;
            db.SaveChanges();
            TempData["Message"] = "Cập nhật thông tin thành công";
            return RedirectToAction("ProfileInfo");
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Prefix = "Password")] ChangePasswordVM model)
        {
            var u = db.Users.Single(x => x.Username == User.Identity.Name);
            if (ModelState.IsValid && !PasswordHelper.Verify(model.OldPassword, u.PasswordHash))
                ModelState.AddModelError("Password.OldPassword", "Mật khẩu hiện tại không đúng");
            if (!ModelState.IsValid)
            {
                var vm = BuildProfile(); vm.Password = model;
                return View("ProfileInfo", vm);
            }
            u.PasswordHash = PasswordHelper.Hash(model.NewPassword);
            db.SaveChanges();
            TempData["Message"] = "Đổi mật khẩu thành công";
            return RedirectToAction("ProfileInfo");
        }

        private ProfileInfoVM BuildProfile()
        {
            var c = db.Customers.Single(x => x.Username == User.Identity.Name);
            return new ProfileInfoVM
            {
                Profile = new ProfileVM { Username = c.Username, CustomerName = c.CustomerName, CustomerEmail = c.CustomerEmail, CustomerPhone = c.CustomerPhone, CustomerAddress = c.CustomerAddress },
                Password = new ChangePasswordVM()
            };
        }
    }
}
