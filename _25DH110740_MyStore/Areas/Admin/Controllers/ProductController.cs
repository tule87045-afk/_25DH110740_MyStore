using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _25DH110740_MyStore.Models;
using PagedList;

namespace _25DH110740_MyStore.Areas.Admin.Controllers
{
    public class ProductController : AdminBaseController
    {
        public ActionResult Index(string searchTerm, string sortOrder, int? categoryId, int? page)
        {
            var q = db.Products.Include("Category").AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm)) q = q.Where(p => p.ProductName.Contains(searchTerm) || p.Brand.Contains(searchTerm));
            if (categoryId.HasValue) q = q.Where(p => p.CategoryId == categoryId.Value);
            switch (sortOrder)
            {
                case "name": q = q.OrderBy(p => p.ProductName); break;
                case "name_desc": q = q.OrderByDescending(p => p.ProductName); break;
                case "price": q = q.OrderBy(p => p.Price); break;
                case "price_desc": q = q.OrderByDescending(p => p.Price); break;
                case "sold_desc": q = q.OrderByDescending(p => p.Sold); break;
                case "stock": q = q.OrderBy(p => p.Stock); break;
                default: q = q.OrderByDescending(p => p.ProductId); break;
            }
            ViewBag.SearchTerm = searchTerm; ViewBag.SortOrder = sortOrder; ViewBag.CategoryId = categoryId;
            ViewBag.Categories = db.Categories.OrderBy(c => c.CategoryName).ToList();
            return View(q.ToPagedList(page ?? 1, 8));
        }

        private void LoadCats(int? selected) { ViewBag.CategoryId = new SelectList(db.Categories.OrderBy(c => c.CategoryName), "CategoryId", "CategoryName", selected); }

        public ActionResult Create() { LoadCats(null); return View("Form", new Product()); }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(Product p, HttpPostedFileBase imageFile)
        {
            if (!ModelState.IsValid) { LoadCats(p.CategoryId); return View("Form", p); }
            Upload(p, imageFile);
            db.Products.Add(p); db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var p = db.Products.Find(id);
            if (p == null) return HttpNotFound();
            LoadCats(p.CategoryId);
            return View("Form", p);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Product p, HttpPostedFileBase imageFile)
        {
            if (!ModelState.IsValid) { LoadCats(p.CategoryId); return View("Form", p); }
            var e = db.Products.Find(p.ProductId);
            if (e == null) return HttpNotFound();
            e.ProductName = p.ProductName; e.CategoryId = p.CategoryId; e.Brand = p.Brand; e.Price = p.Price;
            e.Stock = p.Stock; e.ProductDescription = p.ProductDescription;
            if (!string.IsNullOrWhiteSpace(p.ImageUrl)) e.ImageUrl = p.ImageUrl;
            Upload(e, imageFile);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var p = db.Products.Find(id);
            if (p == null) return HttpNotFound();
            if (db.OrderDetails.Any(d => d.ProductId == id)) { TempData["Error"] = "Sản phẩm đã có trong đơn hàng, không thể xóa (hãy đặt tồn kho = 0)."; return RedirectToAction("Index"); }
            db.Products.Remove(p); db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void Upload(Product p, HttpPostedFileBase f)
        {
            if (f == null || f.ContentLength == 0) return;
            var ext = Path.GetExtension(f.FileName).ToLowerInvariant();
            if (new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg", ".gif" }.Contains(ext) == false) return;
            var name = System.Guid.NewGuid().ToString("N") + ext;
            f.SaveAs(Server.MapPath("~/Content/images/products/" + name));
            p.ImageUrl = "~/Content/images/products/" + name;
        }
    }
}
