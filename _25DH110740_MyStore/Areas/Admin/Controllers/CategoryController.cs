using System.Linq;
using System.Web.Mvc;
using _25DH110740_MyStore.Models;

namespace _25DH110740_MyStore.Areas.Admin.Controllers
{
    public class CategoryController : AdminBaseController
    {
        public ActionResult Index() { return View(db.Categories.Include("Products").OrderBy(c => c.CategoryName).ToList()); }

        public ActionResult Create() { return View("Form", new Category()); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(Category c)
        {
            if (!ModelState.IsValid) return View("Form", c);
            db.Categories.Add(c); db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var c = db.Categories.Find(id);
            return c == null ? (ActionResult)HttpNotFound() : View("Form", c);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Category c)
        {
            if (!ModelState.IsValid) return View("Form", c);
            var e = db.Categories.Find(c.CategoryId);
            if (e == null) return HttpNotFound();
            e.CategoryName = c.CategoryName; db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var c = db.Categories.Include("Products").FirstOrDefault(x => x.CategoryId == id);
            if (c == null) return HttpNotFound();
            if (c.Products.Any()) { TempData["Error"] = "Không thể xóa danh mục đang có sản phẩm"; return RedirectToAction("Index"); }
            db.Categories.Remove(c); db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
