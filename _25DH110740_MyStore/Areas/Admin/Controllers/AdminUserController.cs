using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using _25DH110740_MyStore.Helpers;
using _25DH110740_MyStore.Models;
using PagedList;

namespace _25DH110740_MyStore.Areas.Admin.Controllers
{
    public class AdminUserVM
    {
        [Required(ErrorMessage = "Nhập tên đăng nhập"), StringLength(50, MinimumLength = 4), Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }
        [DataType(DataType.Password), Display(Name = "Mật khẩu (bỏ trống nếu không đổi)")]
        public string Password { get; set; }
        public bool IsEdit { get; set; }
    }

    public class AdminUserController : AdminBaseController
    {
        public ActionResult Index(string searchTerm, int? page)
        {
            var q = db.Users.Where(u => u.Role == "Admin");
            if (!string.IsNullOrWhiteSpace(searchTerm)) q = q.Where(u => u.Username.Contains(searchTerm));
            ViewBag.SearchTerm = searchTerm;
            return View(q.OrderBy(u => u.Username).ToPagedList(page ?? 1, 10));
        }

        public ActionResult Create() { return View("Form", new AdminUserVM()); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(AdminUserVM m)
        {
            if (string.IsNullOrWhiteSpace(m.Password) || m.Password.Length < 6) ModelState.AddModelError("Password", "Mật khẩu tối thiểu 6 ký tự");
            if (db.Users.Any(u => u.Username == m.Username)) ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
            if (!ModelState.IsValid) return View("Form", m);
            db.Users.Add(new User { Username = m.Username, PasswordHash = PasswordHelper.Hash(m.Password), Role = "Admin" });
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id)
        {
            var u = db.Users.FirstOrDefault(x => x.Username == id && x.Role == "Admin");
            return u == null ? (ActionResult)HttpNotFound() : View("Form", new AdminUserVM { Username = u.Username, IsEdit = true });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(AdminUserVM m)
        {
            var u = db.Users.FirstOrDefault(x => x.Username == m.Username && x.Role == "Admin");
            if (u == null) return HttpNotFound();
            if (!string.IsNullOrWhiteSpace(m.Password))
            {
                if (m.Password.Length < 6) { ModelState.AddModelError("Password", "Mật khẩu tối thiểu 6 ký tự"); m.IsEdit = true; return View("Form", m); }
                u.PasswordHash = PasswordHelper.Hash(m.Password); db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            if (id == (string)Session["AdminUser"]) { TempData["Error"] = "Không thể xóa tài khoản đang đăng nhập"; return RedirectToAction("Index"); }
            if (db.Users.Count(u => u.Role == "Admin") <= 1) { TempData["Error"] = "Phải còn ít nhất một tài khoản Admin"; return RedirectToAction("Index"); }
            var u = db.Users.FirstOrDefault(x => x.Username == id && x.Role == "Admin");
            if (u != null) { db.Users.Remove(u); db.SaveChanges(); }
            return RedirectToAction("Index");
        }
    }
}
