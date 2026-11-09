using System.Linq;
using System.Web.Mvc;
using _25DH110740_MyStore.Models;
using _25DH110740_MyStore.Models.ViewModel;
using PagedList;

namespace _25DH110740_MyStore.Controllers
{
    public class HomeController : BaseController
    {
        
        public ActionResult Index(string searchTerm, int? page)
        {
            var q = db.Products.Include("Category").AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var t = searchTerm.Trim();
                q = q.Where(p => p.ProductName.Contains(t) || p.ProductDescription.Contains(t)
                                 || p.Category.CategoryName.Contains(t) || p.Brand.Contains(t));
            }
            var vm = new HomeProductVM
            {
                SearchTerm = searchTerm,
                FeaturedProducts = q.OrderByDescending(p => p.Sold).Take(10).ToList(),
                NewProducts = q.OrderByDescending(p => p.ProductId).Take(20).ToList().ToPagedList(page ?? 1, 6)
            };
            return View(vm);
        }

        
        public ActionResult ProductList(int? id, string sortOrder, string searchTerm, int? page)
        {
            var q = db.Products.Include("Category").AsQueryable();
            Category cat = null;
            if (id.HasValue)
            {
                cat = db.Categories.Find(id.Value);
                if (cat == null) return HttpNotFound();
                q = q.Where(p => p.CategoryId == id.Value);
            }
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var t = searchTerm.Trim();
                q = q.Where(p => p.ProductName.Contains(t) || p.Brand.Contains(t));
            }
            switch (sortOrder)
            {
                case "price_asc": q = q.OrderBy(p => p.Price); break;
                case "price_desc": q = q.OrderByDescending(p => p.Price); break;
                case "name": q = q.OrderBy(p => p.ProductName); break;
                case "bestseller": q = q.OrderByDescending(p => p.Sold); break;
                default: q = q.OrderByDescending(p => p.ProductId); break;
            }
            ViewBag.SearchTerm = searchTerm;
            return View(new ProductListVM { Category = cat, SortOrder = sortOrder, Products = q.ToPagedList(page ?? 1, 8) });
        }

       
        public ActionResult ProductDetail(int id, int? quantity, int? page)
        {
            var product = db.Products.Include("Category").FirstOrDefault(p => p.ProductId == id);
            if (product == null) return HttpNotFound();
            var vm = new ProductDetailVM
            {
                Product = product,
                Quantity = quantity.HasValue && quantity.Value > 0 ? quantity.Value : 1,
                RelatedProducts = db.Products.Where(p => p.CategoryId == product.CategoryId && p.ProductId != id)
                                    .OrderByDescending(p => p.ProductId).Take(8).ToList(),
                TopProducts = db.Products.Where(p => p.CategoryId == product.CategoryId)
                                    .OrderByDescending(p => p.Sold).Take(8).ToList().ToPagedList(page ?? 1, 3)
            };
            return View(vm);
        }
    }
}
